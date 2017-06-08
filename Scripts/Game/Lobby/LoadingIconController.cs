using UnityEngine;
using System.Collections;

public class LoadingIconController : Singleton<LoadingIconController> {
    void Awake()
    {
        base.Awake();
        this.gameObject.SetActive(false);
    }

    public void Show()
    {
        this.gameObject.SetActive(true);
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }
}
