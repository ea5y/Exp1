/// <summary>
/// アイテムアイコン
/// 
/// 2016/03/14
/// </summary>
using UnityEngine;
using System;
using Scm.Common.XwMaster;

[Serializable]
public class ItemIcon
{
	#region フィールド&プロパティ
	/// <summary>
	/// 複数のアセットバンドルの管理
	/// </summary>
	private BundleDataManager BundleDataManager { get; set; }
	#endregion

	#region 初期化
	/// <summary>
	/// コンストラクタ
	/// </summary>
	public ItemIcon() { this.MemberInit(); }

	/// <summary>
	/// クリア
	/// </summary>
	public void Clear()
	{
		this.MemberInit();
	}

	/// <summary>
	/// メンバー初期化
	/// </summary>
	private void MemberInit()
	{
		this.BundleDataManager = new BundleDataManager();
	}
	#endregion

	#region アイコン取得
	/// <summary>
	/// アイテムアイコンを取得する
	/// </summary>
	public void GetItemIcon(int itemMasterID, bool keepAssetReference, System.Action<UIAtlas, string> callback)
	{
		// アイテムアイコンを取得
		string iconName;
		string bundleName;
		if(!this.TryGetItemIconName(itemMasterID, out iconName, out bundleName))
		{
			Debug.LogWarning(string.Format("Failure SetItemIconName itemMasterID={0}", itemMasterID));
			if (callback != null)
				callback(null, "");
			return;
		}

		// アトラス取得
		var bundleData = this.BundleDataManager.GetBundleData<StateIconBundleData>(bundleName);
		if (bundleData != null)
		{
			bundleData.GetStateIconResource(bundleName, true,
								(UIAtlas res) =>
								{
									if (callback != null)
										callback(res, iconName);
								});
		}
	}

	/// <summary>
	/// アイコンのスプライト設定
	/// </summary>
	public static bool SetIconSprite(UISprite setSp, UIAtlas atlas, string spriteName)
	{
		if (setSp == null)
			return false;

		// アトラス設定
		setSp.atlas = atlas;
		// スプライト設定
		setSp.spriteName = spriteName;

		// アトラス内にアイコンが含まれているかチェック
		if (setSp.GetAtlasSprite() == null)
		{
			// アトラスとスプライト名が両方共設定されていてスプライトがない場合はエラー扱いｌ
			if (atlas != null && !string.IsNullOrEmpty(spriteName))
			{
				Debug.LogWarning(string.Format(
					"ItemIcon.SetIconSprite:\r\n" +
					"Sprite Not Found!! SpriteName = {0}", spriteName));
				return false;
			}
		}

		return true;
	}
	#endregion

	#region アイテムアイコン情報
	/// <summary>
	/// アイテムアイコン名とバンドル名を取得する
	/// </summary>
	private bool TryGetItemIconName(int itemMasterID, out string iconName, out string bundleName)
	{
		iconName = string.Empty;
		bundleName = string.Empty;

		// アイテムマスターデータ取得
		ItemMasterData masterData;
		bool isSuccess = MasterData.TryGetItem(itemMasterID, out masterData);
		if(isSuccess)
		{
			iconName = masterData.IconFile;
			bundleName = masterData.IconAssetPath;
		}

		return isSuccess;
	}
	#endregion
}

/// <summary>
/// アセットバンドル内のアイテムアイコンデータ
/// </summary>
public class ItemIconBundleData : BundleData
{
	/// <summary>
	/// アセットバンドル内のアイコンパス
	/// </summary>
	private const string ItemIconPath = "Atlas.prefab";

	/// <summary>
	/// アイテムアイコン
	/// </summary>
	private UIAtlas iconAtlas = null;

	/// <summary>
	/// 全てのリソースを読み込んでいるかどうか
	/// </summary>
	protected override bool IsFinishAllResource()
	{
		if (this.iconAtlas == null)
			return false;
		return true;
	}

	/// <summary>
	/// アイテムアイコン取得
	/// </summary>
	public void GetItemIcon(string bundleName, bool keepAssetReference, System.Action<UIAtlas> callback)
	{
		if(this.iconAtlas == null)
		{
			this.GetAssetAsync<UIAtlas>(bundleName, ItemIconPath, keepAssetReference,
					(UIAtlas resource) =>
					{
						this.iconAtlas = resource;
						if (callback != null)
							callback(resource);
					});
		}
		else
		{
			if (callback != null)
				callback(this.iconAtlas);
		}
	}
}
