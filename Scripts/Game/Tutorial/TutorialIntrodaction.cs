/// <summary>
/// チュートリアル 導入部
/// 
/// 2014/06/23
/// </summary>

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Scm.Common.Master;
using Scm.Common.NGWord;

/// <summary>
/// チュートリアル 導入部クラス
/// </summary>
public class TutorialIntrodaction : TutorialScript
{
    #region Classes
    
    // アタッチオブジェクトクラス
    [System.Serializable]
    public class AttachObjects
    {
        public GameObject CharaBoardParent;
    }

	private class CharaBoardResource
	{
		public bool IsLoaded { get; set; }
		public GameObject Prefab { get; set; }

		public CharaBoardResource()
		{
			Reset();
		}

		public void Reset()
		{
			IsLoaded = false;
			Prefab = null;
		}
	}
    
    #endregion
    
    #region Fields & Properties

    // チュートリアルで使用するアバターのリスト
    [SerializeField]
    private List<AvatarType> avatars;

    [SerializeField]
    private List<int> skins;

    // アタッチオブジェクト
    [SerializeField]
    private AttachObjects attach;
    
    // キャラマスターデータコレクション
	private Dictionary<AvatarType, CharaMasterData> charaMasterData = new Dictionary<AvatarType, CharaMasterData>();
    
	// キャラボードリソース管理クラス
	private CharaBoard charaBoard = new CharaBoard();

	// キャラボードリソースコレクション
	private Dictionary<AvatarType, CharaBoardResource> charaBoardResources;
    
    // キャラボードオブジェクト
    private GameObject charaBoardObj;
    
    // 選択されたアバター
    private AvatarType selectedAvatar;
    
    #endregion
    
    #region MonoBehaviour
	/// <summary>
    /// 更新
    /// </summary>
    protected override void Update()
    {
        base.Update();
    }
    
    /// <summary>
    /// デストラクタ
    /// </summary>
    protected override void OnDestroy()
    {
        base.OnDestroy();
    }
    
    #endregion
    
    #region TutorialScript
    
    /// <summary>
    /// リソースのロード
    /// </summary>
    protected override IEnumerator loadResources()
    {
		yield return StartCoroutine(base.loadResources());

		charaBoardResources = new Dictionary<AvatarType, CharaBoardResource>();
		foreach (var avatar in avatars)
		{
			charaBoardResources.Add(avatar, new CharaBoardResource());
		}

		charaBoardObj = null;

        for (int i = 0; i < avatars.Count; ++i)
		{
            var avatar = avatars[i];
            var skinId = skins[i];
			charaBoardResources[avatar].Reset();
			charaBoard.GetBoard(avatar, skinId, false, go => {
				charaBoardResources[avatar].IsLoaded = true;
				charaBoardResources[avatar].Prefab = go;
			});

			while (!charaBoardResources[avatar].IsLoaded)
				yield return null;
		}

		yield break;
    }
    
    /// <summary>
    /// リソースのアンロード
    /// </summary>
    protected override void unloadResources()
    {
		if(this.charaBoardResources != null)
		{
			foreach (var kvp in this.charaBoardResources)
			{
				kvp.Value.Reset();
			}
		}
		base.unloadResources();
    }
    
    protected override IEnumerator setup()
    {
        yield return StartCoroutine(base.setup());
    }
    
    protected override void startTutorial()
    {
		getCharaMasterData();
		GUILoading.SetActive(false);

		base.startTutorial();
    }
    
    /// <summary>
    /// チュートリアルの終了
    /// </summary>
    protected override void stopTutorial()
    {
        // キャラボード破棄
        if (charaBoardObj != null)
        {
            Object.Destroy(charaBoardObj);
            charaBoardObj = null;
        }
        
        base.stopTutorial();
    }
    
    /// <summary>
    /// スクリプト
    /// </summary>
    protected override IEnumerator script()
    {
		setupMessageWindow();

        yield return new WaitSeconds(1.0f);
        
        openNextWindow();
		setGuide(GUIMessageWindow.GuideMode.Guide3D_UIBG);
        setMessage("やあ、初めまして！\r\nクロスワールドへようこそ！\r\nまずはあなたの名前を教えてほしいな");
        
        yield return new WaitNext();
        
        while (true)
        {
            openInputWindow();
			setGuide(GUIMessageWindow.GuideMode.Guide3D_UIBG);
            setMessage("名前を入力してください");
            
            yield return new WaitInputName();
            
            openNextWindow();
            
            if (checkPlayerName(ScmParam.Net.UserName)) break;
            
            setMessage("ゴメンね、その名前はクロスワールドでは名乗れないんだ\r\n他の名前に変えてもらえるかな？");
            
            yield return new WaitNext();
        }
        
        setMessage(ScmParam.Net.UserName + "さんって言うんだ\r\nよろしくね！");
        
        yield return new WaitNext();
        
        setMessage( "僕の名前はフルモ\r\nクロスワールドの案内役なんだ");
        
        yield return new WaitNext();
        
        setMessage(ScmParam.Net.UserName + "さんのために\r\n三体のクロスアクターを用意したよ");
        
        yield return new WaitNext();
        
        createCharaBoard(avatars[0]);
		setGuide(GUIMessageWindow.GuideMode.Guide2D);
        setMessage("一体目は" + charaMasterData[avatars[0]].Name + "\r\n槍の達人で、近接攻撃に優れたクロスアクターだよ");
        
        yield return new WaitNext();
        
        setMessage(charaMasterData[avatars[0]].Name + "は、フットワークを生かして華麗に戦えるんだ！");
        
        yield return new WaitNext();
        
        createCharaBoard(avatars[1]);
        setMessage("二体目は" + charaMasterData[avatars[1]].Name + "\r\n銃での攻撃に特化した、遠距離攻撃用のクロスアクターなんだ");
        
        yield return new WaitNext();
        
        setMessage(charaMasterData[avatars[1]].Name + "のレーザーに貫けないものはないよ！");
        
        yield return new WaitNext();
        
        createCharaBoard(avatars[2]);
        setMessage("三体目は" + charaMasterData[avatars[2]].Name + "\r\n見ての通り、パワータイプのクロスアクターだよ");
        
        yield return new WaitNext();
        
        setMessage(charaMasterData[avatars[2]].Name + "はどんな敵でも投げちゃう力持ちなんだ！");
        
        yield return new WaitNext();
        
        deleteCharaBoard();
        
        openNextWindow();
		setGuide(GUIMessageWindow.GuideMode.Guide3D_UIBG);
        setMessage("さあ、君の使いたいクロスアクターを選んでね");
        
        yield return new WaitNext();
        
        while (true)
        {
            closeWindow();
            setGuide(GUIMessageWindow.GuideMode.None);
            
            openCharaStorage();
            
            yield return new WaitSeconds(3.0f);
            
            closeCharaStorage();
            
            // TODO:キャラクターストレージで選択
            selectedAvatar = avatars[0];
            
            openYesNoWindow();
			setGuide(GUIMessageWindow.GuideMode.Guide3D_UIBG);
            setMessage(charaMasterData[selectedAvatar].Name + "でいいんだね？");
            
            WaitYesNo waitYesNo = new WaitYesNo();
            yield return waitYesNo;
            
            if (waitYesNo.Result == WaitYesNo.Buttons.Yes) break;
            
            openNextWindow();
            setMessage("他のクロスアクターにする？\r\nどれにするのかな？");
            
            yield return new WaitNext();
        }

		//ScmParam.Common.AvatarType = selectedAvatar;

		openNextWindow();

		setMessage("おめでとう！\r\n" + charaMasterData[selectedAvatar].Name + "はキミのものだよ！");
		
		yield return new WaitNext();
		
		setMessage("じゃあ、" + charaMasterData[selectedAvatar].Name + "に\r\nキミをローディングするね");
        
        yield return new WaitNext();
        
        closeWindow();
        setGuide(GUIMessageWindow.GuideMode.None);
		resetMessageWindow();

		//yield return new WaitTween(GUIMessageWindow.BGPlayTween);

        GUILoading.SetActive(true);
        
        yield return new WaitFrames(1);

		LobbyPacket.SendEnterLobby();

        yield break;
    }
    
    #endregion
    
    #region Private Methods
    
    /// <summary>
    /// キャラマスターデータの取得
    /// </summary>
    private void getCharaMasterData()
    {
        foreach (var avatar in avatars)
        {
            CharaMasterData data;
            if (MasterData.TryGetChara((int)avatar, out data))
            {
                charaMasterData[avatar] = data;
            }
        }
    }        
    
    /// <summary>
    /// キャラボードの生成
    /// </summary>
    /// <param name="type">Type.</param>
    private void createCharaBoard(AvatarType avatar)
    {
        deleteCharaBoard();
        
        if (avatar == AvatarType.None)
            return;
        
		if (charaBoardResources[avatar].Prefab == null)
			return;
        
        charaBoardObj = Object.Instantiate(charaBoardResources[avatar].Prefab, Vector3.zero, Quaternion.identity) as GameObject;
        
        if (charaBoardObj != null)
        {
            charaBoardObj.transform.parent = attach.CharaBoardParent.transform;
            charaBoardObj.transform.localPosition = new Vector3(312.0f, 0.0f, 16.0f);
            charaBoardObj.transform.localRotation = Quaternion.identity;
            charaBoardObj.transform.localScale = Vector3.one;
            
            // TODO:とりあえずスクリプトでレイヤー変更
			Transform[] transforms = charaBoardObj.GetComponentsInChildren<Transform>();
			foreach (var t in transforms)
			{
				t.gameObject.layer = LayerMask.NameToLayer("UIBG");
			}
        }
    }
    
    private void deleteCharaBoard()
    {
        if (charaBoardObj != null)
        {
            GameObject.Destroy(charaBoardObj);
            charaBoardObj = null;
        }
    }
    
    /// <summary>
    /// プレイヤー名のチェック
    /// </summary>
    /// <returns><c>true</c>, if player name was checked, <c>false</c> otherwise.</returns>
    /// <param name="name">Name.</param>
    private bool checkPlayerName(string name)
    {
        if (string.IsNullOrEmpty(name) || NGWord.IsMatchNGName(name))
            return false;
        else
            return true;
    }
    
    #endregion
}

/// <summary>
/// ファイバー用 名前入力が終わるまで待つ
/// </summary>
public class WaitInputName : IFiberWait
{
    private bool flag;
    private string result;
    
    public WaitInputName()
    {
        flag = false;
        result = null;
		//GUIMessageWindow.SetDelegateInput(this.OnOk, null, this.OnChange);
		//GUIMessageWindow.InputValue = ScmParam.Net.UserName;
    }
    
    public void OnOk()
    {
        if (FilterWordController.Instance.IsNeedFilter(result)) {
            GUIMessageWindow.SetModeOK(MasterData.GetText(TextType.TX044_NGWrod),
                () => {
                
                }
            );
            flag = false;
            return;
        }
        ScmParam.Net.UserName = result;
        Debug.Log("SetUserName...");
        flag = true;
    }
    
    public void OnChange()
    {
        result = UIInput.current.value;
        
        if (string.IsNullOrEmpty(result))
        {
            // OKボタンDisable
        }
        else
        {
            // OKボタンEnable
        }
    }
    
    #region IFiberWait
    
    public bool IsWait
    {
        get
        {
            return !flag;
        }
    }
    
    #endregion
}
