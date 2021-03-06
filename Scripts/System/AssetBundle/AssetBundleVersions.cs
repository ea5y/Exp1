/// <summary>
/// アセットバンドルのバージョン管理.
/// 
/// 2014/06/03
/// </summary>
using System;
using System.IO;
using System.Net;
using System.Collections.Generic;

public class AssetBundleVersions : VersionFile
{
	#region 定数.
	// アセットバンドルおよびバージョン管理ファイルのURLフォルダ.
	public string URL { get { return Url; } }
	protected override string Url
	{
		get
		{
            return ObsolateSrc.AssetBundleURL;
		}
	}

	// バージョン管理ファイルの名前.
	public const string FILENAME = "XwAssetBundleVer.json";
	protected override string FileName { get { return FILENAME; } }
	
	// ローカルの保存用パス.
	public const string PATH = "Bundle";
    public const string UNUSED_PATH = "Bundle_Unused";
	protected override string Path { get { return PATH; } }

	// JSONデータの親キー.
	protected const string JSONKEY_ROOT = "XwAssetBundleVer";
	protected override string JsonKey_Root { get { return JSONKEY_ROOT; } }

	// キーに追加する拡張子など.
	protected override string Ext { get { return string.Empty; } }
	#endregion
}