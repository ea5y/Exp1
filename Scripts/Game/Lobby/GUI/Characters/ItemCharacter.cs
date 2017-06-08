using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XDATA;
using Scm.Common.Master;

namespace XUI
{
    public class CharaItemData : CustomControl.ScrollViewCellItemData
    {
        public Chara chara;
        public bool isSelect;
        public CharaTypt charaType;        
    }

    public class ItemCharacter : CustomControl.ScrollViewCellItem
    {        
        public CharaData data;
        public int localIndex = 0;
        
        public CharaTypt charaType = CharaTypt.Unknow;
        public UISprite icon;
        public UISprite hook;
        public UILabel lv;
        public UISprite imgLock;
        public UISprite imgFlag;
        public GameObject effect;
        public UISprite bgSprite;
        public UIButton bgButton;

        public UISprite[] stars;
        public UISprite itemStar;
        public GameObject starsGroup;

        public UIButton btn;
        public int addExp;
        public int costGold;
        public ulong uuid;

        public delegate void SelectedHandle();
        public event SelectedHandle SelectedEvent;

        public UILabel tag;

        private readonly Color expireCharacterColor = new Color(0.109375f, 1, 0);

        public bool WillExpire {
            get {
                return willExpire;
            }
            set {
                willExpire = value;
                if (bgSprite != null) {
                    bgSprite.color = willExpire ? expireCharacterColor : Color.white;
                } else {
                    Debug.LogError("Not set bgSprite");
                }
                if (bgButton != null) {
                    bgButton.hover = bgButton.defaultColor = willExpire ? expireCharacterColor : Color.white;
                } else {
                    Debug.LogError("Not set bgButton");
                }
            }
        }
        private bool willExpire = false;

        private bool _isSelected = false;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;

                if (charaType == CharaTypt.Upgrade)
                {                    
                    this.UseForUpgrading();
                    this.SetHook();
                }

                if (charaType == CharaTypt.Evolve)
                {                    
                    this.UseForEvolving();
                    this.SetHook();
                }

                if (charaType == CharaTypt.Normal)
                {
                    this.UseForNormal();
                    //this.SetEffect();
                    this.SetHook();
                }
                
                //this.SetHook();
            }
        }

        public void SetEffect()
        {
            this.effect.gameObject.SetActive(_isSelected);
        }

        public void SetHook()
        {
            this.hook.gameObject.SetActive(_isSelected);
        }

        public void SetLock()
        {
            this.imgLock.gameObject.SetActive(this.data.chara.Info.IsLock);
        }

        private void MarkFlag()
        {
            switch (this.charaType)
            {
                case CharaTypt.Normal:
                    var temp = transform.parent.GetComponent<ItemCharacters>();
                    var charaData = temp.charasData;
                    charaData[localIndex].selectFlagNormal = _isSelected;
                    break;
                case CharaTypt.Upgrade:
                    var charaData1 = transform.parent.GetComponent<ItemUpgrade>().charasData;
                    charaData1[localIndex].selectFlagUpgrade = _isSelected;
                    break;
                case CharaTypt.Evolve:
                    var charaData2 = transform.parent.GetComponent<ItemEvolve>().charasData;
                    charaData2[localIndex].selectFlagEvolve = _isSelected;
                        break;
            }
        }

        private void UseForUpgrading()
        {
            if (_isSelected == true)
            {
                if (this.data.chara.Info.IsLock)
                {
                    GUITipMessage.Instance.Show("角色被锁定了!");
                    _isSelected = false;
                    return;
                }

                if(this.data.isInDeck)
                {
                    GUITipMessage.Instance.Show("该角色在编队中！");
                    _isSelected = false;
                    return;
                }

                CharaStarMasterData data;
                MasterData.TryGetCharaStarMasterData(this.data.chara.Info.StarId, out data);
                if (data.Star > GUICharacterDetial.Instance.currentCharaData.stars)
                {
                    GUITipMessage.Instance.Show("该角色星级较高！");                    
                }

                if (GUICharacterDetial.Instance.targetCharaData.starData.CanLevelUp)
                {
                    GUICharacterDetial.Instance.fodderIdList.Add(uuid);
                    GUICharacterDetial.Instance.UpgradePrice += costGold;

                    GUICharacterDetial.Instance.ADD_EXP_SUM += addExp;
                }
                else
                {
                    _isSelected = false;
                    GUITipMessage.Instance.Show("已经达到当前星级最高等级!");
                }
            }
            else
            {
                GUICharacterDetial.Instance.ADD_EXP_SUM -= addExp;
                GUICharacterDetial.Instance.fodderIdList.Remove(uuid);

                GUICharacterDetial.Instance.UpgradePrice -= costGold;
            }

            this.MarkFlag();
        }

        private void UseForEvolving()
        {
            if (_isSelected == true)
            {
                if (this.data.chara.Info.IsLock)
                {
                    GUITipMessage.Instance.Show("角色被锁定了!");
                    _isSelected = false;
                    return;
                }

                if (this.data.isInDeck)
                {
                    GUITipMessage.Instance.Show("该角色在编队中！");
                    _isSelected = false;
                    return;
                }

                if (GUICharacterDetial.Instance.currentCharaData.starData.CanStarUp)
                {
                    //Limit
                    if (GUICharacterDetial.Instance.EVOLVE_FODDER_COUNT < GUICharacterDetial.Instance.NeedEvolveFodderCount)
                    {
                        GUICharacterDetial.Instance.fodderIdList.Add(uuid);
                        GUICharacterDetial.Instance.EvolvePrice += costGold;

                        GUICharacterDetial.Instance.EVOLVE_FODDER_COUNT++;
                    }
                    else
                    {
                        _isSelected = false;
                        GUITipMessage.Instance.Show("升星材料已经满了!");
                    }
                }
                else
                {
                    _isSelected = false;
                    GUITipMessage.Instance.Show("等级未到，不能升星!");
                }
            }
            else
            {
                GUICharacterDetial.Instance.EVOLVE_FODDER_COUNT--;
                GUICharacterDetial.Instance.fodderIdList.Remove(uuid);

                if (GUICharacterDetial.Instance.EvolvePrice != 0)
                    GUICharacterDetial.Instance.EvolvePrice -= costGold;
            }
            this.MarkFlag();
        }

        private void UseForNormal()
        {
            GUICharacters.Instance.preChara = transform.GetComponent<ItemCharacter>();

            if (this.IsSelected == true)
            {
                this.MarkFlag();
                GUICharacters.Instance.preChara = transform.GetComponent<ItemCharacter>();
                GUICharacters.Instance.CurCharaData = this.data;
                GUICharacters.Instance.PreCharaData = this.data;
            }
        }
    }

}
