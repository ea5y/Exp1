/// <summary>
/// サーバ情報パケット要求クラス
/// 
/// 2015/12/18
/// </summary>
using System;
using System.Collections;
using System.Collections.Generic;
using Scm.Common.Packet;
using Scm.Common.GameParameter;

/// <summary>
/// Responseイベント引数
/// </summary>
public class ServerStatusEventArgs : EventArgs
{
	private List<ServerStatusPacketParameter> serverStatusList;
	public List<ServerStatusPacketParameter> ServerStatusList { get { return serverStatusList; } }

	public ServerStatusEventArgs(List<ServerStatusPacketParameter> serverStatusList)
	{
		this.serverStatusList = serverStatusList;
	}
}

/// <summary>
/// サーバ情報リクエストインターフェイス
/// </summary>
public interface IServerStatusRequest
{
	/// <summary>
	/// 送信
	/// </summary>
	void Send();

	/// <summary>
	/// 応答イベント
	/// </summary>
	event EventHandler<ServerStatusEventArgs> ResponseEvent;
}

/// <summary>
/// サーバ情報パケット要求クラス
/// </summary>
public class ServerStatusRequest : IServerStatusRequest, IPacketResponse<ServerStatusRes>
{
	#region フィールド&プロパティ
	/// <summary>
	/// 応答イベント
	/// </summary>
	public event EventHandler<ServerStatusEventArgs> ResponseEvent = (sender, e) => { };
	#endregion

	#region 送信
	/// <summary>
	/// 送信処理
	/// </summary>
	public void Send()
	{
		NetworkController.SendServerStatus(this);
	}
	#endregion

	#region IPacketRequest
	/// <summary>
	/// サーバからの応答
	/// </summary>
	/// <param name="packet"></param>
	public void Response(ServerStatusRes packet)
	{
		var statusList = new List<ServerStatusPacketParameter>();
		foreach(var serverStatus in packet.GetServerStatusPackets())
		{
			statusList.Add(serverStatus);
		}

		var eventArgs = new ServerStatusEventArgs(statusList);
		ResponseEvent(this, eventArgs);
	}
	#endregion
}