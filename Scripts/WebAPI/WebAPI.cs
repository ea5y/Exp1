/// <summary>
/// WebAPI処理
/// 
/// 2014/04/16
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WebAPI
{
	#region フィールド＆プロパティ
	// WebAPIホスト
	enum HostType
	{
		Jp,		// 日本サーバー
	}
#if XW_DEBUG
	static readonly List<string> HostList = new List<string>
	{
		"http://test.xworld.jp"
	};
#else
	static readonly List<string> HostList = new List<string>
	{
		"http://xworld.jp",
	};
#endif
	static string HttpHost
	{
		get
		{
			return HostList[(int)HostType.Jp];
		}
	}


	public static string PlatformCode
	{
		get
		{
#if UNITY_ANDROID
			return "android";
#elif UNITY_IOS
			return "iphone";
#else
			return "android";
#endif
		}
	}

	public static string DistributionCode
	{
		get
		{
            if (AuthEntry.Instance.AuthMethod != null) {
                return Scm.Common.GameParameter.DistributionCode.GetCode(AuthEntry.Instance.AuthMethod.distributionCode);
            } else {
                return "";
            }
        }
	}




	// リクエストURI
	public enum AccessType
	{
		GetReviewInfomation = 1,	// ①審査情報取得
		CheckGameMaintenance = 2,	// ②ゲームメンテナンス状態判定
		CheckOpenBrowser = 3,		// ③初回ブラウザ起動可否判定
		CheckDisplayTitle = 4,		// ④ゲームタイトル表示可否判定
		OpenBrowser = 5,			// ⑤初回ブラウザ起動(③のレスポンスis_open_browser=trueの時のみリクエストして処理する)
		RegisrerFree = 6,			// ⑥強制無料コース登録
		RegisterReviewUser = 7,		// ⑦審査チェックユーザ登録
		Max,
		Begin = GetReviewInfomation,
	};
	static readonly List<string> URIList = new List<string>
	{
		"",
		"/ignition/reviewinformation/",
		"/ignition/gamemaintenance/",
		"/ignition/openbrowser/",
		"/ignition/displaytitle/",
		"/register/",
		"/register/free/",
		"/ignition/registerreviewuser/",
	};
	#endregion

	#region 定義
	/// <summary>
	/// Scm.Common.Web.HttpWebClientScm のリクエストパラメータ
	/// </summary>
	public class RequestParam
	{
		public string address;
		public string post;
		public Dictionary<string, string> postParam;

		public RequestParam() {}
		public void SetParam(AccessType accessType, string asobimoID, string asobimoToken, string platform)
		{
			this.Setup(accessType, asobimoID, asobimoToken, platform, "", "");
		}
		public void SetParamOpenBrowser(string openBrowserParameter)
		{
			this.Setup(AccessType.OpenBrowser, "", "", "", openBrowserParameter, "");
		}
		public void SetParamRegisterReviewUser(string asobimoID, string asobimoToken, string platform, string reviewVersion)
		{
			this.Setup(AccessType.RegisterReviewUser, asobimoID, asobimoToken, platform, "", reviewVersion);
		}
		void Setup(AccessType accessType, string asobimoID, string asobimoToken, string platform, string openBrowserParameter, string reviewVersion)
		{
			// URL指定
			this.address = HttpHost + URIList[(int)accessType];
	
			// ポストラパメータ設定
			this.postParam = new Dictionary<string, string>();
			switch (accessType)
			{
			case AccessType.OpenBrowser:
				this.postParam.Add("op", openBrowserParameter);
				break;
			case AccessType.RegisterReviewUser:
				this.postParam.Add("asobimo_id", asobimoID);
				this.postParam.Add("asobimo_token", asobimoToken);
				this.postParam.Add("platform_code", PlatformCode);
				this.postParam.Add("distribution_code", DistributionCode);
				this.postParam.Add("review_version", reviewVersion);
				break;
			case AccessType.GetReviewInfomation:
			case AccessType.CheckGameMaintenance:
			case AccessType.CheckOpenBrowser:
			case AccessType.CheckDisplayTitle:
			default:
				this.postParam.Add("asobimo_id", asobimoID);
				this.postParam.Add("asobimo_token", asobimoToken);
				this.postParam.Add("platform_code", PlatformCode);
				this.postParam.Add("distribution_code", DistributionCode);
				break;
			}
	
			// ポストパラメータを文字列に変換する
			this.post = "";
			foreach (var pair in this.postParam)
				this.post += string.Format("{0}={1}&", pair.Key, pair.Value);
		}
	}
	/// <summary>
	/// Scm.Common.Web.HttpWebClientScm のレスポンスパラメータ
	/// </summary>
	public class ResponseParam
	{
		public string response = string.Empty;
		public JSONObject json;
		public System.Net.HttpStatusCode statusCode;

		public ResponseParam() {}
		public ResponseParam(Scm.Common.Web.ScmTask<Scm.Common.Web.HttpWebResponseScm> task) { this.Setup(task); }
		public void Setup(Scm.Common.Web.ScmTask<Scm.Common.Web.HttpWebResponseScm> task)
		{
			if (task.Status == Scm.Common.Web.ScmTaskStatus.RanToCompletion)
			{
				// 成功
				Scm.Common.Web.HttpWebResponseScm response = task.Result;
				// パラメータ格納
				this.response = response.GetString();
				this.json = new JSONObject(this.response);
				this.statusCode = response.GetStatusCode();
			}
			else if (task.Status == Scm.Common.Web.ScmTaskStatus.Faulted)
			{
				// 失敗
				var e = task.Exception as System.Net.WebException;
				if (e != null)
				{
					var response = e.Response as System.Net.HttpWebResponse;
					this.json = new JSONObject();
					if (response != null)
						this.statusCode = response.StatusCode;
				}
				else
				{
					this.json = new JSONObject();
				}
			}
			else
			{
				// タスクが完了していない
			}
		}
	}
	/// <summary>
	/// 審査情報取得レスポンスデータ
	/// </summary>
	[System.Serializable]
	public class ReviewInfomationResponse
	{
		public bool isUnderReview;		// 審査状態 true: 審査中, false: 審査中ではない
		public string reviewVersion;	// 審査バージョン 審査中のバージョン(運営管理画面で設定されていない場合、審査中ではない場合は空文字)
		public ReviewInfomationResponse(JSONObject json)
		{
			if (json == null)
				return;
			json.GetField(ref isUnderReview, "is_under_review");
			json.GetField(ref reviewVersion, "review_version");
		}
	}
	/// <summary>
	/// ゲームメンテナンス状態判定レスポンスデータ
	/// </summary>
	[System.Serializable]
	public class GameMaintenanceResponse
	{
		public bool isGameMaintenance;	// メンテナンス状態 true: メンテナンス中, false: メンテナンス中ではない
		public GameMaintenanceResponse(JSONObject json)
		{
			if (json == null)
				return;
			json.GetField(ref isGameMaintenance, "is_game_maintenance");
		}
	}
	/// <summary>
	/// 初回ブラウザ起動可否判定レスポンスデータ
	/// </summary>
	[System.Serializable]
	public class OpenBrowserResponse
	{
		public bool isOpenBrowser;			// 初回ブラウザ起動可否 true: 初回ブラウザ起動の必要あり, false: 初回ブラウザ起動の必要なし
		public string openBrowserParameter;	// 初回ブラウザ起動パラメータ 初回ブラウザ起動時に指定するパラメータ(初回ブラウザ起動可否がfalseの場合は空文字列が返る)
		public OpenBrowserResponse(JSONObject json)
		{
			if (json == null)
				return;
			json.GetField(ref isOpenBrowser, "is_open_browser");
			json.GetField(ref openBrowserParameter, "open_browser_parameter");
		}
	}
	/// <summary>
	/// ゲームタイトル表示可否判定レスポンスデータ
	/// </summary>
	[System.Serializable]
	public class DisplayTitleResponse
	{
		public bool isDisplayTitle;	// ゲームタイトル表示可否 true: ゲームタイトル表示可能, false: ゲームタイトル表示不可
		public DisplayTitleResponse(JSONObject json)
		{
			if (json == null)
				return;
			json.GetField(ref isDisplayTitle, "is_display_title");
		}
	}
	/// <summary>
	/// 審査チェックユーザ登録レスポンスデータ
	/// </summary>
	[System.Serializable]
	public class RegisterReviewUserResponse
	{
		public bool result;	// 実行結果 true: 成功, false: 失敗(審査中以外のアクセス、審査バージョンが異なっている場合は失敗を返す)
		public RegisterReviewUserResponse(JSONObject json)
		{
			if (json == null)
				return;
			json.GetField(ref result, "result");
		}
	}
	#endregion
}
