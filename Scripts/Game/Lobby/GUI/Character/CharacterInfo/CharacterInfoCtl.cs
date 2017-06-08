using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Scm.Common.Master;
using System;

namespace XUI
{
    public class CharacterInfoCtl
    {

        public CharacterInfoModel model;
        public CharacterInfView view;
        ButtonInfoType _buttonType = CharacterInfoCtl.ButtonInfoType.Skill;

        public ButtonInfoType buttonInfoType
        {
            get { return _buttonType; }
            set { _buttonType = value; }
        }
        ButtonInfoType _curbuttonInfoType = CharacterInfoCtl.ButtonInfoType.Skill;

        public ButtonInfoType curbuttonInfoType
        {
            get { return _curbuttonInfoType; }
            set { _curbuttonInfoType = value; }
        }
        public enum ButtonInfoType
        {
            Skill,
            Back,
            RewardProgress,
            CharacterProgress,
        }

        public ButtonMode buttonMode;
        public enum ButtonMode
        {
            Info,
            Grow,
            Skin,
            Audio,
            Story,
        }
        int id;
        public CharacterInfoCtl(CharacterInfoModel model, CharacterInfView view)
        {
            if (model == null || view == null) { return; }

            this.view = view;
            this.view.OnButtonInfoClick += View_OnButtonInfoClick;
            this.view.OnButtonModeClick += View_OnButtonModeClick;
            this.view.OnButtonChangSkinClick += view_OnButtonChangSkinClick;
            //this.view.characterListView.uiCenterOnChild.onFinished += OnCenterFinishCharacterListViewCallback;
            this.view.characterListView.uiCenterOnChild.onCenter = OnCenterCharacterListViewCallback;
            //this.view.skinView.uiCenterOnChild.onFinished += OnCenterFinishSkinViewCallback;
            SkinItemView.OnSelected += SkinItemView_OnSelected;

            this.model = model;
            this.model.OnCharaInfoChange += Model_OnCharaInfoChange;
            this.model.OnCharacterNetInfoChange += Model_OnCharacterNetInfoChange;
        }

        public void Init(List<CharaInfo> charaInfoList)
        {
            buttonMode = ButtonMode.Info;
            buttonInfoType = ButtonInfoType.Skill;
            model.Setup(charaInfoList);
            view.Open();

            int index = 0;
            for (int i = 0; i < model.charaInfoList.Count; i++)
            {
                if (id == (int)model.charaInfoList[i].AvatarType)
                {
                    index = i;
                    break;
                }
            }
            float progress = 1.0f * index / Mathf.Max(1, model.charaInfoList.Count - 1);

            view.SetCharacterListView(model, progress);
            if (model.charaInfoList.Count > 0) model.Setup(model.charaInfoList[index]);

            view.characterListView.uiGride.Reposition();
        }

        public void Open(int id)
        {
            //view.characterListView.uiCenterOnChild.onFinished -= OnCenterFinishCharacterListViewCallback;
            this.id = id;
            view.Open();
        }

        public void AddUICenterOnChildOnFinished()
        {
            //view.characterListView.uiCenterOnChild.onFinished += OnCenterFinishCharacterListViewCallback;
        }

        public void Close()
        {
            view.Close();
        }

        GameObject curGo = null;
        public void OnCenterCharacterListViewCallback(GameObject go)
        {
            if (curGo == go) return;
            curGo = go;
            //Debug.Log("curGo.name:" + curGo.name);
            var GUICharacterMain = curGo.GetComponent<GUICharacterMain>();
            model.Setup(GUICharacterMain.charaInfo);
            if (buttonMode == ButtonMode.Skin)
            {
                model.SendGetCharacterAvatarAll();
            }
            else if (buttonMode == ButtonMode.Audio)
            {
                model.SendGetCharacterReplayVoiceAll();
            }
            else if (buttonMode == ButtonMode.Story)
            {
                model.SendGetCharacterStoryAll();
            }
        }

        public void OnCenterFinishCharacterListViewCallback()
        {
            if (view.characterListView.uiCenterOnChild.centeredObject == null) return;
            //Debug.Log("centeredObject.name2:" + view.characterListView.uiCenterOnChild.centeredObject.name);
            var GUICharacterMain = view.characterListView.uiCenterOnChild.centeredObject.GetComponent<GUICharacterMain>();
            model.Setup(GUICharacterMain.charaInfo);
            if (buttonMode == ButtonMode.Skin)
            {
                model.SendGetCharacterAvatarAll();
            }
            else if (buttonMode == ButtonMode.Audio)
            {
                model.SendGetCharacterReplayVoiceAll();
            }
            else if (buttonMode == ButtonMode.Story)
            {
                model.SendGetCharacterStoryAll();
            }
        }

        public void OnCenterFinishSkinViewCallback()
        {
            return;
            if (view.skinView.uiCenterOnChild.centeredObject == null || view.characterListView.uiCenterOnChild.centeredObject == null) return;
            var skinItemView = view.skinView.uiCenterOnChild.centeredObject.GetComponent<SkinItemView>();
            var gUICharacterMain = view.characterListView.uiCenterOnChild.centeredObject.GetComponent<GUICharacterMain>();
            gUICharacterMain.charaInfo.trySkinId = (int)skinItemView.characterAvatarParameter.Id;
            gUICharacterMain.Setup(gUICharacterMain.charaInfo);
            //view.EnableButtonUseSkin(gUICharacterMain.charaInfo.SkinId == gUICharacterMain.charaInfo.trySkinId);
            Debug.Log("skinItemView.characterAvatarParameter.Id:" + skinItemView.characterAvatarParameter.Id);
        }


        void View_OnButtonModeClick(object sender, System.EventArgs e)
        {
            var view = sender as CharacterInfView;
            buttonMode = view.buttonMode;
            if (buttonMode == ButtonMode.Grow)
            {
                //TODO
            }
            else if (buttonMode == ButtonMode.Skin)
            {
                model.SendGetCharacterAvatarAll();
            }
            else if (buttonMode == ButtonMode.Audio)
            {
                model.SendGetCharacterReplayVoiceAll();
            }
            else if (buttonMode == ButtonMode.Story)
            {
                model.SendGetCharacterStoryAll();
            }
            UpdateView();
        }


        void UpdateView()
        {
            view.DisableButtonModeView();
            view.SetName(model.charaInfo.Name);
            if (buttonMode == ButtonMode.Info)
            {
                view.EnableInfoView();
                UpdateInfoView();
            }
            else if (buttonMode == ButtonMode.Grow)
            {
                //view.SetGrowView(model);
            }
            else if (buttonMode == ButtonMode.Skin)
            {
                view.SetSkinView(model);
            }
            else if (buttonMode == ButtonMode.Audio)
            {
                view.SetAudioView(model);
            }
            else if (buttonMode == ButtonMode.Story)
            {
                view.SetStoryView(model);
            }
        }

        void UpdateInfoView()
        {
            view.DisableButtonInfoView();
            if (buttonInfoType == ButtonInfoType.Skill)
            {
                view.SetSkillView(model);
            }
            else if (buttonInfoType == ButtonInfoType.Back)
            {
                view.SetBackView(model);
            }
            else if (buttonInfoType == ButtonInfoType.RewardProgress)
            {
                view.SetRewardProgressView(model);
            }
            else if (buttonInfoType == ButtonInfoType.CharacterProgress)
            {
                view.SetCharacterProgressView(model);
            }
        }

        void Model_OnCharaInfoChange(object sender, System.EventArgs e)
        {
            var model = sender as CharacterInfoModel;
            this.model = model;
            if (model.charaInfo.LockFlag)
            {
                buttonInfoType = ButtonInfoType.CharacterProgress;
            }
            else if (buttonInfoType == ButtonInfoType.CharacterProgress)
            {
                buttonInfoType = ButtonInfoType.Skill;
            }
            view.EnableButtonInfoCollider(!model.charaInfo.LockFlag);
            UpdateView();
            UpdateInfoView();
        }

        void Model_OnCharacterNetInfoChange(object sender, System.EventArgs e)
        {
            var model = sender as CharacterInfoModel;
            this.model = model;
            if (buttonMode == ButtonMode.Skin)
            {
                view.SetSkinView(model);
            }
            else if (buttonMode == ButtonMode.Audio)
            {
                view.SetAudioView(model);
            }
            else if (buttonMode == ButtonMode.Story)
            {
                view.SetStoryView(model);
            }
        }

        void View_OnButtonInfoClick(object sender, System.EventArgs e)
        {
            var view = sender as XUI.CharacterInfView;
            //if (buttonType == view.buttonType) return;
            if (model.charaInfo.LockFlag)
            {
                buttonInfoType = curbuttonInfoType = ButtonInfoType.CharacterProgress;
            }
            else
            {
                buttonInfoType = curbuttonInfoType = view.buttonInfoType;
            }

            UpdateInfoView();
        }

        void SkinItemView_OnSelected(object sender, System.EventArgs e)
        {
            var skinItemView = sender as SkinItemView;
            var gUICharacterMain = view.characterListView.uiCenterOnChild.centeredObject.GetComponent<GUICharacterMain>();
            gUICharacterMain.charaInfo.trySkinId = (int)skinItemView.characterAvatarParameter.Id;
            gUICharacterMain.Setup(gUICharacterMain.charaInfo);
            view.SetOnCenterSkinView(skinItemView.characterAvatarParameter);
        }

        void view_OnButtonChangSkinClick(object sender, System.EventArgs e)
        {
            model.SendSetCurrentAvatar();
        }
    }
}


