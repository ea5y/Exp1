/// <summary>
/// レスポンスインターフェイス
/// 
/// 2015/04/22
/// </summary>
using UnityEngine;
using System.Collections;

using Asobimo.Photon.Packet;

public interface IPacketResponse<T> where T : PacketBase
{
	void Response(T packet);
}
