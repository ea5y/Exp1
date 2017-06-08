using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Scm.Common.Master;
using Scm.Common.Packet;
//using static LobbyPacket;
//using static LobbyPacket;

namespace XUI
{
    public class CharacterInfView : Singleton<CharacterInfView>
    {
        [System.Serializable]
        public class SkillView
        {
            public GameObject parent;
            public UISprite normal = null;
            public UISprite skill1 = null;
            public UISprite skill2 = null;
            public UISprite specialSkill = null;

            public UILabel normalName = null;
            public UILabel skill1Name = null;
            public UILabel skill2Name = null;
            public UILabel specialSkillName = null;

            public GameObject detail;
            public UILabel curSkillName;
            public UILabel curSkillDescription;
        }
        SkillIcon skillIcon { get { return ScmParam.Battle.SkillIcon; } }
        int SkillIconLv { get { return 1; } }

        [System.Serializable]
        public class BackView
        {
            public GameObject parent;
            public UILabel background = null;
        }

        [System.Serializable]
        public class RewardView
        {
            public GameObject parent;
            public GridView gridView;
            public UISprite expProgress;
            public UILabel curExp;
            public UILabel nextExp;
            public UILabel level;
        }

        [System.Serializable]
        public class GridView
        {
            public GameObject parent;
            public GameObject goPrefab = null;
            public GameObject goUIGrid = null;
            public UIGrid uiGride = null;
            public UIScrollBar uiScrollBar = null;
            public UIScrollView uiScrollView = null;
            public UICenterOnChild uiCenterOnChild = null;
            public List<GameObject> goList = new List<GameObject>();
        }

        [System.Serializable]
        public class CharacterPieceView
        {
            public GameObject parent;
            public UISprite back = null;
            public UISprite icon = null;
            public UISprite cover = null;
            public UILabel progress = null;
        }

        [System.Serializable]
        public class SkinChipView
        {
            public GameObject hasSkin;
            public GameObject hasChip;
            public UILabel heroName;
            public UILabel chipName1;
            public UILabel chipName2;
            public UISprite chipIcon;
            public UIProgressBar chipProgressValue;
            public UILabel chipProgressTitle;
        }

        public GridView characterListView;
        public GridView infoView;
        public GridView skinView;
        public GridView audioView;
        public GridView storyView;

        public SkillView skillView;
        public BackView backView;
        public RewardView reward;
        public CharacterPieceView characterPieceView;
        public SkinChipView skinChipView;

        public UIButton buttonSkill;
        public UIButton buttonBack;
        public UIButton buttonReward;

        public UIButton buttonInfo;
        public UIButton buttonGrow;
        public UIButton buttonSkin;
        public UIButton buttonAudio;
        public UIButton buttonStory;
        public GameObject body;
        public UILabel name;
        public GameObject charaType;

        CharacterInfoCtl.ButtonMode _buttonMode;

        public CharacterInfoCtl.ButtonMode buttonMode
        {
            get { return _buttonMode; }
            set { _buttonMode = value; }
        }
        /// <summary>
        /// 按钮类型：1--skill，2--back，3--progress
        /// </summary>
        CharacterInfoCtl.ButtonInfoType _buttonInfoType = CharacterInfoCtl.ButtonInfoType.Skill;

        public CharacterInfoCtl.ButtonInfoType buttonInfoType
        {
            get { return _buttonInfoType; }
            set
            {
                _buttonInfoType = value;
            }
        }

        bool _isLevelRewardLoaded = false;

        public bool isLevelRewardLoaded
        {
            get { return _isLevelRewardLoaded; }
            set { _isLevelRewardLoaded = value; }
        }

        List<LevelRewardView> levelRewardViewList = new List<LevelRewardView>();

        bool _isAudioViewLoaded = false;

        public bool isAudioViewLoaded
        {
            get { return _isAudioViewLoaded; }
            set { _isAudioViewLoaded = value; }
        }

        List<AudioItemView> audioItemViewList = new List<AudioItemView>();

        bool _isStoryViewLoaded = false;

        public bool isStoryViewLoaded
        {
            get { return _isStoryViewLoaded; }
            set { _isStoryViewLoaded = value; }
        }

        bool _isSkinViewLoaded = false;
        public bool isSkinViewLoaded
        {
            get { return _isSkinViewLoaded; }
            set { _isSkinViewLoaded = value; }
        }

        List<StoryItemView> storyItemViewList = new List<StoryItemView>();
        List<SkinItemView> skinItemViewList = new List<SkinItemView>();

        /// <summary>
        /// 基本信息按钮事件
        /// </summary>
        public event EventHandler OnButtonInfoClick = (sender, e) => { };
        /// <summary>
        /// 皮肤、声音、情景等按钮事件
        /// </summary>
        public event EventHandler OnButtonModeClick = (sender, e) => { };
        /// <summary>
        /// 更换皮肤按钮事件
        /// </summary>
        public event EventHandler OnButtonChangSkinClick = (sender, e) => { };

        // Use this for initialization
        void Start()
        {
            EventDelegate.Add(buttonSkill.onClick, OnSkill);
            EventDelegate.Add(buttonBack.onClick, OnBack);
            EventDelegate.Add(buttonReward.onClick, OnReward);
            EventDelegate.Add(buttonInfo.onClick, OnInfo);
            EventDelegate.Add(buttonGrow.onClick, OnGrow);
            EventDelegate.Add(buttonSkin.onClick, OnSkin);
            EventDelegate.Add(buttonAudio.onClick, OnAudio);
            EventDelegate.Add(buttonStory.onClick, OnStory);
            if (skinView.goList.Count > 1)
            {
                EventDelegate.Add(skinView.goList[0].GetComponent<UIButton>().onClick, OnUseSkin);
                EventDelegate.Add(skinView.goList[1].GetComponent<UIButton>().onClick, OnBuySkin);
            }
        }

        public void Open()
        {
            body.SetActive(true);
            buttonSkill.normalSprite = "shubiaoqianhuang";
            buttonBack.normalSprite = "shubiaoqianlan";
            buttonReward.normalSprite = "shubiaoqianlan";
            buttonInfo.normalSprite = "huangbiaoqian";
            buttonGrow.normalSprite = "lanbiaoqian";
            buttonSkin.normalSprite = "lanbiaoqian";
            buttonAudio.normalSprite = "lanbiaoqian";
            buttonSkill.GetComponentInChildren<UILabel>().color = Color.black;
            buttonBack.GetComponentInChildren<UILabel>().color = Color.white;
            buttonReward.GetComponentInChildren<UILabel>().color = Color.white;

            buttonMode = CharacterInfoCtl.ButtonMode.Info;
            buttonInfoType = CharacterInfoCtl.ButtonInfoType.Skill;
        }

        public void Close()
        {
            body.SetActive(false);
            DestroyListChildren();
        }

        public void DisableButtonInfoView()
        {
            skillView.parent.SetActive(false);
            backView.parent.SetActive(false);
            reward.parent.SetActive(false);
            characterPieceView.parent.SetActive(false);
        }

        public void DisableButtonModeView()
        {
            infoView.parent.SetActive(false);
            skinView.parent.SetActive(false);
            audioView.parent.SetActive(false);
            storyView.parent.SetActive(false);
        }

        public void EnableInfoView()
        {
            infoView.parent.SetActive(true);
        }

        public void SetName(string title)
        {
            name.text = title;
        }

        public void EnableButtonInfoCollider(bool enabled)
        {
            buttonSkill.GetComponent<BoxCollider>().enabled = enabled;
            buttonBack.GetComponent<BoxCollider>().enabled = enabled;
            buttonReward.GetComponent<BoxCollider>().enabled = enabled;
        }

        public void SetSkillView(CharacterInfoModel characterInfoModel)
        {
            skillView.parent.SetActive(true);
            CharaLevelMasterData charaLv;
            if (MasterData.TryGetCharaLv((int)characterInfoModel.charaInfo.AvatarType, this.SkillIconLv, out charaLv))
            {
                CharaButtonSetMasterData buttonSet;
                if (MasterData.TryGetCharaButtonSet(charaLv, out buttonSet))
                {
                    SetSkillIcon(skillIcon, buttonSet, this.SkillIconLv);
                    skillView.normalName.text = buttonSet.AttackButton.ButtonName;
                    skillView.skill1Name.text = buttonSet.Skill1Button.ButtonName;
                    skillView.skill2Name.text = buttonSet.Skill2Button.ButtonName;
                    skillView.specialSkillName.text = buttonSet.SpecialSkillButton.ButtonName;
                }
            }
        }

        void SetSkillIcon(SkillIcon skillIcon, CharaButtonSetMasterData data, int lv)
        {
            if (skillIcon == null)
                return;

            var t = skillView;
            if (t.normal != null)
                skillIcon.GetSkillIcon(data, lv, SkillButtonType.Normal, false, (atlas, spriteName) => { this.SetSkillIcon(atlas, spriteName, t.normal); });
            if (t.skill1 != null)
                skillIcon.GetSkillIcon(data, lv, SkillButtonType.Skill1, false, (atlas, spriteName) => { this.SetSkillIcon(atlas, spriteName, t.skill1); });
            if (t.skill2 != null)
                skillIcon.GetSkillIcon(data, lv, SkillButtonType.Skill2, false, (atlas, spriteName) => { this.SetSkillIcon(atlas, spriteName, t.skill2); });
            if (t.specialSkill != null)
                skillIcon.GetSkillIcon(data, lv, SkillButtonType.SpecialSkill, false, (atlas, spriteName) => { this.SetSkillIcon(atlas, spriteName, t.specialSkill); });
            //if (t.technicalSkill != null)
            //    skillIcon.GetSkillIcon(data, lv, SkillButtonType.TechnicalSkill, false, (atlas, spriteName) => { this.SetSkillIcon(atlas, spriteName, t.technicalSkill); });
        }

        void SetSkillIcon(UIAtlas atlas, string spriteName, UISprite sp)
        {
            if (sp == null)
                return;

            // アトラス設定
            sp.atlas = atlas;
            // スプライト設定
            sp.spriteName = spriteName;

            // アトラス内にアイコンが含まれているかチェック
            if (sp.GetAtlasSprite() == null)
            {
                // アトラスとスプライト名が両方共設定されていてスプライトがない場合はエラー扱いｌ
                if (atlas != null && !string.IsNullOrEmpty(spriteName))
                {
                    Debug.LogWarning(string.Format(
                        "SetIconSprite:\r\n" +
                        "Sprite Not Found!! AvatarType = {0} SpriteName = {1}", 1, spriteName));//this.CharaInfo.AvatarType
                }
            }
        }

        public void SetBackView(CharacterInfoModel characterInfoModel)
        {
            backView.parent.SetActive(true);
            backView.background.text = characterInfoModel.charaProfileMasterData != null ? characterInfoModel.charaProfileMasterData.background : "无数据";
        }

        public void SetRewardProgressView(CharacterInfoModel characterInfoModel)
        {
            reward.parent.SetActive(true);
            int level = characterInfoModel.charaInfo.Level == 0 ? 1 : characterInfoModel.charaInfo.Level;
            CharaLevelMasterData data;
            MasterData.TryGetCharaLevelMasterData((int)characterInfoModel.charaInfo.AvatarType, level, out data);

            reward.level.text = level.ToString();
            reward.curExp.text = (characterInfoModel.charaInfo.Exp + 20).ToString();
            reward.nextExp.text = data != null ? data.NextExp.ToString() : "?";
            reward.expProgress.fillAmount = data != null ? (characterInfoModel.charaInfo.Exp + 20) * 1.0f / data.NextExp : 0.5f;

            if (!isLevelRewardLoaded)
            {
                isLevelRewardLoaded = true;
                levelRewardViewList.Clear();
                foreach (var item in characterInfoModel.CharaLevelMasterDataList)
                {
                    var go = NGUITools.AddChild(reward.gridView.goUIGrid, reward.gridView.goPrefab);
                    var levelRewardView = go.GetComponent<LevelRewardView>();
                    levelRewardViewList.Add(levelRewardView);
                    levelRewardView.Setup(item, (int)characterInfoModel.charaInfo.AvatarType);
                }
            }
            else if (levelRewardViewList.Count == characterInfoModel.CharaLevelMasterDataList.Count)
            {
                for (int i = 0; i < levelRewardViewList.Count; i++)
                {
                    var levelRewardView = levelRewardViewList[i];
                    levelRewardView.Setup(characterInfoModel.CharaLevelMasterDataList[i], (int)characterInfoModel.charaInfo.AvatarType);
                }
            }
            reward.gridView.uiGride.Reposition();
            reward.gridView.uiScrollView.ResetPosition();
        }

        public void SetCharacterProgressView(CharacterInfoModel characterInfoModel)
        {
            characterPieceView.parent.SetActive(true);
            //characterProgressView.icon.spriteName = characterInfoModel.charaProfileMasterData.UnlockPieces.ToString();
            characterPieceView.progress.text = characterInfoModel.charaInfo.DeckSlotIndex.ToString();

        }

        public void SetCharacterListView(CharacterInfoModel characterInfoModel, float progress)
        {
            int index = 0;
            foreach (var item in characterInfoModel.charaInfoList)
            {
                var go = NGUITools.AddChild(characterListView.goUIGrid, characterListView.goPrefab);
                var gUICharacterMain = go.GetComponent<GUICharacterMain>();
                gUICharacterMain.Setup(item);
                //            go.transform.localPosition = new Vector3(800 * index, 0, 0);
                index++;
            }
            //characterListView.uiGride.Reposition();
            //characterListView.uiScrollView.ResetPosition();
            StartCoroutine(SetCharacterListViewUIScrollBar(progress));
        }

        IEnumerator SetCharacterListViewUIScrollBar(float progress)
        {
            yield return 1;
            characterListView.uiScrollBar.value = progress;
            yield return 1;
            characterListView.uiCenterOnChild.Recenter();
        }


        public void SetSkinView(CharacterInfoModel characterInfoModel)
        {
            skinView.goUIGrid.DestroyChild();
            skinView.parent.SetActive(true);
            bool isFirst = true;
            foreach (var item in characterInfoModel.characterAvatarParameterList)
            {
                var go = NGUITools.AddChild(skinView.goUIGrid, skinView.goPrefab);
                var skinItemView = go.GetComponent<SkinItemView>();
                skinItemView.Setup(characterInfoModel.charaInfo, item, isFirst);
                isFirst = false;
            }

            if (characterInfoModel.characterAvatarParameterList.Count > 0)
            {
                CharacterAvatarParameter characterAvatarParameter = null;
                foreach (var item in characterInfoModel.characterAvatarParameterList)
                {
                    if (item.Id == characterInfoModel.charaInfo.trySkinId)
                    {
                        characterAvatarParameter = item;
                        break;
                    }
                }
                if (characterAvatarParameter == null) characterAvatarParameter = characterInfoModel.characterAvatarParameterList[0];
                SetOnCenterSkinView(characterAvatarParameter);
            }
            skinView.uiGride.Reposition();
            skinView.uiScrollView.ResetPosition();
        }

        public void SetOnCenterSkinView(CharacterAvatarParameter characterAvatarParameter)
        {
            var gUICharacterMain = characterListView.uiCenterOnChild.centeredObject.GetComponent<GUICharacterMain>();
            if (characterAvatarParameter.Count > 0)
            {
                skinView.goList[0].SetActive(true);
                skinView.goList[1].SetActive(false);
                skinChipView.hasSkin.SetActive(true);
                skinChipView.hasChip.SetActive(false);
                skinChipView.chipName1.text = characterAvatarParameter.Name;
                skinChipView.heroName.text = gUICharacterMain.charaInfo.Name;
            }
            else
            {
                skinView.goList[0].SetActive(false);
                skinView.goList[1].SetActive(true);
                skinChipView.hasSkin.SetActive(false);
                skinChipView.hasChip.SetActive(true);
                skinChipView.chipProgressTitle.text = characterAvatarParameter.PieceCount + "/" + characterAvatarParameter.UnlockPieceCount;
                skinChipView.chipProgressValue.value = 1.0f * characterAvatarParameter.PieceCount / characterAvatarParameter.UnlockPieceCount;
                skinChipView.chipName2.text = characterAvatarParameter.Name;
                charaIcon.GetIcon(gUICharacterMain.charaInfo.AvatarType, gUICharacterMain.charaInfo.SkinId, false, this.SetIconSprite);
            }
        }
        CharaIcon charaIcon { get { return ScmParam.Lobby.CharaIcon; } }
        void SetIconSprite(UIAtlas atlas, string spriteName)
        {
            skinChipView.chipIcon.atlas = atlas;
            skinChipView.chipIcon.spriteName = spriteName;
        }

        public void SetAudioView(CharacterInfoModel characterInfoModel)
        {
            audioView.parent.SetActive(true);
            if (!isAudioViewLoaded || audioItemViewList.Count == 0)
            {
                isAudioViewLoaded = true;
                audioItemViewList.Clear();
                foreach (var item in characterInfoModel.replayVoiceParameterList)
                {
                    var go = NGUITools.AddChild(audioView.goUIGrid, audioView.goPrefab);
                    var audioItemView = go.GetComponent<AudioItemView>();
                    audioItemViewList.Add(audioItemView);
                    audioItemView.Setup(characterInfoModel.charaInfo, item);
                }
            }
            else if (audioItemViewList.Count == characterInfoModel.replayVoiceParameterList.Count)
            {
                for (int i = 0; i < audioItemViewList.Count; i++)
                {
                    var audioItemView = audioItemViewList[i];
                    audioItemView.Setup(characterInfoModel.charaInfo, characterInfoModel.replayVoiceParameterList[i]);
                }
            }
            audioView.uiGride.Reposition();
            audioView.uiScrollView.ResetPosition();

        }

        public void SetStoryView(CharacterInfoModel characterInfoModel)
        {
            storyView.parent.SetActive(true);
            if (!isStoryViewLoaded || storyItemViewList.Count == 0)
            {
                isStoryViewLoaded = true;
                storyItemViewList.Clear();
                foreach (var item in characterInfoModel.characterStoryParameterList)
                {
                    var go = NGUITools.AddChild(storyView.goUIGrid, storyView.goPrefab);
                    var storyItemView = go.GetComponent<StoryItemView>();
                    storyItemViewList.Add(storyItemView);
                    storyItemView.Setup(characterInfoModel.charaInfo, item);
                }
            }
            else if (characterInfoModel.characterStoryParameterList.Count == storyItemViewList.Count)
            {
                for (int i = 0; i < storyItemViewList.Count; i++)
                {
                    var storyItemView = storyItemViewList[i];
                    storyItemView.Setup(characterInfoModel.charaInfo, characterInfoModel.characterStoryParameterList[i]);
                }
            }
            storyView.uiGride.Reposition();
            storyView.uiScrollView.ResetPosition();
        }

        void OnSkill()
        {
            buttonInfoType = CharacterInfoCtl.ButtonInfoType.Skill;
            OnButtonInfoClick(this, EventArgs.Empty);
            buttonSkill.normalSprite = "shubiaoqianhuang";
            buttonBack.normalSprite = "shubiaoqianlan";
            buttonReward.normalSprite = "shubiaoqianlan";
            buttonSkill.GetComponentInChildren<UILabel>().color = Color.black;
            buttonBack.GetComponentInChildren<UILabel>().color = Color.white;
            buttonReward.GetComponentInChildren<UILabel>().color = Color.white;
        }

        void OnBack()
        {
            buttonInfoType = CharacterInfoCtl.ButtonInfoType.Back;
            OnButtonInfoClick(this, EventArgs.Empty);
            buttonSkill.normalSprite = "shubiaoqianlan";
            buttonBack.normalSprite = "shubiaoqianhuang";
            buttonReward.normalSprite = "shubiaoqianlan";
            buttonSkill.GetComponentInChildren<UILabel>().color = Color.white;
            buttonBack.GetComponentInChildren<UILabel>().color = Color.black;
            buttonReward.GetComponentInChildren<UILabel>().color = Color.white;
        }

        void OnReward()
        {
            buttonInfoType = CharacterInfoCtl.ButtonInfoType.RewardProgress;
            OnButtonInfoClick(this, EventArgs.Empty);
            buttonSkill.normalSprite = "shubiaoqianlan";
            buttonBack.normalSprite = "shubiaoqianlan";
            buttonReward.normalSprite = "shubiaoqianhuang";
            buttonSkill.GetComponentInChildren<UILabel>().color = Color.white;
            buttonBack.GetComponentInChildren<UILabel>().color = Color.white;
            buttonReward.GetComponentInChildren<UILabel>().color = Color.black;
        }

        void OnInfo()
        {
            buttonMode = CharacterInfoCtl.ButtonMode.Info;
            OnButtonModeClick(this, EventArgs.Empty);
            buttonInfoType = CharacterInfoCtl.ButtonInfoType.Skill;
            OnButtonInfoClick(this, EventArgs.Empty);
            buttonSkill.normalSprite = "shubiaoqianhuang";
            buttonBack.normalSprite = "shubiaoqianlan";
            buttonReward.normalSprite = "shubiaoqianlan";
            buttonInfo.normalSprite = "huangbiaoqian";
            buttonGrow.normalSprite = "lanbiaoqian";
            buttonSkin.normalSprite = "lanbiaoqian";
            buttonAudio.normalSprite = "lanbiaoqian";
            buttonSkill.GetComponentInChildren<UILabel>().color = Color.black;
            buttonBack.GetComponentInChildren<UILabel>().color = Color.white;
            buttonReward.GetComponentInChildren<UILabel>().color = Color.white;
        }

        void OnGrow()
        {
            buttonMode = CharacterInfoCtl.ButtonMode.Grow;
            OnButtonModeClick(this, EventArgs.Empty);
            buttonGrow.normalSprite = "huangbiaoqian";
            buttonInfo.normalSprite = "lanbiaoqian";
            buttonSkin.normalSprite = "lanbiaoqian";
            buttonAudio.normalSprite = "lanbiaoqian";
        }

        void OnSkin()
        {
            buttonMode = CharacterInfoCtl.ButtonMode.Skin;
            OnButtonModeClick(this, EventArgs.Empty);
            buttonInfo.normalSprite = "lanbiaoqian";
            buttonGrow.normalSprite = "lanbiaoqian";
            buttonSkin.normalSprite = "huangbiaoqian";
            buttonAudio.normalSprite = "lanbiaoqian";
        }

        void OnAudio()
        {
            buttonMode = CharacterInfoCtl.ButtonMode.Audio;
            OnButtonModeClick(this, EventArgs.Empty);
            buttonInfo.normalSprite = "lanbiaoqian";
            buttonGrow.normalSprite = "lanbiaoqian";
            buttonSkin.normalSprite = "lanbiaoqian";
            buttonAudio.normalSprite = "huangbiaoqian";
        }

        void OnStory()
        {
            buttonMode = CharacterInfoCtl.ButtonMode.Story;
            OnButtonModeClick(this, EventArgs.Empty);
        }

        void OnUseSkin()
        {
            OnButtonChangSkinClick(this, EventArgs.Empty);
        }

        void OnBuySkin()
        {

        }

        public void EnableButtonUseSkin(bool isEnable)
        {
            if (skinView.goList.Count > 1)
            {
                skinView.goList[0].GetComponent<UIButton>().enabled = isEnable;
            }
        }

        public void DestroyListChildren()
        {
            characterListView.goUIGrid.transform.DestroyChildren();
        }

    }
}



