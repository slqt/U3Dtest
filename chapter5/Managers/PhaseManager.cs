﻿using UnityEngine;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

//ゲームのフェーズの遷移を管理するクラス.
public class PhaseManager : MonoBehaviour {

	public string currentPhase{
		get{ return m_currentPhase; }
	}
	public GameObject[] guiList;

	public StartupMenuGUI		startupMenuGUI;
	public InstructionGUI		instructionGUI;
	public OnPlayGUI			onPlayGUI;
	public ShowResultGUI		showResultGUI;
	public DevelopmentModeGUI	developmentGUI;

	private MusicManager	m_musicManager;
	private ScoringManager	m_scoringManager;
	private string			m_currentPhase = "Startup";

	// ================================================================ //

	void	Start()
	{
		m_musicManager   = GameObject.Find("MusicManager").GetComponent<MusicManager>();
		m_scoringManager = GameObject.Find("ScoringManager").GetComponent<ScoringManager>();

		SetPhase("Startup");
	}
	
	void	Update()
	{

		switch (currentPhase) {

			case "Play":
			{
				if( m_musicManager.IsFinished() ){
					SetPhase("GameOver");
				}
				/*if(Input.GetKeyDown(KeyCode.E)) {
					SetPhase("GameOver");
				}*/
			}
			break;
		}
	}

	public void SetPhase(string nextPhase)
	{
		switch(nextPhase){

			// スタートメニュー.
			case "Startup":
			{
				DeactiveateAllGUI();
				this.startupMenuGUI.gameObject.SetActive(true);
			}
			break;

			// 説明.
			case "OnBeginInstruction":
			{
				DeactiveateAllGUI();
				this.instructionGUI.gameObject.SetActive(true);
				this.onPlayGUI.gameObject.SetActive(true);
			}
			break;

			//メインゲーム.
			case "Play":
			{
				DeactiveateAllGUI();
				this.onPlayGUI.gameObject.SetActive(true);

				//csvから曲データ読み込み.
				TextReader textReader
					= new StringReader(
						System.Text.Encoding.UTF8.GetString((Resources.Load("SongInfo/songInfoCSV") as TextAsset).bytes )
					);
				SongInfo songInfo = new SongInfo();
				SongInfoLoader loader=new SongInfoLoader();
				loader.songInfo=songInfo;
				loader.ReadCSV(textReader);
				m_musicManager.currentSongInfo = songInfo;
	
				foreach (GameObject audience in GameObject.FindGameObjectsWithTag("Audience"))
				{
					audience.GetComponent<SimpleActionMotor>().isWaveBegin = true;
				}
				//イベント(ステージ演出等)開始.
				GameObject.Find("EventManager").GetComponent<EventManager>().BeginEventSequence();
				//スコア評価開始
				m_scoringManager.BeginScoringSequence();

				//リズムシーケンス描画開始.
				this.onPlayGUI.BeginVisualization();
				this.onPlayGUI.isDevelopmentMode = false;
				//演奏開始.
				m_musicManager.PlayMusicFromStart();
			}
			break;

			case "DevelopmentMode":
			{
				DeactiveateAllGUI();
				this.developmentGUI.gameObject.SetActive(true);
				this.onPlayGUI.gameObject.SetActive(true);

				//csvから曲データ読み込み.
				TextReader textReader
					= new StringReader(
						System.Text.Encoding.UTF8.GetString((Resources.Load("SongInfo/songInfoCSV") as TextAsset).bytes )
					);
				SongInfo songInfo = new SongInfo();
				SongInfoLoader loader=new SongInfoLoader();
				loader.songInfo=songInfo;
				loader.ReadCSV(textReader);
				m_musicManager.currentSongInfo = songInfo;
	
				foreach (GameObject audience in GameObject.FindGameObjectsWithTag("Audience"))
				{
					audience.GetComponent<SimpleActionMotor>().isWaveBegin = true;
				}
				//イベント(ステージ演出等)開始.
				GameObject.Find("EventManager").GetComponent<EventManager>().BeginEventSequence();
				//スコア評価開始.
				m_scoringManager.BeginScoringSequence();

				//リズムシーケンス描画開始.
				this.onPlayGUI.BeginVisualization();
				this.onPlayGUI.isDevelopmentMode = true;

				//developモード専用GUIシーケンス描画開始.
				GameObject.Find("DevelopmentModeGUI").GetComponent<DevelopmentModeGUI>().BeginVisualization();
				//演奏開始
				m_musicManager.PlayMusicFromStart();
			}
			break;

			case "GameOver":
			{
				DeactiveateAllGUI();
				this.showResultGUI.gameObject.SetActive(true);

				//スコア依存のメッセージを表示.
				//Debug.Log( m_scoringManager.scoreRate );
				//Debug.Log(ScoringManager.failureScoreRate);

				ShowResultGUI.RESULT	result = ShowResultGUI.RESULT.GOOD;

				if(m_scoringManager.scoreRate <= ScoringManager.failureScoreRate) {

					result = ShowResultGUI.RESULT.BAD;
					GameObject.Find("Vocalist").GetComponent<BandMember>().BadFeedback();
					
				} else if(m_scoringManager.scoreRate >= ScoringManager.excellentScoreRate) {

					result = ShowResultGUI.RESULT.EXCELLENT;
					GameObject.Find("Vocalist").GetComponent<BandMember>().GoodFeedback();
					GameObject.Find("AudienceVoice").GetComponent<AudioSource>().Play();

				} else {

					result = ShowResultGUI.RESULT.GOOD;
					GameObject.Find("Vocalist").GetComponent<BandMember>().GoodFeedback();
				}

				this.showResultGUI.BeginVisualization(result);
			}
			break;

			case "Restart":
			{
				UnityEngine.SceneManagement.SceneManager.LoadScene("Main");
			}
			break;

			default:
			{
				Debug.LogError("unknown phase: " + nextPhase);
			}
			break;

		} // end of switch

		m_currentPhase = nextPhase;
	}

	private void DeactiveateAllGUI()
	{
		this.startupMenuGUI.gameObject.SetActive(false);
		this.instructionGUI.gameObject.SetActive(false);
		this.onPlayGUI.gameObject.SetActive(false);
		this.showResultGUI.gameObject.SetActive(false);
		this.developmentGUI.gameObject.SetActive(false);
	}
}
