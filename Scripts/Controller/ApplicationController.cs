/// <summary>
/// アプリケーションコントローラー
/// 
/// 2012/12/10
/// </summary>
using UnityEngine;
using System.Collections;
using Scm.Common.GameParameter;

[System.Serializable]
public class ScreenInfo
{
	/// <summary>
	/// UIのサイズ補正を掛ける基準のインチ数(このサイズを超えるとスケールを掛ける)
	/// </summary>
	[SerializeField]
	float _scaleDeviceInch = 6f;
	public float ScaleDeviceInch { get { return _scaleDeviceInch; } }

	/// <summary>
	/// アスペクト比の基準値
	/// </summary>
	[SerializeField]
	Vector2 _scaleAspectRateDefault = new Vector2(16f, 9f);
	public Vector2 ScaleAspectRateDefault { get { return _scaleAspectRateDefault; } }

	/// <summary>
	/// アスペクト比率の最小値
	/// </summary>
	[SerializeField]
	Vector2 _scaleAspectRateMin = new Vector2(4f, 3f);
	public Vector2 ScaleAspectRateMin { get { return _scaleAspectRateMin; } }

	/// <summary>
	/// 最小スケール
	/// </summary>
	[SerializeField]
	float _scaleRateMin = 0.8f;
	public float ScaleRateMin { get { return _scaleRateMin; } }

	/// <summary>
	/// スケールを掛けるデバイスかどうか
	/// </summary>
	public bool IsScaleDevice { get { return this.ScaleDeviceInch <= this.Inch; } }

	/// <summary>
	/// スクリーンの幅
	/// </summary>
	int _width = -1;
	public int Width
	{
		get
		{
			if (_width < 0) _width = Screen.width;
			return _width;
		}
	}

	/// <summary>
	/// スクリーンの高さ
	/// </summary>
	int _height = -1;
	public int Height
	{
		get
		{
			if (_height < 0) _height = Screen.height;
			return _height;
		}
	}

	/// <summary>
	/// スクリーンのインチ数
	/// </summary>
	float _inch = -1f;
	public float Inch
	{
		get
		{
			if (_inch < 0f)
			{
				if (Screen.dpi != 0f)
				{
					float t = Mathf.Sqrt(this.Width * this.Width + this.Height * this.Height);
					_inch = t / Screen.dpi;
				}
			}
			return _inch;
		}
	}

	/// <summary>
	/// スクリーンのアスペクト比率
	/// </summary>
	float _aspectRate = -1f;
	public float AspectRate
	{
		get
		{
			if (_aspectRate < 0f && Height != 0) _aspectRate = (float)this.Width / (float)this.Height;
			return _aspectRate;
		}
	}

	/// <summary>
	/// スケール比率
	/// </summary>
	float _scaleRate = -1f;
	public float ScaleRate
	{
		get
		{
			if (_scaleRate < 0f)
			{
				_scaleRate = 1f;
				if (this.IsScaleDevice)
				{
					var def = this.ScaleAspectRateDefault.x / this.ScaleAspectRateDefault.y;
					var min = this.ScaleAspectRateMin.x / this.ScaleAspectRateMin.y;
					var t1 = def - min;
					var t2 = this.AspectRate - min;
					var t3 = t1 != 0f ? t2 / t1 : 1.0f;
					t3 = Mathf.Clamp01(t3);
					_scaleRate = Mathf.Lerp(this.ScaleRateMin, 1.0f, t3);
				}
			}
			return _scaleRate;
		}
	}

	// スクリーンのアスペクト比
	float _widthAspectRatio = -1f;
	float _heightAspectRatio = -1f;
	/// <summary>
	/// スクリーンのアスペクト比
	/// </summary>
	public void GetAspectRatio(out float w, out float h)
	{
		if (_widthAspectRatio < 0f || _heightAspectRatio < 0f)
		{
			float t = (float)gcd(this.Width, this.Height);
			_widthAspectRatio = (float)this.Width / t;
			_heightAspectRatio = (float)this.Height / t;
		}
		w = _widthAspectRatio;
		h = _heightAspectRatio;
	}

	/// <summary>
	/// 最大公約数を求める
	/// </summary>
	float gcd(float x, float y)
	{
		return y == 0 ? x : gcd(y, x % y);
	}

#if UNITY_EDITOR
	/// <summary>
	/// 更新
	/// </summary>
	public void Update()
	{
		// スクリーンサイズが変更されたかチェックする
		if (_width != Screen.width || _height != Screen.height)
		{
			_width = -1;
			_height = -1;
			_inch = -1f;
			_aspectRate = -1f;
			_scaleRate = -1f;
			_widthAspectRatio = -1f;
			_heightAspectRatio = -1f;
		}
	}
#endif
}

public class ApplicationController : Singleton<ApplicationController>
{
	#region フィールド＆プロパティ
	[SerializeField]
	private int frameRate = -1;

	// スクリーン情報
	[SerializeField]
	ScreenInfo _screenInfo = new ScreenInfo();
	static public ScreenInfo ScreenInfo { get { return Instance != null ? Instance._screenInfo : null; } }

	// アプリのPause状態.
	static bool isPause = false;
	static public bool IsPause { get { return isPause; } } // HACK 別スレッドから参照した際にInstanceの比較を行うとエラーになるので両方static.

	/// <summary>
	/// 言語設定をGameParameterで定義されたタイプで取得する
	/// </summary>
	static public Language Language
	{
		get
		{
#if BETA_SERVER
            return Language.ChineseSimplified;
#else
            if (Instance == null) { return Language.Japanese; }
#if XW_DEBUG
			return ScmParam.Debug.File.Language;
#else
			switch (Application.systemLanguage)
			{
				case SystemLanguage.Japanese:
					return Language.Japanese;
				case SystemLanguage.ChineseSimplified:
                case SystemLanguage.Chinese:
					return Language.ChineseSimplified;
				case SystemLanguage.ChineseTraditional:
					return Language.ChineseTraditional;
				case SystemLanguage.English:
				default:
					return Language.English;
			}
#endif
#endif
        }
	}

	/// <summary>
	/// 現在のプラットフォームタイプ
	/// 主にKPI用に使う
	/// </summary>
	static public Scm.Common.GameParameter.PlatformType PlatformType
	{
		get
		{
#if UNITY_EDITOR || UNITY_STANDALONE
			return Scm.Common.GameParameter.PlatformType.Android;
#elif UNITY_ANDROID
			return Scm.Common.GameParameter.PlatformType.Android;
#elif UNITY_IOS
			return Scm.Common.GameParameter.PlatformType.Ios;
#endif
		}
	}
#endregion

#region Unityリフレクション
	void Start()
	{
		// フレームレートを固定する
		Application.targetFrameRate = this.frameRate;

		// アプリをバックグラウンドでも動作させる
		Application.runInBackground = true;

		// スリープさせない
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
		//TouchScreenKeyboard.hideInput = true;

		// 開始時にボイスチャット開始.
		//VoiceChatController.Start();
	}

	void Update()
	{
#if UNITY_EDITOR
		Application.targetFrameRate = this.frameRate;
		this.KeyUpdate();
		if (ScreenInfo != null) ScreenInfo.Update();
#endif
		// TODO:戻るボタンでアプリ終了は無効化、場面に合わせて処理をする？
		//if (Application.platform == RuntimePlatform.Android && Input.GetKey(KeyCode.Escape))
		//{
		//	Application.Quit();
		//}
	}
#if UNITY_EDITOR
	[SerializeField]
	public Vector3 jumpPos;
	void KeyUpdate()
	{
		if (GUIChat.IsInput)
			return;
		if (Input.GetKey(KeyCode.Space))
		{
			var p = GameController.GetPlayer();
			if (p != null && p.State == Character.StateProc.Move)
			{
				//p.Jump(p.Position + p.transform.forward * 10, p.transform.rotation);
				p.Jump(jumpPos, p.transform.rotation);
			}
		}
	}
#endif
	void OnDestroy()
	{
		// ボイスチャット終了.
		//VoiceChatController.Stop();
	}
	/// <summary>
	/// Homeボタンなどでアプリが裏に回ったor表に復帰した際の通知.
	/// </summary>
	void OnApplicationPause(bool pauseStatus)
	{
		isPause = pauseStatus;
	}
	/// <summary>
	/// アプリケーション終了前に、すべてのゲーム オブジェクトで呼び出されます
	/// エディタでは、ユーザーが再生モードを停止すると呼び出されます
	/// ウェブ プレイヤーでは、ウェブ ビューが閉じられると呼び出されます。
	/// </summary>
	void OnApplicationQuit()
	{
		// コンフィグファイル書き込み
		ConfigFile.Instance.Write();

        Scm.Client.GameListener.DisconnectQuit();

    }
#endregion

#region Method
#endregion
}