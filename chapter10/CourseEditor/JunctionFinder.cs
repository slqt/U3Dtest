using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class JunctionFinder {

	public struct Junction {

		public int		i0;
		public int		i1;
	};

	public List<Vector3>	positions;

	public Junction	junction;
	public bool		is_finded = false;

	public List<Junction>	junctions = new List<Junction>();

	// ------------------------------------------------------------------------------------------------ //

	public void	create()
	{
	}

	// 線が交差しているところを見つける.
	public void	findJunction()
	{
		Vector3	xp;
		float	t0, t1;

		this.junctions.Clear();

		this.is_finded = false;

		for(int i = 0;i < this.positions.Count - 2;i++) {

			// 区間（制御点と制御点のあいだ）その１の、線分.
			Vector3	st0  = this.positions[i];
			Vector3 dir0 = this.positions[i + 1] - this.positions[i];

			for(int j = i + 2;j < this.positions.Count - 1;j++) {

				// 区間その２の、線分
				Vector3	st1 = this.positions[j];
				Vector3 dir1 = this.positions[j + 1] - this.positions[j];

				// 線分その１と線分その２が交差しているか、しらべる.

				if(!this.calcIntersectionVectorAndVector(out xp, out t0, out t1, st0, dir0, st1, dir1)) {

					continue;
				}

				// 交差しているのが線分の範囲外（始点と終点の間じゃない）だったらだめ.
				if(t0 < 0.0f || 1.0f < t0) {

					continue;
				}
				if(t1 < 0.0f || 1.0f < t1) {

					continue;
				}

				// 交差していた！.

				this.junction.i0 = i;
				this.junction.i1 = j;

				this.junctions.Add(this.junction);

				this.is_finded = true;
			}
		}
	}

	// ------------------------------------------------------------------------------------------------ //

	private enum ELEMENT {

		X = 0,
		Y,
		Z
	}

	// 線分同士の交点を求める.
	public bool calcIntersectionVectorAndVector(out Vector3 dst, out float t0, out float t1, Vector3 st0, Vector3 dir0, Vector3 st1,Vector3 dir1)
	{
		Vector3				p0, p1, v0, v1;
		Vector3				v0xv1;
		Vector3				xp0, xp1;
		bool				ret;
		ELEMENT				element;
	
		v0 = dir0;
		v1 = dir1;
		p0 = st0;
		p1 = st1;
	
		v0xv1 = Vector3.Cross(v0, v1);
	
		element = selectMaxAbsoluteElement(v0xv1);

		xp0 = Vector3.zero;
		t0 = 0.0f;
		t1 = 0.0f;

		do {

			ret = false;
	
			switch(element) {

				default:	
				case ELEMENT.X:
				{
					t0 = (p0.y - p1.y)*v1.z - (p0.z - p1.z)*v1.y;
					t0 = -t0/v0xv1.x;
				}
				break;
	
				case ELEMENT.Y:
				{
					t0 = (p0.z - p1.z)*v1.x - (p0.x - p1.x)*v1.z;
					t0 = -t0/v0xv1.y;
				}
				break;
	
				case ELEMENT.Z:
				{
					t0 = (p0.x - p1.x)*v1.y - (p0.y - p1.y)*v1.x;
					t0 = -t0/v0xv1.z;
				}
				break;
			}

			// nan だったらおしまい.
			if(float.IsNaN(t0)) {

				break;
			}
	
			//
	
			xp0 = p0 + v0*t0;
	
			// v0 と v1 が平行に近いときは本来エラーになるべきだが、計算上求められて
			// しまうことがある。
			// なので t0/t1 から求められたそれぞれの交点を比較し、一致しない場合は
			// エラーとみなす。
	
			element = selectMaxAbsoluteElement(v1);

			switch(element) {

				default:	
				case ELEMENT.X:
				{
					t1 = (p0.x + t0*v0.x - p1.x)/v1.x;
				}
				break;
	
				case ELEMENT.Y:
				{
					t1 = (p0.y + t0*v0.y - p1.y)/v1.y;
				}
				break;
	
				case ELEMENT.Z:
				{
					t1 = (p0.z + t0*v0.z - p1.z)/v1.z;
				}
				break;
			}
	
			// nan だったらおしまい.
			if(float.IsNaN(t1)) {

				break;
			}

			xp1 = p1 + v1*t1;
	
			//
	
			float	dist = Vector3.Distance(xp0, xp1);
	
			if(dist > (float)(1.0e-4)) {
	
				break;
			}
	
			//
	
			ret = true;
	
		} while(false);
	
		dst = xp0;

		return(ret);
	}

	private ELEMENT	selectMaxAbsoluteElement(Vector3 v)
	{
		ELEMENT				sel;
	
		if(Mathf.Abs(v.x) > Mathf.Abs(v.y)) {
	
			if(Mathf.Abs(v.z) > Mathf.Abs(v.x)) {
	
				sel = ELEMENT.Z;
	
			} else {
	
				sel = ELEMENT.X;
			}
	
		} else {
	
			if(Mathf.Abs(v.z) > Mathf.Abs(v.y)) {
	
				sel = ELEMENT.Z;
	
			} else {
	
				sel = ELEMENT.Y;
			}
		}
	
		return(sel);
	}


}
