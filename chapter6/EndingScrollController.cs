using UnityEngine;
using System.Collections;

// ----------------------------------------------------------------------------
// 対象とするゲームオブジェクトを画面外の下方向から上へスクロール.
// ----------------------------------------------------------------------------
public class EndingScrollController : MonoBehaviour {

	public GameObject	uiStaffRoll;
	public FadeOut		fadeOut;

	public float		scrollSpeed				= 32.0f;		// スクロールするスピード.
	public float		startPosition			= -1850;		// スクロール開始位置.
	public float 		distanceToStartEaseIn   = 0.05f;		// イーズインを開始する停止位置からの距離.
	public bool 		isStoppedStarScroll		= true;
	
	private float		stopPositionY;							// スクロール停止位置.

	private OpeningSpaceController	endingSpace;

	private bool		isEaseIn = false;

	
	void Start () 
	{	
		// スクロール停止位置を取得.
		this.stopPositionY = this.uiStaffRoll.GetComponent<RectTransform>().localPosition.y;
		
		// メッセージを初期表示位置に移動.
		Vector3		tmpPosition = this.uiStaffRoll.GetComponent<RectTransform>().localPosition;
		this.uiStaffRoll.GetComponent<RectTransform>().localPosition = new Vector3(tmpPosition.x, this.startPosition, tmpPosition.z);
	
		this.endingSpace = GameObject.Find("EndingSpace").GetComponent<OpeningSpaceController>();
	}
	
	void FixedUpdate () 
	{
		// 停止位置までスクロール.

		Vector3		position = this.uiStaffRoll.GetComponent<RectTransform>().localPosition;
		
		if(this.isEaseIn) {

			// イーズイン.
			position.y += (Mathf.Abs(this.stopPositionY - position.y)/this.distanceToStartEaseIn )*this.scrollSpeed*Time.deltaTime;

			this.uiStaffRoll.GetComponent<RectTransform>().localPosition = position;

		} else {

			if(Mathf.Abs(this.stopPositionY - position.y ) < this.distanceToStartEaseIn) {

				// イーズイン開始.
				this.isEaseIn = true;
				if(this.isStoppedStarScroll) {

					this.endingSpace.SetEaseIn();
					this.fadeOut.SetEnable();
				}

			} else {

				// スクロール.
				position.y += this.scrollSpeed*Time.deltaTime;
				this.uiStaffRoll.GetComponent<RectTransform>().localPosition = position;
			}

		}
	}
}
