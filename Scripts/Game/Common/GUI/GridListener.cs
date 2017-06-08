using UnityEngine;
using System.Collections;

public class GridListener : MonoBehaviour
{
    public UIScrollView ScrollView;
    private UIGrid Grid;
    private Vector3 scrollViewInitPos;
    private Vector2 scrollViewOffset;
	// Use this for initialization
	void Awake ()
	{
	    if (null != ScrollView)
	    {
	        scrollViewInitPos = ScrollView.transform.localPosition;
	        scrollViewOffset = ScrollView.panel.clipOffset;
	    }
	    Grid = gameObject.GetComponent<UIGrid>();
	    Grid.onReposition = OnReposition;
	}

    private void OnReposition()
    {
        if (!gameObject.activeInHierarchy)
        {
            return;
        }
//        Debug.Log("Reposition");
        int i = 0;
        foreach (Transform item in transform)
        {
            if (item.gameObject.activeSelf)
            {
                item.SendMessage("OnReposition", i++);
            }
        }
        if (null != ScrollView)
        {
            SpringPanel t = ScrollView.GetComponent<SpringPanel>();
            if (null != t)
            {
                t.target = scrollViewInitPos;
            }
            ScrollView.transform.localPosition = scrollViewInitPos;
            ScrollView.panel.clipOffset = scrollViewOffset;
        }
    }
}
