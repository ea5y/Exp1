/// <summary>
/// 再参戦パケット要求クラス
/// 
/// 2015/12/21
/// </summary>
using System;
using System.Collections;
using Scm.Common.Packet;
using Scm.Common.GameParameter;

/// <summary>
/// Responseイベント引数
/// </summary>
public class ReEnterFieldEventArgs : EventArgs
{
	ReEnterFieldResult result;
	public ReEnterFieldResult Result { get { return result; } }

	public ReEnterFieldEventArgs(ReEnterFieldResult result)
	{
		this.result = result;
	}
}

/// <summary>
/// 再参戦リクエストインターフェイス
/// </summary>
public interface IReEnterFieldRequest
{
	/// <summary>
	/// 送信
	/// </summary>
	void Send();
	
	/// <summary>
	/// 応答イベント
	/// </summary>
	event EventHandler<ReEnterFieldEventArgs> ResponseEvent;
}

/// <summary>
/// 再参戦パケット要求クラス
/// </summary>
public class ReEnterFieldRequest : IReEnterFieldRequest, IPacketResponse<ReEnterFieldRes>
{
	#region フィールド&プロパティ
	/// <summary>
	/// 応答イベント
	/// </summary>
	public event EventHandler<ReEnterFieldEventArgs> ResponseEvent = (sender, e) => { };
	#endregion

	#region 送信
	/// <summary>
	/// 送信処理
	/// </summary>
	public void Send()
	{
		BattlePacket.SendReEnterField(this);
	}
	#endregion

	#region IPacketResponse
	/// <summary>
	/// サーバからの応答
	/// </summary>
	/// <param name="packet"></param>
	public void Response(ReEnterFieldRes packet)
	{
		var eventArgs = new ReEnterFieldEventArgs(packet.ReEnterFieldResult);
		ResponseEvent(this, eventArgs);
	}
	#endregion
}
