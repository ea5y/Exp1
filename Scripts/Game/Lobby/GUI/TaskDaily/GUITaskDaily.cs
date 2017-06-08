using UnityEngine;
using System.Collections;
using System;
using XDATA;
namespace XUI
{
    public class GUITaskDaily : Singleton<GUITaskDaily>
    {
        public TaskDailyView view;

        private CustomControl.ScrollView<ItemTaskDaily> scrollView;

        private void Awake()
        {
            base.Awake();
            this.HideFirst();
        }

        private void HideFirst()
        {
            this.gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            PlayerData.Instance.OnTasksChange -= this.SetTaskScrollView;
        }

        public void Open()
        {
            Net.Network.Instance.StartCoroutine(this._Open());
        }

        private IEnumerator _Open()
        {
            PlayerData.Instance.OnTasksChange += this.SetTaskScrollView;
            yield return PlayerData.Instance.GetTasks();

            PanelManager.Instance.Open(this.view.root);
        }

        public void Close()
        {
            SoundController.PlaySe(SoundController.SeID.WindowClose);
            PanelManager.Instance.Close(this.view.root);
        }

        private void SetTaskScrollView(object sender, EventArgs e)
        {
            if (this.scrollView == null)
                this.scrollView = new CustomControl.ScrollView<ItemTaskDaily>(this.view.grid, this.view.itemTask);

            this.scrollView.CreateWeight(PlayerData.Instance.taskDailyList);
        }
    }
}

