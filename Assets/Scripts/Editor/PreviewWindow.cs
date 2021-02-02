using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PreviewWindow : EditorWindow
{
    public Camera cam;
    private RenderTexture _rt;
    bool _resize = true;
    //[MenuItem("Custom/Preview Window")]
    public static void Open()
    {
        GetWindow<PreviewWindow>().Show();
    }
    private void OnEnable()
    {
        minSize = new Vector2(0, 0);
        maxSize = new Vector2(1920, 1080);
    }


    private void OnGUI()
    {
        if (_resize)
        {
            position = new Rect(position.x, position.y, cam.targetTexture.width, cam.targetTexture.height);
            _resize = false;
        }

        var rt = RenderTexture.active;
        
        //El Render Texture de la cámara seleccionada se vuelve el Render Texture activo.
        RenderTexture.active = cam.targetTexture;
        cam.Render();

        //Creo una textura en la que voy a guardar lo que renderice la cámara.
        Texture2D texture = new Texture2D(cam.targetTexture.width, cam.targetTexture.height, TextureFormat.ARGB32, false);

        //Leo los píxeles que renderizó la cámara y los aplico en la textura.
        texture.ReadPixels(new Rect(0, 0, cam.targetTexture.width, cam.targetTexture.height), 0, 0);
        texture.Apply();
        RenderTexture.active = rt;

        //Dibujo en la ventana la textura con los píxeles ya grabados.
        EditorGUI.DrawPreviewTexture(new Rect(0, 0, position.width, position.height), texture);
    }
}
