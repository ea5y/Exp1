/// <summary>
/// スケール設定をする
/// 
/// 2015/12/07
/// </summary>
using UnityEngine;

public class SetScaleRate : MonoBehaviour
{
	void OnEnable()
	{
		this.SetScale();
	}

#if UNITY_EDITOR
	void Update()
	{
		this.SetScale();
	}
#endif

	public void SetScale()
	{
		var t = ApplicationController.ScreenInfo;
		if (t != null)
		{
			this.transform.localScale = new Vector3(t.ScaleRate, t.ScaleRate, 1.0f);
		}
	}
}
