using UnityEngine;
using System.Collections;

public class TitleSceneControl : MonoBehaviour {

	// 進行状態.
	public enum STEP {

		NONE = -1,

		TITLE = 0,				// タイトル表示（ボタン押し待ち）.
		WAIT_SE_END,			// スタートのSEが終わるのを待ってる.

		NUM,
	};

	private STEP	step = STEP.NONE;
	private STEP	next_step = STEP.NONE;
	private float	step_timer = 0.0f;

	// ================================================================ //
	// MonoBehaviour からの継承.

	void	Start()
	{
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
				// SE の再生が終わったら、ゲームシーンをロードして終了.

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

		/*switch(this.step) {

			case STEP.TITLE:
			{
			}
			break;
		}*/

	}
}
