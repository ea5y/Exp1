using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Scm.Common.Master;
using XDATA;

namespace XUI
{
    public class ItemEvolve : CustomControl.ScrollViewItem
    {
        public List<ItemCharacter> characters
        {
            get { return this.cellItemList as List<ItemCharacter>; }
        }

        public List<CharaData> charasData;
        public List<CharaData> charasDataBuffer;
        CharaIcon CharaIcon { get { return ScmParam.Lobby.CharaIcon; } }

        void OnCharacterClick(GameObject obj, int index)
        {
            Debug.Log("Click!");
            var character = obj.GetComponent<ItemCharacter>();
            if (character.index == index)
            {
                Debug.Log("index: " + index);
                Debug.Log("Selected: " + character.IsSelected);
                if (character.hook.gameObject.activeSelf == false)
                    character.IsSelected = true;
                else
                    character.IsSelected = false;
                Debug.Log("Selected: " + character.IsSelected);
            }
        }

        public override void FillItem(IList datas, int index)
        {

            base.FillItem(datas, index);

            //Get characters
            if (this.cellItemList == null)
            {
                this.cellItemList = new List<ItemCharacter>();
                var charas = transform.GetComponentsInChildren<ItemCharacter>();
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        if (charas[j].localIndex == i)
                        {
                            this.cellItemList.Add(charas[j]);
                            break;
                        }
                    }

                }
            }


            {
                charasData = new List<CharaData>();
                List<CharaData> temp = (List<CharaData>)datas[index];
                for (int i = 0; i < 3; i++)
                {
                    if (i < temp.Count)
                        charasData.Add(temp[i]);
                }
            }

            /*
            if (characters == null)
            {
                characters = new List<ItemCharacter>();
                var charas = transform.GetComponentsInChildren<ItemCharacter>();
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        if (charas[j].localIndex == i)
                        {
                            characters.Add(charas[j]);
                            break;
                        }
                    }

                }
            }*/


            charasDataBuffer = new List<CharaData>(charasData);

            for (int j = 0; j < 3; j++)
            {
                if (charasDataBuffer.Count != 0)
                {
                    //set data
                    characters[j].data = charasDataBuffer[0];

                    //set index
                    characters[j].index = charasDataBuffer[0].index;

                    //set localIndex
                    characters[j].localIndex = j;

                    //set fodder type
                    characters[j].charaType = charasDataBuffer[0].EvolveType;

                    //Set hook
                    characters[j].hook.gameObject.SetActive(charasDataBuffer[0].selectFlagEvolve);

                    //set icon
                    CharaIcon.GetIcon(charasDataBuffer[0].chara.Info.AvatarType, charasDataBuffer[0].chara.Info.SkinId, false,
                        (UIAtlas res, string name) => {
                            characters[j].icon.atlas = res;
                            characters[j].icon.spriteName = name;
                        });

                    //Set lv
                    characters[j].lv.text = charasDataBuffer[0].chara.Info.Level + "";
                    characters[j].WillExpire = this.charasDataBuffer[0].chara.Info.TotalTime > 0;

                    //Set lock
                    characters[j].imgLock.gameObject.SetActive(charasDataBuffer[0].chara.Info.IsLock);

                    //Set flag
                    characters[j].imgFlag.gameObject.SetActive(charasDataBuffer[0].isInDeck);

                    //set star
                    int starsNum = CharaStarMaster.Instance.GetStarByID(charasDataBuffer[0].chara.Info.StarId);
                    if(starsNum != this.characters[j].starsGroup.transform.childCount)
                    {
                        CustomControl.ToolFunc.GridAddItemAndResponse(this.characters[j].starsGroup, this.characters[j].itemStar.gameObject, 14, starsNum);
                    }
                    /*
                    //init star
                    for (int y = 0; y < 6; y++)
                    {
                        characters[j].stars[y].gameObject.SetActive(false);
                    }
                    //update
                    for (int k = 0; k < stars; k++)
                    {
                        characters[j].stars[k].gameObject.SetActive(true);
                    }
                    */

                    //set addExp
                    CharaStarMasterData foodData;
                    MasterData.TryGetCharaStarMasterData(charasDataBuffer[0].chara.Info.StarId, out foodData);
                    characters[j].addExp = foodData.ProvideExp;

                    //set cost
                    characters[j].costGold = foodData.StarUpCostGold;

                    //set uuid
                    characters[j].uuid = charasDataBuffer[0].chara.Info.UUID;

                    var character = characters[j].gameObject;
                    var idx = characters[j].index;
                    //Set event
                    characters[j].btn.onClick.Clear();
                    EventDelegate.Add(characters[j].btn.onClick, () => { OnCharacterClick(character, idx); });


                    //remove
                    charasDataBuffer.RemoveAt(0);
                    characters[j].gameObject.SetActive(true);
                }
                else
                {
                    characters[j].gameObject.SetActive(false);
                }
            }
        }
    }
}

