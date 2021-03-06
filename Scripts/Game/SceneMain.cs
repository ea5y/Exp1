/// <summary>
/// 認証メイン処理
/// 
/// 2013/10/25
/// </summary>
using UnityEngine;
using System.Collections;

public abstract class SceneMain<T> : MonoBehaviour, ISceneMain where T : MonoBehaviour
{
	public static T Instance { get; private set; }

	#region 初期化
	protected virtual void Awake()
	{
		if (SceneMain<T>.Instance != null)
		{
			Debug.LogError(typeof(T) + ":既にインスタンスが存在しています");
		}
		SceneMain<T>.Instance = this as T;
		SceneController.SetNowScene(this);
	}
	#endregion

	#region ISceneMain
	/// <summary>
	/// 通信切断時の処理.trueを返すとタイトルへ戻る.
	/// シーン開始時に通信が切れていた場合も呼ばれる.
	/// </summary>
	public abstract bool OnNetworkDisconnect();
	
	/// <summary>
	/// サーバからによる通信切断時の処理
	/// </summary>
	public abstract void OnNetworkDisconnectByServer();
	#endregion
}

public interface ISceneMain
{
	/// <summary>
	/// 通信切断時の処理.trueを返すとタイトルへ戻る.
	/// シーン開始時に通信が切れていた場合も呼ばれる.
	/// </summary>
	bool OnNetworkDisconnect();

	/// <summary>
	/// サーバからによる通信切断時の処理
	/// </summary>
	void OnNetworkDisconnectByServer();
}
