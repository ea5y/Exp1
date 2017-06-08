using UnityEngine;
using System.Collections;

namespace XUI
{
    [System.Serializable]
    public class TaskDailyView : Singleton<TaskDailyView>
    {
        public GameObject root;
        public UIButton btnClose;

        public UIGrid grid;
        public GameObject itemTask;
    }

}
