using UnityEngine;
using System.Collections;

public class RandomGenerator : MonoBehaviour {

    [SerializeField]
    private GameObject target;  // 生成対象.
    [SerializeField]
    private GenerateParameter param = new GenerateParameter();
    [SerializeField]
    private float	posY = 1.0f;

    private int		counter    = 0;
    private bool	limitCheck = false;
    private bool	ready      = false;

    private ArrayList	childrenArray = new ArrayList();
   
    void Start()
    {
        // 初期配置分がある場合はここで登録しておく（主にデバッグ用）.
        GameObject[] children = GameObject.FindGameObjectsWithTag(target.tag);
        for (int i = 0; i < children.Length; i++ )
        {
            childrenArray.Add(children[i]);
        }

        OnGeneratorStart();
    }

    void Update()
    {
        if (TimingCheck())
        {
            ready = false;
            StartCoroutine("Delay");
        }
    }

    private bool TimingCheck()
    {
        // 準備できてない.
        if (!ready) return false;
        // 1度リミットに到達していて、エンドレスフラグが立っていないときは追加しない.
        if (!param.endless && limitCheck) return false;
        // 個数チェック.
        return (ChildrenNum() < param.limitNum) ? true : false;
    }

    private IEnumerator Delay()
    {
        yield return new WaitForSeconds(param.delayTime);

        Generate();
        ready = true;
    }

    void OnGeneratorStart()
    {
        counter = 0;
        ready = true;
        limitCheck = false;
    }
    void OnGeneratorSuspend()
    {
        ready = false;
    }
    void OnGeneratorResume()
    {
        ready = true;
    }

    /// <summary>
    /// [ Message ] オブジェクト破壊.
    /// </summary>
    void OnDestroyObject( GameObject target )
    {
        // 配列に残っていれば削除.
        childrenArray.Remove(target);
        // 子供が減った通知.
        SendMessage("OnDestroyChild", target, SendMessageOptions.DontRequireReceiver);
        Destroy(target);
    }

    // オブジェクト生成.
    public void Generate()
    {
        RectXZ		rect = param.posXZ;
        Vector3 	pos = new Vector3(rect.x, posY, rect.z);

		if(param.fill) {

            // posRange内にランダムに位置を決める.
            pos.x = Random.Range(-0.5f, 0.5f)*rect.width + rect.x;
            pos.z = Random.Range(-0.5f, 0.5f)*rect.depth + rect.z;

		} else {

            // posRange外周上にランダムに位置を決める.
			float	l = Random.Range(0.0f, rect.width*2.0f + rect.depth*2.0f);

			do {

				if(l < rect.width) {

					pos.x = rect.getXMin() + l;
					pos.z = rect.getZMin();
					break;
				}
				l -= rect.width;

				if(l < rect.depth) {

					pos.x = rect.getXMax();
					pos.z = rect.getZMin() + l;
					break;
				}
				l -= rect.depth;

				if(l < rect.width) {

					pos.x = rect.getXMax() - l;
					pos.z = rect.getZMax();
					break;
				}
				l -= rect.width;

				if(l < rect.depth) {

					pos.x = rect.getXMin();
					pos.z = rect.getZMax() - l;
					break;
				}
				l -= rect.depth;

			} while(false);

        }
		Debug.Log(pos.x + " " + pos.z);

        // インスタンス生成.
        GameObject newChild = Object.Instantiate(target, pos, Quaternion.identity) as GameObject;
        // 自分を親にする.
        newChild.transform.parent = transform;
	
        // 配列更新.
        childrenArray.Add(newChild);

        // 子供を増やした通知.
        SendMessage("OnInstantiatedChild", newChild, SendMessageOptions.DontRequireReceiver);

        counter++;
        if (counter >= param.limitNum)
        {
            limitCheck = true;  // 一度リミットに到達したらチェックを入れる.
        }
    }

    public int ChildrenNum()
    {
        if (childrenArray != null) return childrenArray.Count;
        return 0;
    }
    public GameObject Target() { return target; }

    // 管理している子の参照.
    public ArrayList Children() { return childrenArray; }

    // 生成パラメータセット.
    public void SetParam(GenerateParameter param_) {  param = param_; }

}

