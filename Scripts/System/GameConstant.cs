/// <summary>
/// ゲーム全体で使う文字列やら宣言をまとめたもの
/// 
/// 2012/00/00
/// </summary>
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

using Scm.Common.Packet;
using Scm.Common.PacketCode;
using Scm.Common.GameParameter;

#region マスターデータID
#region キャラクターマスター
public enum AvatarType : int
{
	None = 0,

	Begin = P001_Galatia,
	End = P010_Eria,

	P001_Galatia = 1,
	P002_Giruberuto,
	P003_Kuroriku,
	P004_Raiback,
	P005_Kufun,
	P006_Shirayuki,
	P007_Rerikus,
	P008_Airin,
	P009_Kazakiri,
	P010_Eria,
	P011_Meru,
	P012_Azurael,
	P013_Mineruva,
	P014_Sasha,
	P015_Horus,
	P016_Torfin,
	P017_Deltoa,
	P018_Youko,
	P019_Cylinder,
	P020_Yuuka,
	P021,
	P022,
	P023,
	P024,
	P025,
	P026,
	P027,
	P028,
	P029,
	P030,

	P501_Symphogear1 = 501,
	P502_Symphogear2,
	P503_Symphogear3,
	P504_Shingeki1,
	P505_Shingeki2,
	P506_Shingeki3,
	P507,
	P508,
	P509,
	P510,
	P511,
	P512,
	P513,
	P514,
	P515,
	P516,
	P517,
	P518,
	P519,
	P520,

	P1001_Galatia_ST = 1001,
	P1002_Galatia_SH,
	P1003_Galatia_HT,
	P1004_Giruberuto_SH,
	P1005_Giruberuto_HT,
	P1006_Giruberuto_ST,
	P1007_Raiback_GD,
	P1008_Raiback_EX,
	P1009_Raiback_SH,
	P1010,
	P1011,
	P1012,
	P1013,
	P1014,
	P1015,
	P1016,
	P1017,
	P1018,
	P1019,
	P1020,

	Random = None,
};
public class CharacterName
{
	public const string ModelPath = "Character/Model.asset";
}
public class NpcName
{
	public static string GetCharacterPath(string name)
	{
		return string.Format("Character/{0}.prefab", name);
	}
	public const string AnimationPath = "Animation/Animations.asset";
}
#endregion

#region バトルフィールドマスター
public enum BattleFieldType
{
	None = 0,

	BF001_TowerL,
	BF002_TeamDM,
	BF003_TowerS,
	BF004_Training,
	BF005_BugSlayer,
	BF006_Tutorial,
	BF007_Shiwasu1,
	BF008_Shiwasu2,
	BF009_Shiwasu3,
	BF010_Cooperation,

    BF011_TargetDestroy1,
    BF012_TargetDestroy2,
    BF013_AttackOnTitan,

    BF013_SaveAI,

	Max,
}
#endregion

#region カラーマスター
public enum ColorType
{
	None = 0,

	C001_Red,
	C002_Green,
	C003_Blue,
	C004_Yellow,
	C005_LightBlue,
	C006_Orange,
	C007_Purple,
	C008_MyteamIconColor,
	C009_EnemyIconColor,
	C010_UnkownIconColor,
	C011_MinimapPlayer,
	C012_MinimapJumpPoint,
	C013_MinimapRespawn,
	C014_ChatSay,
	C015_ChatGuild,
	C016_ChatTeam,
	C017_ChatWhisper,
	C018_ChatShout,
	C019_ChatAdminYell,
	C020_ChatAdminGuild,
	C021_ChatAdminTeam,
	C022_ChatAdminWhisper,
	C023_ChatAdminShout,
	C024_ChatSystem,
	C025_ChatError,

	C026_StatusNormalTop,
	C027_StatusNormalBottom,
	C028_StatusUpTop,
	C029_StatusUpBottom,
	C030_StatusDownTop,
	C031_StatusDownBottom,
	C032_StatusRandomTop,
	C033_StatusRandomBottom,
	C034_RankNormalTop,
	C035_RankNormalBottom,
	C036_CapacityNormalTop,
	C037_CapacityNormalBottom,
	C038_CapacityOverTop,
	C039_CapacityOverBottom,

	Max,
}
#endregion

#region テキストマスター
public enum TextCategory
{
    QuestDescription = 36
}

public enum TextType
{
	None = 0,

	// GUIEffectMessage
	TX005_Kill	= 5,	// GUIBreakInfoMessageItem.cs
	TX013_Dead	= 13,	// GUIBreakInfoMessageItem.cs
	TX028_Good	= 28,	// GUIBreakInfoMessageItem.cs

	// GUIChat
	//TX007_Entry		= 7,	// PersonManager.cs
	TX009_LevelUp	= 9,	// BattlePacket.cs
	//TX010_GetExp	= 10,	// BattlePacket.cs
	//TX011_GetMoney	= 11,	// BattlePacket.cs
	//TX012_KillBonus	= 12,	// BattlePacket.cs

	// GUIEffectMessage
	TX017_TeamSkill				= 17,	// GUITacticalMessageItem
	TX018_EnemyTeamSkill		= 18,	// GUITacticalMessageItem
	TX019_EnemyGuardianDestroy	= 19,	// GUITacticalMessageItem
	TX020_GuardianDestroy		= 20,	// GUITacticalMessageItem
	TX021_EnemySubTowerDestroy	= 21,	// GUITacticalMessageItem
	TX022_SubTowerDestroy		= 22,	// GUITacticalMessageItem
	TX023_MainTowerDamageHalf	= 23,	// GUITacticalMessageItem
	TX032_EnemyShieldGenDestroy	= 32,	// GUITacticalMessageItem
	TX033_ShieldGenDestroy		= 33,	// GUITacticalMessageItem
	TX034_EnemyHealPodDestroy	= 34,	// GUITacticalMessageItem
	TX035_HealPodDestroy		= 35,	// GUITacticalMessageItem
	TX036_MainTowerAttack		= 36,	// GUITacticalMessageItem
	TX039_PoiseTeamSkill		= 39,	// GUITacticalMessageItem
	TX040_PoiseEnemyTeamSkill	= 40,	// GUITacticalMessageItem

	// GUIEffectMessage
	TX024_TimeLimit	= 24,	// GUITimeMessageItem
	TX025_TimeLater	= 25,	// GUITimeMessageItem

	// GUIMatching
	TX026_Minutes = 26,

	// GUISystemMessage
	TX027_Disconnect			= 27,	// GameLoginController.cs、SceneController.cs、ReLoginRequestManager.cs、ResultMain.cs
	TX029_DisconnectTitle		= 29,	// CreatePlayerState.cs、GameLoginController.cs、LoginState.cs、ReEntryState.cs、SelectPlayerState.cs、SceneController.cs、ReLoginRequest.cs、ResultMain.cs
	TX031_DisconnectOK			= 31,	// GameLoginController.cs、SceneController.cs、ResultMain.cs
	TX041_Connecting			= 41,	// ConnectState.cs、CreatePlayerState.cs、ReEntryState.cs、SelectPlayerState.cs
	TX047_ReturnTitleInfo		= 47,	// プレイヤー作成時保有プレイヤー数オーバーか失敗時、ログイン時不明なエラー、再参戦時エラー、プレイヤー選択時エラー
	TX061_ServerFull			= 61,	// ログイン時
	TX305_Network_Communication = 305,	// 通信中

	// GUIMessageWindow
	TX042_ReEnter					= 42,	// 再参戦を行うかメッセージ
	TX043_InputPlayerName			= 43,	// プレイヤー名入力
	TX044_NGWrod					= 44,	// NGネーム入力
	TX045_DuplicateName				= 45,	// 名前重複
	TX046_CreateNameSuccess			= 46,	// 名前入力完了
	TX060_ReEnterField_BattleEnd	= 60,	// 再参戦後にバトルが終了していた

	// GUISystemMessage
	TX048_MainteTitle	= 48,	// ログイン時メンテ、サーバーステータス取得
	TX049_Mainte		= 49,	// メンテナンス内容

	// GUICharacterStorage
	TX051_CS_LobbyCharaTitle	= 51,
	TX052_CS_LobbyCharaHelp		= 52,
	TX053_CS_DeckEditTitle		= 53,
	TX054_CS_DeckEditHelp		= 54,

	TX057_Common_YesButton	= 57,
	TX058_Common_NoButton	= 58,

	// GUISystemMessage
	TX062_ReConnect						= 62,	// バトル中の回線切断
	TX063_ReConnectRetry_ButtonTitle	= 63,	// 再接続回数をオーバーした時
	TX064_ReConnectSuccess_Title		= 64,	// 再接続成功
	TX065_ReConnectSuccess				= 65,	// 再接続成功
	TX066_ReConnectBattleEnd_Title		= 66,	// 再接続後にバトルが終了していた
	TX067_ReConnectBattleEnd			= 67,	// 再接続後にバトルが終了していた

	// GUIMapWindow
	TX069_MW_TeamSkillBreakCount		= 69,
	TX072_MW_ItemBreakCount				= 72,
	TX075_MW_SelCharaFailMsg			= 75,
	TX076_MW_SelCharaFailMsgBriefing	= 76,
	TX077_MW_DestroyRespawnObjMsg		= 77,

	// GUITeamMenu
	TX078_TeamMenu_ScreenTitle					= 78,
	TX079_TeamMenu_HelpMessage					= 79,
	TX080_TeamCreate_NameEmpty					= 80,
	TX081_TeamCreate_NameOver					= 81,
	TX082_TeamCreate_PasswordError				= 82,
	TX083_TeamCreateRes_AlreadyEnter			= 83,
	TX084_TeamCreateRes_NameDuplicate			= 84,
	TX085_TeamCreateRes_NGName					= 85,
	TX086_TeamCreateRes_OverLengthName			= 86,
	TX087_TeamCreateRes_SenderInMatching		= 87,
	TX088_TeamCreateRes_Fail					= 88,
	TX089_TeamJoin_PasswordInputTitle			= 89,
	TX090_TeamJoinRes_MemberOver				= 90,
	TX091_TeamJoinRes_UnjustPassword			= 91,
	TX092_TeamJoinRes_TeamNotExist				= 92,
	TX093_TeamJoinRes_SenderInMatching			= 93,
	TX094_TeamJoinRes_InMatching				= 94,
	TX095_TeamJoinRes_InBattle					= 95,
	TX096_TeamJoinRes_Fail						= 96,
	TX097_TeamSecession_WindowTitle				= 97,
	TX098_TeamKick_WindowTitle					= 98,
	TX099_TeamRemoveMemberRes_NotFoundTarget	= 99,
	TX100_TeamRemoveMemberRes_NoAuthority		= 100,
	TX101_TeamRemoveMemberRes_Fail				= 101,
	TX102_TeamDissolveRes_NoAuthority			= 102,
	TX103_TeamDissolveRes_Fail					= 103,
	TX104_GUITeamInfoItem_InBattle				= 104,
	TX105_GUITeamInfoItem_InMatching			= 105,
	TX106_TeamComment0							= 106,
	TX107_TeamComment1							= 107,
	TX108_TeamComment2							= 108,
	TX109_TeamComment3							= 109,
	TX110_TeamComment4							= 110,
	TX111_TeamComment5							= 111,
	TX112_TeamComment6							= 112,
	TX113_TeamComment7							= 113,

	// LobbyPacket
	TX114_EnterLobbyRes_Fatal		= 114,	// EnterLobbyReq にて失敗
	TX115_EnterLobbyRes_LobbyFull	= 115,	// EnterLobbyReq にて失敗
	TX116_EnterLobbyRes_InMatching	= 116,	// EnterLobbyReq にて失敗

	// LobbyPacket
	TX117_MatchingRes_Fatal						= 117,	// MatchingReq にて失敗
	TX118_MatchingRes_EntryByTeamMemberFail		= 118,	// MatchingReq にて失敗
	TX119_MatchingRes_CancelByTeamMemberFail	= 119,	// MatchingReq にて失敗
	TX120_MatchingRes_CancelFail				= 120,	// MatchingReq にて失敗

	// GUIMatching
	TX121_Matching_ScreenTitle	= 121,
	TX122_Matching_HelpMessage	= 122,

	// GUIMatchingState
	TX123_MatchingState_Entry		= 123,
	TX124_MatchingState_Matching	= 124,
	TX125_MatchingState_Waiting		= 125,
	TX126_MatchingState_EnterField	= 126,

	// BattlePacket
	TX127_EnterFieldRes_Fail	= 127,	// EnterFieldReq にて失敗

	// GUIMessageWindow
	TX128_NextLogout	= 128,	// ロビーメニューのログアウト確認メッセージ
	TX129_NextTraining	= 129,	// ひとりで練習モードに行くときの確認メッセージ
	TX130_ExitTraining	= 130,	// ひとりで練習モードを終了するときの確認メッセージ

	// GUISystemMessage
	TX131_SetCharacterDeckRes_Success	= 131,	// デッキ編集で決定が成功した時
	TX132_SetCharacterDeckRes_Fail		= 132,	// デッキ編成で決定が失敗した時
	TX133_SetCharacterDeckRes_CostOver	= 133,	// デッキ編成で決定時にコストオーバーしてる時

	// GUIDeckEdit
	TX134_DeckEdit_ScreenTitle			= 134,	// デッキ編集画面
	TX135_DeckEdit_HelpMessage			= 135,	// デッキ編集画面
	TX136_DeckCharaSelect_HelpMessage	= 136,	// デッキ編集画面

	// GUIChat
	TX137_SetMainOwnCharacterRes_Fail	= 137,	// シンボルキャラクター設定失敗メッセージ
	TX138_Chat_ContinuityNoticeStr		= 138,	// 同じ発言を3回した時のメッセージ

	// GUIOption
	TX139_Option_ScreenTitle	= 139,
	TX140_Option_HelpMessage	= 140,
	TX501_Option_BGMDesc		= 501,
	TX502_Option_SEDesc			= 502,
	TX503_Option_VoiceDesc		= 503,
	TX504_Option_MacroDescFormat= 504,
	TX505_Option_MacroDefault00	= 505,
	TX506_Option_MacroDefault01	= 506,
	TX507_Option_MacroDefault02	= 507,
	TX508_Option_MacroDefault03	= 508,
	TX509_Option_MacroDefault04	= 509,
	TX510_Option_MacroDefault05	= 510,
	TX511_Option_MacroDefault06	= 511,
	TX512_Option_MacroDefault07	= 512,

	// GUILobbySelect
	TX141_LobbySelect_ScreenTitle = 141,
	TX142_LobbySelect_HelpMessage = 142,

	// GUIRanking
	TX143_Ranking_ScreenTitle	= 143,
	TX145_Ranking_NoData		= 145,

	// GUIAuth
	TX146_AppliVersion					= 146,	// GUISystemMessage
	TX147_Auth_Progress					= 147,	// GUIAuth
	TX148_Auth_Error					= 148,	// GUISystemMessage
	TX149_Web_AuthError					= 149,	// GUISystemMessage
	TX150_Download_Progress				= 150,	// GUIAuth
	TX151_Download_Error				= 151,	// GUISystemMessage
	TX152_RootPrivileges_Error			= 152,	// GUISystemMessage
	TX153_SystemLanguage_NotJapanese	= 153,	// GUISystemMessage
	TX555_XigncodeHandleMessage			= 555,	// GUISystemMessage

	// GUISystemMessage
	TX154_Infomation_ScreenTitle	= 154,	// GUISystemMessage 認証とかでのお知らせ時のタイトル
	TX155_Error_ScreenTitle			= 155,	// GUISystemMessage 認証とかでのエラー時のタイトル
	TX156_Error						= 156,	// GUISystemMessage 「エラー」
	TX157_Common_UpgradeButton		= 157,	// GUISystemMessage 「更新」
	TX158_Common_QuitButton			= 158,	// GUISystemMessage 「終了」
	TX159_Common_RetryButton		= 159,	// GUISystemMessage 「リトライ」
	TX160_Meintenance_QuitApp		= 160,	// GUISystemMessage メンテナンスメッセージ

	// GUITitle
	TX50_Title_News					= 50,

	// GUIRanking
	TX167_Ranking_FinalResult				= 167,
	TX168_Ranking_Totalization				= 168,
	TX169_Ranking_DailyHelp					= 169,
	TX170_Ranking_WeeklyHelp				= 170,
	TX171_Ranking_LastWeeklyHelp			= 171,
	TX172_Ranking_MonthHelp					= 172,
	TX173_Ranking_LastMonthHelp				= 173,
	TX174_Ranking_DayFormat					= 174,
	TX175_Ranking_DayFormatExceptYear		= 175,
	TX176_Ranking_DayOfWeekSun				= 176,
	TX177_Ranking_DayOfWeekMon				= 177,
	TX178_Ranking_DayOfWeekTue				= 178,
	TX179_Ranking_DayOfWeekWed				= 179,
	TX180_Ranking_DayOfWeekThu				= 180,
	TX181_Ranking_DayOfWeekFri				= 181,
	TX182_Ranking_DayOfWeekSat				= 182,
	TX183_Ranking_DayPeriodFormat			= 183,
	TX184_Ranking_WeekPeriodFormat			= 184,
	TX185_Ranking_WeekMonDayPeriodFormat	= 185,
	TX186_Ranking_LastWeekPeriodFormat		= 186,
	TX187_Ranking_MonthPeriodFormat			= 187,
	TX188_Ranking_MonthDayOnePeriodFormat	= 188,
	TX189_Ranking_LastMonthPeriodFormat		= 189,
	TX190_Ranking_Progress					= 190,

	// CharaList
	TX239_CharaList_Order					= 239,
	TX245_CharaList_Base					= 245,
	TX246_CharaList_Slot					= 246,
	TX247_CharaList_Deck					= 247,
	TX248_CharaList_Symbol					= 248,
	TX249_CharaList_PowerupLevelMax			= 249,
	TX250_CharaList_Lock					= 250,
	TX312_CharaList_PowerupLevelShortage	= 312,
	TX313_CharaList_RankShortage			= 313,
	TX314_CharaList_Select					= 314,

	// ソート
	TX251_Common_Rank					= 251,
	TX252_Common_Level					= 252,
	TX253_CharaInfo_HitPoint			= 253,
	TX254_CharaInfo_Attack				= 254,
	TX255_CharaInfo_Defense				= 255,
	TX256_CharaInfo_Extra				= 256,
	TX259_Common_Cost					= 259,
	TX260_Sort_Type						= 260,
	TX261_Sort_Obtaining				= 261,
	TX351_Sort_Name						= 351,

	// GUIPowerup
	TX272_Powerup_ScreenTitle				= 272,
	TX273_Powerup_Base_HelpMessage			= 273,
	TX274_Powerup_Bait_HelpMessage			= 274,
	//GUIChat
	TX306_CharaLockRes_Fail					= 306,	// ロック失敗メッセージ
	TX307_PowerupRes_Fail					= 307,	// 合成失敗メッセージ
	//GUIMessageWindow
	TX308_Powerup_Fusion_LockMessage		= 308,	// 合成素材にロックされているキャラクターが含まれていて合成できない時のメッセージ
	TX309_Powerup_Fusion_Message			= 309,	// 合成をするかどうかの確認メッセージ
	TX310_Powerup_Fusion_SlotMessage		= 310,	// 合成素材のキャラでスロットにキャラが入っている時に外す旨を伝える確認メッセージ
	TX311_Powerup_Fusion_HighRankMessage	= 311,	// 合成素材に高ランクのキャラが選択されている時の確認メッセージ

	// GUIEvolution
	TX315_MaterialList_SynchroMaxMessage	= 315,
	TX325_MaterialList_SynchroUpMessage		= 325,
	TX334_MaterialList_PowerupLevelShortage = 334,
	// GUISystemMessage
	TX316_EvolutionRes_Fail					= 316,	// LobbyPacket
	// GUIMessageWindow
	TX317_Evolution_Fsuion_Message			= 317,
	TX318_Evolution_Fusion_EmptyMessage		= 318,
	TX319_Evolution_Fusion_SynchroUpMessage	= 319,
	TX320_Evolution_Fusion_HighRankMessage	= 320,
	// GUIEvolution
	TX326_Evolution_Fusion_ScreenTitle			= 326,
	TX327_Evolution_Fusion_Base_HelpMessage		= 327,
	TX328_Evolution_Fusion_Bait_HelpMessage		= 328,
	TX368_Evolution_Fusion_Enter_HelpMessage	= 368,
	TX369_Evolution_Fusion_NotHave_HelpMessage	= 369,

	// GUIChat
	TX329_SynchroFusionRes_Fail				= 329,	// LobbyPacket
	// GUISynchro
	TX330_Synchro_Fusion_ScreenTitle				= 330,
	TX331_Synchro_Fusion_Base_HelpMessage			= 331,
	TX332_Synchro_Fusion_Bait_HelpMessage			= 332,
	TX408_Synchro_Fusion_NotHave_HelpMessage		= 408,
	TX409_Synchro_Fusion_Enter_HelpMessage			= 409,
	TX410_Synchro_Fusion_SynchroRemain_HelpMessage	= 410,
	TX442_Synchro_Fusion_HighRankMessage			= 442,

	// GUIMessageWindow
	TX333_Synchro_Fusion_Message			= 333,
	TX335_SynchroResult_BonusMax_Message	= 335,

	// GUIPowerupMenu
	TX341_PowerupMenu_ScreenTitle			= 341,
	TX342_PowerupMenu_HelpMessage			= 342,

	// GUIItemBox
	TX343_ItemBox_Select_Title				= 343,
	TX345_ItemBox_Sell_Help					= 345,
	TX346_ItemBox_MultiSell_Help			= 346,
	TX353_ItemBox_MultiSell					= 353,
	TX354_ItemBox_ItemUse					= 354,
	TX359_ItemBox_Sell						= 359,
	TX360_ItemBox_ItemUse_Help				= 360,
	TX416_ItemBox_CheckSell					= 416,

	// GUIMessageWindow
	TX347_SellItem_HaveMoneyOver			= 347,
	TX348_SellItem_SellCheck				= 348,
	// GUIItemSellSimple
	TX349_SellItem_Title					= 349,
	TX350_SellItem_HelpMessage				= 350,

	// GUIPowerupSlot
	TX363_Slot_ScreenTitle			= 363,
	TX364_Slot_Base_HelpMessage		= 364,
	TX365_Slot_Slot_HelpMessage		= 365,
	TX366_Slot_OK_Message			= 366,
	TX367_Slot_Slot_Message			= 367,

	// GUIMail
	TX370_Mail_ScreenTitle						= 370,
	TX438_Mail_HelpMessage						= 438,
	// GUIMessageWindow
	TX378_Mail_AllRead_ConfirmMessage			= 378,
	TX379_Mail_AllRead_Message					= 379,
	TX439_Mail_AllRead_Zero_Message				= 439,
	TX380_Mail_AllMailDelete_ConfirmMessage		= 380,
	TX381_Mail_AllMailDelete_Message			= 381,
	TX440_Mail_AllMailDelete_Zero_Message		= 440,
	TX382_Mail_AllReceive_ConfirmMessage		= 382,
	TX383_Mail_AllReceive_Message				= 383,
	TX384_Mail_AllReceive_DeadlineMessage		= 384,
	TX441_Mail_AllReceive_Zero_Message			= 441,
	TX385_Mail_AllItemMailDelete_ConfirmMessage = 385,
	TX386_Mail_AllItemMailDelete_Message		= 386,
	// GUIMailItem, GUIMailDetail
	TX387_Mail_ReceivedTime_Format				= 387,
	TX388_Mail_ItemExpiration					= 388,
	TX389_Mail_ItemNoneDeadline					= 389,
	TX390_Mail_ItemReceive_Days_Format			= 390,
	TX391_Mail_ItemReceive_Hours_Format			= 391,
	TX392_Mail_ItemReceive_Minutes_Format		= 392,
	TX393_Mail_ItemReceive_Seconds_Format		= 393,
	TX401_Mail_ItemReceived						= 401,
	// GUIMailDetail
	TX396_MailDetail_MailDelete_Message			= 396,
	TX397_MailDetail_OverLockCount_Message		= 397,
	TX398_MailDetail_OverKeepDays_Message		= 398,
	TX399_MailDetail_ItemReceived_Message		= 399,
	TX400_MailDetail_OverReceiveDeadline_Message= 400,

	// GUISymbolChara
	TX402_SymbolChara_Title					= 402,
	TX403_SymbolChara_HelpMessage			= 403,
	TX404_SymbolChara_CharaChangeMessage	= 404,

	// GUICharacterInfo
	TX405_CharacterInfo_Title				= 405,
	TX406_CharacterInfo_HelpMessage			= 406,
	TX407_CharacterInfo_LockErrorMessage	= 407,

	// GUICharacterBox
	TX411_CharaBox_Info						= 411,
	TX412_CharaBox_ScreenTitle				= 412,
	TX413_CharaBox_Select_HelpMessage		= 413,
	TX414_CharaBox_MultiSell_HelpMessage	= 414,
	TX417_CharaBox_CheckSell				= 417,
	TX418_CharaBox_RemoveSlot				= 418,
	TX419_CharaBox_NotRemoveSlot			= 419,

	// GUIFusionMessage
	TX437_FusionMessage_MaxLevel			= 437,

	// GUIAchievement
	TX443_Achievement_Title					= 443,
	TX444_Achievement_Event_1_Help			= 444,
	TX445_Achievement_DailyHelp				= 445,
	TX446_Achievement_AchievementHelp		= 446,
	TX447_Achievement_AllRewardHelp			= 447,
	TX448_Achievement_EventTab				= 448,
	TX449_Achievement_DailyTab				= 449,
	TX450_Achievement_AchievementTab		= 450,
	TX451_Achievement_AllRewardTab			= 451,
	TX452_Achievement_DailyTag				= 452,
	TX453_Achievement_WeeklyTag				= 453,
	TX454_Achievement_EventTag				= 454,
	TX455_Achievement_DailyEventTag			= 455,
	TX456_Achievement_AchievementTag		= 456,
	TX457_Achievement_Progress				= 457,
	TX459_Achievement_RewardDeadline		= 459,
	TX460_Achievement_Compleat				= 460,
	TX462_Achievement_GetOneMessage			= 462,
	TX463_Achievement_GetAllCheckMessage	= 463,
	TX464_Achievement_GetAllMessage			= 464,
	TX465_Achievement_GetButton				= 465,
	TX466_Achievement_AcquiredButton		= 466,
	TX467_Achievement_GetAllButton			= 467,
	TX468_Achievement_RewardTag				= 468,
	TX469_Achievement_RewardGetFailure		= 469,
	TX479_Achievement_EmergencyHelp			= 479,
	TX480_Achievement_PriorityHelp			= 480,
	TX481_Achievement_Event_2_Help			= 481,
	TX482_Achievement_Event_3_Help			= 482,
	TX483_Achievement_Event_4_Help			= 483,
	TX484_Achievement_ReserveHelp			= 484,

	// GUIShopMenu
	TX485_ShopMenu_Title						= 485,
	TX486_ShopMenu_HelpMessage					= 486,
	TX487_ShopMenu_TicketNum					= 487,
	TX488_ShopMenu_CharacterInfo				= 488,
	TX489_ShopMenu_TicketExchange				= 489,
	TX490_ShopMenu_TicketExchangeAlready		= 490,
	TX491_ShopMenu_TicketExchangeConfirmation	= 491,
	TX492_ShopMenu_TicketExchangeCompletion		= 492,
	TX493_ShopMenu_TicketExchangeFailure		= 493,

	// GUICharaTicket
	TX494_CharaTicket_Title						= 494,
	TX495_CharaTicket_HelpMessage				= 495,
	TX529_CharaTicket_NetworkErrorMessage		= 529,
	TX536_CharaTicket_ShopMaintenanceMessage	= 536,

	// GUITicketShop
	TX513_TicketShop_Totle						= 513,
	TX514_TicketShop_HelpMessage				= 514,

	// GUICharaShop
	TX521_CharaShop_Title = 521,
	TX522_CharaShop_HelpMessage					= 522,
	TX530_CharaShop_TicketExchangeMessage		= 530,
	TX531_CharaShop_Error_NotBeRetrieved		= 531,
	TX532_CharaShop_Error_AcquisitionLimit		= 532,
	TX533_CharaShop_Error_PointShortage			= 533,
	TX534_CharaShop_AcquisitionCompletion		= 534,

	// GUIAgeVerification
	TX540_AgeVerification_Title					= 540,
	TX541_AgeVerification_HelpMessage			= 541,

    // MessageWindow
    TX568_LoginMessage                          = 568,

    // Displayed level
    TX601_Lv_Display                            = 601,

    /// <summary>
    /// Warning for network fee before resource download
    /// </summary>
    TX606_NetworkFeeWarning                     = 606,
    TX606_NetworkFeeWarningTitle                = 607,

    /// <summary>
    /// Not enough energy
    /// </summary>
    TX608_NotEnoughEnergy                       = 608,

    /// <summary>
    /// Hint for recruitment text
    /// </summary>
    TX609_RecruitmentTextHint                   = 609,
    /// <summary>
    /// Default recruitment text
    /// </summary>
    TX610_RecruitmentDefaultText                = 610,
    /// <summary>
    /// Confirm to join {0}' s team
    /// </summary>
    TX611_RecruitmentConfirmJoinTeam            = 611,
    /// <summary>
    /// Confirmation
    /// </summary>
    TX612_RecruitmentConfirmTitle               = 612,
    /// <summary>
    /// Confirmation of cancel
    /// </summary>
    TX613_RecruitmentCancelConfirm              = 613,
    /// <summary>
    /// Received team invite from:{0}
    /// </summary>
    TX617_TeamInvite                            = 617,
    /// <summary>
    /// Delete friend {0}, continue?
    /// </summary>
    TX618_DeleteFriendWarning                   = 618,
    /// <summary>
    /// Confirm to delete friend
    /// </summary>
    TX619_DeleteFriendWarningTitle              = 619,
}
#endregion
#endregion

#region Lobby
public enum LobbyType
{
	Begin = 1,
	None = 0,

	Lobby001,
	Lobby002,
	Lobby003,
	Lobby004,
	Lobby005,
	Lobby006,
	Lobby007,
	Lobby008,
	Lobby009,
	Lobby010,
	Lobby011,
	Lobby012,
	Lobby013,
	Lobby014,
	Lobby015,

	Max,
};
public static class LobbyName
{
	public static string GetName(LobbyType type)
	{
		return type.ToString();
	}
}
#endregion

#region キャラボード
public static class CharacterBoard
{
	public const string path = "GUI/Prefabs/";
	public static string GetFilePath(AvatarType type)
	{
		string fileName = string.Format("CharaBoard{0:000}", (int)type);
		return (path + fileName);
	}
}
#endregion

#region キャラプレート
public static class CharacterPlate
{
	public const string path = "GUI/Prefabs/";
	public static string GetFilePath(AvatarType type)
	{
		string fileName = string.Format("CharaPlate{0:000}", (int)type);
		return (path + fileName);
	}
}
#endregion

#region Animation
public enum MotionLayer : int
{
	Move = -1,			// wait, run.
	Action = 0,			// デフォルト. 主にスキルモーションなど.
	ReAction = 1,		// ダメージモーション(hit,guard)など.
}
/// <summary>
/// ユニークモーション. (本来汎用モーションを再生する状況で,特定条件の場合に別モーションを再生する).
/// </summary>
public static class UniqueMotion
{
	private static Dictionary<int, string> deadSelfMotionDic;
	static UniqueMotion()
	{
		deadSelfMotionDic = new Dictionary<int, string>();
		string dead_self_sp = "dead_self_sp";	// 超必自爆死.
		deadSelfMotionDic.Add(790, dead_self_sp);
		deadSelfMotionDic.Add(791, dead_self_sp);
		deadSelfMotionDic.Add(792, dead_self_sp);
		deadSelfMotionDic.Add(793, dead_self_sp);
		deadSelfMotionDic.Add(794, dead_self_sp);
	}

	public static bool TryGetDeadSelfMotionName(int bulletID, out string motionName)
	{
		if (deadSelfMotionDic.TryGetValue(bulletID, out motionName))
		{
			return true;
		}
		return false;
	}
}
public static class MotionName
{
	public static string GetName(MotionState motionState)
	{
		return motionState.ToString();
	}
	/* データベース管理に移行したため、MotionStateに存在するとは限らなくなった.
	public static bool GetState(string clipName, ref MotionState state)
	{
		foreach(MotionState s in System.Enum.GetValues(typeof(MotionState)))
		{
			if (s.ToString() != clipName)
				continue;
			state = s;
			return true;
		}
		Debug.LogWarning(string.Format("{0}:モーションが存在しない", clipName));
		return false;
	}
	*/
}
#endregion

#region Map
public static class MapName
{
	public const string AssetPath = "Map.prefab";

	public static string GetTitleAssetbundleName(int id)
	{
		return string.Format("tfm{0:000}", id);
	}
	public static string GetAssetbundleName(AreaType areaType, int mapID)
	{
		switch (areaType)
		{
		case AreaType.Lobby:
			return string.Format("lfm{0:000}", mapID);
		case AreaType.Field:
			return string.Format("bfm{0:000}", mapID);
		default:
			Debug.LogWarning("AreaType:" + areaType + " unknown");
			return string.Format("bfm{0:000}", mapID);
		}
	}
}
#endregion

#region 共通ボーン
public enum BoneType
{
	Hips,
	LeftUpLeg,
	LeftLeg,
	LeftFoot,
	RightUpLeg,
	RightLeg,
	RightFoot,
	Spine,
	Spine1,
	Head,
	LeftShoulder,
	LeftArm,
	LeftForeArm,
	LeftHand,
	RightShoulder,
	RightArm,
	RightForeArm,
	RightHand,
	Root,
	Hit_Root,

	Max,
}
public static class BoneName
{
	public static string GetName(BoneType type)
	{
		return type.ToString();
	}
}
#endregion

#region Uniqueボーン
public enum UniqueBoneType
{
	weapon,
	Cloth_F,
	Cloth_F_L,
	Cloth_F_R,
	Cloth_B,
	Cloth_B_L,
	Cloth_B_R,
	Cloth_L,
	Cloth_R,
	Hair_F,
	Hair_F_L,
	Hair_F_R,
	Hair_B,
	Hair_B_L,
	Hair_B_R,
	Hair_L,
	Hair_R,

	Max,
}
public static class UniqueBoneName
{
	public static string GetName(UniqueBoneType type)
	{
		return type.ToString();
	}
	public static string GetName(UniqueBoneType uniqueBoneType, int index)
	{
		string name = GetName(uniqueBoneType);
		if (string.IsNullOrEmpty(name))
			return name;
		string nameIndex = string.Format("{0}_{1}", name, index.ToString("D2"));
		return nameIndex;
	}
}
#endregion

#region スキルボタンタイプ
public enum SkillButtonType
{
	Normal,
	Skill1,
	Skill2,
	SpecialSkill,
	TechnicalSkill,

	Max,
	Begin = Normal,
}
public static class SkillButtonName
{
	public static string GetName(SkillButtonType type)
	{
		return type.ToString();
	}
}
#endregion

#region タグ＆レイヤー
public static class TagName
{
}
public static class LayerNumber
{
	public const int Default = 0;	// プリセット
	public const int TransparentFX = 1;	// プリセット
	public const int IgnoreRaycast = 2;	// プリセット
	public const int Water = 4;	// プリセット

	public const int Player = 8;	// プレイヤー
	public const int vsPlayer_Bullet = 10;	// プレイヤー弾丸両方に対してのコリジョン
	public const int Bullet = 11;	// 弾丸系

	public const int MapWallCol = 13;	// 透ける壁設定(GameObjectで事前に設定する)
	public const int MapWallAlpha = 14;	// 実際に透けさせている壁(プログラム上で切り替えている)
	public const int vsPlayer = 15;	// プレイヤーに対してのコリジョン
	public const int vsBullet = 17;	// 弾丸に関してのコリジョン

	public const int UIBG = 20;	// 3Dよりも奥になるUI
	public const int UI3D = 21;	// UI の間に挟む 3D オブジェクト
	public const int UI2D = 22;	// 3Dよりも手前になるUI
	public const int UIFG = 23;	// 何よりも優先して手前に来るUI

	public const int UI3DFX = 25;	// 3Dモデル演出用カメラ
}
#endregion

#region 宣言
[System.Serializable]
public class TeamPrefab
{
	[SerializeField]
	private GameObject blue;
	public GameObject Blue { get { return blue; } }
	[SerializeField]
	private GameObject red;
	public GameObject Red { get { return red; } }
	[SerializeField]
	private GameObject green;
	public GameObject Green { get { return green; } }

	public GameObject GetPrefab(TeamTypeClient team)
	{
		switch (team)
		{
		case TeamTypeClient.Friend:
			return Blue;
		case TeamTypeClient.Enemy:
			return Red;
		default:
			return Green;
		}
	}
}

[System.Serializable]
public class ChatMacroInfo
{
	[SerializeField]
	string _buttonName;
	public string ButtonName { get { return _buttonName; } private set { _buttonName = value; } }

	[SerializeField]
	string _macro;
	public string Macro { get { return _macro; } private set { _macro = value; } }

	[SerializeField]
	int _index;
	public int Index { get { return _index; } private set { _index = value; } }

	public ChatMacroInfo(string buttonName, string macro, int index)
	{
		this.Setup(buttonName, macro);
		this.Index = index;
	}
	public void Setup(string buttonName, string macro)
	{
		this.ButtonName = buttonName;
		this.Macro = macro;
	}

	public ChatMacroInfo Clone() { return (ChatMacroInfo)MemberwiseClone(); }

	public override bool Equals(object obj)
	{
		// objがnullか、型が違うときは、等価でない
		if (obj == null || this.GetType() != obj.GetType())
			return false;
		var t = (ChatMacroInfo)obj;

		if (this.ButtonName != t.ButtonName) return false;
		if (this.Macro != t.Macro) return false;
		if (!this.Index.Equals(t.Index)) return false;

		return true;
	}

	public override int GetHashCode()
	{
		return
			this.ButtonName.GetHashCode()
			^ this.Macro.GetHashCode()
			^ this.Index.GetHashCode()
			;
	}
}

#region Download用設定パラメータ
public class DownloadParam
{
	public System.Uri Uri { get; set; }
	public string Path { get; set; }

	public DownloadParam(string url, string directory, string fileName)
	{
		this.Uri = new System.Uri(System.IO.Path.Combine(url, fileName));
		this.Path = GameGlobal.CreateDirectory(directory);
		this.Path = System.IO.Path.Combine(this.Path, fileName);
	}
}
#endregion
#endregion

#region TeamType JudgeType
/// <summary>
/// プレイヤーから見たチーム(敵か味方かのみ).
/// </summary>
public enum TeamTypeClient : int
{
	/// <summary>
	/// 不明
	/// </summary>
	Unknown = TeamType.Unknown,

	/// <summary>
	/// 敵
	/// </summary>
	Enemy = TeamType.Red,

	/// <summary>
	/// 味方
	/// </summary>
	Friend = TeamType.Blue,
}
/// <summary>
/// プレイヤーから見たジャッジ.
/// </summary>
public enum JudgeTypeClient : int
{
	/// <summary>
	/// 不明
	/// </summary>
	Unknown = JudgeType.Unknown,

	/// <summary>
	/// プレイヤー敗北
	/// </summary>
	PlayerLose = JudgeType.WinnerRed,

	/// <summary>
	/// プレイヤー勝利
	/// </summary>
	PlayerWin = JudgeType.WinnerBlue,

	/// <summary>
	/// 引き分け
	/// </summary>
	Draw = JudgeType.Draw,

	/// <summary>
	/// プレイヤー完全敗北
	/// </summary>
	PlayerCompleteLose = JudgeType.CompleteWinnerRed,

	/// <summary>
	/// プレイヤー完全勝利.
	/// </summary>
	PlayerCompleteWin = JudgeType.CompleteWinnerBlue,
}
#endregion

#region キャッシュデータ
/// <summary>
/// 一時データとその保存時刻を格納するクラス.
/// </summary>
public class CacheData<T>
{
	public float Time { get; private set; }
	public T Data { get; private set; }
	
	public CacheData(float time, T data)
	{
		this.Time = time;
		this.Data = data;
	}
}
#endregion

#region 定数
public static class GameConstant
{
	#region 汎用
	/// <summary>
	/// 無効な値
	/// </summary>
	public const byte InvalidID = 0;
	/// <summary>
	/// 最小移動値
	/// </summary>
	public const float MoveMinThreshold = 0.001f;
	/// <summary>
	/// 移動パケット間隔
	/// </summary>
	public const float MovePacketInterval = 0.1f;
	/// <summary>
	/// プレイヤーの最小ID
	/// </summary>
	public const int MinPlayerId = 1;
	/// <summary>
	/// プレイヤーの最大ID
	/// </summary>
	public const int MaxPlayerId = 200;
	/// <summary>
	/// ダウンモーション(受け身可能)のモーション速度
	/// </summary>
	public const float DownMotionSpeed_Normal = 0.2f;
	/// <summary>
	/// ダウンモーション(受け身不可)のモーション速度
	/// </summary>
	public const float DownMotionSpeed_Spin = 1.5f;
	/// <summary>
	/// ダウン時間
	/// </summary>
	public const float DownTimer = 2f;
	/// <summary>
	/// 起き上がりからの無敵時間
	/// </summary>
	public const float WakeInvincibleTimer = 1f;
	/// <summary>
	/// 空中受身の無敵時間
	/// </summary>
	public const float AirTechInvincibleTimer = 0.5f;
	/// <summary>
	/// メテオスキルID
	/// </summary>
	public const int MeteorSkillID = 190011;
	/// <summary>
	/// メテオ弾丸セットID
	/// </summary>
	public const int MeteorBulletSetID = 212;
	/// <summary>
	/// 直近弾を撃つときのターゲット探索範囲.
	/// </summary>
	public const float BulletNearestSearchRange = 30f;
	public const float BulletNearestSearchAngle = 360f;
	/// <summary>
	/// 直接タップによるロックオン可能範囲.
	/// </summary>
	public const float TapLockonRange = 500f;
	public const float TapLockonAngle = 180f;
	/// <summary>
	/// リスポーン時間 2015/01/27 10秒固定.
	/// </summary>
	public const float RespawnTime = 100f;
	/// <summary>
	/// リスポーン要求後に移動パケットの送信を止める時間.
	/// </summary>
	public const float BlockSendMoveTime_Respawn = 4f;
	#endregion

	#region チーム
	public static readonly Color RedColor = Color.red;
	public static readonly Color BlueColor = Color.cyan;
	private const string RedColorCode = "FF0000";
	private const string BlueColorCode = "00FFFF";
	private const string UnknownColorCode = "B000FF";
	private const string DefaultColorCode = "FFFFFF";
	private static string TeamColorCode(TeamType teamType)
	{
		switch (teamType.GetClientTeam())
		{
		case TeamTypeClient.Enemy:
			return RedColorCode;
		case TeamTypeClient.Friend:
			return BlueColorCode;
		case TeamTypeClient.Unknown:
			return UnknownColorCode;
		}
		return DefaultColorCode;
	}
	public static string StringWithTeamColor(string str, TeamType teamType)
	{
		return string.Format("[{0}]{1}[-]", GameConstant.TeamColorCode(teamType), str);
	}
	#endregion

	#region チーム
	/// <summary>
	/// チームメンバーの数
	/// </summary>
	public const int TeamMemberMax = 5;
	#endregion

	#region エフェクト
	/// <summary>
	/// オブジェクトパス
	/// </summary>
	public static class ObjectPath
	{
		public const string BundlePath = AssetReference.CommonAssetName;
		public const string AssetPath = "Object/";
	}

	/// <summary>
	/// エフェクトパス
	/// </summary>
	public static class EffectPath
	{
		public const string DefaultBundlePath = AssetReference.CommonAssetName;
		public const string AssetPath = "Effect/";

		static public void ConvertAssetBundlePath(ref string bundlePath, ref string assetPath)
		{
			// HACK: リソース整理までの仮対応.
			{
				string[] paths = assetPath.Split('@');
				if(paths.Length == 2)
				{
					bundlePath = paths[0];
					assetPath = paths[1];
				}
			}

			if(string.IsNullOrEmpty(bundlePath))
			{
				bundlePath = DefaultBundlePath;
			}
			assetPath = AssetPath + assetPath;
		}
	}

	/// <summary>
	/// 共通エフェクトパス
	/// </summary>
	public const string EffectLevelUp = "Common/lvup";
	public const string EffectDown = "Common/down";
	public const string EffectJumpBase = "Common/jump_base";
	public const string EffectJumpDirect = "Common/jump_direction";
	public const string EffectBreakFall = "Common/BreakFall";

	public const string EffectBindDark = "Common/impact_area_001";	// 仮モデル.

	/// <summary>
	/// 自滅系エフェクトのパス
	/// </summary>
	public const string SelfDestroyPath = EffectPath.AssetPath + "Common/SelfDestroyEffect";
	/// <summary>
	/// 時間消滅系エフェクトのパス
	/// </summary>
	public const string TimerDestroyPath = EffectPath.AssetPath + "Common/TimerDestroyEffect";
	/// <summary>
	/// 自分弾丸用当たり判定エフェクトのパス
	/// </summary>
	public const string SelfAmmoPath = EffectPath.AssetPath + "Common/SelfAmmo";
	/// <summary>
	/// 接続オブジェクトパス
	/// </summary>
	public const string ConnectObjectStartName = "Connect_";
	#endregion
}
#endregion

#region UI用モードタイプ
public enum UIModeType : int
{
	None = 0,
	Init,
	Auth,
	Title,
	Lobby,
	Battle,
	Result,
}
#endregion