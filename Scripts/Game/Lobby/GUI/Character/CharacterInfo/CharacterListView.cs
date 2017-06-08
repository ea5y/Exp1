using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class CharacterListView : MonoBehaviour {
    public GameObject body;
    public CharacterInfoView characterInfoView;
    public GridView gridView;

    CharacterListModel _characterListModel;

    public CharacterListModel characterListModel
    {
        get { return _characterListModel; }
        set { _characterListModel = value; }
    }
    CharaInfo _charaInfo;

    public CharaInfo charaInfo
    {
        get { return _charaInfo; }
        set { _charaInfo = value; }
    }

    [Serializable]
    public class CharacterInfoView
    {
        public Transform transOn;
        public UISprite icon;
        public UILabel level;
        public UILabel name;
        public UILabel description;
        public UIButton buttonSelect;
        public UISprite[] pages;
        public UISprite[] stars;
    }

    [System.Serializable]
    public class GridView
    {
        public GameObject parent;
        public GameObject goPrefab = null;
        public GameObject goUIGrid = null;
        public GameObject goPage = null;
        public UIGrid uiGride = null;
        public UIScrollBar uiScrollBar = null;
        public UIScrollView uiScrollView = null;
        public UICenterOnChild uiCenterOnChild = null;
    }

    const int MaxItemNum = 6;
    CharaIcon charaIcon { get { return ScmParam.Lobby.CharaIcon; } }
    CharaBoard CharaBoard { get { return ScmParam.Lobby.CharaBoard; } }
    /// <summary>
    /// 查看角色详情
    /// </summary>
    public event EventHandler OnSelectCharaInfo = (sender, e) => { };
	// Use this for initialization
	void Start () {
        EventDelegate.Add(characterInfoView.buttonSelect.onClick, OnSelect);
	}

    public void Setup(CharacterListModel characterListModel)
    {
        this.characterListModel = characterListModel;
        this.charaInfo = characterListModel.charaInfo;
        SetCharacterListView();
        SetPage(0);
    }

    void SetCharacterListView()
    {
        gridView.uiGride.transform.DestroyChildren();
        List<List<CharaInfo>> charaInfoList = new List<List<CharaInfo>>();
        List<CharaInfo> charaInfo = new List<CharaInfo>();
        for (int i = 0; i < characterListModel.charaInfoList.Count; i++)
        {
            if (i % MaxItemNum == 0)
            {
                charaInfo = new List<CharaInfo>();
            }
            charaInfo.Add(characterListModel.charaInfoList[i]);
            if (i == characterListModel.charaInfoList.Count - 1 || (i+1) % MaxItemNum == 0)
            {
                charaInfoList.Add(charaInfo);
            }
        }
        for (int i = 0; i < charaInfoList.Count; i++)
        {
            var go = NGUITools.AddChild(gridView.goUIGrid, gridView.goPrefab);
            var gUICharacterGroupItem = go.GetComponent<GUICharacterGroupItem>();
            gUICharacterGroupItem.Setup(charaInfoList[i],i);
        }

        gridView.uiGride.Reposition();
        gridView.uiScrollView.ResetPosition();
    }

    IEnumerator DelayRecenter()
    {
        yield return 1;
        gridView.uiCenterOnChild.Recenter();
    }

    public void SetCharaInfo(CharacterListModel model)
    {
        this.charaInfo = model.charaInfo;
        //charaIcon.GetIcon(this.charaInfo.AvatarType, false, this.SetIconSprite);
        string typeDescription = "";
        {
            Scm.Common.Master.CharaMasterData data;
            if (MasterData.TryGetChara((int)this.charaInfo.AvatarType, out data))
            if (data != null) typeDescription = data.Description;
        }
        characterInfoView.name.text = this.charaInfo.Name.ToString();
        characterInfoView.level.text = this.charaInfo.Level.ToString();
        characterInfoView.description.text = typeDescription;
        CharaBoard.GetBoard(this.charaInfo.AvatarType, this.charaInfo.SkinId, true, (GameObject resource) => { this.CreateBoard(this.charaInfo.AvatarType, resource, characterInfoView.transOn, null); });
    }

    void SetIconSprite(UIAtlas atlas, string spriteName)
    {
        characterInfoView.icon.atlas = atlas;
        characterInfoView.icon.spriteName = spriteName;
    }

    void CreateBoard(AvatarType avatarType, GameObject resource, Transform parent, UIPlayTween playTween)
    {
        // リソース読み込み完了
        if (resource == null || parent == null)
            return;
        // インスタンス化
        var go = SafeObject.Instantiate(resource) as GameObject;
        if (go == null)
            return;

        // 読み込み中に別のキャラに変更していたら破棄する
        /*
        var info = this.SelectInfo;
        if (info != null && info.AvatarType != avatarType)
        {
            Object.Destroy(go);
            return;
        }*/

        // 名前設定
        go.name = resource.name;
        // 親子付け
        var t = go.transform;
        parent.DestroyChildren();
        t.parent = parent;
        t.localPosition = Vector3.zero;
        t.localRotation = Quaternion.identity;
        t.localScale = Vector3.one;

        // 読み込みが完了してから再生を開始する
        if (playTween != null) playTween.Play(true);
    }

    public void SetPage(int num)
    {
        if (num < 0 || num >= characterInfoView.pages.Length) return;

        for (int i = 0; i < characterInfoView.pages.Length; i++)
        {
            characterInfoView.pages[i].color = i == num ? Color.blue : Color.white;
        }
    }

    public void Open()
    {
        gameObject.SetActive(true);
//        body.SetActive(true);
        //StartCoroutine(DelayRecenter());
    }

    public void Close()
    {
        gameObject.SetActive(false);
//        body.SetActive(false);
        gridView.uiGride.transform.DestroyChildren();
    }

    void OnSelect()
    {
        OnSelectCharaInfo(this, EventArgs.Empty);
    }
}
