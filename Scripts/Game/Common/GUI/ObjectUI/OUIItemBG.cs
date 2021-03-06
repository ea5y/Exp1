/// <summary>
/// 3Dオブジェクトに対するUIアイテム
/// BGスプライト
/// 
/// 2014/06/23
/// </summary>
using UnityEngine;
using System.Collections;
using Scm.Common.GameParameter;

public class OUIItemBG : OUIItemBase
{
	#region フィールド＆プロパティ
	/// <summary>
	/// アタッチオブジェクト
	/// </summary>
	[SerializeField] AttachObject _attach;
	public AttachObject Attach { get { return _attach; } }
	[System.Serializable]
	public class AttachObject
	{
		public UISprite sprite;
	}
	#endregion

	#region 作成
	/// <summary>
	/// プレハブ取得
	/// </summary>
	public static OUIItemBG GetPrefab(GUIObjectUI.AttachObject.Prefab prefab, ObjectBase o)
	{
		if (ScmParam.Common.AreaType == AreaType.Field)
		{
			switch (o.EntrantType)
			{
			case EntrantType.Pc:
				return null;
			}
		}

		return prefab.bg.bg;
	}
	#endregion
}
