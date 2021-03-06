/// <summary>
/// 設定ファイル
/// 
/// 2014/04/22
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SettingFile : FileBase
{
	// 2016/07/04 完全オミット
#if false
	#region フィールド＆プロパティ
	public const string URL = "http://59.106.106.131:30300/"//ObsolateSrc.SettingURL;
	public const string Path = "Work";
	public const string Filename = "XwSettings.json";

	public static SettingFile Instance = new SettingFile();

	[SerializeField]
	private XwSettings data = new XwSettings();
	public  XwSettings Data { get { return data; } }
	#endregion

	#region 宣言
	/// <summary>
	/// Xw settings.
	/// </summary>
	[System.Serializable]
	public class XwSettings
	{
		public Server server = new Server();
		public Url url = new Url();
		public ForAndroid android = new ForAndroid();
		public ForIos ios = new ForIos();
	}
	/// <summary>
	/// Server
	/// </summary>
	[System.Serializable]
	public class Server
	{
		public string master = "";
		public string test = "";
		public void Decode(JSONObject json)
		{
			json.GetField(ref master, "Master",(name) => {ExceptionProc(name);});
			json.GetField(ref test, "Test", (name) => {ExceptionProc(name);});
		}
	}
	/// <summary>
	/// URL
	/// </summary>
	[System.Serializable]
	public class Url
	{
		public string masterData = "";
		public void Decode(JSONObject json)
		{
			json.GetField(ref masterData, "MasterData", (name) => {ExceptionProc(name);});
		}
	}
	/// <summary>
	/// For android.
	/// </summary>
	[System.Serializable]
	public class ForAndroid
	{
		public List<string> bundleVersionList = new List<string>();
		public void Decode(JSONObject json)
		{
			JSONObject jsonList = json.GetField("BundleVersion");
			bundleVersionList.Clear();
			foreach (var strJson in jsonList.list)
				bundleVersionList.Add(strJson.str);
		}
	}
	/// <summary>
	/// For ios.
	/// </summary>
	[System.Serializable]
	public class ForIos
	{
		public List<string> bundleVersionList = new List<string>();
		public void Decode(JSONObject json)
		{
			JSONObject jsonList = json.GetField("BundleVersion");
			bundleVersionList.Clear();
			foreach (var strJson in jsonList.list)
				bundleVersionList.Add(strJson.str);
		}
	}
	#endregion

	#region IO
	public bool Read()
	{
		bool isSuccess = this.Read(Path, Filename);
		if (!isSuccess)
			throw new System.Exception(string.Format("File Not Found \"{0}\"\r\n", FileBase.GetFilePath(Path, Filename)));
		return isSuccess;
	}
	protected override void Decode(string encodeString)
	{
		Decode(this.Data, new JSONObject(encodeString));
	}
	public static void Decode(XwSettings data, JSONObject json)
	{
		// フィールド読み込み
		JSONObject settingJson = json.GetField("XwSettings");
		{
			data.server.Decode(settingJson.GetField("Server"));
			data.url.Decode(settingJson.GetField("Url"));
			data.android.Decode(settingJson.GetField("ForAndroid"));
			data.ios.Decode(settingJson.GetField("ForIos"));
		}
	}
	public static void ExceptionProc(string name)
	{
		throw new System.Exception(string.Format("Not Found Field \"{0}\"\r\n", name));
	}
	public static void Delete()
	{
		Delete(Path, Filename);
	}
	#endregion
#endif
}
