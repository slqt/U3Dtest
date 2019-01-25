using UnityEngine;
using System.Collections;

// ----------------------------------------------------------------------------
// SubScreen右上へのロックスロット表示制御.
// ----------------------------------------------------------------------------
public class LockSlot : MonoBehaviour {

	private bool 			isEnable = false;	// 表示有効.

	public GameObject[]		uiLockSlotImages;	// 各スロットのイメージ.
	
	void Start ()
	{
		isEnable = true;
	
		// 初期値表示.
		for( int i = 0; i < uiLockSlotImages.Length; i++ )
		{
			uiLockSlotImages[i].SetActive(false);
		}
	}
	
	// ------------------------------------------------------------------------
	// ロックオンの数だけロックスロットを使用中として表示.
	// ------------------------------------------------------------------------
	public void SetLockCount( int lockCount )
	{
		if ( isEnable )
		{
			for( int i = 0; i < uiLockSlotImages.Length; i++ )
			{
				if ( i < lockCount )
				{
					uiLockSlotImages[i].SetActive(true);
				}
				else
				{
					uiLockSlotImages[i].SetActive(false);
				}
			}			
		}
	}
}
