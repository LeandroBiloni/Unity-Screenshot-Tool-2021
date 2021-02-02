using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Camera))]
public class CameraInspector : Editor
{
    private bool _usePreset = false;
    private List<CameraPresets> _presetsList;
    private string[] _presetsNames;
    private int _selectedPreset = 0;
    private bool _noPresetsError = false;
    
    private void OnEnable()
    {
        
        //Busco la ubicación de los archivos tipo CameraPresets dentro de la carpeta Assets.
        var folder = AssetDatabase.FindAssets("t:CameraPresets", null);
        //Si no hay ningún preset muestro un error.
        if (folder.Length < 1)
        {
            _noPresetsError = true;
        }
        else
        {
            //Consigo el path de cada preset.
            string[] files = new string[folder.Length];
            for (int i = 0; i < folder.Length; i++)
            {
                files[i] = AssetDatabase.GUIDToAssetPath(folder[i]);
            }

            //Lo agrego a una lista para poder acceder al que quiera.
            _presetsList = new List<CameraPresets>();
            for (int i = 0; i < files.Length; i++)
            {
                _presetsList.Add((CameraPresets)AssetDatabase.LoadAssetAtPath(files[i], typeof(CameraPresets)));
            }

            //Guardo el nombre para poder mostrarlo en el inspector.
            _presetsNames = new string[_presetsList.Count];
            for (int i = 0; i < _presetsList.Count; i++)
            {
                _presetsNames[i] = _presetsList[i].presetName;
            }
        }
        
    }

    public override void OnInspectorGUI()
    {
        if (_noPresetsError)
        {
            NoPresetsError();
        }
        else
        {
            //Decido si quiero usar un preset.
            _usePreset = EditorGUILayout.Toggle("Use Preset", _usePreset);
            if (_usePreset)
            {
                //Listo los presets que se hayan encontrado en la carpeta de Assets.
                EditorGUILayout.LabelField("Preset");
                _selectedPreset = EditorGUILayout.Popup(_selectedPreset, _presetsNames);

                //Aplico los valores del preset a la camara.
                if (GUILayout.Button("Apply Change"))
                    ChangeValues();
            }
        }
        DrawDefaultInspector();
    }

    void ChangeValues()
    {
        GameObject obj = Selection.activeGameObject;
        Camera cam = obj.GetComponent<Camera>();
        var preset = _presetsList[_selectedPreset];
        cam.backgroundColor = preset.background;
        if (preset.selectedProjection == 0)
            cam.orthographic = false;
        else cam.orthographic = true;
        cam.fieldOfView = preset.fieldOfView;
        cam.nearClipPlane = preset.nearClippingPlane;
        cam.farClipPlane = preset.farClippingPlane;
        cam.pixelRect = new Rect(preset.viewportRectX, preset.viewportRectY, preset.viewportRectW, preset.viewportRectH);
        cam.depth = preset.depth;
        cam.useOcclusionCulling = preset.occlusionCulling;
        cam.allowDynamicResolution = preset.allowDynamicResolution;
    }

    void NoPresetsError()
    {
        EditorGUILayout.HelpBox("No presets found.", MessageType.Error);
    }

    
}
