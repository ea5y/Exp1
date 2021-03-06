/// <summary>
/// Tween系を同期させる
/// 
/// ※動的同期リストのリスト生成タイミングに関して
/// ※Start で一度リストを生成するため
/// ※DynamicSync.Root 以下の GameObject が Awake などで生成されていれば
/// ※プログラムから呼び出す必要はない
/// ※逆に Start 以降で GameObject を生成している場合は
/// ※生成後に適宜プログラムからリスト生成をしなければならない
/// 
/// 
/// 2015/04/14
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TweenSync : MonoBehaviour
{
	#region フィールド＆プロパティ
	[Header("Target が元になってリストに登録されている Tween を同期させる")]
	[Header("※同期先の Tween は【From, To, Animation Curve】以外の設定は無視される")]
	[SerializeField]
	UITweener _target = null;
	UITweener Target { get { return _target; } }

	[Header("事前に Tween を指定しておくリスト")]
	[SerializeField]
	List<UITweener> _syncList = new List<UITweener>();
	List<UITweener> SyncList { get { return _syncList; } }

	[Header("動的に生成された GameObject に対して同期させたいリスト")]
	[Header("※Target の TweenGroup と同じ Tween を同期する")]
	[SerializeField]
	List<DynamicSync> _dynamicSyncList = new List<DynamicSync>();
	List<DynamicSync> DynamicSyncList { get { return _dynamicSyncList; } }
	[System.Serializable]
	public class DynamicSync
	{
		[Header("Root 以下の Tween を動的に同期させる")]
		[SerializeField]
		GameObject _root = null;
		GameObject Root { get { return _root; } }

		List<UITweener> _syncList = new List<UITweener>();
		List<UITweener> SyncList
		{
			get
			{
				if (_syncList == null)
				{
					_syncList = new List<UITweener>();
				}
				return _syncList;
			}
		}

		/// <summary>
		/// Tween を factor の値で同期させる
		/// </summary>
		public void TweenSync(float factor)
		{
			this.SyncList.ForEach((tw) => { if (tw != null) tw.Sample(factor, false); });
		}

		/// <summary>
		/// 同期リストを更新する
		/// </summary>
		public void SyncListUpdate(int tweenGroup)
		{
			if (this.Root == null) return;

			// リスト初期化
			this.SyncList.Clear();
			var comps = this.Root.GetComponentsInChildren<UITweener>(true);
			if (comps != null)
			{
				foreach (var tw in comps)
				{
					if (tw == null) continue;
					if (tw.tweenGroup != tweenGroup) continue;

					// 同期させるので同期先の UITweener のスクリプトはオフにする
					tw.enabled = false;
					// リストに登録
					this.SyncList.Add(tw);
				}
			}
		}
	}
	#endregion

	#region 初期化
	void Start()
	{
		// 静的同期リストを更新する
		this.SyncListUpdate();
		// 動的同期リストを更新する
		this.DynamicSyncListUpdate();
	}
	/// <summary>
	/// 同期リストを更新する
	/// </summary>
	public void SyncListUpdate()
	{
		// 同期させるので同期先の UITweener のスクリプトはオフにする
		if (this.SyncList != null)
		{
			this.SyncList.ForEach((tw) => { if (tw != null) tw.enabled = false; });
		}
	}
	/// <summary>
	/// 動的同期リストを更新する
	/// </summary>
	public void DynamicSyncListUpdate()
	{
		if (this.Target == null) return;
		if (this.DynamicSyncList == null) return;

		var tweenGroup = this.Target.tweenGroup;
		this.DynamicSyncList.ForEach((t) => { if (t != null) t.SyncListUpdate(tweenGroup); });
	}
	#endregion

	#region 更新
	void Update()
	{
		if (this.Target == null) return;

		var factor = this.Target.tweenFactor;

		// 静的同期リストを同期させる
		if (this.SyncList != null)
		{
			this.SyncList.ForEach((tw) => { if (tw != null) tw.Sample(factor, false); });
		}
		// 動的同期リストを同期させる
		if (this.DynamicSyncList != null)
		{
			this.DynamicSyncList.ForEach((t) => { if (t != null) t.TweenSync(factor); });
		}
	}
	#endregion
}
