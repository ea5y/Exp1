/// <summary>
/// 
/// 
/// 
/// </summary>
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using Scm.Common.Master;
using Scm.Common.Packet;
using Scm.Common.GameParameter;

namespace PersonState
{
	public class Move : PersonState
	{
		public override Character.StateProc StateProc { get { return Character.StateProc.Move; } }

		private Person.PersonStateAdapter pSAdapter;

		public Move(Person.PersonStateAdapter personStateAdapter)
		{
			this.pSAdapter = personStateAdapter;

			// Init
			this.pSAdapter.ResetAnimation();
		}

		public override bool Update()
		{
			this.pSAdapter.MoveProc();
			return true;
		}

        public override bool IsSkillUsable() { return true; }   // ○スキル使用可.
        public override bool CanFalter() { return true; }       // ○怯みアリ.
        public override bool CanBind() { return true; }         // ○マヒアリ.
        public override bool CanJump() { return true; }         // ○ジャンプアリ.
        public override bool CanWire() { return true; }         // ○ワイヤー移動アリ.
    }
}