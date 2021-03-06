/// <summary>
/// プレイヤー選択パケット要求クラス
/// 
/// 2015/12/18
/// </summary>
using System;
using System.Collections;
using Scm.Common.Packet;
using Scm.Common.GameParameter;

/// <summary>
/// Responseイベント引数
/// </summary>
public class SelectPlayerEventArgs : EventArgs
{
	private bool result;
	public bool Result { get { return result; } }

	public SelectPlayerEventArgs(bool result)
	{
		this.result = result;
	}
}

/// <summary>
/// プレイヤー選択リクエストインターフェイス
/// </summary>
public interface ISelectPlayerRequest
{
	/// <summary>
	/// 送信
	/// </summary>
	void Send(int playerId);

	/// <summary>
	/// 応答イベント
	/// </summary>
	event EventHandler<SelectPlayerEventArgs> ResponseEvent;
}

/// <summary>
/// プレイヤー選択パケット要求クラス
/// </summary>
public class SelectPlayerRequest : ISelectPlayerRequest, IPacketResponse<SelectPlayerRes>
{
	#region フィールド&プロパティ
	/// <summary>
	/// 応答イベント
	/// </summary>
	public event EventHandler<SelectPlayerEventArgs> ResponseEvent = (sender, e) => { };
	#endregion

	#region 送信
	/// <summary>
	/// 送信処理
	/// </summary>
	/// <param name="playerId"></param>
	public void Send(int playerId)
	{
		NetworkController.SendSelectPlayer(playerId, this);
	}
	#endregion

	#region IPacketResponse
	/// <summary>
	/// サーバからの応答
	/// </summary>
	/// <param name="packet"></param>
	public void Response(SelectPlayerRes packet)
	{
		var eventArgs = new SelectPlayerEventArgs(packet.Result);
		ResponseEvent(this, eventArgs);
	}
	#endregion
}
