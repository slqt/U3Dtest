using UnityEngine;
using System.Collections;

public class GUIControl : MonoBehaviour {

	public SceneControl		scene_control = null;
	public ScoreControl		score_control = null;
	
	public GameObject	uiImageStart;		// 『はじめっ！』
	public GameObject	uiImageReturn;		// 『もどる！』

	public RankDisp		rankSmallDefeat;				// 『鬼切り』評価.
	public RankDisp		rankSmallEval;					// 『見切り』評価.
	public RankDisp		rankTotal;						// トータル評価.

	public UnityEngine.Sprite[]	uiSprite_GradeSmall;	// 『鬼切り』『見切り』用の評価の、小さな文字（優/良/可/不可）
	public UnityEngine.Sprite[]	uiSprite_Grade;			// トータル評価の文字（優/良/可/不可）

	// ================================================================ //

	void	Awake()
	{
		this.scene_control = SceneControl.get();
		this.score_control = GetComponent<ScoreControl>();
		
		this.score_control.setNumForce( this.scene_control.result.oni_defeat_num );

		this.rankSmallDefeat.uiSpriteRank = this.uiSprite_GradeSmall;
		this.rankSmallEval.uiSpriteRank   = this.uiSprite_GradeSmall;
		this.rankTotal.uiSpriteRank       = this.uiSprite_Grade;
	}

	void	Start()
	{
	}
	
	void	Update()
	{
		// 『切ったおにの数』をスコアー表示にセット.
		this.score_control.setNum(this.scene_control.result.oni_defeat_num);

		// ---------------------------------------------------------------- //
		// デバッグ用
	#if false
		SceneControl	scene = this.scene_control;

		dbPrint.setLocate(10, 5);
		dbPrint.print(scene.attack_time);
		dbPrint.print(scene.evaluation);
		if(this.scene_control.level_control.is_random) {

			dbPrint.print("RANDOM(" + scene.level_control.group_type_next + ")");

		} else {

			dbPrint.print(scene.level_control.group_type_next);
		}

		dbPrint.print(scene.result.oni_defeat_num);

		// 切りの評価（近くで切った？）の合計
		for(int i = 0;i < (int)SceneControl.EVALUATION.NUM;i++) {

			dbPrint.print(((SceneControl.EVALUATION)i).ToString() + " " + scene.result.eval_count[i].ToString());
		}
	#endif
	}

	// 『はじめっ！』の文字を表示/非表示にする.
	public void	setVisibleStart(bool is_visible)
	{
		this.uiImageStart.SetActive(is_visible);
	}

	// 『もどる！』の文字を表示/非表示にする.
	public void	setVisibleReturn(bool is_visible)
	{
		this.uiImageReturn.SetActive(is_visible);
	}

	// 『鬼切り』の評価の表示をスタートする.
	public void	startDispDefeatRank()
	{
		int		rank  = this.scene_control.result_control.getDefeatRank();

		this.rankSmallDefeat.startDisp(rank);
	}

	// 『鬼切り』の評価の表示を消す.
	public void	hideDefeatRank()
	{
		this.rankSmallDefeat.hide();
	}

	// 『鬼切り』の評価の表示をスタートする.
	public void	startDispEvaluationRank()
	{
		int		rank  = this.scene_control.result_control.getEvaluationRank();

		this.rankSmallEval.startDisp(rank);
	}

	// 『鬼切り』の評価の表示を消す.
	public void	hideEvaluationRank()
	{
		this.rankSmallEval.hide();
	}

	// トータル評価の表示をスタートする.
	public void	startDispTotalRank()
	{
		int		rank  = this.scene_control.result_control.getTotalRank();

		this.rankTotal.startDisp(rank);
	}

	void	OnGUI()
	{			
	}

	// ================================================================ //
	// インスタンス.

	protected	static GUIControl	instance = null;

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
