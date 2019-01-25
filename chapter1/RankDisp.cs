using UnityEngine;
using System.Collections;

public class RankDisp : MonoBehaviour {

	protected const float	ZOOM_TIME = 0.4f;

	public float	timer = 0.0f;
	public float	scale = 1.0f;
	public float	alpha = 0.0f;

	public UnityEngine.UI.Image		uiImageGrade;		// 評価の文字（優/良/可/不可）のイメージ

	public UnityEngine.Sprite[]		uiSpriteRank;		// 『鬼切り』『見切り』用の評価の文字（優/良/可/不可）のスプライト

	// ================================================================ //
	// MonoBehaviour からの継承.

	void	Awake()
	{
	}

	void	Start()
	{
	}
	
	void	Update()
	{
		float	delta_time = Time.deltaTime;

		this.update_sub();

		this.timer += delta_time;
	}

	protected void		update_sub()
	{
		float	zoom_in_time = ZOOM_TIME;
		float	rate;

		if(this.timer < zoom_in_time) {

			rate = this.timer/zoom_in_time;
			rate = Mathf.Pow(rate, 2.5f);
			this.scale = Mathf.Lerp(1.5f, 1.0f, rate);

		} else {

			this.scale = 1.0f;
		}

		if(this.timer < zoom_in_time) {

			rate = this.timer/zoom_in_time;
			rate = Mathf.Pow(rate, 2.5f);
			this.alpha = Mathf.Lerp(0.0f, 1.0f, rate);

		} else {

			this.alpha = 1.0f;
		}

		// アルファーを UI.Image にセットする.

		UnityEngine.UI.Image[]		images = this.GetComponentsInChildren<UnityEngine.UI.Image>();

		foreach(var image in images) {

			Color	color = image.color;

			color.a = this.alpha;

			image.color = color;
		}

		// スケールをセットする.
		this.GetComponent<RectTransform>().localScale = Vector3.one*this.scale;
	}

	// ================================================================ //

	public void		startDisp(int rank)
	{
		this.uiImageGrade.sprite = this.uiSpriteRank[rank];

		this.gameObject.SetActive(true);

		this.timer = 0.0f;

		this.update_sub();
	}
	public void		hide()
	{
		this.gameObject.SetActive(false);
	}

}
