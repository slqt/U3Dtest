
using UnityEngine;


/// <summary>家オブジェクト専用クラス</summary>
public class HouseObject : BaseObject
{
	//==============================================================================================
	// 公開メソッド.

	/// <summary>アニメーションの再生対象オブジェクト</summary>
	public GameObject m_animatingObject = null;

	/// <summary>イベントアクターから来たメッセージを処理する</summary>
	public override bool updateMessage( string message, string[] parameters )
	{
		bool	ret = false;

		if ( !m_isAnimated )
		{
			switch ( message )
			{

				// 開ける.
				case "open":
				{
					m_animatingObject.GetComponent<Animator>().SetTrigger("open");
					m_isAnimated = true;
					ret = true;
				}
				break;
	
				// その他.
				default:
				{
					// すぐ終了.
					ret =  false;
				}
				break;
			}
		}
		else
		{
			if(!m_animatingObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Open")) {

				ret = true;

			} else {

				// アニメーション終了.
				m_isAnimated = false;
				ret =  false;
			}
		}

		return(ret);
	}


	//==============================================================================================
	// 非公開メンバ変数.

	/// <summary>アニメーション中かどうか</summary>
	private bool m_isAnimated = false;
}
