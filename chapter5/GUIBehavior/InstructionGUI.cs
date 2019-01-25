using UnityEngine;
using System.Collections;

// ゲームルールなどの説明画面の挙動.
public class InstructionGUI : MonoBehaviour {

	public SimpleSpriteAnimation sampleBandMemberAniamtion;
	public SimpleSpriteAnimation playerAvatorAnimation;

	// Use this for initialization
	void 	Start()
	{
	}
	
	// Update is called once per frame
	void 	Update()
	{
		//一定時間ごとにキャラをアニメーションさせる。.
		animationCounter+=Time.deltaTime;
		if( animationCounter > 1.0f){
			sampleBandMemberAniamtion.BeginAnimation(1,1,false);
			playerAvatorAnimation.BeginAnimation(2,1,false);
			animationCounter=0;
		}

		//クリックで次に進む.
		if( Input.GetMouseButton(0) ){
			GameObject.Find("PhaseManager").GetComponent<PhaseManager>().SetPhase("Play");
		}
	}

	float animationCounter=0;
	
}
