using UnityEngine;
using System.Collections;
using CustomControl;
using System.Collections.Generic;
using Scm.Common.GameParameter;
using Scm.Common.NGWord;

namespace XUI
{
    public class GUIChatFrameController : Singleton<GUIChatFrameController>
    {
        #region View
        [SerializeField]
        ChatFrameView view;
        public ChatFrameView View { get { return this.view; } }

        [System.Serializable]
        public class ChatFrameView
        {
            public GameObject root;

            [SerializeField]
            BaseView baseView;
            public BaseView Base { get { return this.baseView; } }

            [SerializeField]
            AllView pageSay;
            public AllView PageSay { get { return this.pageSay; } }

            [SerializeField]
            PrivateView pageWhisper;
            public PrivateView PageWhisper { get { return this.pageWhisper; } }

            [SerializeField]
            TeamView pageTeam;
            public TeamView PageTeam { get { return this.pageTeam; } }

            public UILabel lblChatPop;
            public TweenAlpha tweenChatPop;
        }

        [System.Serializable]
        public class BaseView
        {
            public UIButton close;
            public UIButton btnSay;
            public UIButton btnWhiper;
            public UIButton btnTeam;

            public UILabel chatType;
            public UILabel chatTarget;
            public UIInput input;
            public UIButton btnSend;
        }

        [System.Serializable]
        public class AllView : IPage
        {
            public GameObject root;
            public UITextList textList;

            public GameObject Root
            {
                get { return this.root; }
            }

            public void Init()
            {
                //None
            }
        }

        [System.Serializable]
        public class PrivateView : IPage
        {
            public GameObject root;
            public UITextList textList;

            public GameObject Root
            {
                get { return this.root; }
            }

            public void Init()
            {
                //None
            }
        }

        [System.Serializable]
        public class TeamView : IPage
        {
            public GameObject root;
            public UITextList textList;

            public GameObject Root
            {
                get { return this.root; }
            }

            public void Init()
            {
                //None
            }
        }
        #endregion

        #region Controller
        protected  override void Awake()
        {
            base.Awake();
            this.Init();
            this.View.root.SetActive(false);
        }

        void Update()
        {
            if (this.ChatWordChacheClearFiber != null)
            {
                if (!this.ChatWordChacheClearFiber.Update())
                {
                    this.ChatWordChacheClearFiber = null;
                }
            }

            if(this.View.root.gameObject.activeSelf == false && this.View.lblChatPop.alpha == 0)
            {
                while(this.chatPopQueue.Count > 1)
                {
                    this.chatPopQueue.Dequeue();
                }
                if(this.chatPopQueue.Count != 0)
                {
                    var chatInfo = this.chatPopQueue.Dequeue();
                    this.ChatPop(chatInfo);
                }
            }
        }

        private void ChatPop(ChatInfo chatInfo)
        {
            string text = "";

            switch (chatInfo.chatType)
            {
                case ChatType.Whisper:
                    var name = chatInfo.name.Split(':');
                    var relName = name[0].Substring(9, name[0].Length - 10);
                    text = chatInfo.type + "[url=" + (int)ChatType.Whisper + "." + chatInfo.playerID + "." + relName + "." + "]" + " " + name[0] + "[/url] " + "" + "悄悄地对你说:" + " " + chatInfo.text;
                    break;
                case ChatType.Say:
                    Debug.Log("Add : " + chatInfo.text);
                    var name1 = chatInfo.name.Split(':');
                    var relName1 = name1[0].Substring(9, name1[0].Length - 10);
                    text = chatInfo.type + "[url=" + (int)ChatType.Whisper + "." + chatInfo.playerID + "." + relName1 + "." + "]" + name1[0] + "[/url] " + ":" + " " + chatInfo.text;
                    break;
                case ChatType.Team:
                    break;
                case ChatType.Shout:
                    break;
                case ChatType.System:
                    text = chatInfo.name + chatInfo.text;
                    break;
                default:
                    text = chatInfo.name + chatInfo.text;
                    break;
            }

            this.View.lblChatPop.text = text;
            this.ChatPopTweenForward();
        }

        private void ConfigChatPopTweenForward()
        {
            this.View.tweenChatPop.onFinished.Clear();
            EventDelegate.Add(this.View.tweenChatPop.onFinished, this.ChatPopTweenReverse);
        }

        private void ConfigChatPopTweenReverse()
        {
            this.View.tweenChatPop.onFinished.Clear();
        }

        private void ChatPopTweenForward()
        {
            this.ConfigChatPopTweenForward();
            this.View.tweenChatPop.PlayForward();
        }

        private void ChatPopTweenReverse()
        {
            this.ConfigChatPopTweenReverse();
            this.View.tweenChatPop.PlayReverse();
        }

        TabPagesManager pages;
        TabPagesManager Pages { get { return this.pages; } set { this.pages = value; } }

        List<UIButton> tabMenu;
        List<UIButton> TabMenu { get { return this.tabMenu; } set { this.tabMenu = value; } }

        List<string> ChatWordChache { get; set; }

        //Coninuity send the same chat 3 times will be forbid
        static readonly int ConinuityForbidNum = 3;

        //Chache clear time
        static readonly float ChatWordChacheClearTime = 5f;

        //
        Fiber ChatWordChacheClearFiber { get; set; }

        //Input with
        int InputWidth;

        //Chat show
        private Queue<ChatInfo> chatPopQueue = new Queue<ChatInfo>();

        enum PageType
        {
            Say,
            Whisper,
            Team
        }

        public void Show()
        {
            this.View.root.SetActive(true);
            //gameObject.SetActive(true);
        }

        void Init()
        {
            this.TabMenu = new List<UIButton>();
            this.TabMenu.Add(this.View.Base.btnSay);
            this.TabMenu.Add(this.View.Base.btnWhiper);
            this.TabMenu.Add(this.View.Base.btnTeam);

            this.Pages = new TabPagesManager();
            this.Pages.AddPage(this.View.PageSay);
            this.Pages.AddPage(this.View.PageWhisper);
            this.Pages.AddPage(this.View.PageTeam);

            this.ChatWordChache = new List<string>();
            this.SetChatType(ChatType.Say);

            this.InputWidth = this.View.Base.input.label.width;
        }

        void SwitchTo(PageType page)
        {

            switch (page)
            {
                case PageType.Say:
                    this.Pages.SwitchTo(this.View.PageSay);
                    ToolFunc.BtnSwitchTo(this.View.Base.btnSay, this.TabMenu);
                    break;
                case PageType.Whisper:
                    this.Pages.SwitchTo(this.View.PageWhisper);
                    ToolFunc.BtnSwitchTo(this.View.Base.btnWhiper, this.TabMenu);
                    break;
                case PageType.Team:
                    this.Pages.SwitchTo(this.View.PageTeam);
                    ToolFunc.BtnSwitchTo(this.View.Base.btnTeam, this.TabMenu);
                    break;
            }
        }


        #region Color
        //SerializeField chat type Color
        [SerializeField]
        ChatColorCode _chatColor;
        ChatColorCode ChatColor { get { return _chatColor; } }
        [System.Serializable]
        public class ChatColorCode
        {
            public Color say = NGUIText.ParseColor("FFFFFF", 0);
            public Color guild = NGUIText.ParseColor("00FFFF", 0);
            public Color team = NGUIText.ParseColor("0000FF", 0);
            public Color whisper = NGUIText.ParseColor("FFC0CB", 0);
            public Color shout = NGUIText.ParseColor("FF0000", 0);
            public Color adminYell = NGUIText.ParseColor("FFFF00", 0);
            public Color adminGuild = NGUIText.ParseColor("FFFF00", 0);
            public Color adminTeam = NGUIText.ParseColor("FFFF00", 0);
            public Color adminWhisper = NGUIText.ParseColor("FFFF00", 0);
            public Color adminShout = NGUIText.ParseColor("FFFF00", 0);
            public Color system = NGUIText.ParseColor("FF7800", 0);
            public Color error = NGUIText.ParseColor("FF0000", 0);
        }

        static Dictionary<ChatType, string> ColorCodeDict { get; set; }
        static string ErrorColorCode { get; set; }
        static bool CreateColorCode()
        {
            if (Instance == null)
            {
                Debug.LogWarning("Not Found! GUIChatFrameController.Instance");
                return false;
            }

            var cc = Instance.ChatColor;

            // マスターデータからカラーデータを読み込む
            cc.say = MasterData.GetColor(ColorType.C014_ChatSay);
            cc.guild = MasterData.GetColor(ColorType.C015_ChatGuild);
            cc.team = MasterData.GetColor(ColorType.C016_ChatTeam);
            cc.whisper = MasterData.GetColor(ColorType.C017_ChatWhisper);
            cc.shout = MasterData.GetColor(ColorType.C018_ChatShout);
            cc.adminYell = MasterData.GetColor(ColorType.C019_ChatAdminYell);
            cc.adminGuild = MasterData.GetColor(ColorType.C020_ChatAdminGuild);
            cc.adminTeam = MasterData.GetColor(ColorType.C021_ChatAdminTeam);
            cc.adminWhisper = MasterData.GetColor(ColorType.C022_ChatAdminWhisper);
            cc.adminShout = MasterData.GetColor(ColorType.C023_ChatAdminShout);
            cc.system = MasterData.GetColor(ColorType.C024_ChatSystem);
            cc.error = MasterData.GetColor(ColorType.C025_ChatError);

            // カラーコードを追加する
            ColorCodeDict = new Dictionary<ChatType, string>();
            ColorCodeDict.Add(ChatType.Say, NGUIText.EncodeColor(cc.say));
            ColorCodeDict.Add(ChatType.Guild, NGUIText.EncodeColor(cc.guild));
            ColorCodeDict.Add(ChatType.Team, NGUIText.EncodeColor(cc.team));
            ColorCodeDict.Add(ChatType.Whisper, NGUIText.EncodeColor(cc.whisper));
            ColorCodeDict.Add(ChatType.Shout, NGUIText.EncodeColor(cc.shout));
            ColorCodeDict.Add(ChatType.AdminYell, NGUIText.EncodeColor(cc.adminYell));
            ColorCodeDict.Add(ChatType.AdminGuild, NGUIText.EncodeColor(cc.adminGuild));
            ColorCodeDict.Add(ChatType.AdminTeam, NGUIText.EncodeColor(cc.adminTeam));
            ColorCodeDict.Add(ChatType.AdminWhisper, NGUIText.EncodeColor(cc.adminWhisper));
            ColorCodeDict.Add(ChatType.AdminShout, NGUIText.EncodeColor(cc.adminShout));
            ColorCodeDict.Add(ChatType.System, NGUIText.EncodeColor(cc.system));
            ErrorColorCode = NGUIText.EncodeColor(cc.error);

            return true;
        }

        public static string AddColorCode(string text, ChatType chatType)
        {
            if (ColorCodeDict == null)
            {
                if (!CreateColorCode())
                    return text;
            }

            string colorCode;
            if (!ColorCodeDict.TryGetValue(chatType, out colorCode))
            {
                Debug.LogWarning("Not Found! ChatType");
                return text;
            }

            return string.Format("[{0}]{1}[-]", colorCode, text);
        }
        #endregion

        #region Btn Event
        public void OnBtnSayClick()
        {
            this.OnChatTypeSay();
        }

        public void OnBtnWhisperClick()
        {
            this.OnChatTypeWis(this.WhisperPlayerId, this.WhisperPlayerName);
        }

        public void OnBtnTeamClick()
        {
            //TODO
        }

        public void OnCloseClick()
        {
            this.View.root.SetActive(false);
            //gameObject.SetActive(false);
        }

        public void OnSendClick()
        {
            //string rawText = UIInput.current.value;
            string rawText = this.View.Base.input.value;
            // Filter
            string text = ApplicationController.Language == Language.Japanese ? NGWord.DeleteNGWord(rawText) : FilterWordController.Instance.GetFilteredWord(rawText);

            // 文字を消してフォーカスを外す
            //UIInput.current.value = "";
            //UIInput.current.RemoveFocus();

            this.view.Base.input.value = "";
            this.View.Base.input.isSelected = false;

            // チャット送信
            this._SendChat(text, rawText);
        }
        #endregion

        #region Send Chat
        void _SendChat(string text, string rawText)
        {
            // 入力文字がないなら何もしない
            if (string.IsNullOrEmpty(text))
                return;

            // 文字制限以上なら削除
            if (this.View.Base.input != null)
            {
                int textMax = this.View.Base.input.characterLimit;
                if (textMax < text.Length)
                {
                    int sIdx = textMax - 1;
                    int cnt = text.Length - textMax;
                    text = text.Remove(sIdx, cnt);
                }
            }

            // 連続して同じ発言をしているかチェック
            if (IsContinuityChat(text))
            {
                AddMessage(new ChatInfo { playerID = 0, chatType = Scm.Common.GameParameter.ChatType.System, name = "System", text = MasterData.GetText(TextType.TX138_Chat_ContinuityNoticeStr) });
                return;
            }

#if UNITY_EDITOR || XW_DEBUG
            if (!Scm.Client.GameListener.ConnectFlg)
            {
                AddMessage(new ChatInfo { playerID = 0, chatType = this.ChatType, name = "Name", text = text });
                return;
            }
#endif
#if XW_DEBUG || ENABLE_GM_COMMAND
            if (GMCommand.CommandSelf(rawText))
            {
                GUIDebugLog.AddMessage(rawText);
                Debug.Log(rawText);
                return;
            }
            if (GMCommand.IsGMCommand(rawText))
            {
                // GMコマンドならNGワード検出せずそのままサーバーに送る
                // @bf_a 系対策
                text = rawText;
            }
#endif
#if GOONE
		if (GMCommand.IsGMCommand(text))
		{
			CommonPacket.SendGmCommand(text.Substring(1));
			return;
		}
#endif
            if (this.ChatType == ChatType.Say)
                this.WhisperPlayerId = 0;

            //this.WhisperPlayerId
            //luwanzhong: add new idear show whisper in local
            if (this.ChatType == ChatType.Whisper)
            {
                var str = "[私聊] 对[url=" + (int)ChatType.Whisper + "," + this.WhisperPlayerId + "," + this.WhisperPlayerName + "]" + "[" + this.WhisperPlayerName + "]" + "悄悄地说: " + text;
                str = AddColorCode(str, ChatType.Whisper);
                this.View.PageWhisper.textList.Add(str);
            }

            CommonPacket.SendChat(this.ChatType, text, this.WhisperPlayerId);

            Debug.Log("send chat: " + this.ChatType.ToString() + " " + text + " " + this.WhisperPlayerId);
        }

        /// <summary>
        /// If continuity 2 same chat
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        bool IsContinuityChat(string text)
        {
            //Add to Chache
            this.ChatWordChache.Add(text.Trim());

            int sameCnt = 0;

            if (ConinuityForbidNum <= this.ChatWordChache.Count)
            {
                string checkText = this.ChatWordChache[this.ChatWordChache.Count - 1];
                int sIdx = this.ChatWordChache.Count - 2;
                int eIdx = this.ChatWordChache.Count - ConinuityForbidNum;

                for (int i = sIdx; i >= eIdx; i--)
                {
                    string txt = this.ChatWordChache[i];
                    if (txt == checkText)
                        sameCnt++;
                }
            }

            //ChacheClearFiber
            this.ChatWordChacheClearFiber = new Fiber(this.ChatStrChacheClearCoroutine());

            return (ConinuityForbidNum - 1 <= sameCnt);
        }

        IEnumerator ChatStrChacheClearCoroutine()
        {
            var waitFiber = new WaitSeconds(ChatWordChacheClearTime);

            while (waitFiber.IsWait)
                yield return null;

            this.ChatWordChache.Clear();
        }
        #endregion

        #region Type Event
        ChatType ChatType
        {
            get { return ScmParam.Common.ChatType; }
            set
            {
                ScmParam.Common.ChatType = value;
                switch (value)
                {
                    case ChatType.Say:
                        this.View.Base.chatType.text = "[综合]";
                        this.SwitchTo(PageType.Say);
                        break;
                    case ChatType.Whisper:
                        this.View.Base.chatType.text = "[私聊]";
                        this.SwitchTo(PageType.Whisper);
                        break;
                    case ChatType.Team:
                        this.SwitchTo(PageType.Team);
                        break;
                }
                this.View.Base.input.isSelected = true;
            }
        }
        long whisperPlayerId;
        long WhisperPlayerId { get { return this.whisperPlayerId; } set { this.whisperPlayerId = value; } }

        string whisperPlayerName = "";
        string WhisperPlayerName { get { return this.whisperPlayerName; } set { this.whisperPlayerName = value; } }

        //Type Event
        public void OnChatTypeGuild()
        {

        }
        public void OnChatTypeParty()
        {

        }

        public void OnChatTypeShout()
        {

        }

        public void OnChatTypeSay()
        {
            this.SetChatType(ChatType.Say);
            this.SetWhisperTarget("");
        }

        public void OnChatTypeTeam()
        {
            this.SetChatType(ChatType.Team);
        }

        public void OnChatTypeWis(long playerId, string name)
        {
            this.SetChatType(ChatType.Whisper);
            this.WhisperPlayerId = playerId;
            this.WhisperPlayerName = name;
            this.SetWhisperTarget(name);
        }

        void SetChatType(ChatType type)
        {
            this.ChatType = type;
        }

        void SetWhisperTarget(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                this.View.Base.chatTarget.text = "[" + name + "]:";
            }
            else
            {
                this.View.Base.chatTarget.text = "";
            }

            this.View.Base.input.label.width = this.InputWidth - (int)this.View.Base.chatTarget.localSize.x;
        }
        #endregion

        #region Recieve chat
        public void OnReceiveMessage(ChatInfo chatInfo)
        {
            string text = "";

            switch (chatInfo.chatType)
            {
                case ChatType.Whisper:
                    var name = chatInfo.name.Split(':');
                    var relName = name[0].Substring(9, name[0].Length - 10);
                    text = chatInfo.type + "[url=" + (int)ChatType.Whisper + "." + chatInfo.playerID + "." + relName + "." + "]" + " " + name[0] + "[/url] " + "" + "悄悄地对你说:" + " " + chatInfo.text;
                    this.View.PageWhisper.textList.Add(text);
                    break;
                case ChatType.Say:
                    Debug.Log("Add : " + chatInfo.text);
                    var name1 = chatInfo.name.Split(':');
                    var relName1 = name1[0].Substring(9, name1[0].Length - 10);
                    text = chatInfo.type + "[url=" + (int)ChatType.Whisper + "." + chatInfo.playerID + "." + relName1 + "." + "]" + name1[0] + "[/url] " + ":" + " " + chatInfo.text;
                    this.View.PageSay.textList.Add(text);
                    break;
                case ChatType.Team:
                    this.View.PageTeam.textList.Add(text);
                    break;
                case ChatType.Shout:
                    break;
                case ChatType.System:
                    text = chatInfo.name + chatInfo.text;
                    this.View.PageSay.textList.Add(text);
                    break;
                default:
                    text = chatInfo.name + chatInfo.text;
                    this.View.PageSay.textList.Add(text);
                    break;
            }

        }

        public static void AddMessage(ChatInfo chatInfo)
        {
            switch (chatInfo.chatType)
            {
                case ChatType.AdminYell:
                case ChatType.AdminGuild:
                case ChatType.AdminTeam:
                case ChatType.AdminWhisper:
                case ChatType.AdminShout:
                    AddAdminMessage(chatInfo);
                    break;
                default:
                    AddChatMessage(chatInfo);
                    break;
            }
        }

        static void AddAdminMessage(ChatInfo chatInfo)
        {
            // GMメッセージ表示
            GUIGMWindow.AddMessage(chatInfo);

            // 色変換
            SetColorCode(ref chatInfo, false);

            // メッセージ表示
            //GUIChatLog.AddMessage(chatInfo);
            if (Instance != null) Instance._AddMessage(chatInfo);
        }

        static void AddChatMessage(ChatInfo chatInfo)
        {
            // NGUIのBBコード削除
            chatInfo.text = NGUIText.StripSymbols(chatInfo.text);
            // 色変換
            SetColorCode(ref chatInfo, false);

            // メッセージ表示
            //GUIChatLog.AddMessage(chatInfo);
            if (Instance != null) Instance._AddMessage(chatInfo);
        }

        public static void AddSystemMessage(bool isError, string text)
        {
            var chatInfo = new ChatInfo { playerID = 0, chatType = ChatType.System, name = "", text = text };
            // 色変換
            SetColorCode(ref chatInfo, isError);

            // メッセージ表示
            //GUIChatLog.AddMessage(chatInfo);
            if (Instance != null) Instance._AddMessage(chatInfo);
        }

        static void SetColorCode(ref ChatInfo chatInfo, bool isError)
        {
            // 色変換
            if (!string.IsNullOrEmpty(chatInfo.text))
            {
                if (isError)
                    chatInfo.text = AddErrorColorCode(chatInfo.text);
                else
                    chatInfo.text = AddColorCode(chatInfo.text, chatInfo.chatType);
            }
            if (!string.IsNullOrEmpty(chatInfo.name))
            {
                if (isError)
                    chatInfo.name = AddErrorColorCode(chatInfo.name + ":");
                else
                    chatInfo.name = AddColorCode(chatInfo.name + ":", chatInfo.chatType);
            }

            if (!string.IsNullOrEmpty(chatInfo.type))
            {
                if (isError)
                    chatInfo.type = AddErrorColorCode(chatInfo.type);
                else
                    chatInfo.type = AddColorCode(chatInfo.type, chatInfo.chatType);
            }
        }

        public static string AddErrorColorCode(string text)
        {
            return string.Format("[{0}]{1}[-]", ErrorColorCode, text);
        }

        void _AddMessage(ChatInfo chatInfo)
        {
            Debug.Log("Recieve Chat : " + chatInfo.type + " " + chatInfo.text + " " + this.WhisperPlayerId);
            //ScmParam.Common.ChatPopupQueue.Enqueue(chatInfo);
            if(this.View.root.gameObject.activeSelf == false)
                this.chatPopQueue.Enqueue(chatInfo);
            this.OnReceiveMessage(chatInfo);
        }
        #endregion

        #endregion

    }
}

