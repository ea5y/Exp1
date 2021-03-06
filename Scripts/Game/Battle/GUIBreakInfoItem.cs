/// <summary>
/// 倒した相手の情報のアイテム
/// 
/// 2014/07/21
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GUIBreakInfoItem : MonoBehaviour
{
	#region フィールド＆プロパティ
	/// <summary>
	/// アイテム時の情報
	/// </summary>
	[SerializeField]
	Info _itemInfo;
	Info ItemInfo { get { return _itemInfo; } set { _itemInfo = value; } }
	[System.Serializable]
	public class Info
	{
		public bool isWinMyteam;
		public AvatarType winAvatarType;
        public int winSkinId;
		public string winName;
		public AvatarType loseAvatarType;
        public int loseSkinId;
		public string loseName;
		public Info(bool isWinMyteam, AvatarType winAvatarType, int winSkinId, string winName, AvatarType loseAvatarType, int loseSkinId, string loseName)
		{
			this.isWinMyteam = isWinMyteam;
			this.winAvatarType = winAvatarType;
            this.winSkinId = winSkinId;
			this.winName = winName;
			this.loseAvatarType = loseAvatarType;
            this.loseSkinId = loseSkinId;
			this.loseName = loseName;
		}
	}

	/// <summary>
	/// アタッチオブジェクト
	/// </summary>
	[SerializeField]
	AttachObject _attach;
	AttachObject Attach { get { return _attach; } }
	[System.Serializable]
	public class AttachObject
	{
		public UILabel winLabel;
		public UISprite winIconSprite;
		public UISprite winFrameSprite;
		public UILabel loseLabel;
		public UISprite loseFrameSprite;
		public UISprite loseIconSprite;
	}
	#endregion

	#region 初期化
	public static GUIBreakInfoItem Create(GameObject prefab, Transform parent, int itemIndex)
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
		var item = go.GetComponent(typeof(GUIBreakInfoItem)) as GUIBreakInfoItem;
		if (item == null)
			return null;
		// 値初期化
		item.ClearValue();

		return item;
	}
	public void ClearValue()
	{
		this.ClearIconSprite();
	}
	#endregion

	#region セットアップ
	public void Setup(Info info, CharaIcon charaIcon)
	{
		this.ItemInfo = info;

		// UI更新
		{
			var c = GUIBreakInfo.GetColorInfo();
			var t = this.Attach;
			var winNameColor = (info.isWinMyteam ? c.myteamLabel : c.enemyLabel);
			var winFrameColor = (info.isWinMyteam ? c.myteamFrame : c.enemyFrame);
			var loseNameColor = (!info.isWinMyteam ? c.myteamLabel : c.enemyLabel);
			var loseFrameColor = (!info.isWinMyteam ? c.myteamFrame : c.enemyFrame);
			// 勝利チーム設定
			if (t.winLabel != null)
			{
				t.winLabel.text = info.winName;
				t.winLabel.color = winNameColor;
			}
			if (t.winFrameSprite != null)
				t.winFrameSprite.color = winFrameColor;
			// 敗北チーム設定
			if (t.loseLabel != null)
			{
				t.loseLabel.text = info.loseName;
				t.loseLabel.color = loseNameColor;
			}
			if (t.loseFrameSprite != null)
				t.loseFrameSprite.color = loseFrameColor;
		}

		// アイコン設定
		this.SetIconSprite(charaIcon);
	}
	#endregion

	#region アイコン設定
	void ClearIconSprite()
	{
		this.SetWinIconSprite(null, "");
		this.SetLoseIconSprite(null, "");
	}
	void SetIconSprite(CharaIcon charaIcon)
	{
		if (charaIcon == null)
			return;
		charaIcon.GetIcon(this.ItemInfo.winAvatarType, this.ItemInfo.winSkinId, true, this.SetWinIconSprite);
		charaIcon.GetMonoIcon(this.ItemInfo.loseAvatarType, this.ItemInfo.loseSkinId, true, this.SetLoseIconSprite);
	}
	void SetWinIconSprite(UIAtlas atlas, string spriteName)
	{
		this.SetIconSprite(atlas, spriteName, this.Attach.winIconSprite, this.ItemInfo.winAvatarType);
	}
	void SetLoseIconSprite(UIAtlas atlas, string spriteName)
	{
		this.SetIconSprite(atlas, spriteName, this.Attach.loseIconSprite, this.ItemInfo.loseAvatarType);
	}
	void SetIconSprite(UIAtlas atlas, string spriteName, UISprite sp, AvatarType avatarType)
	{
		if (sp == null)
			return;

		// アトラス設定
		sp.atlas = atlas;
		// スプライト設定
		sp.spriteName = spriteName;

		// アトラス内にアイコンが含まれているかチェック
		if (sp.GetAtlasSprite() == null)
		{
			// アトラスとスプライト名が両方共設定されていてスプライトがない場合はエラー扱いｌ
			if (atlas != null && !string.IsNullOrEmpty(spriteName))
			{
				Debug.LogWarning(string.Format(
					"SetIconSprite:\r\n" +
					"Sprite Not Found!! AvatarType = {0} SpriteName = {1}", avatarType, spriteName));
			}
		}
	}
	#endregion
}
