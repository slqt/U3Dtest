
using System;
using UnityEngine;


/// <summary>choice コマンドのイベントアクター</summary>
public class EventActorChoice : EventActor
{
	//==============================================================================================
	// 公開メソッド.

	/// <summary>コンストラクタ</summary>
	public EventActorChoice( BaseObject baseObject, string name, string[] choices )
	{
		m_object  = baseObject;
		m_name    = name;
		m_choices = choices;
	}

	/// <summary>アクターが生成された際に最初に実行されるメソッド</summary>
	public override void start( EventManager evman )
	{
		// 地の文を表示.
		TextManager		text_man = TextManager.get();

		text_man.createButtons(m_choices);
	}

	/// <summary>アクターが破棄されるまで毎フレーム実行されるメソッド</summary>
	/// UnityEngine.MonoBehaviour.Update() の代わり
	public override void execute( EventManager evman )
	{
		TextManager		text_man = TextManager.get();

		do {

			if(text_man.selected_button == "") {
				break;
			}

			int		selected_index = System.Array.IndexOf(m_choices, text_man.selected_button);

			if(selected_index < 0) {
				break;
			}

			// クリックされた選択肢のインデックスをゲーム内変数に設定して終了.
			// (最初の選択肢は 1 になる).
			m_object.setVariable(m_name, (selected_index + 1).ToString());

			text_man.deleteButtons();
			m_isDone = true;

		} while(false);
	}

	/// <summary>アクターで行うべき処理が終わったかどうかを返す</summary>
	public override bool isDone()
	{
		return m_isDone;
	}

	/// <summary>実行終了後にクリックを待つかどうかを返す</summary>
	public override bool isWaitClick( EventManager evman )
	{
		// 選択肢でクリック待ちが入るのでこっちでは待たない.
		return false;
	}


	//==============================================================================================
	// 非公開メンバ変数

	/// <summary>ゲーム内変数を操作するオブジェクト</summary>
	private BaseObject m_object;

	/// <summary>ゲーム内変数名</summary>
	private string m_name;

	/// <summary>選択肢の一覧</summary>
	private string[] m_choices;

	/// <summary>アクターの処理が終了しているかどうか</summary>
	private bool m_isDone = false;


	//==============================================================================================
	// 静的メソッド

	/// <summary>イベントアクターのインスタンスを生成する</summary>
	public static EventActorChoice CreateInstance( string[] parameters, GameObject manager )
	{
		if ( parameters.Length >= 3 )
		{
			// 指定されたオブジェクトを探す.
			BaseObject bo = manager.GetComponent< ObjectManager >().find( parameters[ 0 ] );
			if ( bo != null )
			{
				string[] choices = new string[ parameters.Length - 2 ];
				Array.Copy( parameters, 2, choices, 0, choices.Length );

				// アクターを生成.
				EventActorChoice actor = new EventActorChoice( bo, parameters[ 1 ], choices );
				return actor;
			}
		}

		Debug.LogError( "Failed to create an actor." );
		return null;
	}
}
