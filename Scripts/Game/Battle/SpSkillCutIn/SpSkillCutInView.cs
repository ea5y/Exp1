/// <summary>
/// 必殺カットイン技表示
/// 
/// 2015/12/03
/// </summary>
using UnityEngine;
using System.Collections;

namespace XUI
{
	namespace SpSkillCutIn
	{
		/// <summary>
		/// 必殺カットイン技表示インターフェイス
		/// </summary>
		public interface IView
		{
			/// <summary>
			/// カットイン再生
			/// </summary>
			/// <param name="avatarType"></param>
			/// <param name="skillName"></param>
			void PlayCutIn(AvatarType avatarType, int skinId, string skillName);

			/// <summary>
			/// 演出終了
			/// </summary>
			void Finish();
		}

		/// <summary>
		/// 必殺技カットイン表示
		/// </summary>
		public class SpSkillCutInView : GUIViewBase, IView
		{
			#region フィールド&プロパティ
			/// <summary>
			/// カットイン用Tween再生
			/// </summary>
			[SerializeField]
			private UIPlayTween cutInPlayTween;
			public UIPlayTween CutInPlayTween { get { return cutInPlayTween; } }

			/// <summary>
			/// スキル名ラベル
			/// </summary>
			[SerializeField]
			private UILabel skillNameLabel = null;
			public UILabel SkillNameLabel { get { return skillNameLabel; } }

			/// <summary>
			/// キャラボードを配置するための親Tranform
			/// </summary>
			[SerializeField]
			private Transform charaBoardParent = null;
			public Transform CharaBoardParent { get { return charaBoardParent; } }

			/// <summary>
			/// キャラボード
			/// </summary>
			private CharaBoard CharaBoard { get { return ScmParam.Battle.CharaBoard; } }
			#endregion

			#region アクティブ設定
			/// <summary>
			/// アクティブ状態にする
			/// </summary>
			/// <param name="isActive"></param>
			public void SetActive(bool isActive)
			{
				this.SetRootActive(isActive, true);
			}

			/// <summary>
			/// 現在のアクティブ状態を取得する
			/// </summary>
			/// <returns></returns>
			public GUIViewBase.ActiveState GetActiveState()
			{
				return this.GetRootActiveState();
			}
			#endregion

			#region カットイン再生
			/// <summary>
			/// カットインを再生させる
			/// </summary>
			/// <param name="avatarType"></param>
			/// <param name="skillName"></param>
			public void PlayCutIn(AvatarType avatarType, int skinId, string skillName)
			{
				// キャラボード読み込み
				ReadCharaBoard(avatarType, skinId);

				// ラベルセット
				if (this.SkillNameLabel != null)
				{
					this.SkillNameLabel.text = skillName;
				}

				// カットインのTween再生
				if(this.CutInPlayTween != null)
				{
					this.CutInPlayTween.Play(true);
				}
			}
			#endregion

			#region キャラボード
			/// <summary>
			/// キャラボード読み込み
			/// </summary>
			/// <param name="avatarType"></param>
			private void ReadCharaBoard(AvatarType avatarType, int skinId)
			{
				// NULLチェック
				Transform parent = this.CharaBoardParent;
				if (parent == null) return;

				// 前回のキャラボードが残っている場合は削除する
				DeleteCharaBoard(parent);

				// 立ち絵読み込み
				this.CharaBoard.GetCutIn(avatarType, skinId, true, (GameObject resource) => { this.BoardSetup(resource, parent); });
			}

			/// <summary>
			/// キャラボードのセット
			/// </summary>
			private void BoardSetup(GameObject resource, Transform parent)
			{
				// リソース読み込み完了
				if (resource == null)
					return;
				// インスタンス化
				var go = SafeObject.Instantiate(resource) as GameObject;
				if (go == null)
					return;

				// 名前設定
				go.name = resource.name;
				// 親子付け
				var t = go.transform;
				t.parent = parent;
				t.localPosition = Vector3.zero;
				t.localRotation = Quaternion.identity;
				t.localScale = Vector3.one;
			}

			/// <summary>
			/// キャラボード削除
			/// </summary>
			private void DeleteCharaBoard(Transform parent)
			{
				for (int count = 0; count < parent.childCount; count++)
				{
					Transform child = parent.GetChild(count);
					Object.Destroy(child.gameObject);
				}
			}
			#endregion

			#region 演出終了
			/// <summary>
			/// 演出終了処理
			/// </summary>
			public void Finish()
			{
				// アクティブをOFFに設定
				this.gameObject.SetActive(false);

				// 読み込んだキャラボード削除
				if (this.CharaBoardParent != null)
				{
					DeleteCharaBoard(this.CharaBoardParent);
				}
			}
			#endregion
		}
	}
}