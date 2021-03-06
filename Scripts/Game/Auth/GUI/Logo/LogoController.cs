/// <summary>
/// ロゴ制御
/// 
/// 2015/11/13
/// </summary>
using UnityEngine;
using System;

namespace XUI.Logo
{
	#region 定義
	public enum FadeType
	{
		In,
		Out,
		InOut,
	}
	#endregion

	/// <summary>
	/// ロゴ制御インターフェイス
	/// </summary>
	public interface IController
	{
		#region 終了判定
		/// <summary>
		/// 終了しているかどうか
		/// </summary>
		bool IsFinish { get; }
		#endregion

		#region 初期化
		/// <summary>
		/// 初期化
		/// </summary>
		void Setup();
		#endregion

		#region 破棄
		/// <summary>
		/// 破棄
		/// </summary>
		void Dispose();
		#endregion

		#region アクティブ設定
		/// <summary>
		/// アクティブ設定
		/// </summary>
		void SetActive(bool isActive, bool isTweenSkip);
		#endregion

		#region 更新処理
		/// <summary>
		/// 更新
		/// </summary>
		/// <returns>true=>継続中 false=>終了</returns>
		bool Update();
		#endregion
	}

	/// <summary>
	/// ロゴ制御
	/// </summary>
	public class Controller : IController
	{
		#region フィールド＆プロパティ
		/// <summary>
		/// ステート制御
		/// </summary>
		Statement _state = Statement.Start;
		public Statement State { get { return _state; } private set { _state = value; } }
		public enum Statement
		{
			Start,
			SetFadeIn,
			WaitFadeIn,
			CheckNextFadeOut,
			SetFadeOut,
			WaitFadeOut,
			SetNextFade,
			End,
		}

		/// <summary>
		/// 更新処理が出来る状態かどうか
		/// </summary>
		public bool CanUpdate { get { return this.Models != null && this.View != null; } }

		/// <summary>
		/// 終了しているかどうか
		/// </summary>
		public bool IsFinish { get { return this.State == Statement.End; } }

		// フェード終了時間
		float FinishTime { get; set; }
		// 現在表示しているロゴのインデックス
		int CurrentIndex { get; set; }

		// 現在のデータ
		IModel CurrentData
		{
			get
			{
				IModel model;
				this.Models.TryGetDrawData(this.CurrentIndex, out model);
				return model;
			}
		}
		// 現在のデータが有効かどうか
		bool IsValidCurrentData
		{
			get
			{
				if (!this.CanUpdate) return false;
				// 範囲外チェック
				return this.CurrentIndex >= 0 && this.CurrentIndex < this.Models.Count;
			}
		}

		// モデル
		readonly IModels _models;
		IModels Models { get { return _models; } }
		// ビュー
		readonly IView _view;
		IView View { get { return _view; } }

		// メンバーの初期化
		void MemberInit()
		{
			this.State = Statement.Start;
			this.FinishTime = 0f;
			this.CurrentIndex = 0;
		}
		#endregion

		#region 初期化
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public Controller(IModels models, IView view)
		{
			if (models == null || view == null) return;

			this.MemberInit();

			// ビュー設定
			this._view = view;
			this.View.OnSkip += this.HandleSkip;

			// モデル設定
			this._models = models;
		}
		/// <summary>
		/// 初期化
		/// </summary>
		public void Setup()
		{
			this.MemberInit();

			if (!this.CanUpdate) return;

			// 全ての Tween を初期化する
			this.Models.ForEach(
				(data) =>
				{
					var tween = data.Tween;
					if (tween != null)
					{
						tween.Sample(0f, false);
					}
				});

			this.View.ClearTween();
		}
		#endregion

		#region 破棄
		/// <summary>
		/// 破棄
		/// </summary>
		public void Dispose()
		{
			if (this.CanUpdate)
			{
				this.Models.Dispose();
			}
		}
		#endregion

		#region アクティブ設定
		/// <summary>
		/// アクティブ設定
		/// </summary>
		public void SetActive(bool isActive, bool isTweenSkip)
		{
			if (this.CanUpdate)
			{
				this.View.SetActive(isActive, isTweenSkip);
			}
		}
		#endregion

		#region スキップ
		/// <summary>
		/// スキップハンドル
		/// </summary>
		void HandleSkip(object sender, EventArgs e)
		{
			if (!this.CanUpdate) return;

			var data = this.CurrentData;
			if (data == null) return;

			// スキップ可能かどうか
			if (!data.CanSkip) return;

			// スキップ可能なのでステートを進める
			switch (this.State)
			{
			case Statement.WaitFadeIn:
			case Statement.WaitFadeOut:
				this.State = (Statement)(this.State + 1);
				break;
			}
		}
		#endregion

		#region 更新処理
		/// <summary>
		/// 更新
		/// </summary>
		/// <returns>true=>継続中 false=>終了</returns>
		public bool Update()
		{
			if (this.CanUpdate)
			{
				return this.UpdateState();
			}

			return false;
		}
		bool UpdateState()
		{
			switch (this.State)
			{
				case Statement.Start: return this.State_Start();
				case Statement.End: return this.State_End();
				case Statement.SetFadeIn: return this.State_SetFadeIn();
				case Statement.CheckNextFadeOut: return this.State_CheckNextFadeOut();
				case Statement.SetFadeOut: return this.State_SetFadeOut();
				case Statement.SetNextFade: return this.State_SetNextFade();
				case Statement.WaitFadeIn:
				case Statement.WaitFadeOut:
					return this.State_WaitTime();
			}

			// ステート変更
			this.State = Statement.End;
			return true;	// 処理継続中...
		}
		bool State_WaitTime()
		{
			if (this.FinishTime >= Time.time)
				return true;	// 処理継続中...

			// ステート変更
			this.State = (Statement)(this.State + 1);
			return true;
		}

		bool State_Start()
		{
			// ステート変更
			this.SetNextFadeStatement();
			return true;	// 処理継続中...
		}
		bool State_End()
		{
			return false;	// 処理終了
		}
		bool State_SetFadeIn()
		{
			// フェード時間設定
			var model = this.CurrentData;
			var fadeTime = model.FadeTime;
			if (model.FadeType == FadeType.InOut) fadeTime *= 0.5f;
			this.FinishTime = Time.time + fadeTime;

			// ビュー側のフェードイン
			this.View.FadeIn(model.Tween);

			// ステート変更
			this.State = (Statement)(this.State + 1);
			return true;	// 処理継続中...
		}
		bool State_CheckNextFadeOut()
		{
			var model = this.CurrentData;
			if (model.FadeType == FadeType.In)
			{
				// ステート変更
				this.State = Statement.SetNextFade;
			}
			else
			{
				// ステート変更
				this.State = Statement.SetFadeOut;
			}
			return true;	// 処理継続中...
		}
		bool State_SetFadeOut()
		{
			// フェード時間設定
			var model = this.CurrentData;
			var fadeTime = model.FadeTime;
			if (model.FadeType == FadeType.InOut) fadeTime *= 0.5f;
			this.FinishTime = Time.time + fadeTime;

			// ビュー側のフェードアウト
			this.View.FadeOut(model.Tween);

			// ステート変更
			this.State = (Statement)(this.State + 1);
			return true;	// 処理継続中...
		}
		bool State_SetNextFade()
		{
			var model = this.CurrentData;
			if (model.IsStay)
			{
				this.View.ClearTween();
			}

			this.CurrentIndex++;

			// ステート変更
			this.SetNextFadeStatement();
			return true;	// 処理継続中...
		}
		void SetNextFadeStatement()
		{
			if (this.IsValidCurrentData)
			{
				var model = this.CurrentData;

				if (model.FadeType != FadeType.Out)
				{
					this.State = Statement.SetFadeIn;
				}
				else
				{
					this.State = Statement.SetFadeOut;
				}
			}
			else
			{
				this.State = Statement.End;
			}
		}
		#endregion
	}
}
