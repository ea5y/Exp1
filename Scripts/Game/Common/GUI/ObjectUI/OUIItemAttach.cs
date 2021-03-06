/// <summary>
/// 3Dオブジェクトに対するUIアイテム
/// 各種アイテムアタッチ用スクリプト
/// 
/// 2014/06/23
/// </summary>
using UnityEngine;
using System.Collections;
using Scm.Common.GameParameter;

public class OUIItemAttach : OUIItemBase
{
	#region 作成
	/// <summary>
	/// プレハブ取得
	/// </summary>
	public static OUIItemAttach GetPrefab(GUIObjectUI.AttachObject.Prefab prefab, ObjectBase o)
	{
		return prefab.attach.attach;
	}
	#endregion

	#region OUIItemBase override
	public override void Destroy(float timer)
	{
		// オブジェクトが消えても数秒間は生きている
		Object.Destroy(this.gameObject, timer);
	}
	protected override void SetActive(bool isActive)
	{
		// 常に表示
		base.SetActive(true);
	}
	#endregion
}
