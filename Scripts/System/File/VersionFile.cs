/// <summary>
/// アセットバンドルのバージョン管理.
/// 
/// 2014/06/03
/// </summary>
using System;
using System.IO;
using System.Net;
using System.Collections.Generic;

abstract public class VersionFile
{
	#region Enum定義.
	public enum ReadState
	{
		// 0未満をエラーとし,悪い状態ほど低い値とする.
		Failure_Decord = -9,
		Failure_Read     = -5,
		Non            =  0,
		Reading        =  3,
		Success        =  9,
	}
	#endregion

	// リソースファイルおよびバージョン管理ファイルのURLフォルダ.
	protected abstract string Url { get; }
	// バージョン管理ファイルの名前.
	protected abstract string FileName { get; }
	// ローカルの保存用パス.
	protected abstract string Path { get; }
	// JSONデータの親キー.
	protected abstract string JsonKey_Root { get; }
	// キーに追加する拡張子など.
	protected abstract string Ext { get; }

	/// <summary>
	/// oldVer,newVerのうち進捗が悪い方のStateを返す.
	public ReadState ReadindState
	{
		get
		{
			if(oldVer.ReadindState < newVer.ReadindState)
			{
				return oldVer.ReadindState;
			}
			else
			{
				return newVer.ReadindState;
			}
		}
	}
	private VersionDataFromFile oldVer;
	private VersionDataFromWWW newVer;

	#region メソッド.
	public VersionFile()
	{
		this.oldVer = new VersionDataFromFile(Path, FileName, JsonKey_Root);
		this.newVer = new VersionDataFromWWW(Url+FileName, JsonKey_Root);
	}

	/// <summary>
	/// oldVer,newVerの読み込みを開始する.
	/// </summary>
	public bool Read()
	{
		newVer.ReadAsync();
		return oldVer.Read();
	}

	/// <summary>
	/// バージョンを見比べて更新が必要なファイルの名前を返す.
	/// </summary>
	private List<string> GetDLFileNames()
	{
		var nameList = new List<string>();

		foreach(var key in this.newVer.GetBundleNames())
		{
			if(FileBase.IsExistFile(Path, key+Ext))
			{
				// 既にアセットバンドルが存在している.
				float nVerNum = this.newVer[key].Version;
				float oVerNum = 0;
				VersionCheckParam oVer = oldVer[key];
				if(oVer != null)
				{
					oVerNum = oVer.Version; 
				}
				if(oVerNum != nVerNum)
				{
					// バージョンが古い,または不明の場合.
					nameList.Add(key);
				}
			}
			else
			{
				// アセットバンドルを持っていない.
				nameList.Add(key);
			}
		}

		return nameList;
	}

	/// <summary>
	/// AssetBundleVersions.GetDLFileNames()で取得したファイル名をDownloadParamに変換する.
	/// </summary>
	public List<DownloadParam> GetDownLoadParam()
	{
		List<string> fileNameList = this.GetDLFileNames();
		
		var dlList = new List<DownloadParam>();
		foreach(var fileName in fileNameList)
		{
			dlList.Add(new DownloadParam(Url, Path, fileName+Ext));
		}
		
		return dlList;
	}

#if XW_DEBUG
	public float GetNewVersion(string fileName)
	{
		var key = System.IO.Path.GetFileName(fileName);
		if(key.EndsWith(Ext))
		{
			key = key.Substring(0, key.Length - Ext.Length);
		}
		return GetVersion(newVer[key]); 
	}
	public float GetOldVersion(string fileName)
	{
		var key = System.IO.Path.GetFileName(fileName);
		if(key.EndsWith(Ext))
		{
			key = key.Substring(0, key.Length - Ext.Length);
		}
		float oVerNum = -1;
		if(FileBase.IsExistFile(Path, fileName))
		{
			// 既にアセットバンドルが存在している.
			VersionCheckParam oVer = oldVer[key];
			oVerNum = GetVersion(oVer); 
		}
		
		return oVerNum;
	}
	private float GetVersion(VersionCheckParam ver)
	{
		return ver != null ? ver.Version : -1;
	}
#endif

	const int CRC32Retry = 3;
	/// <summary>
	/// CRC32によるチェックを行い,チェックが通ればバージョンファイルを更新する.
	/// </summary>
	public bool Check(string fileName)
	{
		var key = System.IO.Path.GetFileName(fileName);
		if(key.EndsWith(Ext))
		{
			key = key.Substring(0, key.Length - Ext.Length);
		}
		var newParam = this.newVer[key];
		if(newParam != null)
		{
			for(int i = 0; i < CRC32Retry; ++i)
			{
				// CRC32チェック.
				if(VersionUtil.CheckCRC32(Path, key+Ext, newParam.CRC32UINT))
				{
					// バージョンファイルを更新.
					this.oldVer.Merge(newParam);
					return true;
				}
			}

			// CRCチェックを通らない.
			this.oldVer.Remove(newParam.Name);
		}
		else
		{
			// CRCデータが見つからない.
			BugReportController.SaveLogFile("keyName not match " + key);
		}

		return false;
	}
	#endregion
	
	#region VersionData

	#region VersionData
	/// <summary>
	/// バージョンデータ管理基本クラス.
	/// </summary>
	abstract class VersionData : FileBase
	{
		// JSONデータの親キー.
		private readonly string JsonKey_Root;

		// 読み込み状態.
		public ReadState ReadindState { get; protected set; }
		// バージョンデータ.
		protected Dictionary<string, VersionCheckParam> verDic;
		// バージョンデータ取得用プロパティ.
		public VersionCheckParam this[string key]
		{
			get
			{
				VersionCheckParam ret;
				this.verDic.TryGetValue(key, out ret);
				return ret;
			}
		}
		
		public VersionData(string jsonKey_Root)
		{
			JsonKey_Root = jsonKey_Root;
			ReadindState = VersionFile.ReadState.Non;
		}

		/// <summary>
		/// アセットバンドルの管理名一覧を返す.
		/// </summary>
		public List<string> GetBundleNames()
		{
			return new List<string>(verDic.Keys);
		}

		// FileBase継承.読み込み後に呼ばれるメソッド.
		protected override void Decode(string encodeString)
		{
			DecodeVersionData(encodeString);
		}

		/// <summary>
		/// JSONデータを読み込む.
		/// </summary>
		protected void DecodeVersionData(string jsonStr)
		{
			verDic = new Dictionary<string, VersionCheckParam>();
			try
			{
				var json = new JSONObject(jsonStr);
				var version = json.GetField(JsonKey_Root);
				foreach(var key in version.keys)
				{
					verDic.Add(key, new VersionCheckParam(key, version.GetField(key)));
				}
				this.ReadindState = VersionFile.ReadState.Success;
			}
			catch(Exception e)
			{
				this.ReadindState = VersionFile.ReadState.Failure_Decord;
				BugReportController.SaveLogFile(e.ToString());
			}
		}

		/// <summary>
		/// JSONデータをファイルに書き込む.
		/// </summary>
		protected override void Encode(out string encodeString)
		{
			var json = new JSONObject();
			var version = new JSONObject();
			
			json.AddField(JsonKey_Root, version);
			foreach(var ver in verDic)
			{
				version.AddField(ver.Key, ver.Value.GetJsonObject());
			}
			encodeString = json.ToString();;
		}
	}
	#endregion

	#region VersionDataFromFile
	/// <summary>
	/// ファイルからバージョンデータを読み込む.
	/// </summary>
	class VersionDataFromFile : VersionData
	{
		const int ReadRetry = 3;

		private readonly string Path;
		private readonly string FileName;

		public VersionDataFromFile(string path, string fileName, string jsonKey_Root)
			: base(jsonKey_Root)
		{
			this.Path = path;
			this.FileName = fileName;
		}

		/// <summary>
		/// 読み込み.
		/// </summary>
		public bool Read()
		{
			this.ReadindState = VersionFile.ReadState.Reading;
			for(int i = 0; i < ReadRetry; ++i)
			{
				try
				{
					// 決められたパスに存在するファイルを読み込む.
					if(base.Read(Path, FileName))
					{
						break;
					}
					else
					{	// ファイルが存在しない(アプリDL後初回起動の場合は正常).
						this.verDic = new Dictionary<string, VersionCheckParam>();
						this.ReadindState = VersionFile.ReadState.Success;
						break;
					}
				}
				catch(Exception e)
				{
					this.ReadindState = VersionFile.ReadState.Failure_Read;
					BugReportController.SaveLogFile(e.ToString());
				}
			}

			if(this.ReadindState < VersionFile.ReadState.Non)
			{
				// 読み込み失敗.データが壊れている可能性が高いので削除する.
				FileBase.Delete(Path, FileName);
				return false;
			}
			return true;
		}

		/// <summary>
		/// バージョン情報を更新して書き込む.
		/// </summary>
		public void Merge(VersionCheckParam param)
		{
			verDic[param.Name] = param;
			
			base.Write(Path, FileName);
		}
		/// <summary>
		/// バージョン情報を削除して書き込む.
		/// </summary>
		public void Remove(string name)
		{
			verDic.Remove(name);

			base.Write(Path, FileName);
		}
	}
	#endregion

	#region VersionDataFromWWW
	/// <summary>
	/// WWWからバージョンデータを読み込む.
	/// </summary>
	class VersionDataFromWWW : VersionData
	{
		private readonly string Url;

		public VersionDataFromWWW(string url, string jsonKey_Root)
			: base(jsonKey_Root)
		{
			this.Url = url;
		}
		/// <summary>
		/// 非同期読み込み.
		/// </summary>
		public void ReadAsync()
		{
			this.ReadindState = VersionFile.ReadState.Reading;

			// コールバックを設定して読み込み開始.
			WebClient w = new System.Net.WebClient();
			w.DownloadStringCompleted += Event_CompleteDownloadString;
			w.DownloadStringAsync(new Uri(Url));
		}

		/// <summary>
		/// 読み込み完了時に呼ばれるイベント(読み込み開始と同じスレッド).
		/// </summary>
		private void Event_CompleteDownloadString(object sender, DownloadStringCompletedEventArgs e)
		{
			try
			{
				if(e != null)
				{
					if(e.Error != null)
					{
						// エラー.
						this.ReadindState = VersionFile.ReadState.Failure_Read;
						//BugReportController.SaveLogFile("AssetBundleVer DownloadString\r\n"+e.Error.ToString());
					}
					else if(e.Cancelled == true)
					{
						// エラー.
						this.ReadindState = VersionFile.ReadState.Failure_Read;
					}
					else
					{
						// 文字列をデコード.
						this.DecodeVersionData(e.Result);
					}
				}
			}
			finally
			{
				var w = sender as WebClient;
				if(w != null)
				{
					// 破棄.
					w.Dispose();
				}
			}
		}
	}
	#endregion
	#endregion

	#region VersionCheckParam
	/// <summary>
	/// バージョンチェックパラメータ.
	/// </summary>
	class VersionCheckParam
	{
		// JSONキー.
		private const string JSONKEY_version = "version";
		private const string JSONKEY_crc = "crc";
		
		// 管理名(拡張子を省略したファイル名).
		public string Name { get; private set; }
		// バージョン番号.
		public float Version { get; private set; }
		// CRC値(数値が大きく,JSONObjectデフォルトのfloatでは管理できない).
		public string CRC32 { get; private set; }
		public uint CRC32UINT
		{
			get
			{
				uint crc32;
				if(uint.TryParse(this.CRC32, out crc32))
				{
					return crc32;
				}
				else
				{
					BugReportController.SaveLogFile("TryParse failed. " + Name + "," + this.CRC32);
					return 0;
				}
			}
		}

		public VersionCheckParam(string name, JSONObject jObj)
		{
			this.Name = name;
			this.Version = jObj.GetField(JSONKEY_version).f;
			this.CRC32 = jObj.GetField(JSONKEY_crc).str;
		}
		
		/// <summary>
		/// パラメータをJSONObjectに書き戻す.
		/// </summary>
		public JSONObject GetJsonObject()
		{
			var json = new JSONObject();
			json.AddField(JSONKEY_version, Version);
			json.AddField(JSONKEY_crc, CRC32);
			return json;
		}
	}
	#endregion
}

/// <summary>
/// アセットバンドルバージョンチェックに関する個別機能.
/// </summary>
static public class VersionUtil
{
	/// <summary>
	/// CRC32によるアセットバンドルのチェックサムを行う.
	/// </summary>
	static public bool CheckCRC32(string dir, string fileName, uint crc32)
	{
		string path = FileBase.GetFilePath(dir, fileName);
		if(File.Exists(path))
		{
			using(var fs = File.Open(path, System.IO.FileMode.Open))
			{
				uint comCRC32 = CRC32.ComputeHashCRC32(fs);
				if(comCRC32 == crc32)
				{
					return true;
				}
				else
				{
                    // CRCチェックを通らない.
                    BugReportController.SaveLogFileWithOutStackTrace(fileName + " " + crc32 + ":" + comCRC32);
                    UnityEngine.Debug.LogError(fileName + " " + crc32 + ":" + comCRC32);
                    UnityEngine.Debug.LogError("CRC check fail:" + fileName);
				}
			}
		}
		return false;
	}
}