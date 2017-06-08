using UnityEngine;
using System.Collections;
using Scm.Common.Packet;
using Scm.Common.GameParameter;

public class GUIAnimeRankIn : Singleton<GUIAnimeRankIn> {
    #region View
    [SerializeField]
    RankIn view;
    public RankIn View { get { return this.view; } set { this.view = value; } }

    [System.Serializable]
    public class RankIn
    {
        public GameObject root;
        public UIGrid gridEnemy;
        public UIGrid gridTeam;

        public GameObject itemPlayer;
    }
    #endregion

    #region Controller
    CharaIcon CharaIcon { get { return ScmParam.Lobby.CharaIcon; } }

    void Awake()
    {
        base.Awake();
        this.View.root.SetActive(false);
    }

    public void Show(EnterFieldRes res)
    {
        this.Setup(res);
        PanelManager.Instance.Open(this.View.root);
    }

    public void Close()
    {
        PanelManager.Instance.Close(this.View.root);
    }
    
    public void Setup(EnterFieldRes res)
    {
        var info = res.GetEntrantAll();

        var self = res.GetEntrantRes();

        TeamType myTeam = self.TeamType;

        Debug.Log("myTeam: " + myTeam);
        
        this.View.gridTeam.gameObject.DestroyChild();
        this.View.gridEnemy.gameObject.DestroyChild();
        for (int i = 0; i < info.Length; i++)
        {
            if(info[i].EntrantType == Scm.Common.GameParameter.EntrantType.Pc)
            {
                Debug.Log("Pc: " + info[i].EntrantType);
                if(info[i].TeamType == myTeam)
                {
                    Debug.Log("AddMyTeam");
                    this.AddPlayer(this.View.gridTeam, info[i].UserName, (AvatarType)info[i].Id, info[i].SkinId);
                }else
                {
                    this.AddPlayer(this.View.gridEnemy, info[i].UserName, (AvatarType)info[i].Id, info[i].SkinId);
                }
            }
        }
    }

    TeamType GetMyTeam(EntrantRes[] info)
    {
        int id = NetworkController.ServerValue.InFieldId;
        Debug.Log("MyInFielId: " + id);
        foreach(var v in info)
        {
            if(v.EntrantType == Scm.Common.GameParameter.EntrantType.Pc)
            {
                Debug.Log("InFielId: " + v.InFieldId);
                if (v.InFieldId == id)
                    return (TeamType)v.TeamType;
            }
            
        }
        
        return 0;
    }

    void AddPlayer(UIGrid grid, string playerName, AvatarType avatarType, int skinId)
    {
        GameObject go = NGUITools.AddChild(grid.gameObject, this.View.itemPlayer);
        var item = go.GetComponent<ItemRankPlayer>();

        item.username.text = playerName;
        CharaIcon.GetIcon(avatarType, skinId, false,
                    (UIAtlas res, string name) =>
                    {
                        item.icon.atlas = res;
                        item.icon.spriteName = name;
                    });
    }
    #endregion
}
