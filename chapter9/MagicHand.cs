using UnityEngine;
using System.Collections;

public class MagicHand : MonoBehaviour {

	public GameObject		model;

	public DraggableObject	target;

	protected Animator		animator;

	public Vector3 		m_magicHandOffset = Vector3.zero;

	protected RaycastHit	raycast_hit;

	// -------------------------------------------------------- //
		
	public enum STEP {
		
		NONE = -1,

		HIDE = 0,
		PICKING,
		RELEASE,

		NUM,
	};
	public Step<STEP>			step = new Step<STEP>(STEP.NONE);

	// ================================================================ //
	// MonoBehaviour からの継承.

	void	Awake()
	{
		this.animator = this.GetComponentInChildren<Animator>();

		this.model.SetActive(false);
	}

	void	Start()
	{
		this.step.set_next(STEP.HIDE);
	}
	
	void	Update()
	{
		float	delta_time = Time.deltaTime;

		// ---------------------------------------------------------------- //
		// 次の状態に移るかどうかを、チェックする.

		switch(this.step.do_transition()) {

			case STEP.HIDE:
			{
				do {

					// イベント中は持ち上げられない.
					if(EventManager.get().isExecutingEvents()) {
						break;
					}

					if(!Input.GetMouseButton(0)) {
						break;
					}

					// マウス位置にレイを飛ばしてオブジェクトを検出する.
					Ray		ray = Camera.main.ScreenPointToRay(Input.mousePosition);

					if(!Physics.Raycast(ray, out this.raycast_hit, float.PositiveInfinity, 1 << LayerMask.NameToLayer("Draggable"))) {
						break;
					}

					if(this.raycast_hit.rigidbody == null) {
						break;
					}

					// DraggableObject コンポーネントを取得.
					this.target = this.raycast_hit.rigidbody.gameObject.GetComponent<DraggableObject>();

					if(this.target == null) {
						break;
					}

					this.step.set_next(STEP.PICKING);

				} while(false);
			}
			break;

			case STEP.PICKING:
			{
				if(!Input.GetMouseButton(0)) {
					this.step.set_next(STEP.RELEASE);
				}
			}
			break;

			case STEP.RELEASE:
			{
				if(this.animator.GetCurrentAnimatorStateInfo(0).IsName("release")) {
					this.step.set_next(STEP.HIDE);
				}
			}
			break;
		}

		// ---------------------------------------------------------------- //
		// 状態が遷移したときの初期化.

		while(this.step.get_next() != STEP.NONE) {

			switch(this.step.do_initialize()) {
	
				case STEP.HIDE:
				{
					this.model.SetActive(false);
				}
				break;

				case STEP.PICKING:
				{
					this.model.SetActive(true);

					// マジックハンドを表示・該当オブジェクトへ移動・持ち上げアニメーション再生.
					Vector3		position = m_magicHandOffset + this.target.transform.position + this.target.getYTop()*Vector3.up;

					this.transform.position = position;
					this.animator.SetTrigger("pick");

					// ドラッグ開始音を鳴らす.
					this.GetComponent<AudioSource>().Play();

					// ドラッグ開始を通知.
					this.target.onDragBegin(this.raycast_hit);
				}
				break;

				case STEP.RELEASE:
				{
					this.animator.SetTrigger("to_release");

					// ドラッグ終了を通知.
					this.target.onDragEnd();
				}
				break;
			}
		}

		// ---------------------------------------------------------------- //
		// 各状態での実行処理.

		switch(this.step.do_execution(delta_time)) {

			case STEP.PICKING:
			{
				// マジックハンド移動.
				Vector3		position = m_magicHandOffset + this.target.transform.position + this.target.getYTop()*Vector3.up;

				this.transform.position = position;

				this.target.onDragUpdate();
			}
			break;
		}
	}
}
