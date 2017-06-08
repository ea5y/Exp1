using UnityEngine;
using System.Collections;
using Scm.Common.Packet;
using Scm.Common.Master;

namespace XUI
{
    public class ItemTaskDaily : CustomControl.ScrollViewItem
    {
        private TaskDaily taskData;
        
        public UILabel taskName;

        public UISprite rewardIcon;
        public UILabel rewardCount;

        public GameObject groupProcess;
        public UISprite imgProcess;
        public UILabel lblProcess;

        public UIButton btnRecieve;
        public UILabel lblDone;


        public override void FillItem(IList datas, int index)
        {
            base.FillItem(datas, index);
            
            this.GetData(datas, index);
            
            this.SetName();
            this.SetReward();
            this.SetStatus();
            this.SetEvent();            
        }

        private void GetData(IList datas, int index)
        {
            this.taskData = (TaskDaily)datas[index];
        }
        
        private void SetName()
        {
            this.taskName.text = this.taskData.TaskType;
        }

        private void SetReward()
        {            
            string iconName = "";
            
            switch (this.taskData.RewardType)
            {
                case TaskDaily.DailyRewardType.Gold:
                    iconName = "jinbi";
                    break;
                case TaskDaily.DailyRewardType.Coin:
                    iconName = "dianquan";
                    break;
                case TaskDaily.DailyRewardType.Character:
                    iconName = "wenhao";
                    break;
                case TaskDaily.DailyRewardType.Energy:
                    iconName = "tili";
                    break;
            }

            this.rewardIcon.spriteName = iconName;
            this.rewardCount.text = this.taskData.RewardCount + "";
        }

        private void SetStatus()
        {
            switch (this.taskData.Status)
            {
                case TaskDaily.TaskDailyStatus.Active:
                    this.groupProcess.gameObject.SetActive(true);
                    this.btnRecieve.gameObject.SetActive(false);
                    this.lblDone.gameObject.SetActive(false);
                    break;
                case TaskDaily.TaskDailyStatus.Completed:
                    this.groupProcess.gameObject.SetActive(false);
                    this.btnRecieve.gameObject.SetActive(true);
                    this.lblDone.gameObject.SetActive(false);
                    break;
                case TaskDaily.TaskDailyStatus.Comfirmed:
                    this.groupProcess.gameObject.SetActive(false);
                    this.btnRecieve.gameObject.SetActive(false);
                    this.lblDone.gameObject.SetActive(true);
                    break;
            }

            this.SetProcess();
        }

        private void SetProcess()
        {
            this.lblProcess.text = this.taskData.Process + "/" + this.taskData.ProcessTotal;
            this.imgProcess.fillAmount = (float)this.taskData.Process / this.taskData.ProcessTotal;
        }

        private void SetEvent()
        {
            this.btnRecieve.onClick.Clear();
            EventDelegate.Add(this.btnRecieve.onClick, () => {
                this.GetRaward();
            });
        }

        private void GetRaward()
        {
            SoundController.PlaySe(SoundController.SeID.Button);
            StartCoroutine(Net.Network.GetTaskReward(this.taskData.Id, (res) => {
                this.taskData.Status = TaskDaily.TaskDailyStatus.Comfirmed;
                this.SetStatus();
            }));
        }
    }

}
