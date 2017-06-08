using UnityEngine;
using System.Collections;

public class GUIBaseViewManager : Singleton<GUIBaseViewManager> {
    void Awake()
    {
        base.Awake();
    }

    //panel layer
    public UIPanel GUIPanelBottomLayer;
    public UIPanel GUIPanelMiddleLayer;
    public UIPanel GUIPanelTopLayer;
}

public class TestViewManager : GUIBaseViewManager
{
    void Awake()
    {
       // base.GUIPanelBottomLayer = "";
    }
}
