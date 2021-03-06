/// <summary>
/// 位置スクロール
/// 
/// 2014/11/10
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GUIPositionScroll : MonoBehaviour
{
	#region フィールド＆プロパティ
	/// <summary>
	/// 位置リスト
	/// </summary>
	[SerializeField]
	List<Vector3> _list;
	List<Vector3> List { get { return _list; } }

	/// <summary>
	/// アタッチオブジェクト
	/// </summary>
	[SerializeField]
	AttachObject _attach;
	AttachObject Attach { get { return _attach; } }
	[System.Serializable]
	public class AttachObject
	{
		public TweenPosition tween;
	}
	#endregion

	#region 設定
	public void Setup(int index)
	{
		try
		{
			var position = this.List[index];
			var tw = this.Attach.tween;
			if (tw != null)
			{
				var style = tw.style;
				var animationCurve = tw.animationCurve;
				var eventReceiver = tw.eventReceiver;
				var callWhenFinished = tw.callWhenFinished;
				var delay = tw.delay;
				var com = TweenPosition.Begin(tw.gameObject, tw.duration, position);
				com.style = style;
				com.animationCurve = animationCurve;
				com.eventReceiver = eventReceiver;
				com.callWhenFinished = callWhenFinished;
				com.delay = delay;
			}
		}
		catch (System.Exception e)
		{
			Debug.LogError(e);
			BugReportController.SaveLogFile(e.ToString());
		}
	}
	#endregion
}
