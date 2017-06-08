//author: luwanzhong
//date: 2016-11-18
//desc: panel manager 

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Scm.Common.Packet;
using Scm.Client;
using Asobimo.Photon.Packet;

public class PanelManager : Singleton<PanelManager>
{
    /// <summary>
    /// LayerList
    /// </summary>
    [SerializeField]
    public List<GameObject> layerList = new List<GameObject>();

    /// <summary>
    /// Awake
    /// </summary>
    void Awake()
    {
        base.Awake();
    }

    /// <summary>
    /// Open without senReq
    /// </summary>
    /// <param name="view"></param>
    /// <param name="isMutex"></param>
    /// <param name="prevHide"></param>
    public void Open(GameObject view, bool isMutex = false, bool prevHide = false)
    {
        if (view.activeSelf == false)
        {
            //if mutex
            if (isMutex)
            {
                if (this.layerList.Count != 0)
                {
                    for(int i = 0; i < this.layerList.Count; i++)
                    {
                        var iPanel = this.layerList[i].GetComponent<IPanel>();
                        if(iPanel != null)
                            iPanel.Reset();
                        this.layerList[i].SetActive(false);
                    }

                    while(this.layerList.Count != 0)
                        this.layerList.RemoveAt(0);
                }

                view.gameObject.SetActive(true);
                this.layerList.Add(view);
            }
            else
            {
                if (prevHide)
                {
                    this.layerList[this.layerList.Count - 1].SetActive(false);
                }
                view.gameObject.SetActive(true);
                this.layerList.Add(view);
            }
        }

    }

    public void Close(GameObject view)
    {
        view.gameObject.SetActive(false);
        PanelManager.Instance.layerList.Remove(view);
    }

    public void Back()
    {
        if (PanelManager.Instance.layerList.Count == 0)
            return;

        var iPanel = this.layerList[this.layerList.Count - 1].GetComponent<IPanel>();
        if(iPanel != null)
            StartCoroutine(iPanel.Reverse());

        this.Close(this.layerList[this.layerList.Count - 1]);

        if (PanelManager.Instance.layerList.Count == 0)
            return;
        if (PanelManager.Instance.layerList[PanelManager.Instance.layerList.Count - 1].activeSelf == false)
            PanelManager.Instance.layerList[PanelManager.Instance.layerList.Count - 1].SetActive(true);
    }
    
    public IEnumerator BackWithTween()
    {
        if (PanelManager.Instance.layerList.Count == 0)
            yield break;

        var iPanel = this.layerList[this.layerList.Count - 1].GetComponent<IPanel>();
        if(iPanel != null)
            yield return iPanel.Reverse();

        Debug.Log("On: " + this.layerList.Count);
        this.Close(this.layerList[this.layerList.Count - 1]);

        Debug.Log("Off: " + this.layerList.Count);

        if (PanelManager.Instance.layerList.Count == 0)
            yield break;
        if (PanelManager.Instance.layerList[PanelManager.Instance.layerList.Count - 1].activeSelf == false)
            PanelManager.Instance.layerList[PanelManager.Instance.layerList.Count - 1].SetActive(true);
    }

    /// <summary>
    /// To Home
    /// </summary>
    /// <param name="root"></param>
    static public void Home(GameObject root)
    {
       
    }
}

public interface IPanel
{
    void Open();
    void Close();

    void Forward();
    IEnumerator Reverse();

    void Reset();
}

public class PanelBase<T> : MonoBehaviour, IPanel where T : MonoBehaviour
{ 
    public static T Instance {get;private set;}
    public bool isMutex = false;

    public static void Unload()
    {
        Instance = null;
    }

    protected virtual void Awake()
    {
        if(PanelBase<T>.Instance == null)
            PanelBase<T>.Instance = this as T;
        else
            Debug.LogError(typeof(T) + ":单例冲突！");
    }

    public virtual void Open()
    {

    }

    public virtual void Close()
    {

    }

    public virtual void Forward()
    {

    }

    public virtual IEnumerator Reverse()
    {
        yield return null;
    }

    public virtual void Reset()
    {

    }
}

