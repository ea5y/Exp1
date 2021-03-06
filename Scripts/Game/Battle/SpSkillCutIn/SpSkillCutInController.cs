/// <summary>
/// 必殺技カットイン制御
/// 
/// 2015/12/03
/// </summary>
using UnityEngine;
using System.Collections;

namespace XUI
{
	namespace SpSkillCutIn
	{
		/// <summary>
		/// 必殺技カットイン制御インターフェイス
		/// </summary>
		public interface IController
		{
			/// <summary>
			/// カットイン再生
			/// </summary>
			/// <param name="avatarType"></param>
			/// <param name="skillName"></param>
			void Play(AvatarType avatarType, int skinId, string skillName);

			/// <summary>
			/// 更新
			/// </summary>
			void Update();
		}

		/// <summary>
		/// 必殺技カットイン制御
		/// </summary>
		public class Controller : IController
		{
			#region フィールド&プロパティ
			/// <summary>
			/// 再生時間計測用
			/// </summary>
			private float time;

			/// <summary>
			/// 演出中かどうか
			/// </summary>
			private bool isPlayEffect = false;

			/// <summary>
			/// モデル
			/// </summary>
			private readonly IModel model;

			/// <summary>
			/// ビュー
			/// </summary>
			private readonly IView view;
			#endregion

			#region 初期化
			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="model"></param>
			/// <param name="view"></param>
			public Controller(IModel model, IView view)
			{
				this.model = model;
				this.view = view;
			}
			#endregion

			#region 再生
			/// <summary>
			/// カットインを再生する
			/// </summary>
			/// <param name="avatarType"></param>
			/// <param name="skillName"></param>
			public void Play(AvatarType avatarType, int skinId, string skillName)
			{
				if (this.model == null || this.view == null) { Debug.Log("Null"); return; }

				// スキル名が空文字の時は演出させない
				if (string.IsNullOrEmpty(skillName)) return;

				// タイマーセット
				this.time = this.model.PlayTime;
				this.isPlayEffect = true;

				// カットインを再生
				this.view.PlayCutIn(avatarType, skinId, skillName);

			}
			#endregion

			#region 更新
			public void Update()
			{
				// 演出でない場合は時間処理を行わない
				if (!this.isPlayEffect) return;

				// 演出時間カウント
				this.time -= Time.deltaTime;

				Player player = GameController.GetPlayer();
				if (this.time < 0 ||
					(player != null && player.StatusType == Scm.Common.GameParameter.StatusType.Dead))
				{
					// 終了時間になったもしくはプレイヤーが死亡した場合は演出を終了させる
					Finish();
				}
			}
			#endregion

			#region 演出終了
			/// <summary>
			/// 再生終了処理
			/// </summary>
			private void Finish()
			{
				this.isPlayEffect = false;
				this.time = 0;

				if (this.view != null)
				{
					// ビュー側の演出終了処理を行う
					this.view.Finish();
				}
			}
			#endregion
		}
	}
}