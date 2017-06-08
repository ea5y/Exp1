using UnityEngine;
using System.Collections;

public class IntroduceItemView : MonoBehaviour {
    public GameObject goLock;
    public UISprite back;
    public UILabel title;
    public UIButton buttonSelect;

	// Use this for initialization
	void Start () {
        EventDelegate.Add(buttonSelect.onClick, OnSelect);
	}
	
	// Update is called once per frame
    public void Setup(CharacterInfoModel model)
    {
	
	}

    void OnSelect()
    { 
    
    }
}
