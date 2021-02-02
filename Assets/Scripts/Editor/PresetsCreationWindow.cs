using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class PresetsCreationWindow : EditorWindow
{
    private string _cameraPresetName;
    private string _screenshotPresetName;
    private string _defaultPathName;

    bool _showError;


    GUIStyle _importantStyle = new GUIStyle();
    private void OnEnable()
    {
        minSize = new Vector2(300, 300);
        maxSize = new Vector2(300, 300);
        _importantStyle.fontStyle = FontStyle.Bold;
    }

    [MenuItem("Screenshots/Presets Creator")]
    public static void Open()
    {
        GetWindow<PresetsCreationWindow>().Show();
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Camera Preset File Name", _importantStyle);
        _cameraPresetName = EditorGUILayout.TextField(_cameraPresetName);
        if (GUILayout.Button("Create Camera Preset"))
            CreateAsset(_cameraPresetName, 1);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Screenshot Preset File Name", _importantStyle);
        _screenshotPresetName = EditorGUILayout.TextField(_screenshotPresetName);
        if (GUILayout.Button("Create Screenshot Preset"))
            CreateAsset(_screenshotPresetName, 2);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Default Path File Name", _importantStyle);
        _defaultPathName = EditorGUILayout.TextField(_defaultPathName);
        if (GUILayout.Button("Create Default Path File"))
            CreateAsset(_defaultPathName, 3);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Created objects will be empty,");
        EditorGUILayout.LabelField("you have to edit them.");

        if (_showError)
            ShowError();
    }

    void CreateAsset(string file, int selection)
    {
        if (string.IsNullOrWhiteSpace(file))
        {
            _showError = true;
        }
        else
        {
            _showError = false;
            Object obj = new Object();
            switch (selection)
            {
                case 1:
                    obj = ScriptableObject.CreateInstance<CameraPresets>();
                    break;

                case 2:
                    obj = ScriptableObject.CreateInstance<ScreenshotsPresets>();
                    break;

                case 3:
                    obj = ScriptableObject.CreateInstance<DefaultPaths>();
                    break;
            }
            //Verifico si existe la carpeta, si no existe la creo y dentro de ella creo el asset, si existe creo el asset en la carpeta existente.
            if (AssetDatabase.IsValidFolder("Assets/Presets"))
            {

                string path = AssetDatabase.GenerateUniqueAssetPath("Assets/Presets/" + file + ".asset");

                AssetDatabase.CreateAsset(obj, path);
            }
            else
            {
                AssetDatabase.CreateFolder("Assets", "Presets");

                string path = "Assets/Presets/" + file + ".asset";
                AssetDatabase.CreateAsset(obj, path);
            }
        }
    }

    private void ShowError()
    {
        EditorGUILayout.HelpBox("Name or path can't be empty.", MessageType.Error);
    }
}
