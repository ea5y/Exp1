/// <summary>
/// Android端末でRoot権限を持っているかのチェック(Avabelのソース改変)
/// 
/// 2015/04/01
/// </summary>

using UnityEngine;
using System.Collections;

public class AndroidRootCheck
{
	static public bool RootCheck()
	{
//#if UNITY_ANDROID && !UNITY_EDITOR
//		if (HasRootPrivileges()/* || IsSUPackOrTestOS()*/)
//		{
//			//ルート化されていた場合の処理
//			GUISystemMessage.SetModeOK("エラー", MasterData.GetText(TextType.TX152_RootPrivileges_Error), Application.Quit);
//			return true;
//		}
//#endif
		return false;
	}

	/// <summary>
	/// Root権限を持っているか(suコマンドが使用可能か)どうかをチェックする
	/// </summary>
	static bool HasRootPrivileges()
	{
		System.Diagnostics.Process proc = null;
		try
		{
			proc = new System.Diagnostics.Process();
			proc.StartInfo.FileName = "su";
			if (proc.Start())
			{
				//終了まで待機
				proc.WaitForExit();
			}
			BugReportController.SaveLogFileWithOutStackTrace("root");
			return true;
		}
		catch
		{
			//権限が無ければ例外になる
			return false;
		}
		finally
		{
			//念のため開放処理
			if (proc != null)
			{
				proc.Close();
				proc.Dispose();
			}
		}
	}

/*
	/// <summary>
	/// スーパーユーザー用のパッケージがインストールされているかチェックする
	/// </summary>
	/// <returns>
	/// インストール済み:true / 未インストール:false
	/// </returns>
	bool IsSUPackOrTestOS()
	{
		//Javaクラス参照
		AndroidJavaClass javaClass = new AndroidJavaClass("com.asobimo.avabel.AndroidPackageSignature");	
		
		//呼び出し
		if (javaClass.CallStatic<bool>("isSUPack") ||
			javaClass.CallStatic<bool>("isTestOS"))
		{
			return true;
		}
		
		return false;
	}
*/
}