using UnityEngine;
using System.Collections;

namespace XUI.UserInfo
{
    [System.Serializable]
    public class Base
    {
        //btn back
        [SerializeField]
        XUIButton _btnBack;
        public XUIButton BtnBack { get { return _btnBack; } }

        //btn basic data
        [SerializeField]
        XUIButton _btnBasicData;
        public XUIButton BtnBasicData { get { return _btnBasicData; } }

        //btn combat Gains
        [SerializeField]
        XUIButton _btnCombatGains;
        public XUIButton BtnCombatGains { get { return _btnCombatGains; } }

        //btn chara statistics
        [SerializeField]
        public XUIButton _btnCharaStatistics;
        public XUIButton BtnCharaStatistics { get { return _btnCharaStatistics; } }
    }
}

