//author: luwanzhong
//date: 2016-11-18
//desc: userInfo Controller

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Scm.Common.Packet;
using Scm.Common.GameParameter;
using Scm.Common.Master;
using Scm.Client;
using CustomControl;
using System;
using Scm.Common;

namespace XUI.UserInfo{
    public class UserInfoController : Singleton<UserInfoController> {
    #region property
        Model _model;
        private Model Model { get { return _model; } set { _model = value; } }

        [SerializeField]
        UserInfoView _view;
        public UserInfoView View { get { return _view; } }

        private List<XUIButton> baseBtnList;

        private enum BasePage
        {
            BaseData,
            CombatGains,
            CharaStatistics
        }

        /// <summary>
        /// awake
        /// </summary>
        void Awake()
        {
            base.Awake();
            this.Init();
        }

        /// <summary>
        /// pages base
        /// </summary>
        CustomControl.TabPagesManager pagesBase;
        CustomControl.TabPagesManager PagesBase { get { return this.pagesBase; } set { this.pagesBase = value; } }

        /// <summary>
        /// pages statistics
        /// </summary>
        CustomControl.TabPagesManager pagesStatistics;
        CustomControl.TabPagesManager PagesStatistics { get { return this.pagesStatistics; } set { this.pagesStatistics = value; } }

        /// <summary>
        /// charaIcon
        /// </summary>
        CharaIcon CharaIcon { get { return ScmParam.Lobby.CharaIcon; } }

#endregion

    #region init
        /// <summary>
        /// Init
        /// </summary>
        public void Init()
        {
            if (this.Model == null)
                this.Model = new Model();

            this.baseBtnList = new List<XUIButton>();
            this.baseBtnList.Add(this.View.Base.BtnBasicData);
            this.baseBtnList.Add(this.View.Base.BtnCombatGains);
            this.baseBtnList.Add(this.View.Base.BtnCharaStatistics);

            if (this.PagesBase == null)
                this.PagesBase = new CustomControl.TabPagesManager();
            this.PagesBase.AddPage(_view.BasicData);
            this.PagesBase.AddPage(_view.CombatGains);
            this.PagesBase.AddPage(_view.CharaStatistics);
            this.SwitchTo(BasePage.BaseData);

            if (this.PagesStatistics == null)
                this.PagesStatistics = new CustomControl.TabPagesManager();
            this.PagesStatistics.AddPage(_view.CharaStatistics.Detail);
            this.PagesStatistics.AddPage(_view.CharaStatistics.Slider);
            this.PagesStatistics.SwitchTo(_view.CharaStatistics.Detail);

            this.RegistViewEventNew();

            this.View.Root.SetActive(false);
        }

    #endregion

    #region show
        public void Show()
        {
            Net.Network.Instance.StartCoroutine(Open());
        }

        public IEnumerator Open()
        {
            //loadingdata
            yield return Net.Network.GetPlayerStatusInfo(0, this.SyncBasicDataModel);
            
            TopBottom.Instance.OnIn = () =>
            {
                //Instance.gameObject.SetActive(true);
                PanelManager.Instance.Open(_view.Root);
            };
            TopBottom.Instance.OnBack = (v) =>
            {
                //Instance.gameObject.SetActive(false);
                PanelManager.Instance.Close(_view.Root);
                v();
            };
            TopBottom.Instance.In("用户");
            /*TopBar.Instance.OnIn= ()=>{
                PanelManager.Instance.Open(_view.Root);
            };
            TopBar.Instance.OnBack = (v)=>{
                PanelManager.Instance.Close(_view.Root);
                v();
            };
            TopBar.Instance.In("用户");*/
            
            yield return Net.Network.GetCombatGiansBaseInfo(this.SyncCombatGainsBaseModel, false);
            yield return Net.Network.GetPlayerMatchingInfo(0, this.SyncChCharaStatisticsDetailModel, false);

            Debug.Log("charaId: " + _model.baseData.charaId);
            Debug.Log("skinId: " + _model.baseData.skinId);

            this.SetPortrait(_model.baseData.charaId, _model.baseData.skinId);
        }

        void OnEnable()
        {
            this.SwitchTo(BasePage.BaseData);
        }

        void OnDisable()
        {
            //_view.BasicData.Portrait.DestroyChild();
            _view.BasicData.Portrait.DestroyChildImmediate();
        }


    #endregion

    #region sync model
        /// <summary>
        /// basic data
        /// </summary>
        /// <param name="code"></param>
        /// <param name="res"></param>
        public void SyncBasicDataModel(PlayerStatusRes res)
        {
            var info = res.GetPlayerStatusParam();

            //this._model.baseData.avatarType = info.;
            this._model.baseData.charaId = info.CharacterMasterId;
            this._model.baseData.lv = info.Level;
            this._model.baseData.gameTimes = info.BattleCount;
            this._model.baseData.winTimes = info.WinCount;
            this._model.baseData.killTimes = info.KillCount;
            this._model.baseData.mvpTimes = info.MVPCount;
            this._model.baseData.rank = info.Rank;
            this._model.baseData.rankMax = info.MaxRank;
            this._model.baseData.currentScore = info.RankExp;
            this._model.baseData.needScore = info.NextRankExp;
            this._model.baseData.update = true;
            this._model.baseData.skinId = info.SkinId;

            this.SetBasicData();
        }

        /// <summary>
        /// combatGains base
        /// </summary>
        /// <param name="code"></param>
        /// <param name="res"></param>
        public void SyncCombatGainsBaseModel(BattleHistoryRes res)
        {
            var info = res.GetBattleHistoryRecords();
            _model.combatGains.records = info;
            _model.combatGains.updateRecords = true;

            this.SetCombatGainsBase(_model.combatGains.records, this.CharaIcon);
        }

        /// <summary>
        /// combatGains oneGame
        /// </summary>
        /// <param name="code"></param>
        /// <param name="res"></param>
        public void SyncCombatGainsOneGameModel(BattleHistoryDetailRes res)
        {
            var info = res.GetBattleHistoryDetailItems();
            _model.combatGains.detailItems = info;
            _model.combatGains.updateDetailItems = true;

            this.SetCombatGainsOneGame(_model.combatGains.detailItems, this.CharaIcon);
        }

        /// <summary>
        /// combatGains player
        /// </summary>
        /// <param name="code"></param>
        /// <param name="res"></param>
        public void SyncCombatGainsPlayerModel(PlayerMiscInfoRes res)
        {
            var quickInfo = res.GetQuickMatching();
            var rankInfo = res.GetRankMatching();

            _model.combatGains.playerMatchInfo.rankInfo = res;
            _model.combatGains.playerMatchInfo.quickMatchInfo = quickInfo;
            _model.combatGains.playerMatchInfo.rankMatchInfo = rankInfo;
            _model.combatGains.updatePlayerMatchInfo = true;

            this.SetCombatGainsPlayer(_model.combatGains.playerMatchInfo.rankInfo, _model.combatGains.playerMatchInfo.quickMatchInfo, _model.combatGains.playerMatchInfo.rankMatchInfo);
        }

        /// <summary>
        /// charaStatistics detail
        /// </summary>
        /// <param name="code"></param>
        /// <param name="res"></param>
        public void SyncChCharaStatisticsDetailModel(PlayerMiscInfoRes res)
        {
            var quickInfo = res.GetQuickMatching();
            var rankInfo = res.GetRankMatching();

            _model.charaStatistics.myMatchInfo.quickMatchInfo = quickInfo;
            _model.charaStatistics.myMatchInfo.rankMatchInfo = rankInfo;
            _model.charaStatistics.updateMyMatchInfo = true;

            this.SetCharaStatisticsDetial(_model.charaStatistics.myMatchInfo.quickMatchInfo, _model.charaStatistics.myMatchInfo.rankMatchInfo);
        }
        #endregion

    #region sync view
        #region BasicData
        /// <summary>
        /// Set BasicData
        /// </summary>
        public void SetBasicData()
        {
            _view.BasicData.userName.text = NetworkController.ServerValue.PlayerInfo.UserName;
            _view.BasicData.Lv.text = _model.baseData.lv + "";
            _view.BasicData.GameTimes.text = _model.baseData.gameTimes + "";
            _view.BasicData.WinTimes.text = _model.baseData.winTimes + "";
            _view.BasicData.KillTimes.text = _model.baseData.killTimes + "";
            _view.BasicData.MvpTime.text = _model.baseData.mvpTimes + "";
            _view.BasicData.RankLabel.text = this.GetRank() + "";
            this.SetRankFilled();
            this.SetRankIcon();
            _view.BasicData.MaxRank.text = _model.baseData.rankMax + "";

//            _view.BasicData.Detail.currentScore.text = _model.baseData.currentScore + "";
//            _view.BasicData.Detail.needScore.text = _model.baseData.needScore + "";
        }

        private int GetRank()
        {
            return PlayerRankMaster.Instance.GetRankByScore(_model.baseData.currentScore);
        }

        private void SetRankIcon()
        {
            _view.BasicData.RankIcon.spriteName = PlayerRankMaster.Instance.GetRankByScore(_model.baseData.currentScore) + "";
        }
        
        private void SetRankFilled()
        {
            this.SetCurScore(_model.baseData.currentScore, PlayerRankMaster.Instance.GetRankByScore(_model.baseData.currentScore));
        }

        private void SetCurScore(long curScore, int rank)
        {
            var overScore = this.GetOverScore(curScore, rank);
            var needScore = this.GetNeedScore(rank);
            _view.BasicData.RankFilled.fillAmount = (float)overScore / needScore * (float)0.755;
        }

        private float GetOverScore(long curScore, int rank)
        {
            float overScore = 0;
            if (rank == 1)
                overScore = curScore;
            else
                overScore = curScore - PlayerRankMaster.Instance.GetNextScore(--rank);
            return overScore;
        }

        private float GetNeedScore(int rank)
        {
            float result = 0;
            if(rank == 1)
                result = PlayerRankMaster.Instance.GetNextScore(rank);
            else
                result = PlayerRankMaster.Instance.GetNextScore(rank) - PlayerRankMaster.Instance.GetNextScore(--rank);
            return result;
        }

        private Animation anime;

        public void SetPortrait(int charaId, int skinId)
        {
            //_view.BasicData.Portrait.DestroyChild();
            _view.BasicData.Portrait.DestroyChildImmediate();

            //CharaModel.Create(_view.BasicData.Portrait, new CharaInfo((AvatarType)charaId, skinId));            
            CharaBoard charaBoard = ScmParam.Lobby.CharaBoard;
            if (charaBoard != null)
            {
                charaBoard.GetBoard((AvatarType)charaId, (int)skinId, false,
                    (res) =>
                    {
                        if (res == null)
                            return;
                        var go = SafeObject.Instantiate(res) as GameObject;
                        if (go == null)
                            return;
                        go.name = res.name;
                        var t = go.transform;
                        t.parent = _view.BasicData.Portrait.transform;
                        AvatarMasterData data;
                        AvatarMaster.Instance.TryGetMasterData((int)skinId, out data);

                        t.localPosition = new Vector3(data.OffsetX, data.OffsetY, 0);
                        t.localScale = new Vector3(data.Scale, data.Scale, data.Scale);
                    });
            }
        }
        #endregion

        #region CombatGains
        /// <summary>
        /// Set CombatGains Base
        /// </summary>
        /// <param name="packet"></param>
        /// <param name="charaIcon"></param>
        public void SetCombatGainsBase(BattleHistoryRecord[] packet, CharaIcon charaIcon)
        {
            //this.ShowArrows("down");
            //_view.CombatGains.Base.grid.gameObject.DestroyChild();
            _view.CombatGains.Base.grid.gameObject.DestroyChildImmediate();
            foreach (BattleHistoryRecord record in packet)
            {
                var itemInfo = record.GetPlayerItemParams();
                GameObject go = NGUITools.AddChild(_view.CombatGains.Base.grid.gameObject, _view.CombatGains.Base.ItemBase.root);
                
                ItemBase item = go.GetComponent<ItemBase>();

                var blueTeam = this.GetTeam(itemInfo, TeamType.Blue);
                var redTeam = this.GetTeam(itemInfo, TeamType.Red);

                //get my team
                var myTeam = this.GetMyTeam(itemInfo);

                //set left team
                this.SetTeam(item.iconListLeft, myTeam == TeamType.Blue ? blueTeam : redTeam, charaIcon);

                //set right team
                this.SetTeam(item.iconListRight, myTeam == TeamType.Blue ? redTeam : blueTeam, charaIcon);

                var p = GameController.GetPlayer();
                //set result
                int result = this.GetResult((JudgeType)record.JudgeTypeID, myTeam);
                switch (result)
                {
                    case 1:
                        item.result.spriteName = "icon_win";
                        break;
                    case -1:
                        item.result.spriteName = "icon_lose";
                        break;
                    case 0:
                        item.result.spriteName = "icon_draw";
                        break;
                }
                
                //set game type
                item.gameType.text = (ScoreType)record.ScoreType + "";
                #if PLATE_NUMBER_REVIEW
                item.gameType.text = "快速比赛";
                #endif

                //set game time
                DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
                var dateTime = dtStart.AddSeconds(record.BattleTime);
                item.gameTime.text = dateTime.ToString("yyyy/MM/dd HH:mm:ss");//record.BattleTime.UnixTimeToDateTime().ToString("yyyy/MM/dd HH:mm:ss");
      
                var self = this;
                long battleID = record.BattleID;
                long battleTime = record.Duration;
                var fieldId = record.FieldID;

                //regist touch event
                EventDelegate.Add(item.btnEnter.onClick, 
                    () => {
                        //
                        self.OnCombatGainsBaseItemClick(battleID, battleTime, fieldId, result, record.JudgeTypeID);
                    });
            }
            _view.CombatGains.Base.grid.repositionNow = true;
            _view.CombatGains.Base.grid.GetComponent<UIGrid>().Reposition();
        }

        private int GetResult(JudgeType type, TeamType myTeam)
        {
            /*
            if (type == JudgeType.WinnerBlue || type == JudgeType.CompleteWinnerBlue)
                return myTeam == TeamType.Blue;
            else
            {
                return myTeam == TeamType.Red;
            }*/

            if(type == JudgeType.WinnerBlue || type == JudgeType.CompleteWinnerBlue)
            {
                if (myTeam == TeamType.Blue)
                    return 1;
                else
                    return -1;
            }
            else if (type == JudgeType.WinnerRed || type == JudgeType.CompleteWinnerRed)
            {
                if (myTeam == TeamType.Red)
                    return 1;
                else
                    return -1;
            }
            else 
                return 0;            
        }

        private void ShowArrows(string direc)
        {
            if (direc == "down")
            {
                this.View.CombatGains.Base.arrowsDown.SetActive(true);
                this.View.CombatGains.Base.arrowsDown.gameObject.GetComponent<UISprite>().depth = 1000;
            }
            if (direc == "up")
            {
                this.View.CombatGains.Base.arrowsUp.SetActive(true);
                this.View.CombatGains.Base.arrowsUp.gameObject.GetComponent<UISprite>().depth = 1000;
            }
        }

        private void HideArrows(string direc)
        {
            if (direc == "down")
            {
                this.View.CombatGains.Base.arrowsDown.SetActive(false);
            }
            if (direc == "up")
            {
                this.View.CombatGains.Base.arrowsUp.SetActive(false);
            }
        }

        TeamType GetMyTeam(BattleHistoryPlayerItem[] itemInfo)
        {
            int id = NetworkController.ServerValue.PlayerId;
            foreach(var info in itemInfo){
                if(info.PlayerID == id)
                    return (TeamType)info.TeamType;
            }

            return 0;
        }

        void SetTeam(UISprite[] iconList, List<BattleHistoryPlayerItem> teamInfoList, CharaIcon charaIcon)
        {
            for (int i = 0; i < iconList.Length; i++)
            {
                if (i > teamInfoList.Count - 1)
                {
                    iconList[i].spriteName = " ";
                }
                else
                {
                    var index = i;
                    charaIcon.GetIcon((AvatarType)teamInfoList[i].CharacterID, teamInfoList[i].SkinID, false, 
                        (UIAtlas res, string name) => {
                            iconList[index].atlas = res;
                            iconList[index].spriteName = name;
                        });
                }
            }
        }

        List<BattleHistoryPlayerItem> GetTeam(BattleHistoryPlayerItem[] infoList, TeamType team)
        {
            List<BattleHistoryPlayerItem> teamList = new List<BattleHistoryPlayerItem>();
            foreach (var info in infoList)
            {
                if ( team == ( TeamType )info.TeamType )
                {
                    teamList.Add(info);
                }
            }
            return teamList;
        }

        public void SetCombatGainsOneGame(BattleHistoryDetailItem[] detail, CharaIcon charaIcon)
        {
            //get team
            var blueTeam = this.GetTeam(detail, TeamType.Blue);
            var redTeam = this.GetTeam(detail, TeamType.Red);

            //get my team
            var myTeam = this.GetMyTeam(detail);

            //set team left
            this.SetTeam(_view.CombatGains.OneGame.groupLeft, myTeam == TeamType.Blue ? blueTeam : redTeam, charaIcon, true);

            //set team right
            this.SetTeam(_view.CombatGains.OneGame.groupRight, myTeam == TeamType.Blue ? redTeam : blueTeam, charaIcon, false);
        }

        TeamType GetMyTeam(BattleHistoryDetailItem[] itemInfo)
        {
            int id = NetworkController.ServerValue.PlayerId;
            foreach (var info in itemInfo)
            {
                if (info.PlayerId == id)
                    return (TeamType)info.TeamType;
            }

            return 0;
        }

        List<BattleHistoryDetailItem> GetTeam(BattleHistoryDetailItem[] infoList, TeamType team)
        {
            List<BattleHistoryDetailItem> teamList = new List<BattleHistoryDetailItem>();
            foreach (var info in infoList)
            {
                if (team == (TeamType)info.TeamType)
                {
                    teamList.Add(info);
                }
            }
            return teamList;
        }

        void SetTeam(GameObject widget, List<BattleHistoryDetailItem> teamInfoList, CharaIcon charaIcon, bool isLeft)
        {


            float spacing = 78;
            float posY = 213;
            //widget.DestroyChild();
            widget.DestroyChildImmediate();
            for (int i = 0; i < teamInfoList.Count; i++)
            {
                posY -= spacing;
                GameObject go = NGUITools.AddChild(widget, _view.CombatGains.OneGame.itemInfo);
                go.transform.parent = widget.transform;
                go.transform.localPosition = new Vector3(-14, posY, 0);

                ItemCombatGainsOneGame item = go.GetComponent<ItemCombatGainsOneGame>();
                //set mvp
                item.mvp.SetActive(teamInfoList[i].RankInBattle == 1);

                //set bg
                if (isLeft)
                {
                    item.bg.spriteName = "blue_bottom";
                    if(item.bgSelf.gameObject.activeSelf == true)
                        item.bgSelf.gameObject.SetActive(false);
                    if (NetworkController.ServerValue.PlayerId == teamInfoList[i].PlayerId)
                    {
                        item.bg.spriteName = "green_bottom";
                        item.bgSelf.gameObject.SetActive(true);
                    }
                    //item.bgSelf.
                }
                else
                {
                    if(item.bgSelf.gameObject.activeSelf == true)
                        item.bgSelf.gameObject.SetActive(false);
                    item.bg.spriteName = "red_bottom";
                }

                //set name
                item.nameL.text = teamInfoList[i].Name;
                //set icon
                charaIcon.GetIcon((AvatarType)teamInfoList[i].Character, teamInfoList[i].SkinId, false,
                        (UIAtlas res, string name) =>
                        {
                            item.icon.atlas = res;
                            item.icon.spriteName = name;
                        });
                //set kill times
                item.killTimes.text = teamInfoList[i].Defeat + "";
                //set dead times
                item.deadTimes.text = teamInfoList[i].Death + "";
                //set damage
                item.damage.text = teamInfoList[i].Attack + "";
                //set active time
                item.gameTime.text = CustomControl.ToolFunc.TimeSecondIntToString(teamInfoList[i].ControlTime, false);

                var playerId = teamInfoList[i].PlayerId;

                //btn enter
                EventDelegate.Add(item.btnEnter.onClick, 
                    () => {
                        this.OnCombatGainsOneGameItemClick(playerId);
                    });
            }

        }
        
        public void SetCombatGainsPlayer(PlayerMiscInfoRes res, PlayerMatchParameter quickInfo, PlayerMatchParameter rankInfo)
        {
            //left info
            var viewInfo = _view.CombatGains.Player;
            //viewInfo.lv.text = PlayerLevelMaster.Instance.GetLevelByExp() + "";
            viewInfo.gameTimes.text = quickInfo.BattleCount + rankInfo.BattleCount + "";
            viewInfo.winTimes.text = quickInfo.WinCount + rankInfo.BattleCount + "";
            viewInfo.killTimes.text = quickInfo.KillCount + rankInfo.BattleCount + "";
            viewInfo.mvpTimes.text = quickInfo.MVPCount + rankInfo.BattleCount + "";

            //rank info
            var rank = PlayerRankMaster.Instance.GetRankByScore(res.Score);
            viewInfo.rank.text = rank + "";
            var nextScore = PlayerRankMaster.Instance.GetNextScore(rank);
            viewInfo.rankField.fillAmount = res.Score / nextScore;
            viewInfo.maxRank.text = res.MaxRank + "";

            //detail
            viewInfo.Detail.currentScore.text = res.Score + "";
            viewInfo.Detail.needScore.text = res.NextScore + "";
        }
        #endregion

        #region CharaStatistics
        public void SetCharaStatisticsDetial(PlayerMatchParameter quickInfo, PlayerMatchParameter rankInfo)
        {
            return;
            var quick = _view.CharaStatistics.Detail.Left;
            quick.gameTimes.text = quickInfo.BattleCount + "";
            quick.winTimes.text = quickInfo.WinCount + "";
            quick.killTimes.text = quickInfo.KillCount + "";
            quick.maxKillTimesOneGame.text = quickInfo.MaxKillCount + "";
            quick.damageSum.text = quickInfo.TotalDamage + "";
            quick.maxDamageOneGame.text = quickInfo.MaxDamage + "";
            quick.gameTime.text = quickInfo.ControlTime + "";
            quick.mvpTimes.text = quickInfo.MVPCount + "";

            var rank = _view.CharaStatistics.Detail.Right;
            rank.gameTimes.text = rankInfo.BattleCount + "";
            rank.winTimes.text = rankInfo.WinCount + "";
            rank.killTimes.text = rankInfo.KillCount + "";
            rank.maxKillTimesOneGame.text = rankInfo.MaxKillCount + "";
            rank.damageSum.text = rankInfo.TotalDamage + "";
            rank.maxDamageOneGame.text = rankInfo.MaxDamage + "";
            rank.gameTime.text = rankInfo.ControlTime + "";
            rank.mvpTimes.text = rankInfo.MVPCount + "";
        }

        public void SetCharaStatisticsSlider()
        {

        }
        #endregion
    #endregion

    #region view event
        //RegistViewEvent new
        void RegistViewEventNew()
        {
            EventDelegate.Add(_view.Base.BtnBasicData.onClick, this.OnBtnBasicDataClickNew);
            EventDelegate.Add(_view.Base.BtnCombatGains.onClick, this.OnCombatGainsClickNew);
            EventDelegate.Add(_view.Base.BtnCharaStatistics.onClick, this.OnCharaStatisticsClickNew);

//            EventDelegate.Add(_view.Base.BtnBack.onClick, this.OnBackClickNew);
            EventDelegate.Add(_view.CombatGains.Player.btnClose.onClick, this.OnCombatGainsPlayerBtnCloseClickNew);
            EventDelegate.Add(_view.CombatGains.OneGame.btnClose.onClick, this.OnCombatGainsOneGameBtnCloseClick);

//            EventDelegate.Add(_view.CharaStatistics.BtnDetail.onClick, this.OnBtnCharaStatisticsDetailClickNew);
//            EventDelegate.Add(_view.CharaStatistics.BtnSlider.onClick, this.OnBtnCharaStatisticsSliderClickNew);
        }

        void SwitchTo(BasePage page)
        {
            switch (page)
            {
                case BasePage.BaseData:
                    this.BtnSelected(this.View.Base.BtnBasicData);
                    this.PagesBase.SwitchTo(_view.BasicData);
                    if (this.anime != null)
                        this.anime.Play("wait");
                    break;
                case BasePage.CombatGains:
                    this.BtnSelected(this.View.Base.BtnCombatGains);
                    this.PagesBase.SwitchTo(_view.CombatGains);
                    break;
                case BasePage.CharaStatistics:
                    this.BtnSelected(this.View.Base.BtnCharaStatistics);
                    this.PagesBase.SwitchTo(_view.CharaStatistics);
                    break;
            }
        }

        void BtnSelected(XUIButton btnSel)
        {
            foreach (UIButton btn in this.baseBtnList)
            {
                if (btnSel == btn)
                {
                    btn.enabled = false;
                    btn.SetState(UIButton.State.Disabled, true); //btn.disabledSprite
                    continue;
                }
                if (btn.enabled == false)
                    btn.enabled = true;
            }
        }

        void OnBtnBasicDataClickNew()
        {
            SoundController.PlaySe(SoundController.SeID.WindowClose);
            this.SwitchTo(BasePage.BaseData);
        }

        void OnCombatGainsClickNew()
        {
            SoundController.PlaySe(SoundController.SeID.WindowClose);
            Debug.Log("click!" + Time.time);
            this.SwitchTo(BasePage.CombatGains);
        }

        void OnCharaStatisticsClickNew()
        {
            //=>Wait for new UI
            //this.SwitchTo(BasePage.CharaStatistics);
        }

        void OnBackClickNew()
        {
            PanelManager.Instance.Back();
        }

        void OnCombatGainsPlayerBtnCloseClickNew()
        {
            PanelManager.Instance.Close(_view.CombatGains.Player.root);
        }

        void OnCombatGainsOneGameBtnCloseClick()
        {
            SoundController.PlaySe(SoundController.SeID.WindowClose);
            PanelManager.Instance.Close(this.View.CombatGains.OneGame.root);
        }

        void OnBtnCharaStatisticsDetailClickNew()
        {
            this.PagesStatistics.SwitchTo(_view.CharaStatistics.Detail);
        }

        void OnBtnCharaStatisticsSliderClickNew()
        {
            this.PagesStatistics.SwitchTo(_view.CharaStatistics.Slider);
        }

        //btn combatgains baseitem
        void OnCombatGainsBaseItemClick(long battleId, long gameTime, int fieldId, int isWin, int type)
        {
            SoundController.PlaySe(SoundController.SeID.Select);
            //set result
            switch (isWin)
            {
                case 1:
                    this.View.CombatGains.OneGame.resultLeft.spriteName = "icon_win";
                    this.View.CombatGains.OneGame.resultRight.spriteName = "icon_lose";
                    break;
                case -1:
                    this.View.CombatGains.OneGame.resultLeft.spriteName = "icon_lose";
                    this.View.CombatGains.OneGame.resultRight.spriteName = "icon_win";
                    break;
                case 0:
                    this.View.CombatGains.OneGame.resultLeft.spriteName = "icon_draw";
                    this.View.CombatGains.OneGame.resultRight.spriteName = "icon_draw";
                    break;
            }

            //set game time
            this.View.CombatGains.OneGame.gameTime.text = CustomControl.ToolFunc.TimeSecondIntToString(gameTime, false);
            //set map
            BattleFieldMasterData data;
            BattleFieldMaster.Instance.TryGetMasterData(fieldId, out data);
            this.View.CombatGains.OneGame.gameMap.text = (BattleType)data.BattleType + "";

            StartCoroutine(OpenCombatGainsOneGame(battleId));
        }

        IEnumerator OpenCombatGainsOneGame(long battleId)
        {
            //loading
            yield return Net.Network.GetCombatGiansOneGameInfo(battleId, this.SyncCombatGainsOneGameModel);

            //open
            PanelManager.Instance.Open(_view.CombatGains.OneGame.root, false, false);
        }

        //btn combatgains one game item
        void OnCombatGainsOneGameItemClick(long playerId)
        {
            //=>Wait for new UI
            //LobbyPacket.SendPlayerMiscInfo(playerId, 1);
            //_view.CombatGains.Player.root.SetActive(true);
        }
    #endregion
    }
}
