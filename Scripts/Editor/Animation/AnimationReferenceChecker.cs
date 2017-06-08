using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

public class AnimationReferenceChecker {

    [MenuItem("Assets/Check Animation Pack References")]
    public static void DoCheckAnimationPackReferences() {

        if (Selection.activeObject == null) {
            EditorUtility.DisplayDialog("Check Animation Pack Error", "Not selected an animation reference", "OK");
            return;
        }

        AnimationReference reference = Selection.activeObject as AnimationReference;
        if (reference == null) {
            EditorUtility.DisplayDialog("Check Animation Pack Error", "Not selected an animation reference", "OK");
            return;
        }

        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        Debug.Log("path=" + path);

        Regex re = new Regex("p([0-9]{3}).*");
        string[] parts = path.Split(Path.DirectorySeparatorChar);
        int characterId = -1;
        for (int i = parts.Length - 1; i >= 0; i--) {
            Debug.Log(parts[i]);
            Match match = re.Match(parts[i]);
            if (match.Success && match.Groups.Count > 0) {
                Debug.Log("success:" + match.Groups[1].Value);
                if (int.TryParse(match.Groups[1].Value, out characterId)) {
                    break;
                }
            }
        }

        if (characterId < 0) {
            Debug.LogError("Not found directory contains p0?? for character.");
            return;
        }

        switch (characterId) {
        case 1:
            characterId = 1001;
            break;
        case 2:
            characterId = 1004;
            break;
        case 4:
            characterId = 1007;
            break;
        default:
            break;
        }

        MasterData.Read();

        Dictionary<int, Scm.Common.Master.AnimationPackMasterData> commonPacks;
        Dictionary<int, Scm.Common.Master.AnimationPackMasterData> animPacks;
        if (!MasterData.TryGetCommonAnimationPack(out commonPacks)) {
            Debug.LogError("Not found common animation pack:");
        }
        if (!MasterData.TryGetAnimationPack((AvatarType)characterId, out animPacks)) {
            Debug.LogError("Not found character's animation pack:" + characterId);
        }
        HashSet<Scm.Common.Master.AnimationPackMasterData> foundList = new HashSet<Scm.Common.Master.AnimationPackMasterData>();
        for (int i = 0; i < reference.AnimationClipList.Length; i++) {
            AnimationClip clip = reference.AnimationClipList[i];
            int index = clip.name.IndexOf('-');
            string newName = clip.name.Substring(index + 1);

            int commonCount = 0;
            int animCount = 0;
            foreach (Scm.Common.Master.AnimationPackMasterData data in commonPacks.Values) {
                //Debug.Log("Common data.AnimationName:" + data.AnimationName);
                if (data.AnimationName == newName) {
                    commonCount++;
                    foundList.Add(data);
                }
            }
            foreach (Scm.Common.Master.AnimationPackMasterData data in animPacks.Values) {
                //Debug.Log("Chara data.AnimationName:" + data.AnimationName);
                if (data.AnimationName == newName) {
                    animCount++;
                    foundList.Add(data);
                }
            }
            if (commonCount == 0 && animCount == 0) {
                Debug.LogError("Extra animation clip found:" + clip.name);
            } else if (commonCount > 0 && animCount > 0) {
                Debug.LogError("Duplicated (both in common and character) animation clip found:" + clip.name);
            }
        }
        foreach (Scm.Common.Master.AnimationPackMasterData data in commonPacks.Values) {
            if (!foundList.Contains(data)) {
                Debug.LogError("Not found common clip:" + data.AnimationName + "(" + data.ID + ")");
            }
        }
        foreach (Scm.Common.Master.AnimationPackMasterData data in animPacks.Values) {
            if (!foundList.Contains(data)) {
                Debug.LogError("Not found character's clip:" + data.AnimationName + "(" + data.ID + ")");
            }
        }
        Debug.Log("Check Finished");
    }
}
