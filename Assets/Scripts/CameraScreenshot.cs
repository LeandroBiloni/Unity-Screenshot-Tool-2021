using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CameraScreenshot : MonoBehaviour
{
    public Camera cam;
    public int renderTextureWidth;
    public int renderTextureHeight;
    public RenderTexture renderTexture;
    public List<ScreenshotsPresets> presets;

    public void Initialize()
    {
        presets = new List<ScreenshotsPresets>();
        cam = GetComponent<Camera>();
        renderTexture = cam.targetTexture;
        //Si la cámara no tiene un Render Texture le creo uno.
        if (renderTexture == null)
        {
            renderTexture = new RenderTexture(Screen.currentResolution.width, Screen.currentResolution.height, 24);
            renderTexture.name = "DefaultRT";
        }

        //Guardo los datos para poder mostrarlos desde el inspector.
        renderTextureHeight = renderTexture.height;
        renderTextureWidth = renderTexture.width;

        //Le asigno el Render Texture a la cámara.
        cam.targetTexture = renderTexture;

        //Busco todos los archivos del tipo ScreenshotsPresets.
        var folder = AssetDatabase.FindAssets("t:ScreenshotsPresets", null);

        //Guardo el path.
        string[] files = new string[folder.Length];
        for (int i = 0; i < folder.Length; i++)
        {
            files[i] = AssetDatabase.GUIDToAssetPath(folder[i]);
        }

        //Los agrego a la lista de presets para poder acceder a ellos.
        for (int i = 0; i < files.Length; i++)
        {
            presets.Add((ScreenshotsPresets)AssetDatabase.LoadAssetAtPath(files[i], typeof(ScreenshotsPresets)));
        }
    }
}
