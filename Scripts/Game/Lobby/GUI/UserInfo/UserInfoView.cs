//author: luwanzhong
//date: 2016-11-18
//desc: userInfo view

using UnityEngine;
using System.Collections;

namespace XUI.UserInfo{
   
    [System.Serializable]
    public class UserInfoView// : GUIViewBase
    {
#region SerializeField
        //root
        [SerializeField]
        GameObject _root;
        public GameObject Root { get { return this._root; } }

        //loading prefab
        public GameObject loadingPrefab;

        #region Base
        [SerializeField]
        XUI.UserInfo.Base _base;        
        public XUI.UserInfo.Base Base{ get { return this._base; } } 
        #endregion

        #region BasicData
        [SerializeField]
        XUI.UserInfo.BasicData _basicData;
        public XUI.UserInfo.BasicData BasicData{ get { return this._basicData; } }
        #endregion

        #region CombatGains
        [SerializeField]
        XUI.UserInfo.CombatGains _combatGains;
        public XUI.UserInfo.CombatGains CombatGains{ get { return this._combatGains; } }
        #endregion

        #region CharaStatistics
        [SerializeField]
        XUI.UserInfo.CharaStatistics _charaStatistics;
        public XUI.UserInfo.CharaStatistics CharaStatistics{ get { return this._charaStatistics; } }
        #endregion
#endregion

    }
}
