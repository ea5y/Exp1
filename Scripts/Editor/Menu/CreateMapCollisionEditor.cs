using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

public class CreateMapCollisionEditor : EditorWindow
{
	#region 定義
	// モードのボタン定義
	// これを増やすと勝手にボタンが増えます
	// ただボタン名は下のListに追加しないとエラーが出るので追加してください
	enum Mode
	{
		Outer,
		Large,
		Middle,
		Small,
		Object,
	}


	// Modeの順番に対応してるのでenu追加したらこっちにも名前追加してください
	List<string> ButtonNames = new List<string>()
	{
		"外壁",
		"大壁",
		"中壁",
		"小壁",
		"オブジェクト",
	};
	#endregion

	#region フィールド＆プロパティ
	Mode NowSelectMode { get; set; }
	#endregion

	#region 関数
	[MenuItem("Scm/Map/Create Map Collision...")]
	public static void OpenWindow()
	{
		var wnd = EditorWindow.GetWindow<CreateMapCollisionEditor>(false, "Create Map Collision", true);
		wnd.minSize = new Vector2(400,300);
		wnd.Init();
	}

 	public void Init()
	{
		this.NowSelectMode = Mode.Outer;
	}

	void OnGUI()
	{
		GUILayout.Space(5f);
		GUILayout.Label(
			"選択オブジェクトのコリジョンを生成します。（複数選択可）\r\n" +
			" \r\n" +
			" ・外壁       …カメラコリジョンのみ生成\r\n" +
			" ・大壁(6m)   …高さ100m、弾丸6m で生成\r\n" +
			" ・中壁(2m)   …高さ2m で生成\r\n" +
			" ・小壁(0.8m) …高さ0.8m、弾丸0.5m で生成\r\n" +
			" ・オブジェクト…オブジェクトサイズで全種生成"
		);


		GUILayout.Space(15f);

		// 各モードボタンの表示
		GUILayout.Label("Mode");
		string[] selectString = ButtonNames.ToArray();
		this.NowSelectMode = (Mode)GUILayout.SelectionGrid((int)this.NowSelectMode, selectString, selectString.Length);

		GUILayout.Space(30f);


		// ここで実行ボタン判定　押されてたら中の処理を実行
		if(GUILayout.Button("実行",GUILayout.MinWidth(80f),GUILayout.MaxWidth(80f)))
		{
			int count = Exec();
			if (count > 0)
			{
				var str = string.Format("{0}個の MeshRenderer に{1}を追加しました", count, this.ButtonNames[(int)this.NowSelectMode]);
				Debug.Log(str);
			}
			else
			{
				var str = "選択されているオブジェクトに MeshRenderer がありません";
				Debug.Log(str);
				EditorUtility.DisplayDialog("Create Map Collision", str, "OK");
			}
		}
	}
	int Exec()
	{
		int count = 0;
		var gos = new List<GameObject>(Selection.gameObjects);
		foreach (var go in gos)
		{
			// メッシュレンダラーがあるオブジェクトのみ
			MeshRenderer mr = go.GetComponent<MeshRenderer>();
			if (mr == null)
				continue;
			count++;

			var src = go.AddComponent<BoxCollider>();
			var cameraName = go.name + "_Col_vsC";
			var bulletName = go.name + "_Col_vsB";
			var playerName = go.name + "_Col_vsP";

			BoxCollider bulletBC;

			switch (this.NowSelectMode)
			{
			case Mode.Outer:
				// 外壁
				AddChild(src.size, src.center, go, cameraName, LayerNumber.MapWallCol);
				break;
			case Mode.Large:
				// 大壁
				AddChild(new Vector3(src.size.x, 6f, 1f), new Vector3(src.center.x, 3f, 0f), go, bulletName, LayerNumber.vsBullet, out bulletBC);
				AddChild(bulletBC.size, bulletBC.center, go, cameraName, LayerNumber.MapWallCol);
				AddChild(new Vector3(src.size.x + 2f, 100f, 3f), new Vector3(src.center.x, 50f, src.center.z), bulletBC.gameObject, playerName, LayerNumber.vsPlayer);
				break;
			case Mode.Middle:
				AddChild(new Vector3(src.size.x, 2f, 1f), new Vector3(src.center.x, 1f, 0f), go, bulletName, LayerNumber.vsBullet, out bulletBC);
				AddChild(new Vector3(src.size.x + 2f, 2f, 3f), new Vector3(src.center.x, 1f, 0f), bulletBC.gameObject, playerName, LayerNumber.vsPlayer);
				break;
			case Mode.Small:
				AddChild(new Vector3(src.size.x, 0.5f, 1f), new Vector3(src.center.x, 0.25f, 0f), go, bulletName, LayerNumber.vsBullet, out bulletBC);
				AddChild(new Vector3(src.size.x + 2f, 0.8f, 3f), new Vector3(src.center.x, 0.4f, 0f), bulletBC.gameObject, playerName, LayerNumber.vsPlayer);
				break;
			case Mode.Object:
				// オブジェクト
				AddChild(src.size, src.center, go, bulletName, LayerNumber.vsBullet, out bulletBC);
				AddChild(src.size, src.center, go, cameraName, LayerNumber.MapWallCol);
				AddChild(new Vector3(src.size.x + 2f, src.size.y, src.size.z + 2f), src.center, bulletBC.gameObject, playerName, LayerNumber.vsPlayer);
				break;
			}

			UnityEngine.Object.DestroyImmediate(src);
		}

		return count;
	}
	static void AddChild(Vector3 size, Vector3 center, GameObject parent, string name, int layer, out BoxCollider child)
	{
		child = AddChild(size, center, parent, name, layer);
	}
	static BoxCollider AddChild(Vector3 size, Vector3 center, GameObject parent, string name, int layer)
	{
		// 子どものゲームオブジェクトのセットアップ.
		var go = new GameObject();
		if (parent != null)
		{
			var t = go.transform;
			t.parent = parent.transform;
			t.localPosition = Vector3.zero;
			t.localRotation = Quaternion.identity;
			t.localScale = Vector3.one;
		}
		go.name = name;
		go.layer = layer;
		
		// コライダー追加
		var bc = go.AddComponent<BoxCollider>();
		bc.size = size;
		bc.center = center;
		
		return bc;
	}
	#endregion
}
