using UnityEngine;
using System.Collections;

// 木を並べる.
public class ForestCreator {

	public RoadCreator		road_creator = null;
	public GameObject		main_camera;
	public ToolGUI			tool_gui = null;

	// 入力.

	public GameObject		TreePrefab = null;
	public float			start;
	public float			end;

	public float			place_max;

	//

	public bool				is_created = false;

	// ---------------------------------------------------------------- //

	// ルート.
	public GameObject	root_r = null;			// 右側用.
	public GameObject	root_l = null;			// 左側用.

	// ---------------------------------------------------------------- //

	// ベースライン.
	public class BaseLine {

		public Vector3[]	points;				// 制御点.
		public float		total_distance;
	};

	protected BaseLine	base_line_r;			// 右側用.
	protected BaseLine	base_line_l;			// 左側用.

	// ---------------------------------------------------------------- //

	public float	fluc_amplitude   = 10.0f;		// ベースラインが蛇行する幅.
	public float	fluc_cycle       = 100.0f;		// ベースラインが蛇行する周期.
	public float	base_offset      = 30.0f;		// 道路の中心からベースラインまでのオフセット.
	public float	base_pitch       = 20.0f;		// 木同士の間隔（一番せまいところ）.
	public float	max_pitch_factor = 5.0f;		// 木同士の間隔（一番広いところ。倍率）.

	protected UIIcon	start_icon;
	protected UIIcon	end_icon;

	// ---------------------------------------------------------------- //
	
	public void		create()
	{
		this.start_icon = this.tool_gui.createForestIcon();
		this.end_icon = this.tool_gui.createForestIcon();

		this.start_icon.setVisible(false);
		this.end_icon.setVisible(false);
	}

	public void		execute()
	{
		if(this.is_created) {

			this.draw_base_line(this.base_line_r);
			this.draw_base_line(this.base_line_l);
		}
	}

	// ベースラインを描画する（デバッグ用）.
	public void		draw_base_line(BaseLine base_line)
	{
		for(int i = 0;i < base_line.points.Length - 1;i++) {

			Debug.DrawLine(base_line.points[i], base_line.points[i + 1], Color.red, 0.0f, false);
		}
	}

	public void		createForest()
	{
		//

		if(this.start > this.end) {

			float	temp = this.start;
			this.start = this.end;
			this.end   = temp;
		}

		// 親になるオブジェクトを作成しておく.

		this.root_r = new GameObject();
		this.root_l = new GameObject();

		this.root_r.name = "Trees(right)";
		this.root_l.name = "Trees(left)";

		//

		this.base_line_r = new BaseLine();
		this.base_line_l = new BaseLine();

		this.base_line_r.points = new  Vector3[(int)this.end - (int)this.start + 1];
		this.base_line_l.points = new  Vector3[(int)this.end - (int)this.start + 1];

		// 右側.

		this.create_base_line(this.base_line_r, (int)this.start, (int)this.end,  this.base_offset, this.fluc_amplitude, this.fluc_cycle);
		this.create_tree_on_line(this.root_r, this.base_line_r);

		// 左側.

		this.create_base_line(this.base_line_l, (int)this.start, (int)this.end, -this.base_offset, this.fluc_amplitude, this.fluc_cycle);
		this.create_tree_on_line(this.root_l, this.base_line_l);

		//

		this.is_created = true;
	}

	// ベースラインを生成する.
	public void		create_base_line(BaseLine base_line, int start, int end, float base_offset, float fluc_amp, float fluc_cycle)
	{
		int		n = 0;
		float	offset;

		// 道路の中心線上の道のり.
		float	center_distance = 0.0f;
		
		// 道路の断面.
		RoadCreator.Section[]	sections = this.road_creator.sections;

		// ベースライン上の道のり.
		base_line.total_distance = 0.0f;

		for(int i = start;i <= end;i++) {

			// 道路の中心線上の道のり.
			//
			if(i > start) {

				center_distance += (sections[i].center - sections[i - 1].center).magnitude;
			}

			// -------------------------------------------- //
			// 道路と直交する方向のオフセットを求める.

			offset = base_offset;

			// サイン波になるように.

			float	angle = Mathf.Repeat(center_distance, fluc_cycle)/fluc_cycle*Mathf.PI*2.0f;

			offset += fluc_amp*Mathf.Sin(angle);

			// -------------------------------------------- //

			Vector3	point         = sections[i].center;
			Vector3	offset_vector = sections[i].right;

			point += offset*offset_vector;

			base_line.points[n] = point;

			// ベースライン上の道のり.
			//
			if(n > 0) {

				base_line.total_distance += Vector3.Distance(base_line.points[n], base_line.points[n - 1]);
			}

			//

			n++;
		}
	}

	public const float	FADE_LENGTH_SCALE = 0.25f;		// フェードイン/フェードアウトする長さ（全体に対する割合）.

	// ベースライン上に木を生成する.
	public void		create_tree_on_line(GameObject root, BaseLine base_line)
	{
		float		rate;
		float		pitch = 0.0f;

		float		distance_local = 0.0f;
		Vector3		point_previous = base_line.points[0];
		float		current_distance = 0.0f;
		int			instance_count = 0;
		int			instance_num_max;

		// 木の間隔（最大値）.
		float		max_pitch = this.base_pitch*this.max_pitch_factor;

		// ベースライン上に木を並べる.
		foreach(Vector3 point in base_line.points) {

			Vector3	dir      = point - point_previous;		// 区間の向き.
			float	distance = dir.magnitude;				// 区間の長さ.

			// 正規化に失敗した（＝大きさが０だった）ときは、zero になる.
			dir.Normalize();

			// 区間（制御点と制御点の間）内で生成できるインスタンスの最大数.
			// （バグで無限ループにはまってしまわないように）.
			instance_num_max = Mathf.CeilToInt(distance/this.base_pitch) + 2;

			instance_count = 0;

			while(true) {

				// 次の制御点までの距離が pitch 以下になったら、もう並べられない.
				// （次の区間へ）.
				if(distance - distance_local < pitch) {

					distance_local -= distance;
					break;
				}

				distance_local   += pitch;		// 現在の区間内で進んだ道のり.
				current_distance += pitch; 		// ベースラインの始点から進んだ道のり.

				GameObject tree = GameObject.Instantiate(this.TreePrefab) as GameObject;
	
				Vector3	position = point_previous + dir*distance_local;
	
				tree.transform.position = position;
				tree.tag = "Tree";

				tree.transform.parent = root.transform;

				// 木の間隔を更新する.

				float	fade_length = base_line.total_distance*FADE_LENGTH_SCALE;

				if(current_distance < fade_length) {

					// 始点からフェードイン.
					// 間隔をだんだん狭くする.
					//
					// 距離  [0         ～ fade_length].
					// ピッチ[max_pitch ～ base_pitch].

					rate = Mathf.InverseLerp(0.0f, fade_length, current_distance);

					pitch = Mathf.Lerp(max_pitch, this.base_pitch, rate);

				} else if(base_line.total_distance - current_distance < fade_length){

					// 終点に向けてフェードアウト.
					// 間隔をだんだん広くする.
					//
					// 距離  [base_line.total_distance - fade_length ～ base_line.total_distance].
					// ピッチ[base_pitch                             ～ max_pitch].

					rate = Mathf.InverseLerp(base_line.total_distance - fade_length, base_line.total_distance, current_distance);

					pitch = Mathf.Lerp(this.base_pitch, max_pitch, rate);

				} else {

					// 一定間隔で.

					pitch = this.base_pitch;
				}

				//

				instance_count++;

				if(instance_count >= instance_num_max) {

					break;
				}
			}

			if(instance_count >= instance_num_max) {

				Debug.LogError("error");
				break;
			}

			//

			point_previous = point;
		}
	}

	// ---------------------------------------------------------------- //

	// 作ったものを全て削除する.
	public void	clearOutput()
	{
		GameObject.Destroy(this.root_r);
		GameObject.Destroy(this.root_l);

		//

		this.is_created = false;
	}

	// 森の開始点を設定する.
	public void		setStart(float start)
	{
		this.start = start;
		this.start = Mathf.Clamp(this.start, 0.0f, place_max);

		Vector3		start_position = this.road_creator.getPositionAtPlace(this.start);

		//

		Vector3		screen_position = this.main_camera.GetComponent<Camera>().WorldToScreenPoint(start_position);

		screen_position -= new Vector3(Screen.width/2.0f, Screen.height/2.0f, 0.0f);

		this.start_icon.setPosition(screen_position);
	}

	// 森の終了点を設定する.
	public void		setEnd(float end)
	{
		this.end = end;
		this.end = Mathf.Clamp(this.end, 0.0f, place_max);

		Vector3		end_position = this.road_creator.getPositionAtPlace(this.end);


		//

		Vector3		screen_position = this.main_camera.GetComponent<Camera>().WorldToScreenPoint(end_position);

		screen_position -= new Vector3(Screen.width/2.0f, Screen.height/2.0f, 0.0f);

		this.end_icon.setPosition(screen_position);
	}

	// アイコンの表示／非表示をセットする.
	public void		setIsDrawIcon(bool sw)
	{
		this.start_icon.setVisible(sw);
		this.end_icon.setVisible(sw);

		if(sw) {

			this.setStart(this.start);
			this.setEnd(this.end);
		}
	}

}
