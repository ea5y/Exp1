/// <summary>
/// デバッグ用.プレイヤーのStateを監視する
/// 
/// 2015/06/10
/// </summary>
using UnityEngine;
using System.Collections;

public class PlayerStateMonitor : MonoBehaviour
{
#if UNITY_EDITOR && XW_DEBUG
	#region フィールド&プロパティ
	const float CenterMag = 0.6f;
	public TextAnchor textAnchor;
	private new GUIText guiText;
	#endregion

	#region 更新
	void Awake()
	{
		guiText = this.gameObject.GetComponent<GUIText>();
		SetGUITextAnchor();
	}
	void Update()
	{
		if (ScmParam.Debug.File.IsDrawFPS)
		{
			if(PlayerManager.Instance != null && PlayerManager.Instance.Player != null)
			{
				this.SetGUIText(PlayerManager.Instance.Player.GetStateInfoStr());
			}
			else
			{
				this.SetGUIText(string.Empty);
			}
		}
		else
		{
			this.SetGUIText(string.Empty);
		}
	}
	void OnValidate()
	{
		this.SetGUITextAnchor();
	}
	#endregion

	#region SetGUIText
	void SetGUIText(string text)
	{
		if (guiText != null)
		{
			guiText.text = text;
		}
	}
	void SetGUITextAnchor()
	{
		if (guiText != null)
		{
			// 位置修正
			Vector2 offset = Vector2.zero;
			switch (this.textAnchor)
			{
			case TextAnchor.LowerLeft:
				guiText.anchor = TextAnchor.LowerLeft;
				break;
			case TextAnchor.LowerCenter:
				guiText.anchor = TextAnchor.LowerRight;
				offset.x = Screen.width * CenterMag;
				break;
			case TextAnchor.LowerRight:
				guiText.anchor = TextAnchor.LowerRight;
				offset.x = Screen.width;
				break;
			case TextAnchor.MiddleLeft:
				guiText.anchor = TextAnchor.MiddleLeft;
				offset.y = Screen.height / 2;
				break;
			case TextAnchor.MiddleCenter:
				guiText.anchor = TextAnchor.MiddleRight;
				offset.x = Screen.width * CenterMag;
				offset.y = Screen.height / 2;
				break;
			case TextAnchor.MiddleRight:
				guiText.anchor = TextAnchor.MiddleRight;
				offset.x = Screen.width;
				offset.y = Screen.height / 2;
				break;
			case TextAnchor.UpperLeft:
				guiText.anchor = TextAnchor.UpperLeft;
				offset.y = Screen.height;
				break;
			case TextAnchor.UpperCenter:
				guiText.anchor = TextAnchor.UpperRight;
				offset.x = Screen.width * CenterMag;
				offset.y = Screen.height;
				break;
			case TextAnchor.UpperRight:
				guiText.anchor = TextAnchor.UpperRight;
				offset.x = Screen.width;
				offset.y = Screen.height;
				break;
			}
			guiText.pixelOffset = offset;
		}
	}
	#endregion
#endif
}
