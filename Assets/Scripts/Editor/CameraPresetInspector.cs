using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CameraPresets))]
public class CameraPresetInspector : Editor
{
    private CameraPresets _preset;
    private string[] _projection = { "Perspective", "Ortographic" };

    private void OnEnable()
    {
        _preset = (CameraPresets)Selection.activeObject;
    }

    public override void OnInspectorGUI()
    {
        _preset.presetName = EditorGUILayout.TextField("Preset name: ", _preset.presetName);
        _preset.background = EditorGUILayout.ColorField("Background Color: ", _preset.background);
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Projection");
        _preset.selectedProjection = EditorGUILayout.Popup(_preset.selectedProjection, _projection);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("FieldOfView");
        _preset.fieldOfView = EditorGUILayout.Slider(_preset.fieldOfView, 0, 179);
        GUILayout.EndHorizontal();
        _preset.nearClippingPlane = EditorGUILayout.FloatField("Near Clipping Plane:", _preset.nearClippingPlane);
        _preset.farClippingPlane = EditorGUILayout.FloatField("Far Clipping Plane:", _preset.farClippingPlane);
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Viewport Rect");
        _preset.viewportRectX = EditorGUILayout.FloatField("Rect X:", _preset.viewportRectX);
        _preset.viewportRectY = EditorGUILayout.FloatField("Rect Y:", _preset.viewportRectY);
        _preset.viewportRectW = EditorGUILayout.FloatField("Rect W:", _preset.viewportRectW);
        _preset.viewportRectH = EditorGUILayout.FloatField("Rect H:", _preset.viewportRectH);
        EditorGUILayout.Space();
        _preset.depth = EditorGUILayout.FloatField("Depth:", _preset.depth);
        _preset.occlusionCulling = EditorGUILayout.Toggle("Occlusion Culling: ", _preset.occlusionCulling);
        _preset.allowDynamicResolution = EditorGUILayout.Toggle("Allow Dynamic Resolution: ", _preset.allowDynamicResolution);

        
        bool save = GUILayout.Button("Save Preset");
        EditorGUILayout.LabelField("Save preset changes and write them on disk.");
        if (save)
            SavePreset();
    }

    //Guardo los cambios hechos en el archivo y lo escribo en el disco.
    void SavePreset()
    {
        EditorUtility.SetDirty(_preset);
        AssetDatabase.SaveAssets();
        EditorGUILayout.HelpBox("File saved.", MessageType.Error);
        Debug.Log("asd");
    }
}
