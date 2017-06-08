using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Scm.Common.Master;
using XUI.LobbyResident;
using Scm.Common.Packet;

namespace XUI
{
    public class GUICharacterList : Singleton<GUICharacterList>
    {
        [SerializeField]
        public CharacterListView view;
        public UIButton buttonClose;
        public UIButton buttonHome;
        CharacterListCtl controller;
        Camera ui3DCamera = null;

        protected override void Awake()
        {
            base.Awake();
            gameObject.SetActive(false);
            Constrcut();
            EventDelegate.Add(buttonClose.onClick, _Close);
            EventDelegate.Add(buttonHome.onClick, _Close);
        }

        // Use this for initialization
        void Start()
        {
            FindCamera();
            InitData();
        }

        void FindCamera()
        {
            var cameras = GameObject.FindObjectsOfType<Camera>();
            foreach (var item in cameras)
            {
                if (item.gameObject.name == "2_UI3DCamera")
                {
                    ui3DCamera = item;
                    break;
                }
            }
        }

        static public void Open()
        {
            if (Instance != null) Instance._Open();
        }
        void _Open()
        {
            TopBottom.Instance.OnIn = () =>
            {
                //RadarController.Open();
                GUILobbyResident.SetActive(false);
                if (controller != null) controller.Open();
                InitData();
            };
            TopBottom.Instance.OnBack = (v) =>
            {
                _Close();
                v();
            };
            TopBottom.Instance.In();

            if (ui3DCamera != null) ui3DCamera.rect = new Rect(0, 0, 1, 1);
        }

        static public void Close()
        {
            TopBottom.Instance.Back();
        }

        private void _Close()
        {
            if (controller != null) controller.Close();
            GUILobbyResident.SetActive(true);
            //RadarController.Close();
        }

        void Constrcut()
        {
            var model = new CharacterListModel();
            controller = new CharacterListCtl(model, view);
        }

        void InitData()
        {
            Debug.Log("GetAll");
            LobbyPacket.SendPlayerCharacterAll(this.Response);
        }

        void Response(LobbyPacket.PlayerCharacterAllResArgs args)
        {
            if (args.List == null || args.List.Count == 0) return;
            GUISystemMessage.Close();

            controller.Setup(args.List);
        }
    }
}


