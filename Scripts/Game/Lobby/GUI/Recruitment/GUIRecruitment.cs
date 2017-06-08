using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Asobimo.WebAPI;
using Scm.Common.Master;
using Scm.Common.XwMaster;
using Scm.Common.GameParameter;
using Scm.Common.NGWord;

namespace XUI
{
    /// <summary>
    /// Recruitment manager
    /// </summary>
    public class GUIRecruitment : Singleton<GUIRecruitment>
    {

        protected override void Awake() {
            base.Awake();
        }

        /// <summary>
        /// Called when recruitment button is clicked.
        /// > when recruitment is shown: unpublish it
        /// > otherwise: popup a window to show the recruitment content and request publishing
        /// </summary>
        public void OnRecruitmentClick() {
            if (Entrant.GetRecruitment(NetworkController.ServerValue.InFieldId) != null) {
                // already publishing
                GUIMessageWindow.SetModeYesNo(MasterData.GetText(TextType.TX613_RecruitmentCancelConfirm),
                    () => {
                        GUIMessageWindow.Close();
                        NetworkController.SendRecruitment(false, string.Empty);
                    },
                    () => {
                        GUIMessageWindow.Close();
                    });
            } else {
                GUIMessageWindow.InputCharacterLimit = 16;
                string text = MasterData.GetText(TextType.TX610_RecruitmentDefaultText);

                Action send = () => {
                    string newText = ApplicationController.Language == Language.Japanese ? NGWord.DeleteNGWord(text) : FilterWordController.Instance.GetFilteredWord(text);
                    NetworkController.SendRecruitment(true, newText);
                    GUIMessageWindow.Close();
                };

                GUIMessageWindow.SetModeInput(MasterData.GetText(TextType.TX609_RecruitmentTextHint),
                      text, 
                      text, false,
                      () => {
                          send();
                      },
                      () => { GUIMessageWindow.Close(); }, 
                      (input) => {
                          text = input;
                          send();
                      }, (input) => { text = input; });
            }
        }

        public void OnRecruitmentItemClick(int inFieldId) {
            var recruitment = Entrant.GetRecruitment(inFieldId);
            EntrantInfo entrant;
            Entrant.TryGetEntrant(inFieldId, out entrant);
            if (recruitment == null || entrant == null) {
                return;
            }

            string text = MasterData.GetText(TextType.TX611_RecruitmentConfirmJoinTeam, new string[] { entrant.UserName });
            GUISystemMessage.SetModeYesNo(MasterData.GetText(TextType.TX612_RecruitmentConfirmTitle), text, 
                () => {
                    GUISystemMessage.Close();
                    CommonPacket.SendJoinTeamReq((int)recruitment.TeamId);
                },
                () => {
                    GUISystemMessage.Close();
                });
        }
    }
    
}
