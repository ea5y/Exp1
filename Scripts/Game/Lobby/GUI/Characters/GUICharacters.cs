using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Scm.Common.Master;
using XDATA;
using Scm.Common.Packet;
using Scm.Common.GameParameter;

namespace XUI
{
    public enum TweenWay
    {
        Forward,
        Reverse
    }

    public class GUICharacters : PanelBase<GUICharacters>
    {
        //public static GUICharacters Inst {get{return Instance as GUICharacters;}}

        public CharactersView view;
        private TweenPosition baseTween;
        private TweenAlpha rightTween;
        private TweenScale frameTween;
        
        private CustomControl.ScrollView<ItemCharacters> scrollView;

        private DeckInfo deckInfo;        

        public ItemCharacter preChara;
        private CharaData preCharaData;
        public CharaData PreCharaData
        {
            get { return this.preCharaData; }
            set
            {
                this.preCharaData = value;
            }
        }
        
        private CharaData curCharaData;
        public CharaData CurCharaData
        {
            get { return this.curCharaData; }
            set
            {
                this.curCharaData = value;
                this.SetBaseView();
            }
        }   

        private bool isLoaded = false;
        private bool isFirst = false;
        
        public EventHandler OnCharaLockFlagChange = (s, e) => { };

        bool hasSelected = false;

        #region BaseView
        private void SetBaseView()
        {
            //Portriat
            this.SetPortrait();
            //Name
            this.SetName(this.CurCharaData.chara.Info.Name);
            //RemainTime
            this.SetRemainTime(this.CurCharaData.chara.Info.RemainTime, this.CurCharaData.chara.Info.TotalTime);
            //Stars
            this.SetStars();
            //Lock
            this.SetBtnLock(this.CurCharaData.chara.Info.IsLock);
        }

        public void SetPortrait()
        {
            //this.view.portrait.DestroyChild();
            //this.view.portraitPreview.DestroyChild();
            this.view.portrait.DestroyChildImmediate();
            this.view.portraitPreview.DestroyChildImmediate();
            
            this.view.portrait.SetActive(true);

            //CharaModel.Create(this.view.portrait, this.CurCharaData.chara.Info);
            CharaBoard charaBoard = ScmParam.Lobby.CharaBoard;
            if (charaBoard != null)
            {
                charaBoard.GetBoard(this.CurCharaData.chara.Info.AvatarType, (int)this.CurCharaData.chara.Info.SkinId, false,
                    (res) =>
                    {
                        if (res == null)
                            return;
                        var go = SafeObject.Instantiate(res) as GameObject;
                        if (go == null)
                            return;
                        go.name = res.name;
                        var t = go.transform;
                        t.parent = this.view.portrait.transform;
                        AvatarMasterData data;
                        AvatarMaster.Instance.TryGetMasterData((int)this.CurCharaData.chara.Info.SkinId, out data);

                        if (data.OffsetX == 0 && data.OffsetY == 0 && data.Scale == 0)
                        {
                            t.localPosition = new Vector3(data.OffsetX, data.OffsetY, 0);
                            t.localScale = new Vector3(0.65f, 0.65f, 0.65f);
                            return;
                        }
                        t.localPosition = new Vector3(data.OffsetX, data.OffsetY, 0);
                        t.localScale = new Vector3(data.Scale, data.Scale, data.Scale);
                    });
            }

            //Name
            this.SetName(this.CurCharaData.chara.Info.Name);
            //RemainTime
            this.SetRemainTime(this.CurCharaData.chara.Info.RemainTime, this.CurCharaData.chara.Info.TotalTime);
        }

        public void SetPreview(CharaInfo charaInfo, CharacterAvatarParameter skinInfo)
        {
            Debug.Log("CharaID: " + (int)charaInfo.AvatarType);
            Debug.Log("SkinID: " + (int)charaInfo.SkinId);
            this.view.portrait.SetActive(false);
            //this.view.portraitPreview.DestroyChild();
            this.view.portraitPreview.DestroyChildImmediate();
            CharaBoard charaBoard = ScmParam.Lobby.CharaBoard;
            if (charaBoard != null)
            {
                charaBoard.GetBoard(charaInfo.AvatarType, (int)charaInfo.SkinId, false,
                    (res) =>
                    {
                        if (res == null)
                            return;
                        var go = SafeObject.Instantiate(res) as GameObject;
                        if (go == null)
                            return;
                        go.name = res.name;
                        var t = go.transform;
                        t.parent = this.view.portraitPreview.transform;

                        AvatarMasterData data;
                        AvatarMaster.Instance.TryGetMasterData((int)this.CurCharaData.chara.Info.SkinId, out data);

                        if (data.OffsetX == 0 && data.OffsetY == 0 && data.Scale == 0)
                        {
                            t.localPosition = new Vector3(data.OffsetX, data.OffsetY, 0);
                            t.localScale = new Vector3(0.65f, 0.65f, 0.65f);
                            return;
                        }
                        t.localPosition = new Vector3(data.OffsetX, data.OffsetY, 0);
                        t.localScale = new Vector3(data.Scale, data.Scale, data.Scale);
                    });
            }
            
            this.SetName(skinInfo.Name);
            this.SetRemainTime(skinInfo.RemainTime, skinInfo.TotalTime, skinInfo.Count != 0);
        }

        public void BackFromPreview()
        {
            this.view.portrait.SetActive(true);
            //this.view.portraitPreview.DestroyChild();
            this.view.portraitPreview.DestroyChildImmediate();
            //Name
            this.SetName(this.CurCharaData.chara.Info.Name);
            //RemainTime
            this.SetRemainTime(this.CurCharaData.chara.Info.RemainTime, this.CurCharaData.chara.Info.TotalTime);
        }

        private void SetName(string name)
        {
            this.view.charaName.text = name;
        }

        private void SetRemainTime(long remainTime, int totalTime, bool isShow = true)
        {
            if (totalTime == 0)
                this.view.remainTime.text = "时限：永久";
            else
                this.view.remainTime.text = "时限：" + CustomControl.ToolFunc.TimeCountDown(remainTime);
            this.view.remainTime.gameObject.SetActive(isShow);
        }

        private void SetStars()
        {
            var starsNum = CharaStarMaster.Instance.GetStarByID(this.CurCharaData.chara.Info.StarId);
            if (starsNum == this.view.starsGroup.transform.childCount)
                return;

            CustomControl.ToolFunc.GridAddItemAndResponse(this.view.starsGroup, this.view.itemStar.gameObject, 36, starsNum);

        }
        #endregion
                
        void Awake()
        {
            base.Awake();
            
            this.GetTween();
            this.HideFirst();
        }
        
        private void OnDisable()
        {
            //this.view.portrait.DestroyChild();
            //this.view.grid.gameObject.DestroyChild();
            this.view.portrait.DestroyChildImmediate();
            this.view.grid.gameObject.DestroyChildImmediate();
            this.DeleteEvent();
        }

        private void GetTween()
        {
            this.baseTween = this.view.group.GetComponent<TweenPosition>();
            this.rightTween = this.view.right.GetComponent<TweenAlpha>();
            this.frameTween = this.view.frame.GetComponent<TweenScale>();
        }

        private void HideFirst()
        {
            this.gameObject.SetActive(false);
        }

        public void HideBtnEnterAndSell(bool isHide)
        {
            this.view.btnEnter.gameObject.SetActive(!isHide);
            this.view.btnCell.gameObject.SetActive(!isHide);
        }

        private void registerEvent()
        {
            PlayerData.Instance.OnDeckInfoChange += this.GetDeckInfo;
            PlayerData.Instance.OnCharaListChange += this.GetCharaList;

            this.OnCharaDataListChange += this.UpdateScrollView;

            this.OnCharaLockFlagChange += this.OnLockFlagChange;
            
        }
                
        private void DeleteEvent()
        {
            PlayerData.Instance.OnDeckInfoChange -= this.GetDeckInfo;
            PlayerData.Instance.OnCharaListChange -= this.GetCharaList;
            this.OnCharaDataListChange -= this.UpdateScrollView;

            this.OnCharaLockFlagChange -= this.OnLockFlagChange;
        }

        private void GetDeckInfo(object sender, EventArgs e)
        {
            this.deckInfo = ((PlayerData)sender).deckInfo;
        }

        private void UpdateScrollView(object sender, EventArgs e)
        {
            this.SetScrollView();
            this.SetLimit();
        }

        private void SetLimit()
        {
            this.view.lblLimit.text = this.CharaDataList.Count + " / " + PlayerData.Instance.CharaBoxCapacity;
        }
        
        private void GetCharaList(object sender, EventArgs e)
        {
            //construct  
            var charaList = new List<Chara>(((PlayerData)sender).CharaList);
            var temp = new List<CharaData>();
            for (int i = 0; i < charaList.Count; i++)
            {
                var chara = new CharaData(charaList[i]);
                temp.Add(chara);
            }

            foreach (var charaData in temp)
            {
                foreach (var deck in this.deckInfo.CharaInfoList)
                {
                    if (deck.UUID == charaData.chara.Info.UUID)
                    {
                        charaData.isInDeck = true;
                        break;
                    }
                }
            }
            //assign
            this.CharaDataList = temp;

           
        }

        public EventHandler OnCharaDataListChange = (s, e) => { };
        private List<CharaData> charaDataList;
        public List<CharaData> CharaDataList
        {
            get
            {
                return this.charaDataList;
            }
            set
            {
                Debug.Log("Assign 1");
                this.charaDataList = value;
                this.OnCharaDataListChange(this, EventArgs.Empty);
            }
        }
        private void SetScrollView()
        {
            
            this.hasSelected = false;
            //sort
            this.CharaDataList.Sort((a, b) => {
                int t = b.chara.Info.Level.CompareTo(a.chara.Info.Level);
                if(t == 0)
                {
                    return b.chara.Info.UUID.CompareTo(a.chara.Info.UUID);
                }
                return t;
            });

            //set index
            if(!hasSelected)
            {
                for (int i = 0; i < this.CharaDataList.Count; i++)
                {
                    this.CharaDataList[i].index = i;
                }
            }            

            //pack
            var datas = CharaData.Pack(this.CharaDataList, 4);

            //create
            if(this.scrollView == null)
                this.scrollView = new CustomControl.ScrollView<ItemCharacters>(this.view.grid, this.view.itemPrefab);
            this.scrollView.CreateWeight(datas);
            Debug.Log("Line: " + datas.Count);

            foreach (var charaData in this.CharaDataList)
            {
                if (charaData.selectFlagNormal)
                {
                    this.scrollView.FindCellItemAndChange<CharaData, ItemCharacter>(charaData.index, (data, item) => {
                        if(item != null)
                            item.IsSelected = true;
                    });
                    hasSelected = true;
                    Debug.Log("Has selected");
                    break;
                }
            }
            if (!hasSelected)
            {
                this.scrollView.FindCellItemAndChange<CharaData, ItemCharacter>(0, (data, item) => {
                    data.selectFlagNormal = true;
                    item.IsSelected = true;

                });

                Debug.Log("Selected 0");
            }
        }
        
        public void Open()
        {
            Net.Network.Instance.StartCoroutine(this._Open());                 
        }

        private IEnumerator _Open()
        {
            this.registerEvent();

            //first get deck info
            yield return PlayerData.Instance.GetDeckInfo();

            yield return PlayerData.Instance.GetCharaListTest();

            TopBottom.Instance.OnIn = () =>
            {
                PanelManager.Instance.Open(this.view.root);
            };
            TopBottom.Instance.OnBack = (v) =>
            {
                StartCoroutine(PanelManager.Instance.BackWithTween());
                v();
            };
            TopBottom.Instance.In("角色");
                        
            /*
            //select
            foreach (var charaData in this.CharaDataList)
            {
                if (charaData.selectFlagNormal)
                {
                    this.scrollView.FindCellItemAndChange<CharaData, ItemCharacter>(charaData.index, (data, item) => {
                        item.IsSelected = true;
                    });
                    hasSelected = true;
                    Debug.Log("Has selected");
                    break;
                }
            }
            if (!hasSelected)
            {
                this.scrollView.FindCellItemAndChange<CharaData, ItemCharacter>(0, (data, item) => {
                    data.selectFlagNormal = true;
                    item.IsSelected = true;

                });

                Debug.Log("Selected 0");
            }
            */
            //show tip if num > 180;
            if (this.CharaDataList.Count > PlayerData.Instance.CharaBoxCapacity)
                GUITipMessage.Instance.Show("角色即将达到上限！到达上限后将不再掉落角色！购买角色能拿到！");

        }

        public void OnBtnLockClick()
        {
            Net.Network.Instance.StartCoroutine( this.CurCharaData.chara.Lock((res) => {
                this.CurCharaData.chara.Info.IsLock = res.LockFlag;
                this.OnCharaLockFlagChange(this, EventArgs.Empty);
            }));
        }

        private void OnLockFlagChange(object obj, EventArgs e)
        {
            this.SetBtnLock(this.CurCharaData.chara.Info.IsLock);
            this.scrollView.FindCellItemAndChange<CharaData, ItemCharacter>(this.CurCharaData.index, (data, item) => {
                data.chara.Info.IsLock = this.CurCharaData.chara.Info.IsLock;
                item.SetLock();
            });
        }

        private void SetBtnLock(bool isLock)
        {
            if (isLock)
                this.view.btnLock.normalSprite = "suoding";
            else
                this.view.btnLock.normalSprite = "jiesuo";  
        }
        
        public void OnBtnEnterClick()
        {
            SoundController.PlaySe(SoundController.SeID.WindowClose);
            TopBottom.Instance.mUILabel.text = "角色详细";
            TopBottom.Instance.OnBack = (v) =>
            {
                StartCoroutine(PanelManager.Instance.BackWithTween());
                TopBottom.Instance.mUILabel.text = "角色";
                TopBottom.Instance.OnBack = (u) =>
                {
                    StartCoroutine(PanelManager.Instance.BackWithTween());
                    u();
                };
            };
            this.Forward();
        }

		private XUI.ProductShowData GetSellHeroInfo()
		{
			XUI.ProductShowData data = new XUI.ProductShowData();
			data.type = ShopItemType.Character;
			data.characterId = this.CurCharaData.chara.Info.CharacterMasterID;
			data.skinId = this.CurCharaData.chara.Info.SkinId;
			data.tip = "确定要解雇吗？";
			data.desc ="Lv." + this.CurCharaData.chara.Info.Level;
			return data;
		}

        public void OnBtnCellClick()
        {
            List<ulong> uuids = new List<ulong>();
            uuids.Add(this.CurCharaData.chara.Info.UUID);
            if(this.CurCharaData.chara.Info.IsLock)
            {
                GUITipMessage.Instance.Show("角色被锁定了!");
                return;
            }
            if(this.CurCharaData.isInDeck)
            {
                GUITipMessage.Instance.Show("该角色在编队中！");
                return;
            }


			GUIProductsWindow.Instance.OpenOkNo(GetSellHeroInfo(),() => {
				Net.Network.Instance.StartCoroutine(Net.Network.SellCharacter(uuids.ToArray(), (res) => {
					if(res.Result == false)
					{
						GUITipMessage.Instance.Show("解雇失败！");
						return;
					}

					//show tip
					GUITipMessage.Instance.Show("获得 " + res.SoldPrice + " 金币！");
					//Set right
					List<CharaData> bufferList = new List<CharaData>();
					bool rejectFlag = false;
					for(int i = 0; i < this.CharaDataList.Count; i++)
					{
						rejectFlag = false;
						if (this.CharaDataList[i].chara.Info.UUID == this.CurCharaData.chara.Info.UUID)
							rejectFlag = true;
						if (rejectFlag == true)
							continue;
						bufferList.Add(this.CharaDataList[i]);
					}
					this.CharaDataList = bufferList;

				}));
			});
            
        }

        public void Forward()
        {
            GUICharacterDetial.Instance.Open(this.CurCharaData.chara);
            this.HideBtnEnterAndSell(true);
            this.TweenForward();
        }

        public void Reverse()
        {
            this.TweenReverse();
        }
        
        #region Tween
        private void TweenForward()
        {            
            this.ConfigTweenOnFinishForward();
            this.StartRightTween(TweenWay.Forward);
        }

        public void TweenReverse()
        {
            this.ConfigTweenOnFinishReverse();
            this.StartBaseTween(TweenWay.Reverse);
        }

        private void ClearTweenOnFinish()
        {
            this.baseTween.onFinished.Clear();
            this.rightTween.onFinished.Clear();
        }
         
        private void ConfigTweenOnFinishForward()
        {
            this.ClearTweenOnFinish();
            EventDelegate.Add(this.rightTween.onFinished, ()=> { this.StartBaseTween(TweenWay.Forward); } );
            EventDelegate.Add(this.baseTween.onFinished, () => {                
                StartCoroutine(GUICharacterDetial.Instance.TweenForward());
            });
        }

        private void ConfigTweenOnFinishReverse()
        {
            this.ClearTweenOnFinish();
            EventDelegate.Add(this.baseTween.onFinished, ()=> {
                this.HideBtnEnterAndSell(false);
                this.StartRightTween(TweenWay.Reverse);
            } );
            EventDelegate.Add(this.rightTween.onFinished, () => {
                TouchFilter.Instance.Enable(false);
            });
        }
        
        private void StartBaseTween(TweenWay way)
        {
            if (way == TweenWay.Forward)
            {
                this.baseTween.PlayForward();
                this.frameTween.PlayForward();
            }
                
            else
            {
                this.baseTween.PlayReverse();
                this.frameTween.PlayReverse();
            }
                
        }

        private void StartRightTween(TweenWay way)
        {            
            if (way == TweenWay.Forward)
            {
                TouchFilter.Instance.Enable(true);
                this.rightTween.PlayForward();                
            }                
            else
            {
                this.rightTween.PlayReverse();                
            }                
        }

        private void StartFrameTween(TweenWay way)
        {
            if(way == TweenWay.Forward)
            {
                this.frameTween.PlayForward();
            }else
            {
                this.frameTween.PlayReverse();
            }
        }
        #endregion

        public override void Reset()
        {
            Debug.Log("ResetTween!");
            this.ResetTween();
        }

        private void ResetTween()
        {
            var transBase = this.baseTween.gameObject.transform;
            if (transBase.localPosition.x == -6)
                return;
            this.baseTween.ResetToBeginning();
            this.frameTween.ResetToBeginning();
            this.rightTween.ResetToBeginning();
            transBase.localPosition = new Vector3(-6, transBase.localPosition.y, 0);
            var transFrame = this.frameTween.gameObject.transform;
            transFrame.localScale = new Vector3(1,1,1);
            var transRight = this.rightTween.gameObject.transform;
            transRight.GetComponent<UIWidget>().alpha = 1;

            this.HideBtnEnterAndSell(false);
        }


    }

}
