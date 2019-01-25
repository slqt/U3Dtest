using UnityEngine;
using System.Collections;

/// <summary>
/// アイテムの接触判定
/// </summary>
public class ItemCollider : MonoBehaviour
{
    private bool 	isFinished;

    void Start()
    {
        isFinished = false;
        GetComponent<Collider>().isTrigger = true;  // トリガーをたてておく
    }

	void	Update()
	{
		// 自爆.
		if(Input.GetKeyDown(KeyCode.D)) {

			this.on_player_hit();
		}
	}

    void OnTriggerEnter(Collider collider)
    {
        if (isFinished) return; 	// 1回だけ衝突をみたいのでその監視用.
                                	// isTrigge=falseしても複数回とってしまう.

        GameObject obj = collider.gameObject;

        if (obj.tag.Equals("Player"))   // プレイヤーか判定.
        {
			this.on_player_hit();
        }
    }

	protected void	on_player_hit()
	{
        isFinished = true;

        GameObject ui = GameObject.Find("/UI");
        if (ui) ui.SendMessage("OnHitItem", gameObject.name);

        Note note = GetComponent<Note>();
        if (note) note.SendMessage("OnHitItem");
	}
}
