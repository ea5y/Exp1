using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Scm.Common.Master;
using XUI.LobbyResident;
using Scm.Common.Packet;

namespace XUI
{
    public class GUICharacterInf : Singleton<GUICharacterInf>
    {
        [SerializeField]
        public CharacterInfView view;
        public GameObject uiRoot;
        public UIButton buttonClose;
        public UIButton buttonHome;
        CharacterInfoCtl controller;
        Camera ui3DCamera = null;

        // Use this for initialization
        void Start()
        {
            EventDelegate.Add(buttonClose.onClick, Close);
            EventDelegate.Add(buttonHome.onClick, Close);
            Constrcut();
            FindCamera();
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

        static public void Open(int id)
        {
            if (Instance != null) Instance._Open(id);
        }
        void _Open(int id)
        {
            GUILobbyResident.SetActive(false);
            //GUIScreenTitle.Play(true, "角色");
            //GUIHelpMessage.Play(true, "help");
            /*{
                TopBottom.Instance.OnIn = () =>
                {
                    if (controller != null)
                    {
                        controller.Open(id);

                        InitData();
                    } 
                };
                TopBottom.Instance.OnBack = (v) =>
                {
                    Close();
                    v();
                };
                TopBottom.Instance.In();
            }*/
            controller.Open(id);
            InitData();
            if (ui3DCamera != null) ui3DCamera.rect = new Rect(0.11f, 0.10f, 0.50f, 0.78f);
        }

        IEnumerator AddUICenterOnChildOnFinished()
        {
            yield return 20;
            controller.AddUICenterOnChildOnFinished();
        }

        public void Close()
        {
            if (controller != null) controller.Close();
            GUILobbyResident.SetActive(true);
            if (ui3DCamera != null) ui3DCamera.rect = new Rect(0, 0, 1, 1);
        }

        void Constrcut()
        {
            var model = new CharacterInfoModel();
            controller = new CharacterInfoCtl(model, view);
        }

        void InitData()
        {
            LobbyPacket.SendPlayerCharacterAll(this.Response);

        }

        void Response(LobbyPacket.PlayerCharacterAllResArgs args)
        {

            GUISystemMessage.Close();
            controller.Init(args.List);
            StartCoroutine(AddUICenterOnChildOnFinished());
        }
    }
}

