
using UnityEngine;


/// <summary>宝箱オブジェクト専用クラス</summary>
public class TreasureBoxObject : BaseObject
{
	//==============================================================================================
	// 公開メソッド.

	/// <summary>アニメーションの再生対象オブジェクト</summary>
	public GameObject m_animatingObject = null;

	/// <summary>イベントアクターから来たメッセージを処理する</summary>
	public override bool updateMessage( string message, string[] parameters )
	{
		if ( !m_isAnimated )
		{
			switch ( message )
			{
			// 開ける.
			case "open":
				this.play_open_animation();
				m_isAnimated = true;
				return true;
				// break; return があるので “break” は実行されない.
			// 閉じる.
			case "close":
				this.play_close_animation();
				m_isAnimated = true;
				return true;
				// break;

			// その他.
			default:
				Debug.LogError( "Invalid message \"" + message + "\"");
				return false;  // すぐ終了.
				// break;
			}
		}
		else
		{
			if (this.is_animation_playing())
			{
				return true;
			}
			else
			{
				// アニメーション終了
				m_isAnimated = false;
				return false;
			}
		}
	}

	private void	play_open_animation()
	{
		m_animatingObject.GetComponent<Animator>().SetTrigger("open");
	}
	private void	play_close_animation()
	{
		m_animatingObject.GetComponent<Animator>().SetTrigger("close");
	}
	private bool	is_animation_playing()
	{
		return(false);
		//return(m_animatingObject.GetComponent<Animator>().is.isPlaying);
	}
	//==============================================================================================
	// 非公開メンバ変数.

	/// <summary>アニメーション中かどうか</summary>
	private bool m_isAnimated = false;
}
