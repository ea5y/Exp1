/// <summary>
/// マスターデータ
/// 
/// 2013/00/00
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Scm.Common;
using Scm.Common.Master;
using Scm.Common.XwMaster;
using CharaMaster = Scm.Common.Master.CharaMaster;
using CharaMasterData = Scm.Common.Master.CharaMasterData;
using ItemMaster = Scm.Common.Master.ItemMaster;
using LoginBonusMaster = Scm.Common.Master.LoginBonusMaster;
using LoginBonusMasterData = Scm.Common.Master.LoginBonusMasterData;

public class MasterData : MonoBehaviour
{
	#region フィールド＆プロパティ
	public static MasterData Instance { get; private set; }

	public const string Path = "Work/Master";

    public const string EulaPath = "eula.csv";

	bool IsReadTextMaster { get; set; }	// テキストマスター読み込みフラグ
	#endregion

	#region 初期化
	void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
        MasterData.Read();
	}
	#endregion

	#region ファイル読み込み
	public static void Read()
	{
        Debug.Log(">>>>>>>>>>>>>Reading MasterData");
		// 読み込み順番は重要！！

		// バージョンマスター
		// バージョンマスター
		ReadFile(Path, VersionMaster.Instance.Filename, (string path) => { return VersionMaster.Instance.ReadMasterData(path); });

		// NGワードマスター
		// NGワードマスター
		ReadFile(Path, Scm.Common.XwMaster.OkWordMaster.Instance.Filename, (string path) => { return Scm.Common.XwMaster.OkWordMaster.Instance.ReadMasterData(path); });
		// OKワードマスター
		ReadFile(Path, Scm.Common.XwMaster.NgWordMaster.Instance.Filename, (string path) => { return Scm.Common.XwMaster.NgWordMaster.Instance.ReadMasterData(path); });

		// アイコンマスタ
		// アイコンマスターデータ
		ReadFile(Path, Scm.Common.XwMaster.IconMaster.Instance.Filename, (string path) => { return Scm.Common.XwMaster.IconMaster.Instance.ReadMasterData(path); });

		// スキルマスタ
		// スキル表示名マスターデータ
		ReadFile(Path, SkillDisplayNameMaster.Instance.Filename, (string path) => { return SkillDisplayNameMaster.Instance.ReadMasterData(path); });
		// スキル吹っ飛びパターンマスターデータ
		ReadFile(Path, SkillBlowPatternMaster.Instance.Filename, (string path) => { return SkillBlowPatternMaster.Instance.ReadMasterData(path); });
		// スキル弾丸補正マスターデータ
		ReadFile(Path, SkillBulletOffsetMaster.Instance.Filename, (string path) => { return SkillBulletOffsetMaster.Instance.ReadMasterData(path); });
		// スキルモーション移動マスターデータ
		ReadFile(Path, SkillMotionMoveSetMaster.Instance.Filename, (string path) => { return SkillMotionMoveSetMaster.Instance.ReadMasterData(path); });
		// スキルモーションマスターデータ
		ReadFile(Path, SkillMotionMaster.Instance.Filename, (string path) => { return SkillMotionMaster.Instance.ReadMasterData(path); });
		// スキルエフェクトマスターデータ
		ReadFile(Path, SkillEffectMaster.Instance.Filename, (string path) => { return SkillEffectMaster.Instance.ReadMasterData(path); });
		// スキル弾丸マスターデータ
		ReadFile(Path, SkillBulletMasterClient.Instance.Filename, (string path) => { return SkillBulletMasterClient.Instance.ReadMasterData(path); });
		// スキル／弾丸セットマスターデータ
		ReadFile(Path, SkillBulletSetMaster.Instance.Filename, (string path) => { return SkillBulletSetMaster.Instance.ReadMasterData(path); });
		// スキル親弾丸／子弾丸セットマスターデータ
		ReadFile(Path, SkillBulletBulletSetMaster.Instance.Filename, (string path) => { return SkillBulletBulletSetMaster.Instance.ReadMasterData(path); });
		// スキルモーション／エフェクトセットマスターデータ
		ReadFile(Path, SkillMotionEffectSetMaster.Instance.Filename, (string path) => { return SkillMotionEffectSetMaster.Instance.ReadMasterData(path); });
		// スキルマーカーデータ
		ReadFile(Path, SkillMarkerMaster.Instance.Filename, (string path) => { return SkillMarkerMaster.Instance.ReadMasterData(path); });
		// スキルエイミングマーカーデータ
		ReadFile(Path, SkillAimingMarkerMaster.Instance.Filename, (string path) => { return SkillAimingMarkerMaster.Instance.ReadMasterData(path); });
		// スキルキャストマーカーデータ
		ReadFile(Path, SkillCastMarkerMaster.Instance.Filename, (string path) => { return SkillCastMarkerMaster.Instance.ReadMasterData(path); });
		// スキルスーパーアーマーデータ
		ReadFile(Path, SkillSuperArmorSetMaster.Instance.Filename, (string path) => { return SkillSuperArmorSetMaster.Instance.ReadMasterData(path); });
		// スキル立体機動マスターデータ
		ReadFile(Path, Skill3dManeuverGearMaster.Instance.Filename, (string path) => { return Skill3dManeuverGearMaster.Instance.ReadMasterData(path); });
		// キャラボイスデータ
		ReadFile(Path, CharaVoiceMaster.Instance.Filename, (string path) => { return CharaVoiceMaster.Instance.ReadMasterData(path); });
		// スキルデータ
		ReadFile(Path, SkillMasterClient.Instance.Filename, (string path) => { return SkillMasterClient.Instance.ReadMasterData(path); });
		// スキル投技データ
		ReadFile(Path, SkillGrappleMaster.Instance.Filename, (string path) => { return SkillGrappleMaster.Instance.ReadMasterData(path); });
		// スキル投技 受け側詳細データ
		ReadFile(Path, SkillGrappleDefenseDetailMaster.Instance.Filename, (string path) => { return SkillGrappleDefenseDetailMaster.Instance.ReadMasterData(path); });
		// スキル投技／子弾丸セットデータ
		ReadFile(Path, SkillGrappleBulletSetMaster.Instance.Filename, (string path) => { return SkillGrappleBulletSetMaster.Instance.ReadMasterData(path); });
		// スキル弾丸カーブデータ
		ReadFile(Path, SkillBulletCurveMaster.Instance.Filename, (string path) => { return SkillBulletCurveMaster.Instance.ReadMasterData(path); });
		// スキル／重力セットマスターデータ
		ReadFile(Path, SkillGravitySetMaster.Instance.Filename, (string path) => { return SkillGravitySetMaster.Instance.ReadMasterData(path); });
		// スキル／移動セットマスターデータ
		ReadFile(Path, SkillMoveSetMaster.Instance.Filename, (string path) => { return SkillMoveSetMaster.Instance.ReadMasterData(path); });

		// 旧キャラマスタ
		// キャラボタンデータ
		ReadFile(Path, CharaButtonMaster.Instance.Filename, (string path) => { return CharaButtonMaster.Instance.ReadMasterData(path); });
		// キャラボタンセットデータ
		ReadFile(Path, CharaButtonSetMaster.Instance.Filename, (string path) => { return CharaButtonSetMaster.Instance.ReadMasterData(path); });
		// キャラレベルデータ
		ReadFile(Path, CharaLevelMaster.Instance.Filename, (string path) => { return CharaLevelMaster.Instance.ReadMasterData(path); });
		// キャラ説明文
		ReadFile(Path, Scm.Common.XwMaster.CharaDescriptionMaster.Instance.Filename, (string path) => { return Scm.Common.XwMaster.CharaDescriptionMaster.Instance.ReadMasterData(path); });
		// キャラ名
		ReadFile(Path, Scm.Common.XwMaster.CharaNameMaster.Instance.Filename, (string path) => { return Scm.Common.XwMaster.CharaNameMaster.Instance.ReadMasterData(path); });
		// キャラデータ
		ReadFile(Path, CharaMaster.Instance.Filename, (string path) => { return CharaMaster.Instance.ReadMasterData(path); });
		// キャラボイスデータはスキルデータの上.

		// 新キャラマスタ
		// 強化スロット比率
		ReadFile(Path, Scm.Common.XwMaster.CharaSlotPowerupRateMaster.Instance.Filename, (string path) => { return Scm.Common.XwMaster.CharaSlotPowerupRateMaster.Instance.ReadMasterData(path); });
		// キャラランク
		ReadFile(Path, Scm.Common.XwMaster.CharaRankMaster.Instance.Filename, (string path) => { return Scm.Common.XwMaster.CharaRankMaster.Instance.ReadMasterData(path); });
		// キャラ
		ReadFile(Path, Scm.Common.XwMaster.CharaMaster.Instance.Filename, (string path) => { return Scm.Common.XwMaster.CharaMaster.Instance.ReadMasterData(path); });

		// アニメーションパックマスタ
		// アニメーションデータ
		ReadFile(Path, AnimationMaster.Instance.Filename, (string path) => { return AnimationMaster.Instance.ReadMasterData(path); });
		// アニメーションパックデータ
		ReadFile(Path, AnimationPackMaster.Instance.Filename, (string path) => { return AnimationPackMaster.Instance.ReadMasterData(path); });
		// 歩きアニメーションリスト
		ReadFile(Path, AnimationRunListMaster.Instance.Filename, (string path) => { return AnimationRunListMaster.Instance.ReadMasterData(path); });
		// アニメーション／演出データ
		ReadFile(Path, AnimationActSetMaster.Instance.Filename, (string path) => { return AnimationActSetMaster.Instance.ReadMasterData(path); });

		// オブジェクトマスタ
		// オブジェクトスキルマスターデータ
		ReadFile(Path, ObjectSkillMaster.Instance.Filename, (string path) => { return ObjectSkillMaster.Instance.ReadMasterData(path); });
		// オブジェクトパラメータマスターデータ
		ReadFile(Path, ObjectParameterMaster.Instance.Filename, (string path) => { return ObjectParameterMaster.Instance.ReadMasterData(path); });
		// オブジェクト転送位置マスターデータ
		ReadFile(Path, ObjectTransportPositionMaster.Instance.Filename, (string path) => { return ObjectTransportPositionMaster.Instance.ReadMasterData(path); });
		// オブジェクト名マスターデータ
		ReadFile(Path, Scm.Common.XwMaster.ObjectNameMaster.Instance.Filename, (string path) => { return Scm.Common.XwMaster.ObjectNameMaster.Instance.ReadMasterData(path); });
		// オブジェクトマスターデータ
		ReadFile(Path, ObjectMaster.Instance.Filename, (string path) => { return ObjectMaster.Instance.ReadMasterData(path); });
		// オブジェクト／ドロップアイテムマスターデータ
		ReadFile(Path, ObjectDropItemSetMaster.Instance.Filename, (string path) => { return ObjectDropItemSetMaster.Instance.ReadMasterData(path); });
		// オブジェクト／ショップアイテムマスターデータ
		ReadFile(Path, ObjectShopItemSetMaster.Instance.Filename, (string path) => { return ObjectShopItemSetMaster.Instance.ReadMasterData(path); });
		// オブジェクト／子オブジェクト
		ReadFile(Path, ObjectObjectSetMaster.Instance.Filename, (string path) => { return ObjectObjectSetMaster.Instance.ReadMasterData(path); });
		// オブジェクト／弾丸セット
		ReadFile(Path, ObjectSkillBulletSetMaster.Instance.Filename, (string path) => { return ObjectSkillBulletSetMaster.Instance.ReadMasterData(path); });

		// カラーマスタ
		// カラー
		ReadFile(Path, ColorMaster.Instance.Filename, (string path) => { return ColorMaster.Instance.ReadMasterData(path); });

		// 状態効果マスタ
		// 状態効果
		ReadFile(Path, StateEffectMaster.Instance.Filename, (string path) => { return StateEffectMaster.Instance.ReadMasterData(path); });
		// 状態効果レベル
		ReadFile(Path, StateEffectLevelMaster.Instance.Filename, (string path) => { return StateEffectLevelMaster.Instance.ReadMasterData(path); });

		// 旧アイテムマスタ
		ReadFile(Path, ItemCubeMaster.Instance.Filename, (string path) => { return ItemCubeMaster.Instance.ReadMasterData(path); });
		ReadFile(Path, ItemInfoMaster.Instance.Filename, (string path) => { return ItemInfoMaster.Instance.ReadMasterData(path); });
		// アイテムデータ
		ReadFile(Path, ItemMaster.Instance.Filename, (string path) => { return ItemMaster.Instance.ReadMasterData(path); });

		// 新アイテムマスタ
		// アイテムデータ
		ReadFile(Path, Scm.Common.XwMaster.ItemMaster.Instance.Filename, (string path) => { return Scm.Common.XwMaster.ItemMaster.Instance.ReadMasterData(path); });

		// プレイヤーグレードマスタ
		// グレード称号
		ReadFile(Path, PlayerGradeTitleMaster.Instance.Filename, (string path) => { return PlayerGradeTitleMaster.Instance.ReadMasterData(path); });
		// プレイヤーグレード
		ReadFile(Path, PlayerGradeMaster.Instance.Filename, (string path) => { return PlayerGradeMaster.Instance.ReadMasterData(path); });

		// テキストマスタ
		// テキストデータ
		ReadFile(Path, TextMaster.Instance.Filename, (string path) => { Instance.IsReadTextMaster = TextMaster.Instance.ReadMasterData(path); return Instance.IsReadTextMaster; });

		// バトルフィールドマスタ
		// バトルルールデータ
		ReadFile(Path, BattleRuleMaster.Instance.Filename, (string path) => { return BattleRuleMaster.Instance.ReadMasterData(path); });
		// バトルフィールドデータ
		ReadFile(Path, BattleFieldMaster.Instance.Filename, (string path) => { return BattleFieldMaster.Instance.ReadMasterData(path); });
		// バトルイベント時間データ
		ReadFile(Path, BattleFieldTimeEventMaster.Instance.Filename, (string path) => { return BattleFieldTimeEventMaster.Instance.ReadMasterData(path); });

		// バトルレベルマスタ
		ReadFile(Path, Scm.Common.XwMaster.BattleLevelMaster.Instance.Filename, (string path) => { return Scm.Common.XwMaster.BattleLevelMaster.Instance.ReadMasterData(path); });

		//	二つ名マスタ.
		//	二つ名 単語マスタ.
		ReadFile(Path, AliasWordMaster.Instance.Filename, (string path) => { return AliasWordMaster.Instance.ReadMasterData(path); });
		//	二つ名 助詞マスタ.
		ReadFile(Path, AliasParticleMaster.Instance.Filename, (string path) => { return AliasParticleMaster.Instance.ReadMasterData(path); });

		//	所属勢力マスタ.
		//	所属勢力マスタ.
		ReadFile(Path, AffiliationForceMaster.Instance.Filename, (string path) => { return AffiliationForceMaster.Instance.ReadMasterData(path); });

		//	コンボ賞賛マスタ.
		//	コンボ賞賛マスタ.
		ReadFile(Path, ComboPraiseMaster.Instance.Filename, (string path) => { return ComboPraiseMaster.Instance.ReadMasterData(path); });

		// ObjectUIマスタ
		// ObjectUIマスタ
		ReadFile(Path, ObjectUserInterfaceMaster.Instance.Filename, (string path) => { return ObjectUserInterfaceMaster.Instance.ReadMasterData(path); });

		// 強化合成
		// 強化合成レベルマスタ
		ReadFile(Path, Scm.Common.XwMaster.CharaPowerupLevelMaster.Instance.Filename, (string path) => { return Scm.Common.XwMaster.CharaPowerupLevelMaster.Instance.ReadMasterData(path); });

		// 進化合成
		// 進化合成素材マスタ
		ReadFile(Path, Scm.Common.XwMaster.CharaEvolutionMaterialMaster.Instance.Filename, (string path) => { return Scm.Common.XwMaster.CharaEvolutionMaterialMaster.Instance.ReadMasterData(path); });

		// シンクロ合成
		// シンクロ制限マスタ
		ReadFile(Path, Scm.Common.XwMaster.CharaSynchroRestrictionMaster.Instance.Filename, (string path) => { return Scm.Common.XwMaster.CharaSynchroRestrictionMaster.Instance.ReadMasterData(path); });

		// クライアント/サーバ共通定義設定
		// コモンマスタ
		ReadFile(Path, Scm.Common.XwMaster.CommonSettingMaster.Instance.Filename, (string path) => { return Scm.Common.XwMaster.CommonSettingMaster.Instance.ReadMasterData(path); });

		// アチーブメント
		// アチーブメントタブマスター
		ReadFile(Path, Scm.Common.XwMaster.AchievementTabMaster.Instance.Filename, (string path) => { return Scm.Common.XwMaster.AchievementTabMaster.Instance.ReadMasterData( path ); });
		// アチーブメントカテゴリマスタ
		ReadFile(Path, Scm.Common.XwMaster.AchievementCategoryMaster.Instance.Filename, (string path) => { return Scm.Common.XwMaster.AchievementCategoryMaster.Instance.ReadMasterData(path); });
		// アチーブメントアイテムセットマスタ
		ReadFile(Path, Scm.Common.XwMaster.AchievementItemSetMaster.Instance.Filename, (string path) => { return Scm.Common.XwMaster.AchievementItemSetMaster.Instance.ReadMasterData(path); });
		// アチーブメント期間マスタ
		ReadFile(Path, Scm.Common.XwMaster.AchievementPeriodMaster.Instance.Filename, (string path) => { return Scm.Common.XwMaster.AchievementPeriodMaster.Instance.ReadMasterData(path); });
		// アチーブメントマスタ
		ReadFile(Path, Scm.Common.XwMaster.AchievementMaster.Instance.Filename, (string path) => { return Scm.Common.XwMaster.AchievementMaster.Instance.ReadMasterData(path); });

		// ショップマスター
		ReadFile(Path, Scm.Common.XwMaster.ShopItemMaster.Instance.Filename, (string path) => { return Scm.Common.XwMaster.ShopItemMaster.Instance.ReadMasterData( path ); });

		// チケット枚数マスター
		ReadFile(Path, Scm.Common.XwMaster.ShopTicketMaster.Instance.Filename, (string path) => { return Scm.Common.XwMaster.ShopTicketMaster.Instance.ReadMasterData( path ); });

        //BattleField

        ReadFile(Path, BattleFieldWaypointMaster.Instance.Filename, (string path) => { return BattleFieldWaypointMaster.Instance.ReadMasterData(path); });
        ReadFile(Path, CharaProfileMaster.Instance.Filename, (string path) => { return CharaProfileMaster.Instance.ReadMasterData(path); });
        ReadFile(Path, RewardMaster.Instance.Filename, (string path) => { return RewardMaster.Instance.ReadMasterData(path); });
        ReadFile(Path, ReplayVoiceMaster.Instance.Filename, (string path) => { return ReplayVoiceMaster.Instance.ReadMasterData(path); });
        ReadFile(Path, StoryEffectMaster.Instance.Filename, (string path) => { return StoryEffectMaster.Instance.ReadMasterData(path); });
        ReadFile(Path, StoryItemMaster.Instance.Filename, (string path) => { return StoryItemMaster.Instance.ReadMasterData(path); });
        ReadFile(Path, StoryMaster.Instance.Filename, (string path) => { return StoryMaster.Instance.ReadMasterData(path); });

        ReadFile(Path, PlayerLevelMaster.Instance.Filename, (string path) => { return PlayerLevelMaster.Instance.ReadMasterData(path); });
        ReadFile(Path, PlayerRankMaster.Instance.Filename, (string path) => { return PlayerRankMaster.Instance.ReadMasterData(path); });

        ReadFile(Path, AvatarMaster.Instance.Filename, (string path) => { return AvatarMaster.Instance.ReadMasterData(path); });
        ReadFile(Path, ChinaNgWordMaster.Instance.Filename, (string path) => { return ChinaNgWordMaster.Instance.ReadMasterData(path); });

        ReadFile(Path, CharaLevelMaster.Instance.Filename, (string path) => { return CharaLevelMaster.Instance.ReadMasterData(path); });
        ReadFile(Path, CharaStarMaster.Instance.Filename, (string path) => { return CharaStarMaster.Instance.ReadMasterData(path); });

        ReadFile(Path, LoginBonusMaster.Instance.Filename, (string path) => { return LoginBonusMaster.Instance.ReadMasterData(path); });
        ReadFile(Path, DailyQuestMaster.Instance.Filename, (string path) => { return DailyQuestMaster.Instance.ReadMasterData(path); });
        ReadFile(Path, GuideMaster.Instance.Filename, (string path) => { return GuideMaster.Instance.ReadMasterData(path); });
        ReadFile(Path, CharaButtonDescMaster.Instance.Filename, (string path) => { return CharaButtonDescMaster.Instance.ReadMasterData(path); });
        ReadFile(Path, ChargeDiscountMaster.Instance.Filename, (string path) => { return ChargeDiscountMaster.Instance.ReadMasterData(path); });

        ReadFile(Path, EulaPath, (string path) => { return Eula.ReadFile(path); });
        ReadFile(Path, AppMarketMaster.Instance.Filename, (string path) => { return AppMarketMaster.Instance.ReadMasterData(path); });
        ReadFile(Path, ServerMappingMaster.Instance.Filename, (string path) => { return ServerMappingMaster.Instance.ReadMasterData(path); });
        ReadFile(Path, SaleEventMaster.Instance.Filename, (string path) => { return SaleEventMaster.Instance.ReadMasterData(path); });

        if (FilterWordController.Instance != null) FilterWordController.Instance.Init();
        Debug.Log("<<<<<<<<<<<Read MasterData");   
    }
	static bool ReadFile(string directory, string fileName, System.Func<string, bool> readFunc)
	{
		string path = GameGlobal.GetPath(directory, fileName);
		{
			bool isExist = System.IO.File.Exists(path);
			if (isExist)
			{
				try
				{
					bool isRead = readFunc(path);
					if (isRead)
						return true;
				}
				catch (System.Exception e)
				{
					Debug.LogWarning(e.ToString());
					BugReportController.SaveLogFile(e.ToString());
				}
			}
		}

		// エラーメッセージ
		Debug.LogWarning(string.Format("ファイルが読み込めない:{0}", path));

		return false;
	}
	#endregion

	#region マスターデータ取得
	#region バージョンマスタ
	public static bool IsValidVersion(string versionName)
	{
		return VersionMaster.Instance.IsValidVersion(versionName);
	}
	public static bool TryGetVersion(int id, out VersionMasterData data)
	{
		return VersionMaster.Instance.TryGetMasterData(id, out data);
	}
	#endregion

	#region スキルマスタ
	public static bool TryGetSkill(int skillID, out SkillMasterData skill)
	{
		return SkillMasterClient.Instance.TryGetMasterData(skillID, out skill);
	}
	public static bool TryGetBullet(int bulletID, out SkillBulletMasterData bullet)
	{
		return SkillBulletMasterClient.Instance.TryGetMasterData(bulletID, out bullet);
	}
	public static bool TryGetBulletSet(int bulletSetID, out SkillBulletSetMasterData bulletSet)
	{
		return SkillBulletSetMaster.Instance.TryGetMasterData(bulletSetID, out bulletSet);
	}
	public static bool TryGetBulletSetList(int skillID, out List<SkillBulletSetMasterData> bulletSetList)
	{
		return SkillBulletSetMaster.Instance.TryGetSkillBulletSet(skillID, out bulletSetList);
	}
	public static bool TryGetChildBulletSet(int childBulletID, out SkillBulletBulletSetMasterData childBulletSet)
	{
		return SkillBulletBulletSetMaster.Instance.TryGetMasterData(childBulletID, out childBulletSet);
	}
	public static bool TryGetChildBulletSetList(int bulletID, out List<SkillBulletBulletSetMasterData> childBulletSetList)
	{
		return SkillBulletBulletSetMaster.Instance.TryGetChildBulletSet(bulletID, out childBulletSetList);
	}
	public static bool TryGetBulletOffset(int bulletOffsetID, out SkillBulletOffsetMasterData bulletOffset)
	{
		return SkillBulletOffsetMaster.Instance.TryGetMasterData(bulletOffsetID, out bulletOffset);
	}
	public static bool TryGetBulletCurve(int bulletOffsetId, out SkillBulletCurveMasterData bulletCurve)
	{
		return SkillBulletCurveMaster.Instance.TryGetMasterDataBySkillBulletOffsetId(bulletOffsetId, out bulletCurve);
	}
	public static bool TryGetMotion(int motionID, out SkillMotionMasterData motion)
	{
		return SkillMotionMaster.Instance.TryGetMasterData(motionID, out motion);
	}
	public static bool TryGetMotionMove(int motionMoveId, out SkillMotionMoveSetMasterData motionMove)
	{
		return SkillMotionMoveSetMaster.Instance.TryGetMasterData(motionMoveId, out motionMove);
	}
	public static bool TryGetEffectSet(int effectSetID, out SkillMotionEffectSetMasterData effectSet)
	{
		return SkillMotionEffectSetMaster.Instance.TryGetMasterData(effectSetID, out effectSet);
	}
	public static bool TryGetEffectSetList(int motionID, out List<SkillMotionEffectSetMasterData> effectSetList)
	{
		return SkillMotionEffectSetMaster.Instance.TryGetSkillEffectSet(motionID, out effectSetList);
	}
	public static bool TryGetMotion(int effectID, out SkillEffectMasterData effect)
	{
		return SkillEffectMaster.Instance.TryGetMasterData(effectID, out effect);
	}
	public static bool TryGetSkillMarker(int bulletID, out SkillMarkerMasterData skillMarker)
	{
		return SkillMarkerMaster.Instance.TryGetMasterDataBySkillBulletId(bulletID, out skillMarker);
	}
	public static bool TryGetAimingMarker(int bulletID, out SkillAimingMarkerMasterData aimingMarker)
	{
		return SkillAimingMarkerMaster.Instance.TryGetMasterDataBySkillBulletId(bulletID, out aimingMarker);
	}
	public static bool TryGetCastMarker(int bulletID, out SkillCastMarkerMasterData castMarker)
	{
		return SkillCastMarkerMaster.Instance.TryGetMasterDataBySkillBulletId(bulletID, out castMarker);
	}
	public static bool TryGetSkillGrapple(int bulletID, out SkillGrappleMasterData grapple)
	{
		return SkillGrappleMaster.Instance.TryGetMasterDataBySkillBulletId(bulletID, out grapple);
	}
	public static bool TryGetSkillGrappleDefenseDetail(int grappleID, int characterId, out SkillGrappleDefenseDetailMasterData grappleDefenseDetail)
	{
		return SkillGrappleDefenseDetailMaster.Instance.TryGetMasterData(grappleID, characterId, out grappleDefenseDetail);
	}
	public static bool TryGetGrappleChildBulletSet(int grappleId, out SkillGrappleBulletSetMasterData grappleChildBulletSet)
	{
		return SkillGrappleBulletSetMaster.Instance.TryGetMasterData(grappleId, out grappleChildBulletSet);
	}
	public static bool TryGetGrappleChildBulletSetList(int grappleID, out List<SkillGrappleBulletSetMasterData> grappleChildBulletSetList)
	{
		return SkillGrappleBulletSetMaster.Instance.TryGetChildBulletSet(grappleID, out grappleChildBulletSetList);
	}
	public static bool TryGetSkillBlowPattern(int blowPatternID, out SkillBlowPatternMasterData blowPattern)
	{
		return SkillBlowPatternMaster.Instance.TryGetMasterData(blowPatternID, out blowPattern);
	}
	public static bool TryGetSuperArmor(int superArmorID, out SkillSuperArmorSetMasterData superArmor)
	{
		return SkillSuperArmorSetMaster.Instance.TryGetMasterData(superArmorID, out superArmor);
	}
	public static bool TryGet3dManeuverGear(int bulletID, out Skill3dManeuverGearMasterData maneuverGear3d)
	{
		return Skill3dManeuverGearMaster.Instance.TryGetMasterDataBySkillBulletId(bulletID, out maneuverGear3d);
	}
	public static bool TryGetMoveSet(int skillID, out List<SkillMoveSetMasterData> moveSetList)
	{
		return SkillMoveSetMaster.Instance.TryGetMoveSet(skillID, out moveSetList);
	}
	public static bool TryGetGravityrSet(int skillID, out List<SkillGravitySetMasterData> gravitySetList)
	{
		return SkillGravitySetMaster.Instance.TryGetGravityrSet(skillID, out gravitySetList);
	}
	#endregion

	#region 旧キャラマスタ
	public static bool TryGetChara(int charaID, out CharaMasterData chara)
	{
		return CharaMaster.Instance.TryGetMasterData(charaID, out chara);
	}

    public static bool TryGetAvatar(int charaID, int avatarId, out AvatarMasterData avatar) {
        if (avatarId == 0) {
            avatarId = AvatarMaster.Instance.GetDefaultAvatarId(charaID);
        }
        return AvatarMaster.Instance.TryGetMasterData(avatarId, out avatar);
    }

    public static bool TryGetCharaLv(int charaID, int lv, out CharaLevelMasterData charaLv)
	{
		charaLv = null;
		CharaMasterData chara;
		if (TryGetChara(charaID, out chara))
			return TryGetCharaLv(chara, lv, out charaLv);
		return false;
	}
	public static bool TryGetCharaLv(CharaMasterData chara, int lv, out CharaLevelMasterData charaLv)
	{
		charaLv = null;
		if (chara == null) { return false; }
		return chara.TryGetLevelMasterData(lv, out charaLv);
	}
	public static bool TryGetCharaButtonSet(CharaLevelMasterData charaLv, out CharaButtonSetMasterData buttonSet)
	{
		buttonSet = null;
		if(charaLv == null) { return false; }
		return CharaButtonSetMaster.Instance.TryGetMasterData(charaLv.ButtonSetId, out buttonSet);

	}
	public static bool TryGetCharaButtonSet(int charaID, int lv, out CharaButtonSetMasterData buttonSet)
	{
		buttonSet = null;
		CharaLevelMasterData charaLv;
		if(TryGetCharaLv(charaID, lv, out charaLv))
			return TryGetCharaButtonSet(charaLv, out buttonSet);
		return false;
	}
	#endregion

	#region 新キャラマスタ
	public static bool TryGetChara(int charaID, out Scm.Common.XwMaster.CharaMasterData chara)
	{
		return Scm.Common.XwMaster.CharaMaster.Instance.TryGetMasterData(charaID, out chara);
	}
	public static bool TryGetCharaRank(int charaID, int rank, out Scm.Common.XwMaster.CharaRankMasterData charaRank)
	{
		return Scm.Common.XwMaster.CharaRankMaster.Instance.TryGetMasterData(charaID, rank, out charaRank);
	}
	#endregion

	#region アニメーションパックマスタ
	public static bool TryGetCommonAnimationPack(out Dictionary<int, AnimationPackMasterData> animationPacks)
	{
		return AnimationPackMaster.Instance.TryGetCommonMastarData(out animationPacks);
	}
	public static bool TryGetAnimationPack(AvatarType avatarType, out Dictionary<int, AnimationPackMasterData> animationPacks)
	{
		return AnimationPackMaster.Instance.TryGetMastarDataByCharacterId((int)avatarType, out animationPacks);
	}
	public static bool TryGetCommonUpperCombineAnimationPack(out Dictionary<int, AnimationPackMasterData> animationPacks)
	{
		return AnimationPackMaster.Instance.TryGetCommonUpperCombineMastarData(out animationPacks);
	}
	public static bool TryGetUpperCombineAnimationPack(AvatarType avatarType, out Dictionary<int, AnimationPackMasterData> animationPacks)
	{
		return AnimationPackMaster.Instance.TryGetUpperCombineMastarDataByCharacterId((int)avatarType, out animationPacks);
	}
	public static bool TryGetAnimationRunList(int id, out AnimationRunListMasterData animationRunList)
	{
		return AnimationRunListMaster.Instance.TryGetMasterData(id, out animationRunList);
	}
	public static bool TryGetAnimationActSet(AvatarType avatarType, out Dictionary<int, AnimationActSetMasterData> dictionary)
	{
		return AnimationActSetMaster.Instance.TryGetMasterData((int)avatarType, out dictionary);
	}
	#endregion

	#region オブジェクトマスタ
	public static bool TryGetObject(int objectID, out ObjectMasterData data)
	{
		return ObjectMaster.Instance.TryGetMasterData(objectID, out data);
	}
	#endregion

	#region 状態効果マスタ
	public static bool TryGetStateEffect(int stateEffectID, out StateEffectMasterData stateEffect)
	{
		bool isSuccess = StateEffectMaster.Instance.TryGetMasterData(stateEffectID, out stateEffect);
		if (!isSuccess)
		{
			string msg = string.Format("NotFound StateEffectID = {0}", stateEffectID);
			Debug.LogWarning(msg);
			BugReportController.SaveLogFileWithOutStackTrace(msg);
		}

		return isSuccess;
	}
	public static bool TryGetStateEffectLv(int stateEffectID, int lv, out StateEffectLevelMasterData stateEffectLv)
	{
		stateEffectLv = null;
		bool isSuccess = false;
		{
			StateEffectMasterData stateEffect;
			if (StateEffectMaster.Instance.TryGetMasterData(stateEffectID, out stateEffect))
			{
				isSuccess = TryGetStateEffectLv(stateEffect, lv, out stateEffectLv);
				if (!isSuccess)
				{
				}
			}
			else
			{
				string msg = string.Format("NotFound StateEffectID = {0}", stateEffectID);
				Debug.LogWarning(msg);
				BugReportController.SaveLogFileWithOutStackTrace(msg);
			}
		}

		return isSuccess;
	}
	public static bool TryGetStateEffectLv(StateEffectMasterData stateEffect, int lv, out StateEffectLevelMasterData stateEffectLv)
	{
		stateEffectLv = null;
		if (stateEffect == null) { return false; }
		return stateEffect.TryGetLevelMasterData(lv, out stateEffectLv);
	}
	#endregion

	#region 新アイテムマスタ
	public static bool TryGetItem(int itemMasterID, out Scm.Common.XwMaster.ItemMasterData item)
	{
		return Scm.Common.XwMaster.ItemMaster.Instance.TryGetMasterData(itemMasterID, out item);
	}
	#endregion

	#region プレイヤーグレードマスタ
	public static bool TryGetPlayerGrade(int grade, out PlayerGradeMasterData playerGrade)
	{
		bool isSuccess = PlayerGradeMaster.Instance.TryGetMasterDataByGrade(grade, out playerGrade);
		if(!isSuccess)
		{
			string msg = string.Format("NotFound playerGradeID = {0}", grade);
			BugReportController.SaveLogFile (msg);
			Debug.LogWarning(msg);
		}
		return isSuccess;
	}
	#endregion

	#region バトルフィールドマスタ
	public static bool TryGetBattleField(int battleFieldID, out BattleFieldMasterData data)
	{
		return BattleFieldMaster.Instance.TryGetMasterData(battleFieldID, out data);
	}
	#endregion

	#region 二つ名マスタ
	public static bool TryGetAliasWord(int aliasWordId, out AliasWordMasterData data)
	{
		return AliasWordMaster.Instance.TryGetMasterData(aliasWordId, out data);
	}
	public static bool TryGetAliasParticle(int aliasParticleId, out AliasParticleMasterData data)
	{
		return AliasParticleMaster.Instance.TryGetMasterData(aliasParticleId, out data);
	}
	#endregion

	#region 所属勢力マスタ
	public static bool TryGetAffiliationForce(int affiliationForceId, out AffiliationForceMasterData data)
	{
		return AffiliationForceMaster.Instance.TryGetMasterData(affiliationForceId, out data);
	}
	#endregion

	#region コンボ賞賛マスタ
	public static bool TryGetComboPraise(int combo, out ComboPraiseMasterData data)
	{
		return ComboPraiseMaster.Instance.TryGetPraiseMasterData(combo, out data);
	}
	#endregion

	#region ObjectUIマスタ
	public static bool TryGetObjectUI(int id, out ObjectUserInterfaceMasterData data)
	{
		return ObjectUserInterfaceMaster.Instance.TryGetMasterData(id, out data);
	}
	#endregion

	#region カラーマスタ
	public static bool TryGetColor(ColorType colorType, out Color color)
	{
		ColorMasterData data;
		if (ColorMaster.Instance.TryGetMasterData((int)colorType, out data))
		{
			color = new Color(data.R, data.G, data.B, data.A);
			return true;
		}
		else
		{
			BugReportText(colorType);
			color = Color.white;
			return false;
		}
	}
	public static Color GetColor(ColorType colorType)
	{
		return GetColor(colorType, Color.white);
	}
	public static Color GetColor(ColorType colorType, Color errorColor)
	{
		ColorMasterData data;
		if (ColorMaster.Instance.TryGetMasterData((int)colorType, out data))
		{
			return new Color32((byte)data.R, (byte)data.G, (byte)data.B, (byte)data.A);
		}
		else
		{
			BugReportText(colorType);
			return errorColor;
		}
	}
	static void BugReportText(ColorType colorType)
	{
		// バグレポート
		string msg = string.Format("NotFound ColorType = {0}", colorType);
		Debug.LogWarning(msg);
		BugReportController.SaveLogFileWithOutStackTrace(msg);
	}
	#endregion

	#region テキストマスタ
	public static string GetText(TextType textType)
	{
		string text = "";
		if (Instance == null || !Instance.IsReadTextMaster)
		{
			return text;
		}
		if (!TextMaster.Instance.GetText((int)textType, out text))
		{
			// エラーメッセージ
			BugReportText(textType);
		}
		return text;
	}

    public static string GetText(TextCategory textCategory, int tag, string[] replace)
    {
        string text = "";
        if (!TextMaster.Instance.GetTextByTag1((int)TextCategory.QuestDescription, tag, replace, out text))
        {
            BugReportText(textCategory);
        }
        return text;
    }

	public static string GetText(TextType textType, params string[] replace)
	{
		string text = "";
		if (Instance == null || !Instance.IsReadTextMaster)
		{
			return text;
		}
		if (!TextMaster.Instance.GetText((int)textType, replace, out text))
		{
			// エラーメッセージ
			BugReportText(textType);
		}
		return text;
	}
	// 応急処置:取得に失敗してもBugReportを吐かない.
	public static string GetTextNonBugReport(TextType textType)
	{
		string text = "";
		if (Instance == null || !Instance.IsReadTextMaster)
		{
			return text;
		}
		if (!TextMaster.Instance.GetText((int)textType, out text))
		{
		}
		return text;
	}
	// テキストマスタから取得出来ない時はデフォルトのテキストを取得する
	public static string GetTextDefalut(TextType textType, string defalut)
	{
		string masterText = GetText(textType);
		string text = string.IsNullOrEmpty(masterText) ? defalut : masterText;
		return text;
	}
	static void BugReportText(TextType textType)
	{
		// バグレポート
		string msg = string.Format("NotFound TextType = {0}({1})", textType, (int)textType);
		Debug.LogWarning(msg);
		BugReportController.SaveLogFileWithOutStackTrace(msg);
	}

    static void BugReportText(TextCategory textCategory)
    {
        // バグレポート
        string msg = string.Format("NotFound TextCategory = {0}({1})", textCategory, (int)textCategory);
        Debug.LogWarning(msg);
        BugReportController.SaveLogFileWithOutStackTrace(msg);
    }
    #endregion

    #region 強化合成レベルマスタ
    public static bool TryGetCharaPowerupLevel(int rank, int powerupLevel, out Scm.Common.XwMaster.CharaPowerupLevelMasterData data)
	{
		return Scm.Common.XwMaster.CharaPowerupLevelMaster.Instance.TryGetMasterData(rank, powerupLevel, out data);
	}
	public static bool TryGetCharaMaxPowerupLevel(int rank, out Scm.Common.XwMaster.CharaPowerupLevelMasterData data)
	{
		return Scm.Common.XwMaster.CharaPowerupLevelMaster.Instance.TryGetMaxLevelMasterData(rank, out data);
	}
	#endregion

	#region 進化合成素材マスタ
	public static bool TryGetCharaEvolutionMaterial(int charaMasterId, int rank, out List<Scm.Common.XwMaster.CharaEvolutionMaterialMasterData> dataList)
	{
		return Scm.Common.XwMaster.CharaEvolutionMaterialMaster.Instance.TryGetMasterDataList(charaMasterId, rank, out dataList);
	}
	#endregion

	#region シンクロ制限マスタ
	public static bool TryGetSynchroRestrictionByTotalBonus(int totalBonus, out Scm.Common.XwMaster.CharaSynchroRestrictionMasterData data)
	{
		return Scm.Common.XwMaster.CharaSynchroRestrictionMaster.Instance.TryGetMasterDataByTotalBonus(totalBonus, out data);
	}
	#endregion

	#region クライアント/サーバ共通定義設定
	public static string GetCommonSettingValue(string name)
	{
		string value = Scm.Common.XwMaster.CommonSettingMaster.Instance.GetSettingValue(name);
		if(string.IsNullOrEmpty(value))
		{
			// 見つからない
			string msg = System.Reflection.MethodBase.GetCurrentMethod().Name + "> Setting Name=" + name + " not found." + " value = " + value;
			Debug.LogWarning(msg);
		}

		return value;
	}
	#endregion

	#region アチーブメントマスタ
	public static bool TryGetAchievement(int id, out Scm.Common.XwMaster.AchievementMasterData data)
	{
		return Scm.Common.XwMaster.AchievementMaster.Instance.TryGetMasterData(id, out data);
	}
	#endregion

	#region アチーブメントタブマスタ
	public static bool TryGetAchievementTab( int id, out Scm.Common.XwMaster.AchievementTabMasterData data )
	{
		return Scm.Common.XwMaster.AchievementTabMaster.Instance.TryGetMasterData(id, out data );
	}
	#endregion

	#region アイコンマスタ
	public static bool TryGetIcon(int id, out Scm.Common.XwMaster.IconMasterData data)
	{
		return Scm.Common.XwMaster.IconMaster.Instance.TryGetMasterData(id, out data);
	}
	#endregion

	#region ショップマスタ
	public static bool TryGetShop( int id, out Scm.Common.XwMaster.ShopItemMasterData data)
	{
		return Scm.Common.XwMaster.ShopItemMaster.Instance.TryGetMasterData(id, out data );
	}
	#endregion

	#region チケットマスタ
	public static bool TryGetTicket( int id, out Scm.Common.XwMaster.ShopTicketMasterData data )
	{
		return Scm.Common.XwMaster.ShopTicketMaster.Instance.TryGetMasterData(id, out data );
	}
	#endregion

    #region BattleField Waypoint
    public static List<BattleFieldWaypointMasterData> TryGetBattleFieldWaypoint(int id)
    {
        return BattleFieldWaypointMaster.Instance.GetMasterDataByFieldID(id);
    }
    #endregion

    #region GetCharaProfileMasterData
    public static bool TryGetCharaProfileMasterData(int id, out CharaProfileMasterData data)
    {
        return CharaProfileMaster.Instance.TryGetMasterData(id, out data);
    }

    public static bool TryGetCharaLevelMasterData(int id, int level, out CharaLevelMasterData data)
    {
        return CharaLevelMaster.Instance.TryGetMasterData(id, level, out data);
    }

    public static bool TryGetCharaStarMasterData(int id, out CharaStarMasterData data)
    {
        return CharaStarMaster.Instance.TryGetMasterData(id, out data);
    }

    public static List<CharaLevelMasterData> TryGetLevelListByCharacterId(int id)
    {
        return CharaLevelMaster.Instance.GetLevelListByCharacterId(id);
    }

    public static bool TryGetRewardMasterData(int id, out RewardMasterData data)
    {
        return RewardMaster.Instance.TryGetMasterData(id, out data);
    }

    public static bool TryGetReplayVoiceMasterData(int id, out ReplayVoiceMasterData data)
    {
        return ReplayVoiceMaster.Instance.TryGetMasterData(id, out data);
    }

    public static bool TryGetStoryMasterData(int id, out StoryMasterData data)
    {
        return StoryMaster.Instance.TryGetMasterData(id, out data);
    }

    public static bool TryGetStoryItemMasterData(int id, out StoryItemMasterData data)
    {
        return StoryItemMaster.Instance.TryGetMasterData(id, out data);
    }

    public static bool TryGetStoryEffectMasterData(int id, out StoryEffectMasterData data)
    {
        return StoryEffectMaster.Instance.TryGetMasterData(id, out data);
    }

    public static string[] TryGetChinaNgWordMasterData()
    {
        return ChinaNgWordMaster.Instance.Words;
    }

    #endregion

    #region LoginBonus
    public static bool TryGetLoginBonusMasterData(int id, out LoginBonusMasterData data)
    {
        return LoginBonusMaster.Instance.TryGetMasterData(id, out data);
    }
    #endregion

    #region QuestDaily
    public static bool TryGetDailyQuestMasterData(int id, out DailyQuestMasterData data)
    {
        return DailyQuestMaster.Instance.TryGetMasterData(id, out data);
    }
    #endregion

    #region Get guide
    public static bool TryGetGuideMasterData(int id, out GuideMasterData data)
    {
        return GuideMaster.Instance.TryGetMasterData(id, out data);
    }
    #endregion

    #region Get all guide
    public static List<GuideMasterData> GetAllGuideMasterData()
    {
        return GuideMaster.Instance.GetAllData();
    }
    #endregion

    #region Get market url
    public static string TryGetAppMarketUrl(string distributionCode)
    {
        return AppMarketMaster.Instance.GetMarketUrl(distributionCode);
    }
    #endregion
    #endregion
}

/// <summary>
/// クライアント/サーバ共通定義設定データクラス
/// </summary>
public static class MasterDataCommonSetting
{
	/// <summary>
	/// 合成
	/// </summary>
	public static class Fusion
	{
		/// <summary>
		/// 強化スロット最大数
		/// </summary>
		public static readonly int MaxPowerupSlotNum = MasterData.GetCommonSettingValue("FusionMaxPowerupSlotNum").TryParseInt(20);

		/// <summary>
		/// シンクロ最大回数値取得
		/// </summary>
		public static readonly int MaxSynchroCount = MasterData.GetCommonSettingValue("FusionMaxSynchroCount").TryParseInt(99);

		/// <summary>
		/// パラメータごとのシンクロボーナス最大値取得
		/// </summary>
		public static readonly int FusionMaxParameterSynchroBonus = MasterData.GetCommonSettingValue("FusionMaxParameterSynchroBonus").TryParseInt(20);
	}

	/// <summary>
	/// プレイヤー
	/// </summary>
	public static class Player
	{
		/// <summary>
		/// ゲーム内通貨の最大値
		/// </summary>
		public static readonly int PlayerMaxGameMoney = MasterData.GetCommonSettingValue("PlayerMaxGameMoney").TryParseInt(99999999);

		/// <summary>
		/// キャラクターの最大ランク数
		/// </summary>
		public static readonly int PlayerMaxRank = MasterData.GetCommonSettingValue("PlayerMaxRank").TryParseInt(5);
	}

	/// <summary>
	/// メール
	/// </summary>
	public static class Mail
	{
		/// <summary>
		/// 運営メール最大ロック件数
		/// </summary>
		public static readonly int AdminMailMaxLockCount = MasterData.GetCommonSettingValue("AdminMailMaxLockCount").TryParseInt(20);

		/// <summary>
		/// 運営メール最大件数
		/// </summary>
		public static readonly int AdminMailMaxCount = MasterData.GetCommonSettingValue("AdminMailMaxCount").TryParseInt(500);

		/// <summary>
		/// 運営メール保持日数
		/// </summary>
		public static readonly int AdminMailKeepDays = MasterData.GetCommonSettingValue("AdminMailKeepDays").TryParseInt(90);

		/// <summary>
		/// プレゼントメール最大ロック件数
		/// </summary>
		public static readonly int PresentMailMaxLockCount = MasterData.GetCommonSettingValue("PresentMailMaxLockCount").TryParseInt(20);

		/// <summary>
		/// プレゼントメール最大件数
		/// </summary>
		public static readonly int PresentMailMaxCount = MasterData.GetCommonSettingValue("PresentMailMaxCount").TryParseInt(100);

		/// <summary>
		/// プレゼントメール保持日数
		/// </summary>
		public static readonly int PresentMailKeepDays = MasterData.GetCommonSettingValue("PresentMailKeepDays").TryParseInt(90);

	}

}