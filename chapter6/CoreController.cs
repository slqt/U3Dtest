using UnityEngine;
using System.Collections;

// ----------------------------------------------------------------------------
// BOSS Core 破壊された時のメッセージ表示.
// ----------------------------------------------------------------------------
public class CoreController : MonoBehaviour {
	
	private PrintMessage printMessage;		// SubScreenのメッセージ領域.
	
	void Start () {
		
		// PrintMessage のインスタンスを取得.
		printMessage = Navi.get().GetPrintMessage();			
	}
	
	// ------------------------------------------------------------------------
	// BOSS Core が破壊された時の処理.
	// ------------------------------------------------------------------------
	void OnDestroy()
	{
		if ( this.GetComponent<EnemyStatus>() )
		{
			if (
				this.GetComponent<EnemyStatus>().GetIsBreakByPlayer() ||
				this.GetComponent<EnemyStatus>().GetIsBreakByStone() )
			{
				printMessage.SetMessage(" ");
				printMessage.SetMessage("DEFEATED SPIDER-TYPE.");
				printMessage.SetMessage("MISSION ACCOMPLISHED.");
				printMessage.SetMessage(" ");
			}
		}
	}
	
}
