/// <summary>
/// 画面上にデバッグメッセージを表示する.Editor専用.
/// 
/// 2013/01/18
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GUIDebugMessage : MonoBehaviour
{
	#region インスペクタ用フィールド.
	/// <summary>
	/// 最大表示行数.
	/// </summary>
	public int limitLines = 5;
	/// <summary>
	/// メッセージ当たりの表示時間.
	/// </summary>
	public float time = 2.0f;
	/// <summary>
	/// 最大表示行数を超えた時の表示時間.
	/// </summary>
	public float timeWhenLimitOver = 1.0f;
	#endregion

#if UNITY_EDITOR

	#region privateフィールド.
	static private GUIDebugMessage instance;
	private float dequeueCount;
	private Queue<string> drawMsgQueue;
	#endregion

	#region MonoBehaviour Messages.
	void Awake()
	{
		instance = this;
		this.drawMsgQueue = new Queue<string>();
	}
	void Update()
	{
		if (0 < this.drawMsgQueue.Count)
		{
			// カウント毎に一行ずつ消していく
			this.dequeueCount -= Time.deltaTime;
			if (this.dequeueCount <= 0f)
			{
				this.RemoveMsg();
			}
		}
		else
		{
			this.dequeueCount = this.time;
		}
	}
	#endregion

	#region 画面メッセージ
	public static void AddMsg(string text)
	{
		if (instance != null)
		{
			instance._AddMsg(text);
		}
	}
	private void _AddMsg(string text)
	{
		// テキスト設定
		string[] textArray = text.Split(new string[] { "\r", "\n", "\r\n" }, 3, System.StringSplitOptions.None);
		foreach (string s in textArray)
		{
			this.drawMsgQueue.Enqueue(s);
		}

		float dTime = this.limitLines < this.drawMsgQueue.Count ? this.timeWhenLimitOver : this.time;
		if (dTime < this.dequeueCount)
		{
			this.dequeueCount = dTime;
		}

		this.DrawMsg();
	}
	private void RemoveMsg()
	{
		this.drawMsgQueue.Dequeue();

		this.dequeueCount = this.limitLines < this.drawMsgQueue.Count ? this.timeWhenLimitOver : this.time;

		this.DrawMsg();
	}
	private void DrawMsg()
	{
		if (this.gameObject.GetComponent<GUIText>() != null)
		{
			// 表示メッセージ設定
			string drawText = string.Empty;
#if XW_DEBUG
			if (DebugKeyCommand.Instance == null ? ScmParam.Debug.File.IsDrawFPS : DebugKeyCommand.DrawFPS)
#endif
			{
				int count = 0;
				foreach (string text in this.drawMsgQueue)
				{
					if (this.limitLines <= count)
					{
						break;
					}
					drawText += string.Format("{0}_{1}\r\n", this.drawMsgQueue.Count - count, text);
					count++;
				}
			}
			this.gameObject.GetComponent<GUIText>().text = drawText;
		}
	}
	#endregion
#else
	[System.Diagnostics.Conditional("UNITY_EDITOR")]
	public static void AddMsg(string text) { }
#endif
}
