using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

[Serializable]
public class OptionModel
{
    public float CameraSensitivity;
    public string Forwards;
    public string Backwards;
    public string Left;
    public string Rigth;
    public float Volume;

    public GameOptions ToGameOptions()
    {
        GameOptions options = new GameOptions();
        options.CameraSensitivity = CameraSensitivity;
        options.Volume = Volume;
        options.Rigth = ConvertStringToKeyCode(Rigth);
        options.Left = ConvertStringToKeyCode(Left);
        options.Forwards = ConvertStringToKeyCode(Forwards);
        options.Backwards = ConvertStringToKeyCode(Backwards);
        return options;
    }
    private KeyCode ConvertStringToKeyCode(string keyName)
    {
        try
        {
            KeyCode keyCode = (KeyCode)Enum.Parse(typeof(KeyCode), keyName);
            return keyCode;
        }
        catch (ArgumentException)
        {
            Debug.LogError("Invalid key name: " + keyName);
            return KeyCode.None;
        }
    }
}