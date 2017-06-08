using UnityEngine;
using System.Collections;
using Scm.Common.Packet;

public class AudioItemView : MonoBehaviour {
    public UIButton buttonPlay;
    public UILabel title;

    const string CueSheetName_Character = "X_world_Voice_p{0:D3}";
    ReplayVoiceParameter replayVoiceParameter;
    CharaInfo charaInfo;

	// Use this for initialization
	void Start () {
        EventDelegate.Add(buttonPlay.onClick, OnPlay);
	}

    public void Setup(CharaInfo charaInfo, ReplayVoiceParameter replayVoiceParameter)
    {
        this.replayVoiceParameter = replayVoiceParameter;
        this.charaInfo = charaInfo;
        gameObject.SetActive(true);
        title.text = replayVoiceParameter.Desc;

        if (replayVoiceParameter.Locked)
        {
            buttonPlay.GetComponent<Collider>().enabled = false;
            buttonPlay.normalSprite = "lock";
            buttonPlay.disabledSprite = "lock";
        }
        else
        {
            buttonPlay.GetComponent<Collider>().enabled = true;
            buttonPlay.normalSprite = "canplay";
            buttonPlay.disabledSprite = "canplay";
        }
    }
	
	void OnPlay () {
        AvatarType avatarType = ObsolateSrc.GetBaseAvatarType(charaInfo.AvatarType);
        string cueSheetName = string.Format(CueSheetName_Character, (int)avatarType);
        Debug.Log("cueSheetName:" + cueSheetName);
        SoundController.PlayUISe(cueSheetName, "down");
	}
}
