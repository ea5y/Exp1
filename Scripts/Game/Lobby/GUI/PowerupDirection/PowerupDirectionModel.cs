using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace XUI.PowerupDirection
{

	public interface IModel
	{

		#region === Event ===

		event EventHandler OnBaseCharaIdChange;

		event EventHandler OnResultChange;

		event EventHandler OnBaitIdsChange;

		event EventHandler OnExpSliderChange;

		#endregion === Event ===

		#region === Property ===

		/// <summary>
		/// 強化キャラID
		/// </summary>
		int BaseCharaId { get; set; }
		
		/// <summary>
		/// 強化時の開始Exp比率[0-1]
		/// </summary>
		float StartExpRate { get; }

		/// <summary>
		/// レベルアップ回数
		/// </summary>
		int LvUpCount { get; }

		/// <summary>
		/// 強化後の開始Exp比率[0-1]
		/// </summary>
		float EndExpRate { get; }

		/// <summary>
		/// レベルが最大になるか
		/// </summary>
		bool IsLvMax { get; }

		/// <summary>
		/// エサリスト
		/// </summary>
		IList<int> BaitIds { get; }
		
        /// <summary>
        /// Same length as BaitIds, skin ids
        /// </summary>
        IList<int> SkinIds { get; }

		/// <summary>
		/// 強化結果
		/// </summary>
		Scm.Common.GameParameter.PowerupResult Result { get; set; }

		#endregion === Property ===


		/// <summary>
		/// 初期化
		/// </summary>
		void Setup();

		/// <summary>
		/// オブジェクトの破棄
		/// </summary>
		void Dispose();

		/// <summary>
		/// エサをセットする
		/// </summary>
		void SetBaitIds(params int[] ids);

		/// <summary>
		/// 経験値Sliderセット
		/// </summary>
		void SetExpSlider(float startExp, float endExp, int lvUp, bool lvMax);

	}

	public class Model : IModel
	{
		#region === Field ===

		private int baseCharaId = 0;

		private float startExpRate = 0;

		private int lvUpCount = 0;

		private float endExpRate = 0;

		private bool isLvMax = false;

		private Scm.Common.GameParameter.PowerupResult result;

		private List<int> baitIds = new List<int>(10);

        private List<int> skinIds = new List<int>(10);

		#endregion === Field ===

		#region === Event ===

		public event EventHandler OnBaseCharaIdChange = (sender, e) => { };

		public event EventHandler OnResultChange = (sender, e) => { };

		public event EventHandler OnBaitIdsChange = (sender, e) => { };

		public event EventHandler OnExpSliderChange = (sender, e) => { };

		#endregion === Event ===

		#region === Property ===

		/// <summary>
		/// ベースキャラID
		/// </summary>
		public int BaseCharaId
		{
			get { return baseCharaId; }
			set
			{
				if(baseCharaId != value) {
					baseCharaId = value;

					OnBaseCharaIdChange(this, EventArgs.Empty);
				}
			}
		}


		public float StartExpRate { get { return startExpRate; } }

		public int LvUpCount { get { return lvUpCount; } }

		public float EndExpRate { get { return endExpRate; } }

		public bool IsLvMax { get { return isLvMax; } }



		/// <summary>
		/// エサのIDリスト
		/// </summary>
		public IList<int> BaitIds
		{
			get { return baitIds; }
		}

        /// <summary>
        /// Bait id's skins
        /// </summary>
        public IList<int> SkinIds {
            get { return skinIds; }
        }

		public Scm.Common.GameParameter.PowerupResult Result
		{
			get { return result; }
			set
			{
				if(result != value) {
					result = value;

					OnResultChange(this, EventArgs.Empty);
				}
			}
		}


		#endregion === Property ===

		/// <summary>
		/// オブジェクトの破棄
		/// </summary>
		public void Dispose()
		{
			OnBaseCharaIdChange = null;
			OnResultChange = null;
			OnBaitIdsChange = null;
			OnExpSliderChange = null;

			baitIds = null;
		}

		public void Setup()
		{
			baseCharaId = 0;
			startExpRate = 0;
			lvUpCount = 0;
			endExpRate = 0;
			isLvMax = false;
			baitIds.Clear();
		}

		/// <summary>
		/// エサをセットする
		/// </summary>
		/// <param name="ids"></param>
		public void SetBaitIds(params int[] ids)
		{
			baitIds.Clear();
			if(ids != null) {
				baitIds.AddRange(ids);

				OnBaitIdsChange(this, EventArgs.Empty);
			}
		}



		/// <summary>
		/// 経験値Sliderセット
		/// </summary>
		public void SetExpSlider(float startExp, float endExp, int lvUp, bool lvMax)
		{
			if(startExpRate != startExp ||
				endExpRate != endExp ||
				lvUpCount != lvUp ||
				isLvMax != lvMax) {

				startExpRate = startExp;
				endExpRate = endExp;
				lvUpCount = lvUp;
				isLvMax = lvMax;


				OnExpSliderChange(this, EventArgs.Empty);
			}

		}
		
	}
}

