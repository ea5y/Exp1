/// <summary>
/// オプションのチェックボックスアイテム
/// 
/// 2015/01/08
/// </summary>
using UnityEngine;
using System.Collections;

public class GUIOptionItemCheckBox : GUIOptionItemBase
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
		public UIToggle checkBox;	// チェックボックス
	}

	// 値が変化した時の処理
	System.Action<bool> ChangeFunc;
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
	public static GUIOptionItemCheckBox Create(GameObject prefab, Transform parent, int itemIndex)
	{
		// インスタンス化
		var go = GUIOptionItemBase.CreateBase(prefab, parent, itemIndex);
		if (go == null)
			return null;

		// コンポーネント取得
		var item = go.GetComponent(typeof(GUIOptionItemCheckBox)) as GUIOptionItemCheckBox;
		if (item == null)
			return null;
		// 値初期化
		item.ClearValue();

		return item;
	}
	public void ClearValue()
	{
		this.Setup("", false, null);
	}
	#endregion

	#region セットアップ
	public void Setup(string descText, bool value, System.Action<bool> changeFunc)
	{
		this.ChangeFunc = (changeFunc != null ? changeFunc : delegate { });

		// UI更新
		{
			var t = this.Attach;
			if (t.descLabel != null)
				t.descLabel.text = descText;
			if (t.checkBox != null)
				t.checkBox.value = value;
		}
	}
	#endregion

	#region NGUIリフレクション
	public void OnValueChange()
	{
		if (UIToggle.current == null)
			return;
		this.ChangeFunc(UIToggle.current.value);
	}
	#endregion
}
