/// <summary>
/// チュートリアル 締め
/// 
/// 2014/06/17
/// </summary>
using UnityEngine;
using System.Collections;

public class TutorialConclusion : TutorialScript
{
    #region MonoBehaviour
    protected override void Update()
    {
        base.Update();
    }
    
    protected override void OnDestroy()
    {
        base.OnDestroy();
    }
    
    #endregion
    
    #region TutorialScript
    
    protected override IEnumerator loadResources()
    {
        yield return StartCoroutine(base.loadResources());
    }
    
    protected override void unloadResources()
    {
        base.unloadResources();
    }
    
    protected override IEnumerator setup()
    {
        yield return StartCoroutine(base.setup());
    }
    
    protected override void startTutorial()
    {
        base.startTutorial();
    }
    
    protected override void stopTutorial()
    {
        base.stopTutorial();
    }
    
    protected override IEnumerator script()
    {
		// 暫定処置
		while (!MapManager.Instance.MapExists) yield return null;
		yield return new WaitFrames(1);

		setupMessageWindow();
		lobbyMainMenuEnabled(false);
		lobbyBattleMenuEnabled(false);
		moveStickEnabled(false);

        yield return new WaitSeconds(2.0f);
        
        openNextWindow();
        setGuidePlate();
        
        setMessage("クロスワールドの事が\r\nこれで大体判ってもらえたかな？");
        
        yield return new WaitNext();
        
        setMessage("まだまだいっぱい\r\n伝えたい事はあるんだけど……\r\n今はここまで。");
        
        yield return new WaitNext();

        setMessage("また説明の為に現れるから\r\nその時はよろしくね！");
        
        yield return new WaitNext();
        
        setMessage("キミの活躍を期待しているよ！\r\nそれじゃ、またね！");
        
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
    
    #region Privete Methods
    #endregion
}
