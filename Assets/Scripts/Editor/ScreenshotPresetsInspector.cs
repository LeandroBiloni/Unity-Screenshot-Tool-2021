using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ScreenshotsPresets))]
public class ScreenshotPresetsInspector : Editor
{
    private ScreenshotsPresets _preset;
    private void OnEnable()
    {
        _preset = (ScreenshotsPresets)Selection.activeObject;
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        bool save = GUILayout.Button("Save Preset");
        EditorGUILayout.LabelField("Save preset changes and write them on disk.");
        if (save)
            SavePreset();
    }

    void SavePreset()
    {
        EditorUtility.SetDirty(_preset);
        AssetDatabase.SaveAssets();
        EditorGUILayout.HelpBox("File saved.", MessageType.Error);
        Debug.Log("asd");
    }
}
