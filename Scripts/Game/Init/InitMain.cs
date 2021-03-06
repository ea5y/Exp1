/// <summary>
/// 初期化メイン処理
/// 
/// 2013/01/07
/// </summary>
using UnityEngine;
using System.Collections;

public class InitMain : SceneMain<InitMain>
{
	#region フィールド＆プロパティ
	public GameObject[] dontDestroyObjects;
	#endregion

	#region MonoBehaviourリフレクション
	protected override void Awake()
	{
		base.Awake();

		if(RestrictBoot())
		{
			return;
		}

#if XW_DEBUG
		// デバッグファイル読み込み
		try
		{
			if (DebugFile.Instance.Read()) {
                Debug.Log("<color=#ff0000>Debug file read success</color>");
            }
			ScmParam.Debug.File = DebugFile.Instance.Clone();
		}
		catch (System.Exception e)
		{
			Debug.LogWarning(DebugFile.Filename + "でエラー\r\n" + e);
		}
#endif

		// コンフィグファイル読み込み
		try
		{
			ConfigFile.Instance.Read();
		}
		catch (System.Exception e)
		{
			Debug.LogWarning(ConfigFile.Filename + "でエラー\r\n" + e);
		}

		// シーンで残しておきたいもの
		foreach(GameObject go in this.dontDestroyObjects)
		{
			Object.DontDestroyOnLoad(go);
		}

#if XW_DEBUG
		// デバッグシーン追加
		DebugMain.SceneAdditive();
#endif
	}

	void Start()
	{
		// 言語設定
		Scm.Common.Utility.Language = ApplicationController.Language;

		// サウンド音量設定
		SoundController.Bgm_Volume = ConfigFile.Option.Bgm;
		SoundController.Se_Volume = ConfigFile.Option.Se;
        //if (Scm.Common.Utility.Language == Scm.Common.GameParameter.Language.Japanese)
        //{
        //	SoundController.Voice_Volume = ConfigFile.Option.Voice;
        //}
        //else
        //{
        //	// 日本以外はボイスオフ
        //	SoundController.Voice_Volume = 0f;
        //}
        // 中国版はボイスオフ
#if PLATE_NUMBER_REVIEW
        SoundController.Voice_Volume = 0;
#else
        SoundController.Voice_Volume = ConfigFile.Option.Voice;
#endif

		// シーン切り替え
		AuthMain.LoadScene();
	}
	#endregion

	static bool RestrictBoot()
	{
		return false;
		//// 言語チェック
		//if(Application.systemLanguage != SystemLanguage.Japanese)
		//{
		//	GUISystemMessage.SetModeOK(
		//		"Error",
		//		MasterData.GetTextDefalut(TextType.TX153_SystemLanguage_NotJapanese, ObsolateSrc.Defalut_TX153_SystemLanguage_NotJapanese),
		//		Application.Quit);
		//	return true;
		//}
		
		//// ルート権限チェック.
		//return AndroidRootCheck.RootCheck();
	}

	#region SceneMain
	public override bool OnNetworkDisconnect()
	{
		return false;
	}

	public override void OnNetworkDisconnectByServer() { }
	#endregion
}
