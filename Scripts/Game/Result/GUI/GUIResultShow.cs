using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Scm.Common.GameParameter;
using Scm.Common.Master;
using System;

namespace XUI
{
    public class GUIResultShow : Singleton<GUIResultShow>
    {
        public ResultView View;
        private BridgingResultInfo resultInfo;
        private MemberInfo myResult;
        private CharaIcon CharaIcon { get { return ScmParam.Lobby.CharaIcon; } }
        private Action actionNext;

        private bool isPlayExpSlider = false;
        private Dictionary<string, int> rewardDic = new Dictionary<string, int>();
        private List<List<ExpSliderData>> stepBuffer = new List<List<ExpSliderData>>();

        private int counter = 0;
        private Queue<List<ExpSliderData>> upSteps = new Queue<List<ExpSliderData>>();
        
        private CustomControl.SliderTool rankSliderTool;
        private CustomControl.SliderTool levelSliderTool;

        public class ExpSliderData : CustomControl.SliderData
        {
            public int starId;
        }

        #region All
        public void NextScene()
        {
            this.actionNext();
        }

        void Awake()
        {
            base.Awake();
            this.isPlayExpSlider = false;
            this.View.root.SetActive(false);
        }

        private void OnEnable()
        {
            this.counter = 0;
        }

        public void Show(BridgingResultInfo resultInfo)
        {
            this.resultInfo = resultInfo;
            this.myResult = this.GetMyResult();

            this.SetBtnNext();
            this.SetResultBase(this.GetResult(resultInfo.JudgeTypeClient), resultInfo.GameSecond, resultInfo.FieldId);
            this.SetResultMembers(resultInfo.MemberList);
            this.gameObject.SetActive(true);
        }

        private void SetBtnNext()
        {
            if (resultInfo.scoreType == ScoreType.Ranking)
            {
                this.actionNext = () => {
                    this.GotoRank();
                };
            }
            else
            {
                this.actionNext = () => {
                    this.GotoReward();
                };
            }
        }

        int GetResult(JudgeTypeClient type)
        {
            if (type == JudgeTypeClient.PlayerWin || type == JudgeTypeClient.PlayerCompleteWin)
                return 1;
            if (type == JudgeTypeClient.PlayerLose || type == JudgeTypeClient.PlayerCompleteLose)
                return -1;
            if (type == JudgeTypeClient.Draw)
                return 0;
            return 0;
        }

        public void SetResultBase(int isWin, long gameTime, int fieldId)
        {
            //set result
            this.View.allView.resultLeft.spriteName = isWin == 1 ? "icon_win" : "icon_lose";
            this.View.allView.resultRight.spriteName = isWin == 1 ? "icon_lose" : "icon_win";
            if (isWin == 0)
            {
                this.View.allView.resultLeft.spriteName = "icon_draw";
                this.View.allView.resultRight.spriteName = "icon_draw";
            }
            //set game time
            this.View.allView.gameTime.text = CustomControl.ToolFunc.TimeSecondIntToString(gameTime, false);
            //set map
            BattleFieldMasterData data;
            BattleFieldMaster.Instance.TryGetMasterData(fieldId, out data);
            this.View.allView.gameMap.text = (BattleType)data.BattleType + "";
        }

        public void SetResultMembers(List<MemberInfo> detail)
        {
            //get team
            var blueTeam = this.GetTeam(detail, TeamType.Blue);
            var redTeam = this.GetTeam(detail, TeamType.Red);


            //get my team
            var myTeam = this.GetMyTeam(detail);

            Debug.Log("myTeam: " + myTeam.ToString());

            //set team left
            this.SetTeam(this.View.allView.groupLeft, myTeam == TeamType.Blue ? blueTeam : redTeam, CharaIcon, true);

            //set team right
            this.SetTeam(this.View.allView.groupRight, myTeam == TeamType.Blue ? redTeam : blueTeam, CharaIcon, false);
        }

        TeamType GetMyTeam(List<MemberInfo> itemInfo)
        {
            int id = NetworkController.ServerValue.InFieldId;
            Debug.Log("MyInFielId: " + id);
            foreach (var info in itemInfo)
            {
                //info.avatarInfo.
                Debug.Log("FielId: " + info.avatarInfo.Id);
                if (info.inFieldID == id)
                    return (TeamType)info.teamType;
            }

            return 0;
        }

        private MemberInfo GetMyResult()
        {
            foreach (var info in this.resultInfo.MemberList)
            {
                if (info.inFieldID == NetworkController.ServerValue.InFieldId)
                {
                    return info;
                }
            }

            return null;
        }

        List<MemberInfo> GetTeam(List<MemberInfo> infoList, TeamType team)
        {
            List<MemberInfo> teamList = new List<MemberInfo>();
            foreach (var info in infoList)
            {
                if (team == (TeamType)info.teamType)
                {
                    teamList.Add(info);
                }
            }
            return teamList;
        }

        void SetTeam(GameObject widget, List<MemberInfo> teamInfoList, CharaIcon charaIcon, bool isLeft)
        {
            float spacing = 78;
            float posY = 213;
            //widget.DestroyChild();
            widget.DestroyChildImmediate();
            for (int i = 0; i < teamInfoList.Count; i++)
            {
                posY -= spacing;
                GameObject go = NGUITools.AddChild(widget, this.View.allView.itemInfo);
                go.transform.parent = widget.transform;
                go.transform.localPosition = new Vector3(-14, posY, 0);

                ItemCombatGainsOneGame item = go.GetComponent<ItemCombatGainsOneGame>();

                //set mvp
                item.mvp.SetActive(teamInfoList[i].battleRank == BattleRank.S);

                //set bg
                if (isLeft)
                {
                    item.bg.spriteName = "blue_bottom";
                    if (item.bgSelf.gameObject.activeSelf == true)
                        item.bgSelf.gameObject.SetActive(false);
                    if (NetworkController.ServerValue.PlayerId == teamInfoList[i].avatarInfo.Id)
                    {
                        item.bg.spriteName = "green_bottom";
                        item.bgSelf.gameObject.SetActive(true);
                    }
                    //item.bgSelf.
                }
                else
                {
                    if (item.bgSelf.gameObject.activeSelf == true)
                        item.bgSelf.gameObject.SetActive(false);
                    item.bg.spriteName = "red_bottom";
                }

                //set name
                item.nameL.text = teamInfoList[i].name;
                //set icon
                charaIcon.GetIcon(teamInfoList[i].avatarType, teamInfoList[i].skinId, false,
                        (UIAtlas res, string name) =>
                        {
                            item.icon.atlas = res;
                            item.icon.spriteName = name;
                        });
                //set kill times
                item.killTimes.text = teamInfoList[i].kill + "";
                //set dead times
                item.deadTimes.text = teamInfoList[i].death + "";
                //set damage
                item.damage.text = teamInfoList[i].attack + "";
                //set active time
                item.gameTime.text = CustomControl.ToolFunc.TimeSecondIntToString(teamInfoList[i].controlTime, false);

            }

        }
        #endregion

        #region Reward
        private void GotoReward()
        {
            this.Goto(this.View.allView.root, this.View.detailView.root);
            this.Goto(this.View.detailView.rankView.root, this.View.detailView.rewardView.root);

            this.SetLeft();
            this.SetReward();
        }

        private void SetLeft()
        {
            this.View.detailView.left.lblName.text = this.myResult.name;
            this.SetPortrait(myResult.avatarType, myResult.skinId);
        }

        public void SetPortrait(AvatarType avatarType, int skinId)
        {
            CharaBoard charaBoard = ScmParam.Lobby.CharaBoard;
            if (charaBoard != null)
            {
                charaBoard.GetBoard(avatarType, skinId, false,
                    (res) =>
                    {
                        if (res == null)
                            return;
                        var go = SafeObject.Instantiate(res) as GameObject;
                        if (go == null)
                            return;
                        go.name = res.name;
                        var t = go.transform;
                        t.parent = this.View.detailView.left.portrait.transform;
                        t.localPosition = Vector3.zero;
                        t.localScale = new Vector3(0.7f, 0.7f, 0.7f);
                    });
            }
        }

        private void SetReward()
        {
            this.SetCharas();
            this.SetJiangli();
        }

        private void SetJiangli()
        {
            this.rewardDic.Add("Gold", myResult.gainGold);
            this.rewardDic.Add("Coin", myResult.gainCoin);
            this.rewardDic.Add("Energy", myResult.gainEnergy);

            int counter = 0;
            foreach (var r in this.rewardDic)
            {
                if (this.rewardDic[r.Key] != 0)
                {
                    var reward = this.View.detailView.rewardView.root.transform.Find("Reward").Find("Reward_" + ++counter);
                    //reward.DestroyChildren();
                    reward.gameObject.DestroyChildImmediate();
                    var go = NGUITools.AddChild(reward.gameObject, this.View.detailView.rewardView.itemReward);
                    go.SetActive(true);
                    ItemReward item = go.GetComponent<ItemReward>();

                    switch (r.Key)
                    {
                        case "Gold":
                            item.icon.spriteName = "gold2";
                            item.num.text = "金币x" + this.rewardDic[r.Key];
                            break;
                        case "Coin":
                            item.icon.spriteName = "coin2";
                            item.num.text = "点劵x" + this.rewardDic[r.Key];
                            break;
                        case "Energy":
                            item.icon.spriteName = "tili";
                            item.num.text = "体力x" + this.rewardDic[r.Key];
                            break;
                    }
                }

            }

            if (myResult.rewardChara != null)
            {
                var reward = this.View.detailView.rewardView.root.transform.Find("Reward").Find("Reward_" + ++counter);
                //reward.DestroyChildren();
                reward.gameObject.DestroyChildImmediate();
                var go = NGUITools.AddChild(reward.gameObject, this.View.detailView.rewardView.itemReward);
                go.SetActive(true);
                ItemReward item = go.GetComponent<ItemReward>();

                CharaIcon.GetIcon((AvatarType)myResult.rewardChara.MasterId, 0, false, (a, s) =>
                {
                    item.icon.atlas = a;
                    item.icon.spriteName = s;
                });
                item.num.text = "角色x" + 1;
            }
        }
                
        private void SetCharas()
        {
            if (resultInfo.myCharas.Length == 0)
                return;
            for (int i = 1; i <= resultInfo.myCharas.Length; i++)
            {
                var charaInfo = resultInfo.myCharas[i - 1];
                var chara = this.View.detailView.rewardView.root.transform.Find("EXP").Find("Chara_" + i);
                //chara.DestroyChildren();
                chara.gameObject.DestroyChildImmediate();
                var go = NGUITools.AddChild(chara.gameObject, this.View.detailView.rewardView.itemChara);
                go.SetActive(true);

                //Icon
                ItemChara item = go.GetComponent<ItemChara>();
                item.icon.spriteName = "";
                AvatarMasterData adata = null;
                MasterData.TryGetAvatar(charaInfo.MasterId, charaInfo.SkinId, out adata);
                CharaIcon.GetBustIcon((AvatarType)adata.CharacterId, adata.ID, false, (a, s) =>
                {
                    item.icon.atlas = a;
                    item.icon.spriteName = s;
                });

                //init lv and fillAmount
                item.lv.text = CharaStarMaster.Instance.GetLevelByID(charaInfo.StarId) + "";
                item.expSlider.fillAmount = this.GetSliderCurAmount(charaInfo.Exp, charaInfo.StarId);

                //create data
                var slider = item.expSlider;
                var lv = item.lv;
                ExpSliderData data = new ExpSliderData();
                this.CreateExpSliderData(data, slider, lv, charaInfo.Exp, slider.fillAmount, myResult.gainExp, charaInfo.StarId, -1);

                //fill buffer
                if (this.levelSliderTool == null)
                    this.levelSliderTool = new CustomControl.SliderTool(1);
                this.levelSliderTool.FillStepBuffer(data);
                
            }
            
            this.levelSliderTool.AddToQueue();
            this.levelSliderTool.Enable = true;
        }

        private float GetSliderCurAmount(int curExp, int curStarId)
        {
            var expOverFlow = curExp - CharaStarMaster.Instance.GetBaseExp(curStarId);
            var needExp = this.GetNeedExp(curStarId);

            float amount = 0;
            if (needExp != 0)
                amount = (float)expOverFlow / needExp > 1.0f ? 1.0f : (float)expOverFlow / needExp;
            return amount;
        }

        private void CreateExpSliderData(ExpSliderData data, UISprite slider, UILabel lv, int curExp, float fillAmount, int addExp, int starId, int index)
        {
            if (index == -1)
                index = 0;
            data.index = index;
            data.slider = slider;
            data.lblLv = lv;
            data.nextLv = this.GetNextLv(starId);
            data.starId = starId;
            data.amount = this.GetAmount(fillAmount, starId);
            data.hook = data.amount == -1 ? false : true;
            data.residueExp = this.GetResidueExp(curExp, addExp, starId);
            data.curExpAfterAdd = this.GetCurExpAfterAdd(curExp, addExp, starId);
            data.totalAmount = this.GetTotalAmount(fillAmount, addExp, starId);
            if (data.totalAmount >= 1)
            {
                data.data = new ExpSliderData();
                CreateExpSliderData((ExpSliderData)data.data, data.slider, data.lblLv, (int)data.curExpAfterAdd, 0, (int)data.residueExp, this.GetNextStarId(starId), ++index);
            }
        }

        private float GetAmount(float amount, int starId)
        {
            float result = amount;
            var needExp = this.GetNeedExp(starId);
            if (needExp == 0)
                result = -1;
            return result;
        }

        private float GetResidueExp(int curExp, int addExp, int starId)
        {
            var overExp = curExp - CharaStarMaster.Instance.GetBaseExp(starId);
            var needExp = this.GetNeedExp(starId);
            float result = 0;
            if (needExp != 0)
            {
                result = addExp + overExp - needExp;
            }
            result = result > 0 ? result : 0;
            return result;
        }

        private float GetCurExpAfterAdd(int curExp, int addExp, int starId)
        {
            var overExp = curExp - CharaStarMaster.Instance.GetBaseExp(starId);
            var needExp = this.GetNeedExp(starId);

            CharaStarMasterData starData;
            MasterData.TryGetCharaStarMasterData(starId, out starData);

            float result = 0;
            if (starData.CanLevelUp)
            {
                result = overExp + addExp > needExp ? needExp + curExp - overExp : curExp + addExp;
            }
            else
            {
                result = curExp;
            }
            return result;
        }

        private int GetNextStarId(int starId)
        {
            return CharaStarMaster.Instance.GetNextStarId(starId);
        }

        private float GetNeedExp(int starId)
        {
            return CharaStarMaster.Instance.GetDeltaExp(starId);
        }

        private float GetTotalAmount(float fillAmount, int addExp, int starId)
        {
            var needExp = this.GetNeedExp(starId);

            float result = 0;
            if (needExp != 0)
            {
                result = fillAmount + (float)addExp / needExp;
            }
            result = result >= 1 ? 1 : result;
            return result;
        }             
        
        private int GetNextLv(int starId)
        {
            int nextStarId = this.GetNextStarId(starId);
            int lv = CharaStarMaster.Instance.GetLevelByID(nextStarId);
            return lv;
        }

        #endregion
        
        #region Rank
        public void GotoRank()
        {
            this.Goto(this.View.allView.root, this.View.detailView.root);
            this.Goto(this.View.detailView.rewardView.root, this.View.detailView.rankView.root);
            this.SetRank();
        }

        private void SetRank()
        {
            this.SetPortrait(myResult.avatarType, myResult.skinId);
            this.SetRankIcon();
            this.SetFillScore();
            this.SetAddScore();
            this.SetTotalScore();
        }

        private void SetRankIcon()
        {
            this.View.detailView.rankView.rankIcon.spriteName = PlayerRankMaster.Instance.GetRankByScore(myResult.score - myResult.gainScore) + "";
        }

        private void SetTotalScore()
        {
            this.View.detailView.rankView.totalScore.text = PlayerRankMaster.Instance.GetRankByScore(myResult.score - myResult.gainScore) + "";
        }

        private void SetAddScore()
        {
            this.View.detailView.rankView.addScore.text = Math.Sign(myResult.gainScore) > 0 ? "+" + myResult.gainScore : myResult.gainScore + "";
        }

        private void SetFillScore()
        {
            var rank = PlayerRankMaster.Instance.GetRankByScore(myResult.score - myResult.gainScore);
            
            this.SetCurScore(myResult.score - myResult.gainScore, rank);
            this.UpScore(rank);
        }

        private void SetCurScore(long curScore, int rank)
        {
            var overScore = this.GetOverScore(curScore, rank);
            var needScore = this.GetNeedScore(rank);
            this.View.detailView.rankView.fillScore.fillAmount = (float)overScore / needScore * (float)0.755;
        }

        private void UpScore(int rank)
        {
            if (this.rankSliderTool == null)
            {
                this.rankSliderTool = new CustomControl.SliderTool(0.755f);
                this.rankSliderTool.Speed = 0.2f;
            }
                
            this.rankSliderTool.IsUp = Math.Sign(myResult.gainScore) < 0 ? false : true;

            CustomControl.SliderData data = new CustomControl.SliderData();
            if (this.rankSliderTool.IsUp)
                this.CreateRankSliderUpData(data, this.View.detailView.rankView.fillScore, this.View.detailView.rankView.totalScore, this.View.detailView.rankView.rankIcon, myResult.score - myResult.gainScore, this.View.detailView.rankView.fillScore.fillAmount, myResult.gainScore, rank, 0);
            else
                this.CreateRankSliderDownData(data, this.View.detailView.rankView.fillScore, this.View.detailView.rankView.totalScore, this.View.detailView.rankView.rankIcon, myResult.score - myResult.gainScore, this.View.detailView.rankView.fillScore.fillAmount, myResult.gainScore, rank, 0);

            this.rankSliderTool.FillStepBuffer(data);
            this.rankSliderTool.AddToQueue();
            this.rankSliderTool.Enable = true;
        }

        #region RankDown
        private void CreateRankSliderDownData(CustomControl.SliderData data, UISprite slider, UILabel lblLv, UISprite imgLv, long curScore, float fillAmount, long addScore, int rank, int index)
        {
            data.index = index;
            data.slider = slider;
            data.imgLv = imgLv;
            data.nextLv = this.GetNextRankDown(rank);
            data.amount = fillAmount;
            data.hook = this.GetHookDown(rank);
            data.residueExp = this.GetResidueScoreDown(curScore, addScore, rank);
            data.curExpAfterAdd = this.GetCurScoreAfterAddDown(curScore, addScore, rank);
            data.totalAmount = this.GetRankTotalAmountDown(fillAmount, addScore, rank);
            if (data.totalAmount <= 0)
            {
                data.data = new CustomControl.SliderData();
                CreateRankSliderDownData((CustomControl.SliderData)data.data, data.slider, data.lblLv, data.imgLv, (int)data.curExpAfterAdd, 0, (int)data.residueExp, data.nextLv, ++index);
            }
        }

        private float GetRankTotalAmountDown(float fillAmount, long addScore, int rank)
        {
            var needScore = this.GetNeedScore(rank);

            float result = 0;
            result = fillAmount - Math.Abs((float)addScore) / needScore * this.rankSliderTool.MaxAmount;
            result = result < 0 ? 0 : result;
            return result;
        }

        private float GetCurScoreAfterAddDown(long curScore, long addScore, int rank)
        {
            float overScore = this.GetOverScore(curScore, rank);
            float result = 0;
            if (rank - 1 != 0)
                result = overScore + addScore < 0 ? curScore - overScore : curScore + addScore;
            else
                result = overScore + addScore < 0 ? 0 : curScore + addScore;
            return result;
        }

        private float GetResidueScoreDown(long curScore, long addScore, int rank)
        {
            float overScore = this.GetOverScore(curScore, rank);
            float result = 0;
            result = addScore + overScore;
            result = result < 0 ? result : 0;
            return result;
        }

        private bool GetHookDown(int rank)
        {
            //var result = rank - 1;
            return rank - 1 < 0 ? false : true;
        }

        private int GetNextRankDown(int rank)
        {
            var result = rank - 1;
            result = result < 0 ? 1 : result;
            return result;
        }
        #endregion

        #region RankUp
        private void CreateRankSliderUpData(CustomControl.SliderData data, UISprite slider, UILabel lblLv, UISprite imgLv, long curScore, float fillAmount, long addScore, int rank, int index)
        {
            data.index = index;
            data.slider = slider;
            data.lblLv = lblLv;
            data.imgLv = imgLv;
            data.nextLv = this.GetNextRankUp(rank);
            data.amount = fillAmount;
            data.hook = this.GetHookUp(rank);
            data.residueExp = this.GetResidueScoreUp(curScore, addScore, rank);
            data.curExpAfterAdd = this.GetCurScoreAfterAddUp(curScore, addScore, rank);
            data.totalAmount = this.GetRankTotalAmountUp(fillAmount, addScore, rank);
            if (data.totalAmount >= this.rankSliderTool.MaxAmount)
            {
                data.data = new CustomControl.SliderData();
                CreateRankSliderUpData((CustomControl.SliderData)data.data, data.slider, data.lblLv, data.imgLv, (int)data.curExpAfterAdd, 0, (int)data.residueExp, data.nextLv, ++index);
            }
        }

        private int GetNextRankUp(int rank)
        {
            var nextScore = PlayerRankMaster.Instance.GetNextScore(rank);
            return PlayerRankMaster.Instance.GetRankByScore((long)nextScore);
        }

        private bool GetHookUp(int rank)
        {
            bool result = true;
            var nextScore = PlayerRankMaster.Instance.GetNextScore(rank);
            if (nextScore == -1)
                result = false;
            return result;
        }

        private float GetResidueScoreUp(long curScore, long addScore, int rank)
        {
            float overScore = this.GetOverScore(curScore, rank);
            var nextScore = this.GetNeedScore(rank);

            float result = 0;
            if (nextScore != -1)
                result = addScore + overScore - nextScore;
            result = result > 0 ? result : 0;
            return result;
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
            if (rank == 1)
                result = PlayerRankMaster.Instance.GetNextScore(rank);
            else
                result = PlayerRankMaster.Instance.GetNextScore(rank) - PlayerRankMaster.Instance.GetNextScore(--rank);
            return result;
        }

        private float GetCurScoreAfterAddUp(long curScore, long addScore, int rank)
        {
            float overScore = this.GetOverScore(curScore, rank);
            var needScore = this.GetNeedScore(rank);

            float result = 0;
            if (needScore != -1)
            {
                result = overScore + addScore > needScore ? needScore + curScore - overScore : curScore + addScore;
            }
            else
            {
                result = curScore;
            }
            return result;
        }
        
        private float GetRankTotalAmountUp(float fillAmount, long addScore, int rank)
        {
            var needScore = this.GetNeedScore(rank);

            float result = 0;
            if (needScore != -1)
            {
                result = fillAmount + (float)addScore / needScore * this.rankSliderTool.MaxAmount;
            }
            result = result >= this.rankSliderTool.MaxAmount ? this.rankSliderTool.MaxAmount : result;
            return result;
        }
        #endregion        
        
        #endregion

        private void Update()
        {
            if (this.levelSliderTool != null)
                this.levelSliderTool.Update();

            if (this.rankSliderTool != null)
                this.rankSliderTool.Update();
        }

        public void GotoLobby()
        {
            this.gameObject.SetActive(false);
            ResultMain.GotoNextScene();
        }

        private void Goto(GameObject source, GameObject targ)
        {
            source.SetActive(false);
            targ.SetActive(true);
        }
    }    
}

