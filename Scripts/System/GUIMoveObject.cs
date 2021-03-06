/// <summary>
/// オブジェクト移動
/// 
/// 2013/03/27
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(TweenPosition))]
[RequireComponent(typeof(UIPlayTween))]
public class GUIMoveObject : MonoBehaviour
{
	#region フィールド＆プロパティ
	public List<Transform> moveTransformList = new List<Transform>();
	public uint nowIndex = 0;

	public UIPlayTween PlayTween { get; set; }
	public TweenPosition TweenPosition { get; set; }
	#endregion

	#region MonoBehaviourリフレクション
	void Start()
	{
		this.PlayTween = this.gameObject.GetSafeComponent<UIPlayTween>();
		this.TweenPosition = this.gameObject.GetSafeComponent<TweenPosition>();

		this.SetIndex(this.nowIndex, false);
	}
	#endregion

	#region Method
	public void SetTransform(Transform transform)
	{
		int index = this.moveTransformList.IndexOf(transform);
		if (0 > index)
			{ return; }

		this.SetIndex((uint)index, true);
	}
	public void SetIndex(uint index, bool isTween)
	{
		if (!isTween)
		{
			if (index < this.moveTransformList.Count)
			{
				this.transform.localPosition = this.moveTransformList[(int)index].localPosition;
				this.nowIndex = index;
			}
			return;
		}

		if (this.PlayTween == null)
			{ return; }
		if (this.TweenPosition == null)
			{ return; }
		if (index >= this.moveTransformList.Count)
			{ return; }

		this.TweenPosition.from = this.transform.localPosition;
		this.TweenPosition.to = this.moveTransformList[(int)index].localPosition;
		this.nowIndex = index;

		this.PlayTween.resetOnPlay = true;
		this.PlayTween.Play(true);
	}
	public void SetIndex(uint index)
	{
	}
	#endregion
}
