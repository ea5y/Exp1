/// <summary>
/// XUIButtonイベント時にスプライトの制御を行うスクリプト.
/// 
/// 2014/06/11.
/// </summary>
using UnityEngine;
using System.Collections;
using System;

[System.Serializable]
public class XUISpriteButtonEvent : IXUIButtonEvent, ICloneable
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
		/// スプライトの各パラメターをセットする.
		/// </summary>
		public void SpriteSetParameter(UISprite sprite)
		{
			color = sprite.color;
		}
	}
	
	#endregion
	
	#region フィールド&プロパティ
	
	[SerializeField]
	private UISprite targetSprite;
	public UISprite TargetSprite { get { return targetSprite; } }

	public EventData normal;
	
	public EventData hover;
	
	public EventData pressed;
	
	public EventData disabled;
	
	#endregion
	
	#region XUIButtonイベント.
	
	public void OnNormal(XUIButton button, bool immediate)
	{
		SetSpriteData(this.normal, button, immediate);
	}
	
	public void OnHover(XUIButton button, bool immediate)
	{
		SetSpriteData(this.hover, button, immediate);
	}
	
	public void OnPressed(XUIButton button, bool immediate)
	{
		SetSpriteData(this.pressed, button, immediate);
	}
	
	public void OnDisabled(XUIButton button, bool immediate)
	{
		SetSpriteData(this.disabled, button, immediate);
	}
	
	#endregion
	
	#region スプライトに各データをセット.
	
	private void SetSpriteData(EventData eventData, XUIButton button, bool immediate)
	{
		if(!eventData.isEnable || this.targetSprite == null)
		{
			return;
		}
		
		// 色セット.
		TweenColor tc = TweenColor.Begin(this.targetSprite.gameObject, button.duration, eventData.color);
		if(immediate && tc != null)
		{
			tc.value = tc.to;
			tc.enabled = false;
		}
	}
	
	#endregion
	
	#region Inspectorメニュー
	
	/// <summary>
	/// スプライトのパラメータをセットする.
	/// </summary>
	public void SpriteCopyParameters()
	{
		if(this.targetSprite == null)
			return;
		
		this.normal.SpriteSetParameter(this.targetSprite);
		this.hover.SpriteSetParameter(this.targetSprite);
		this.pressed.SpriteSetParameter(this.targetSprite);
		this.disabled.SpriteSetParameter(this.targetSprite);
	}
	
	#endregion
	
	#region 複製.
	
	public XUISpriteButtonEvent Clone()
	{
		return (XUISpriteButtonEvent)MemberwiseClone();
	}
	
	object ICloneable.Clone()
	{
		return Clone();
	}
	
	#endregion

}
