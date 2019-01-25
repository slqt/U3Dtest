using UnityEngine;
using System.Collections;

public class UIIcon : MonoBehaviour {

	// ================================================================ //
	// MonoBehaviour からの継承.

	void	Start()
	{
	}
	
	void 	Update()
	{

	}

	// ================================================================ //

	public void		setVisible(bool is_visible)
	{
		this.gameObject.SetActive(is_visible);
	}

	public void		setPosition(float x, float y)
	{
		this.GetComponent<RectTransform>().localPosition = new Vector3(x, y, 0.0f);
	}

	public void		setPosition(Vector3 position)
	{
		this.setPosition(position.x, position.y);
	}
}
