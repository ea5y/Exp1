/// <summary>
/// XUIButtonイベント時にTweenScaleの制御を行うスクリプト.
/// 
/// 2014/06/11.
/// </summary>
using UnityEngine;
using System.Collections;
using System;

[System.Serializable]
public class XUITweenScaleButtonEvent : IXUIButtonEvent, ICloneable
{
	#region イベントデータ
	
	[System.Serializable]
	public class EventData
	{
		/// <summary>
		/// データのセットを有効にするかどうか.
		/// </summary>
		public bool isEnable;
		
		/// <summary>
		/// 終了値.
		/// </summary>
		public Vector3 endTo;
		
		/// <summary>
		/// 再生モード.
		/// </summary>
		public UITweener.Style playStyle;
		
		/// <summary>
		/// 各パラメターをセットする.
		/// </summary>
		public void SetParameter(Vector3 to, UITweener.Style style)
		{
			endTo = to;
			playStyle = style;
		}
	}
	
	#endregion
	
	#region フィールド&プロパティ
	
	[SerializeField]
	private TweenScale targetTweenScale;
	public TweenScale TargetTweenScale { get { return targetTweenScale; } }
	
	private EventData normal = new EventData();
	
	public EventData hover;
	
	public EventData pressed;
	
	public EventData disabled;
	
	/// <summary>
	/// Tween期間のデフォルト値.
	/// </summary>
	private float defaultDuration;
	
	/// <summary>
	/// TweenのFromデフォルト値..
	/// </summary>
	private Vector3 defaultFrom;
	
	#endregion
	
	#region 初期化.
	
	public void Init()
	{
		// ノーマルイベントデータのセット.
		if(this.targetTweenScale != null)
		{
			this.normal.SetParameter(this.targetTweenScale.to, this.targetTweenScale.style);
			this.normal.isEnable = false;
			this.defaultFrom = this.targetTweenScale.from;
			this.defaultDuration = this.targetTweenScale.duration;
		}
	}
	
	#endregion
	
	#region XUIButtonイベント.
	
	/// <summary>
	/// Normalイベント以外のイベント処理が実行された時のみNormalイベント処理を行う.
	/// </summary>
	public void OnNormal(XUIButton button, bool immediate)
	{
		if(this.targetTweenScale != null)
		{
			SetTweenScaleData(this.normal, this.defaultFrom, this.defaultDuration, button, immediate);
			this.normal.isEnable = false;
		}
	}
	
	public void OnHover(XUIButton button, bool immediate)
	{
		if(this.targetTweenScale == null)
			return;
		
		if(this.hover.isEnable)
		{
			// Hoverイベントが有効ならイベント処理を行う.
			SetTweenScaleData(this.hover, this.targetTweenScale.value, button.duration, button, immediate);
			
			// Hoverイベント終了後にNormalイベント処理を行わせる.
			this.normal.isEnable = true;
		}
		else
		{
			if(this.normal.isEnable)
			{
				// 主に前回のイベント時にNormalイベントを有効にしているのに前回のイベントからすぐに.
				// このメソッドのイベントが呼ばれてイベントを無効にしていた場合.
				OnNormal(button, immediate);
			}
			this.normal.isEnable = false;
		}
	}
	
	public void OnPressed(XUIButton button, bool immediate)
	{
		if(this.targetTweenScale == null)
			return;
		
		if(this.pressed.isEnable)
		{
			// Hoverイベントが有効ならイベント処理を行う.
			SetTweenScaleData(this.pressed, this.targetTweenScale.value, button.duration, button, immediate);
			
			// Hoverイベント終了後にNormalイベント処理を行わせる.
			this.normal.isEnable = true;
		}
		else
		{
			if(this.normal.isEnable)
			{
				// 主に前回のイベント時にNormalイベントを有効にしているのに前回のイベントからすぐに.
				// このメソッドのイベントが呼ばれて無効にしていた場合.
				OnNormal(button, immediate);
			}
			this.normal.isEnable = false;
		}
	}
	
	public void OnDisabled(XUIButton button, bool immediate)
	{
		if(this.targetTweenScale == null)
			return;
		
		if(this.disabled.isEnable)
		{
			// Hoverイベントが有効ならイベント処理を行う.
			SetTweenScaleData(this.disabled, this.targetTweenScale.value, button.duration, button, immediate);
			
			// Hoverイベント終了後にNormalイベント処理を行わせる.
			this.normal.isEnable = true;
		}
		else
		{
			if(this.normal.isEnable)
			{
				// 主に前回のイベント時にNormalイベントを有効にしているのに前回のイベントからすぐに.
				// このメソッドのイベントが呼ばれて無効にしていた場合.
				OnNormal(button, immediate);
			}
			this.normal.isEnable = false;
		}
	}
	
	#endregion
	
	#region TweenScaleに各データをセット.
	
	private void SetTweenScaleData(EventData eventData, Vector3 from, float duration, XUIButton button, bool immediate)
	{
		if(!eventData.isEnable)
		{
			return;
		}
		
		// TweenScale再生.
		this.targetTweenScale.style = eventData.playStyle;
		this.targetTweenScale.from = from;
		this.targetTweenScale.to = eventData.endTo;
		this.targetTweenScale.duration = duration;
		this.targetTweenScale.ResetToBeginning();
		this.targetTweenScale.Play(true);
		
		if(immediate)
		{
			this.targetTweenScale.value = this.targetTweenScale.to;
			this.targetTweenScale.enabled = false;
		}
	}
	
	#endregion
	
	#region Inspectorメニュー
	
	/// <summary>
	/// TweenするオブジェクトのScaleをセットする.
	/// </summary>
	public void TransformCopyParameters()
	{
		if(this.targetTweenScale == null)
			return;
		
		Transform transform = this.targetTweenScale.gameObject.transform;
		this.hover.SetParameter(transform.localScale, this.targetTweenScale.style);
		this.pressed.SetParameter(transform.localScale, this.targetTweenScale.style);
		this.disabled.SetParameter(transform.localScale, this.targetTweenScale.style);
	}
	
	#endregion
	
	#region 複製.
	
	public XUITweenScaleButtonEvent Clone()
	{
		return (XUITweenScaleButtonEvent)MemberwiseClone();
	}
	
	object ICloneable.Clone()
	{
		return Clone();
	}
	
	#endregion
}
