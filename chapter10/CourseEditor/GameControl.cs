using UnityEngine;
using System.Collections;

public class GameControl : MonoBehaviour {

	public ToolControl		tool_control = null;
	public AudioClip		goalAudioClip = null;

	private RaycastHit	hit;
	private bool		is_hitted = false;
	private int			current_block_index = 0;
	

	private int			goal_poly_index = -1;
	private int			current_poly_index = -1;

	private bool		is_goaled = false;				//!< ゴールした？.

	private AudioSource	goal_audio = null;

	// ------------------------------------------------------------------------ //

	// Use this for initialization
	void Start () {

		this.goal_audio = this.gameObject.AddComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
	
		RoadCreator	road_creator = this.tool_control.road_creator;

		// -------------------------------------------------------------------------------------------- //
		// 車が走っているのがどのブロックなのかを調べる.
		// （ブロック＝コースを進行方向にそって分割したもの）.

		GameObject	car_object = this.tool_control.car_object;

		if(car_object != null) {


			// 車がいるブロックを調べる.

			// 車の真下に向かって Ray を飛ばし、道路とコリジョンをとる.

			Vector3		start = car_object.transform.position;
			Vector3		end   = start + Vector3.down*10.0f;

			is_hitted = Physics.Linecast(start, end, out hit, (1 << LayerMask.NameToLayer("Road Coli")));

			if(is_hitted) {

				this.current_block_index = road_creator.getRoadBlockIndexByName(hit.collider.name);

				if(this.current_block_index != -1) {

					// 車がいる前後のブロックのみ、コリジョンを有効にする.
					//
					for(int i = 0;i < road_creator.road_mesh.Length;i++) {
	
						if(this.current_block_index - 1 <= i && i <= this.current_block_index + 1) {
	
							road_creator.setEnableToBlock(i, true);
	
						} else {
	
							road_creator.setEnableToBlock(i, false);
						}
					}

					//

					current_poly_index = hit.triangleIndex/2;
				}
			}
			
			// ゴールした？　判定.		
			if(!this.is_goaled) {

				do {

					if(road_creator.road_mesh == null) {

						break;
					}

					if(this.current_block_index < road_creator.road_mesh.Length - 1) {

						break;
					}


					if(this.current_poly_index < this.goal_poly_index - 1) {

						break;
					}


					this.is_goaled = true;
					this.goal_audio.PlayOneShot(this.goalAudioClip);

				} while(false);
			}
		}
	}
#if false
	void	OnGUI()
	{

		RoadCreator	road_creator = this.tool_control.road_creator;

		if(road_creator.road_mesh != null) {

			GUI.Label(new Rect(100, 100, 100, 20), this.current_block_index.ToString() + " " + road_creator.road_mesh.Length);

			GUI.Label(new Rect(100, 120, 100, 20), this.current_poly_index + "/" + this.goal_poly_index.ToString());
		}
	}
#endif

	// テスト走行を開始する.
	public void	startTestRun()
	{
		// 環境音を鳴らす.
		this.GetComponent<AudioSource>().Play();

		RoadCreator	road_creator = this.tool_control.road_creator;

		if(road_creator.split_points.Length > 2) {

			int		s = road_creator.split_points[road_creator.split_points.Length - 1 - 1];
			int		e = road_creator.split_points[road_creator.split_points.Length - 1];

			this.goal_poly_index = e - s;
		}
	}

	// テスト走行を終了する.
	public void stopTestRun()
	{
		// 環境音を止める.
		this.GetComponent<AudioSource>().Stop();
	}

	// 生成物をクリアーするときに呼ばれる.
	public void	onClearOutput()
	{
		this.is_goaled = false;
	}
}
