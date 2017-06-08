using UnityEngine;
using System.Collections;
using Scm.Common.Packet;
using System.Collections.Generic;
using System;

public class DungeonController : Singleton<DungeonController> {
    #region View
    [SerializeField]
    private Dungeon dungeon;
    public Dungeon View { get { return dungeon; } }

    [System.Serializable]
    public class Dungeon
    {
        public GameObject root;
        public UIButton btnClose;

        public UILabel energy;

        public UIButton dungeonNormal;
        public UIButton dungeonDiffcult;
    }


    #endregion
    
    private int currentEnergy = 0;
    private int maxEnergy = 0;
    private List<DungeonParameter> dungeonList = new List<DungeonParameter>();

    private bool isMatching = false;
    private bool IsMatching {
        get { return this.isMatching; }
        set
        {
            this.isMatching = value;
            this.Goto();
        }
    }

    private void Goto()
    {
        this.View.root.SetActive(!this.IsMatching);
    }

    void Awake()
    {
        base.Awake();
        this.View.root.SetActive(false);
    }

    public void Open()
    {
        Net.Network.Instance.StartCoroutine(this.Init());
        
    }

    public IEnumerator Init()
    {
        GUIMatchingState.Instance.SetCancelCallback(() => {
            this.IsMatching = false;
        });

        yield return Net.Network.GetDungeonInfo(
            (res) =>
            {
                this.currentEnergy = res.Energy;
                this.maxEnergy = res.MaxEnergy;
                var dungeonList = res.GetDungeonParameters();
                for (int i = 0; i < dungeonList.Length; i++)
                {
                    this.dungeonList.Add(dungeonList[i]);
                }

                this.SetView();
            });

        //PanelManager.Instance.Open(this.View.root);
        this.View.root.SetActive(true);
    }

    private void SetView()
    {
        this.SetEnergy();
        this.SetDungeonItem();
    }

    private void SetDungeonItem()
    {
        var itemNormal = this.View.dungeonNormal.GetComponent<DungeonItem>();
        itemNormal.needEnergy.text = this.dungeonList[0].CostEnergy + "";

        var itemDiffcult = this.View.dungeonDiffcult.GetComponent<DungeonItem>();
        itemDiffcult.needEnergy.text = this.dungeonList[1].CostEnergy + "";
    }

    private void SetEnergy()
    {
        this.View.energy.text = this.currentEnergy + " / " + this.maxEnergy;
    }
    
    public void Close()
    {
        this.View.root.SetActive(false);
    }

    public void OnDungeonNormalClick()
    {
        if(this.dungeonList[0].DeniedTeamMemberCount > 0)
        {
            GUITipMessage.Instance.Show("队伍中有人体力不足！");
        }
        else
        {
            LobbyPacket.SendMatchingEntry((BattleFieldType)this.dungeonList[0].FieldId, Scm.Common.GameParameter.ScoreType.QuickMatching);
            this.IsMatching = true;
        }
    }

    public void OnDungeonDiffcultClick()
    {
        if (this.dungeonList[1].DeniedTeamMemberCount > 0)
        {
            GUITipMessage.Instance.Show("队伍中有人体力不足！");
        }
        else
        {
            LobbyPacket.SendMatchingEntry((BattleFieldType)this.dungeonList[1].FieldId, Scm.Common.GameParameter.ScoreType.QuickMatching);
            this.IsMatching = true;
        }
    }
}
