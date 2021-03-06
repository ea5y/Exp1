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

namespace PlayerState
{
	public class Wake : PlayerState
	{
		public override Character.StateProc StateProc { get { return Character.StateProc.Wake; } }

		private IEnumerator updateFiber;

		private Player.PlayerStateAdapter playerAdapter;
		private MotionState motion;

		public Wake(Player.PlayerStateAdapter playerAdapter, IEnumerator wakeMotionFiber)
		{
			this.playerAdapter = playerAdapter;
			this.updateFiber = WakeCoroutine(wakeMotionFiber);
		}

		public override bool Update()
		{
			return updateFiber.MoveNext();
		}

		IEnumerator WakeCoroutine(IEnumerator wakeMotionFiber)
		{
			// モーション
			while(wakeMotionFiber != null && wakeMotionFiber.MoveNext())
			{
				this.playerAdapter.SetAbsoluteGuardCounter(GameConstant.WakeInvincibleTimer);
				yield return null;
			}
			this.playerAdapter.SetAbsoluteGuardCounter(GameConstant.WakeInvincibleTimer);
		}

		public override bool IsSkillUsable() { return false; }	// ×スキル使用不可.
		public override bool CanFalter() { return false; }		// ×怯みナシ.
		public override bool CanBind() { return false; }		// ×マヒナシ(起き上がり後,効果時間内なら改めてマヒる).
		public override bool CanJump() { return false; }		// ×ジャンプしない.
		public override bool CanWire() { return false; }		// ×ワイヤー移動しない.
	}
}