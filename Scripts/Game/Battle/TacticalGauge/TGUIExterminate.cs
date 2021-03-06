/// <summary>
/// 戦略ゲージ
/// 殲滅戦
/// 
/// 2014/07/18
/// </summary>
using UnityEngine;
using System.Collections;

using Scm.Common.GameParameter;

namespace TacticalGauge
{
	namespace TGUIExterminate
	{
		/// <summary>
		/// マネージャークラス
		/// </summary>
		[System.Serializable]
		public class Manager
		{
			#region フィールド＆プロパティ
			/// <summary>
			/// スコアフォーマット
			/// </summary>
			[SerializeField]
			string _scoreFormat = "{0}/{1}";
			string ScoreFormat { get { return _scoreFormat; } }

			/// <summary>
			/// アタッチオブジェクト
			/// </summary>
			[SerializeField]
			AttachObject _attach;
			public AttachObject Attach { get { return _attach; } }
			[System.Serializable]
			public class AttachObject
			{
				public GameObject root;
			}

			/// <summary>
			/// 味方情報
			/// </summary>
			[SerializeField]
			TeamInfo _myteam;
			TeamInfo Myteam { get { return _myteam; } }

			/// <summary>
			/// 敵情報
			/// </summary>
			[SerializeField]
			TeamInfo _enemy;
			TeamInfo Enemy { get { return _enemy; } }
			#endregion

			#region 初期化
			public void Clear()
			{
				this.Myteam.SetRemainingPoint(0, 0, this.ScoreFormat);
				this.Enemy.SetRemainingPoint(0, 0, this.ScoreFormat);
			}
			#endregion

			#region 殲滅戦残りポイント
			public void SetRemainingPoint(bool isMyteam, int remain, int total)
			{
				if (isMyteam)
					this.Myteam.SetRemainingPoint(remain, total, this.ScoreFormat);
				else
					this.Enemy.SetRemainingPoint(remain, total, this.ScoreFormat);
			}
			#endregion
		}

		/// <summary>
		/// チーム情報クラス
		/// </summary>
		[System.Serializable]
		public class TeamInfo
		{
			#region フィールド＆プロパティ
			/// <summary>
			/// アタッチオブジェクト
			/// </summary>
			[SerializeField]
			AttachObject _attach;
			AttachObject Attach { get { return _attach; } }
			[System.Serializable]
			public class AttachObject
			{
				public UISprite gaugeSprite;
				public UILabel scoreLabel;
			}

			// フィル値
			public float FillAmout { get { return (Total <= 0 ? 0f : (float)Remain / (float)Total); } }

			// 残り時間
			int Remain { get; set; }
			// 合計時間
			int Total { get; set; }

			// メンバー初期化
			void MemberInit()
			{
				this.Remain = 0;
				this.Total = 0;
			}
			#endregion

			#region 初期化
			public TeamInfo() { this.MemberInit(); }
			#endregion

			#region 殲滅戦残りポイント
			public void SetRemainingPoint(int remain, int total, string format)
			{
				this.Remain = remain;
				this.Total = total;

				// UIに反映する
				if(this.Attach.gaugeSprite != null)
				{
					this.Attach.gaugeSprite.fillAmount = this.FillAmout;
				}
				if(this.Attach.scoreLabel != null)
				{
					this.Attach.scoreLabel.text = string.Format(format, remain, total);
				}
			}
			#endregion
		}
	}
}