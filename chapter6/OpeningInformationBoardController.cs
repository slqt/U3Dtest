using UnityEngine;
using System.Collections;

// ----------------------------------------------------------------------------
// オープニング画面で表示するメッセージを画面外の下方向から上へスクロール.
// ----------------------------------------------------------------------------
public class OpeningInformationBoardController : MonoBehaviour {
	
	public float		scrollSpeed   = 130.0f;				// タイトルがスクロールするスピード.
	public float		startPosition = -520.0f;			// スクロール開始位置.
	
	private float 		stopPositionY;						// タイトルが停止する位置.

	public GameObject	uiInformationBoard;

	// ================================================================ //
	
	void Start () 
	{
		// スクロール停止位置を取得.
		stopPositionY = uiInformationBoard.GetComponent<RectTransform>().localPosition.y;
		
		// メッセージを初期表示位置に移動.
		Vector3 tmpPosition =  uiInformationBoard.GetComponent<RectTransform>().localPosition;

		uiInformationBoard.GetComponent<RectTransform>().localPosition = new Vector3( tmpPosition.x, startPosition, 0 );
	}
	
	void Update () 
	{
		// 停止位置までスクロール.
		Vector3 position = uiInformationBoard.GetComponent<RectTransform>().localPosition;
		if ( position.y < stopPositionY )
		{
			position.y += scrollSpeed * Time.deltaTime;
			uiInformationBoard.GetComponent<RectTransform>().localPosition = new Vector3( position.x, position.y, 0 );
		}

	}
}
