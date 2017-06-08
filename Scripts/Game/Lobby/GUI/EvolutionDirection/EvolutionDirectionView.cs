using UnityEngine;
using System.Collections;
using System;

namespace XUI.EvolutionDirection
{

	public interface IView
	{
		#region === Event ===

		event EventHandler OnCharaCreated;

		event EventHandler OnSkip;

		#endregion === Event ===

		int MaterialIconCount { get; }

		#region === アクティブ ===

			/// <summary>
			/// アクティブ状態を取得する
			/// </summary>
		bool IsActive { get; }
		
		/// <summary>
		/// アクティブ状態にする
		/// </summary>
		void SetActive(bool isActive, bool isTweenSkip);

		/// <summary>
		/// 現在のアクティブ状態を取得する
		/// </summary>
		GUIViewBase.ActiveState GetActiveState();

		#endregion === アクティブ ===

		/// <summary>
		/// キャラモデルをセットする
		/// </summary>
		void SetBaseCharaInfo(EntrantInfo info);


		/// <summary>
		/// 素材アイコンの表示セット
		/// </summary>
		/// <param name="index"></param>
		/// <param name="visible"></param>
		void SetMaterialIconVisible(int index, bool visible);


		/// <summary>
		/// 素材アイコンのスプライトセット
		/// </summary>
		/// <param name="index"></param>
		/// <param name="atlas"></param>
		/// <param name="spriteName"></param>
		void SetMaterialIcon(int index, UIAtlas atlas, string spriteName);
	}

	[DisallowMultipleComponent]
	public class EvolutionDirectionView : GUIViewBase, IView
	{
		[Serializable]
		private class MaterialIcon
		{
			public GameObject group = null;

			public UISprite sprite = null;
		}

		#region === Field ===

		// キャラのNullObject
		[SerializeField]
		private Transform charaNullTarget = null;

		// アイコン 一覧
		[SerializeField]
		private MaterialIcon[] materialIconList = null;
		

		// キャラ表示
		private EntrantInfo baseCharaInfo = null;

		#endregion === Field ===
		
		#region === Event ===

		public event EventHandler OnCharaCreated = (sender, e) => { };

		public event EventHandler OnSkip = (sender, e) => { };

		#endregion === Event ===

		#region === Property ===

		/// <summary>
		/// アイコンの個数
		/// </summary>
		public int MaterialIconCount
		{
			get { return (materialIconList == null ? 0 : materialIconList.Length); }
		}

		#endregion === Property ===

		#region === アクティブ ===

		/// <summary>
		/// アクティブ状態にする
		/// </summary>
		public void SetActive(bool isActive, bool isTweenSkip)
		{
			this.SetRootActive(isActive, isTweenSkip);
		}

		/// <summary>
		/// 現在のアクティブ状態を取得する
		/// </summary>
		public GUIViewBase.ActiveState GetActiveState()
		{
			return this.GetRootActiveState();
		}

		#endregion === アクティブ ===

		/// <summary>
		/// オブジェクトが破棄されたとき
		/// </summary>
		private void OnDestroy()
		{
			OnCharaCreated = null;
			OnSkip = null;

			if(baseCharaInfo != null) {
				baseCharaInfo.Remove(true);
				baseCharaInfo = null;
			}
		}

		#region === NGUIリフレクション ===
		
		public void OnDirectionSkip()
		{
			OnSkip(this, EventArgs.Empty);
		}

		#endregion === NGUIリフレクション ===


		/// <summary>
		/// キャラモデルをセットする
		/// </summary>
		public void SetBaseCharaInfo(EntrantInfo info)
		{
			if(baseCharaInfo != null) {
				baseCharaInfo.Remove(true);
			}

			baseCharaInfo = info;
			if(baseCharaInfo == null) return;


			// 位置調整とかする
			var child = baseCharaInfo.GameObject.transform.GetChild(0);
			if(child != null && child.transform.childCount > 0) {
				HandleLoadModelCompletedEvent();
			} else {
				baseCharaInfo.GameObject.LoadModelCompletedEvent += HandleLoadModelCompletedEvent;
			}

		}

		/// <summary>
		/// モデル読み込み終了時
		/// </summary>
		private void HandleLoadModelCompletedEvent()
		{
			if(baseCharaInfo == null) return;

			baseCharaInfo.GameObject.gameObject.SetLayerChild(LayerNumber.UI3DFX);

			baseCharaInfo.GameObject.GetComponent<Person>().enabled = false;

			//// 指定ボーンにアタッチする.
			//Transform attachBone = baseCharaInfo.GameObject.transform.Search("Hit_Root");
			//if(attachBone != null) {
			//	Debug.Log(attachBone.position);
			//	baseCharaInfo.GameObject.transform.position = -attachBone.localPosition;
			//}
			if(charaNullTarget != null) {
				baseCharaInfo.GameObject.transform.position = charaNullTarget.localPosition;
			}
			baseCharaInfo.GameObject.transform.localRotation = Quaternion.Euler(0, 180, 0);

			OnCharaCreated(this, EventArgs.Empty);
		}

		/// <summary>
		/// 素材アイコンの表示セット
		/// </summary>
		/// <param name="index"></param>
		/// <param name="visible"></param>
		public void SetMaterialIconVisible(int index, bool visible)
		{
			if(materialIconList == null || index < 0 || index >= materialIconList.Length) return;

			materialIconList[index].group.SetActive(visible);
		}

		/// <summary>
		/// 素材アイコンのスプライトセット
		/// </summary>
		/// <param name="index"></param>
		/// <param name="atlas"></param>
		/// <param name="spriteName"></param>
		public void SetMaterialIcon(int index, UIAtlas atlas, string spriteName)
		{
			if(materialIconList == null || index < 0 || index >= materialIconList.Length) return;

			CharaIcon.SetIconSprite(materialIconList[index].sprite, atlas, spriteName);
		}

	}
}

