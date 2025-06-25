using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Playables;

public class FileManager
{
    private const string key = "secretKey";
    public static List<string> ReadTextFile(string filePath, bool includeBlankLines = true)
    {
        if (!filePath.StartsWith('/'))
        {
            filePath = FilePath.root + filePath;
        }
        
        List<string> lines = new List<string>();
        try
        {
            using (StreamReader sr = new StreamReader(filePath))
            {
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    if(includeBlankLines || !string.IsNullOrWhiteSpace(line))
                    {
                        lines.Add(line);
                    }
                }
            }
        }
        catch(FileNotFoundException ex)
        {
            Debug.LogError($"file not found: '{ex.FileName}'");
        }

        return lines;
    }

    public static List<string> ReadTextAsset(string filePath, bool includeBlankLines = true)
    {
        TextAsset asset = Resources.Load<TextAsset>(filePath);
        if(asset == null)
        {
            Debug.LogError($"asset not found: '{filePath}'");
            return null;
        }

        return ReadTextAsset(asset, includeBlankLines);
    }
    public static List<string> ReadTextAsset(TextAsset asset, bool includeBlankLines = true)
    {
        List<string> lines = new List<string>();    
        using (StringReader sr  = new StringReader(asset.text))
        {
            while (sr.Peek() > -1)
            {
                string line = sr.ReadLine();
                if (includeBlankLines || !string.IsNullOrWhiteSpace(line))
                {
                    lines.Add(line);
                }
            }
        }
        return lines;
    }

    public static bool TryCreateDirectoryFromPath(string path)
    {
        if(Directory.Exists(path) || File.Exists(path))
        {
            return true;
        }

        if(path.Contains("."))
        {
            path = Path.GetDirectoryName(path);
            if(Directory.Exists(path) )
            {
                return true;
            }
        }

        if(path == string.Empty)
        {
            return false;
        }

        try
        {
            Directory.CreateDirectory(path);
            return true;
        }
        catch(System.Exception e)
        {
            Debug.LogError($"can not create directory {e}");
            return false;
        }
    }

    public static void Save(string filePath, string JSONData, bool encrypt = false)
    {
        if (!TryCreateDirectoryFromPath(filePath))
        {
            Debug.LogError($"failed to save file '{filePath}'");
            return; 
        }

        if(encrypt)
        {
            byte[] dataBytes = Encoding.UTF8.GetBytes(JSONData);
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] encryptedBytes = XOR(dataBytes, keyBytes);
        }
        else
        {
            StreamWriter sw = new StreamWriter(filePath);
            sw.Write(JSONData);
            sw.Close();
        }        

        Debug.Log($"save data to file '{filePath}'");
    }

    public static T Load<T>(string filePath, bool encrypt = false)
    {
        if(File.Exists(filePath))
        {
            if (encrypt)
            {
                byte[] encryptedBytes = File.ReadAllBytes(filePath);
                byte[] keyBytes = Encoding.UTF8.GetBytes(key);
                byte[] decryptedBytes = XOR(encryptedBytes, keyBytes);

                string decryptedString = Encoding.UTF8.GetString(decryptedBytes);

                return JsonUtility.FromJson<T>(decryptedString);
            }
            else
            {
                string JSONData = File.ReadAllLines(filePath)[0];
                return JsonUtility.FromJson<T>(JSONData);
            }           
        }
        else
        {
            Debug.LogError($"file does not exist '{filePath}'");
            return default(T);
        }
    }   

    private static byte[] XOR(byte[] input, byte[] key)
    {
        byte[] output = new byte[input.Length];

        for(int i = 0; i < input.Length; i++)
        {
            output[i] = (byte)(input[i] ^ key[i % key.Length]);
        }

        return output;
    }
}
