using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CarWheels {
	
	public class Wheel {
	
		public WheelCollider	collider;							// コライダー.
	
		public GameObject		tire_model;							// タイヤのモデル.
	
		public bool				is_front = false;		// 前輪？.
		public bool				is_steer = false;		// ステアリング輪？.
		public bool				is_drive = false;		// ドライブ輪？.
	
		public WheelFrictionCurve	fc_forward;
		public WheelFrictionCurve	fc_sideways;
	
		public Vector3			wheelVelo   = Vector3.zero;
		public Vector3			groundSpeed = Vector3.zero;
	}
	public List<Wheel>	wheels = new List<Wheel>();

	// ---------------------------------------------------------------- //

	protected CarControl	car;

	public float	side_slip_rate = 0.0f;

	protected const float	TIRE_LOCAL_Y_MAX = 0.0f;		// タイヤモデルのローカル Y 座標の最大値（ボディから飛び出ないようにするため）.

	// ================================================================ //

	public CarWheels(CarControl car)
	{
		this.car = car;
	}

	// タイヤのセットアップ（全部）.
	public void	setupWheels()
	{
		foreach(GameObject wheel in this.car.front_wheels) {

			this.wheels.Add(this.setup_wheel(wheel, true));
		}
		
		foreach(GameObject wheel in this.car.rear_wheels) {

			this.wheels.Add(this.setup_wheel(wheel, false));
		}
		
	}

	public void		execute()
	{
		// パラメーターを全部のホイールにコピー.
		// （デバッグ時の調整用）.
		foreach(Wheel wc in this.wheels) {

			JointSpring		suspension_spring = wc.collider.suspensionSpring;
		
			suspension_spring.spring = this.car.suspension_spring;
			suspension_spring.damper = this.car.suspension_damper;

			wc.collider.suspensionSpring = suspension_spring;

			wc.collider.mass             = this.car.wheel_mass;
			wc.collider.wheelDampingRate = this.car.wheel_damping_rate;
		}
	}

	// エンジン、ブレーキのパワーをコライダーに反映する.
	public void		applyEngineAndBrakePower()
	{
		float	engine = this.car.engine_power;

		if(this.car.input.is_reverse) {

			engine *= -1.0f;
		}


		foreach(Wheel wheel in this.wheels) {

			if(wheel.is_drive) {

				wheel.collider.motorTorque = engine;

			} else {

				wheel.collider.motorTorque = 0.0f;
			}

			if(wheel.is_front) {

				wheel.collider.brakeTorque = this.car.hand_brake_power;

			} else {

				wheel.collider.brakeTorque = this.car.brake_power;
			}
		}
	}

	// ステアリングをコライダーに反映する.
	public void		applySteering()
	{
		float	angle = this.car.input.steer*30.0f;

		foreach(Wheel wheel in this.wheels) {

			if(!wheel.is_steer) {

				continue;
			}

			if(wheel.is_front) {

				wheel.collider.steerAngle =  angle;

			} else {

				wheel.collider.steerAngle = -angle;
			}
		}
	}

	// タイヤの摩擦のコントロール.
	public void		updateFriction()
	{
		this.side_slip_rate = Mathf.Pow(this.car.relative_velocity.x, 2.0f)*0.1f;

		float	extremum_rate  = Mathf.Lerp(1.0f, 0.9f, this.side_slip_rate);
		float	asymptote_rate = Mathf.Lerp(1.0f, 0.9f, this.side_slip_rate);

		foreach(Wheel w in this.wheels) {

			WheelFrictionCurve	fc_sideways = w.fc_sideways;

			if(!w.is_front) {
	
				fc_sideways.extremumValue  *= extremum_rate;
				fc_sideways.asymptoteValue *= asymptote_rate;
			}

			w.collider.sidewaysFriction = fc_sideways;
		}
	}

	// タイヤモデル.
	public void		tireModelControl()
	{
		foreach(var wheel in this.wheels) {

			WheelCollider	wc = wheel.collider;

			Vector3		position;
			Quaternion	rotation;

			wc.GetWorldPose(out position, out rotation);

			wheel.tire_model.transform.position = position;
			wheel.tire_model.transform.rotation = rotation;

			// タイヤモデルがボディから飛び出ないよう、上に上がりすぎないようにする.
			Vector3		local_position = wheel.tire_model.transform.localPosition;

			if(local_position.y > TIRE_LOCAL_Y_MAX) {

				local_position.y = TIRE_LOCAL_Y_MAX;
				wheel.tire_model.transform.localPosition = local_position;
			}
		}
	}


	// ---------------------------------------------------------------- //

	// タイヤのセットアップ.
	protected Wheel		setup_wheel(GameObject wheel_go, bool is_front)
	{
		WheelCollider	wc = wheel_go.GetComponentInChildren<WheelCollider>();

		// ホイールコライダーとタイヤモデルの位置が一致するよう、サスペンションの根元の位置をあげる.
		wc.transform.position += Vector3.up*(wc.suspensionDistance - wc.suspensionSpring.targetPosition);

		Wheel	wheel = new Wheel(); 

		wheel.collider   = wc;
		wheel.tire_model = wheel_go.transform.FindChild("tire_model").gameObject;

		wheel.is_front =  is_front;
		wheel.is_steer =  is_front;
		wheel.is_drive = !is_front;

		wheel.fc_forward  = wc.forwardFriction;
		wheel.fc_sideways = wc.sidewaysFriction;

		return(wheel);
	}
}