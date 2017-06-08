using UnityEngine;
using System.Collections;
using Scm.Common.Master;
using Scm.Common.Packet;

namespace XUI
{
    public class SkinPreviewController : Singleton<SkinPreviewController>
    {
        public static CharaInfo charaInfo;
        public static CharacterAvatarParameter skinInfo;
        private static bool isPreviewing = false;
        private static int trySkinId = -1;
        public static int TrySkinId
        {
            get
            {
                return trySkinId;
            }
            set
            {
                trySkinId = value;
                if(charaInfo.SkinId == TrySkinId)
                {
                    //hide use and shop button               
                    GUICharacterDetial.Instance.SetSkinBtnState(false, false, "使用中");

                    return;
                }
                
                if (skinInfo.Count > 0)
                {
                    //hide btnShop show btnUse 
                    GUICharacterDetial.Instance.SetSkinBtnState(true, false);
                }else
                {
                    //hide btnUse show btnShop
                    GUICharacterDetial.Instance.SetSkinBtnState(false, true, "未拥有");
                }
            }
        }
        
        private void OnEnable()
        {
            charaInfo = GUICharacters.Instance.CurCharaData.chara.Info;
        }

        private void Update()
        {
            if (TrySkinId != -1 && isPreviewing == false)
            {
                isPreviewing = true;
                var info = new CharaInfo((AvatarType)charaInfo.CharacterMasterID, TrySkinId);
                GUICharacters.Instance.SetPreview(info, skinInfo);

                GUICharacterDetial.Instance.SetPreview3D(info);
            }                
        }

        private void OnDisable()
        {
            isPreviewing = false;
            TrySkinId = -1;
            GUICharacters.Instance.BackFromPreview();
        }

        public static void Preview(CharacterAvatarParameter info)
        {
            skinInfo = info;
            isPreviewing = false;
            TrySkinId = (int)info.Id;
        }
    }

}
