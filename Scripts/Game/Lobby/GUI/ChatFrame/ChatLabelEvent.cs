using UnityEngine;
using System.Collections;
using Scm.Common.GameParameter;

public class ChatLabelEvent : MonoBehaviour {

    void OnClick()
    {
        UILabel test = GetComponent<UILabel>();
        string url = test.GetUrlAtPosition(UICamera.lastWorldPosition);
        if (!string.IsNullOrEmpty(url))
        {
            var str = url.Split('.');
            switch ((ChatType)int.Parse(str[0]))
            {
                case ChatType.Whisper:
                    XUI.GUIChatFrameController.Instance.OnChatTypeWis(long.Parse(str[1]), str[2]);
                    break;
                case ChatType.Say:
                    XUI.GUIChatFrameController.Instance.OnChatTypeSay();
                    break;
            }
            Debug.Log("PlayerId: " + url);
        }
    }

}
