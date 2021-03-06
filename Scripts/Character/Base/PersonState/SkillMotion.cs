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
	public class SkillMotion : PersonState
	{
		public override Character.StateProc StateProc { get { return Character.StateProc.SkillMotion; } }
		
		private Person.PersonStateAdapter pSAdapter;
		
		public SkillMotion(Person.PersonStateAdapter personStateAdapter)
		{
			this.pSAdapter = personStateAdapter;
		}
		
		public override bool Update()
		{
			this.pSAdapter.SkillMotionProc();
			return true;
		}
		public override void Finish()
		{
			this.pSAdapter.SkillMotionFinish();
		}

        public override bool IsSkillUsable() { return true; }	// ○スキル使用可(キャンセルorリンク).
        public override bool CanFalter() { return true; }		// ○怯みアリ(スーパーアーマー除く).
        public override bool CanBind() { return true; }			// ○マヒアリ.
        public override bool CanJump() { return true; }			// ○ジャンプアリ.
        public override bool CanWire() { return true; }         // ○ワイヤー移動アリ.
    }
}