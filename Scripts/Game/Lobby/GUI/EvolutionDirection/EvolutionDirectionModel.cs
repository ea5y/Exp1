using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace XUI.EvolutionDirection
{

	public interface IModel
	{
		#region === Event ===

		event EventHandler OnBaseCharaIdChange;

		event EventHandler OnMaterialIdsChange;

		event EventHandler OnRankChange;

		event EventHandler OnSynchroRemainUpCountChange;

		#endregion === Event ===

		#region === Property ===

		/// <summary>
		/// ベースキャラID
		/// </summary>
		int BaseCharaId { get; set; }

		int OldRank { get; }

		int NewRank { get; }

		int SynchroRemainUpCount { get; set; }

		IList<int> MaterialIds { get; }

        IList<int> SkinIds { get; }

		#endregion === Property ===

		/// <summary>
		/// 素材をセットする
		/// </summary>
		void SetMaterialIds(int[] ids, int[] skinIds);

		/// <summary>
		/// ランクをセットする
		/// </summary>
		void SetRank(int oldRank, int newRank);


		/// <summary>
		/// オブジェクトの破棄
		/// </summary>
		void Dispose();

		void Setup();
	}

	public class Model : IModel
	{
		#region === Field ===
		
		private int baseCharaId = 0;

		private int oldRank = 0;

		private int newRank = 0;

		private int synchroRemainUpCount = 0;
		
		private List<int> materialIds = new List<int>(5);

        private List<int> skinIds = new List<int>(5);

		#endregion === Field ===

		#region === Event ===

		public event EventHandler OnBaseCharaIdChange = (sender, e) => { };

		public event EventHandler OnMaterialIdsChange = (sender, e) => { };

		public event EventHandler OnRankChange = (sender, e) => { };

		public event EventHandler OnSynchroRemainUpCountChange = (sender, e) => { };

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
		public int OldRank { get { return oldRank; } }

		public int NewRank { get { return newRank; } }

		public int SynchroRemainUpCount
		{
			get { return synchroRemainUpCount; }
			set
			{
				if(synchroRemainUpCount != value) {
					synchroRemainUpCount = value;

					OnSynchroRemainUpCountChange(this, EventArgs.Empty);
				}
			}
		}
		
		public IList<int> MaterialIds { get { return materialIds; } }

        public IList<int> SkinIds { get { return skinIds; } }

		#endregion === Property ===


		/// <summary>
		/// オブジェクトの破棄
		/// </summary>
		public void Dispose()
		{
			materialIds = null;
			OnBaseCharaIdChange = null;
			OnMaterialIdsChange = null;
			OnRankChange = null;
			OnSynchroRemainUpCountChange = null;
		}

		public void Setup()
		{
			baseCharaId = 0;
			oldRank = 0;
			newRank = 0;
			synchroRemainUpCount = 0;
			materialIds.Clear();
		}

		/// <summary>
		/// 素材をセットする
		/// </summary>
		public void SetMaterialIds(int[] ids, int[] skins)
		{
			materialIds.Clear();
            skinIds.Clear();
			if(ids != null) {
				materialIds.AddRange(ids);
			}
            if (skins != null) {
                skinIds.AddRange(skins);
            }
            if (ids != null || skins != null) {
                OnMaterialIdsChange(this, EventArgs.Empty);
            }
        }

		/// <summary>
		/// ランクをセットする
		/// </summary>
		public void SetRank(int oldRank, int newRank)
		{
			if(this.oldRank != oldRank || this.newRank != newRank) {

				this.oldRank = oldRank;
				this.newRank = newRank;

				OnRankChange(this, EventArgs.Empty);
			}
		}

	}
}

