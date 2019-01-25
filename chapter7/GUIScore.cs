using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GUIScore : MonoBehaviour {

	protected	int			digitNum = 4;

	private	int			targetNum;
	private int			currentNum;
	private float		timer;
	
	public delegate void CallBack();
	public CallBack	CallBackCountUp = null;

	public GameObject	numberPrefab;

	protected List<GUINumber>	numbers = new List<GUINumber>();

	// ================================================================ //
	// MonoBehaviour からの継承.

	void	Awake()
	{
	}

	// Use this for initialization
	void Start ()
	{
		for(int i = 0;i < this.digitNum;i++) {

			GUINumber	number = this.create_number(32.0f*(this.digitNum - 1 - i), 0.0f);

			this.numbers.Add(number);
		}
	
		this.timer	= 0.0f;
	}

	public void	Update()
	{
		if( this.targetNum > this.currentNum )
		{
			this.timer += Time.deltaTime;
			
			if( timer > 0.05f )
			{
				this.timer	= 0.0f;
				
				// あまりに差があるときは5づつカウントアップする
				if( this.targetNum - this.currentNum > 10 )
				{
					this.currentNum += 5;
				}
				else
				{
					this.currentNum++;
				}
				if( CallBackCountUp != null )
				{
					CallBackCountUp();
				}
			}
		}

		this.drawNum(currentNum, 0.0f, 0.0f, 0.0f, 1.0f);
	}
	
	private void drawNum( int num, float x, float y, float z, float scale )
	{
		int		n = num;

		for(int i = 0;i < this.numbers.Count;i++) {

			this.numbers[i].setNumber(n%10);

			n /= 10;
		}
	}
	
	//表示する数字を設定
	public void setNum( int num )
	{
		if( this.targetNum == this.currentNum )
		{
			this.timer	= 0.0f;
		}
		this.targetNum	= num;
	}
	
	//表示する数字を即時で設定
	public void setNumForce( int num )
	{
		this.targetNum		= num;
		this.currentNum		= num;
	}

	// ================================================================ //

	// 数字のイメージを作る.
	protected GUINumber	create_number(float x, float y)
	{
		GUINumber	number = GameObject.Instantiate(this.numberPrefab).GetComponent<GUINumber>();

		number.GetComponent<RectTransform>().SetParent(this.GetComponent<RectTransform>());
		number.GetComponent<RectTransform>().localPosition = new Vector3(x, y, 0.0f);

		return(number);
	}
}
