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
	public class UserJump : PlayerState
	{
		public override Character.StateProc StateProc { get { return Character.StateProc.UserJump; } }

		private IEnumerator updateFiber;

		private Player.PlayerStateAdapter playerAdapter;
		private Vector3 vec;

		public UserJump(Player.PlayerStateAdapter playerAdapter, Vector3 vec)
		{
			this.playerAdapter = playerAdapter;
			this.vec = vec;
			this.updateFiber = UserJumpProcCoroutine();
		}

		public override bool Update()
		{
			return updateFiber.MoveNext();
		}
		public override void Finish()
		{
			this.playerAdapter.ResetAnimation();
		}

		IEnumerator UserJumpProcCoroutine()
		{
			const float GravityMag = 10f;				// 重力倍率.カッコよく見せるための嘘数値.
			
			//float gravity = this.CharacterMove.GravityBaseY * GravityMag;
			Vector3 velocity = vec;						// 秒間移動量.

			this.playerAdapter.ForceSendMovePacket();

			// エフェクト.
			EffectManager.Create(GameConstant.EffectJumpBase, this.playerAdapter.Player.transform.position, this.playerAdapter.Player.transform.rotation);
			EffectManager.Create(GameConstant.EffectJumpDirect, this.playerAdapter.Player.transform.position, Quaternion.LookRotation(velocity));

			this.playerAdapter.Player.CharacterMove.DirectionReset();
			this.playerAdapter.Player.CharacterMove.IsGrounded = false;

			// 上昇中.
			IEnumerator jumpProcAnimFiber = this.playerAdapter.JumpProcAnimUp();
			Vector3 outMove;
			do
			{
				this.playerAdapter.CalculateMove(velocity, out outMove);
				this.playerAdapter.Player.MovePosition(outMove, true);
				this.playerAdapter.Player.CharacterMove.GravityMag = GravityMag;
				this.playerAdapter.Player.CharacterMove.UseInertia = false;
				jumpProcAnimFiber.MoveNext();
				
				yield return null;
				
				velocity = this.playerAdapter.Player.CharacterMove.Velocity;
			}
			while(0 < outMove.y);

			// 下降中.
			jumpProcAnimFiber = this.playerAdapter.JumpProcAnimDown();

			this.playerAdapter.SetFallProcCoroutine(jumpProcAnimFiber);
			yield return null;
		}


		public override bool IsSkillUsable() { return true; }	// ○スキル使用可.ただし空中使用可のスキルのみ.
		public override bool CanFalter() { return true; }		// ○怯みアリ.
		public override bool CanBind() { return true; }			// ○マヒアリ(ダウン落下に移行).
		public override bool CanJump() { return true; }			// ○ジャンプアリ(連続ジャンプもありうる).
		public override bool CanWire() { return true; }			// ○ワイヤー移動アリ.
	}
}