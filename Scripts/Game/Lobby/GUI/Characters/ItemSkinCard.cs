using UnityEngine;
using System.Collections;
using Scm.Common.Packet;
using Scm.Common.Master;
using Scm.Common.GameParameter;

namespace XUI
{
    public class ItemSkinCard : CustomControl.ScrollView3DItem
    {
        public CharacterAvatarParameter skinInfo;

        public GameObject skin;
        public UISprite imgSkin;
        public UISprite bg;
        public UISprite highBg;
        public UIButton btn;

        public Material skinMask;

        private bool isFirstSelected = true;

        public bool isSelected;
        public bool IsSelected
        {
            get { return this.isSelected; }
            set
            {
                this.isSelected = value;

                this.highBg.gameObject.SetActive(this.isSelected);
                if (this.IsSelected == true)
                {                    
                    foreach (var item in GUICharacterDetial.Instance.skinCardScrollView.GetScrollItems())
                    {
                        if (item.Index != this.Index)
                            item.IsSelected = false;
                    }
                                       
                    Debug.Log("Preview!");
                    SkinPreviewController.Preview(this.skinInfo);
                }
            }
        }

        CharaIcon CharaIcon { get { return ScmParam.Lobby.CharaIcon; } }

        public override void Select()
        {
            base.Select();
            if (this.IsSelected == false)
            {
                this.IsSelected = !this.IsSelected;
            }
        }

        public override void Finish()
        {
            base.Finish();            
        }

        public override void FillItem(IList datas, int index)
        {
            this.Index = index;
            this.skinInfo = datas[index] as CharacterAvatarParameter;

            //Set skin
            Net.Network.Instance.StartCoroutine(this.SetUp());
        }

        IEnumerator SetUp()
        {
            yield return new WaitForFixedUpdate();
            AvatarMasterData aData;
            MasterData.TryGetAvatar(this.skinInfo.CharacterId, (int)this.skinInfo.Id, out aData);

            this.SetWidgets();
            this.SetFrame(aData.ShopItemTag);
            this.SetGrey();
               
            yield return this.SetSkinIcon(aData);
            GUICharacterDetial.Instance.skinCardScrollView.counter++;
        }

        private void SetWidgets()
        {
            this.widgets.Add(this.bg);
            this.widgets.Add(this.highBg);
            this.widgets.Add(this.imgSkin);
        }

        private void SetGrey()
        {
            if (this.skinInfo.Count < 1)
            {
                this.bg.color = Color.grey;
                this.highBg.color = Color.grey;
                this.imgSkin.color = Color.grey;
            }
        }

        private IEnumerator SetSkinIcon(AvatarMasterData aData)
        {
            bool isLoaded = false;
            CharaIcon.GetBustIcon((AvatarType)aData.CharacterId, aData.ID, false, (a, s) => {
                this.imgSkin.atlas = a;
                this.imgSkin.spriteName = s;
                isLoaded = true;
            });

            while (isLoaded == false)
                yield return null;
        }

        private void SetFrame(ShopItemTag tag)
        {
            if(tag == ShopItemTag.Light)
            {
                this.bg.spriteName = "kuang_b01";
                this.bg.width = 150;
                this.bg.height = 172;
                this.bg.transform.localPosition = new Vector3(6, 0, 0);

                this.highBg.spriteName = "kuang_y01";
                this.highBg.width = 150;
                this.highBg.height = 172;
                this.highBg.transform.localPosition = new Vector3(6, 0, 0);
            }
            else
            {
                this.bg.spriteName = "lankuang";
                this.bg.width = 142;
                this.bg.height = 170;
                this.bg.transform.localPosition = new Vector3(0, 0, 0);

                this.highBg.spriteName = "huangkuang";
                this.highBg.width = 142;
                this.highBg.height = 170;
                this.highBg.transform.localPosition = new Vector3(0, 0, 0);
            }
        }
    }
}

