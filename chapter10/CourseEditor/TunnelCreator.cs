using UnityEngine;
using System.Collections;

public class TunnelCreator {

	public GameObject		TunnelPrefab = null;
	public GameObject		main_camera;
	public ToolGUI			tool_gui = null;

	public RoadCreator		road_creator = null;

	public GameObject		instance = null;

	public float			place = 0.0f;

	public float			place_min = 0.0f;
	public float			place_max = 0.0f;

	public Vector3[]		vertices_org;			// もともとの形.

	public float			mesh_length;			// Z方向の長さ.

	public bool				is_created = false;		// トンネル作った？.

	// ---------------------------------------------------------------- //

	protected UIIcon	icon;

	// ------------------------------------------------------------------------ //

	public void		create()
	{
		this.icon = this.tool_gui.createTunnelIcon();

		this.icon.setVisible(false);
	}

	// トンネルの場所をセットする.
	public void	setPlace(float place)
	{
		this.place = place;
		this.place = Mathf.Clamp(this.place, this.place_min, this.place_max);

		if(this.is_created) {

			this.modifyShape();

			//

			Vector3		screen_position = this.main_camera.GetComponent<Camera>().WorldToScreenPoint(this.instance.transform.position);

			screen_position -= new Vector3(Screen.width/2.0f, Screen.height/2.0f, 0.0f);

			this.icon.setPosition(screen_position);
		}
	}

	public void modifyShape()
	{
		Mesh mesh = this.instance.GetComponent<MeshFilter>().mesh;

		Vector3[] vertices = mesh.vertices;

		for(int i = 0;i < vertices.Length;i++) {

			vertices[i] = this.vertices_org[i];

			float	z = this.place;

			// Z座標を、道路の中心線上の位置に変換する.
			// 整数部　…　制御点のインデックス.
			// 小数部　…　制御点間での比率.

			z += vertices[i].z/RoadCreator.PolygonSize.z;

			int		place_i = (int)z; 				// 整数部　…　制御点のインデックス.
			float	place_f = z - (float)place_i;	// 小数部　…　制御点間での比率.

			if(place_i >= this.road_creator.sections.Length - 1) {

				place_i = this.road_creator.sections.Length - 1 - 1;
				place_f = 1.0f;
			}

			RoadCreator.Section		section_prev = this.road_creator.sections[place_i];
			RoadCreator.Section		section_next = this.road_creator.sections[place_i + 1];

			// Z 軸が道路の中心線と同じ向きになるように回転する.

			vertices[i].z = 0.0f;
			vertices[i] = Quaternion.LookRotation(section_prev.direction, section_prev.up)*vertices[i];

			// 前後の制御点の間を小数部で補間する.

			vertices[i] += Vector3.Lerp(section_prev.center, section_next.center, place_f);
		}

		//
		{
			int		place_i = (int)place;
			float	place_f = place - (float)place_i;

			RoadCreator.Section		section_prev = this.road_creator.sections[place_i];
			RoadCreator.Section		section_next = this.road_creator.sections[place_i + 1];

			this.instance.transform.position = Vector3.Lerp(section_prev.center, section_next.center, place_f);
			this.instance.transform.rotation = Quaternion.LookRotation(section_prev.direction, section_prev.up);

			for(int i = 0;i < vertices.Length;i++) {

				vertices[i] = this.instance.transform.InverseTransformPoint(vertices[i]);
			}
		}

		//

		mesh.vertices = vertices;
	}
	public void	createTunnel()
	{
		this.instance = GameObject.Instantiate(this.TunnelPrefab) as GameObject;

		Mesh mesh = this.instance.GetComponent<MeshFilter>().mesh;

		this.vertices_org = mesh.vertices;


		this.mesh_length = 0.0f;

		foreach(Vector3 vertex in this.vertices_org) {

			this.mesh_length = Mathf.Max(this.mesh_length, vertex.z);
		}

		this.place_min = 0.0f;
		this.place_max = (float)this.road_creator.sections.Length - 1.0f;
		this.place_max -= this.mesh_length/RoadCreator.PolygonSize.z;

		//

		this.modifyShape();

		//

		this.is_created = true;

		this.setPlace(this.place_min);

		// アイコンを表示する.
		this.setIsDrawIcon(true);
	}

	// 作ったものを全て削除する.
	public void		clearOutput()
	{
		if(this.is_created) {

			GameObject.Destroy(this.instance);
	
			this.vertices_org = null;
			this.mesh_length = 0.0f;
			this.place = 0.0f;
	
			this.is_created = false;
		}
	}

	// アイコンの表示／非表示をセットする.
	public void		setIsDrawIcon(bool sw)
	{
		this.icon.setVisible(sw);
	}
}
