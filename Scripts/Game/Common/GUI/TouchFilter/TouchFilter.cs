using UnityEngine;
using System.Collections;

public class TouchFilter : Singleton<TouchFilter> {
    private void Awake()
    {
        base.Awake();
        this.gameObject.SetActive(false);
    }

    public void Enable(bool isEnable)
    {
        this.gameObject.SetActive(isEnable);
    }     
}
