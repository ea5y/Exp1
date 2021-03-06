/// <summary>
/// 3Dオブジェクトに対するUIを設定するベースクラス
/// 
/// 2015/02/26
/// </summary>
using UnityEngine;
using System.Collections;

public abstract class GUIObjectUIBaseData : ScriptableObject
{
	public abstract OUIItemRoot Create(ObjectBase o);
}
