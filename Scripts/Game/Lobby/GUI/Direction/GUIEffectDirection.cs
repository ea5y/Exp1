using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

/// <summary>
/// 演出用
/// </summary>
public class GUIEffectDirection : MonoBehaviour
{
	
	private class WaitEffectItem : IFiberWait
	{
		private IEffectMessageItem messageItem;

		public WaitEffectItem(IEffectMessageItem item)
		{
			this.messageItem = item;
		}

		#region IFiberWait

		public bool IsWait
		{
			get
			{
				bool isWait = (!this.messageItem.IsDelete);
				//if(this.messageItem.IsDelete) {
				//	// アイテム削除
				//	this.messageItem.Delete();
				//}
				
				return isWait;
			}
		}

		#endregion
	}


	#region === Field ===

	/// <summary>
	/// 生成元のエフェクトメッセージアイテム
	/// </summary>
	[SerializeField]
	private GUIEffectMsgItemBase prefab = null;

	/// <summary>
	/// 各レイヤーごとにアイテムを追加する親オブジェクト
	/// </summary>
	[SerializeField]
	private GameObject parent = null;
	

	private FiberSet fiberSet = new FiberSet();

	private GUIEffectMsgItemBase currentEffect = null;

	private Action endCallback = null;

	#endregion === Field ===



	/// <summary>
	/// 実行する
	/// </summary>
	public void ExecuteEffect()
	{
		ExecuteEffect(null);
	}

	/// <summary>
	/// コールバック指定で実行する
	/// </summary>
	/// <param name="callback"></param>
	public void ExecuteEffect(Action callback)
	{
		// 実行中の時は追加しない
		if(currentEffect != null) {
			return;
		}

		endCallback = callback;

		Create();
	}


	/// <summary>
	/// スキップする
	/// </summary>
	public void Skip()
	{
		// 実行中のみ
		if(currentEffect == null) return;

		currentEffect.IsDelete = true;
	}

	/// <summary>
	/// 削除
	/// </summary>
	public void Close()
	{
		if(currentEffect == null) return;

		currentEffect.Delete();
		currentEffect = null;

		fiberSet.Clear();

		endCallback = null;
	}


	private void Create()
	{
		// アイテム生成
		currentEffect = SafeObject.Instantiate(prefab);

		// 親子関連づけ
		currentEffect.gameObject.SetParentWithLayer(parent, false);

		currentEffect.Setup(true);
		
		fiberSet.AddFiber(CreateEnumerator(currentEffect));
	}

	/// <summary>
	/// メッセージ更新処理
	/// </summary>
	private IEnumerator CreateEnumerator(GUIEffectMsgItemBase item)
	{
		yield return new WaitEffectItem(item);
	}

	private void Update()
	{
		if(fiberSet.Count == 0) return;
		
		if(!fiberSet.Update()) {
			if(endCallback != null) {
				endCallback();
				endCallback = null;
			}
		}
	}

#if UNITY_EDITOR

	[UnityEditor.CustomEditor(typeof(GUIEffectDirection))]
	private class GUIEffectDirectionEditor : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			var gui = target as GUIEffectDirection;
			if(gui != null && Application.isPlaying) {
				if(GUILayout.Button("Execute")) {
					gui.ExecuteEffect();
				}
				if(GUILayout.Button("Skip")) {
					gui.Skip();
				}
			}

			base.OnInspectorGUI();
		}
	}


#endif

}
