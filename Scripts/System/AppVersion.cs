/// <summary>
/// アプリバージョン情報
/// 
/// 2015/03/22
/// </summary>
using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public static class AppVersion
{
	#region アプリバージョン
	[System.Obsolete("Use PluginController.PackageInfo.versionName")]
	public static string GetVersion()
	{
#if UNITY_EDITOR || UNITY_STANDALONE
		string versionName = "";
#elif UNITY_ANDROID
		string versionName = GetAppVersionName_Android();
#elif UNITY_IPHONE
		string versionName = IOSAppVersion.GetAppVersionName_iOS();
#else
		string versionName = "";
#endif
		return versionName;

	}
	#endregion

	#region Android
#if UNITY_ANDROID
	/// <summary>
	/// バージョンネームを取得する
	/// PlayerSettings上では[ Bundle Version ]の値
	/// </summary>
	static string GetAppVersionName_Android()
	{
		AndroidJavaObject pInfo = GetPackageInfo_Android();
		string versionName = pInfo.Get<string>("versionName");
		return versionName;
	}

	/// <summary>
	/// Version情報を保持しているPackageInfoクラスを取得する
	/// </summary>
	static AndroidJavaObject GetPackageInfo_Android()
	{
		AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject context = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity").Call<AndroidJavaObject>("getApplicationContext");
		AndroidJavaObject pManager = context.Call<AndroidJavaObject>("getPackageManager");
		AndroidJavaObject pInfo = pManager.Call<AndroidJavaObject>("getPackageInfo", context.Call<string>("getPackageName"), pManager.GetStatic<int>("GET_ACTIVITIES"));

		return pInfo;
	}
#endif //UNITY_ANDROID
	#endregion

	#region iOS
#if UNITY_IPHONE
	public class IOSAppVersion
	{
		[DllImport("__Internal")]
		private static extern string GetVersionName_();

		/// <summary>
		/// iOS版でのバージョンを取得する
		/// </summary>
		public static string GetAppVersionName_iOS()
		{
			string versionName = GetVersionName_();
			return versionName;
		}

	}
 #endif // #if UNITY_IPHONE
	#endregion
}
