/// <summary>
/// 状態アイコン(バフやデバフの戦闘中の状態アイコン)
/// 
/// 2014/09/25
/// </summary>
using UnityEngine;
using System.Collections;
using System;

public class StateIcon
{
	#region 定数

	/// <summary>
	/// アセットバンドル内のバンドル名
	/// </summary>
	private const string BundleName = "ui_ic_st_b001";

	#endregion

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
	public StateIcon()
	{
		Init();
	}

	/// <summary>
	/// 読み込んだアイコンを削除する
	/// </summary>
	public void Clear()
	{
		Init();
	}

	/// <summary>
	/// 読み込んだアイコン等を初期化する
	/// </summary>
	private void Init()
	{
		this.BundleDataManager = new BundleDataManager();
	}

	#endregion

	#region アイコン取得

	/// <summary>
	/// 状態アイコンアトラス取得
	/// </summary>
	public void GetIcon(System.Action<UIAtlas> callback)
	{
		var bundleData = this.BundleDataManager.GetBundleData<StateIconBundleData>(BundleName);
		if (bundleData != null)
		{
			bundleData.GetStateIconResource(BundleName, true,
							(UIAtlas res) =>
							{
								if (callback != null)
									callback(res);
							});
		}
	}

	#endregion
}


/// <summary>
/// アセットバンドル内の状態アイコンデータ
/// </summary>
public class StateIconBundleData : BundleData
{
	#region 定数

	/// <summary>
	/// アセットバンドル内のアイコンパス
	/// </summary>
	private const string IconAssetPath = "Atlas.prefab";

	#endregion

	#region フィールド&プロパティ

	/// <summary>
	/// 取得するアトラス
	/// </summary>
	private UIAtlas iconAtlas = null;

	#endregion

	#region アセットリソース取得

	/// <summary>
	/// アイコンアセットリソースを取得する
	/// </summary>
	public void GetStateIconResource(string bundleName, bool keepAssetReference, System.Action<UIAtlas> callback)
	{
		if (this.iconAtlas == null)
		{
			this.GetAssetAsync<UIAtlas>(bundleName, IconAssetPath, keepAssetReference,
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

	#endregion

	#region override IsFinishAllResource

	/// <summary>
	/// 全てのリソースを読み込んでいるかどうか
	/// </summary>
	protected override bool IsFinishAllResource()
	{
		if (this.iconAtlas == null)
			return false;

		return true;
	}

	#endregion
}