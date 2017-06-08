using UnityEngine;
using System.Collections;
using Scm.Common.Packet;

public class StoryItemView : MonoBehaviour {
    public UIButton buttonPlay;
    public UILabel title;

    CharacterStoryParameter characterStoryParameter;
    CharaInfo charaInfo;

	// Use this for initialization
	void Start () {
        EventDelegate.Add(buttonPlay.onClick, OnPlay);
	}

    public void Setup(CharaInfo charaInfo, CharacterStoryParameter characterStoryParameter)
    {
        this.characterStoryParameter = characterStoryParameter;
        this.charaInfo = charaInfo;
        gameObject.SetActive(true);
        title.text = characterStoryParameter.Desc;
        if (characterStoryParameter.Locked)
        {
            buttonPlay.GetComponent<Collider>().enabled = false;
            buttonPlay.enabled = false;
            buttonPlay.normalSprite = "ui_icon_lock_02";
        }
        else
        {
            buttonPlay.GetComponent<Collider>().enabled = true;
            buttonPlay.enabled = true;
            buttonPlay.normalSprite = "ui_icon_page_next01";
        }
    }

    void OnPlay()
    { 
    
    }
}
