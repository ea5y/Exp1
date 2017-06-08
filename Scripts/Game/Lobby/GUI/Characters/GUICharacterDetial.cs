using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Scm.Common.Master;
using System;
using Scm.Common.Packet;
using UnityEngine.UI.Extensions;

using XDATA;

namespace XUI
{
    public class GUICharacterDetial : PanelBase<GUICharacterDetial>
    {
        #region Property
        private int STARS_LIMIT_NUM = 4;

        public CharacterDetialView view;
        public TweenPosition tabMenuTween;
        public TweenPosition rightTween;

        private Camera camera3D;

        private CustomControl.TabPagesManager detialPages;
        private List<UIButton> detialTabList;
        private enum DetailPageType
        {
            Info,
            Grow,
            Skin,
            Wallpaper
        }

        private List<UIButton> infoTabList;

        private CustomControl.TabPagesManager growPages;
        private List<UIButton> growTabList;
        private enum GrowPageType
        {
            Upgrade,
            Evolve
        }

        private Chara chara;

        private SkillIcon skillIcon { get { return ScmParam.Battle.SkillIcon; } }

        CharaIcon CharaIcon { get { return ScmParam.Lobby.CharaIcon; } }

        public List<ulong> fodderIdList = new List<ulong>();

        //price
        private int upgradePrice = 0;
        public int UpgradePrice
        {
            get
            {
                return this.upgradePrice;
            }

            set
            {
                this.upgradePrice = value;
                this.SetUpgradePrice();
            }
        }

        private void SetUpgradePrice()
        {
            this.view.growView.UpgradeView.price.text = this.UpgradePrice + "";

            if (this.UpgradePrice > PlayerData.Instance.Gold)
                this.view.growView.UpgradeView.price.color = Color.red;
            else
                this.view.growView.UpgradeView.price.color = Color.white;
        }

        private int evolvePrice = 0;
        public int EvolvePrice
        {
            get
            {
                return this.evolvePrice;
            }
            set
            {
                this.evolvePrice = value;
                this.SetEvolvePrice();
            }
        }

        private void SetEvolvePrice()
        {
            this.view.growView.EvolveView.price.text = this.EvolvePrice + "";
            if (this.EvolvePrice > PlayerData.Instance.Gold)
                this.view.growView.EvolveView.price.color = Color.red;
            else
                this.view.growView.EvolveView.price.color = Color.white;
        }

        public struct CurrentCharaData
        {
            public ulong uuid;
            public int starId;
            public int lv;
            public int stars;
            public int exp;
            public CharaLevelMasterData property;
            public CharaStarMasterData starData;
        };

        public struct TargetCharaData
        {
            public ulong uuid;
            public int starId;
            public int lv;
            public int stars;
            public int exp;
            public CharaLevelMasterData property;
            public CharaStarMasterData starData;
        };

        public CurrentCharaData currentCharaData;
        public TargetCharaData targetCharaData;

        event EventHandler OnCurrentCharaDataChange = (s, e) => { };
        event EventHandler OnTargetCharaDataChange = (s, e) => { };

        private CustomControl.ScrollView<ItemUpgrade> upgradeScrollView;
        private CustomControl.ScrollView<ItemEvolve> evolveScrollView;

        public CustomControl.ScrollPage<ItemSkinCard> skinCardScrollView;
        public CustomControl.ScrollView<ItemWallPaper> wallpaperScrollView;
        #endregion

        private void Awake()
        {
            base.Awake();

            this.SetDetialTabList();
            this.SetDetialPages();

            this.SetInfoTabList();

            this.SetGrowTabList();
            this.SetGrowPages();

            this.registerEventOnce();
            this.RegisterSkillTouchEvent();

            this.GetTween();            
        }

        private void Start()
        {
            //First hide
            this.Hide();
        }
        
        private void GetTween()
        {
            this.tabMenuTween = this.view.tabMenu.GetComponent<TweenPosition>();
            this.rightTween = this.view.right.GetComponent<TweenPosition>();
        }

        private void SetDetialTabList()
        {
            this.detialTabList = new List<UIButton>();
            this.detialTabList.Add(this.view.btnInfo);
            this.detialTabList.Add(this.view.btnGrow);
            this.detialTabList.Add(this.view.btnSkin);
            this.detialTabList.Add(this.view.btnWallpaper);
        }

        private void SetDetialPages()
        {
            this.detialPages = new CustomControl.TabPagesManager();
            this.detialPages.AddPage(this.view.infoView);
            this.detialPages.AddPage(this.view.growView);
            this.detialPages.AddPage(this.view.skinView);
            this.detialPages.AddPage(this.view.wallpaperView);
        }

        private void SetInfoTabList()
        {
            this.infoTabList = new List<UIButton>();
            this.infoTabList.Add(this.view.infoView.btnAbility);
            this.infoTabList.Add(this.view.infoView.btnBack);
        }

        //little pages
        private void InfoGoto(GameObject from, GameObject to)
        {
            from.SetActive(false);
            to.SetActive(true);
        }

        private void SetGrowTabList()
        {
            this.growTabList = new List<UIButton>();
            this.growTabList.Add(this.view.growView.btnUpgrade);
            this.growTabList.Add(this.view.growView.btnEvolve);
        }

        private void SetGrowPages()
        {
            this.growPages = new CustomControl.TabPagesManager();
            this.growPages.AddPage(this.view.growView.UpgradeView);
            this.growPages.AddPage(this.view.growView.EvolveView);
        }
        
        private void Hide()
        {
            this.view.root.SetActive(false);
        }

        public void SetSkinBtnState(bool use, bool shop, string str = "")
        {
            this.view.skinView.btnSure.gameObject.SetActive(use);
            //this.view.skinView.btnShop.gameObject.SetActive(shop);
            this.view.skinView.btnShop.gameObject.SetActive(false);

            this.view.skinView.lblStatus.gameObject.SetActive(!use);
            this.view.skinView.lblStatus.text = str;
        }

        

        private void OnEnable()
        {
            this.InitView();
            this.SetCamera3D();
        }
        
        private void OnDisable()
        {
            this.ResetCamera3D();
            this.DetroyPreview();
        }

        private void DetroyPreview()
        {
            //this.view.skinView.portriat3D.DestroyChild();
            this.view.skinView.portriat3D.DestroyChildImmediate();
        }

        public override void Reset()
        {
            this.ResetTween();
        }

        private void ResetTween()
        {
            this.tabMenuTween.ResetToBeginning();
            this.rightTween.ResetToBeginning();
        }

        private void RegisterEvent()
        {
            this.OnCurrentCharaDataChange += this.UpdateUpgradeLeft;
            this.OnCurrentCharaDataChange += this.UpdateEvolveLeft;
            this.OnCurrentCharaDataChange += this.UpdateInfoProperty;

            this.OnTargetCharaDataChange += this.UpdateUpgradeLeftTarget;
            this.OnTargetCharaDataChange += this.UpdateEvolveLeftTarget;
            this.OnTargetCharaDataChange += this.UpdateUpgradeBtnSureState;
            this.OnTargetCharaDataChange += this.UpdateEvolveBtnSureState;

            GUICharacters.Instance.OnCharaDataListChange += this.UpdateUpgradeScrollView;
            GUICharacters.Instance.OnCharaDataListChange += this.UpdateEvolveScrollView;

            PlayerData.Instance.OnGoldChange += this.GetGold;
        }

        private void GetGold(object sender, EventArgs e)
        {
            this.view.growView.UpgradeView.curGold.text = ((PlayerData)sender).Gold + "";
            this.view.growView.EvolveView.curGold.text = ((PlayerData)sender).Gold + "";
        }


        private void InitView()
        {
            this.SwitchTo(DetailPageType.Info);
            this.InfoGoto(this.view.infoView.backView, this.view.infoView.abilityView);
            CustomControl.ToolFunc.BtnSwitchTo(this.view.infoView.btnAbility, this.infoTabList);
            this.SwitchTo(GrowPageType.Upgrade);
        }
        
        private void DeleteCharaListEvent()
        {
            this.OnCurrentCharaDataChange -= this.UpdateUpgradeLeft;
            this.OnCurrentCharaDataChange -= this.UpdateEvolveLeft;
            this.OnCurrentCharaDataChange -= this.UpdateInfoProperty;

            this.OnTargetCharaDataChange -= this.UpdateUpgradeLeftTarget;
            this.OnTargetCharaDataChange -= this.UpdateEvolveLeftTarget;
            this.OnTargetCharaDataChange -= this.UpdateUpgradeBtnSureState;
            this.OnTargetCharaDataChange -= this.UpdateEvolveBtnSureState;

            GUICharacters.Instance.OnCharaDataListChange -= this.UpdateUpgradeScrollView;
            GUICharacters.Instance.OnCharaDataListChange -= this.UpdateEvolveScrollView;

            PlayerData.Instance.OnGoldChange += this.GetGold;
        }

        private void UpdateEvolveScrollView(object sender, EventArgs e)
        {
            this.SetEvolveScrollView();
        }

        private void SetEvolveScrollView()
        {
            //reject
            List<CharaData> realList = new List<CharaData>();
            List<CharaData> charaDataList = new List<CharaData>(GUICharacters.Instance.CharaDataList);

            for (int i = 0; i < charaDataList.Count; i++)
            {
                //reject self
                var stars = CharaStarMaster.Instance.GetStarByID(charaDataList[i].chara.Info.StarId);

                if (charaDataList[i].chara.Info.UUID != currentCharaData.uuid 
                    && stars == currentCharaData.stars 
                    && charaDataList[i].chara.Info.CharacterMasterID == this.chara.Info.CharacterMasterID
                    && charaDataList[i].chara.Info.TotalTime == 0)
                {
                    realList.Add(charaDataList[i]);
                }
            }

            //Sort
            realList.Sort((a, b) => {
                int t = b.chara.Info.Level.CompareTo(a.chara.Info.Level);
                if (t == 0)
                {
                    return b.chara.Info.UUID.CompareTo(a.chara.Info.UUID);
                }
                return t;
            });

            var datas = CharaData.Pack(realList, 3);
            if (this.evolveScrollView == null)
                this.evolveScrollView = new CustomControl.ScrollView<ItemEvolve>(this.view.growView.EvolveView.grid, this.view.growView.EvolveView.itemEvolve);

            this.evolveScrollView.CreateWeight(datas);
        }
        
        private void UpdateUpgradeScrollView(object sender, EventArgs e)
        {
            this.SetUpgradeScrollView();
        }

        private void SetUpgradeScrollView()
        {
            //reject
            List<CharaData> realList = new List<CharaData>();
            List<CharaData> charaDataList = new List<CharaData>(GUICharacters.Instance.CharaDataList);

            for (int i = 0; i < charaDataList.Count; i++)
            {
                //reject self
                var stars = CharaStarMaster.Instance.GetStarByID(charaDataList[i].chara.Info.StarId);

                if (charaDataList[i].chara.Info.UUID != currentCharaData.uuid
                    && charaDataList[i].chara.Info.TotalTime == 0)
                {
                    realList.Add(charaDataList[i]);
                }
            }

            //Sort
            realList.Sort((a, b) => {
                int t = b.chara.Info.Level.CompareTo(a.chara.Info.Level);
                if (t == 0)
                {
                    return b.chara.Info.UUID.CompareTo(a.chara.Info.UUID);
                }
                return t;
            });

            var datas = CharaData.Pack(realList, 3);
            if (this.upgradeScrollView == null)
                this.upgradeScrollView = new CustomControl.ScrollView<ItemUpgrade>(this.view.growView.UpgradeView.grid, this.view.growView.UpgradeView.itemUpgrade);

            this.upgradeScrollView.CreateWeight(datas);
        }

        public override IEnumerator Reverse()
        {
            yield return this.TweenReverse();
            GUICharacters.Instance.Reverse();
        }
        
        private void SwitchTo(DetailPageType pageType)
        {
            SoundController.PlaySe(SoundController.SeID.WindowClose);
            switch(pageType)
            {
                case DetailPageType.Info:
                    this.detialPages.SwitchTo(this.view.infoView);
                    CustomControl.ToolFunc.BtnSwitchTo(this.view.btnInfo, this.detialTabList);
                    break;
                case DetailPageType.Grow:
                    CustomControl.ToolFunc.BtnSwitchTo(this.view.btnGrow, this.detialTabList);
                    this.detialPages.SwitchTo(this.view.growView);                    
                    break;
                case DetailPageType.Skin:
                    this.detialPages.SwitchTo(this.view.skinView);
                    CustomControl.ToolFunc.BtnSwitchTo(this.view.btnSkin, this.detialTabList);
                    break;
                case DetailPageType.Wallpaper:
                    CustomControl.ToolFunc.BtnSwitchTo(this.view.btnWallpaper, this.detialTabList);
                    this.detialPages.SwitchTo(this.view.wallpaperView);
                    break;
            }
        }

        private void SwitchTo(GrowPageType pageType)
        {
            SoundController.PlaySe(SoundController.SeID.WindowClose);
            switch (pageType)
            {
                case GrowPageType.Upgrade:                    
                    this.growPages.SwitchTo(this.view.growView.UpgradeView);
                    CustomControl.ToolFunc.BtnSwitchTo(this.view.growView.btnUpgrade, this.growTabList);
                    break;
                case GrowPageType.Evolve:
                    this.growPages.SwitchTo(this.view.growView.EvolveView);
                    CustomControl.ToolFunc.BtnSwitchTo(this.view.growView.btnEvolve, this.growTabList);
                    break;
            }
        }

        public void Open(Chara chara)
        {
            this.chara = chara;
            PanelManager.Instance.Open(this.view.root);

            this.RegisterEvent();
            this.InitCurrentCharaData();

            this.SetInfoView();
        }

        public void InitCurrentCharaData()
        {
            //get star data
            int starId = this.chara.Info.StarId == 0 ? 1 : this.chara.Info.StarId;
            CharaStarMasterData starData;
            MasterData.TryGetCharaStarMasterData(starId, out starData);
            Debug.Log("starId: " + starId);

            //get property
            int level = this.chara.Info.Level == 0 ? 1 : this.chara.Info.Level;
            CharaLevelMasterData property;
            MasterData.TryGetCharaLevelMasterData((int)this.chara.Info.AvatarType, 1, out property);

            //currentCharaData
            this.UpdateCurrentCharaData(this.chara.Info.UUID, this.chara.Info.StarId, this.chara.Info.Level, this.chara.Info.Exp, property, starData);
        }
        
        #region InfoView
        private void SetInfoView()
        {
            this.SetAbility();
            this.SetBack();
        }

        private void SetAbility()
        {
            this.SetRadar();
            this.SetSkill();
        }

        private void SetInfoProperty()
        {
            float curHP, curDEF, curATK, curSPD;
            this.CalcCurProperty(currentCharaData, out curHP, out curDEF, out curATK, out curSPD);

            this.view.infoView.hp.text = string.Format("{0:F2}", curHP);
            this.view.infoView.def.text = string.Format("{0:F2}", curDEF);
            this.view.infoView.atk.text = string.Format("{0:F2}", curATK);
            this.view.infoView.spd.text = string.Format("{0:F2}", curSPD);
        }

        private void SetRadar()
        {
            CharaProfileMasterData data;
            MasterData.TryGetCharaProfileMasterData((int)this.chara.Info.AvatarType, out data);

            if(data == null)
            {
                Debug.LogError("==>雷达：没取到Master数据");
                return;
            }
            float baseNum = 10;
            Radar.Instance.SetValues(data.Ctrl / baseNum, data.Atk / baseNum, data.Def / baseNum, data.Spd / baseNum, data.Aid / baseNum);
        }

        private string skillName1;
        private string skillName2;
        private string skillName3;
        private string skillName4;

        private string skillDesc1;
        private string skillDesc2;
        private string skillDesc3;
        private string skillDesc4;
        private void SetSkill()
        {
            CharaLevelMasterData charaLv;
            if (MasterData.TryGetCharaLv((int)this.chara.Info.AvatarType, 1, out charaLv))
            {
                CharaButtonSetMasterData data;
                if (MasterData.TryGetCharaButtonSet(charaLv, out data))
                {
                    skillIcon.GetSkillIcon(data, 1, SkillButtonType.Normal, false, (atlas, spriteName) => { this.SetSkillIcon(atlas, spriteName, this.view.infoView.skill_1); });
                    skillIcon.GetSkillIcon(data, 1, SkillButtonType.Skill1, false, (atlas, spriteName) => { this.SetSkillIcon(atlas, spriteName, this.view.infoView.skill_2); });
                    skillIcon.GetSkillIcon(data, 1, SkillButtonType.Skill2, false, (atlas, spriteName) => { this.SetSkillIcon(atlas, spriteName, this.view.infoView.skill_3); });
                    skillIcon.GetSkillIcon(data, 1, SkillButtonType.SpecialSkill, false, (atlas, spriteName) => { this.SetSkillIcon(atlas, spriteName, this.view.infoView.skill_4); });
                }

                //Set desc
                this.skillName1 = data.AttackButton.ButtonName;
                this.skillName2 = data.Skill1Button.ButtonName;
                this.skillName3 = data.Skill2Button.ButtonName;
                this.skillName4 = data.SpecialSkillButton.ButtonName;
                this.skillDesc1 = this.GetSkillDesc(data.AttackButton);
                this.skillDesc2 = this.GetSkillDesc(data.Skill1Button);
                this.skillDesc3 = this.GetSkillDesc(data.Skill2Button);
                this.skillDesc4 = this.GetSkillDesc(data.SpecialSkillButton);
            }
        }

        private void SetSkillIcon(UIAtlas atlas, string spriteName, UISprite sp)
        {
            if (sp == null)
                return;

            sp.atlas = atlas;
            sp.spriteName = spriteName;

            if (sp.GetAtlasSprite() == null)
            {
                if (atlas != null && !string.IsNullOrEmpty(spriteName))
                {
                    Debug.LogWarning(string.Format(
                        "SetIconSprite:\r\n" +
                        "Sprite Not Found!! AvatarType = {0} SpriteName = {1}", 1, spriteName));//this.CharaInfo.AvatarType
                }
            }
        }

        private string GetSkillDesc(CharaButtonMasterData skillBtn)
        {
            var result = CharaButtonDescMaster.Instance.GetByCharaButtonId(skillBtn.ID);
            if(result == null)
            {
                Debug.LogError("更新Master数据！");
                return "";
            }
            return result.Desc;
        }

        private void RegisterSkillTouchEvent()
        {
            UIEventListener.Get(this.view.infoView.skill_1.gameObject).onPress = this.OnSkill1Press;
            UIEventListener.Get(this.view.infoView.skill_2.gameObject).onPress = this.OnSkill2Press;
            UIEventListener.Get(this.view.infoView.skill_3.gameObject).onPress = this.OnSkill3Press;
            UIEventListener.Get(this.view.infoView.skill_4.gameObject).onPress = this.OnSkill4Press;
        }

        private void OnSkillPress(bool isDown, Action func)
        {
            if(isDown)
            {
                if(this.view.infoView.skillDesc.activeSelf == false)
                {
                    this.view.infoView.skillDesc.SetActive(true);
                    func();
                }
            }
            else
            {
                this.view.infoView.skillDesc.SetActive(false);
            }
        }

        private void OnSkill1Press(GameObject obj, bool isDown)
        {
            this.OnSkillPress(isDown, () => {
                    this.view.infoView.lblSkillDesc.text = this.skillDesc1;
                    this.view.infoView.lblSkillName.text = this.skillName1;
                    });
        }

        private void OnSkill2Press(GameObject obj, bool isDown)
        {
            this.OnSkillPress(isDown, () => {
                    this.view.infoView.lblSkillDesc.text = this.skillDesc2;
                    this.view.infoView.lblSkillName.text = this.skillName2;
                    });
        }

        private void OnSkill3Press(GameObject obj, bool isDown)
        {
            this.OnSkillPress(isDown, () => {
                    this.view.infoView.lblSkillDesc.text = this.skillDesc3;
                    this.view.infoView.lblSkillName.text = this.skillName3;
                    });
        }

        private void OnSkill4Press(GameObject obj, bool isDown)
        {
            this.OnSkillPress(isDown, () => {
                    this.view.infoView.lblSkillDesc.text = this.skillDesc4;
                    this.view.infoView.lblSkillName.text = this.skillName4;
                    });
        }

        private void SetBack()
        {
            CharaProfileMasterData data;
            MasterData.TryGetCharaProfileMasterData(this.chara.Info.CharacterMasterID, out data);
            this.view.infoView.back.text = data != null ? data.background : "无数据";
        }
        #endregion

        #region GrowView
        private void SetGrowView()
        {
            this.InitBtn();
            this.GetGold(PlayerData.Instance, EventArgs.Empty);
            this.SetUpgradeScrollView();
            this.SetEvolveScrollView();
            this.ResetUpgradeScrollView();
            this.ClearFoddersData();
        }
        
        void InitBtn()
        {
            this.EnableBtn(this.view.growView.UpgradeView.btnSure, false);
            this.EnableBtn(this.view.growView.EvolveView.btnSure, false);
        }
                
        void UpdateCurrentCharaData(ulong uuid, int currentStarId, int currentlv, int currentExp, CharaLevelMasterData property, CharaStarMasterData starData)
        {
            this.currentCharaData.uuid = uuid;
            this.currentCharaData.starId = currentStarId;
            this.currentCharaData.lv = currentlv;
            this.currentCharaData.stars = CharaStarMaster.Instance.GetStarByID(currentStarId);
            this.currentCharaData.exp = currentExp;

            this.currentCharaData.property = property;
            this.currentCharaData.starData = starData;

            this.OnCurrentCharaDataChange(this, EventArgs.Empty);
        }
        
        public void ResetUpgradeScrollView()
        {
            if (GUICharacters.Instance.CharaDataList == null)
                return;
            foreach(var charaData in GUICharacters.Instance.CharaDataList)
            {
                if(charaData.selectFlagUpgrade)
                {
                    charaData.selectFlagUpgrade = false;

                    var item = this.upgradeScrollView.FindCellItem<ItemCharacter>(charaData.index);
                    if(item != null)
                        item.IsSelected = false;
                }
            }

            this.ClearFoddersData();
        }

        public void ResetEvolveScrollView()
        {
            if (null == GUICharacters.Instance || GUICharacters.Instance.CharaDataList == null)
                return;
            foreach(var charaData in GUICharacters.Instance.CharaDataList)
            {
                if (charaData.selectFlagEvolve)
                {
                    charaData.selectFlagEvolve = false;

                    var item = this.evolveScrollView.FindCellItem<ItemCharacter>(charaData.index);
                    if (item != null)
                        item.IsSelected = false;
                }
            }

            this.ClearFoddersData();
        }
        
        void UpdateTargetCharaData(int starId, int level, int exp, CurrentCharaData currentCharaData)
        {
            Debug.Log("TargetChara updated!");
            //show property
            CharaStarMasterData starDataUpdated;
            MasterData.TryGetCharaStarMasterData(starId, out starDataUpdated);

            //Updata target charaData
            targetCharaData.uuid = currentCharaData.uuid;
            targetCharaData.starId = starId;
            targetCharaData.lv = level;
            targetCharaData.stars = CharaStarMaster.Instance.GetStarByID(starId);
            targetCharaData.exp = exp;
            targetCharaData.property = currentCharaData.property;
            targetCharaData.starData = starDataUpdated;

            this.OnTargetCharaDataChange(this, EventArgs.Empty);
        }

        #region Upgrade        
        void SetUpgradeLeft()
        {
            this.SetUpgradePropertyText();
            this.SetLv();
            this.SetExpSlider();
        }
        
        void SetUpgradePropertyText()
        {
            float curHP, curDEF, curATK, curSPD;
            this.CalcCurProperty(currentCharaData, out curHP, out curDEF, out curATK, out curSPD);

            if (targetCharaData.property != null && targetCharaData.lv > currentCharaData.lv)
            {
                float tarHP, tarDEF, tarATK, tarSPD;
                this.CalcTarProperty(targetCharaData, out tarHP, out tarDEF, out tarATK, out tarSPD);
                this.view.growView.UpgradeView.lvTarget.text = targetCharaData.lv + "";
                this.view.growView.UpgradeView.hp.text = string.Format("{0:F2} [ff0000](+{1:F2})[-]", curHP, tarHP - curHP);
                this.view.growView.UpgradeView.def.text = string.Format("{0:F2} [ff0000](+{1:F2})[-]", curDEF, tarDEF - curDEF);
                this.view.growView.UpgradeView.atk.text = string.Format("{0:F2} [ff0000](+{1:F2})[-]", curATK, tarATK - curATK);
                this.view.growView.UpgradeView.spd.text = string.Format("{0:F2} [ff0000](+{1:F2})[-]", curSPD, tarSPD - curSPD);
            }
            else
            {
                this.view.growView.UpgradeView.hp.text = string.Format("{0:F2}", curHP);
                this.view.growView.UpgradeView.def.text = string.Format("{0:F2}", curDEF);
                this.view.growView.UpgradeView.atk.text = string.Format("{0:F2}", curATK);
                this.view.growView.UpgradeView.spd.text = string.Format("{0:F2}", curSPD);
            }
        }

        void SetLv()
        {
            this.view.growView.UpgradeView.lvCurrent.text = currentCharaData.lv + "";
            if (targetCharaData.property != null)
                this.view.growView.UpgradeView.lvTarget.gameObject.SetActive(targetCharaData.lv > currentCharaData.lv);
        }

        void SetExpSlider()
        {
            var baseExp = CharaStarMaster.Instance.GetBaseExp(currentCharaData.starId);
            if(baseExp == 0 && currentCharaData.starId != 1)
                baseExp = CharaStarMaster.Instance.GetBaseExp(--currentCharaData.starId);
            var expOverFlow = currentCharaData.exp - baseExp;
            var needEXP = CharaStarMaster.Instance.GetDeltaExp(currentCharaData.starId);
            this.view.growView.UpgradeView.expTarget.fillAmount = (float)(expOverFlow + ADD_EXP_SUM) / needEXP > 1.0f ? 1.0f : (float)(expOverFlow + ADD_EXP_SUM) / needEXP;
            this.view.growView.UpgradeView.expCurrent.fillAmount = (float)expOverFlow / needEXP > 1.0f ? 1.0f : (float)expOverFlow / needEXP;

            if (needEXP == 0)
                needEXP = CharaStarMaster.Instance.GetDeltaExp(CharaStarMaster.Instance.GetNextStarId(currentCharaData.starId));
            this.view.growView.UpgradeView.exp.text = expOverFlow + ADD_EXP_SUM + " / " + needEXP;
        }
        
        private int add_exp_sum = 0;
        public int ADD_EXP_SUM
        {
            get { return add_exp_sum; }
            set
            {
                add_exp_sum = value;
                Debug.Log("Add_EXP_SUM: " + add_exp_sum);

                int starId, level, exp;
                CharaStarMaster.Instance.AddExp(currentCharaData.starId, currentCharaData.exp, add_exp_sum, out starId, out level, out exp);
                this.UpdateTargetCharaData(starId, level, exp, this.currentCharaData);
            }
        }

        struct SendPacketUpgrade
        {
            public ulong uuidSelf;
            public ulong[] uuidsFodderArray;
            public Action<PowerupRes> response;
        };
        SendPacketUpgrade sendPacketUpgrade;

        private void OnBtnUpgradeSureClick()
        {
            this.InitSendPacketUpgrade();
            StartCoroutine(Net.Network.UpgradeGrow(sendPacketUpgrade.uuidSelf, sendPacketUpgrade.uuidsFodderArray, sendPacketUpgrade.response));
        }

        void InitSendPacketUpgrade()
        {
            sendPacketUpgrade.uuidSelf = currentCharaData.uuid;
            sendPacketUpgrade.uuidsFodderArray = this.fodderIdList.ToArray();
            sendPacketUpgrade.response = (res) => {
                if (res.PowerupResult == Scm.Common.GameParameter.PowerupResult.Fail)
                {
                    GUITipMessage.Instance.Show("升级失败！");
                    return;
                }

                //play special efficiency
                ParticleSystem ps = this.view.growView.UpgradeView.afx.GetComponent<ParticleSystem>();
                if (ps != null)
                {
                    ps.Stop();
                    ps.Play();
                }

                //gold
                PlayerData.Instance.Gold = res.Money;

                //res.GetParam
                this.chara.UpdateInfo(new CharaInfo(res.GetParam()));

                //Set left
                this.UpdateCurrentCharaData(currentCharaData.uuid, targetCharaData.starId, targetCharaData.lv, targetCharaData.exp, targetCharaData.property, targetCharaData.starData);

                //Set right
                List<CharaData> bufferList = new List<CharaData>();
                bool rejectFlag = false;
                for (int i = 0; i < GUICharacters.Instance.CharaDataList.Count; i++)
                {
                    rejectFlag = false;
                    for (int j = 0; j < this.fodderIdList.Count; j++)
                    {
                        if (GUICharacters.Instance.CharaDataList[i].chara.Info.UUID == this.fodderIdList[j])
                        {
                            rejectFlag = true;
                        }
                    }
                    if (rejectFlag == true)
                    {
                        continue;
                    }
                    bufferList.Add(GUICharacters.Instance.CharaDataList[i]);
                }
                GUICharacters.Instance.CharaDataList = bufferList;
                //Clear
                this.ClearFoddersData();
            };
        }
        #endregion

        #region Evolve
        public int NeedEvolveFodderCount
        {
            get
            {               
                if(currentCharaData.starId != 0)
                {
                    CharaStarMasterData data;
                    CharaStarMaster.Instance.TryGetMasterData(currentCharaData.starId, out data);
                    return data.StarUpCharacterCount;
                }else
                {
                    return 0;
                }
            }
        }

        private int evolveFodderCount = 0;
        public int EVOLVE_FODDER_COUNT
        {
            get { return evolveFodderCount; }
            set
            {
                evolveFodderCount = value;
                Debug.Log("EVOLVE_FODDER_COUNT: " + evolveFodderCount);

                var starId = currentCharaData.starId;
                if (evolveFodderCount == NeedEvolveFodderCount && evolveFodderCount != 0)
                {
                    starId = CharaStarMaster.Instance.GetNextStarId(currentCharaData.starId);
                }
                   
                this.UpdateTargetCharaData(starId, currentCharaData.lv, currentCharaData.exp, currentCharaData);
            }
        }

        private void SetEvolveLeft()
        {
            this.SetEvolvePropertyText();
            this.SetEvolveStarSlider((float)EVOLVE_FODDER_COUNT / this.NeedEvolveFodderCount);
        }

        private void SetEvolveStarSlider(float amount)
        {
            if(this.currentCharaData.stars >= this.STARS_LIMIT_NUM)
            {
                this.view.growView.EvolveView.lblTip.text = "已到达最高星级！";
                return;
            }

            if(this.NeedEvolveFodderCount == 0)
            {
                this.view.growView.EvolveView.lblTip.text = "等级未到，不能升星！";
                return;
            }

            if(this.EVOLVE_FODDER_COUNT < this.NeedEvolveFodderCount)
            {
                this.view.growView.EvolveView.lblTip.text = "还需要牺牲 " + (this.NeedEvolveFodderCount - this.EVOLVE_FODDER_COUNT) + " 个相同星级的该角色！";
            }
            else
            {
                this.view.growView.EvolveView.lblTip.text = "恭喜，可以升星了！";
            }
       }

        void SetEvolvePropertyText()
        {
            //Calc
            float curHP, curDEF, curATK, curSPD;
            this.CalcCurProperty(currentCharaData, out curHP, out curDEF, out curATK, out curSPD);

            if (targetCharaData.property != null && targetCharaData.stars > currentCharaData.stars)
            {
                float tarHP, tarDEF, tarATK, tarSPD;
                this.CalcTarProperty(targetCharaData, out tarHP, out tarDEF, out tarATK, out tarSPD);
                this.view.growView.EvolveView.hp.text = string.Format("{0:F2} [ff0000](+{1:F2})[-]", curHP, tarHP - curHP);
                this.view.growView.EvolveView.def.text = string.Format("{0:F2} [ff0000](+{1:F2})[-]", curDEF, tarDEF - curDEF);
                this.view.growView.EvolveView.atk.text = string.Format("{0:F2} [ff0000](+{1:F2})[-]", curATK, tarATK - curATK);
                this.view.growView.EvolveView.spd.text = string.Format("{0:F2} [ff0000](+{1:F2})[-]", curSPD, tarSPD - curSPD);
            }
            else
            {
                this.view.growView.EvolveView.hp.text = string.Format("{0:F2}", curHP);
                this.view.growView.EvolveView.def.text = string.Format("{0:F2}", curDEF);
                this.view.growView.EvolveView.atk.text = string.Format("{0:F2}", curATK);
                this.view.growView.EvolveView.spd.text = string.Format("{0:F2}", curSPD);
            }
        }

        struct SendPacketEvolve
        {
            public ulong uuidSelf;
            public ulong[] uuidsFodderArray;
            public Action<EvolutionRes> response;
        };
        SendPacketEvolve sendPacketEvolve;
        
        private void OnbtnEvolveSureClick()
        {
            this.InitSendPacketEvolve();
            StartCoroutine(Net.Network.EvolveGrow(sendPacketEvolve.uuidSelf, sendPacketEvolve.uuidsFodderArray, sendPacketEvolve.response));
        }

        void InitSendPacketEvolve()
        {
            sendPacketEvolve.uuidSelf = currentCharaData.uuid;
            sendPacketEvolve.uuidsFodderArray = this.fodderIdList.ToArray();
            sendPacketEvolve.response = (res) => {
                if(!res.Result)
                {
                    GUITipMessage.Instance.Show("升星失败！");
                    return;
                }

                //play special efficiency
                ParticleSystem ps = this.view.growView.EvolveView.afx.GetComponent<ParticleSystem>();
                if (ps != null)
                {
                    ps.Stop();
                    ps.Play();
                }

                //gold
                PlayerData.Instance.Gold = res.Money;

                //res.GetParam
                this.chara.UpdateInfo(new CharaInfo(res.GetParam()));

                //Set left
                this.UpdateCurrentCharaData(currentCharaData.uuid, targetCharaData.starId, targetCharaData.lv, targetCharaData.exp, targetCharaData.property, targetCharaData.starData);

                //Set right
                List<CharaData> bufferList = new List<CharaData>();
                bool rejectFlag = false;
                for (int i = 0; i < GUICharacters.Instance.CharaDataList.Count; i++)
                {
                    rejectFlag = false;
                    for (int j = 0; j < this.fodderIdList.Count; j++)
                    {
                        if (GUICharacters.Instance.CharaDataList[i].chara.Info.UUID == this.fodderIdList[j])
                        {
                            rejectFlag = true;
                        }
                    }
                    if (rejectFlag == true)
                    {
                        continue;
                    }
                    bufferList.Add(GUICharacters.Instance.CharaDataList[i]);
                }
                GUICharacters.Instance.CharaDataList = bufferList;

                //Clear
                this.ClearFoddersData();
            };
        }
        #endregion

        void EnableBtn(UIButton button, bool isEnable)
        {
            button.enabled = isEnable;
            button.defaultColor = isEnable == true ? Color.white : Color.gray;
        }

        void ClearFoddersData()
        {
            this.ADD_EXP_SUM = 0;
            this.EVOLVE_FODDER_COUNT = 0;
            this.UpgradePrice = 0;
            this.EvolvePrice = 0;
            this.fodderIdList.Clear();
        }

        void CalcCurProperty(CurrentCharaData charaData, out float hp, out float def, out float atk, out float spd)
        {
            hp = charaData.property.HitPoint * charaData.starData.HPBase;
            def = charaData.property.Defense * charaData.starData.DEFBase;
            atk = charaData.property.Attack * charaData.starData.ATKBase;
            spd = charaData.property.Speed * charaData.starData.SPDBase;
        }

        void CalcTarProperty(TargetCharaData charaData, out float hp, out float def, out float atk, out float spd)
        {
            hp = charaData.property.HitPoint * charaData.starData.HPBase;
            def = charaData.property.Defense * charaData.starData.DEFBase;
            atk = charaData.property.Attack * charaData.starData.ATKBase;
            spd = charaData.property.Speed * charaData.starData.SPDBase;
        }

        #endregion

        #region SkinView
        private void SetSkinView()
        {
            StartCoroutine(this.GetSkin());
        }

        IEnumerator GetSkin()
        {
            yield return this.chara.GetSkin((res) => {
                var skins = res.GetCharacterAvatarParameters();
                Net.Network.Instance.StartCoroutine(this.CreateSkinScrollAndSelectCurrent(skins));
            });
        }      

        IEnumerator CreateSkinScrollAndSelectCurrent(CharacterAvatarParameter[] skins)
        {
            if (this.skinCardScrollView == null)
            {
                this.skinCardScrollView = new CustomControl.ScrollPage<ItemSkinCard>(this.view.skinView.realView);
                
            }

            yield return this.skinCardScrollView.Create(this.view.skinView.itemSkinCard, skins);
            Debug.Log("CurrentSkinId: " + this.chara.Info.SkinId);

            AvatarMasterData avatar;
            MasterData.TryGetAvatar(this.chara.Info.CharacterMasterID, this.chara.Info.SkinId, out avatar);

            
            foreach (var skinItem in this.skinCardScrollView.GetScrollItems())
            {
                if (skinItem.skinInfo.Id == avatar.ID)
                {
                    skinItem.IsSelected = true;
                    this.skinCardScrollView.SelectHalf(skinItem.Index);
                }
                else
                {
                    skinItem.IsSelected = false;
                }
            }
        }

        private void SetCamera3D()
        {
            if (this.camera3D == null)
                this.camera3D = GameObject.Find("2_UI3DCamera").GetComponent<Camera>();
            this.camera3D.transform.localPosition = new Vector3(1.08f, 0.25f, -1.22f);
            camera3D.fieldOfView = 90;
            camera3D.rect = new Rect(0.685f, 0.24f, 0.245f, 0.59f);
        }

        private void ResetCamera3D()
        {
            if (this.camera3D != null)
            {
                this.camera3D.transform.localPosition = new Vector3(0.0f, 0.0f, -1.8f);
                camera3D.fieldOfView = 60;
                camera3D.rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
            }
       }

        public void SetPreview3D(CharaInfo charaInfo)
        {
            //this.view.skinView.portriat3D.DestroyChild();
            this.view.skinView.portriat3D.DestroyChildImmediate();
            CharaModel.Create(this.view.skinView.portriat3D, charaInfo);
            this.SetCamera3DDetail(charaInfo);
        }

        private void SetCamera3DDetail(CharaInfo info)
        {
            AvatarMasterData aData;
            MasterData.TryGetAvatar(info.CharacterMasterID, info.SkinId, out aData);
            
            //strArray[0] = localPosition.x, strArray[1] = localPosition.y, strArray[2] = localPosition.z
            //strArray[3] = localRotation.x, strArray[4] = localRotation.y, strArray[5] = Field of View
            var strArray = aData.CameraStr.Split(';');
            Debug.Log("str: " + aData.CameraStr);

            if(strArray.Length > 0)
            {
                this.camera3D.transform.localPosition = new Vector3(float.Parse(strArray[0]), float.Parse(strArray[1]), float.Parse(strArray[2]));
                this.camera3D.transform.localRotation = Quaternion.Euler(float.Parse(strArray[3]), float.Parse(strArray[4]), 0);
                this.camera3D.fieldOfView = float.Parse(strArray[5]);
            }
        }

        #endregion

        #region WallpaperView
        private void SetWallpaperView()
        {
            /*
            if(this.wallpaperScrollView == null)
            {
                this.wallpaperScrollView = new CustomControl.ScrollView<ItemWallPaper>(this.view.wallpaperView.grid, this.view.wallpaperView.itemWallpaper);
            }
            int[] test = { 1, 2, 3, 4, 5 };
            this.wallpaperScrollView.Create(test);
            this.CreateWallpaper();*/
            this.GetWallpaper();
        }

        private void GetWallpaper()
        {
            Net.Network.Instance.StartCoroutine(this.chara.GetWallpaper((res)=> {
                if (this.wallpaperScrollView == null)
                {
                    this.wallpaperScrollView = new CustomControl.ScrollView<ItemWallPaper>(this.view.wallpaperView.grid, this.view.wallpaperView.itemWallpaper);
                }
                var datas = res.GetCharacterWallpaperParameters();
                 this.wallpaperScrollView.CreateWeight(datas);
            }));
        }

        private void CreateWallpaper()
        {
            // 生成.
            AssetReference assetReference = AssetReference.GetAssetReference("wallpaper");
            StartCoroutine(assetReference.GetAssetAsync<TextAsset>("ui_iconA_p001_001.bytes", (res) =>
            {
                //GetAssetCallBack(resource, objectData, info, assetReference);
                //var texture = (Texture)resource;
                //Debug.Log("Texture: " + res.name);
                Texture2D t = new Texture2D(100, 100);
                t.LoadImage(res.bytes);

                Debug.Log("TTT: " + t.name);
            }
            ));
        }
        #endregion

        #region Tween
        public IEnumerator TweenForward()
        {
            this.StartTabMenuTween(TweenWay.Forward);
            yield return new WaitForSeconds(this.tabMenuTween.duration);
            this.StartRightTween(TweenWay.Forward);
            yield return new WaitForSeconds(this.rightTween.duration);
            TouchFilter.Instance.Enable(false);
        } 
        
        private IEnumerator TweenReverse()
        {
            TouchFilter.Instance.Enable(true);
            this.DeleteCharaListEvent();
            this.StartRightTween(TweenWay.Reverse);
            yield return new WaitForSeconds(this.rightTween.duration);
            this.StartTabMenuTween(TweenWay.Reverse);
            yield return new WaitForSeconds(this.tabMenuTween.duration);           
        }
        
        private void StartTabMenuTween(TweenWay way)
        {
            if (way == TweenWay.Forward)
                this.tabMenuTween.PlayForward();
            else
                this.tabMenuTween.PlayReverse();
        }

        private void StartRightTween(TweenWay way)
        {
            if (way == TweenWay.Forward)
                this.rightTween.PlayForward();
            else
                this.rightTween.PlayReverse();
        }
        #endregion

        #region Event
        private void registerEventOnce()
        {
            EventDelegate.Add(this.view.btnInfo.onClick, () => { this.SwitchTo(DetailPageType.Info); });
            EventDelegate.Add(this.view.btnGrow.onClick, () => {
                this.SetGrowView();
                this.SwitchTo(DetailPageType.Grow); });
            EventDelegate.Add(this.view.btnSkin.onClick, () => {                
                this.SetSkinView();
                this.SwitchTo(DetailPageType.Skin);
            });
            EventDelegate.Add(this.view.btnWallpaper.onClick, () => {
                this.SwitchTo(DetailPageType.Wallpaper);
                this.SetWallpaperView();
            });

            EventDelegate.Add(this.view.infoView.btnAbility.onClick, () => {
                this.InfoGoto(this.view.infoView.backView, this.view.infoView.abilityView);
                CustomControl.ToolFunc.BtnSwitchTo(this.view.infoView.btnAbility, this.infoTabList);
            });
            EventDelegate.Add(this.view.infoView.btnBack.onClick, () => {
                this.InfoGoto(this.view.infoView.abilityView, this.view.infoView.backView);
                CustomControl.ToolFunc.BtnSwitchTo(this.view.infoView.btnBack, this.infoTabList);
            });
            
            EventDelegate.Add(this.view.growView.btnUpgrade.onClick, () => { this.SwitchTo(GrowPageType.Upgrade); });
            EventDelegate.Add(this.view.growView.btnEvolve.onClick, () => { this.SwitchTo(GrowPageType.Evolve); });

            EventDelegate.Add(this.view.growView.UpgradeView.btnSure.onClick, () => { this.OnBtnUpgradeSureClick(); });
            EventDelegate.Add(this.view.growView.EvolveView.btnSure.onClick, () => { this.OnbtnEvolveSureClick(); });

            EventDelegate.Add(this.view.skinView.btnSure.onClick, () => {
                Net.Network.Instance.StartCoroutine(
                    Net.Network.ChangeSkin((long)this.chara.Info.UUID, SkinPreviewController.TrySkinId, (res) => {
                        if (res.Result == true)
                        {
                            this.chara.Info.SkinId = SkinPreviewController.TrySkinId;
                            GUICharacters.Instance.SetPortrait();
                            Debug.Log("ChangeSkin success!");
                            GUITipMessage.Instance.Show("皮肤更换成功！");
                            this.SetSkinBtnState(false, false, "使用中");
                        }
                    }));
            });

            EventDelegate.Add(this.view.skinView.btnShop.onClick, () => {
                Debug.Log("ToShop.");
            });
            
        }

        private void UpdateInfoProperty(object sender, EventArgs e)
        {
            this.SetInfoProperty();
        }

        private void UpdateEvolveBtnSureState(object sender, EventArgs e)
        {
            this.EnableBtn(this.view.growView.EvolveView.btnSure, this.targetCharaData.stars > this.currentCharaData.stars && this.EvolvePrice <= PlayerData.Instance.Gold);
        }

        private void UpdateUpgradeBtnSureState(object sender, EventArgs e)
        {
            this.EnableBtn(this.view.growView.UpgradeView.btnSure, this.targetCharaData.exp > this.currentCharaData.exp && this.UpgradePrice <= PlayerData.Instance.Gold);
        }

        private void UpdateEvolveLeftTarget(object sender, EventArgs e)
        {
            this.SetEvolveLeft();
        }

        private void UpdateUpgradeLeftTarget(object sender, EventArgs e)
        {
            this.SetUpgradeLeft();
        }

        private void UpdateEvolveLeft(object sender, EventArgs e)
        {
            this.SetEvolveLeft();
        }

        private void UpdateUpgradeLeft(object sender, EventArgs e)
        {
            this.SetUpgradeLeft();
        }
        
        #endregion
    }

}
