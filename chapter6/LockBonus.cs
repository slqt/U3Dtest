using UnityEngine;
using System.Collections;

// ----------------------------------------------------------------------------
// SubScreen右上へのロックボーナス表示制御.
// ----------------------------------------------------------------------------
public class LockBonus : MonoBehaviour {
	
	public UnityEngine.Sprite[]		uiLockBonusSprites;		// ロックボーナス x0 ～ x64 の画像（スプライト）.
	public UnityEngine.UI.Image		uiLockBonusImage;		// ロックボーナスのイメージ.

	private bool isEnable = false;			// 表示有効.

	// ================================================================ //

	void Start ()
	{
		isEnable = true;

		// 初期値表示.
		if(this.uiLockBonusSprites.Length > 0) {

			this.uiLockBonusImage.sprite = this.uiLockBonusSprites[0];
		}
	}
	
	// ------------------------------------------------------------------------
	// 指定したロックボーナスの画像表示.
	// ------------------------------------------------------------------------
	public void SetLockCount( int lockCount )
	{
		if ( isEnable )
		{
			if(0 <= lockCount && lockCount < this.uiLockBonusSprites.Length) {

				this.uiLockBonusImage.sprite = this.uiLockBonusSprites[lockCount];
			}
		}
	}
}
