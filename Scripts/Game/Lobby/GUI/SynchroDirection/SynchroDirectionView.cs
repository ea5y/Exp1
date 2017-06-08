using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace XUI.SynchroDirection
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

		void Setup();

		void SetResultMessage(Scm.Common.GameParameter.PowerupResult result);

		/// <summary>
		/// キャラモデルをセットする
		/// </summary>
		void SetBaseCharaInfo(EntrantInfo info);


		/// <summary>
		/// エサキャラモデルをセットする
		/// </summary>
		void SetBaitCharaInfo(EntrantInfo info);

		
		// 動かすのに泣く泣く・・・
		void MoveStart();

		void MoveEnd();
	}

	[DisallowMultipleComponent]
	public class SynchroDirectionView : GUIViewBase, IView
	{
		[Serializable]
		private class ResultMessage
		{
			public Scm.Common.GameParameter.PowerupResult result = Scm.Common.GameParameter.PowerupResult.Good;

			public GameObject resultObject = null;
		}


		#region === Field ===

		// 移動先NullObject
		[SerializeField]
		private Transform moveNullTarget = null;
		
		// キャラのNullObject
		[SerializeField]
		private Transform charaNullTarget = null;

		// エサキャラのNullObject
		[SerializeField]
		private Transform baitNullTarget = null;

		// 成功とかのメッセージ
		[SerializeField]
		private List<ResultMessage> successMesList = new List<ResultMessage>();


		private UITweener charaTween = null;

		private UITweener baitTween = null;


		// キャラ表示
		private EntrantInfo baseCharaInfo = null;

		// エサキャラ
		private EntrantInfo baitCharaInfo = null;


		private bool isMoving = false;

		private Vector3 lookPos = Vector3.zero;

		#endregion === Field ===

		#region === Event ===

		public event EventHandler OnCharaCreated = (sender, e) => { };

		public event EventHandler OnSkip = (sender, e) => { };

		#endregion === Event ===

		#region === Property ===

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

			// キャラ破棄
			if(baseCharaInfo != null) {
				baseCharaInfo.Remove(true);
				baseCharaInfo = null;
			}
			if(baitCharaInfo != null) {
				baitCharaInfo.Remove(true);
				baitCharaInfo = null;
			}
		}


		public void Setup()
		{
			if(charaNullTarget != null) {
				charaTween = charaNullTarget.GetComponent<UITweener>();
			}

			if(baitNullTarget != null) {
				baitTween = baitNullTarget.GetComponent<UITweener>();
			}
			
			lookPos = UI3DFXCamera.Instance.transform.position;
			if(moveNullTarget != null) {
				lookPos.y = moveNullTarget.position.y;
			}

		}

		#region === NGUIリフレクション ===

		public void OnDirectionSkip()
		{
			OnSkip(this, EventArgs.Empty);
		}

		#endregion === NGUIリフレクション ===

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
		/// キャラモデルをセットする
		/// </summary>
		public void SetBaseCharaInfo(EntrantInfo info)
		{
			if(baseCharaInfo != null) {
				baseCharaInfo.Remove(true);
				baseCharaInfo = null;
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

			baseCharaInfo.GameObject.LoadModelCompletedEvent -= HandleLoadModelCompletedEvent;


			baseCharaInfo.GameObject.gameObject.SetLayerChild(LayerNumber.UI3DFX);

			baseCharaInfo.GameObject.GetComponent<Person>().enabled = false;

			if(charaNullTarget != null) {
				baseCharaInfo.GameObject.transform.position = charaNullTarget.localPosition;
			}
			baseCharaInfo.GameObject.transform.LookAt(lookPos);


			OnCharaCreated(this, EventArgs.Empty);
		}



		/// <summary>
		/// エサキャラモデルをセットする
		/// </summary>
		public void SetBaitCharaInfo(EntrantInfo info)
		{
			if(baitCharaInfo != null) {
				baitCharaInfo.Remove(true);
				baitCharaInfo = null;
			}

			baitCharaInfo = info;
			if(baitCharaInfo == null) return;

			// 位置調整とかする
			var child = baitCharaInfo.GameObject.transform.GetChild(0);
			if(child != null && child.transform.childCount > 0) {
				HandleLoadBaitModelCompletedEvent();
			} else {
				baitCharaInfo.GameObject.LoadModelCompletedEvent += HandleLoadBaitModelCompletedEvent;
			}

		}

		/// <summary>
		/// モデル読み込み終了時
		/// </summary>
		private void HandleLoadBaitModelCompletedEvent()
		{
			if(baitCharaInfo == null) return;

			baitCharaInfo.GameObject.LoadModelCompletedEvent -= HandleLoadBaitModelCompletedEvent;
			
			baitCharaInfo.GameObject.gameObject.SetLayerChild(LayerNumber.UI3DFX);

			baitCharaInfo.GameObject.GetComponent<Person>().enabled = false;

			if(baitNullTarget != null) {
				baitCharaInfo.GameObject.transform.position = baitNullTarget.localPosition;
			}
			baitCharaInfo.GameObject.transform.LookAt(lookPos);

			OnCharaCreated(this, EventArgs.Empty);
		}


		/// <summary>
		/// キャラの移動開始する
		/// </summary>
		public void MoveStart()
		{
			if(charaTween == null || baitTween == null) return;

			isMoving = true;

			charaTween.ResetToBeginning();
			charaTween.PlayForward();
			EventDelegate.Add(charaTween.onFinished, () => MoveEnd(), true);

			baitTween.ResetToBeginning();
			baitTween.PlayForward();
			EventDelegate.Add(baitTween.onFinished, () => baitCharaInfo.GameObject.gameObject.SetActive(false), true);
		}

		/// <summary>
		/// 移動終了する
		/// </summary>
		public void MoveEnd()
		{
			isMoving = false;
		}


		private void LateUpdate()
		{
			// 移動入れるか・・・
			if(!isMoving) return;
			
			if(charaNullTarget != null) {
				baseCharaInfo.GameObject.transform.position = charaNullTarget.localPosition;
				baseCharaInfo.GameObject.transform.LookAt(lookPos);
			}

			if(baitNullTarget != null) {
				baitCharaInfo.GameObject.transform.position = baitNullTarget.localPosition;
				baitCharaInfo.GameObject.transform.LookAt(lookPos);
			}


		}
	}
}