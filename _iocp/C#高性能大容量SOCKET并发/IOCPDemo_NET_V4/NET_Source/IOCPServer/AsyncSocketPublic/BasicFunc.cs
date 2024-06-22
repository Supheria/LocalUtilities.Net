using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Net;

class BasicFunc
{
    public static bool IsFileInUse(string fileName)
    {
        try
        {
            using var _ = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.None);
            return false;
        }
        catch
        {
            return true;
        }
    }

    public static string MD5String(string value)
    {
        System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
        byte[] data = Encoding.Default.GetBytes(value);
        byte[] md5Data = md5.ComputeHash(data);
        md5.Clear();
        string result = "";
        for (int i = 0; i < md5Data.Length; i++)
        {
            result += md5Data[i].ToString("x").PadLeft(2, '0');
        }
        return result;
    }
}
