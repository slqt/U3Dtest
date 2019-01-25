using UnityEngine;
using System.Collections;


public class GameCtrl : MonoBehaviour {

	// true にすると、モンスターが出現しなくなる。デバッグ用.
	private const bool	DISABLE_MONSTER_SPAWN = false;

	private const int	START_STAGE = 1;

	public Map m_map;

	public int RETRY = 2;

	public GameObject m_enemyPrefab;
	public GameObject m_treasureGen;	
	
	// BGM関係.
	public AudioChannels	m_audio;
	public AudioClip 		m_bgmClip;
	
	public AudioClip		m_seStageClear;
	public AudioClip		m_seGameOver;
	
	public FollowCamera		m_camera;
	private int				m_retry_remain;
	private ArrayList		m_objList = new ArrayList();
	private int				m_stageNo = START_STAGE;
	
	// ================================================================ //
	// MonoBehaviour からの継承.

	void 	Start()
	{
		GameStart();
	}
	
	void 	Update()
	{
		if (GlobalParam.GetInstance().IsAdvertiseMode() && Input.GetMouseButtonDown(0))
			UnityEngine.SceneManagement.SceneManager.LoadScene("TitleScene");
	
	}

	// ================================================================ //

	void Init()
	{
		m_retry_remain = RETRY;
		GameObject.Find("Map").SendMessage("OnGameStart");
		GameObject.Find("Player").SendMessage("OnGameStart");
	}
	
	
	
	public void Retry()
	{
		if (m_retry_remain > 0) {
			m_retry_remain--;
			Restart();

		} else {
			StartCoroutine("GameOverSeq");
		}
	}
	
	IEnumerator GameOverSeq()
	{
		m_audio.StopAll();
		m_audio.PlayOneShot(m_seGameOver,1.0f,0);
		Hud.get().DrawGameOver(true);
		yield return new WaitForSeconds(5.0f);
		UnityEngine.SceneManagement.SceneManager.LoadScene("TitleScene");
	}
	
	public int GetRetryRemain()
	{
		return m_retry_remain;	
	}
	
	public void PlayerIsDead()
	{
		if (GlobalParam.GetInstance().IsAdvertiseMode())
			UnityEngine.SceneManagement.SceneManager.LoadScene("TitleScene");
		else
			Retry();
	}
	
	public void AddObjectToList(GameObject o)
	{
		m_objList.Add(o);
	}
	
	public void RemoveObjectFromList(GameObject o)
	{
		m_objList.Remove(o);
	}
	
	
	public void Restart()
	{
		GameObject[] enemys = GameObject.FindGameObjectsWithTag("Enemy");
		for (int i = 0; i < enemys.Length; i++)
			enemys[i].SendMessage("OnRestart");
		GameObject.Find("Player").SendMessage("OnRestart");
		TreasureGenerator treasureGen = FindObjectOfType(typeof(TreasureGenerator)) as TreasureGenerator;
		if (treasureGen != null)
			treasureGen.SendMessage("OnRestart");
		HitStop(true);
		Hud.get().DrawStageStart(true);
		StartCoroutine("RestartWait");
	
	}
	
	IEnumerator RestartWait()
	{
		yield return new WaitForSeconds(1.0f);
		HitStop(false);
		Hud.get().DrawStageStart(false);
	}
	
	public void GameStart()
	{
		m_retry_remain = RETRY;
		Hud.get().DrawStageClear(false);
		Hud.get().OnGameStart();
		m_stageNo = START_STAGE;
//		GameObject.Find("Map").SendMessage("OnGameStart");
		GameObject.Find("Player").SendMessage("OnGameStart");
		OnStageStart();
	}
	
	public void OnEatAll()
	{
		GameObject.Find("Player").SendMessage("OnStageClear");
		GameObject[] enemys = GameObject.FindGameObjectsWithTag("Enemy");
		for (int i = 0; i < enemys.Length; i++)
			enemys[i].SendMessage("OnStageClear");
		GameObject.Find("Player").SendMessage("OnStageClear");
		Hud.get().DrawStageClear(true);
		StartCoroutine("StageClear");

	}
	
	public void OnStageStart()
	{
		// 敵全消し.
		GameObject[] enemys = GameObject.FindGameObjectsWithTag("Enemy");
		for (int i = 0; i < enemys.Length; i++)
			Destroy(enemys[i]);
		
		// その他オブジェクト消す.
		TreasureGenerator treasureGenInst = FindObjectOfType(typeof(TreasureGenerator)) as TreasureGenerator;
		if (treasureGenInst != null)
			Destroy(treasureGenInst.gameObject);
		
		GameObject.Find("Map").SendMessage("OnStageStart");

		if(!DISABLE_MONSTER_SPAWN) {

			// 敵スポーン.
			for (int i = 1; i < 5; i++) {
	
				Vector3 pos = m_map.GetSpawnPoint(i);
	
				if(pos == Vector3.zero) {
					// このタイプの敵は作らない.
					continue;
				}
	
				GameObject	e = (GameObject)Instantiate(m_enemyPrefab,pos,Quaternion.identity);
				MonsterCtrl	mc = e.GetComponent<MonsterCtrl>();
				mc.m_aiType = (MonsterCtrl.AI_TYPE)((int)MonsterCtrl.AI_TYPE.TRACER + i - 1);
				mc.SetSpawnPosition(m_map.GetSpawnPoint(Map.SPAWN_POINT_TYPE.BLOCK_SPAWN_POINT_PLAYER + i));
				mc.SetDifficulty(m_stageNo);
				
				mc.SendMessage("OnStageStart");
			}
		}	
		
		// ステージ番号更新.
		Hud.get().SetStageNumber(m_stageNo);
		
		// 宝物スポーン.
		Vector3 trasurePos = m_map.GetSpawnPoint(Map.SPAWN_POINT_TYPE.BLOCK_SPAWN_TREASURE);
		
		if (trasurePos != Vector3.zero)
			 Instantiate(m_treasureGen,trasurePos,Quaternion.identity);
		
		Hud.get().DrawStageClear(false);
		GameObject.Find("Player").SendMessage("OnStageStart");
		Hud.get().DrawStageStart(true);
		HitStop(true);
		
		// BGM再生.
		m_audio.StopAll();
		m_audio.PlayLoop(m_bgmClip,1.0f,0.0f);
		
		StartCoroutine("StageStartWait");
	}
	
	IEnumerator StageStartWait()
	{
		yield return new WaitForSeconds(1.0f);
		Hud.get().DrawStageStart(false);
		HitStop(false);
	}
	
	
	IEnumerator StageClear()
	{
		m_audio.StopAll();
		m_audio.PlayOneShot(m_seStageClear,1.0f,0);
		yield return new WaitForSeconds(3.0f);
		m_stageNo++;
		OnStageStart();
	}
	
	public int GetStageNo()
	{
		return m_stageNo;
	}
	
	public void HitStop(bool enable)
	{
		GameObject[] enemys = GameObject.FindGameObjectsWithTag("Enemy");
		for (int i = 0; i < enemys.Length; i++)
			enemys[i].SendMessage("HitStop",enable);
		GameObject.FindGameObjectWithTag("Player").SendMessage("HitStop",enable);
	}
	
	public void OnAttack()
	{
		m_camera.OnAttack();
		HitStop(true);
	}
	
	public void OnEndAttack()
	{
		m_camera.OnEndAttack();
		HitStop(false);
	}
}
