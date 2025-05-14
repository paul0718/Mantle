using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;


public class SFXEnumGenerator : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("Tools/Generate SFX Enum")]
    public static void GenerateSFXEnum()
    {
        string enumName = "SFXNAME";
        string filePath = "Assets/Scripts/Utils/";
        string fileName = enumName + ".cs";

        string[] audioFilesMP3 = Directory.GetFiles("Assets/Resources/Audio/", "*.mp3", SearchOption.AllDirectories);
        string[] audioFilesWAV = Directory.GetFiles("Assets/Resources/Audio/", "*.wav", SearchOption.AllDirectories);
        string[] audioFiles = audioFilesMP3.Concat(audioFilesWAV).ToArray();

        StringBuilder enumBuilder = new StringBuilder();
        
        enumBuilder.AppendLine("public enum " + enumName);
        enumBuilder.AppendLine("{");

        foreach (var file in audioFiles)
        {
            string clipName = Path.GetFileNameWithoutExtension(file);

            string validEnumName = MakeValidEnumName(clipName);
            
            if (!string.IsNullOrEmpty(validEnumName))
            {
                enumBuilder.AppendLine($"\t{validEnumName},");
            }
        }

        enumBuilder.AppendLine("}");
        if (!Directory.Exists(filePath))
        {
            Directory.CreateDirectory(filePath);
        }

        File.WriteAllText(filePath + fileName, enumBuilder.ToString());

        AssetDatabase.Refresh();

        Debug.Log("Successfully generated SFXNAME");
    }
#endif
    public static string MakeValidEnumName(string clipName)
    {
        string validName = Regex.Replace(clipName, @"[^a-zA-Z0-9_]", "");
        validName = validName.Replace(" ", "_");

        if (char.IsDigit(validName[0]))
        {
            validName = "_" + validName;
        }

        return validName;
    }
}
