using System;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
using System.IO;

namespace ReferenceViewer
{
    public class Creator
    {
        public static void Build(Action callback = null)
        {
            if (!UnityEditor.SceneManagement.EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;

            var currentScene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().name;
		
            var data = new Data();

            Generate.Build(AssetDatabase.GetAllAssetPaths(), assetData =>
            {
                data.assetData.AddRange(assetData);
                EditorUtility.UnloadUnusedAssetsImmediate();

				if(string.IsNullOrEmpty(currentScene))
					UnityEditor.SceneManagement.EditorSceneManager.NewScene(UnityEditor.SceneManagement.NewSceneSetup.DefaultGameObjects);
				else
                	UnityEditor.SceneManagement.EditorSceneManager.OpenScene(currentScene);
                
				Export(data);
                if (callback != null)
                    callback();
            });
        }

        private static void Export(Data data)
        {
            data.assetData = data.assetData.OrderBy(d => Path.GetExtension(d.path)).ToList();
            const string directory = "build/ReferenceViewer";

            Directory.CreateDirectory(directory);

            foreach (var assetData in data.assetData.Where(assetData => assetData.sceneData.Count != 0))
            {
                assetData.sceneData =
                    assetData.sceneData.Distinct(new CompareSelector<SceneData, string>(s => s.name + s.guid)).ToList();
            }
            File.WriteAllBytes(directory + "/data.dat", ObjectToByteArray(data));
        }

        static byte[] ObjectToByteArray(object obj)
        {
            if (obj == null)
                return null;
            var bf = new BinaryFormatter();
            var ms = new MemoryStream();
            bf.Serialize(ms, obj);
            return ms.ToArray();
        }
    }
}