// Unity 3.5 の Car Tutorial を移植、改造したものです.
// 一部、移植が終わっていない個所があります m(_ _)m.
// スミマセン……。.

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CarControl : MonoBehaviour {

	public GameObject[]			front_wheels;
	public GameObject[]			rear_wheels;

	public GameObject			centerOfMass;
	
	protected const float	KPH_TO_MPS = 1000.0f/3600.0f;		// [km/hour] -> [m/sec].
	protected const int		GEAR_COUNT = 5;
	protected const float	TOP_SPEED  = 160.0f*KPH_TO_MPS;


	// ---------------------------------------------------------------- //

	[System.NonSerialized]
	public float		suspension_spring  = 35000.0f;		// suspensionSpring.spring
	[System.NonSerialized]
	public float		suspension_damper  = 4500.0f;		// suspensionSpring.damper
	[System.NonSerialized]
	public float		wheel_mass         = 20.0f;			// WheelCollider.mass
	[System.NonSerialized]
	public float		wheel_damping_rate = 10.0f;			// WheelCollider.wheelDampingRate

	protected float		brake_power_max      = 1000.0f;		// ブレーキ踏んだ時のブレーキ.
	protected float		hand_brake_power_max = 1000.0f;		// ハンドブレーキのパワー.

	protected float		engine_power_scale   = 1000.0f;		// エンジンパワーにかける倍率.

	protected Rigidbody		car_rigidbody;

	// ---------------------------------------------------------------- //

	public CarWheels			wheels;
	public CarInput				input;
	protected mpiCamera			mpi_camera;
	protected CarSoundSqueal	squeal_sound;

	public Vector3		relative_velocity = Vector3.zero;	// （ローテーションのみ、車ローカル）.

	protected bool		is_can_steer = true;
	protected bool		is_can_drive = true;

	public float 		engine_power     = 0.0f;
	public float 		brake_power      = 0.0f;
	public float 		hand_brake_power = 0.0f;
	protected int		current_gear     = 0;

	protected Vector3	previous_velocity = Vector3.zero;
	protected float		turn_speed = 0.0f;					// [degree/sec]

	protected float[]	top_speed_of_gears;			// ギアーごとの最高速度.
	protected float[]	engine_power_of_gears;		// ギアーごとのエンジンパワー.

	protected float		initialDragMultiplierX = 10.0f;
	protected Vector3	dragMultiplier = new Vector3(2.0f, 5.0f, 1.0f);

	protected float		handbrakeXDragFactor = 0.5f;

	protected float		flying_timer = 0.0f;		// ジャンプ中タイマー.
	protected bool		is_flying    = false;		// ジャンプ中？.

	protected const float	RESET_TIME  = 5.0f;
	protected float			reset_timer = 0.0f;

	// ================================================================ //
	// MonoBehaviour からの継承.

	void	Awake()
	{
		this.input         = new CarInput(this);
		this.wheels        = new CarWheels(this);
		this.squeal_sound  = this.gameObject.GetComponent<CarSoundSqueal>();
		this.car_rigidbody = this.gameObject.GetComponent<Rigidbody>();
	}
	
	void 	Start()
	{
		this.mpi_camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<mpiCamera>();

		if(this.mpi_camera != null) {

			if(this.mpi_camera.enabled) {
	
				mpiCamera.Posture	posture = this.mpi_camera.getPosture();
	
				posture.interest = this.transform.position;
	
				this.mpi_camera.setPosture(posture);
			}
		}

		// タイヤのセットアップ.
		this.wheels.setupWheels();
		
		// 重心位置のセットアップ.
		this.setup_center_of_mass();

		// ギアー（ミッション）のセットアップ.	
		this.setup_gears();
		
		this.initialDragMultiplierX = this.dragMultiplier.x;
	}

	void 	Update()
	{
		// 車の前方向を +Z とする速度.
		// （ローテーションのみ、車ローカル）.
		this.relative_velocity = this.transform.InverseTransformDirection(this.car_rigidbody.velocity);

		this.wheels.execute();		
		this.input.execute();
		
		this.check_is_flipped();
	
		// タイヤモデル.	
		this.wheels.tireModelControl();

		// ミッションの制御.
		this.mission_control();

#if false
		float	speed_kph = this.car_rigidbody.velocity.magnitude*3600.0f/1000.0f;
		
		dbPrint.setLocate(10, 5);
		dbPrint.print("speed "        + speed_kph.ToString("F2"));

		//dbPrint.print("velocity   "   + this.relative_velocity);
		//dbPrint.print("turn       "   + this.turn_speed.ToString("F2"));
		//dbPrint.print("rel velo z "           + this.relative_velocity.z);
		//dbPrint.print("throttle "           + this.throttle);
		//dbPrint.print("engine_power " + this.engine_power.ToString("0000000.00"));
		//dbPrint.print("brake_power  " + this.brake_power.ToString("F2"));
		//dbPrint.print("current_accel "     + this.current_accel);
		//dbPrint.print("current_gear  " + this.current_gear);
		//dbPrint.print("side_slip     " + this.wheels.side_slip_rate);
		//dbPrint.print("reverse_timer " + this.input.gear_reverse_timer);
		//dbPrint.print("reverse       " + this.input.is_reverse);
		//dbPrint.print("is_flying " + this.is_flying);
		//dbPrint.print("flying_timer " + this.flying_timer);
		//foreach(var wc in this.wheels.wheels) {

			//dbPrint.print(wc.collider.rpm);
			//dbPrint.print(wc.groundSpeed.x);
		//}

		//dbPrint.print(this.relative_velocity.z);
		//for(int i = 0; i < GEAR_COUNT;i++) {

			//dbPrint.print(this.top_speed_of_gears[i]);
		//}
#endif
	}

	void	LateUpdate()
	{
		if(this.mpi_camera != null) {

			if(this.mpi_camera.enabled) {
	
				this.mpi_camera.parallelInterestTo(this.transform.position);
			}
		}

		// 空中でひっくり返ってしまわないよう、姿勢を無理やり補正する.
		//
		// （物理計算の後にやらないと効果がないみたいなので、Update() ではなく、
		// 　LateUpdate() でやってます）.
		//
		if(this.is_flying) {
			
			Vector3		front;
			Vector3		front_xz;
			
			front = this.transform.rotation*Vector3.forward;
			front.Normalize();
			
			front_xz = front;
			front_xz.y = this.car_rigidbody.velocity.y/200.0f;		// 適当に上下に向くように.
			front_xz.Normalize();
			
			if(front_xz.magnitude > 0.01f) {
				
				Quaternion	q = Quaternion.LookRotation(front_xz, Vector3.up);
				
				float	rate;
				
				// 徐々に強制力が強くなる.
				rate = this.flying_timer/3.0f;
				rate = Mathf.Min(rate, 1.0f);
				
				this.transform.rotation = Quaternion.Lerp(this.transform.rotation, q, rate);
			}
		}
	
	}

	void	FixedUpdate()
	{
		this.relative_velocity = this.transform.InverseTransformDirection(this.car_rigidbody.velocity);
		
		// 接地してる？　をチェックする.
		this.check_ground_contact();	

		// タイヤの摩擦のコントロール.
		this.wheels.updateFriction();
		
		//this.drag_control();
		
		// アクセル、ブレーキのパワーを更新する.
		this.update_engine_brake_power();

		// エンジン、ブレーキのパワーをタイヤに反映する.
		if(this.is_can_drive) {

			this.wheels.applyEngineAndBrakePower();
		}

		// ステアリングをコライダーに反映する.
		if(this.is_can_steer) {
	
			this.wheels.applySteering();
		}
	
		// ジャンプ中？　チェック.
		this.check_is_flying();
	
		if(Input.GetKeyDown(KeyCode.R)) {

			this.reset();
		}

		this.turn_speed = Vector3.Angle(this.car_rigidbody.velocity, this.previous_velocity)/Time.fixedDeltaTime;

		this.previous_velocity = this.car_rigidbody.velocity;
	}

	// ================================================================ //

	// ミッションのコントロール.
	protected void	mission_control()
	{
		this.current_gear = 0;

		for(int i = 0; i < GEAR_COUNT - 1;i++) {

			if(this.relative_velocity.z > this.top_speed_of_gears[i]) {

				this.current_gear = i + 1;
			}
		}
	}

	// 接地してる？　をチェックする.
	protected void	check_ground_contact()
	{
		this.is_can_drive = false;
		this.is_can_steer = false;
		
		foreach(CarWheels.Wheel w in this.wheels.wheels) {

			if(w == null) {
				
				continue;
			}
			if(!w.collider.isGrounded) {

				continue;
			}

			if(w.is_steer) {

				this.is_can_steer = true;
			}
			if(w.is_drive) {

				this.is_can_drive = true;
			}
		}
	}

	// アクセル、ブレーキのパワーを更新する.
	protected void	update_engine_brake_power()
	{
		if(this.input.throttle > 0.0f) {

			float	norm_power = (this.engine_power/this.engine_power_of_gears[this.engine_power_of_gears.Length - 1])*2.0f;

			this.engine_power += Time.deltaTime*200.0f*this.evaluate_norm_power(norm_power)*this.engine_power_scale;

		} else {

			this.engine_power -= Time.deltaTime*200.0f*this.engine_power_scale;
		}

		if(this.input.brake > 0.0f) {

			this.brake_power = this.brake_power_max;

		} else {

			if(this.input.throttle > 0.0f) {

				this.brake_power = 0.0f;
			}
		}

		float	power_min = 0.0f;
		float	power_max = this.engine_power_of_gears[this.current_gear]*this.engine_power_scale;

		this.engine_power = Mathf.Clamp(this.engine_power, power_min, power_max)/**1000.0f*/;

		//

		// ハンドブレーキ.
		if(this.input.hand_brake > 0.0f) {

			this.hand_brake_power = this.hand_brake_power_max;

		} else {

			this.hand_brake_power = 0.0f;
		}
	}

	// よくわからない.
	protected float		evaluate_norm_power(float norm_power)
	{
		float	ret = 1.0f;

		if(norm_power < 1.0f ) {

			ret = 10.0f - norm_power*9.0f;

		} else {

			ret = 1.9f - norm_power*0.9f;
		}

		return(ret);
	}

	// 空気抵抗.
	// よくわからない.
	protected void	drag_control()
	{
		Vector3		relativeDrag = new Vector3(	 -this.relative_velocity.x*Mathf.Abs(this.relative_velocity.x), 
		                                         -this.relative_velocity.y*Mathf.Abs(this.relative_velocity.y), 
		                                         -this.relative_velocity.z*Mathf.Abs(this.relative_velocity.z) );
		
		Vector3	drag = Vector3.Scale(dragMultiplier, relativeDrag);

		/*if(this.initialDragMultiplierX > this.dragMultiplier.x) {
			
			// Handbrake code

			drag.x /= (this.relative_velocity.magnitude/(TOP_SPEED/(1.0f + 2.0f*this.handbrakeXDragFactor)));
			drag.z *= (1.0f + Mathf.Abs(Vector3.Dot(this.car_rigidbody.velocity.normalized, this.transform.forward)));

			drag += this.car_rigidbody.velocity*Mathf.Clamp01(this.car_rigidbody.velocity.magnitude/TOP_SPEED);

		} else {

			 // No handbrake
			drag.x *= TOP_SPEED/this.relative_velocity.magnitude;
		}*/
		
		/*if(Mathf.Abs(relativeVelocity.x) < 5 && !handbrake) {

			drag.x = -relativeVelocity.x * dragMultiplier.x;
		}*/

		// ジャンプ中は空気抵抗を減らす.
		if(this.is_flying) {
			
			float	rate;
			float	scale;
			
			rate = Mathf.InverseLerp(0.0f, 1.0f, this.flying_timer);
			rate = Mathf.Clamp01(rate);
			
			scale = Mathf.Lerp(1.0f, 0.1f, rate);
			
			drag *= scale;
		}

		this.car_rigidbody.AddForce(this.transform.TransformDirection(drag)*this.car_rigidbody.mass*Time.deltaTime);
	}

	// ひっくり返った？　チェック.
	protected void	check_is_flipped()
	{
		if(this.transform.localEulerAngles.z > 80.0f && this.transform.localEulerAngles.z < 280.0f) {

			this.reset_timer += Time.deltaTime;

		} else {

			this.reset_timer = 0.0f;
		}
		
		if(this.reset_timer > RESET_TIME) {

			this.reset();
			this.reset_timer = 0.0f;
		}
	}

	// リセットする.
	protected void	reset()
	{
		this.transform.rotation = Quaternion.LookRotation(this.transform.forward);
		this.transform.position += Vector3.up*0.5f;

		this.car_rigidbody.velocity        = Vector3.zero;
		this.car_rigidbody.angularVelocity = Vector3.zero;

		this.engine_power = 0.0f;
	}

	// ジャンプ中？　チェック.
	protected void	check_is_flying()
	{
		this.is_flying = true;
		
		// 四輪すべてが浮いていたら、ジャンプ中とする.
		foreach(var w in this.wheels.wheels) {
			
			if(w == null) {
				
				continue;
			}
			if(w.collider.isGrounded) {
				
				this.is_flying = false;
			}
		}
		
		if(this.is_flying) {
			
			this.flying_timer += Time.deltaTime;
			
		} else {
			
			this.flying_timer = 0.0f;
		}
	}

	// ================================================================ //

	// 重心位置のセットアップ.
	protected void	setup_center_of_mass()
	{
		if(this.centerOfMass != null) {

			this.GetComponent<Rigidbody>().centerOfMass = this.centerOfMass.transform.localPosition;
		}
	}

	// ギアー（ミッション）のセットアップ.	
	protected void	setup_gears()
	{
		this.engine_power_of_gears  = new float[GEAR_COUNT];
		this.top_speed_of_gears     = new float[GEAR_COUNT];
		
		float	temp_top_speed = TOP_SPEED;
		
		for(int i = 0;i < GEAR_COUNT;i++) {

			if(i > 0) {

				this.top_speed_of_gears[i] = temp_top_speed/4.0f + this.top_speed_of_gears[i - 1];

			} else {

				this.top_speed_of_gears[i] = temp_top_speed/4.0f;
			}

			temp_top_speed *= 3.0f/4.0f;
		}
		
		float	engine_factor = TOP_SPEED/this.top_speed_of_gears[this.top_speed_of_gears.Length - 1];
		
		for(int i = 0;i < GEAR_COUNT;i++) {

			float 	max_linear_drag = this.top_speed_of_gears[i]*this.top_speed_of_gears[i];// * dragMultiplier.z;

			this.engine_power_of_gears[i] = max_linear_drag*engine_factor;
		}
	}
	
	
	// ================================================================ //

}
