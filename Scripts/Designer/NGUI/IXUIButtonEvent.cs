/// <summary>
/// ボタンの各イベント時の機能.
/// 
/// 2014/06/05.
/// </summary>
using UnityEngine;
using System.Collections;

public interface IXUIButtonEvent
{
	void OnNormal(XUIButton current, bool immediate);
	void OnHover(XUIButton current, bool immediate);
	void OnPressed(XUIButton current, bool immediate);
	void OnDisabled(XUIButton current, bool immediate);
}