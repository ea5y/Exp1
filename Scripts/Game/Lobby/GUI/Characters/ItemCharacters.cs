using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Scm.Common.Master;

using XDATA;
namespace XUI
{
    public class ItemCharacters : CustomControl.ScrollViewItem
    {
        public List<ItemCharacter> characters
        {
            get
            {
                return this.cellItemList as List<ItemCharacter>;
            }
        }

        public List<CharaData> charasData;
        public List<CharaData> charasDataBuffer;

        CharaIcon CharaIcon { get { return ScmParam.Lobby.CharaIcon; } }

        public override void FillItem(IList datas, int index)
        {
            base.FillItem(datas, index);

            //Get characters
            if (this.cellItemList == null)
            {
                this.cellItemList = new List<ItemCharacter>();
                var charas = transform.GetComponentsInChildren<ItemCharacter>();
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if (charas[j].localIndex == i)
                        {
                            this.cellItemList.Add(charas[j]);
                            break;
                        }
                    }

                }
            }

            //Get charasData
            this.charasData = (List<CharaData>)datas[index];

            //Get charasDataBuffer
            this.charasDataBuffer = new List<CharaData>(charasData);

            for (int i = 0; i < this.cellItemList.Count; i++)
            {
                if (this.charasDataBuffer.Count != 0)
                {
                    //set data
                    this.characters[i].data = charasDataBuffer[0];

                    //set index
                    this.characters[i].index = charasDataBuffer[0].index;

                    //set localIndex
                    this.characters[i].localIndex = i;

                    //set type;
                    this.characters[i].charaType = charasDataBuffer[0].NormalType;

                    //set hook
                    this.characters[i].hook.gameObject.SetActive(this.charasDataBuffer[0].selectFlagNormal);
                    //this.characters[i].hook.gameObject.SetActive(false);

                    //set effect
                    //this.characters[i].effect.gameObject.SetActive(this.charasDataBuffer[0].selectFlagNormal);
                    this.characters[i].effect.gameObject.SetActive(false);

                    if (this.charasDataBuffer[0].selectFlagNormal)
                    {
                        this.characters[i].IsSelected = true;
                    }

                    //set icon
                    var testId = i;
                    this.characters[testId].icon.spriteName = "";
                    CharaIcon.GetIcon(charasDataBuffer[0].chara.Info.AvatarType, charasDataBuffer[0].chara.Info.SkinId, false,
                        (UIAtlas res, string name) =>
                        {
                            this.characters[testId].icon.atlas = res;
                            this.characters[testId].icon.spriteName = name;
                        });

                    //set lv
                    this.characters[i].lv.text = this.charasDataBuffer[0].chara.Info.Level + "";
                    this.characters[i].WillExpire = this.charasDataBuffer[0].chara.Info.TotalTime > 0;

                    //set star
                    int starsNum = CharaStarMaster.Instance.GetStarByID(this.charasDataBuffer[0].chara.Info.StarId);
                    if (starsNum != this.characters[i].starsGroup.transform.childCount)
                    {
                        CustomControl.ToolFunc.GridAddItemAndResponse(this.characters[i].starsGroup, this.characters[i].itemStar.gameObject, 14, starsNum);
                    }

                    //set uuid
                    this.characters[i].uuid = this.charasDataBuffer[0].chara.Info.UUID;

                    //set event
                    var item = this.characters[i];
                    var itemData = this.charasData[i];
                    this.characters[i].btn.onClick.Clear();
                    EventDelegate.Add(this.characters[i].btn.onClick, () => { this.OnCharaClick(item, itemData); });

                    //set tag
                    this.characters[i].tag.text = this.charasDataBuffer[0].index + "";

                    //set lock
                    this.characters[i].imgLock.gameObject.SetActive(this.charasDataBuffer[0].chara.Info.IsLock);

                    //Set flag
                    characters[i].imgFlag.gameObject.SetActive(charasDataBuffer[0].isInDeck);

                    //remove
                    this.charasDataBuffer.RemoveAt(0);
                    this.characters[i].gameObject.SetActive(true);
                }
                else
                {
                    this.characters[i].gameObject.SetActive(false);
                }
            }
        }

        public void OnCharaClick(ItemCharacter item, CharaData data)
        {
            Debug.Log("Click CharaIndex: " + data.index);
            if (GUICharacters.Instance.preChara != null)
            {
                GUICharacters.Instance.PreCharaData.selectFlagNormal = false;
                GUICharacters.Instance.preChara.IsSelected = false;
            }

            if (item.hook.gameObject.activeSelf == false)
                item.IsSelected = true;
            else
                item.IsSelected = false;
        }
    }
}