/// <summary>
/// オプションのポップアップリストアイテム
/// 
/// 2015/01/08
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GUIOptionItemPopupList : GUIOptionItemBase
{
	#region フィールド＆プロパティ
	/// <summary>
	/// アタッチオブジェクト
	/// </summary>
	[SerializeField]
	AttachObject _attach;
	AttachObject Attach { get { return _attach; } }
	[System.Serializable]
	public class AttachObject
	{
		public UILabel descLabel;	// 説明文
		public UIPopupList popupList;	// ポップアップリスト
	}

	// 値が変化した時の処理
	System.Action<int> ChangeFunc;
	// シリアライズされていないメンバーの初期化
	void MemberInit()
	{
		this.ChangeFunc = delegate { };
	}
	#endregion

	#region 初期化
	void Awake()
	{
		this.MemberInit();
	}
	public static GUIOptionItemPopupList Create(GameObject prefab, Transform parent, int itemIndex)
	{
		// インスタンス化
		var go = GUIOptionItemBase.CreateBase(prefab, parent, itemIndex);
		if (go == null)
			return null;

		// コンポーネント取得
		var item = go.GetComponent(typeof(GUIOptionItemPopupList)) as GUIOptionItemPopupList;
		if (item == null)
			return null;
		// 値初期化
		item.ClearValue();

		return item;
	}
	public void ClearValue()
	{
		this.Setup("", "", null, null);
	}
	#endregion

	#region セットアップ
	public void Setup(string descText, string value, Dictionary<int, string> popupListDict, System.Action<int> changeFunc)
	{
		this.ChangeFunc = (changeFunc != null ? changeFunc : delegate { });

		// UI更新
		{
			var t = this.Attach;
			if (t.descLabel != null)
				t.descLabel.text = descText;
			if (t.popupList != null)
			{
				t.popupList.Clear();
				if (popupListDict != null)
				{
					foreach (var pair in popupListDict)
						t.popupList.AddItem(pair.Value, pair.Key);
				}
				t.popupList.value = value;
			}
		}
	}
	#endregion

	#region NGUIリフレクション
	public void OnValueChange()
	{
		if (UIPopupList.current == null)
			return;
		if (UIPopupList.current.data == null)
			return;
		this.ChangeFunc((int)UIPopupList.current.data);
	}
	#endregion
}
