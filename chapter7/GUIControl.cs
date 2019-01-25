using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GUIControl : MonoBehaviour {

	public GameObject	uiStockIconPrefab;
	public GameObject	numberPrefab;

	public GameObject				uiCanvas;
	public UnityEngine.UI.Image		uiStomachGreenImage;
	public UnityEngine.UI.Text		uiGameOverText;
	public UnityEngine.UI.Text		uiGoalText;

	public UnityEngine.Sprite[]		numSprites;

	public GUIScore		guiScore;							//!< スコア表示用
	public GUIScore		guiHeight;							//!< 登った段数表示用

	protected float		stomach_rate = 1.0f;				//!< 胃袋の緑の部分の比率.

	protected bool		is_disp_goal = false;				//!< 「GOAL」表示する？.
	protected bool		is_disp_gameover = false;			//!< 「GAME OVER」表示する？.
	
	protected SceneControl		scene_control = null;
	
	protected List<UnityEngine.UI.Image>	ui_stock_icons = new List<UnityEngine.UI.Image>();

	// ================================================================ //
	// MonoBehaviour からの継承.

	void 	Start()
	{

		this.scene_control = GameObject.FindGameObjectWithTag("SceneControl").GetComponent<SceneControl>();
		
		//

		this.is_disp_goal = false;
		this.is_disp_gameover = false;

		//

		for(int i = 0;i < this.scene_control.player_stock;i++) {

			UnityEngine.UI.Image	stock_icon = this.create_stock_icon(215.0f + i*40.0f, 200.0f);

			this.ui_stock_icons.Add(stock_icon);
		}
	}
	
	void 	Update()
	{
		this.guiScore.setNum(this.scene_control.stack_control.score);
	
		this.guiHeight.setNum(this.scene_control.height_level);

		if(this.stomach_rate > 0.0f) {

			this.uiStomachGreenImage.GetComponent<RectTransform>().localScale = new Vector3(1.0f, this.stomach_rate, 1.0f);
			this.uiStomachGreenImage.gameObject.SetActive(true);

		} else {

			this.uiStomachGreenImage.gameObject.SetActive(false);
		}

		if(this.is_disp_gameover) {
			this.uiGameOverText.gameObject.SetActive(true);
		}

		if(this.is_disp_goal) {
			this.uiGoalText.gameObject.SetActive(true);
		}
	}

	// ================================================================ //

	public void		setStockCount(int stock_count)
	{
		for(int i = 0;i < this.ui_stock_icons.Count;i++) {

			UnityEngine.UI.Image	stock_icon = this.ui_stock_icons[i];

			if(i < stock_count) {
				stock_icon.gameObject.SetActive(true);
			} else {
				stock_icon.gameObject.SetActive(false);
			}
		}
	}

	public void		setDispGameOver(bool is_disp)
	{
		this.is_disp_gameover = is_disp;
	}

	public void		setDispGoal(bool is_disp)
	{
		this.is_disp_goal = is_disp;
	}

	public void		setStomachRate(float rate)
	{
		this.stomach_rate = rate;
	}

	// ================================================================ //

	// 残機アイコンを作る.
	protected UnityEngine.UI.Image	create_stock_icon(float x, float y)
	{
		UnityEngine.UI.Image	icon = GameObject.Instantiate(this.uiStockIconPrefab).GetComponent<UnityEngine.UI.Image>();

		icon.GetComponent<RectTransform>().SetParent(this.uiCanvas.GetComponent<RectTransform>());
		icon.GetComponent<RectTransform>().localPosition = new Vector3(x, y, 0.0f);

		return(icon);
	}

	// ================================================================ //
	// インスタンス.

	private	static GUIControl	instance = null;

	public static GUIControl	get()
	{
		if(GUIControl.instance == null) {

			GameObject		go = GameObject.Find("GameCanvas");

			if(go != null) {

				GUIControl.instance = go.GetComponent<GUIControl>();

			} else {

				Debug.LogError("Can't find game object \"GUIControl\".");
			}
		}

		return(GUIControl.instance);
	}
}
