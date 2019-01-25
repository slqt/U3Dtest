using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour {

	public GameCtrl 		m_gameCtrl;			// ゲーム.
	public GameObject		m_sword;			// プレイヤーの剣.
	public GameObject		m_scoreBorad;		// スコア表示オブジェクト.
	private AudioChannels	m_audio;			// オーディオ.
	public AudioClip 		m_swordAttackSE;	// 攻撃SE.
	public GameObject 		SWORD_ATTACK_OBJ;	// 攻撃範囲オブジェクト.
	
	private bool 		m_equiped = false;  // ソードを装備中.
	private Transform 	m_target;  // 攻撃対象.
	
	// 得点.
	private const int POINT = 500;
	private const int COMBO_BONUS = 200;

	private int 	m_combo = 0;
	
	protected Animator	m_animator;

	// 初期化.
	void	Awake()
	{
		m_animator = GetComponent<Animator>();

		if(m_animator == null) {

			Debug.LogError("Can't find Animator component.");
		}
	}

	void 	Start()
	{
		m_equiped = false;
		m_sword.GetComponent<Renderer>().enabled = false;

		m_audio = FindObjectOfType(typeof(AudioChannels)) as AudioChannels;
		m_combo = 0;
	}
	
	// ステージ開始時.
	void OnStageStart()
	{
		m_equiped = false;
		m_sword.GetComponent<Renderer>().enabled = false;
	}
	
	// ソードを拾った.
	void OnGetSword()
	{
		if (!m_equiped) {
			m_sword.GetComponent<Renderer>().enabled = true;
			m_equiped = true;
			m_animator.SetTrigger("begin_idle_sword");
		} else {
			int point = POINT + COMBO_BONUS * m_combo;

			Hud.get().CreateScoreBoard(this.transform.position, point);
			Hud.get().AddScore(point);
			m_combo++;
		}
	}
	
	void Remove()  
	{
		m_sword.GetComponent<Renderer>().enabled = false;
		m_equiped = false;

		// レイヤーのアニメーションを同期させたい（左右の腕の振りがずれないようにしたい）
		// ので、ステートをリセットする（補間しない）.
		m_animator.Play("idle", 0);
		m_animator.Play("idle", 1);

		m_combo = 0;
	}

	
	// 自動攻撃する.
	public void AutoAttack(Transform other)
	{
		if (m_equiped) {
			m_target = other;
			StartCoroutine("SwordAutoAttack");
		}
	}
	
	// 攻撃可能か？.
	public bool CanAutoAttack()
	{
		if (m_equiped)
			return true;
		else
			return false;
	}
		
	
	IEnumerator SwordAutoAttack()
	{
		m_gameCtrl.OnAttack();

		// 振り向く.
		Vector3		target_pos = m_target.transform.position;
		target_pos.y = transform.position.y;
		transform.LookAt(target_pos);
		yield return null;

		// 攻撃.
		m_animator.SetTrigger("begin_attack");
		yield return new WaitForSeconds(0.3f);

		m_audio.PlayOneShot(m_swordAttackSE,1.0f,0.0f);		
		yield return new WaitForSeconds(0.2f);

		Vector3 projectilePos;
		projectilePos = transform.position + transform.forward * 0.5f;
		Instantiate(SWORD_ATTACK_OBJ,projectilePos,Quaternion.identity);
		yield return null;

		// 向きを元に戻す.
		Remove();
		m_gameCtrl.OnEndAttack();
	}
}
