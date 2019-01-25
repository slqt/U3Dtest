using UnityEngine;
using System.Collections;

public class CarSoundControl : MonoBehaviour {

	public AudioClip	audio_clip_engine;			// エンジン音（ループ）.
	public AudioClip	audio_clip_hit_wall;		// 壁ヒット音.
	public AudioClip	audio_clip_landing;			// 着地音（ジャンプ後）.

	public AudioSource	audio_engine;

	protected bool	is_contact_wall = false;		// 壁ヒットした？.
	protected float	wall_hit_timer = 0.0f;			// 壁ヒット後タイマー.
	protected float	hit_speed_wall = 0.0f;			// 壁にヒットしたスピード.

	protected bool	is_landing = false;				// 着地した？.
	protected float	landing_timer = 0.0f;			// 着地後タイマー.
	protected float	landing_speed = 0.0f;			// 着地したスピード.

	// ================================================================ //
	// MonoBehaviour からの継承.

	void 	Start()
	{

		this.audio_engine = this.gameObject.AddComponent<AudioSource>();

		this.audio_engine.clip = this.audio_clip_engine;
		this.audio_engine.loop = true;
		this.audio_engine.Play();
	}
	
	void 	Update()
	{
	
		// スピードに応じてピッチを上げる.
	
		float		rate;
		float		pitch;
	
		float		speed0 = 0.0f;
		float		speed1 = 60.0f;
	
		float		pitch0 = 1.0f;
		float		pitch1 = 2.0f;
	
		rate = Mathf.InverseLerp(speed0, speed1, this.GetComponent<Rigidbody>().velocity.magnitude);
		rate = Mathf.Clamp01(rate);
	
		pitch = Mathf.Lerp(pitch0, pitch1, rate);
	
		this.audio_engine.pitch = pitch;

		//

		// 壁ヒット音の制御.
		this.wall_hit_control();

		// 着地音の制御.
		this.landing_control();

		//

		this.is_contact_wall = false;
		this.is_landing = false;
	}

	// ================================================================ //

	// 壁ヒット音の制御.
	private void	wall_hit_control()
	{
		if(this.wall_hit_timer > 0.0f) {

			this.wall_hit_timer -= Time.deltaTime;

		} else {

			if(this.is_contact_wall) {
	
				float		speed0 = 1.0f;
				float		speed1 = 10.0f;
				float		rate;
				float		volume;
	
				rate = Mathf.InverseLerp(speed0, speed1, this.hit_speed_wall);
				rate = Mathf.Clamp01(rate);
	
				volume = Mathf.Lerp(0.1f, 1.0f, rate);
	
				this.GetComponent<AudioSource>().volume = volume;
				this.GetComponent<AudioSource>().PlayOneShot(this.audio_clip_hit_wall);

				this.wall_hit_timer = 1.0f;

				//Debug.Log("speed " + this.hit_speed_wall.ToString() + ":vol " + volume);

			} else {

				this.wall_hit_timer = 0.0f;
			}
		}

	}

	private static float	LANDING_SPEED_MIN = 1.0f;			// 着地音が最小になる着地スピード.
	private static float	LANDING_SPEED_MAX = 5.0f;			// 着地音が最大になる着地スピード.

	// 着地音の制御.
	private void	landing_control()
	{
		if(this.landing_timer > 0.0f) {

			this.landing_timer -= Time.deltaTime;

		} else {

			bool	is_play_sound = false;

			do {

				if(!this.is_landing) {

					break;
				}
				if(this.landing_speed < LANDING_SPEED_MIN) {

					break;
				}

				is_play_sound = true;

			} while(false);

			if(is_play_sound) {

				float		speed0 = LANDING_SPEED_MIN;
				float		speed1 = LANDING_SPEED_MAX;
				float		rate;
				float		volume;
	
				rate = Mathf.InverseLerp(speed0, speed1, this.landing_speed);
				rate = Mathf.Clamp01(rate);
	
				volume = Mathf.Lerp(0.5f, 1.0f, rate);
	
				this.GetComponent<AudioSource>().volume = volume;
				this.GetComponent<AudioSource>().PlayOneShot(this.audio_clip_landing);

				this.landing_timer = 1.0f;

				//Debug.Log("speed " + this.landing_speed.ToString() + ":vol " + volume);

			} else {

				this.landing_timer = 0.0f;
			}
		}

	}

	// OnCollisionEnter() だけでは拾えないことが多いので、OnCollisionStay()
	// でもやる.
	void 	OnCollisionEnter(Collision other)
	{
		this.collision_common(other);
	}
	void 	OnCollisionStay(Collision other)
	{
		this.collision_common(other);
	}

	private void	collision_common(Collision other)
	{
		foreach(var contact in other.contacts) {

			float	d = Vector3.Dot(contact.normal, Vector3.up);

			// ヒットしたポリゴンの法線が水平に近かったら、壁とみなす.
			//
			if(Mathf.Cos(80.0f*Mathf.Deg2Rad) > Mathf.Abs(d)) {

				this.is_contact_wall = true;

				// this.rigidbody.velocity は押し出し後の速度（？）っぽい.
				// 「あたる直前」の速度ではない.
				// other.relativeVelocity を使う.

				this.hit_speed_wall = Vector3.Dot(contact.normal, other.relativeVelocity);

				if(this.hit_speed_wall < 0.0f) {

					this.hit_speed_wall = 0.0f;
				}
			}

			// ヒットしたポリゴンの法線が垂直に近かったら、地面とみなす.
			//
			if(Mathf.Cos(10.0f*Mathf.Deg2Rad) < Mathf.Abs(d)) {

				this.is_landing = true;

				this.landing_speed = Vector3.Dot(contact.normal, other.relativeVelocity);

				if(this.landing_speed < 0.0f) {

					this.landing_speed = 0.0f;
				}
			}
		}

	}
}
