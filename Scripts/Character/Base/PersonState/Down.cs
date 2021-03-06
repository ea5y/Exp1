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

namespace PersonState {

	public class Down : PersonState {

		public override Character.StateProc StateProc { get { return Character.StateProc.Down; } }

		protected IEnumerator updateFiber;

		protected Person.PersonStateAdapter personAdapter;

		public Down(Person.PersonStateAdapter personAdapter_, Vector3 blownVec, float bulletDirection, MotionState staMotion, MotionState midMotion, MotionState endMotion)
		{
			this.personAdapter = personAdapter_;
			this.updateFiber = DownCoroutine(blownVec, bulletDirection, staMotion, midMotion, endMotion);
		}
		protected Down() { }

		public override bool Update()
		{
            if (updateFiber == null) {
                return false;
            }
			return updateFiber.MoveNext();
		}
		public override void Finish()
		{
			this.ResetModelRotation();
		}

		private IEnumerator DownCoroutine(Vector3 blownVec, float bulletDirection, MotionState staMotion, MotionState midMotion, MotionState endMotion)
		{
			IEnumerator blownCoroutine = BlownCoroutine(blownVec, bulletDirection, staMotion, midMotion, endMotion);
			while(blownCoroutine.MoveNext()) { yield return null; }

			// DownTimer時間で起き上がる.
			float wakeTime = Time.time + GameConstant.DownTimer;
			while(Time.time < wakeTime) { yield return null; }

			this.personAdapter.Person.Wake();
			yield return null;
		}

		protected IEnumerator BlownCoroutine(Vector3 blownVec, float bulletDirection, MotionState staMotion, MotionState midMotion, MotionState endMotion)
		{
			// モーションファイバー.
			IEnumerator motionFiber = this.personAdapter.BotOrderAnimCoroutine(staMotion, midMotion);
			motionFiber.MoveNext();

			this.personAdapter.Person.transform.rotation = Quaternion.Euler(new Vector3(0, bulletDirection + 180, 0));
			//this.SetNextRotation(this.playerAdapter.Player.transform.rotation);
			Vector3 velocity = this.personAdapter.Person.transform.rotation * blownVec;

			{
				// 強制的に浮いた状態にする.
				this.personAdapter.Person.CharacterMove.DirectionReset();
				this.personAdapter.Person.CharacterMove.IsGrounded = false;

				Vector3 movement = Vector3.one;
				while(0 < movement.y || !this.personAdapter.Person.CharacterMove.IsGrounded)
				{
					// 移動.
					this.personAdapter.CalculateMove(velocity, out movement);
					this.personAdapter.Person.MovePosition(movement);
					this.personAdapter.Person.CharacterMove.GravityMag = 1;
					this.personAdapter.Person.CharacterMove.UseInertia = false;
					// 角度.
					if(this.personAdapter.Person.AvaterModel.ModelTransform)
					{
						this.personAdapter.Person.AvaterModel.ModelTransform.LookAt(this.personAdapter.Person.Position - velocity);
					}
					// モーション.
					motionFiber.MoveNext();

					yield return null;

					velocity = this.personAdapter.Person.CharacterMove.Velocity;
				}

				this.DownBlownFinish(endMotion);
			}
		}

		private void DownBlownFinish(MotionState motion)
		{
			this.personAdapter.SendMotion(motion);
			this.personAdapter.Person.ScmAnimation.UpdateAnimation(motion, (int)MotionLayer.ReAction, 0f, PlayMode.StopAll);
			this.ResetModelRotation();
			this.personAdapter.Person.CharacterMove.DirectionReset();
			EffectManager.CreateDown(this.personAdapter.Person, GameConstant.EffectDown);
		}

		private void ResetModelRotation()
		{
            if (this.personAdapter == null) {
                return;
            }
			this.personAdapter.SetModelRotation(Quaternion.identity);
		}

		public override bool IsSkillUsable() { return false; }	// ×スキル使用不可.
		public override bool CanFalter() { return false; }		// ×怯みナシ.
		public override bool CanBind() { return false; }		// △マヒモーションにはならない(起き上がり後,効果時間内なら改めてマヒる.マヒ時受け身不可).
		public override bool CanJump() { return false; }		// ×ジャンプしない.
		public override bool CanWire() { return false; }		// ×ワイヤー移動しない.
	}
}