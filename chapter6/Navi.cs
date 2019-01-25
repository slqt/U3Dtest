using UnityEngine;
using System.Collections;

public class Navi : MonoBehaviour {

	public GameObject	uiCanvas;

	public UnityEngine.UI.Text	uiScoreText;
	public UnityEngine.UI.Text	uiHiScoreText;

	public GameObject	uiMessageGameOver;
	public GameObject	uiMessageAccomplished;
	public GameObject 	uiMessageHiScore;

	public UnityEngine.UI.Image[] 	uiPlayerLeftImages;

	protected int	score = 0;
	protected int	hiScore = 0;

	protected LockSlot		lock_slot;					// ロックオンスロット表示.
	protected LockBonus		lock_bonus;					// ロックオンボーナス表示.
	protected PrintMessage	print_message;				// メッセージウインドウ.

	// ================================================================ //
	// MonoBehaviour からの継承.

	void	Awake()
	{
		this.lock_slot     = this.GetComponent<LockSlot>();
		this.lock_bonus    = this.GetComponent<LockBonus>();
		this.print_message = this.GetComponent<PrintMessage>();

	}

	void	Start()
	{
		// グローバルパラメーターからhi-scoreを取得.
		this.SetHiScore(GlobalParam.GetInstance().GetHiScore());
	}

	// ================================================================ //

	// ------------------------------------------------------------------------
	// SCOREに加算.
	// ------------------------------------------------------------------------
	public void AddScore( int score )
	{
		// 加算
		this.score += score;
		
		// 表示
		this.uiScoreText.text = this.score.ToString();
	}
	
	// ------------------------------------------------------------------------
	// SCOREを返す.
	// ------------------------------------------------------------------------
	public int GetScore()
	{
		return score;
	}

	// ------------------------------------------------------------------------
	// HISCOREの設定.
	// ------------------------------------------------------------------------
	public void SetHiScore( int hiScore )
	{
		// 保持
		this.hiScore = hiScore;
	
		// 表示
		uiHiScoreText.text = this.hiScore.ToString();
	}

	// ------------------------------------------------------------------------
	// HISCOREの設定.
	// ------------------------------------------------------------------------
	public int GetHiScore()
	{
		return this.hiScore;
	}

	// ================================================================ //

	// 『GAME OVER』の文字を表示する.
	public void ShowGameOver()
	{
		this.uiMessageGameOver.SetActive(true);
	}

	// ミッション成功（ボスを倒したとき）のメッセージを表示する.
	public void ShowMisssionAccomplished()
	{
		this.uiMessageAccomplished.SetActive(true);
	}

	// ------------------------------------------------------------------------
	// HISCOREかどうかの状態を保持.
	// ------------------------------------------------------------------------
	public void SetIsHiScore( bool isHiScore )
	{
		if ( isHiScore )
		{
			// HISCORE表示処理.
			StartCoroutine( WaitAndPrintHiScoreMessage( 0.5f ) );
		}
	}

	// ------------------------------------------------------------------------
	// HISCORE時のメッセージ表示.
	//  - 指定時間表示タイミングを遅らせる.
	// ------------------------------------------------------------------------
	IEnumerator WaitAndPrintHiScoreMessage( float waitForSeconds )
	{
		// 一定時待つ.
		yield return new WaitForSeconds( waitForSeconds );

		// HISCOREのメッセージを表示.
		this.uiMessageHiScore.SetActive(true);
	}

	// ================================================================ //

	// ロックオンスロットモジュールを取得する.
	public LockSlot		GetLockSlot()
	{
		return(this.lock_slot);
	}

	// ロックオンボーナスモジュールを取得する.
	public LockBonus	GetLockBonus()
	{
		return(this.lock_bonus);
	}

	// ================================================================ //

	// プレイヤー残機数をセットする.
	public void		SetPlayerLeftCount(int count)
	{
		for(int i = 0;i < this.uiPlayerLeftImages.Length;i++) {

			UnityEngine.UI.Image	left_image = this.uiPlayerLeftImages[i];

			if(i < count) {

				left_image.gameObject.SetActive(true);

			} else {

				left_image.gameObject.SetActive(false);
			}
		}
	}

	// ================================================================ //

	// メッセージウインドウを取得する.
	public PrintMessage	GetPrintMessage()
	{
		return(this.print_message);
	}

	// ================================================================ //
	//																	//
	// ================================================================ //

	protected static	Navi instance = null;

	public static Navi	get()
	{
		if(Navi.instance == null) {

			GameObject		go = GameObject.Find("GameCanvas");

			if(go != null) {

				Navi.instance = go.GetComponent<Navi>();

				if(Navi.instance == null) {
					Debug.LogError("[Navi] Component not attached.");
				}

			} else {
				Debug.LogError("[Navi] Can't find game object \"GameCanvas\".");
			}
		}

		return(Navi.instance);
	}

	// ================================================================ //
}
