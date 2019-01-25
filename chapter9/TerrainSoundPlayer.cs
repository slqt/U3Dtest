
using UnityEngine;


/// <summary>Terrain と DraggableObject が衝突したときのサウンドを再生するクラス</summary>
public class TerrainSoundPlayer : MonoBehaviour
{
	//==============================================================================================
	// MonoBehaviour 関連のメンバ変数・メソッド.

	/// <summary>イベントマネージャオブジェクト</summary>
	public EventManager		m_manager = null;

	/// <summary>水面？</summary>
	public bool				m_isWater = false;

	//==============================================================================================

	void	Awake()
	{
		m_manager = EventManager.get();
	}

	/// <summary>オブジェクトとの衝突</summary>
	private void OnCollisionEnter( Collision collision )
	{
		if ( m_manager.isExecutingEvents() ) return;  // イベント実行中は鳴らさない.

		DraggableObject draggable = collision.gameObject.GetComponent< DraggableObject >();
		if ( draggable != null && GetComponent<AudioSource>() != null )
		{
			GetComponent<AudioSource>().Play();
		}

		// 水面エフェクトはここで再生.
		if(m_isWater) {

			EffectManager.get().playLandingWaterEffect(draggable);
		}
	}
}
