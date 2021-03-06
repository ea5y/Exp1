/// <summary>
/// XUIButtonイベント時にラベルの制御を行うスクリプト.
/// 
/// 2014/06/10.
/// </summary>
using UnityEngine;
using System.Collections;
using System;

[System.Serializable]
public class XUILabelButtonEvent : IXUIButtonEvent, ICloneable
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
		/// 色.
		/// </summary>
		public Color color;
		
		/// <summary>
		/// エフェクト.
		/// </summary>
		public UILabel.Effect effect;

		/// <summary>
		/// エフェクトの色
		/// </summary>
		public Color effectColor;
		
		/// <summary>
		/// ラベルの各パラメターをセットする.
		/// </summary>
		public void LabelSetParameter(UILabel label)
		{
			color = label.color;
			effect = label.effectStyle;
			effectColor = label.effectColor;
		}
	}
	
	#endregion
	
	#region フィールド&プロパティ
	
	[SerializeField]
	private UILabel targetLabel;
	public UILabel TargetLabel { get { return this.targetLabel; } }

	public EventData normal;
	
	public EventData hover;
	
	public EventData pressed;
	
	public EventData disabled;
	
	#endregion
	
	#region XUIButtonイベント.
	
	public void OnNormal(XUIButton button, bool immediate)
	{
		SetLabelData(this.normal, button, immediate);
	}
	
	public void OnHover(XUIButton button, bool immediate)
	{
		SetLabelData(this.hover, button, immediate);
	}
	
	public void OnPressed(XUIButton button, bool immediate)
	{
		SetLabelData(this.pressed, button, immediate);
	}
	
	public void OnDisabled(XUIButton button, bool immediate)
	{
		SetLabelData(this.disabled, button, immediate);
	}
	
	#endregion
	
	#region ラベルに各データをセット.
	
	private void SetLabelData(EventData eventData, XUIButton button, bool immediate)
	{
		if(!eventData.isEnable || this.targetLabel == null)
		{
			return;
		}
		
		this.targetLabel.effectStyle = eventData.effect;
		
		// 色セット.
		TweenColor tc = TweenColor.Begin(this.targetLabel.gameObject, button.duration, eventData.color);
		if(immediate && tc != null)
		{
			tc.value = tc.to;
			tc.enabled = false;
		}

		// エフェクトの色セット
		this.targetLabel.effectColor = eventData.effectColor;
	}
	
	#endregion
	
	#region Inspectorメニュー
	
	/// <summary>
	/// ラベルのパラメータをセットする.
	/// </summary>
	public void LabelCopyParameters()
	{
		if(this.targetLabel == null)
			return;
		
		this.normal.LabelSetParameter(this.targetLabel);
		this.hover.LabelSetParameter(this.targetLabel);
		this.pressed.LabelSetParameter(this.targetLabel);
		this.disabled.LabelSetParameter(this.targetLabel);
	}
	
	#endregion
	
	#region 複製.
	
	public XUILabelButtonEvent Clone()
	{
		return (XUILabelButtonEvent)MemberwiseClone();
	}
	
	object ICloneable.Clone()
	{
		return Clone();
	}
	
	#endregion
}
