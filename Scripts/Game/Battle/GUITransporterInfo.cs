/// <summary>
/// トランスポーター情報
/// 
/// 2014/11/18
/// </summary>
using UnityEngine;
using System.Collections;

public class GUITransporterInfo : Singleton<GUITransporterInfo>
{
	#region フィールド＆プロパティ
	/// <summary>
	/// タイマー表示フォーマット
	/// </summary>
	[SerializeField]
	string _timerFormat = "{0:00.000}";
	string TimerFormat { get { return _timerFormat; } }

	/// <summary>
	/// 初期化時のアクティブ状態
	/// </summary>
	[SerializeField]
	bool _isStartActive = false;
	bool IsStartActive { get { return _isStartActive; } }

	/// <summary>
	/// アタッチオブジェクト
	/// </summary>
	[SerializeField]
	AttachObject _attach;
	AttachObject Attach { get { return _attach; } }
	[System.Serializable]
	public class AttachObject
	{
		public UIPlayTween rootTween;
		public UISlider remainingSlider;
		public UILabel remainingLabel;
	}

	// アクティブ設定
	bool IsActive { get; set; }
	// タイマー
	float Timer { get; set; }
	// 残り時間
	float RemainingTime { get; set; }
	// スライダーの値
	float SliderValue { get { return (0f < this.Timer ? this.RemainingTime / this.Timer : 0f); } }

	// シリアライズされていないメンバー初期化
	void MemberInit()
	{
		this.IsActive = false;
		this.Timer = 0f;
		this.RemainingTime = 0f;
	}
	#endregion

	#region 初期化
	override protected void Awake()
	{
		base.Awake();
		this.MemberInit();

		// 表示設定
		this._SetActive(this.IsStartActive);
	}
	#endregion

	#region アクティブ設定
	public static void SetActive(bool isActive)
	{
		if (Instance != null) Instance._SetActive(isActive);
	}
	void _SetActive(bool isActive)
	{
		this.IsActive = isActive;

		// アニメーション開始
		if (this.Attach.rootTween != null)
			this.Attach.rootTween.Play(isActive);
		else
			this.gameObject.SetActive(isActive);
	}
	#endregion

	#region 設定
	public static void SetTimer(float timer, float remainingTime)
	{
		if (Instance != null) Instance._SetTimer(timer, remainingTime);
	}
	void _SetTimer(float timer, float remainingTime)
	{
		this.Timer = timer * 0.1f;
		this.RemainingTime = remainingTime * 0.1f;
		this.SliderUpdate();
		this.LabelUpdate();

		if (0f < this.Timer)
		{
			this._SetActive(true);
		}
	}
	#endregion

	#region 更新
	void Update()
	{
		this.SliderUpdate();
		this.LabelUpdate();

		this.RemainingTime -= Time.deltaTime;
		if (0f >= this.RemainingTime)
		{
			this.RemainingTime = 0f;
			this._SetActive(false);
		}
	}
	void SliderUpdate()
	{
		if (this.Attach.remainingSlider != null)
			this.Attach.remainingSlider.value = this.SliderValue;
	}
	void LabelUpdate()
	{
		if (this.Attach.remainingLabel != null)
			this.Attach.remainingLabel.text = string.Format(this.TimerFormat, this.RemainingTime);
	}
	#endregion

	#region デバッグ
#if UNITY_EDITOR && XW_DEBUG
	[SerializeField]
	DebugParameter _debugParam = new DebugParameter();
	DebugParameter DebugParam { get { return _debugParam; } }
	[System.Serializable]
	public class DebugParameter
	{
		public bool execute;
		public float timer = 10f;
		public float remainingTime = 10f;
	}
	void DebugUpdate()
	{
		var t = this.DebugParam;
		if (t.execute)
		{
			t.execute = false;
			this._SetTimer(t.timer * 10, t.remainingTime * 10);
		}
	}
	void OnValidate()
	{
		this.DebugUpdate();
	}
#endif
	#endregion
}
