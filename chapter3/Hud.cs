using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Hud : MonoBehaviour {

	protected const int	PLAYER_IMAGE_WIDTH = 32;

	public GameObject			m_uiRemainImagePrefab;		// 残り人数表示用.
	public GameObject			m_uiScoreBoardPrefab;		// 

	public GameObject			m_uiCanvas;
	public UnityEngine.UI.Text	m_uiDemoText;
	public UnityEngine.UI.Image	m_uiReadyImage;
	public UnityEngine.UI.Image	m_uiGameOverImage;
	public UnityEngine.UI.Image	m_uiStageClearImage;

	public UnityEngine.UI.Text	m_uiScoreText;
	public UnityEngine.UI.Text	m_uiStageText;

	protected int m_score;

	// ---------------------------------------------------------------- //

	protected GameCtrl m_gameCtrl;

	protected List<UnityEngine.UI.Image>	m_remainImages = new List<UnityEngine.UI.Image>();
	
	// ================================================================ //
	// MonoBehaviour からの継承.

	void 	Start()
	{
		m_gameCtrl = GameObject.Find("GameCtrl").GetComponent<GameCtrl>();
		OnGameStart();
		
		GlobalParam param = GlobalParam.GetInstance();
		if (param != null) {
			if (param.IsAdvertiseMode()) {
				m_uiDemoText.gameObject.SetActive(true);
			} else {
				m_uiDemoText.gameObject.SetActive(false);
			}
		}		
	}

	void	Update()
	{
		// 残機表示.

		int remain = m_gameCtrl.GetRetryRemain();

		if(m_remainImages.Count < remain) {

			for(int i = m_remainImages.Count;i < remain;i++) {

				UnityEngine.UI.Image	remain_image = this.create_remain_image();

				float	x = -Screen.width/2.0f + PLAYER_IMAGE_WIDTH + i*PLAYER_IMAGE_WIDTH;
				float	y =  Screen.height/2.0f - 48.0f;

				remain_image.GetComponent<RectTransform>().localPosition = new Vector3(x, y, 0.0f);

				m_remainImages.Add(remain_image);
			}

		} else if(m_remainImages.Count > remain) {

			for(int i = m_remainImages.Count - 1;i > remain - 1;i--) {

				GameObject.Destroy(m_remainImages[i].gameObject);
			}

			m_remainImages.RemoveRange(remain, m_remainImages.Count - remain);
		}
	}

	public void OnGameStart()
	{
		this.SetScore(0);
	}

	// 『Game Over』を表示/非表示する.	
	public void DrawGameOver(bool visible)
	{
		m_uiGameOverImage.gameObject.SetActive(visible);
	}
	
	// 『Stage Clear』を表示/非表示する.	
	public void DrawStageClear(bool visible)
	{
		m_uiStageClearImage.gameObject.SetActive(visible);
	}
	
	// 『Ready』を表示/非表示する.	
	public void DrawStageStart(bool visible)
	{
		m_uiReadyImage.gameObject.SetActive(visible);
	}
	
	// ステージ番号をセットする.
	public void	SetStageNumber(int number)
	{
		m_uiStageText.text = "Stage " + number;
	}

	// スコアーをセットする.
	public void	SetScore(int score)
	{
		m_score = score;
		m_uiScoreText.text = "SCORE: " + m_score;
	}

	// スコアーを加算する.
	public void AddScore(int point)
	{
		this.SetScore(m_score + point);
	}

	// 宝箱をひろったとき等のスコアー表示を作る.
	public void	CreateScoreBoard(Vector3 position, int score)
	{
		ScoreBoard	board = GameObject.Instantiate(this.m_uiScoreBoardPrefab).GetComponent<ScoreBoard>();

		board.GetComponent<RectTransform>().SetParent(this.m_uiCanvas.GetComponent<RectTransform>());
		board.SetScore(score);
		board.SetPosition(position);
	}

	// ================================================================ //

	// 残機アイコンを作る.
	protected UnityEngine.UI.Image	create_remain_image()
	{
		UnityEngine.UI.Image	remain_image = GameObject.Instantiate(this.m_uiRemainImagePrefab).GetComponent<UnityEngine.UI.Image>();

		remain_image.GetComponent<RectTransform>().SetParent(this.m_uiCanvas.GetComponent<RectTransform>());

		return(remain_image);
	}
	
	// ================================================================ //
	// インスタンス.

	protected static Hud	instance = null;

	public static Hud	get()
	{
		if(Hud.instance == null) {

			GameObject		go = GameObject.Find("GameCanvas");

			if(go != null) {

				Hud.instance = go.GetComponent<Hud>();

			} else {

				Debug.LogError("Can't find game object \"Hud\".");
			}
		}

		return(Hud.instance);
	}


}
