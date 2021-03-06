/// <summary>
/// キャラクターストレージアイテム
/// 
/// 2014/07/08
/// </summary>
using UnityEngine;
using System.Collections;

using Scm.Common.Master;

public class GUICharacterStorageItem : MonoBehaviour
{
	#region フィールド＆プロパティ
	/// <summary>
	/// ストレージ内全ての中からのインデックス
	/// </summary>
	[SerializeField]
	int _indexInTotal = -1;
	public int IndexInTotal { get { return _indexInTotal; } private set { _indexInTotal = value; } }

	/// <summary>
	/// アイテムタイプ
	/// </summary>
	[SerializeField]
	ItemType _type = ItemType.Empty;
	public ItemType Type { get { return _type; } private set { _type = value; } }
	public enum ItemType
	{
		None,
		Icon,
		Empty,
		Add,
	}

	/// <summary>
	/// 所有しているキャラの情報
	/// </summary>
	[SerializeField]
	CharaInfo _charaInfo;
	public CharaInfo CharaInfo { get { return _charaInfo; } private set { _charaInfo = value; } }


    public CharaIcon CharaIcon;
    public bool IsUsed = false;
	/// <summary>
	/// スタイル情報
	/// </summary>
	[SerializeField]
	StyleInfo _styleInfo;
	StyleInfo StyleInfo { get { return _styleInfo; } set { _styleInfo = value; } }
	// スタイル名
	public string StyleName { get { return (_styleInfo != null ? _styleInfo.name : ""); } }

	/// <summary>
	/// アタッチオブジェクト
	/// </summary>
	[SerializeField]
	AttachObject _attach;
	AttachObject Attach { get { return _attach; } }
	[System.Serializable]
	public class AttachObject
	{
		public UIButton button;
		public UISprite iconSprite;
		public UISprite styleSprite;
		public UISprite selectSprite;
		public GameObject deckUsedGroup;

		public Type type;
		[System.Serializable]
		public class Type
		{
			public GameObject loadingGroup;
			public GameObject iconGroup;
			public GameObject emptyGroup;
			public GameObject addGroup;
		}
	}
	#endregion

	#region NGUIリフレクション
	public void OnButton()
	{
		switch (this.Type)
		{
		case ItemType.None:
			break;
		case ItemType.Icon:
            GUIDeckEditCharacterStorage.SetSelectItem(this);
//			GUICharacterStorage.SetSelectItem(this);
			break;
		case ItemType.Empty:
			break;
		case ItemType.Add:
			break;
		}
	}
	#endregion

	#region 初期化
	public static GUICharacterStorageItem Create(GameObject prefab, Transform parent, int itemIndex)
	{
		// インスタンス化
		var go = SafeObject.Instantiate(prefab) as GameObject;
		if (go == null)
			return null;

		// 名前
		go.name = string.Format("{0}{1}", prefab.name, itemIndex);
		// 親子付け
		go.transform.parent = parent;
		go.transform.localPosition = prefab.transform.localPosition;
		go.transform.localRotation = prefab.transform.localRotation;
		go.transform.localScale = prefab.transform.localScale;
		// 可視化
		go.SetActive(true);

		// コンポーネント取得
		var item = go.GetComponentInChildren(typeof(GUICharacterStorageItem)) as GUICharacterStorageItem;
		if (item == null)
			return null;
		// 値初期化
		item.ClearValue();

		return item;
	}
	public void ClearValue()
	{
		this.SetSelectSpriteActive(false);
		this.Setup(ItemType.None, -1);
		this.ClearIconSprite();
		this.ClearStyle();
		this.ChangeLoadingIcon(true);
	    this.IsUsed = false;
	}
	#endregion

	#region セットアップ
	public void Setup(ItemType type, int indexInTotal)
	{
		this.Setup(type, indexInTotal, false, null, null);
	}

	public void Setup(ItemType type, int indexInTotal, bool isUsed, CharaInfo info, CharaIcon charaIcon)
	{
		this.Type = type;
		this.IndexInTotal = indexInTotal;
		this.CharaInfo = (info != null ? info : new CharaInfo());
	    this.CharaIcon = charaIcon;
		this.SetSelectSpriteActive(false);
	    this.IsUsed = isUsed;

		var t = this.Attach.type;
		switch (type)
		{
		case ItemType.None:
			if (this.Attach.deckUsedGroup != null) this.Attach.deckUsedGroup.SetActive(false);
			if (t.loadingGroup != null) t.loadingGroup.SetActive(false);
			if (t.iconGroup != null) t.iconGroup.SetActive(false);
			if (t.emptyGroup != null) t.emptyGroup.SetActive(false);
			if (t.addGroup != null) t.addGroup.SetActive(false);
			break;
		case ItemType.Icon:
			this.ChangeLoadingIcon(true);
			//this.SetStyle(info.avatarType);
			if (this.Attach.deckUsedGroup != null) this.Attach.deckUsedGroup.SetActive(isUsed);
			if (t.emptyGroup != null) t.emptyGroup.SetActive(false);
			if (t.addGroup != null) t.addGroup.SetActive(false);
			if (charaIcon != null) charaIcon.GetIcon(this.CharaInfo.AvatarType, this.CharaInfo.SkinId, false, this.SetIconSprite);
			break;
		case ItemType.Empty:
			if (t.loadingGroup != null) t.loadingGroup.SetActive(false);
			if (t.iconGroup != null) t.iconGroup.SetActive(false);
			if (t.emptyGroup != null) t.emptyGroup.SetActive(true);
			if (t.addGroup != null) t.addGroup.SetActive(false);
			break;
		case ItemType.Add:
			if (t.loadingGroup != null) t.loadingGroup.SetActive(false);
			if (t.iconGroup != null) t.iconGroup.SetActive(false);
			if (t.emptyGroup != null) t.emptyGroup.SetActive(false);
			if (t.addGroup != null) t.addGroup.SetActive(true);
			break;
		}
	}

    public void SetUse(bool pUse)
    {
        Setup(this.Type, this.IndexInTotal, pUse, this.CharaInfo, this.CharaIcon);
        SetSelectSpriteActive(pUse);
    }

	void ChangeLoadingIcon(bool isLoading)
	{
		var t = this.Attach.type;
		if (t.loadingGroup != null)
			t.loadingGroup.SetActive(isLoading);
		if (t.iconGroup != null)
			t.iconGroup.SetActive(!isLoading);
	}
	/// <summary>
	/// 選択枠の表示設定
	/// </summary>
	public void SetSelectSpriteActive(bool isActive)
	{
		if (this.Attach.selectSprite != null)
			this.Attach.selectSprite.gameObject.SetActive(isActive);
	}
	#endregion

	#region アイコン設定
	void ClearIconSprite()
	{
		this.SetIconSprite(null, "");
	}
	void SetIconSprite(CharaIcon charaIcon)
	{
		if (charaIcon == null)
			return;
		if (this.CharaInfo.AvatarType == AvatarType.None)
			this.ClearIconSprite();
		else
			charaIcon.GetIcon(this.CharaInfo.AvatarType, this.CharaInfo.SkinId, false, this.SetIconSprite);
	}
	void SetIconSprite(UIAtlas atlas, string spriteName)
	{
		var sp = this.Attach.iconSprite;
		if (sp == null)
			return;
		var button = this.Attach.button;
		if (button == null)
			return;

		// アトラス設定
		sp.atlas = atlas;
		// スプライト設定
		// ボタンの通常スプライトの方にも適用しないとホバーした時とか元に戻ってしまう
		sp.spriteName = spriteName;
		button.normalSprite = spriteName;

		// アトラス内にアイコンが含まれているかチェック
		if (sp.GetAtlasSprite() == null)
		{
			// アトラスとスプライト名が両方共設定されていてスプライトがない場合はエラー扱いｌ
			if (atlas != null && !string.IsNullOrEmpty(spriteName))
			{
				Debug.LogWarning(string.Format(
					"SetIconSprite:\r\n" +
					"Sprite Not Found!! AvatarType = {0} SpriteName = {1}", this.CharaInfo.AvatarType, spriteName));
			}
		}

		// アイコン表示
		this.ChangeLoadingIcon(false);
	}
	#endregion

	#region スタイル設定
	void ClearStyle()
	{
		this.SetStyle(AvatarType.None);
	}
	void SetStyle(AvatarType avatarType)
	{
		// スタイル情報を取得する
		StyleInfo info = null;
		bool isEnable = false;
		this.StyleInfo = null;
		if (ObsolateSrc.TryGetStyleInfo(avatarType, out info))
		{
			this.StyleInfo = info;
			isEnable = true;
		}

		// スタイルアイコン設定
		var sp = this.Attach.styleSprite;
		if (sp == null)
		{
			Debug.LogWarning("StyleSprite Non Attach!");
		}
		else
		{
			// スタイルアイコン表示設定
			sp.gameObject.SetActive(isEnable);
			if (isEnable)
			{
				// スプライト変更
				// 同じアトラス内にあることを前提とする
				if (info != null)
				{
					sp.spriteName = info.iconName;

					// アトラス内にアイコンが含まれているかチェック
					if (sp.GetAtlasSprite() == null)
					{
						// アトラスとスプライト名が両方共設定されていてスプライトがない場合はエラー扱いｌ
						if (sp.atlas != null && !string.IsNullOrEmpty(info.iconName))
						{
							Debug.LogWarning(string.Format(
								"SetStyle:\r\n" +
								"Sprite Not Found!! StyleType = {0} SpriteName = {1}", info.type, info.iconName));
						}
					}
				}
			}
		}
	}
	#endregion
}
