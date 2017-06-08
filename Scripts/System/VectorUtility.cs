using UnityEngine;
using System.Collections;

public static class VectorExtend
{
    public static void LookTo(this Transform pTransform, Transform pTarget)
    {
        Vector3 t = pTarget.position;
        t.y = pTransform.position.y;
        pTransform.LookAt(t);
    }
}

public struct Matrix2x2
{
    public float m00;
    public float m10;
    public float m01;
    public float m11;

    public Matrix2x2(float radian)
    {
        float sin = Mathf.Sin(radian);
        float cos = Mathf.Cos(radian);
        m00 = cos;
        m01 = -sin;
        m10 = sin;
        m11 = cos;
    }

    public Matrix2x2(Vector4 pVector4)
    {
        m00 = pVector4.x;
        m01 = pVector4.y;
        m10 = pVector4.z;
        m11 = pVector4.w;
    }

    public static Matrix2x2 zero
    {
        get
        {
            return new Matrix2x2()
          {
              m00 = 0.0f,
              m01 = 0.0f,
              m10 = 0.0f,
              m11 = 0.0f,
          };
        }
    }

    public static Vector2 operator *(Matrix2x2 lhs, Vector2 v)
    {
        Vector2 vector2;
        vector2.x = (float)((double)lhs.m00 * (double)v.x + (double)lhs.m01 * (double)v.y);
        vector2.y = (float)((double)lhs.m10 * (double)v.x + (double)lhs.m11 * (double)v.y);
        return vector2;
    }

    //Ignor Y
    public static Vector3 operator *(Matrix2x2 lhs, Vector3 v)
    {
        Vector3 vector3;
        vector3.x = (float)((double)lhs.m00 * (double)v.x + (double)lhs.m01 * (double)v.z);
        vector3.z = (float)((double)lhs.m10 * (double)v.x + (double)lhs.m11 * (double)v.z);
        vector3.y = v.y;
        return vector3;
    }
}
