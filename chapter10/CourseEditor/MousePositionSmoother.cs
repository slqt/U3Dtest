using UnityEngine;
using System.Collections;

// マウスカーソルの位置をスムージングする.
public class MousePositionSmoother {

	// ヒストリーの数.
	private static int	HISTORY_NUM = 5;

	// 現在記録しているヒストリーの数.
	private int			current_num = 0;

	// ヒストリー（位置の履歴）.
	private Vector3[]	history;

	// ------------------------------------------------------------------------ //

	public void	create()
	{
		this.history = new Vector3[HISTORY_NUM];
	}

	// マウス位置を追加（スムージングした位置を返す）.
	public Vector3	append(Vector3 current_position)
	{
		// 最新の位置を、ヒストリーに追加する.

		this.history[this.current_num%HISTORY_NUM] = current_position;

		this.current_num++;

		// 過去数フレームの位置を平均化して、スムーズにする.

		int	smooth_num = Mathf.Min(HISTORY_NUM, this.current_num);

		Vector3	smooth_position = Vector3.zero;

		for(int i = 0;i < smooth_num;i++) {

			smooth_position += this.history[i];
		}

		smooth_position /= (float)smooth_num;

		return(smooth_position);
	}

	// リセット（ヒストリーをクリアーする）.
	public void	reset()
	{
		this.current_num = 0;
	}
}
