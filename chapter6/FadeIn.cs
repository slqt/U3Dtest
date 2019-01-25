using UnityEngine;
using System.Collections;

// ----------------------------------------------------------------------------
// 画面をフェードイン.
//  - alpha(透過)を 1(透過率0%) -> 0(透過率100%) に変化させてフェードイン効果を再現する.
// ----------------------------------------------------------------------------
public class FadeIn : MonoBehaviour {
	
	protected float		alphaRate = 1f;				// 透過率.
	protected Color		textureColor;				// テクスチャの色情報.
	
	public UnityEngine.UI.Image		uiImage;

	void Start () {
	
		// FadeOutされた状態.
		this.textureColor    = this.uiImage.color;
		this.textureColor.a  = this.alphaRate;
		this.uiImage.color   = this.textureColor;
		this.uiImage.enabled = true;
	}

	void Update () {
	
		// 透過が100%に達していない?.	
		if ( this.alphaRate > 0 ) {

			// FadeIn.
			this.alphaRate -= 0.007f;
			this.textureColor.a = this.alphaRate;
			this.uiImage.color = this.textureColor;
		}

	}
}
