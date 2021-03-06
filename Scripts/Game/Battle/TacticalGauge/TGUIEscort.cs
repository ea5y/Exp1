/// <summary>
/// 戦略ゲージ
/// Escort
/// 
/// 2014/07/22
/// </summary>
using UnityEngine;
using System.Collections;

namespace TacticalGauge
{
	namespace TGUIEscort
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
			public void Clear()
			{
				this.MemberInit();
				this.SetRemainingPoint(0, 0);
			}
			#endregion

			#region 残りポイント
			public void SetRemainingPoint(int remain, int total)
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
					this.Attach.scoreLabel.text = string.Format(this.ScoreFormat, remain, total);
				}
			}
			#endregion

            //hostage state
            [SerializeField]
            UISprite _hostageStateSlider;
            public UISprite HostageStateSlider { get { return _hostageStateSlider; } }
            [SerializeField]
            UISprite _hostageSprite;
            public UISprite HostageSprite { get { return _hostageSprite; } }
            [SerializeField]
            UISprite _waypoint;
            public UISprite Waypoint { get { return _waypoint; } }

            //hostage guide
            public GameObject guideHostage;
            public GameObject guideDestination;
            public GameObject hostageMarker;
            public GameObject destinationMarker;
            public GameObject timeTips;
		}
	}
}