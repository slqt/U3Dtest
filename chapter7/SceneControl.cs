using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SceneControl : MonoBehaviour {


	public GameObject	StackBlockPrefab = null;


	public PlayerControl	player_control = null;

	public StackBlockControl	stack_control = null;
	public BGControl			bg_control = null;
	public GoalSceneControl		goal_scene = null;
	public VanishEffectControl	vanish_fx_control = null;

	public float	slider_value = 0.5f;

	// 各色のマテリアル（Blockl.cs）.
	//
	// ・実体を一個にしたい.
	// ・Inspector で変更できるようにしたい
	//
	// ので、インスタンスがひとつしか作られない、SceneControl に持たせています.
	//
	public Material[]	block_materials;


	// ---------------------------------------------------------------- //

	public int		height_level = 0;

	public static int	MAX_HEIGHT_LEVEL = 50;

	public int			player_stock;				// プレイヤーの残り.

	// ---------------------------------------------------------------- //

	public enum STEP {

		NONE = -1,

		PLAY = 0,			// ゲーム中.
		GOAL_ACT,			// ゴール演出.
		MISS,				// ミス演出.

		GAMEOVER,			// ゲームオーバー

		NUM,
	};

	public STEP			step;
	public STEP			next_step = STEP.NONE;
	public float		step_timer = 0.0f;


	// ---------------------------------------------------------------- //

	public enum SE {

		NONE = -1,

		DROP = 0,			// ブロックをドロップしたとき.
		DROP_CONNECT,		// ブロックが消えるとき（同じ色のブロックが４つ並んだとき）.
		LANDING,			// 上から降ってきたブロックが着地したとき.
		SWAP,				// 上下のブロックが回転して入れ替わるとき.
		EATING,				// ケーキを食べるとき.
		JUMP,				// 上から降ってきたブロックが着地したとき.
		COMBO,				// 連鎖したとき.

		CLEAR,				// クリアー.
		MISS,				// ミス.

		NUM,
	};

	public AudioClip[]	audio_clips;

	public AudioSource[]	audios;

	// ---------------------------------------------------------------- //

	public void	playSe(SE se)
	{
		if(se == SE.SWAP) {

			this.audios[1].clip = this.audio_clips[(int)se];
			this.audios[1].Play();

		} else {

			this.audios[0].PlayOneShot(this.audio_clips[(int)se]);
		}
	}

	void	Awake()
	{
		this.player_stock = 3;
	}

	void	Start()
	{

		//

		Block.materials = this.block_materials;

		this.stack_control = new StackBlockControl();

		this.stack_control.StackBlockPrefab = this.StackBlockPrefab;
		this.stack_control.scene_control = this;
		this.stack_control.create();

		this.vanish_fx_control = GameObject.FindGameObjectWithTag("VanishEffectControl").GetComponent<VanishEffectControl>();

		//

		this.player_control = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();
		this.player_control.scene_control = this;

		this.bg_control = GameObject.FindGameObjectWithTag("BG").GetComponent<BGControl>();

		this.goal_scene = new GoalSceneControl();
		this.goal_scene.scene_control = this;
		this.goal_scene.create();

		//

		this.audios = this.GetComponents<AudioSource>();

		//

		this.slider_value = Mathf.InverseLerp(RotateAction.ROTATE_TIME_SWAP_MIN, RotateAction.ROTATE_TIME_SWAP_MAX, RotateAction.rotate_time_swap);

		this.height_level = 0;

		this.bg_control.setHeightRateDirect((float)this.height_level/(float)MAX_HEIGHT_LEVEL);

	}
	
	void	Update()
	{
		this.step_timer += Time.deltaTime;

	#if false
		if(Input.GetKeyDown(KeyCode.G)) {

			this.next_step = STEP.GOAL_ACT;
		}
		if(Input.GetKeyDown(KeyCode.W)) {

			this.height_level = MAX_HEIGHT_LEVEL - 1;
	
			this.bg_control.setHeightRateDirect((float)this.height_level/(float)MAX_HEIGHT_LEVEL);
		}
	#endif

		// -------------------------------------------------------- //
		// 次の状態に移るかどうかを、チェックする.

		switch(this.step) {

			case STEP.PLAY:
			{
				do {

					if(this.player_control.isHungry()) {

						this.player_stock--;
						this.player_stock = Mathf.Max(0, this.player_stock);

						GUIControl.get().setStockCount(this.player_stock);

						this.next_step = STEP.MISS;

						break;
					}
	
					if(this.height_level >= MAX_HEIGHT_LEVEL) {
	
						this.next_step = STEP.GOAL_ACT;
						break;
					}

				} while(false);
			}
			break;

			case STEP.MISS:
			{
				if(this.step_timer > 1.0f) {

					if(	this.player_stock == 0) {

						this.next_step = STEP.GAMEOVER;

					} else {

						this.player_control.revive();
						this.next_step = STEP.PLAY;
					}
				}
			}
			break;

			case STEP.GOAL_ACT:
			case STEP.GAMEOVER:
			{
				// マウスがクリックされた
				//
				if(Input.GetMouseButtonDown(0)) {
		
					UnityEngine.SceneManagement.SceneManager.LoadScene("TitleScene");
				}
			}
			break;
		}

		// -------------------------------------------------------- //
		// 状態が遷移したときの初期化.

		if(this.next_step != STEP.NONE) {

			switch(this.next_step) {
	
				case STEP.MISS:
				{
					this.playSe(SE.MISS);
				}
				break;

				case STEP.GAMEOVER:
				{
					GUIControl.get().setDispGameOver(true);
				}
				break;

				case STEP.GOAL_ACT:
				{
					this.goal_scene.start();
				}
				break;
			}

			this.step      = this.next_step;
			this.next_step = STEP.NONE;

			this.step_timer = 0.0f;
		}

		// -------------------------------------------------------- //
		// 各状態での実行処理.	

		switch(this.step) {

			case STEP.GOAL_ACT:
			{
				this.goal_scene.execute();
			}
			break;
		}

		// ---------------------------------------------------------------- //
				
		this.stack_control.update();

		GUIControl.get().setStomachRate(this.player_control.getLifeRate());

	}

	public void		heightGain()
	{
		if(this.height_level < MAX_HEIGHT_LEVEL) {

			this.height_level++;
	
			this.bg_control.setHeightRate((float)this.height_level/(float)MAX_HEIGHT_LEVEL);
		}
	}

}
