using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace XUI.SynchroDirection
{

	public interface IModel
	{
		#region === Event ===

		event EventHandler OnResultChange;

		event EventHandler OnBaseCharaIdChange;

		event EventHandler OnBaitCharaIdChange;
		
		#endregion === Event ===

		#region === Property ===

		/// <summary>
		/// ベースキャラID
		/// </summary>
		int BaseCharaId { get; set; }

		/// <summary>
		/// エサ
		/// </summary>
		int BaitCharaId { get; set; }


		int HpUp { get; set; }

		bool IsHpMax { get; set; }

		int AtkUp { get; set; }

		bool IsAtkMax { get; set; }

		int DefUp { get; set; }

		bool IsDefMax { get; set; }

		int ExUp { get; set; }

		bool IsExMax { get; set; }

		/// <summary>
		/// 強化結果
		/// </summary>
		Scm.Common.GameParameter.PowerupResult Result { get; set; }

		#endregion === Property ===

		/// <summary>
		/// オブジェクトの破棄
		/// </summary>
		void Dispose();

		void Setup();
	}

	public class Model : IModel
	{
		#region === Field ===

		private Scm.Common.GameParameter.PowerupResult result;

		private int baseCharaId = 0;

		private int baitCharaId = 0;

		private int hpUp = 0;

		private bool isHpMax = false; 

		private int atkUp = 0;

		private bool isAtkMax = false;

		private int defUp = 0;

		private bool isDefMax = false;

		private int exUp = 0;

		private bool isExMax = false;
		
		#endregion === Field ===

		#region === Event ===

		public event EventHandler OnResultChange = (sender, e) => { };

		public event EventHandler OnBaseCharaIdChange = (sender, e) => { };

		public event EventHandler OnBaitCharaIdChange = (sender, e) => { };

		#endregion === Event ===


		#region === Property ===


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

		/// <summary>
		/// エサキャラID
		/// </summary>
		public int BaitCharaId
		{
			get { return baitCharaId; }
			set
			{
				if(baitCharaId != value) {
					baitCharaId = value;

					OnBaitCharaIdChange(this, EventArgs.Empty);
				}
			}
		}

		public int HpUp { get { return hpUp; } set { hpUp = value; } }

		public bool IsHpMax { get { return isHpMax; } set { isHpMax = value; } }

		public int AtkUp { get { return atkUp; } set { atkUp = value; } }

		public bool IsAtkMax { get { return isAtkMax; } set { isAtkMax = value; } }

		public int DefUp { get { return defUp; } set { defUp = value; } }

		public bool IsDefMax { get { return isDefMax; } set { isDefMax = value; } }

		public int ExUp { get { return exUp; } set { exUp = value; } }

		public bool IsExMax { get { return isExMax; } set { isExMax = value; } }

		#endregion === Property ===


		/// <summary>
		/// オブジェクトの破棄
		/// </summary>
		public void Dispose()
		{
			OnResultChange = null;
			OnBaseCharaIdChange = null;
			OnBaitCharaIdChange = null;
		}

		public void Setup()
		{
			baseCharaId = 0;
			baitCharaId = 0;

			hpUp = 0;
			atkUp = 0;
			defUp = 0;
			exUp = 0;
		}
		

	}
}

