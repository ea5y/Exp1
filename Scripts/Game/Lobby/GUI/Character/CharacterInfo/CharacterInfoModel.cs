using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Scm.Common.Master;
using Scm.Common.Packet;

public class CharacterInfoModel {

    /// <summary>
    /// 角色变更
    /// </summary>
    public event EventHandler OnCharaInfoChange = (sender, e) => { };

    /// <summary>
    /// 角色
    /// </summary>
    CharaInfo _charaInfo;

    public CharaInfo charaInfo
    {
        get { return _charaInfo; }
        set
        {
            if (true)//_charaInfo != value
            {
                _charaInfo = value;
                MasterData.TryGetCharaProfileMasterData((int)_charaInfo.AvatarType, out _charaProfileMasterData);
                CharaLevelMasterDataList = MasterData.TryGetLevelListByCharacterId((int)_charaInfo.AvatarType);
                // 通知
                this.OnCharaInfoChange(this, EventArgs.Empty);
            }
        }
    }

    CharaProfileMasterData _charaProfileMasterData;
    public CharaProfileMasterData charaProfileMasterData
    {
        get { return _charaProfileMasterData; }
        set { _charaProfileMasterData = value; }
    }

    public List<CharaLevelMasterData> CharaLevelMasterDataList = new List<CharaLevelMasterData>();
    public List<CharaInfo> charaInfoList = new List<CharaInfo>();
    /// <summary>
    /// 角色皮肤变更
    /// </summary>
    public event EventHandler OnCharacterNetInfoChange = (sender, e) => { };
    public List<CharacterAvatarParameter> characterAvatarParameterList = new List<CharacterAvatarParameter>();
    public List<ReplayVoiceParameter> replayVoiceParameterList = new List<ReplayVoiceParameter>();
    public List<CharacterStoryParameter> characterStoryParameterList = new List<CharacterStoryParameter>();
    
    public void Setup(CharaInfo charaInfo)
    {
        this.charaInfo = charaInfo;
    }

    public void Setup(List<CharaInfo> charaInfoList)
    {
        this.charaInfoList = charaInfoList;
        if (charaInfoList.Count > 0) this.charaInfo = this.charaInfoList[0];
    }

    public void SendGetCharacterAvatarAll()
    {
        CommonPacket.SendGetCharacterAvatarAll((long)charaInfo.UUID, ResponseGetCharacterAvatarAll);
    }
    void ResponseGetCharacterAvatarAll(List<CharacterAvatarParameter> args)
    {        
        characterAvatarParameterList = args;
        OnCharacterNetInfoChange(this, EventArgs.Empty);
    }

    public void SendSetCurrentAvatar()
    {
        if (charaInfo.SkinId == charaInfo.trySkinId) return;
        CommonPacket.SendSetCurrentAvatar((long)charaInfo.UUID, charaInfo.trySkinId, ResponseSetCurrentAvatar);
    }

    void ResponseSetCurrentAvatar(SetCurrentAvatarRes packet)
    {
        GUIChat.AddSystemMessage(false,packet.Result ? "更换皮肤成功！": "更换皮肤失败！");//MasterData.GetText(TextType.TX131_SetCharacterDeckRes_Success)
        Debug.Log("ResponseSetCurrentAvatar:" + packet.Result);
        if (packet.Result) charaInfo.SkinId = charaInfo.trySkinId;
    }

    public void SendGetCharacterReplayVoiceAll()
    {
        long id = (long)charaInfo.UUID;
        CommonPacket.SendGetCharacterReplayVoiceAll(5558, ResponseGetCharacterReplayVoiceAll);
    }
    void ResponseGetCharacterReplayVoiceAll(List<ReplayVoiceParameter> args)
    {
        replayVoiceParameterList = args;
        OnCharacterNetInfoChange(this, EventArgs.Empty);
    }

    public void SendGetCharacterStoryAll()
    {
        long id = (long)charaInfo.UUID;
        CommonPacket.SendGetCharacterStoryAll(5558, ResponseGetCharacterStoryAll);
    }
    void ResponseGetCharacterStoryAll(List<CharacterStoryParameter> args)
    {
        characterStoryParameterList = args;
        OnCharacterNetInfoChange(this, EventArgs.Empty);
    }
}
