using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CameraCreatorWindow : EditorWindow
{
    public string cameraName;
    public Vector3 cameraPosition;
    private bool _showError;
    public RenderTexture renderTexture;
    GUIStyle _labelStyle;
    FontStyle _font;
    Vector2 _size = new Vector2(400, 250);
    private void Awake()
    {
        _font = new FontStyle();
        _font = FontStyle.Bold;
        _labelStyle = new GUIStyle();
        _labelStyle.normal.textColor = Color.black;
        _labelStyle.fontStyle = _font;
    }
    [MenuItem("Screenshots/Camera Creator")]
    public static void Open()
    {
        GetWindow<CameraCreatorWindow>().Show();
    }

    private void OnGUI()
    {
        maxSize = _size;
        minSize = _size;

        CreateCamera();

        if (_showError)
            ShowError();
    }

    void CreateCamera()
    {
        EditorGUILayout.LabelField("Camera Name", _labelStyle);
        cameraName = EditorGUILayout.TextField(cameraName);

        EditorGUILayout.LabelField("Camera Position", _labelStyle);
        cameraPosition = EditorGUILayout.Vector3Field("", cameraPosition);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Render Texture", _labelStyle);
        EditorGUILayout.LabelField("If left empty a new Render Texture will be created.");

        EditorGUILayout.Space();
        renderTexture = (RenderTexture)EditorGUILayout.ObjectField(renderTexture, typeof(RenderTexture), false);

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        bool create = GUILayout.Button("Create Camera");
        if (create)
        {
            //Veo que el nombre no este vacío
            if (string.IsNullOrWhiteSpace(cameraName))
            {
                _showError = true;
            }
            else
            {
                _showError = false;

                //Creo la cámara en la posición asignada.
                GameObject cam = new GameObject(cameraName, typeof(Camera));
                cam.transform.position = cameraPosition;

                //Si no se agregó un Render Texture creo uno y se lo asigno a la cámara.
                if (renderTexture == null)
                {
                    renderTexture = new RenderTexture(Screen.currentResolution.width, Screen.currentResolution.height, 24);
                    renderTexture.name = cameraName + " DefaultRT";
                }
                cam.GetComponent<Camera>().targetTexture = renderTexture;
                cam.AddComponent<CameraScreenshot>();
            }
        }
    }

    private void ShowError()
    {
        EditorGUILayout.HelpBox("Name can't be empty.", MessageType.Error);
    }
}
