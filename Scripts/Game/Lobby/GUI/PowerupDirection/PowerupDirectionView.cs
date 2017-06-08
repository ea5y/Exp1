using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

namespace XUI.PowerupDirection
{


	public interface IView
	{
		#region === Event ===

		event EventHandler OnCharaCreated;

		event EventHandler OnSkip;

		#endregion === Event ===

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

		#region === Property ===

		/// <summary>
		/// アイコンの個数
		/// </summary>
		int BaitIconCount { get; }

		#endregion === Property ===

		/// <summary>
		/// キャラモデルをセットする
		/// </summary>
		void SetBaseCharaInfo(EntrantInfo info);

		/// <summary>
		/// リザルトメッセージをセットする
		/// </summary>
		void SetResultMessage(Scm.Common.GameParameter.PowerupResult result);

		/// <summary>
		/// エサアイコンの表示切り替え
		/// </summary>
		/// <param name="index"></param>
		/// <param name="visible"></param>
		void SetBaitIconVisible(int index, bool visible);

		/// <summary>
		/// エサアイコンの変更
		/// </summary>
		/// <param name="index"></param>
		/// <param name="atlas"></param>
		/// <param name="spriteName"></param>
		void SetBaitIcon(int index, UIAtlas atlas, string spriteName);
	}

	[DisallowMultipleComponent]
	public class PowerupDirectionView : GUIViewBase, IView
	{
		[Serializable]
		private class ResultMessage
		{
			public Scm.Common.GameParameter.PowerupResult result = Scm.Common.GameParameter.PowerupResult.Good;

			public GameObject resultObject = null;
		}

		[Serializable]
		private class BaitIcon
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
		private BaitIcon[] baitIconList = null;

		// 成功とかのメッセージ
		[SerializeField]
		private List<ResultMessage> successMesList = new List<ResultMessage>();

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
		public int BaitIconCount
		{
			get { return (baitIconList == null ? 0 : baitIconList.Length); }
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
			if(child != null&& child.transform.childCount > 0) {
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

			baseCharaInfo.GameObject.LoadModelCompletedEvent -= HandleLoadModelCompletedEvent;

			baseCharaInfo.GameObject.gameObject.SetLayerChild(LayerNumber.UI3DFX);

			baseCharaInfo.GameObject.GetComponent<Person>().enabled = false;

			baseCharaInfo.GameObject.transform.position = charaNullTarget.localPosition;
			baseCharaInfo.GameObject.transform.localRotation = Quaternion.Euler(0, 180, 0);
			
			OnCharaCreated(this, EventArgs.Empty);
		}
		

		/// <summary>
		/// リザルトメッセージをセットする
		/// </summary>
		public void SetResultMessage(Scm.Common.GameParameter.PowerupResult result)
		{
			for(int i = 0; i < successMesList.Count; i++) {
				successMesList[i].resultObject.SetActive(successMesList[i].result == result);
			}
		}


		/// <summary>
		/// エサアイコンの表示セット
		/// </summary>
		/// <param name="index"></param>
		/// <param name="visible"></param>
		public void SetBaitIconVisible(int index, bool visible)
		{
			if(baitIconList == null || index < 0 || index >= baitIconList.Length) return;

			baitIconList[index].group.SetActive(visible);
		}

		/// <summary>
		/// エサアイコンのスプライトセット
		/// </summary>
		/// <param name="index"></param>
		/// <param name="atlas"></param>
		/// <param name="spriteName"></param>
		public void SetBaitIcon(int index, UIAtlas atlas, string spriteName)
		{
			if(baitIconList == null || index < 0 || index >= baitIconList.Length) return;
			
			CharaIcon.SetIconSprite(baitIconList[index].sprite, atlas, spriteName);
		}


	}
}


