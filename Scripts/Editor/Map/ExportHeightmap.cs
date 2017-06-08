using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;

public class ExportHeightmap {

    [MenuItem("Scm/Map/Export Heightmap...")]
    public static void DoExportHeightmap() {

        if (Selection.activeGameObject == null) {
            EditorUtility.DisplayDialog("Export Map Error", "Not selected a map object", "OK");
            return;
        }

        Collider[] colliders = Selection.activeGameObject.GetComponentsInChildren<Collider>();

        if (colliders.Length == 0) {
            EditorUtility.DisplayDialog("Export Map Error", "No collider found in map object", "OK");
            return;
        }

        string path = EditorUtility.SaveFilePanel("Save to binary heightmap file...", Directory.GetCurrentDirectory(),
                Selection.activeGameObject.name + ".lhm", "lhm");

        int count = colliders.Length;

        // calculate outside bounds
        Bounds bounds = new Bounds();
        for (int i = 0; i < count; i++) {
            if (i == 0) {
                bounds = colliders[i].bounds;
            } else {
                bounds.Encapsulate(colliders[i].bounds);
            }
        }

        Vector2 minXZ = new Vector2(bounds.min.x, bounds.min.z);
        Vector2 maxXZ = new Vector2(bounds.max.x, bounds.max.z);
        // The extra heights in fact are not necessary
        float realMinY = bounds.min.y;
        float realMaxY = bounds.max.y;
        float minY = Mathf.Max(realMinY, -20.0f);
        float maxY = Mathf.Min(realMaxY, 20.0f);
        float sampleYStart = 200.0f;

        const float xzSample = 0.5f;

        float fWidth = (maxXZ.x - minXZ.x) / xzSample;
        float fHeight = (maxXZ.y - minXZ.y) / xzSample;
        int iWidth = Mathf.CeilToInt(fWidth);
        int iHeight = Mathf.CeilToInt(fHeight);
        float fYSample = (maxY - minY) / 254.0f;

        Debug.Log("minXZ=" + minXZ + ",maxXZ=" + maxXZ + ",minY=" + minY + ",maxY=" + maxY + ",colliders=" + colliders.Length + ",quantY=" + fYSample);
        if (iWidth >= short.MaxValue || iHeight >= short.MaxValue || iWidth <= 0 || iHeight <= 0) {
            EditorUtility.DisplayDialog("Export Map Error", "Size not valid", "OK");
            return;
        }

        /*
        byte fy;
        fy = MixSampleHeight(colliders, -13.625f, -15.53125f, realMinY, realMaxY, minY, fYSample, sampleYStart);
        Debug.Log("fy=" + fy);
        fy = MixSampleHeight(colliders, -16.1875f, -18.09375f, realMinY, realMaxY, minY, fYSample, sampleYStart);
        Debug.Log("fy2=" + fy);
        return;//test*/

        bool canceled = false;

        Texture2D texture = new Texture2D(iWidth, iHeight);
        using (FileStream outfs = new FileStream(path, FileMode.Create)) {
            FileOutputStream fos = new FileOutputStream(outfs);
            DataOutputStream dos = new DataOutputStream(fos);
            dos.setReverse(true);

            // header
            dos.write(Encoding.UTF8.GetBytes("LHM"), 0, 3);
            // version
            dos.writeByte(0x01);
            dos.writeByte(0x02);
            dos.writeByte(0x00);
            // width
            dos.writeShort((short)iWidth);
            // height
            dos.writeShort((short)iHeight);
            // threashold upper;
            dos.writeFloat(0.0f);
            // threashold lower)
            dos.writeFloat(0.0f);
            // offset x
            dos.writeFloat(minXZ.x);
            // offset z
            dos.writeFloat(minXZ.y);
            // sample
            dos.writeFloat(xzSample);
            // number of layer
            dos.writeByte(1);

            // offset y
            dos.writeFloat(minY);
            // sample
            dos.writeFloat(fYSample);

            // height(map)
            for (int n = 0; n < iHeight; n++) {
                for (int m = 0; m < iWidth; m++) {

                    float x = minXZ.x + m * xzSample;
                    float z = minXZ.y + n * xzSample;

                    byte y = MixSampleHeight(colliders, x, z, realMinY, realMaxY, minY, fYSample, sampleYStart);

                    //Debug.Log("m=" + m + ",n=" + n + ",y=" + y);

                    dos.writeByte((byte)y);
                    texture.SetPixel(m, n, y == 0 ? new Color32(255, 0, 255, 255) : new Color32(y, y, y, 255));
                }

                float progress = (float)((n * iWidth)) / (iWidth * iHeight);
                if (EditorUtility.DisplayCancelableProgressBar("Export Heightmap", "Processing...", progress)) {
                    canceled = true;
                    break;
                }
            }

            // color
            {
                // has color
                dos.writeByte(1);
                for (int n = 0; n < iHeight; n++) {
                    for (int m = 0; m < iWidth; m++) {
                        dos.writeByte((byte)0x00);
                        dos.writeByte((byte)0x00);
                        dos.writeByte((byte)0x00);
                    }
                }
            }

            EditorUtility.ClearProgressBar();
            File.WriteAllBytes(Path.ChangeExtension(path, "png"), texture.EncodeToPNG());
        }

        if (canceled) {
            File.Delete(path);
        }
    }

    private static byte MixSampleHeight(Collider[] colliders, float x, float z, float realMinY, float realMaxY, float minY, float fYSample, float sampleYStart) {
        const float nearby_sample_dist = 0.125f;

        bool hasHeight = true;
        float y00;
        float y01;
        float y10;
        float y11;
        float y000;
        float y002;
        float y020;
        float y022;
        float y100;
        float y103;
        float y130;
        float y133;
        if (!SampleHeight(colliders, x - nearby_sample_dist, z - nearby_sample_dist, realMinY, realMaxY, fYSample, sampleYStart, out y00)) {
            hasHeight = false;
        }
        if (!SampleHeight(colliders, x - nearby_sample_dist, z + nearby_sample_dist, realMinY, realMaxY, fYSample, sampleYStart, out y01)) {
            hasHeight = false;
        }
        if (!SampleHeight(colliders, x + nearby_sample_dist, z - nearby_sample_dist, realMinY, realMaxY, fYSample, sampleYStart, out y10)) {
            hasHeight = false;
        }
        if (!SampleHeight(colliders, x + nearby_sample_dist, z + nearby_sample_dist, realMinY, realMaxY, fYSample, sampleYStart, out y11)) {
            hasHeight = false;
        }

        if (!SampleHeight(colliders, x - nearby_sample_dist * 2.5f, z - nearby_sample_dist * 2.5f, realMinY, realMaxY, fYSample, sampleYStart, out y000)) {
            hasHeight = false;
        }
        if (!SampleHeight(colliders, x - nearby_sample_dist * 2.5f, z + nearby_sample_dist * 2.5f, realMinY, realMaxY, fYSample, sampleYStart, out y002)) {
            hasHeight = false;
        }
        if (!SampleHeight(colliders, x + nearby_sample_dist * 2.5f, z - nearby_sample_dist * 2.5f, realMinY, realMaxY, fYSample, sampleYStart, out y020)) {
            hasHeight = false;
        }
        if (!SampleHeight(colliders, x + nearby_sample_dist * 2.5f, z + nearby_sample_dist * 2.5f, realMinY, realMaxY, fYSample, sampleYStart, out y022)) {
            hasHeight = false;
        }

        if (!SampleHeight(colliders, x - nearby_sample_dist * 8.0f, z - nearby_sample_dist * 8.0f, realMinY, realMaxY, fYSample, sampleYStart, out y100)) {
            hasHeight = false;
        }
        if (!SampleHeight(colliders, x - nearby_sample_dist * 8.0f, z + nearby_sample_dist * 8.0f, realMinY, realMaxY, fYSample, sampleYStart, out y103)) {
            hasHeight = false;
        }
        if (!SampleHeight(colliders, x + nearby_sample_dist * 8.0f, z - nearby_sample_dist * 8.0f, realMinY, realMaxY, fYSample, sampleYStart, out y130)) {
            hasHeight = false;
        }
        if (!SampleHeight(colliders, x + nearby_sample_dist * 8.0f, z + nearby_sample_dist * 8.0f, realMinY, realMaxY, fYSample, sampleYStart, out y133)) {
            hasHeight = false;
        }

        byte y = 0;
        if (hasHeight) {
            y = (byte)Mathf.Min((((y00 + y01 + y10 + y11 + y000 + y002 + y020 + y022 + y100 + y103 + y130 + y133) / 12 - minY) / fYSample) + 1, 255);
        }

        return y;
    }

    private static bool SampleHeight(Collider[] colliders, float x, float z, float minY, float maxY, float ySample, float sampleYStart, out float result) {
        bool hasResult = false;
        result = float.MinValue;
        for (int i = 0; i < colliders.Length; i++) {
            if (colliders[i].gameObject.layer != LayerNumber.vsPlayer
                && colliders[i].gameObject.layer != LayerNumber.vsPlayer_Bullet) {
                continue;
            }
            RaycastHit hitInfo;
            if (colliders[i].Raycast(new Ray(new Vector3(x, sampleYStart, z), Vector3.down), out hitInfo, sampleYStart - minY + 1.0f /* threshold */)) {
                result = Mathf.Max(result, hitInfo.point.y);
                hasResult = true;
            }
        }
        return hasResult;
    }
}
