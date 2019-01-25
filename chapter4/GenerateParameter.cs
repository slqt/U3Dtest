using UnityEngine;
using System.Collections;

[System.Serializable]
public class GenerateParameter 
{
    public RectXZ	posXZ = new RectXZ(0.0f, 0.0f, 1800.0f, 1800.0f);

    public bool		fill = false;   	// true : posXZ内を全部対象とする .
                                		// flase: posXZの外周上を対象とする.
    public int		limitNum  = 1;		// フィールドに存在できる数.
    public float	delayTime = 1.0f;	// 生成前のディレイ.
    public bool		endless   = true;	// リミット数から減った時に自動追加するか.

}

[System.Serializable]
public class RectXZ {

	public float	x, z;
	public float	width;
	public float	depth;

	public RectXZ(float x, float z, float width, float depth)
	{
		this.x = x;
		this.z = z;
		this.width = width;
		this.depth = depth;
	}

	public float getXMin()
	{
		return(this.x - this.width/2.0f);
	}
	public float getXMax()
	{
		return(this.x + this.width/2.0f);
	}
	public float getZMin()
	{
		return(this.z - this.depth/2.0f);
	}
	public float getZMax()
	{
		return(this.z + this.depth/2.0f);
	}
}
