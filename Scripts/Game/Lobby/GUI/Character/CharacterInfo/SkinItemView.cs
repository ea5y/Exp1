using UnityEngine;
using System;
using System.Collections;
using Scm.Common.Packet;

public class SkinItemView : MonoBehaviour {
    public UIButton buttonSelect;
    public UISprite icon;
    public UISprite iconSelect;
    public UILabel title;

    CharaIcon charaIcon { get { return ScmParam.Lobby.CharaIcon; } }
    CharaInfo _charaInfo;

    public CharaInfo charaInfo
    {
        get { return _charaInfo; }
        set { _charaInfo = value; }
    }
    CharacterAvatarParameter _characterAvatarParameter;

    public CharacterAvatarParameter characterAvatarParameter
    {
        get { return _characterAvatarParameter; }
        set { _characterAvatarParameter = value; }
    }
    /// <summary>
    /// 角色皮肤变更
    /// </summary>
    static public event EventHandler OnSelected = (sender, e) => { };
	// Use this for initialization
	void Start () {
        EventDelegate.Add(buttonSelect.onClick, OnSelect);
	}
	
    public void Setup(CharaInfo charaInfo, CharacterAvatarParameter characterAvatarParameter, bool isFirst)
    {
        this.charaInfo = charaInfo;
        this.characterAvatarParameter = characterAvatarParameter;
        gameObject.SetActive(true);
        title.text = characterAvatarParameter.Name;
        charaIcon.GetIcon(this.charaInfo.AvatarType, this.charaInfo.SkinId, false, this.SetIconSprite);
        if (charaInfo.trySkinId > 0)
        {
            iconSelect.gameObject.SetActive(charaInfo.trySkinId == characterAvatarParameter.Id);
            transform.localScale = charaInfo.trySkinId == characterAvatarParameter.Id ? Vector3.one * 1.1f : Vector3.one * 1;
        }
        else if (charaInfo.SkinId > 0)
        {
            iconSelect.gameObject.SetActive(charaInfo.SkinId == characterAvatarParameter.Id);//buttonSelect.normalSprite = charaInfo.SkinId == characterAvatarParameter.Id ? "Xworld_unselect_chara_board_02" : "Xworld_unselect_chara_board";
            transform.localScale = charaInfo.SkinId == characterAvatarParameter.Id ? Vector3.one * 1.1f : Vector3.one * 1;
        }
        else
        {
            iconSelect.gameObject.SetActive(isFirst);
            transform.localScale = isFirst ? Vector3.one * 1.1f : Vector3.one * 1;
        }
    }

    void SetIconSprite(UIAtlas atlas, string spriteName)
    {
        icon.atlas = atlas;
        icon.spriteName = spriteName;
    }

    void OnSelect()
    {
        OnSelected(this, EventArgs.Empty);
        Debug.Log("characterAvatarParameter.Id:" + characterAvatarParameter.Id + " charaInfo.SkinId:" + charaInfo.SkinId);
    }

    void OnEnable()
    {
        OnSelected += SkinItemView_OnSelected;
    }

    void OnDisable()
    {
        OnSelected -= SkinItemView_OnSelected;
    }

    void SkinItemView_OnSelected(object sender, EventArgs e)
    {
        var skinItemView = sender as SkinItemView;
        iconSelect.gameObject.SetActive(skinItemView == this);
        transform.localScale = skinItemView == this ? Vector3.one * 1.1f : Vector3.one * 1;
    }
}
