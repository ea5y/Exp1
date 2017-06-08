using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using System;
using XDATA;
public static class IOHelper
{
    static string fileName { get { return "playerData_" + NetworkController.ServerValue.PlayerId + ".json"; } }
    static string folderName = "PlayerData";

    static string FileName { get { return Path.Combine(FolderName, fileName); } }
    static string FolderName { get { return Path.Combine(Application.persistentDataPath, folderName); } }

    /// <summary>
    /// If exists file
    /// </summary>
    /// <returns></returns>
    public static bool HasPlayerDataFile()
    {
        return Directory.Exists(FolderName) && File.Exists(FileName);
    }

    /// <summary>
    /// Read data from local 
    /// </summary>
    public static PlayerData ReadDataFromLocal()
    {
        PlayerData data;
        //If has folder
        if (!Directory.Exists(FolderName))
        {
            Directory.CreateDirectory(FolderName);
        }
        
        //If has file
        if (File.Exists(FileName))
        {
            //Read
            FileStream fs = new FileStream(FileName, FileMode.Open);
            StreamReader sr = new StreamReader(fs);
            var values = sr.ReadToEnd();
            //Decrypt
            values = RijndaelDecrypt(values, "rrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrr");
            //To Obj
            data = JsonUtility.FromJson<PlayerData>(values);
            
            return data;
        }
        return null;
    }

    /// <summary>
    /// Save Data to local
    /// </summary>
    /// <param name="data">Data obj, here only has PlayerData obj</param>
    public static void SaveDataToLocal(object data)
    {
        //If has folder
        if (!Directory.Exists(FolderName))
        {
            Directory.CreateDirectory(FolderName);
        }
        
        //To json
        string values = JsonUtility.ToJson(data);

        //Encrypt
        values = RijndaelEncrypt(values, "rrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrr");
        
        //Write
        FileStream file = new FileStream(FileName, FileMode.Create);
        StreamWriter sw = new StreamWriter(file);
        sw.Write(values);

        //Close
        sw.Close();
        file.Close();
        file.Dispose();
    }


    /// <summary>
    /// Rijndael加密算法
    /// </summary>
    /// <param name="pString">待加密的明文</param>
    /// <param name="pKey">密钥,长度可以为:64位(byte[8]),128位(byte[16]),192位(byte[24]),256位(byte[32])</param>
    /// <param name="iv">iv向量,长度为128（byte[16])</param>
    /// <returns></returns>
    private static string RijndaelEncrypt(string pString, string pKey)
    {
        //密钥
        byte[] keyArray = UTF8Encoding.UTF8.GetBytes(pKey);
        //待加密明文数组
        byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(pString);

        //Rijndael解密算法
        RijndaelManaged rDel = new RijndaelManaged();
        rDel.Key = keyArray;
        rDel.Mode = CipherMode.ECB;
        rDel.Padding = PaddingMode.PKCS7;
        ICryptoTransform cTransform = rDel.CreateEncryptor();

        //返回加密后的密文
        byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
        return Convert.ToBase64String(resultArray, 0, resultArray.Length);
    }

    /// <summary>
    /// ijndael解密算法
    /// </summary>
    /// <param name="pString">待解密的密文</param>
    /// <param name="pKey">密钥,长度可以为:64位(byte[8]),128位(byte[16]),192位(byte[24]),256位(byte[32])</param>
    /// <param name="iv">iv向量,长度为128（byte[16])</param>
    /// <returns></returns>
    private static String RijndaelDecrypt(string pString, string pKey)
    {
        //解密密钥
        byte[] keyArray = UTF8Encoding.UTF8.GetBytes(pKey);
        //待解密密文数组
        byte[] toEncryptArray = Convert.FromBase64String(pString);

        //Rijndael解密算法
        RijndaelManaged rDel = new RijndaelManaged();
        rDel.Key = keyArray;
        rDel.Mode = CipherMode.ECB;
        rDel.Padding = PaddingMode.PKCS7;
        ICryptoTransform cTransform = rDel.CreateDecryptor();

        //返回解密后的明文
        byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
        return UTF8Encoding.UTF8.GetString(resultArray);
    }
}

