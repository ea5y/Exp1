using UnityEngine;
using System.Collections;

public class GUITipMessage : Singleton<GUITipMessage> {
    public GameObject messagePrefab;
    public UIPanel panel;

    void Awake()
    {
        base.Awake();
    }

    public void Show(string message)
    {
        var go = NGUITools.AddChild(this.panel.gameObject, this.messagePrefab);
        var item = go.GetComponent<MessageItem>();
        item.message.text = message;
    }
}
