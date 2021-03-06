/// <summary>
/// UIデバッグパラメータ基底クラス
/// 
/// 2015/12/22
/// </summary>
#if UNITY_EDITOR && XW_DEBUG
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface IDebugParamEvent
{
	void Update();
}

[System.Serializable]
public abstract class GUIDebugParamBase
{
	/// <summary>
	/// 閉じるイベント
	/// </summary>
	public event System.Action ExecuteClose = delegate { };
	[SerializeField]
	bool executeClose = false;

	/// <summary>
	/// アクティブ化イベント
	/// </summary>
	public event System.Action ExecuteActive = delegate { };
	[SerializeField]
	bool executeActive = false;

	// イベントリスト
	List<IDebugParamEvent> EventList { get; set; }
	// マスターデータ読み込みフラグ
	bool IsReadMasterData { get; set; }

	/// <summary>
	/// コンストラクタ
	/// </summary>
	public GUIDebugParamBase()
	{
		this.EventList = new List<IDebugParamEvent>();
		this.IsReadMasterData = false;
	}

	/// <summary>
	/// イベント追加
	/// </summary>
	public void AddEvent(IDebugParamEvent e)
	{
		this.EventList.Add(e);
	}

	/// <summary>
	/// 更新処理
	/// </summary>
	public void Update()
	{
		if (this.executeClose)
		{
			this.executeClose = false;
			FiberController.AddFiber(this.DebugCloseCoroutine());
		}
		if (this.executeActive)
		{
			this.executeActive = false;
			this.ExecuteActive();
		}

		this.EventList.ForEach(a => { a.Update(); });
	}

	/// <summary>
	/// 閉じる
	/// OnValidate から直接 GameObject.SetActive(false) にしてしまうと
	/// エラーになるため1フレーム遅らせる
	/// </summary>
	IEnumerator DebugCloseCoroutine()
	{
		this.ExecuteClose();
		yield break;
	}

	/// <summary>
	/// マスターデータ読み込み
	/// </summary>
	public bool ReadMasterData()
	{
		string err = null;
		if (UnityEngine.Object.FindObjectOfType(typeof(FiberController)) == null)
			err += "FiberController.prefab を入れて下さい\r\n";
		if (MasterData.Instance == null)
			err += "MasterData.prefab を入れて下さい\r\n";
		if (!string.IsNullOrEmpty(err))
		{
			Debug.LogWarning(err);
			return false;
		}

		if (!this.IsReadMasterData)
		{
			MasterData.Read();
			this.IsReadMasterData = true;
		}
		return this.IsReadMasterData;
	}
}
#endif
