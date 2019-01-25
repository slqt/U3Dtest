
using UnityEngine;


/// <summary>タイトル画面からのゲーム開始用クラス</summary>
public class TitleControl : MonoBehaviour
{
	//==============================================================================================
	// 内部データ型.

	/// <summary>遷移状態</summary>
	private enum STEP
	{
		NONE = -1,
		SELECT = 0,   // 選択中.
		PLAY_JINGLE,  // ジングル再生中.
		START_GAME,   // ゲーム開始.
		NUM
	}

	/// <summary>ゲームの章</summary>
	private enum CHAPTER
	{
		NONE = -1,
		PROLOGUE = 0,
		C1,
		C2,
		C3_0,
		C3_1,
		C4,
		C5,
		EPILOGUE,
		NUM
	}


	//==============================================================================================
	// MonoBehaviour 関連のメンバ変数・メソッド.

	/// <summary>タイトル画面のテクスチャ</summary>
	public Texture2D m_titleTexture = null;

	/// <summary>ジングル音のオーディオクリップ</summary>
	public AudioClip m_startSound = null;

	/// <summary>スタートアップメソッド</summary>
	private void Start()
	{
		m_chapterNames = new string[ ( int ) CHAPTER.NUM ];

		m_chapterNames[ ( int ) CHAPTER.PROLOGUE ] = "プロローグ";
		m_chapterNames[ ( int ) CHAPTER.C1 ]       = "第一章";
		m_chapterNames[ ( int ) CHAPTER.C2 ]       = "第二章";
		m_chapterNames[ ( int ) CHAPTER.C3_0 ]     = "第三章　前半";
		m_chapterNames[ ( int ) CHAPTER.C3_1 ]     = "第三章　後半";
		m_chapterNames[ ( int ) CHAPTER.C4 ]       = "第四章";
		m_chapterNames[ ( int ) CHAPTER.C5 ]       = "第五章";
		m_chapterNames[ ( int ) CHAPTER.EPILOGUE ] = "エピローグ";

		//

		m_textManager = TextManager.get();

		m_textManager.showTitle();
#if UNITY_EDITOR
		m_textManager.createButtons(m_chapterNames, Color.black, new Color(1.0f, 1.0f, 1.0f, 0.5f));
#endif
	}

	/// <summary>フレーム毎更新メソッド</summary>
	private void Update()
	{
		// ステップ内の遷移チェック.
		if ( m_nextStep == STEP.NONE )
		{
			switch ( m_step )
			{
			case STEP.NONE:
				m_nextStep = STEP.SELECT;
				break;

			case STEP.SELECT:
#if UNITY_EDITOR
				do {

					if(m_textManager.selected_button == "") {
						break;
					}

					int		selected_index = System.Array.IndexOf(m_chapterNames, m_textManager.selected_button);

					if(selected_index < 0) {
						break;
					}

					m_selectedChapter = selected_index;
					m_nextStep        = STEP.PLAY_JINGLE;

				} while(false);
#else
				if ( Input.GetMouseButtonDown( 0 ) )
				{
					m_nextStep = STEP.PLAY_JINGLE;
				}
#endif //!UNITY_EDITOR
				break;

			case STEP.PLAY_JINGLE:
				if ( !GetComponent<AudioSource>().isPlaying )
				{
					m_nextStep = STEP.START_GAME;
				}
				break;
			}
		}

		// 状態が遷移したときの初期化.
		while ( m_nextStep != STEP.NONE )
		{
			m_step = m_nextStep;
			m_nextStep = STEP.NONE;

			switch ( m_step )
			{
			case STEP.PLAY_JINGLE:
				// ジングル音再生.
				GetComponent<AudioSource>().clip = m_startSound;
				GetComponent<AudioSource>().Play();
				break;

			case STEP.START_GAME:
#if !UNITY_EDITOR
				// プロローグから開始.
				GlobalParam.getInstance().setStartScriptFiles("c00_main", "c00_sub");
#else
				// 選択によって読み込むファイルを変える.
				switch ( m_selectedChapter )
				{
				case ( int ) CHAPTER.PROLOGUE:
					GlobalParam.getInstance().setStartScriptFiles("c00_main", "c00_sub");
					break;

				case ( int ) CHAPTER.C1:
					GlobalParam.getInstance().setStartScriptFiles("c01_main", "c01_sub");
					break;

				case ( int ) CHAPTER.C2:
					GlobalParam.getInstance().setStartScriptFiles("c02_main", "c02_sub");
					break;

				case ( int ) CHAPTER.C3_0:
					GlobalParam.getInstance().setStartScriptFiles("c03_0_main", "c03_0_sub");
					break;

				case ( int ) CHAPTER.C3_1:
					GlobalParam.getInstance().setStartScriptFiles("c03_1_main", "c03_1_sub");
					break;

				case ( int ) CHAPTER.C4:
					GlobalParam.getInstance().setStartScriptFiles("c04_main", "c04_sub");
					break;

				case ( int ) CHAPTER.C5:
					GlobalParam.getInstance().setStartScriptFiles("c05_main", "c05_sub");
					break;

				case ( int ) CHAPTER.EPILOGUE:
					GlobalParam.getInstance().setStartScriptFiles("c90_main", "c90_sub");
					break;
				}
#endif //!UNITY_EDITOR

				// ゲームシーンをロード.
				UnityEngine.SceneManagement.SceneManager.LoadScene( "GameScene" );

				break;
			}
		}
	}

	//==============================================================================================
	// 非公開メンバ変数.

	/// <summary>現在の状態</summary>
	private STEP m_step = STEP.NONE;

	/// <summary>次に遷移する状態</summary>
	private STEP m_nextStep = STEP.NONE;

	/// <summary>各章の名前</summary>
	private string[] m_chapterNames;

	private TextManager	m_textManager;

#if UNITY_EDITOR
	/// <summary>デバッグモードで選択した章.
	private int m_selectedChapter = 0;
#endif //UNITY_EDITOR
}
