using System.Collections;
using System.Collections.Generic;
using TMPro;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class LoadUISettings : MonoBehaviour
{
    public UnityEngine.UI.Slider volume;
    public UnityEngine.UI.Slider cammeraSensitivity;
    public TMP_InputField forwardInput;
    public TMP_InputField backwardInput;
    public TMP_InputField leftInput;
    public TMP_InputField rigthInput;

    private OptionManager optionManager;
    // Start is called before the first frame update
    void Start()
    {
        optionManager = OptionManager.Instance;
        volume.value = optionManager.gameOptions.Volume;
        cammeraSensitivity.value = optionManager.gameOptions.CameraSensitivity;
        forwardInput.text = optionManager.gameOptions.Forwards.ToString();
        backwardInput.text = optionManager.gameOptions.Backwards.ToString();
        leftInput.text = optionManager.gameOptions.Left.ToString();
        rigthInput.text = optionManager.gameOptions.Rigth.ToString();
    }
    public void SaveInputs()
    {
        OptionModel model = new OptionModel();
        model.Forwards = forwardInput.text;
        model.Backwards = backwardInput.text;
        model.Left = leftInput.text;
        model.Rigth = rigthInput.text;
        model.Volume = volume.value;
        model.CameraSensitivity = cammeraSensitivity.value;
        optionManager.UpdateSettings(model);
        SceneManager.LoadScene(0);

    }
    public void OnTextChanged(TMP_InputField field)
    {
        if (field.text.Length > 1)
            field.text = field.text.Substring(0, 1);
        field.text = field.text.ToUpper();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
