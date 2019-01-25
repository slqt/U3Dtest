using UnityEngine;
using System.Collections;

public class TitleSceneControl : MonoBehaviour {

	// 進行状態.
	public enum STEP {

		NONE = -1,

		TITLE = 0,				// タイトル表示（ボタン押し待ち）.
		WAIT_SE_END,			// スタートのSEが終わるのを待ってる.
		FADE_WAIT,				// フェード終了待ち.

		NUM,
	};

	private STEP	step = STEP.NONE;
	private STEP	next_step = STEP.NONE;
	private float	step_timer = 0.0f;

	private FadeControl	fader = null;					// フェードコントロール	.
	
	public UnityEngine.UI.Image		uiImageStart;		// 『開始っ！』の UI.Image.	
	
	// 始めが押された時にアニメーションをする時間
	private const float	TITLE_ANIME_TIME = 0.1f;
	private const float	FADE_TIME = 1.0f;
	
	// -------------------------------------------------------------------------------- //

	void 	Start()
	{
		// プレイヤーを操作不能にする.
		PlayerControl	player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();
		player.UnPlayable();
		
		// フェードコントロール.
		this.fader = FadeControl.get();
		this.fader.fade( 1.0f, new Color( 0.0f, 0.0f, 0.0f, 1.0f ), new Color( 0.0f, 0.0f, 0.0f, 0.0f) );
		
		this.next_step = STEP.TITLE;
	}

	void 	Update()
	{
		this.step_timer += Time.deltaTime;

		// 次の状態に移るかどうかを、チェックする.
		switch(this.step) {

			case STEP.TITLE:
			{
				// マウスがクリックされた.
				//
				if(Input.GetMouseButtonDown(0)) {

					this.next_step = STEP.WAIT_SE_END;
				}
			}
			break;

			case STEP.WAIT_SE_END:
			{
				// SE の再生が終わったらフードアウト.
			
				bool	to_finish = true;

				do {

					if(!this.GetComponent<AudioSource>().isPlaying) {

						break;
					}

					if(this.GetComponent<AudioSource>().time >= this.GetComponent<AudioSource>().clip.length) {

						break;
					}

					to_finish = false;

				} while(false);

				if(to_finish) {

					this.fader.fade( FADE_TIME, new Color( 0.0f, 0.0f, 0.0f, 0.0f ), new Color( 0.0f, 0.0f, 0.0f, 1.0f) );
				
					this.next_step = STEP.FADE_WAIT;
				}
			}
			break;
			
			case STEP.FADE_WAIT:
			{
				// フェードが終了したら、ゲームシーンをロードして終了.
				if(!this.fader.isActive()) 
				{
					UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
				}
			}
			
			break;
		}

		// 状態がかわったときの初期化処理.

		if(this.next_step != STEP.NONE) {

			switch(this.next_step) {

				case STEP.WAIT_SE_END:
				{
					// 開始のSEを鳴らす.
					this.GetComponent<AudioSource>().Play();
				}
				break;
			}

			this.step = this.next_step;
			this.next_step = STEP.NONE;

			this.step_timer = 0.0f;
		}

		// 各状態での実行処理.

		switch(this.step) {

			case STEP.WAIT_SE_END:
			{
				float	scale	= 1.0f;
				
				float	rate = this.step_timer/TITLE_ANIME_TIME;
					
				scale = Mathf.Lerp(2.0f, 1.0f, rate);

				this.uiImageStart.GetComponent<RectTransform>().localScale = Vector3.one*scale;
			}
			break;
		}

	}
}
