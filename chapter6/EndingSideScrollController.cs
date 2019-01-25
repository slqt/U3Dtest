using UnityEngine;
using System.Collections;

// ----------------------------------------------------------------------------
// 対象とするゲームオブジェクトを横方向にスクロール.
// ----------------------------------------------------------------------------
public class EndingSideScrollController : MonoBehaviour {

	public float		scrollSpeed   = 10.0f;				// スクロールするスピード.
	public Vector3 		startPosition = Vector3.zero;		// スクロール開始位置.

	protected float 	distanceToStartEaseIn = 10.0f;		// イーズインを開始する停止位置からの距離.
	
	protected Vector3	stopPosition;						// スクロール停止位置.

	public GameObject	uiImage;

	void Awake () 
	{
		// スクロール停止位置を取得.
		this.stopPosition = this.uiImage.GetComponent<RectTransform>().localPosition;
	
		// イーズインを開始する停止位置からの距離 を求める.
		Vector3	v = this.stopPosition - this.startPosition;
		v.z = 0.0f;
	
		this.distanceToStartEaseIn = v.magnitude*0.1f;

		// ゲームオブジェクトを初期表示位置に移動.
		Vector3 tmpPosition = this.uiImage.GetComponent<RectTransform>().localPosition;
		this.uiImage.GetComponent<RectTransform>().localPosition = new Vector3(startPosition.x, startPosition.y, tmpPosition.z);
	}
	
	void FixedUpdate () 
	{	
		// 停止位置までスクロール.
		Vector3 	position = this.uiImage.GetComponent<RectTransform>().localPosition;
		Vector3		to_goal  = this.stopPosition - position;

		to_goal.z = 0.0f;

		if(to_goal.magnitude < this.distanceToStartEaseIn) {

			// イーズイン.
			float		rate = to_goal.magnitude/this.distanceToStartEaseIn;

			rate = Mathf.Max(0.01f, rate);

			float		additionDistance = rate*this.scrollSpeed*Time.deltaTime;

			if(to_goal.magnitude < additionDistance) {

				position = this.stopPosition;

			} else {

				to_goal.Normalize();
				position += to_goal*additionDistance;
			}

		} else {

			float		additionDistance = this.scrollSpeed * Time.deltaTime;

			to_goal.Normalize();

			// スクロール.
			position += to_goal*additionDistance;
		}

		this.uiImage.GetComponent<RectTransform>().localPosition = position;
	}
}
