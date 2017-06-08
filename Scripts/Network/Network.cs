//author: luwanzhong
//date: 2016-11-18
//desc: net controller new

using UnityEngine;
using System.Collections;
using Scm.Common.Packet;
using Scm.Client;
using System.Collections.Generic;
using System;
using Asobimo.Photon.Packet;
using System.Threading;
using Scm.Common.PacketCode;
using Scm.Common.GameParameter;
using Scm.Common;
using Scm.Common.Master;

namespace Net
{
   
    /// <summary>
    /// Send req and excute callback
    /// Include show loadingIcon and Error, Timeout deal.
    /// </summary>
    public class Network : Singleton<Network>
    {
        #region Req
        //
        public static bool loading = false;

        public IEnumerator AddReq<T>(PacketBase packet, int timeOutSecs, Action<T> callback, bool isShowLoadingIcon = true) where T : PacketBase, new()
        {
            yield return AddReqTest(SendN(packet, timeOutSecs, callback, isShowLoadingIcon));
        }

        public static List<IEnumerator> reqList = new List<IEnumerator>();
        public static int reqCount = 0;

        public static Queue<IEnumerator> reqQueue = new Queue<IEnumerator>();
        private bool isReqing = false;

        public IEnumerator Req()
        {
            if (this.isReqing == false)
            {
                while (true)
                {
                    if (reqQueue.Count == 0)
                        break;
                    if (this.isReqing == false)
                        this.isReqing = true;
                    yield return StartCoroutine(reqQueue.Dequeue());
                }
                this.isReqing = false;
            }
        }

        void AddReq(IEnumerator req)
        {
            Network.reqQueue.Enqueue(req);
            
            StartCoroutine( this.Req());
        }

        IEnumerator AddReqTest(IEnumerator req)
        {
            Network.reqQueue.Enqueue(req);
            yield return this.Req();
        }

        void StartReq()
        {
            if(this.isReqing == false)
                StartCoroutine(this.Req());
        }

        static IEnumerator SendN<T>(PacketBase packet, int timeOutSecs, Action<T> callback, bool isShowLoadingIcon = true) where T : PacketBase, new()
        {
            // Show loading icon
            if(isShowLoadingIcon)
            {
                Debug.Log("show loading icon");
                Network.loading = true;
                LoadingIconController.Instance.Show();
            }

            GameListener.SendDirect<T>(packet, 10, (code, res, returnCode) =>
            {
                NetworkController.InvokeAsync(
                    () => 
                    {
                        Debug.Log("Return packet...");
                        switch (code)
                        {
                            case ResponseResultCode.Error:
                                Network.Instance.Error();
                                break;
                            case ResponseResultCode.Timeout:
                                Network.Instance.Timeout();
                                break;
                            case ResponseResultCode.Success:
                                Debug.Log("Req: success!");
								Debug.Log("returncode:" + returnCode);
								if (callback != null && returnCode == ReturnCode.Ok)
                                    callback.Invoke(res);

                                NetErrorMessage.Show((RequestCode)res.Code, res, returnCode);
                                break;
                        }
                        Net.Network.loading = false;
                    });
            });

            //Hide loading icon
            if (isShowLoadingIcon)
            {
                while (true)
                {
                    if (Net.Network.loading == false)
                    {
                        Debug.Log("hide loading icon");
                        if(Net.Network.reqQueue.Count == 0)
                            LoadingIconController.Instance.Hide();
                        break;
                    }
                    yield return null;
                }
            }
        }

        private void Error()
        {
            GUIMessageWindow.SetModeOK("请求错误！", null);
            
            Debug.Log("Req: Error!");
        }

        private void Timeout()
        {
            GUIMessageWindow.SetModeOK("请求超时！", null);
            Debug.Log("Req: TimeOut!");
        }
        #endregion


        #region User Info
        public static IEnumerator GetPlayerStatusInfo(int inFieldID, Action<PlayerStatusRes> callback)
        {
            var pakect = new PlayerStatusReq();
            pakect.InFieldId = (short)inFieldID;
            yield return Network.Instance.AddReq<PlayerStatusRes>(pakect, 10, callback);
        }

        public static IEnumerator GetCombatGiansBaseInfo(Action<BattleHistoryRes> callback, bool isShowLoadingIcon)
        {
            var packet = new BattleHistoryReq();
            yield return Network.Instance.AddReq<BattleHistoryRes>(packet, 10, callback, isShowLoadingIcon);
        }

        public static IEnumerator GetCombatGiansOneGameInfo(long battleId, Action<BattleHistoryDetailRes> callback)
        {
            var packet = new BattleHistoryDetailReq();
            packet.BattleID = battleId;
            yield return Network.Instance.AddReq<BattleHistoryDetailRes>(packet, 10, callback);
        }

        public static IEnumerator GetCombatGiansPlayerInfo(long playerId, Action<PlayerMiscInfoRes> callback)
        {
            var p = new PlayerMiscInfoReq();
            p.PlayerId = playerId;
            yield return Network.Instance.AddReq<PlayerMiscInfoRes>(p, 10, callback);
        }

        public static IEnumerator GetPlayerMatchingInfo(long playerId, Action<PlayerMiscInfoRes> callback, bool isShowLoadingIcon)
        {
            var p = new PlayerMiscInfoReq();
            p.PlayerId = playerId;
            yield return Network.Instance.AddReq<PlayerMiscInfoRes>(p, 10, callback, isShowLoadingIcon);
        }
        #endregion

        #region Friends
        public static IEnumerator AddFriend(long playerId, Action<FriendRes> callback, bool isShowLoadingIcon)
        {
            var p = new FriendReq();
            p.SetAddFriendParameter(playerId);
            yield return Network.Instance.AddReq<FriendRes>(p, 10, callback, isShowLoadingIcon);
        }

		public static IEnumerator SearchFriend(string playerName, Action<SearchPlayerRes> callback, bool isShowLoadingIcon)
		{
			var p = new SearchPlayerReq ();
			p.PlayerName = playerName;
			yield return Network.Instance.AddReq<SearchPlayerRes> (p, 10, callback, isShowLoadingIcon);
		}	

        public static IEnumerator GetFriendsList(Action<FriendRes> callback)
        {
            var p = new FriendReq();
            p.SetGetFriendListParameter();
            yield return Network.Instance.AddReq<FriendRes>(p, 10, callback);
        }

        public static IEnumerator GetApplyList(Action<FriendRes> callback, bool isShowLoadingIcon)
        {
            var p = new FriendReq();
            p.SetGetRequestListParameter();
            yield return Network.Instance.AddReq<FriendRes>(p, 10, callback, isShowLoadingIcon);
        }

        public static IEnumerator AcceptFriendRequest(long reqId, Action<FriendRes> callback)
        {
            var p = new FriendReq();
            p.SetAcceptParameter(reqId);
            yield return Network.Instance.AddReq<FriendRes>(p, 10, callback);
        }

        public static IEnumerator RejectFriendRequest(long reqId, Action<FriendRes> callback)
        {
            var p = new FriendReq();
            p.SetRejectParameter(reqId);
            yield return Network.Instance.AddReq<FriendRes>(p, 10, callback);
        }
        #endregion

        #region RankIn
        public static IEnumerator GetRankInInfo(Action<EnterFieldRes> callback, bool isShowLoadingIcon)
        {
            var p = new EnterFieldReq();
            yield return Network.Instance.AddReq<EnterFieldRes>(p, 10, callback, isShowLoadingIcon);
        }
        #endregion
        
        #region Grow
        public static IEnumerator UpgradeGrow(ulong baseCharaUUID, ulong[] baitCharaUUIDs, Action<PowerupRes> callback)
        {
            var p = new PowerupReq();
            p.BasePlayerCharacterUuid = (long)baseCharaUUID;
            p.BaitPlayerCahracterUuids = baitCharaUUIDs.ToLongArray();
            yield return Network.Instance.AddReq<PowerupRes>(p, 10, callback);
        }

        public static IEnumerator EvolveGrow(ulong baseCharaUUID, ulong[] baitCharaUUIDs, Action<EvolutionRes> callback)
        {
            var p = new EvolutionReq();
            p.BasePlayerCharacterUuid = (long)baseCharaUUID;
            p.BaitPlayerCharacterUuids = baitCharaUUIDs.ToLongArray();
            yield return Network.Instance.AddReq<EvolutionRes>(p, 10, callback);
        }
        #endregion

        //CharaList
        public static IEnumerator GetCharaList(Action<PlayerCharacterAllRes> callback)
        {
            var p = new PlayerCharacterAllReq();
            yield return Network.Instance.AddReq<PlayerCharacterAllRes>(p, 10, callback);
        }

        //DungeonLInfo
        public static IEnumerator GetDungeonInfo(Action<DungeonListRes> callback)
        {
            var p = new DungeonListReq();
            yield return Network.Instance.AddReq<DungeonListRes>(p, 10, callback);
        }

        #region LoginAwardList
        public static IEnumerator GetLoginAwardInfo(Action<LoginBonusRes> callback)
        {
            var p = new LoginBonusReq();
            yield return Network.Instance.AddReq<LoginBonusRes>(p, 10, callback);
        }

        public static IEnumerator GetLoginAward(Action<LoginBonusRewardRes> callback)
        {
            var p = new LoginBonusRewardReq();
            yield return Network.Instance.AddReq<LoginBonusRewardRes>(p, 10, callback, false);
        }
        #endregion

        public static IEnumerator GetSkinInfo(long uuid, Action<GetCharacterAvatarAllRes> callback)
        {
            var p = new GetCharacterAvatarAllReq();
            p.CharacterUuid = uuid;
            yield return Network.Instance.AddReq<GetCharacterAvatarAllRes>(p, 10, callback);
        }

        public static IEnumerator ChangeSkin(long uuid, int avartId, Action<SetCurrentAvatarRes> callback)
        {
            var p = new SetCurrentAvatarReq();
            p.CharacterUuid = uuid;
            p.AvatarId = avartId;
            yield return Network.Instance.AddReq<SetCurrentAvatarRes>(p, 10, callback);
        }

        public static IEnumerator GetTaskDailyInfo(Action<QuestRes> callback)
        {
            var p = new QuestReq();
            p.Command = QuestReq.COMMAND_LIST;
            p.QuestCategory = QuestCategory.Daily;
            yield return Network.Instance.AddReq<QuestRes>(p, 10, callback);
        }

        public static IEnumerator GetTaskReward(int id, Action<QuestRes> callback)
        {
            var p = new QuestReq();
            p.Command = QuestReq.COMMAND_CONFIRM;
            p.QuestId = id;
            yield return Network.Instance.AddReq<QuestRes>(p, 10, callback);
        }

        public static IEnumerator LockChara(ulong uuid, bool isLock, Action<SetLockPlayerCharacterRes> callback)
        {
            var p = new SetLockPlayerCharacterReq();
            p.PlayerCharacterUuid = (long)uuid;
            p.LockFlag = isLock;
            yield return Network.Instance.AddReq<SetLockPlayerCharacterRes>(p, 10, callback);
        }

        public static IEnumerator GetWallpaperInfo(long uuid, Action<GetCharacterWallpaperAllRes> callback)
        {
            var p = new GetCharacterWallpaperAllReq();
            p.CharacterUuid = uuid;
            yield return Network.Instance.AddReq<GetCharacterWallpaperAllRes>(p, 10, callback);
        }

        public static IEnumerator GetDeckNum(Action<CharacterDeckNumRes> callback)
        {
            var p = new CharacterDeckNumReq();
            yield return Network.Instance.AddReq<CharacterDeckNumRes>(p, 10, callback);
        }

        public static IEnumerator GetDeckInfo(int deckId, Action<CharacterDeckRes> callback)
        {
            var p = new CharacterDeckReq();
            p.DeckId = deckId;
            yield return Network.Instance.AddReq<CharacterDeckRes>(p, 10, callback);
        }
        
        public static IEnumerator AfterBuy(Action<ReceiveWebStoreRes> callback)
        {
            var p = new ReceiveWebStoreReq();
            yield return Network.Instance.AddReq<ReceiveWebStoreRes>(p, 10, callback);
        }

        public static IEnumerator SellCharacter(ulong[] uuids, Action<SellMultiPlayerCharacterRes> callback)
        {
            var p = new SellMultiPlayerCharacterReq();
            p.PlayerCharacterUuids = uuids.ToLongArray();
            yield return Network.Instance.AddReq<SellMultiPlayerCharacterRes>(p, 10, callback);
        }

        public static IEnumerator GetGift(string code, Action<GiftPackageRes> callback)
        {
            var p = new GiftPackageReq();
            p.GiftCode = code;
            yield return Network.Instance.AddReq<GiftPackageRes>(p, 10, callback);
        }

        public static IEnumerator GetClashAvatarWithShopProductPF(int[] avatarIds, Action<ObtainedCharacterAvatarAllRes> callback)
        {
            var p = new ObtainedCharacterAvatarAllReq();
            p.AvatarIds = avatarIds;
            yield return Network.Instance.AddReq<ObtainedCharacterAvatarAllRes>(p, 10, callback);
        }
    }
}

