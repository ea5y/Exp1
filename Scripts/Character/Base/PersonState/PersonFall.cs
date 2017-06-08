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
    public class PersonFall : PersonState {
        const float LandingStopSpeed = 9;   // これ以上速い速度で落ちたら着地硬直が発生.

        public override Character.StateProc StateProc { get { return Character.StateProc.UserJump; } }

        private IEnumerator updateFiber;

        private Person.PersonStateAdapter personAdapter;
        private Vector3 end;
        private Vector3 velocity;

        public PersonFall(Person.PersonStateAdapter personAdapter, IEnumerator fallAnimFiber) {
            this.personAdapter = personAdapter;
            this.updateFiber = FallProcCoroutine(fallAnimFiber);
        }

        public override bool Update() {
            return updateFiber.MoveNext();
        }
        public override void Finish() {
            this.personAdapter.ResetAnimation();
            this.personAdapter.SendMovePacket();
        }

        IEnumerator FallProcCoroutine(IEnumerator fallAnimFiber) {
            // 自由落下.
            float maxFallSpeed2 = this.personAdapter.Person.CharacterMove.Velocity.sqrMagnitude;
            this.personAdapter.Person.gameObject.layer = Person.PERSON_LAYER;
            while (!this.personAdapter.Person.CharacterMove.IsGrounded) {
                maxFallSpeed2 = Mathf.Max(maxFallSpeed2, this.personAdapter.Person.CharacterMove.Velocity.sqrMagnitude);
                fallAnimFiber.MoveNext();
                yield return null;
            }
            // 着地硬直.
            if (LandingStopSpeed * LandingStopSpeed < maxFallSpeed2) {
                this.personAdapter.Person.ScmAnimation.UpdateAnimation(MotionState.jump_end, (int)MotionLayer.ReAction);
                float nextTime = Time.time + this.personAdapter.Person.ScmAnimation.GetAnimationLength(MotionState.jump_end.ToString());
                // エフェクト.
                EffectManager.CreateDown(this.personAdapter.Person, GameConstant.EffectDown);
                this.personAdapter.Person.CharacterMove.DirectionReset();
                while (Time.time < nextTime) { yield return null; }
                this.personAdapter.SendMotion(MotionState.jump_end);
            }
        }

        public override bool IsSkillUsable() { return true; }   // ○スキル使用可.ただし空中使用可のスキルのみ.
        public override bool CanFalter() { return true; }       // ○怯みアリ.
        public override bool CanBind() { return true; }         // ○マヒアリ(ダウン落下に移行).
        public override bool CanJump() { return true; }         // ○ジャンプアリ(連続ジャンプもありうる).
        public override bool CanWire() { return true; }			// ○ワイヤー移動アリ.
    }
}
