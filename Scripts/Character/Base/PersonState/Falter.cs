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
	public class Falter : PersonState
        {
		public override Character.StateProc StateProc { get { return Character.StateProc.Recoil; } }

		private IEnumerator updateFiber;

		private Person.PersonStateAdapter personAdapter;
		private Vector3 blowDirection;
		private float blowSpeed;
		private MotionState motion;

		public Falter(Person.PersonStateAdapter personAdapter_, Vector3 blowDirection, float blowSpeed, MotionState motion)
		{
			this.personAdapter = personAdapter_;
			this.blowDirection = blowDirection;
			this.blowSpeed = blowSpeed;
			this.motion = motion;
			this.updateFiber = FalterCoroutine();
		}

		public override bool Update()
		{
			return updateFiber.MoveNext();
		}

		IEnumerator FalterCoroutine()
		{
			// モーション
			this.personAdapter.PlayActionAnimation(motion);
			// ボイス.
			//this.personAdapter.PlayVoice(MotionName.GetName(motion));

			float time = this.personAdapter.Person.ScmAnimation.GetMotionTime(motion);
			float endTime = Time.time + time;
			while(Time.time < endTime)
			{
				// 移動
				Vector3 movement;
				this.personAdapter.Person.CharacterMove.CalculateMoveImpulse(blowDirection * blowSpeed, Time.deltaTime, out movement);
				this.personAdapter.Person.MovePosition(movement);
				yield return null;
			}
		}


		public override bool IsSkillUsable() { return false; }	// ×スキル使用不可.
		public override bool CanFalter() { return true; }		// ○怯みアリ(上書き).
		public override bool CanBind() { return false; }		// △マヒモーションにならない(怯みモーションを最後まで再生した後,まだマヒ効果時間内なら改めてマヒる).
		public override bool CanJump() { return true; }			// ○ジャンプする.
		public override bool CanWire() { return true; }			// ○ワイヤー移動する.
	}
}