using UnityEngine;

public class FilePath
{
    private const string homeDirectorySymbol = "~/";

    public static readonly string root = $"{Application.dataPath}/gameData/";

    public static readonly string gameSaves = $"{runtimePath}Save Files/";

    //resources paths
    public static readonly string resources_font = "Fonts/";

    public static readonly string resources_graphics = "Graphics/";
    public static readonly string resources_backgroundImages = "BackGround/";
    public static readonly string resources_backgroundVideos = $"{resources_graphics}BG Videos/";
    public static readonly string resources_blendTextures = $"{resources_graphics}Transition Effects/";

    public static readonly string resources_audio = "Audio/";
    public static readonly string resources_sfx = $"{resources_audio}SFX/";
    public static readonly string resources_music = $"{resources_audio}Music/";
    public static readonly string resources_ambience = $"{resources_audio}Ambience/";

    public static readonly string resources_dialogueFiles = $"Dialogue Files/";

    public static string GetPathToResource(string defaultPath, string resourceName)
    {
        if (resourceName.StartsWith(homeDirectorySymbol))
        {
            return resourceName.Substring(homeDirectorySymbol.Length);
        }

        return defaultPath + resourceName;
    }

    public static string runtimePath
    {
        get
        {
            #if UNITY_EDITOR
                return "Assets/appdata/";
            #else
                return Application.persistentDataPath + "/appdata/";
            #endif
        }
    }
}
