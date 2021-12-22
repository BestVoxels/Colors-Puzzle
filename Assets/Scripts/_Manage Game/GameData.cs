using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class GameData : MonoBehaviour
{
    public static void SaveString(string data, string fileName)
    {
        string path = GetPath(fileName);

        BinaryFormatter binaryFormatter = new BinaryFormatter();

        using (FileStream fileStream = File.Open(path, FileMode.OpenOrCreate))
        {
            binaryFormatter.Serialize(fileStream, data);
        }
    }

    public static string LoadString(string fileName)
    {
        string path = GetPath(fileName);

        BinaryFormatter binaryFormatter = new BinaryFormatter();

        using (FileStream fileStream = File.Open(path, FileMode.Open))
        {
            return (string)binaryFormatter.Deserialize(fileStream);
        }
    }

    public static void SaveStringArray(string[] data, string fileName)
    {
        string path = GetPath(fileName);

        BinaryFormatter binaryFormatter = new BinaryFormatter();

        using (FileStream fileStream = File.Open(path, FileMode.OpenOrCreate))
        {
            binaryFormatter.Serialize(fileStream, data);
        }
    }

    public static string[] LoadStringArray(string fileName)
    {
        string path = GetPath(fileName);

        BinaryFormatter binaryFormatter = new BinaryFormatter();

        using (FileStream fileStream = File.Open(path, FileMode.Open))
        {
            return (string[])binaryFormatter.Deserialize(fileStream);
        }
    }

    public static void SaveBoolArray(bool[] data, string fileName)
    {
        string path = GetPath(fileName);

        BinaryFormatter binaryFormatter = new BinaryFormatter();

        using (FileStream fileStream = File.Open(path, FileMode.OpenOrCreate))
        {
            binaryFormatter.Serialize(fileStream, data);
        }
    }

    public static bool[] LoadBoolArray(string fileName)
    {
        string path = GetPath(fileName);

        BinaryFormatter binaryFormatter = new BinaryFormatter();

        using (FileStream fileStream = File.Open(path, FileMode.Open))
        {
            return (bool[])binaryFormatter.Deserialize(fileStream);
        }
    }

    public static bool IsFileExist(string fileName)
    {
        return File.Exists(GetPath(fileName));
    }

    private static string GetPath(string name)
    {
        return Path.Combine(Application.persistentDataPath, name + ".dat");
    }
}