/// <summary>
/// チュートリアル ロビー
/// 
/// 2014/06/17
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Scm.Common.Master;
using Scm.Common.GameParameter;

public class TutorialLobby : TutorialScript
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

    /// <summary>
    /// スクリプト
    /// </summary>
    protected override IEnumerator script()
    {
		int avatarType = NetworkController.ServerValue.PlayerInfo.Id;
		CharaMasterData charaMasterData;
		if(!MasterData.TryGetChara(avatarType, out charaMasterData))
		{
			Debug.LogError("AvatarType" + avatarType + "is not found");
			yield break;
		}

        // 暫定処置
        while (!MapManager.Instance.MapExists) yield return null;
        yield return new WaitFrames(1);

		setupMessageWindow();
        lobbyMainMenuEnabled(false);
        lobbyBattleMenuEnabled(false);
        moveStickEnabled(false);
        deleteTutorialNotice();

        yield return new WaitSeconds(2.0f);

        openNextWindow();
		setGuide(GUIMessageWindow.GuideMode.Guide3D_UIBG);
        
		setMessage("やあ！\r\n改めまして、こんにちは！\r\nここがクロスワールドだよ！");

        yield return new WaitNext();

        setMessage("今いるココはロビーって呼ばれている\r\n非戦闘空間だよ");

        yield return new WaitNext();

        setMessage("ここでの戦闘は\r\nMCに禁じられているから\r\n戦う事は出来ないんだ");
  
        yield return new WaitNext();

        setMessage(charaMasterData.Name + "の身体はどうかな？\r\nコンフリクトを\r\n起こしてなければいいけど……");

        yield return new WaitNext();

		setGuide(GUIMessageWindow.GuideMode.Guide2D);
        setMoveStickNotice();

        setMessage("……ちょっと歩いてみようか？\r\n左下にあるスティックで移動出来るよ");

        yield return new WaitNext();
  
        closeWindow();
        deleteTutorialNotice();
        moveStickEnabled(true);
  
        yield return new WaitPlayerMove(5.0f);
        
        moveStickEnabled(false);
        openNextWindow();
		setGuide(GUIMessageWindow.GuideMode.Guide3D_UIBG);
        setMessage("OK！\r\n大丈夫みたいだね、良かったぁ");

        yield return new WaitNext();

//        setMessage("スティックを前に入力すれば\r\nクロスアクターが前に");
//
//        yield return waitNext;
//
//        setMessage("後ろに倒せば後ろに進むよ");
//
//        yield return waitNext;
        
        setMessage("画面をフリックすると\r\n辺りを見渡せるよ\r\nやってみてね");

        yield return new WaitNext();

        closeWindow();
        setGuide(GUIMessageWindow.GuideMode.None);
  
        yield return new WaitCameraDrag(5.0f);
        
        openNextWindow();
		setGuide(GUIMessageWindow.GuideMode.Guide3D_UIBG);
        setMessage("視界へのアクセスも\r\n上手くいったみたいだね");
        
        yield return new WaitNext();

        setMessage("クロスアクターへのコネクトは\r\n問題ないみたいだね");
        
        yield return new WaitNext();

        setMessage("じゃあ、今度は\r\n戦闘の練習をしてみようか");
        
        yield return new WaitNext();

        setMessage("キミの視界の右上にある\r\n参戦ボタンを押してね");
        setLobbyBattleButtonNotice();
        
        yield return new WaitNext();

        closeWindow();
        setGuide(GUIMessageWindow.GuideMode.None);
        deleteTutorialNotice();
//        lobbyMainMenuEnabled(true);
		resetMessageWindow();

		//yield return new WaitTween(GUIMessageWindow.BGPlayTween);

		lobbyBattleMenuEnabled(true);

		yield return new WaitButtonClick(GUILobbyMenu.BattleButton);

		gmCommand("@bf 6");

        yield break;
    }
    
    #endregion

	#region Protected Methods
	#endregion

	#region Private Methods
    #endregion
}
