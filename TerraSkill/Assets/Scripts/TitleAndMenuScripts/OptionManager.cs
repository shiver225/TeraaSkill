using Palmmedia.ReportGenerator.Core.Common;
using System.Text.Json;
using System.IO;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using static UnityEditor.Progress;

public class OptionManager : MonoBehaviour
{
    private readonly string SaveFileLocation = "Assets/Save Files/Options.json";
    public static OptionManager Instance { get; private set; }
    public GameOptions gameOptions { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            LoadSetting();
        }
        else
            Destroy(Instance);
    }
    public void CreateSetting()
    {
        if (!File.Exists(SaveFileLocation))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(SaveFileLocation));
            OptionModel options = new OptionModel()
            {
                CameraSensitivity = 1,
                Forwards = "W",
                Backwards = "S",
                Left = "A",
                Rigth = "D",
                Volume = 1
            };
            string content = JsonUtility.ToJson(options);
            
            File.WriteAllText(SaveFileLocation, content);
        }
    }
    public void LoadSetting()
    {
        if (!File.Exists(SaveFileLocation))
        {
            CreateSetting();
        }
        string content = File.ReadAllText(SaveFileLocation);
        OptionModel options = JsonUtility.FromJson<OptionModel>(content);
        gameOptions = options.ToGameOptions();
    }
    public void UpdateSettings(OptionModel options)
    {
        string content = JsonUtility.ToJson(options);
        File.WriteAllText(SaveFileLocation, content);
    }
}
