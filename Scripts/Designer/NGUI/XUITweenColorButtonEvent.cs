/// <summary>
/// XUIButtonイベント時にTweenColorの制御を行うスクリプト.
/// 
/// 2014/06/12.
/// </summary>
using UnityEngine;
using System.Collections;
using System;

[System.Serializable]
public class XUITweenColorButtonEvent : IXUIButtonEvent, ICloneable
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
		public Color endTo;
		
		/// <summary>
		/// 再生モード.
		/// </summary>
		public UITweener.Style playStyle;
		
		/// <summary>
		/// 各パラメターをセットする.
		/// </summary>
		public void SetParameter(Color to, UITweener.Style style)
		{
			endTo = to;
			playStyle = style;
		}
	}
	
	#endregion
	
	#region フィールド&プロパティ
	
	[SerializeField]
	private TweenColor targetTweenColor;
	public TweenColor TargetTweenColor { get { return targetTweenColor; } }

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
	private Color defaultFrom;
	
	#endregion
	
	#region 初期化.
	
	public void Init()
	{
		// ノーマルイベントデータのセット.
		if(this.targetTweenColor != null)
		{
			this.normal.SetParameter(this.targetTweenColor.to, this.targetTweenColor.style);
			this.normal.isEnable = false;
			this.defaultFrom = this.targetTweenColor.from;
			this.defaultDuration = this.targetTweenColor.duration;
		}
	}
	
	#endregion
	
	#region XUIButtonイベント.
	
	/// <summary>
	/// Normalイベント以外のイベント処理が実行された時のみNormalイベント処理を行う.
	/// </summary>
	public void OnNormal(XUIButton button, bool immediate)
	{
		if(this.targetTweenColor != null)
		{
			SetTweenColorData(this.normal, this.defaultFrom, this.defaultDuration, button, immediate);
			this.normal.isEnable = false;
		}
	}
	
	public void OnHover(XUIButton button, bool immediate)
	{
		if(this.targetTweenColor == null)
			return;
		
		if(this.hover.isEnable)
		{
			// Hoverイベントが有効ならイベント処理を行う.
			SetTweenColorData(this.hover, this.targetTweenColor.value, button.duration, button, immediate);
			
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
		if(this.targetTweenColor == null)
			return;
		
		if(this.pressed.isEnable)
		{
			// Hoverイベントが有効ならイベント処理を行う.
			SetTweenColorData(this.pressed, this.targetTweenColor.value, button.duration, button, immediate);
			
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
		if(this.targetTweenColor == null)
			return;
		
		if(this.disabled.isEnable)
		{
			// Hoverイベントが有効ならイベント処理を行う.
			SetTweenColorData(this.disabled, this.targetTweenColor.value, button.duration, button, immediate);
			
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
	
	private void SetTweenColorData(EventData eventData, Color from, float duration, XUIButton button, bool immediate)
	{
		if(!eventData.isEnable)
		{
			return;
		}
		
		// TweenColor再生.
		this.targetTweenColor.style = eventData.playStyle;
		this.targetTweenColor.from = from;
		this.targetTweenColor.to = eventData.endTo;
		this.targetTweenColor.duration = duration;
		this.targetTweenColor.ResetToBeginning();
		this.targetTweenColor.Play(true);
		
		if(immediate)
		{
			this.targetTweenColor.value = this.targetTweenColor.to;
			this.targetTweenColor.enabled = false;
		}
	}
	
	#endregion
	
	#region Inspectorメニュー
	
	/// <summary>
	/// TweenするオブジェクトのColorをセットする.
	/// </summary>
	public void WidgetCopyParameters()
	{
		if(this.targetTweenColor == null)
			return;
		
		UIWidget widget = this.targetTweenColor.gameObject.GetSafeComponent<UIWidget>();
		if(widget == null)
			return;
		
		this.hover.SetParameter(widget.color, this.targetTweenColor.style);
		this.pressed.SetParameter(widget.color, this.targetTweenColor.style);
		this.disabled.SetParameter(widget.color, this.targetTweenColor.style);
	}
	
	#endregion
	
	#region 複製.
	
	public XUITweenColorButtonEvent Clone()
	{
		return (XUITweenColorButtonEvent)MemberwiseClone();
	}
	
	object ICloneable.Clone()
	{
		return Clone();
	}
	
	#endregion
}
