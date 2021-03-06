/// <summary>
/// デバッグ処理
/// 
/// 2015/03/20
/// </summary>
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class DebugMain : MonoBehaviour
{
#if XW_DEBUG
	#region フィールド＆プロパティ
	const string SceneName = "ScmDebug";
	const float ActiveLongPressTime = 1f;

	// デバッグログウィンドウ表示時間
	float ActiveTime { get; set; }
	// シリアライズされていないメンバーの初期化
	void MemberInit()
	{
		this.ActiveTime = 0f;
	}
	#endregion

	#region シーン追加
	public static void SceneAdditive()
	{
		// 同期シーンロード命令.
		SceneManager.LoadScene(SceneName, LoadSceneMode.Additive);
	}
	public static IEnumerator SceneAdditiveAsync()
	{
		// 非同期シーンロード命令.
		AsyncOperation loadScene = SceneManager.LoadSceneAsync(SceneName, LoadSceneMode.Additive);
		loadScene.allowSceneActivation = false;
		
		loadScene.allowSceneActivation = true;
		// シーンロードが終わるまで待つ.
		while(!loadScene.isDone)
			yield return null;
	}
	#endregion

	#region 更新
	void Update()
	{
		//if (!Input.GetKey(KeyCode.Escape) && !Input.GetKey(KeyCode.Home))
		//{
		//	this.ActiveTime = Time.time;
		//}
		//if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Home))
		//{
		//	this.ActiveTime = Time.time + ActiveLongPressTime;
		//}
		//if (Time.time > this.ActiveTime)
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			GUIDebugLog.SetActive(true);
		}
	}
	#endregion
#endif
}
