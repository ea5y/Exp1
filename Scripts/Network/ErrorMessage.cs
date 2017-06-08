using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Scm.Common.PacketCode;
using Scm.Common.Packet;
using Scm.Common;
using Asobimo.Photon.Packet;

public class ErrorMessage {
    public static void Show<T>(RequestCode code, T res, ReturnCode returnCode) where T : PacketBase
    {
        switch (code)
        {
		case RequestCode.FriendReq:
			switch ((res as FriendRes).Command) {
			case FriendReq.COMMAND_ADD_FRIEND:
				switch (returnCode) {
				case ReturnCode.InvalidOperation:
					GUIMessageWindow.SetModeOK ("尝试加自己或是已在好友列表的玩家为好友！", null);
					break;
				case ReturnCode.ColdDown:
					GUIMessageWindow.SetModeOK ("上次提交的请求仍在冷却时间中！", null);
					break;
				case ReturnCode.OperationInProgress:
					GUIMessageWindow.SetModeOK ("正在处理中！", null);
					break;
				}
				break;
			case FriendReq.COMMAND_ACCEPT_REQUEST:
				switch (returnCode) {
				case ReturnCode.NotFound:
					GUIMessageWindow.SetModeOK ("请求ID不存在！", null);
					break;
				case ReturnCode.InvalidOperation:
					GUIMessageWindow.SetModeOK ("请求已处理！", null);
					break;
				case ReturnCode.Expired:
					GUIMessageWindow.SetModeOK ("请求超时！", null);
					break;
				case ReturnCode.Overflow:
					GUIMessageWindow.SetModeOK ("自己或目标的好友数量超过上限80个！", null);
					break;
				case ReturnCode.Database:
					GUIMessageWindow.SetModeOK ("数据库出错！", null);
					break;
				}
				break;
			case FriendReq.COMMAND_REJECT_REQUEST:
				switch (returnCode) {
				case ReturnCode.NotFound:
					GUIMessageWindow.SetModeOK ("请求ID不存在！", null);
					break;
				case ReturnCode.InvalidOperation:
					GUIMessageWindow.SetModeOK ("请求已处理！", null);
					break;
				case ReturnCode.Expired:
					GUIMessageWindow.SetModeOK ("请求已超时！", null);
					break;
				}
				break;
			case FriendReq.COMMAND_DEL_FRIEND:
				switch (returnCode) {
				case ReturnCode.InvalidOperation:
					GUIMessageWindow.SetModeOK ("非好友关系！", null);
					break;
				}
				break;
			case FriendReq.COMMAND_IGNORE_REQUEST:
				switch (returnCode) {
				case ReturnCode.NotFound:
					GUIMessageWindow.SetModeOK ("请求ID不存在！", null);
					break;
				case ReturnCode.InvalidOperation:
					GUIMessageWindow.SetModeOK ("请求已处理！", null);
					break;
				}
				break;
			}
			break;
		case RequestCode.SearchPlayer:
			switch (returnCode) {
			case ReturnCode.Fatal:
				GUIMessageWindow.SetModeOK ("玩家不在线！", null);
				break;
			case ReturnCode.NotFound:
				GUIMessageWindow.SetModeOK ("找不到该玩家！", null);
				break;
			}
			break;
        }
    }
}
