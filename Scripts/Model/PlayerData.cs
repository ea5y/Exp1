using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Scm.Common.Packet;

/// <summary>
/// LWZ:For system display
/// </summary>
namespace XDATA
{
    public class PlayerData
    {
        /// <summary>
        /// Singleton
        /// </summary>
        private static PlayerData _instance;
        public static PlayerData Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new PlayerData();
                return _instance;
            }
        }

        #region Gold and Coin
        public event EventHandler OnGoldChange = (s, e) => { };
        private int gold = -1;
        public int Gold
        {
            get
            {
                if (this.gold == -1)
                    this.gold = NetworkController.ServerValue.PlayerInfo.Gold;
                return this.gold;
            }
            set
            {
                this.gold = value;
                Debug.Log("Gold change");
                this.OnGoldChange(this, EventArgs.Empty);
            }
        }

        public event EventHandler OnCoinChange = (s, e) => { };
        private int coin = 0;
        public int Coin
        {
            get
            {
                return this.coin;
            }
            set
            {
                if (value < 0) return;
                this.coin = value;
                Debug.Log("Coin change");
                this.OnCoinChange(this, EventArgs.Empty);
            }
        }
        #endregion

        #region Energy
        public event EventHandler OnEnergyChange = (s, e) => {};
        public int MaxEnergy{get; private set;}
        private int energy = 0;
        public int Energy
        {
            get
            {
                return this.energy;
            }
            set
            {
                this.energy = value;
                this.OnEnergyChange(this, EventArgs.Empty);
            }
        }
 
        public IEnumerator GetEnergy()
        {
            yield return Net.Network.GetDungeonInfo((res)=>{
                    if(res == null)
                    {
                        Debug.LogError("DataEnergy == null");
                        return;
                    }
                    this.MaxEnergy = res.MaxEnergy;
                    this.Energy = res.Energy;
                    });
        }
        #endregion

        #region TaskList
        public event EventHandler OnTasksChange = (s, e) => { };
        public List<TaskDaily> taskDailyList;

        public IEnumerator GetTasks()
        {
            yield return Net.Network.GetTaskDailyInfo((res) => {
                var tasks = res.GetQuestList();
                var list = new List<TaskDaily>();
                foreach (var t in tasks)
                {
                    list.Add(new TaskDaily(t));
                }

                this.taskDailyList = list;

                this.OnTasksChange(this, EventArgs.Empty);
            });
        }
        #endregion
        
        #region CharaList
        public event EventHandler OnCharaListChange = (s, e) => { };
        public List<Chara> CharaList { get; private set; }
        public int CharaBoxCapacity { get; private set; }
        public IEnumerator GetCharaListTest(List<Chara> charaList = null, bool isFirst = true)
        {
            if (charaList != null)
            {
                this.CharaList = charaList;
                this.OnCharaListChange(this, EventArgs.Empty);
                yield break;
            }
                        
            if (isFirst)
            {
                yield return Net.Network.GetCharaList((res) =>
                {
                    var list = new List<Chara>();
                    this.CharaBoxCapacity = res.CharacterBoxCapacity;
                    foreach (var t in res.GetPlayerCharacterPackets())
                    {
                        if (!Scm.Common.Master.CharaMaster.Instance.IsValidCharacter(t.CharacterMasterId))
                        {
                            continue;
                        }

                        var charaInfo = new CharaInfo(t, (ulong)res.SymbolPlayerCharacterUuid);
                        list.Add(new Chara(charaInfo));
                    }
                    //Assign
                    this.CharaList = list;

                    this.OnCharaListChange(this, EventArgs.Empty);
                });
                yield break;
            }

            if (this.CharaList != null)
            {
                this.OnCharaListChange(this, EventArgs.Empty);
            }
        }
        #endregion

        #region DeckInfo
        public event EventHandler OnDeckInfoChange = (s, e) => { };
        public DeckInfo deckInfo;
        public IEnumerator GetDeckInfo()
        {
            bool isCompleted = false;
            yield return Net.Network.GetDeckNum((res)=> {
                Net.Network.Instance.StartCoroutine(Net.Network.GetDeckInfo(res.CurrentDeckId, (r) => {
                    var deckInfo = new DeckInfo(r);
                    this.deckInfo = deckInfo;
                    this.OnDeckInfoChange(this, EventArgs.Empty);
                    isCompleted = true;
                }));
            });

            while (!isCompleted)
            {
                yield return null;
            }
        }
        #endregion

        #region FriendsInfo
        public event EventHandler OnFriendsListChange = (s, e) => {};
        public List<FriendParameter> friendsList;
        public IEnumerator GetFriendsList()
        {
            yield return Net.Network.GetFriendsList((res) => {
                    this.friendsList = new List<FriendParameter>();
                    var temp = res.GetFriendListParameters();
                    foreach(var friend in temp)
                    {
                        this.friendsList.Add(friend);
                    }
                    this.OnFriendsListChange(this, EventArgs.Empty);
                    });
        }

        public event EventHandler OnApplyListChange = (s, e) => {};
        public List<FriendRequestParameter> applyList;
        public IEnumerator GetApplyList()
        {
            yield return Net.Network.GetApplyList((res) => {
                    this.applyList = new List<FriendRequestParameter>();
                    var temp = res.GetRequestListParameters();
                    foreach(var apply in temp)
                    {
                        this.applyList.Add(apply);
                    }
                    this.OnApplyListChange(this, EventArgs.Empty);
                    }, false);
        }
        #endregion
    }

}
