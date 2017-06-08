using UnityEngine;
using System.Collections;
using XUI;

public class ChatBattle : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnClickChat()
    {
        GUIChatFrameController.Instance.Show();
    }
}
