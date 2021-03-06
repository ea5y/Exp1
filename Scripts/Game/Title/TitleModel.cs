/// <summary>
/// タイトルデータ
/// 
/// 2015/12/14
/// </summary>
using UnityEngine;
using System.Collections;

namespace XUI
{
	namespace Title
	{
		/// <summary>
		/// タイトルデータインターフェイス
		/// </summary>
		public interface IModel
		{
			/// <summary>
			/// プレイヤー名最大制限数
			/// </summary>
			int PlayerNameMax { get; }

			/// <summary>
			/// アプリのバージョン
			/// </summary>
			string AppVersion { get; }
		}

		/// <summary>
		/// タイトルデータ
		/// </summary>
		[System.Serializable]
		public class Model : IModel
		{
			/// <summary>
			/// プレイヤー名最大制限数
			/// </summary>
			public int PlayerNameMax { get { return 8; } }

			/// <summary>
			/// アプリのバージョン
			/// 現在2015/12/14ではGoOne版のバージョンを表示する
			/// </summary>
			//public string AppVersion { get { return GoOneObsolateSrc.GetGoOneVersion(); } }
			public string AppVersion
			{
				get
				{
					return PluginController.PackageInfo.versionName1;
				}
			}
		}
	}
}