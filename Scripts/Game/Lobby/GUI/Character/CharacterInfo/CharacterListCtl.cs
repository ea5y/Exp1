using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Scm.Common.Master;

namespace XUI
{
    public class CharacterListCtl
    {

        public CharacterListModel model;
        public CharacterListView view;

        int index = 0;
        public CharacterListCtl(CharacterListModel model, CharacterListView view)
        {
            if (model == null || view == null) { return; }

            this.view = view;
            this.view.gridView.uiCenterOnChild.onFinished += OnCenterFinishCharacterListViewCallback;
            this.view.OnSelectCharaInfo += view_OnSelectCharaInfo;
            GUICharacterItem.OnSelectCharacterItem += GUICharacterItem_OnSelectCharacterItem;
            this.model = model;
            this.model.OnCharaInfoChange += Model_OnCharaInfoChange;
        }

        public void Setup(List<CharaInfo> charaInfoList)
        {
            model.Setup(charaInfoList);
            view.Setup(this.model);
            if (charaInfoList.Count > 0) model.Setup(charaInfoList[0]);
        }

        void view_OnSelectCharaInfo(object sender, System.EventArgs e)
        {
            GUICharacterList.Close();
            GUICharacterInf.Open((int)model.charaInfo.AvatarType);
        }

        void GUICharacterItem_OnSelectCharacterItem(object sender, System.EventArgs e)
        {
            var gUICharacterItem = sender as GUICharacterItem;
            model.Setup(gUICharacterItem.charaInfo);
        }

        public void Open()
        {
            view.Open();
        }

        public void Close()
        {
            view.Close();
        }

        void Model_OnCharaInfoChange(object sender, System.EventArgs e)
        {
            var model = sender as CharacterListModel;
            this.model = model;
            view.SetCharaInfo(this.model);
        }

        public void OnCenterFinishCharacterListViewCallback()
        {
            if (view.gridView.uiCenterOnChild.centeredObject == null) return;
            //Debug.Log("centeredObject.name2:" + view.gridView.uiCenterOnChild.centeredObject.name);
            var gUICharacterGroupItem = view.gridView.uiCenterOnChild.centeredObject.GetComponent<GUICharacterGroupItem>();
            view.SetPage(gUICharacterGroupItem.index);
        }
    }
}

