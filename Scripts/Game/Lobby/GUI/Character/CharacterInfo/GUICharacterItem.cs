using UnityEngine;
using System;
using System.Collections;

public class GUICharacterItem : MonoBehaviour {
    public UIButton buttonSelect;
    public UISprite icon;
    public UISprite selected;
    //public GameObject effect;
    public UILabel level;
    public UISprite[] stars;
    static public event EventHandler OnSelectCharacterItem = (sender, e) => { };

    CharaIcon charaIcon { get { return ScmParam.Lobby.CharaIcon; } }
    CharaInfo _charaInfo;
    Vector3 lastPosition = Vector3.zero;
    bool isSelect = false;

    public CharaInfo charaInfo
    {
        get { return _charaInfo; }
        set { _charaInfo = value; }
    }

	// Use this for initialization
	void Start () {
        EventDelegate.Add(buttonSelect.onClick, OnSelectItem);
	}

    void Update()
    {
//        if (!isSelect) return;
//
//        if (!lastPosition.Equals(transform.position))
//        {
//            lastPosition = transform.position;
//            selected.gameObject.SetActive(false);
//        }
//        else if (icon.isVisible &&!selected.gameObject.activeSelf)
//        {
//            selected.gameObject.SetActive(true);
//        }
    }

    public void Setup(CharaInfo charaInfo, bool isSelect = false)
    {
        this.charaInfo = charaInfo;
        gameObject.SetActive(true);
        charaIcon.GetIcon(this.charaInfo.AvatarType, this.charaInfo.SkinId, false, this.SetIconSprite);
        level.text = this.charaInfo.Level.ToString();
        selected.gameObject.SetActive(isSelect);
        this.isSelect = isSelect;
    }

    void SetIconSprite(UIAtlas atlas, string spriteName)
    {
        icon.atlas = atlas;
        icon.spriteName = spriteName;
    }

    void OnSelectItem()
    {
        OnSelectCharacterItem(this, EventArgs.Empty);
    }


    void OnEnable()
    {
        OnSelectCharacterItem += GUICharacterItem_OnSelectCharacterItem;
    }

    void OnDisable()
    {
        OnSelectCharacterItem -= GUICharacterItem_OnSelectCharacterItem;
    }

    void GUICharacterItem_OnSelectCharacterItem(object sender, EventArgs e)
    {
        var gUICharacterItem = sender as GUICharacterItem;
        selected.gameObject.SetActive(gUICharacterItem.charaInfo.Name == charaInfo.Name);
        isSelect = gUICharacterItem.charaInfo.Name == charaInfo.Name;
    }
}
