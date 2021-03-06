/// <summary>
/// XUIButtonイベント時にTweenAlphaの制御を行うスクリプト.
/// 
/// 2014/06/12.
/// </summary>
using UnityEngine;
using System.Collections;
using System;

[System.Serializable]
public class XUITweenAlphaButtonEvent : IXUIButtonEvent, ICloneable
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
		public float endTo;
		
		/// <summary>
		/// 再生モード.
		/// </summary>
		public UITweener.Style playStyle;
		
		/// <summary>
		/// 各パラメターをセットする.
		/// </summary>
		public void SetParameter(float to, UITweener.Style style)
		{
			endTo = to;
			playStyle = style;
		}
	}
	
	#endregion
	
	#region フィールド&プロパティ
	
	[SerializeField]
	private TweenAlpha targetTweenAlpha;
	public TweenAlpha TargetTweenAlpha { get { return targetTweenAlpha; } }

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
	private float defaultFrom;
	
	#endregion
	
	#region 初期化.
	
	public void Init()
	{
		// ノーマルイベントデータのセット.
		if(this.targetTweenAlpha != null)
		{
			this.normal.SetParameter(this.targetTweenAlpha.to, this.targetTweenAlpha.style);
			this.normal.isEnable = false;
			this.defaultFrom = this.targetTweenAlpha.from;
			this.defaultDuration = this.targetTweenAlpha.duration;
		}
	}
	
	#endregion
	
	#region XUIButtonイベント.
	
	/// <summary>
	/// Normalイベント以外のイベント処理が実行された時のみNormalイベント処理を行う.
	/// </summary>
	public void OnNormal(XUIButton button, bool immediate)
	{
		if(this.targetTweenAlpha != null)
		{
			SetTweenAlphaData(this.normal, this.defaultFrom, this.defaultDuration, button, immediate);
			this.normal.isEnable = false;
		}
	}
	
	public void OnHover(XUIButton button, bool immediate)
	{
		if(this.targetTweenAlpha == null)
			return;
		
		if(this.hover.isEnable)
		{
			// Hoverイベントが有効ならイベント処理を行う.
			SetTweenAlphaData(this.hover, this.targetTweenAlpha.value, button.duration, button, immediate);
			
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
		if(this.targetTweenAlpha == null)
			return;
		
		if(this.pressed.isEnable)
		{
			// Hoverイベントが有効ならイベント処理を行う.
			SetTweenAlphaData(this.pressed, this.targetTweenAlpha.value, button.duration, button, immediate);
			
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
		if(this.targetTweenAlpha == null)
			return;
		
		if(this.disabled.isEnable)
		{
			// Hoverイベントが有効ならイベント処理を行う.
			SetTweenAlphaData(this.disabled, this.targetTweenAlpha.value, button.duration, button, immediate);
			
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
	
	#region TweenColorに各データをセット.
	
	private void SetTweenAlphaData(EventData eventData, float from, float duration, XUIButton button, bool immediate)
	{
		if(!eventData.isEnable)
		{
			return;
		}
		
		// TweenAlpha再生.
		this.targetTweenAlpha.style = eventData.playStyle;
		this.targetTweenAlpha.from = from;
		this.targetTweenAlpha.to = eventData.endTo;
		this.targetTweenAlpha.duration = duration;
		this.targetTweenAlpha.ResetToBeginning();
		this.targetTweenAlpha.Play(true);
		
		if(immediate)
		{
			this.targetTweenAlpha.value = this.targetTweenAlpha.to;
			this.targetTweenAlpha.enabled = false;
		}
	}
	
	#endregion
	
	#region Inspectorメニュー
	
	/// <summary>
	/// TweenするオブジェクトのAlphaをセットする.
	/// </summary>
	public void WidgetCopyParameters()
	{
		if(this.targetTweenAlpha == null)
			return;
		
		UIWidget widget = this.targetTweenAlpha.gameObject.GetSafeComponent<UIWidget>();
		if(widget == null)
			return;
		
		this.hover.SetParameter(widget.alpha, this.targetTweenAlpha.style);
		this.pressed.SetParameter(widget.alpha, this.targetTweenAlpha.style);
		this.disabled.SetParameter(widget.alpha, this.targetTweenAlpha.style);
	}
	
	#endregion
	
	#region 複製.
	
	public XUITweenAlphaButtonEvent Clone()
	{
		return (XUITweenAlphaButtonEvent)MemberwiseClone();
	}
	
	object ICloneable.Clone()
	{
		return Clone();
	}
	
	#endregion
}
