using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Scm.Common.Master;
using Scm.Common.Packet;

public class CharacterListModel
{

    /// <summary>
    /// 角色变更
    /// </summary>
    public event EventHandler OnCharaInfoChange = (sender, e) => { };
    static public event EventHandler OnCharaProfileChange = (sender, e) => { };

    /// <summary>
    /// 角色
    /// </summary>
    CharaInfo _charaInfo;

    public CharaInfo charaInfo
    {
        get { return _charaInfo; }
        set
        {
            if (_charaInfo != value)
            {
                _charaInfo = value;
                MasterData.TryGetCharaProfileMasterData((int)_charaInfo.AvatarType, out _charaProfileMasterData);
                // 通知
                this.OnCharaInfoChange(this, EventArgs.Empty);
                OnCharaProfileChange(_charaProfileMasterData, EventArgs.Empty);
            }
        }
    }
    CharaProfileMasterData _charaProfileMasterData;
    public CharaProfileMasterData charaProfileMasterData
    {
        get { return _charaProfileMasterData; }
        set { _charaProfileMasterData = value; }
    }

    public List<CharaInfo> charaInfoList = new List<CharaInfo>();
   
    
    public void Setup(CharaInfo charaInfo)
    {
        this.charaInfo = charaInfo;
    }

    public void Setup(List<CharaInfo> charaInfoList)
    {
        this.charaInfoList = charaInfoList;
    }
}
