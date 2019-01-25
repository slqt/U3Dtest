using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//ゲームプレイ中に表示するGUIの挙動.
public class OnPlayGUI : MonoBehaviour
{
	public static float markerEnterOffset = 2.5f;	// マーカーの表示を開始するタイミング(何ビート後のアクションが出現するか).
	public static float markerLeaveOffset =-1.0f;	// マーカーの表示を終了するタイミング(何ビート後のアクションが出現するか).

	public static int 	messageShowFrameDuration = 40;


	public bool 		isDevelopmentMode = false;
	protected Vector2 	markerOrigin = new Vector2(-140.0f, -60.0f);		// ビートサークルの位置.

	public GameObject	uiCanvas;
	public GameObject	markerPrefab = null;

	public UnityEngine.UI.Text		uiScoreText;				// "Score : " + スコアーのテキスト.
	public UnityEngine.UI.RawImage	uiTemperBarRawImage;		// テンションゲージの、伸び縮みするバー.
	public UnityEngine.UI.Text		uiTemperText;				// "Temper" のテキスト.
	public UnityEngine.UI.Image		uiExcellentImage;			// 入力結果のイメージ　Excellent.
	public UnityEngine.UI.Image		uiGoodImage;				// 入力結果のイメージ　Good.
	public UnityEngine.UI.Image		uiMissImage;				// 入力結果のイメージ　Miss.
	public UnityEngine.UI.Image		uiHitImage;					// 入力成功のときのエフェクト.
	
	// ---------------------------------------------------------------- //

	protected UnityEngine.UI.Image 		ui_message_image;		// 表示中の入力結果イメージ.

	protected const int		MARKER_POOL_COUNT = 16;		// マーカーの最大表示個数.
	protected const float	HIT_EFFECT_ZOOM_DURATION = 10.0f/60.0f;
	protected const float	HIT_EFFECT_DISP_DURATION = 15.0f/60.0f;

	protected List<Marker>	m_markers = new List<Marker>();

	protected float		m_pixelsPerBeats          = Screen.width * 1.0f/markerEnterOffset;
	protected int		m_messageShowCountDown    = 0;
	protected float		m_hit_effect_timer        = -1.0f;
	protected float		m_lastInputScore          = 0;


	// 時間的に進んでいるシークユニット（表示終了位置）.
	protected SequenceSeeker<OnBeatActionInfo>	m_seekerFront = new SequenceSeeker<OnBeatActionInfo>();

	// 時間的に遅れているシークユニット（表示開始位置）.
	protected SequenceSeeker<OnBeatActionInfo>	m_seekerBack = new SequenceSeeker<OnBeatActionInfo>();

	protected MusicManager		m_musicManager;
	protected ScoringManager	m_scoringManager;
	protected GameObject		m_playerAvator;

	// ================================================================ //

	void	Awake()
	{
		m_markers.Clear();

		for(int i = 0;i < MARKER_POOL_COUNT;i++) {

			GameObject	marker_go = GameObject.Instantiate(this.markerPrefab);
			Marker		marker    = marker_go.GetComponent<Marker>();

			marker_go.GetComponent<RectTransform>().SetParent(this.uiCanvas.GetComponent<RectTransform>());
			marker.setVisible(false);

			m_markers.Add(marker);
		}
	}

	void Start()
	{
		m_musicManager   = GameObject.Find("MusicManager").GetComponent<MusicManager>();
		m_scoringManager = GameObject.Find("ScoringManager").GetComponent<ScoringManager>();
		m_playerAvator   = GameObject.Find("PlayerAvator");

	}
	void	Update()
	{
		if(m_musicManager.IsPlaying()) {

			m_seekerBack.ProceedTime( m_musicManager.beatCountFromStart - m_musicManager.previousBeatCountFromStart);
			m_seekerFront.ProceedTime(m_musicManager.beatCountFromStart - m_musicManager.previousBeatCountFromStart);
		}

		// マーカーの表示.
		this.draw_markers();

		// スコアーの表示.
		this.uiScoreText.text = "Score: " + m_scoringManager.score;

		// 盛り上がりゲージ表示.
		this.draw_temper_guage();

		if(m_musicManager.IsPlaying()) {

			// メッセージ（Excellent/Good/Miss）を表示.
			this.draw_message();

			// アクションタイミングのヒットエフェクト.
			this.draw_hit_effect();
		}
	}

	// ================================================================ //

	public void BeginVisualization()
	{
		m_musicManager   = GameObject.Find("MusicManager").GetComponent<MusicManager>();
		m_scoringManager = GameObject.Find("ScoringManager").GetComponent<ScoringManager>();

		m_seekerBack.SetSequence(m_musicManager.currentSongInfo.onBeatActionSequence);
		m_seekerFront.SetSequence(m_musicManager.currentSongInfo.onBeatActionSequence);
		m_seekerBack.Seek(markerLeaveOffset);
		m_seekerFront.Seek(markerEnterOffset);
	}

    public void RythmHitEffect(int actionInfoIndex, float score)
    {
		m_lastInputScore          = score;
		m_hit_effect_timer = 0.0f;
		m_messageShowCountDown    = messageShowFrameDuration;

		AudioClip	clip;

		if(score < 0) {

			clip = m_playerAvator.GetComponent<PlayerAction>().headBangingSoundClip_BAD;
			ui_message_image = uiMissImage;

		} else if(score <= ScoringManager.goodScore) {

			clip = m_playerAvator.GetComponent<PlayerAction>().headBangingSoundClip_GOOD;
			ui_message_image = uiGoodImage;

		} else{

			clip = m_playerAvator.GetComponent<PlayerAction>().headBangingSoundClip_GOOD;
			ui_message_image = uiExcellentImage;
		}

		m_playerAvator.GetComponent<AudioSource>().clip = clip;
		m_playerAvator.GetComponent<AudioSource>().Play();
    }

	public void Seek(float beatCount)
	{
		m_seekerBack.Seek(beatCount + markerLeaveOffset);
		m_seekerFront.Seek(beatCount + markerEnterOffset);
	}

	// ================================================================ //

	// マーカーをぜんぶ表示する.
	private void	draw_markers()
	{
		foreach(var marker in m_markers) {

			marker.setVisible(false);
			marker.hideLineNumberText();
		}

		if(m_musicManager.IsPlaying()) {

			SongInfo	song =  m_musicManager.currentSongInfo;

			// 表示を開始するマーカー（遅れているシークユニットのシーク位置）.
			int		begin = m_seekerBack.nextIndex;
			// 表示を終了するマーカー（進んでいるシークユニットのシーク位置）.
			int		end   = m_seekerFront.nextIndex;

			float	x_offset;
			int		marker_draw_index = 0;

			// アクションタイミングを示すアイコンを描画.
			for(int drawnIndex = begin;drawnIndex < end;drawnIndex++) {

				float 	size = ScoringManager.timingErrorToleranceGood * m_pixelsPerBeats;

				OnBeatActionInfo	info = song.onBeatActionSequence[drawnIndex];

				// テンションが高いとき、ジャンプアクションのマーカーを大きくする.
				if(m_scoringManager.temper > ScoringManager.temperThreshold && info.playerActionType == PlayerActionEnum.Jump) {
					size *= 1.5f;
				}

				// ビートサークルからマーカーまでの X 座標のオフセットを求める.
				x_offset = info.triggerBeatTiming - m_musicManager.beatCount;
				x_offset *= m_pixelsPerBeats;

				float	pos_x = markerOrigin.x + x_offset;
				float	pos_y = markerOrigin.y;

				m_markers[marker_draw_index].draw(pos_x, pos_y, size);

				// 開発モードのときは、テキストファイル中の行番号を表示する.
				if(isDevelopmentMode) {

					m_markers[marker_draw_index].dispLineNumberText(info.line_number);
				}

				marker_draw_index++;
			}
		}
	}

	// 盛り上がりゲージ表示.
	protected void	draw_temper_guage()
	{
		float	temper = m_scoringManager.temper;

		this.uiTemperBarRawImage.GetComponent<RectTransform>().localScale = new Vector3(temper, 1.0f, 1.0f);
		this.uiTemperBarRawImage.uvRect = new Rect(0.0f, 0.0f, temper, 1.0f);

		// ハイテンションのときの明滅色.

		Color	blink_color = Color.white;

		if(m_scoringManager.temper > ScoringManager.temperThreshold) {

			 int	frame_rate = Application.targetFrameRate;

			float	c = 0.7f + 0.3f*Mathf.Abs(Time.frameCount%frame_rate - frame_rate/2)/(float)frame_rate;

			blink_color.g = c;
			blink_color.b = c;
		}

		this.uiTemperBarRawImage.color = blink_color;
		this.uiTemperText.color = blink_color;
	}

	// メッセージ（Excellent/Good/Miss）を表示.
	protected void	draw_message()
	{
		this.uiExcellentImage.gameObject.SetActive(false);
		this.uiGoodImage.gameObject.SetActive(false);
		this.uiMissImage.gameObject.SetActive(false);

		if(m_messageShowCountDown > 0) {

			float	alpha = 1.0f;

			if(m_messageShowCountDown > 20.0f) {

				alpha = 1.0f;

			} else {

				alpha = Mathf.InverseLerp(0.0f, 20.0f, m_messageShowCountDown);
			}


			this.ui_message_image.gameObject.SetActive(true);
			this.ui_message_image.color = new Color(1.0f, 1.0f, 1.0f, alpha);
			m_messageShowCountDown--;
		}
	}

	// アクションタイミングのヒットエフェクト.
	protected void	draw_hit_effect()
	{

		if(m_hit_effect_timer >= 0.0f) {

			if(m_hit_effect_timer > HIT_EFFECT_DISP_DURATION) {

				m_hit_effect_timer = -1.0f;
			}
		}

		if(m_hit_effect_timer >= 0.0f) {

			float	rate = Mathf.Clamp01(m_hit_effect_timer/HIT_EFFECT_ZOOM_DURATION);

			rate = Mathf.Pow(rate, 0.5f);

			float	scale = Mathf.Lerp(0.5f, 2.0f, rate);

			if(m_lastInputScore >= ScoringManager.excellentScore) {

				scale *= 2.0f;

			} else if(m_lastInputScore > ScoringManager.missScore) {

				scale *= 1.2f;

			} else {

				scale *= 0.5f;
			}

			this.uiHitImage.gameObject.SetActive(true);
			this.uiHitImage.GetComponent<RectTransform>().localScale = Vector3.one*scale;

			m_hit_effect_timer += Time.deltaTime;

		} else {

			this.uiHitImage.gameObject.SetActive(false);
		}
	}


}
