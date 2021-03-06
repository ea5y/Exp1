/// <summary>
/// 進化素材リスト表示
/// 
/// 2016/02/08
/// </summary>
using UnityEngine;
using System.Collections;

namespace XUI
{
	namespace CharaList
	{
		/// <summary>
		/// 進化素材リスト表示インターフェイス
		/// </summary>
		public interface IEvolutionMaterialListView
		{
			/// <summary>
			/// 注目メッセージ設定
			/// </summary>
			void SetAttentionActive(bool isActive, string message);
		}

		/// <summary>
		/// 進化素材リスト表示
		/// </summary>
		public class EvolutionMaterialListView : CharaListView, IEvolutionMaterialListView
		{
			#region 注目メッセージ
			[SerializeField]
			private UILabel _attentionLabel = null;
			private UILabel AttentionLabel { get { return _attentionLabel; } }

			/// <summary>
			/// 注目メッセージ設定
			/// </summary>
			public void SetAttentionActive(bool isActive, string message)
			{
				if (this.AttentionLabel == null) { return; }
				this.AttentionLabel.text = message;
				this.AttentionLabel.gameObject.SetActive(isActive);
			}
			#endregion
		}
	}
}