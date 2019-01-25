// 有効にすると、スペースキーでズームイン/ズームアウトができるようになります.
//#define	ENABLE_ZOOM_IN_DEBUG

using UnityEngine;
using System.Collections;

public class FollowCamera : MonoBehaviour {

	private const	float	ZOOM_IN_DURATION  = 0.2f;		// [sec] ズームインにかかる時間.
	private const	float	ZOOM_OUT_DURATION = 1.5f;		// [sec] ズームアウトにかかる時間.

	public Vector3		m_position_offset;
	public Vector3		m_position_offset_zoom_in;

	public Transform	m_target;
	private bool		m_zoom_in;
	private float		m_zoom_in_timer = 0.0f;

	// Use this for initialization
	void Start ()
	{
		// ズームインが終わった＝ズームアウトした状態からスタート.
		m_zoom_in = false;
		m_zoom_in_timer = ZOOM_OUT_DURATION;
	}

	void	Update()
	{
#if ENABLE_ZOOM_IN_DEBUG
		if(Input.GetKeyDown(KeyCode.Space)) {

			if(m_zoom_in) {

				OnEndAttack();

			} else {

				OnAttack();
			}
		}
#endif

	}
	
	// Update is called once per frame
	void	LateUpdate()
	{
		float	rate = 0.0f;

		if(m_zoom_in) {

			// ズームインのとき.
			rate = m_zoom_in_timer/ZOOM_IN_DURATION;			// 「時間」を「開始から終了までの比率」に変換.
			rate = Mathf.Clamp01(rate);							// 0.0 から 1.0 の範囲に収める.
			rate = Mathf.Sin(rate*Mathf.PI/2.0f);
			rate = Mathf.Pow(rate, 0.5f);

		} else {

			// ズームアウトのとき.
			rate = m_zoom_in_timer/ZOOM_OUT_DURATION;
			rate = Mathf.Clamp01(rate);
			rate = Mathf.Sin(rate*Mathf.PI/2.0f);
			rate = Mathf.Pow(rate, 0.5f);

			// タイマーの 0.0 ～ 1.0 が、ズーム率の 0.0 ～ 1.0 になるようにする.
			rate = 1.0f - rate;
		}

		// 位置のオフセットの計算.
		Vector3		offset = Vector3.Lerp(m_position_offset, m_position_offset_zoom_in, rate);
		transform.position = m_target.position + offset;

		// 画角の計算、設定.
		float	fov = Mathf.Lerp(60.0f, 30.0f, rate);
		this.GetComponent<Camera>().fieldOfView = fov;

		m_zoom_in_timer += Time.deltaTime;
	}
	
	// 攻撃した/されたときに呼ばれる.
	public void OnAttack()
	{
#if false
		m_zoom_in = true;
		m_zoom_in_timer = 0.0f;
#else
	// ズームアウトの途中でズームインが始まっても、大丈夫なコード.
	// 少し難しいので、最初はわからなくても気にしないでください.

		if(m_zoom_in) {

			// ズームイン中だったら何もしない.

		} else {

			m_zoom_in = true;

			if(m_zoom_in_timer < ZOOM_OUT_DURATION) {

				// ズームアウト中だったら、同じ位置からズームインに切り替わるよう、.
				// タイマーを変換する.

				float	rate;

				// タイマーからズーム率を求める.
				rate = m_zoom_in_timer/ZOOM_OUT_DURATION;
				rate = Mathf.Clamp01(rate);
				rate = Mathf.Sin(rate*Mathf.PI/2.0f);
				rate = Mathf.Pow(rate, 0.5f);
				rate = 1.0f - rate;

				// ズーム率から、ズームインのときのタイマーの値に逆変換する.
				rate = Mathf.Pow(rate, 1.0f/0.3f);
				rate = Mathf.Asin(rate)/(Mathf.PI/2.0f);
				m_zoom_in_timer = ZOOM_IN_DURATION*rate;


			} else {

				m_zoom_in_timer = 0.0f;
			}
		}
#endif
	}
	
	public void OnEndAttack()
	{
#if false
		m_zoom_in = false;
		m_zoom_in_timer = 0.0f;
#else
	// ズームインの途中でズームアウトが始まっても、大丈夫なコード.
	// 少し難しいので、最初はわからなくても気にしないでください.
		if(!m_zoom_in) {

		} else {

			m_zoom_in = false;

			if(m_zoom_in_timer < ZOOM_IN_DURATION) {

				float	rate;

				rate = m_zoom_in_timer/ZOOM_IN_DURATION;
				rate = Mathf.Clamp01(rate);
				rate = Mathf.Sin(rate*Mathf.PI/2.0f);
				rate = Mathf.Pow(rate, 0.3f);

				rate = 1.0f - rate;
				rate = Mathf.Pow(rate, 1.0f/0.5f);
				rate = Mathf.Asin(rate)/(Mathf.PI/2.0f);
				m_zoom_in_timer = ZOOM_OUT_DURATION*rate;

			} else {

				m_zoom_in_timer = 0.0f;
			}
		}

#endif

	}
}
