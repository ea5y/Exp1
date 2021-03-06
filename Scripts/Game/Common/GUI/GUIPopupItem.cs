/// <summary>
/// ポップアップアイテム
/// 
/// 2014/06/18
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GUIPopupItem : MonoBehaviour
{
	#region フィールド＆プロパティ
	/// <summary>
	/// アタッチオブジェクト
	/// </summary>
	[SerializeField] AttachObject _attach;
	public AttachObject Attach { get { return _attach; } }
	[System.Serializable]
	public class AttachObject
	{
		public UIWidget frameWidget;
		public UIPlayTween beginTween;
		public UIPlayTween moveTween;
		public UIPlayTween endTween;
	}

	public bool IsFinished { get; private set; }

	public float FrameHeight  { get; private set; }
	public TweenPosition BeginTween { get; private set; }
	public TweenPosition MoveTween { get; private set; }
	public TweenPosition EndTween { get; private set; }

	public int Index  { get; private set; }
	public System.Action<GUIPopupItem> EndFinishedFunc { get; private set; }

	bool IsInit { get; set; }
	#endregion

	#region 初期化＆削除
	void Awake()
	{
		this.Init();
	}
	void Init()
	{
		if (this.IsInit)
			return;
		this.FrameHeight = this.Attach.frameWidget.height;
		this.BeginTween = GetTweenPosition(this.Attach.beginTween);
		this.MoveTween = GetTweenPosition(this.Attach.moveTween);
		this.EndTween = GetTweenPosition(this.Attach.endTween);
		this.IsInit = true;
	}
	static TweenPosition GetTweenPosition(UIPlayTween p)
	{
		var coms = new List<TweenPosition>(p.tweenTarget.GetComponents<TweenPosition>());

		TweenPosition ret = null;
		foreach (var com in coms)
		{
			if (p.tweenGroup != com.tweenGroup)
				continue;
			ret = com;
			break;
		}

		return ret;
	}
	public void Destroy()
	{
		Object.Destroy(this.gameObject);
	}
	#endregion

	#region 設定
	public void Play(int index, System.Action<GUIPopupItem> onEndFinished)
	{
		// 初期化が済んでいなければ初期化する
		this.Init();

		// 項目設定
		this.Index = index;
		{
			float y = this.FrameHeight * this.Index;
			// 開始位置設定
			this.BeginTween.from.y -= y;
			this.BeginTween.to.y -= y;
			// 移動位置設定
			this.MoveTween.from.y -= y;
			this.MoveTween.to.y -= y;
		}
		this.EndFinishedFunc = (onEndFinished == null ? (item)=>{} : onEndFinished);

		// 再生
		this.IsFinished = false;
		this.Attach.beginTween.Play(true);
	}
	public bool SetNextMode()
	{
		// 再生中かどうか
		if (!this.IsFinished)
			return false;

		// 終了
		this.IsFinished = false;
		if (this.Index == 0)
		{
			this.Attach.endTween.Play(true);
		}
		else
		{
			this.Index--;
			this.MoveTween.from.y = this.MoveTween.to.y;
			this.MoveTween.to.y += this.FrameHeight;
			this.Attach.moveTween.Play(true);
		}

		return true;
	}
	#endregion

	#region NGUIリフレクション
	public void OnBeginFinished()
	{
		this.IsFinished = true;
	}
	public void OnMoveFinished()
	{
		this.IsFinished = true;
	}
	public void OnEndFinished()
	{
		this.IsFinished = true;
		this.EndFinishedFunc(this);
	}
	#endregion
}
