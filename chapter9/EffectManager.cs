using UnityEngine;
using System.Collections;

public class EffectManager : MonoBehaviour {

	public GameObject 		appearEffectPrefab       = null;		// 表示開始時に表示するエフェクト.
	public GameObject 		landingWaterEffectPrefab = null;		// 着水時に表示するエフェクト.
	public GameObject		fightingEffectPrefab     = null;		// 戦闘の時に表示するエフェクト.

	protected GameObject	fighting_effect = null;					// 表示中の、先頭エフェクト.

	// ================================================================ //
	// MonoBehaviour からの継承.

	void	Start()
	{
	}
	
	void	Update()
	{
	}

	// 表示開始時に表示するエフェクトを再生.
	public void		playAppearEffect(BaseObject bo)
	{
		ParticleSystem	ps = GameObject.Instantiate(this.appearEffectPrefab).GetComponent<ParticleSystem>();


		// キャラクターのちょっと手前に表示させるための計算.
		Vector3		bo_center = bo.transform.position + 0.5f*(bo.getYTop() + bo.getYBottom())*Vector3.up;

		Quaternion	qt = Quaternion.AngleAxis(Camera.main.transform.eulerAngles.x, Vector3.right);
		Ray 	ray = new Ray(bo_center, qt*-Vector3.forward);

		// 再生.
		ps.transform.position = ray.GetPoint(100.0f);
		ps.Play();

		// 再生終了時にエフェクトの GameObject は削除.
		GameObject.Destroy(ps.gameObject, 1.0f);
	}

	// 着水時に表示するエフェクトを再生.
	public void		playLandingWaterEffect(BaseObject bo)
	{
		ParticleSystem	ps = GameObject.Instantiate(this.landingWaterEffectPrefab).GetComponent<ParticleSystem>();

		Vector3 position = bo.transform.position;

		// 水面と同じ高さになるように.
		position.y = 70.0f;
		// 水柱がキャラに隠れないよう少し前に出す.
		position.z -= 70.0f;

		// 再生.
		ps.transform.position = position;
		ps.Play();

		// 再生終了時にエフェクトの GameObject は削除.
		GameObject.Destroy(ps.gameObject, 1.0f);
	}

	// 戦闘の時に表示するエフェクトを再生.
	public void		playFightingEffect(Vector3 position)
	{
		this.fighting_effect = GameObject.Instantiate(this.fightingEffectPrefab);

		this.fighting_effect.transform.position = position;
	}

	// 戦闘の時に表示するエフェクトを停止.
	public void		stopFightingEffect()
	{
		if(this.fighting_effect != null) {

			GameObject.Destroy(this.fighting_effect);
		}
	}

	// ================================================================ //
	// インスタンス.

	protected static EffectManager	instance = null;

	public static EffectManager	get()
	{
		if(instance == null) {

			GameObject	go = GameObject.FindGameObjectWithTag("System");

			if(go == null) {

				Debug.Log("Can't find \"System\" GameObject.");

			} else {

				instance = go.GetComponent<EffectManager>();
			}
		}
		return(instance);
	}

}
