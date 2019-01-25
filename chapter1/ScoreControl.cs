using UnityEngine;
using System.Collections;

public class ScoreControl : MonoBehaviour {

	public	bool		drawZero;

	public	AudioClip	CountUpSound = null;			// カウントアップ.
	public	AudioClip[]	CountUpSounds = null;			// カウントアップ.
	public	AudioSource	count_up_audio;					// カウントアップ音再生用.
	private	int			CountLevel;

	private	int			targetNum;
	private int			currentNum;
	private float		timer;

	public GameObject				uiScore;				// スコアー全体の GameObject.
	public UnityEngine.UI.Image		uiImageDigit0;
	public UnityEngine.UI.Image[]	uiImageScoreDigits;
	public UnityEngine.Sprite[]		numSprites;
	
	// Use this for initialization
	void Start ()
	{
		this.count_up_audio = this.gameObject.AddComponent<AudioSource>();
	
		this.timer	= 0.0f;
	}

	public void	Update()
	{
		if( this.targetNum > this.currentNum )
		{
			this.timer += Time.deltaTime;
			
			if( timer > 0.1f )
			{
				// ランダムでSEを鳴らす.
				int	idx = Random.Range(0, this.CountUpSounds.Length);
		
				this.count_up_audio.PlayOneShot( this.CountUpSounds[idx] );

				this.timer	= 0.0f;
				
				// あまりに差があるときは5づつカウントアップする.
				if( this.targetNum - this.currentNum > 10 )
				{
					this.currentNum += 5;
				}
				else
				{
					this.currentNum++;
				}
				CountLevel++;
			}
		}
		else
		{
			CountLevel	= 0;
		}

		// 各桁の Image に数字のテクスチャーをセットする.

		float	scale = 1.0f;

		if(this.targetNum != this.currentNum) {

			scale = 2.5f - 1.5f*(this.timer*10.0f);
		}

		int		disp_number = Mathf.Max(0, this.currentNum);

		for(int i = 0;i < this.uiImageScoreDigits.Length;i++) {

			int		number_at_digit = disp_number%10;

			if(number_at_digit == 0) {

				if(!this.drawZero) {
					continue;
				}
			}

			this.uiImageScoreDigits[i].sprite = this.numSprites[number_at_digit];
			this.uiImageScoreDigits[i].GetComponent<RectTransform>().localScale = Vector3.one*scale;

			disp_number /= 10;
		}

		// 表示/非表示.

		if(SceneControl.get().IsDrawScore()) {

			this.uiScore.SetActive(true);

		} else {

			this.uiScore.SetActive(false);
		}
	}

	//表示する数字を設定.
	public void setNum( int num )
	{
		if( this.targetNum == this.currentNum )
		{
			this.timer	= 0.0f;
		}
		this.targetNum	= num;
	}
	
	//表示する数字を即時で設定.
	public void setNumForce( int num )
	{
		this.targetNum		= num;
		this.currentNum		= num;
	}
	
	public bool isActive()
	{
		return ( this.targetNum != this.currentNum ) ? true : false;
	}
}
