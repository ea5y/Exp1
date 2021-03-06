/// <summary>
/// AssetResource の管理クラス
/// 同じ種類のアセットバンドルを複数管理する
/// 
/// 2014/07/16
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


#region obsolate
[System.Obsolete]
public class AssetResourceManager<D> where D : BundleDataOld
{
	/// <summary>
	/// アセットバンドルをキーにしたアセットバンドル内のリソースデータディクショナリ
	/// </summary>
	Dictionary<string, D> _bundleDataDict = null;
	Dictionary<string, D> BundleDataDict
	{
		set { _bundleDataDict = value; }
		get
		{
			if (_bundleDataDict == null)
				_bundleDataDict = new Dictionary<string, D>();
			return _bundleDataDict;
		}
	}

	/// <summary>
	/// AssetResource 取得メソッド
	/// AssetResource が既にある場合はアセットバンドルは読み込み中か読み込みが完了している
	/// AssetResource がない場合は AssetResource を新しく作成してアセットバンドルの読み込みを開始する
	/// </summary>
	public AssetResource<T> GetAssetResource<T>(string bundleName, string resourcePath, bool keepAssetReference,
		System.Func<D> createBundleDataFunc,
		System.Func<D, AssetResource<T>> getAssetResourceFunc,
		System.Action<D, AssetResource<T>> setAssetResourceFunc) where T : Object
	{
		AssetResource<T> assetResource = null;

		// アセットバンドル名をキーにデータを取得する
		D data;
		if (!this.BundleDataDict.TryGetValue(bundleName, out data))
		{
			// ない場合は新しく作成して登録する
			data = createBundleDataFunc();
			this.BundleDataDict.Add(bundleName, data);
		}

		// 保持している AssetResource があるかどうか
		assetResource = getAssetResourceFunc(data);
		if (assetResource != null)
		{
			// 既にあるのでそれを利用する
			// リソースは読み込み中か読み込みが完了している
			return assetResource;
		}

		// リソースを新しく作成する
		AssetReference assetReference = data.GetAssetReference(bundleName, keepAssetReference);
		assetResource = new AssetResource<T>(assetReference, resourcePath, (AssetResource<T> ar, T resource) =>
		{
			// 読み込み終了
			if (resource == null)
			{
				// リソースが読み込めなかった
				Debug.LogWarning(string.Format(
					"Resource Loading Error:\r\n" +
					"bundleName = {0}, assetPath = {1}", bundleName, ar.AssetPath));
			}
		});
		// 新しく作成した AssetResource を保持する
		setAssetResourceFunc(data, assetResource);

		// アセットバンドル内のすべてのリソースを読み込んでいる場合は
		// AssetReference を破棄する
		data.AssetReferenceClearCheck();

		return assetResource;
	}
}





/// <summary>
/// アセットバンドル内のリソースデータ基底クラス
/// </summary>
public abstract class BundleDataOld
{
	/// <summary>
	/// 読み込んだアセットバンドルを保持しておく用
	/// </summary>
	AssetReference AssetReference { get; set; }

	public BundleDataOld() { }

	/// <summary>
	/// 保持してある AssetReference の取得
	/// 保持してなければ作成する
	/// </summary>
	public AssetReference GetAssetReference(string bundleName, bool keepAssetReference)
	{
		// 保持している AssetReference があるかどうか
		AssetReference assetReference = this.AssetReference;
		if (assetReference == null)
		{
			// アセットバンドルの読み込み開始
			assetReference = AssetReference.GetAssetReference(bundleName);
			// AssetReference を保持しておくかどうか
			// アセットバンドル内のリソースを一つだけ読み込んで使用する場合は false の方がいい
			// 同じアセットバンドル内で他のリソースも読み込む場合は true の方がいい
			if (keepAssetReference)
				this.AssetReference = assetReference;
		}

		return assetReference;
	}

	/// <summary>
	/// AssetReference の破棄チェック
	/// 全てのリソースを読み込んでいる場合は不要になるので破棄する
	/// </summary>
	public void AssetReferenceClearCheck()
	{
		if (this.AssetReference == null)
			return;
		if (!this.IsFinishAllResource())
			return;

		this.AssetReference = null;
	}

	/// <summary>
	/// 全てのリソースを読み込んでいるかどうか
	/// </summary>
	protected abstract bool IsFinishAllResource();
}
#endregion
