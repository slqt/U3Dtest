using UnityEngine;
using System.Collections;

public class EngineUnitController : MonoBehaviour {
	
	private PrintMessage printMessage;		// SubScreenのメッセージ領域.
	
	void Start () 
	{
		// PrintMessage のインスタンスを取得.
		printMessage = Navi.get().GetPrintMessage();	
	}
	
	void OnDestroy()
	{
		if ( this.GetComponent<EnemyStatus>() )
		{
			if (
				this.GetComponent<EnemyStatus>().GetIsBreakByPlayer() ||
				this.GetComponent<EnemyStatus>().GetIsBreakByStone() )
			{
				printMessage.SetMessage(" ");
				printMessage.SetMessage("DESTROYED DEFENSIVE BULKHEAD.");
				printMessage.SetMessage(" ");
			}
		}
	}
}
