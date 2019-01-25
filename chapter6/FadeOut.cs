using UnityEngine;
using System.Collections;

// ----------------------------------------------------------------------------
// 画面をフェードアウト.
//  - alpha(透過)を 0(透過率100%) -> 1(透過率0%) に変化させてフェードアウト効果を再現する.
// ----------------------------------------------------------------------------
public class FadeOut : MonoBehaviour 
{
	public UnityEngine.UI.Image		uiImage;

	protected float		alphaRate = 0f;						// 透過率.
	protected Color		textureColor;						// テクスチャの色情報.

	protected string 	openingSceneName = "opening";		// オープニングシーン名.

	protected bool 		isEnabled = false;

	void Start () 
	{
		// FadeOutされた状態.
		this.textureColor    = this.uiImage.color;
		this.textureColor.a  = this.alphaRate;
		this.uiImage.color   = this.textureColor;
		this.uiImage.enabled = true;
	}

	void Update () 
	{
		if( this.isEnabled ) {

			// 透過が100%に達していない?.
			if( this.alphaRate < 1.0f ) {

				// FadeOut.
				this.alphaRate += 0.007f;
				this.textureColor.a = this.alphaRate;
				this.uiImage.color  = this.textureColor;

			} else {

				// ゲームシーンを呼び出す.
				UnityEngine.SceneManagement.SceneManager.LoadScene( openingSceneName );
			}
		}
	}
	
	public void SetEnable()
	{
		StartCoroutine( WaitAndEnable( 8f ) );
	}
	
	IEnumerator WaitAndEnable( float waitForSeconds )
	{
		// 指定した時間を待つ.
		yield return new WaitForSeconds( waitForSeconds );
		
		this.isEnabled = true;
	}
}
