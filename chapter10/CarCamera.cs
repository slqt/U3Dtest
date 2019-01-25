using UnityEngine;
using System.Collections;

public class CarCamera : MonoBehaviour
{
	public Transform	target = null;

	public float		height          = 1.0f;
	public float		positionDamping = 30.0f;
	public float		velocityDamping = 30.0f;
	public float		distance        = 4.0f;

	public LayerMask	ignoreLayers = -1;

	// ---------------------------------------------------------------- //

	private RaycastHit	hit = new RaycastHit();

	private Vector3		prev_velocity = Vector3.zero;
	private LayerMask	raycastLayers = -1;
	
	private Vector3		current_velocity = Vector3.zero;

	protected const float	USE_DIR_START_SPEED = 1.0f;
	protected const float	USE_DIR_END_SPEED   = 0.01f;
	
	// ================================================================ //
	// MonoBehaviour からの継承.

	void 	Start()
	{
		raycastLayers = ~ignoreLayers;
	}

	void 	FixedUpdate()
	{
		Vector3		target_velocity;

		target_velocity = target.root.GetComponent<Rigidbody>().velocity;

		// 速度は大きさが０に近い時に向きが不安定になるようなので、
		// 代わりに車の向きを使う.
		if(target_velocity.magnitude <= USE_DIR_START_SPEED) {

			float	rate = Mathf.InverseLerp(USE_DIR_START_SPEED, USE_DIR_END_SPEED, target_velocity.magnitude);

			rate = Mathf.Clamp01(rate);
			rate = Mathf.Lerp(-Mathf.PI/2.0f, Mathf.PI/2.0f, rate);
			rate = Mathf.Sin(rate);
			rate = Mathf.InverseLerp(-1.0f, 1.0f, rate);

			if(Vector3.Dot(target_velocity, this.target.forward) >= 0.0f) {
	
				target_velocity = Vector3.Lerp(target_velocity.normalized, this.target.forward, rate);

			} else {

				target_velocity = Vector3.Lerp(target_velocity.normalized, -this.target.forward, rate);
			}
		}

		target_velocity.Normalize();

		// バッグで走っているときに、カメラが車の前に回り込んでしまわないように.
		//
		if(Vector3.Dot(target_velocity, this.target.forward) < 0.0f) {

			target_velocity = -target_velocity;
		}

		if(Vector3.Angle(target_velocity, this.target.forward) > 90.0f) {

			target_velocity = this.prev_velocity;
		}

		this.current_velocity   = Vector3.Lerp(this.prev_velocity, target_velocity, this.velocityDamping*Time.deltaTime);
		this.current_velocity.y = 0;
		this.prev_velocity = this.current_velocity;

		this.current_velocity = this.current_velocity.normalized;

	}
	
	void 	LateUpdate()
	{
		this.calcPosture();
	}

	// ================================================================ //

	public void	calcPosture()
	{
		float	speed_factor = Mathf.Clamp01(target.root.GetComponent<Rigidbody>().velocity.magnitude/70.0f);

		// パース.
		GetComponent<Camera>().fieldOfView = Mathf.Lerp(55.0f, 72.0f, speed_factor);

		// 視点-注視点の距離.
		float		distance = Mathf.Lerp(7.5f, 6.5f, speed_factor);

		Vector3		interest = this.target.position + Vector3.up*height;
		Vector3		eye      = interest - (this.current_velocity*distance);

		eye.y = interest.y + 2.0f;
		
		Vector3		eye_vector_reverse = eye - interest;

		if(Physics.Raycast(interest, eye_vector_reverse, out hit, distance, raycastLayers)) {

			eye = hit.point;
		}

		transform.position = eye;
		transform.LookAt(interest);
	}

	// リセット（車　生成直後に呼ぶ）.
	public void	reset()
	{
		// 車生成直後は rigidbody.velocity が０なので、かわりに rotation を
		// 使ってカメラの向きを求める.
		//
		this.current_velocity   = this.target.TransformDirection(Vector3.forward);
		this.current_velocity.y = 0.0f;

		this.prev_velocity = this.current_velocity;
	}


	public void	setEnable(bool sw)
	{
		this.enabled = sw;
	}
}
