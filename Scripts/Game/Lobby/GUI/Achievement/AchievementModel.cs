/// <summary>
/// アチーブメントデータ
/// 
/// 2016/04/25
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;

namespace XUI.Achievement {

	#region ==== 定数 ====

	/// <summary>
	/// タブタイプ
	/// </summary>
	public enum AchievementTabType {
		EmergencyEvent	= 0,
		PriorityEvent	= 1,
		Event_01		= 2,
		Event_02		= 3,
		Event_03		= 4,
		Event_04		= 5,
		DailyWeekly		= 6,
		Achevement		= 7,
		Reserve			= 8,
		Reward			= 9
	}

	#endregion ==== 定数 ====

	/// <summary>
	/// アチーブメントデータインターフェイス
	/// </summary>
	public interface IModel {

		#region ==== プロパティ ====

		/// <summary>
		/// アチーブの全リスト
		/// </summary>
		List<AchievementInfo> InfoList { get; set; }

		/// <summary>
		/// タブ用のリスト
		/// </summary>
		List<AchievementInfo> TabList { get; }

		/// <summary>
		/// 現在のタブタイプ
		/// </summary>
		AchievementTabType TabType { get; set; }

		#endregion ==== プロパティ ====

		#region ==== アクション ====

		/// <summary>
		/// 指定範囲の情報を取得
		/// </summary>
		/// <param name="start"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		List<AchievementInfo> GetTabList( int start, int count );

		#endregion ==== アクション ====
	}

	/// <summary>
	/// アチーブメントデータ
	/// </summary>
	public class Model : IModel {

		#region ==== フィールド ====

		private List<AchievementInfo> infoList = null;
		private List<AchievementInfo> tabList = new List<AchievementInfo>();
		private AchievementTabType tabType = AchievementTabType.Event_01;

		#endregion ==== フィールド ====

		#region ==== プロパティ ====

		/// <summary>
		/// アチーブの全リスト
		/// </summary>
		public List<AchievementInfo> InfoList { get { return infoList; } set { infoList = value; } }

		/// <summary>
		/// タブ用のリスト
		/// </summary>
		public List<AchievementInfo> TabList { get { return tabList; } }

		/// <summary>
		/// 現在のタブタイプ
		/// </summary>
		public AchievementTabType TabType { get { return tabType; } set { tabType = value; } }

		#endregion ==== プロパティ =====

		#region ==== イベント ====

		/// <summary>
		/// タブカウント変更イベント
		/// </summary>
		public event EventHandler<TabCountChangeEventArgs> OnTabCountChange = ( sender, e ) => { };

		#endregion ==== イベント ====

		#region ==== アクション ====

		/// <summary>
		/// 指定範囲の情報を取り出す
		/// </summary>
		/// <param name="start"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public List<AchievementInfo> GetTabList( int start, int count ) {

			int end = start + count;
			List<AchievementInfo> infos = new List<AchievementInfo>();

			infos.AddRange( tabList.Where( x => x.Index >= start && x.Index < end ));

			return infos;
		}

		#endregion ==== アクション ====
	}
}
