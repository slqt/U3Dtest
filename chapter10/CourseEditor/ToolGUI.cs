using UnityEngine;
using System.Collections;
using System.Linq;

public class ToolGUI : MonoBehaviour {

	public enum BUTTON {

		NONE = -1,

		NEW = 0,					// クリアー.
#if UNITY_EDITOR
		LOAD,						// ロード.
		SAVE,						// セーブ.
#endif
		CREATE_ROAD,				// 道路を生成する.
		RUN,						// 車で走る.

		TUNNEL_CREATE,				// トンネルを生成する.
		TUNNEL_FORWARD,				// トンネルを前に移動する.
		TUNNEL_BACKWARD,			// トンネルを後ろに移動する.

		FOREST_CREATE,				// 森を作る.
		FOREST_START_FORWARD,		// 森の開始地点を前に移動する.
		FOREST_START_BACKWARD,		// 森の開始地点を後ろに移動する.
		FOREST_END_FORWARD,			// 森の終了地点を前に移動する.
		FOREST_END_BACKWARD,		// 森の終了地点を後ろに移動する.

		BUIL_CREATE,				// ビル街を作る.
		BUIL_START_FORWARD,			// ビル街の開始地点を前に移動する.
		BUIL_START_BACKWARD,		// ビル街の開始地点を後ろに移動する.
		BUIL_END_FORWARD,			// ビル街の終了地点を前に移動する.
		BUIL_END_BACKWARD,			// ビル街の終了地点を後ろに移動する.

		JUMP_CREATE,				// ジャンプ台を作る.
		JUMP_FORWARD,				// ジャンプ台を前に移動する.
		JUMP_BACKWARD,				// ジャンプ台を後ろに移動する.

		NUM,
	};

	public UIButton[] buttons;

	public AudioClip		audio_clip_click;		// クリック音.

	public bool				is_edit_mode = true;	// エディットモード？.

	// ------------------------------------------------------------------------ //

	public GameObject	uiCanvas;
	public GameObject	uiButtonPrefab;

	public GameObject	uiForestIconPrefab;
	public GameObject	uiBuildingIconPrefab;
	public GameObject	uiTunnelIconPrefab;
	public GameObject	uiJumpIconPrefab;

	// ================================================================ //
	// MonoBehaviour からの継承.

	void 	Start()
	{

		this.buttons = new UIButton[(int)BUTTON.NUM];

		foreach(var i in this.buttons.Select((v, i) => i)) {

			UIButton	button = (GameObject.Instantiate(this.uiButtonPrefab) as GameObject).GetComponent<UIButton>();

			button.GetComponent<RectTransform>().SetParent(this.uiCanvas.GetComponent<RectTransform>());

			switch((BUTTON)i) {

				case BUTTON.TUNNEL_FORWARD:
				case BUTTON.TUNNEL_BACKWARD:
				case BUTTON.FOREST_START_FORWARD:
				case BUTTON.FOREST_START_BACKWARD:
				case BUTTON.FOREST_END_FORWARD:
				case BUTTON.FOREST_END_BACKWARD:
				case BUTTON.BUIL_START_FORWARD:
				case BUTTON.BUIL_START_BACKWARD:
				case BUTTON.BUIL_END_FORWARD:
				case BUTTON.BUIL_END_BACKWARD:
				case BUTTON.JUMP_FORWARD:
				case BUTTON.JUMP_BACKWARD:
				{
					button.is_repeat_button = true;
				}
				break;
			}

			this.buttons[i] = button;
		}

		this.is_edit_mode = true;

		//

		int		x, y;

		y = 210;
		x = -265;

		this.create_button_file(x, y);

		x += 105;
		this.create_button_road(x, y);

		x += 105;
		this.create_button_tunnel(x, y);

		x += 105;
		this.create_button_forest(x, y);

		x += 105;
		this.create_button_buil(x, y);

		x += 105;
		this.create_button_jump(x, y);
	}
	
	void Update ()
	{
		foreach(var i in this.buttons.Select((v, i) => i)) {

			// 押した瞬間の SE.
			//
			if(this.buttons[i].trigger_on) {

				if(i == (int)BUTTON.CREATE_ROAD) {

				} else {

					this.GetComponent<AudioSource>().PlayOneShot(this.audio_clip_click);
				}
			}
		}
	}

	// ================================================================ //

	// テスト走行開始のときに呼ばれる.
	public void	onStartTestRun()
	{
		this.is_edit_mode = false;

		this.buttons[(int)BUTTON.RUN].setText("戻～る");
	}

	// テスト走行終わりのときに呼ばれる.
	public void	onStopTestRun()
	{
		this.is_edit_mode = true;

		this.buttons[(int)BUTTON.RUN].setText("車で走～る");
	}

	// ================================================================ //

	// ボタンを作る.
	protected void	create_button(BUTTON id, int x, int y, string text)
	{
		UIButton	button = this.buttons[(int)id];

		button.setPosition(x, y);
		button.setText(text);
	}

	// 半分サイズのボタンを作る.
	protected void	create_half_button(BUTTON id, int x, int y, string text)
	{
		UIButton	button = this.buttons[(int)id];

		button.setPosition(x, y);
		button.setScale(0.4f, 1.0f);
		button.setText(text);
	}

	// ファイル関連.
	protected void	create_button_file(int x, int y)
	{
		this.create_button(BUTTON.NEW, x, y, "無くな～る");
		y -= 30;

#if UNITY_EDITOR
		this.create_button(BUTTON.LOAD, x, y, "読んでく～る");
		y -= 30;
		this.create_button(BUTTON.SAVE, x, y, "書いてく～る");
		y -= 30;
#endif
	}


	// 道路生成とか.
	protected void	create_button_road(int x, int y)
	{
		this.create_button(BUTTON.CREATE_ROAD, x, y, "道にな～る");
		y -= 30;

		this.create_button(BUTTON.RUN, x, y, "車で走～る");
		y -= 30;
	}

	// トンネル関連.
	protected void	create_button_tunnel(int x, int y)
	{
		this.create_button(BUTTON.TUNNEL_CREATE, x, y, "長いトンネ～ル");
		y -= 30;

		this.create_half_button(BUTTON.TUNNEL_FORWARD,  x - 25,      y, "<<");
		this.create_half_button(BUTTON.TUNNEL_BACKWARD, x - 25 + 50, y, ">>");
		y -= 30;
	}

	// 森関連.
	protected void	create_button_forest(int x, int y)
	{
		this.create_button(BUTTON.FOREST_CREATE, x, y, "森があ～る");
		y -= 30;

		this.create_half_button(BUTTON.FOREST_START_BACKWARD, x - 25,      y, "<<");
		this.create_half_button(BUTTON.FOREST_START_FORWARD,  x - 25 + 50, y, ">>");
		y -= 30;

		this.create_half_button(BUTTON.FOREST_END_BACKWARD, x - 25,      y, "<<");
		this.create_half_button(BUTTON.FOREST_END_FORWARD,  x - 25 + 50, y, ">>");
		y -= 30;
	}

	// ビル関連.
	protected void	create_button_buil(int x, int y)
	{
		this.create_button(BUTTON.BUIL_CREATE, x, y, "高いビ～ル");
		y -= 30;

		this.create_half_button(BUTTON.BUIL_START_BACKWARD, x - 25,      y, "<<");
		this.create_half_button(BUTTON.BUIL_START_FORWARD,  x - 25 + 50, y, ">>");
		y -= 30;

		this.create_half_button(BUTTON.BUIL_END_BACKWARD, x - 25,      y, "<<");
		this.create_half_button(BUTTON.BUIL_END_FORWARD,  x - 25 + 50, y, ">>");
		y -= 30;
	}

	// ジャンプ台関連.
	protected void	create_button_jump(int x, int y)
	{
		this.create_button(BUTTON.JUMP_CREATE, x, y, "空も飛べ～る");
		y -= 30;

		this.create_half_button(BUTTON.JUMP_BACKWARD, x - 25,      y, "<<");
		this.create_half_button(BUTTON.JUMP_FORWARD,  x - 25 + 50, y, ">>");
		y -= 30;
	}

	// ================================================================ //

	// 『森』アイコンを作る.
	public UIIcon		createForestIcon()
	{
		UIIcon	icon = GameObject.Instantiate(this.uiForestIconPrefab).GetComponent<UIIcon>();

		icon.GetComponent<RectTransform>().SetParent(this.uiCanvas.GetComponent<RectTransform>());

		return(icon);
	}

	// 『ビル』アイコンを作る.
	public UIIcon		createBuildingIcon()
	{
		UIIcon	icon = GameObject.Instantiate(this.uiBuildingIconPrefab).GetComponent<UIIcon>();

		icon.GetComponent<RectTransform>().SetParent(this.uiCanvas.GetComponent<RectTransform>());

		return(icon);
	}

	// 『トンネル』アイコンを作る.
	public UIIcon		createTunnelIcon()
	{
		UIIcon	icon = GameObject.Instantiate(this.uiTunnelIconPrefab).GetComponent<UIIcon>();

		icon.GetComponent<RectTransform>().SetParent(this.uiCanvas.GetComponent<RectTransform>());

		return(icon);
	}

	// 『ジャンプ』アイコンを作る.
	public UIIcon		createJumpIcon()
	{
		UIIcon	icon = GameObject.Instantiate(this.uiJumpIconPrefab).GetComponent<UIIcon>();

		icon.GetComponent<RectTransform>().SetParent(this.uiCanvas.GetComponent<RectTransform>());

		return(icon);
	}
}
