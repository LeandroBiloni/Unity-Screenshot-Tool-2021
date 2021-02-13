using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Windows;

[CustomEditor(typeof(CameraScreenshot))]
public class ScreenshotInspector : Editor
{
    private CameraScreenshot _camScreenshot;
    
    //Tamaños
    private int _textureWidth;
    private int _textureHeigth;
    private bool _useCustomSize = true;

    //Presets
    private int _selectedPreset;
    private string[] _presets;

    //Nombre y path
    private bool _useCustomPath = true;
    private string _savePath;
    private string _fileName;
    private string[] _defaultPaths;
    private int _selectedPath;

    //Formato
    private string[] _formats = { "PNG", "JPG" };
    private int _selectedFormat;

    //Errores
    private bool _noRenderTextureError;
    private bool _noScreenshotPresetsError;
    private bool _noFileOrPathNameError;
    private bool _noDefaultPathsError;
    private bool _invalidSavePathError;

    GameObject _camObj;
    Vector3 _pos;
    GUIStyle _importantStyle = new GUIStyle();
    Camera _sceneCamera;
    private void OnEnable()
    {
        _importantStyle.fontStyle = FontStyle.Bold;
        _camObj = Selection.activeGameObject;
        _pos = _camObj.transform.position;
        _camScreenshot = _camObj.GetComponent<CameraScreenshot>();
        _camScreenshot.Initialize();
        if (_camScreenshot.presets.Count < 1)
        {
            _noScreenshotPresetsError = true;
        }
        else
        {
            _presets = new string[_camScreenshot.presets.Count];
            for (int i = 0; i < _camScreenshot.presets.Count; i++)
            {
                _presets[i] = _camScreenshot.presets[i].presetName;
            }
        }
        
        _textureWidth = _camScreenshot.renderTextureWidth;
        _textureHeigth = _camScreenshot.renderTextureHeight;

        //Busco la ubicación de los archivos tipo DefaultPath dentro de la carpeta Assets.
        var folder = AssetDatabase.FindAssets("t:DefaultPaths", null);

        //Si no hay ningún preset muestro un error.
        if (folder.Length < 1)
        {
            _noDefaultPathsError = true;
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
            _defaultPaths = new string[files.Length];
            for (int i = 0; i < files.Length; i++)
            {
                _defaultPaths[i] = ((DefaultPaths)AssetDatabase.LoadAssetAtPath(files[i], typeof(DefaultPaths))).screenshotPath;
            }
        }
        _sceneCamera = EditorWindow.GetWindow<SceneView>().camera;
    }


    //REVISAAARRRR
    private void OnSceneGUI()
    {
        if (_camObj.transform.position != _pos)
            _pos = _camObj.transform.position;
        GUIHandles();
    }

    public override void OnInspectorGUI()
    {
        //Decido si voy a configurar el tamaño de la captura o si voy a usar un preset
        _useCustomSize = EditorGUILayout.Toggle("Use Custom Sizes", _useCustomSize);
        if (_useCustomSize)
        {
            GUILayout.Label("Set your desired Width and Height");
            _textureWidth = EditorGUILayout.IntField("Render Texture Width", _textureWidth);
            _textureHeigth = EditorGUILayout.IntField("Render Texture Height", _textureHeigth);
            if (_textureHeigth <= 0)
                _textureHeigth = 1;
            if (_textureWidth <= 0)
                _textureWidth = 1;

            //Aplico los valores al Render Texture.
            if (GUILayout.Button("Apply Change"))
                ChangeRenderTextureValues(_textureHeigth, _textureWidth, _camScreenshot.gameObject.GetComponent<Camera>().fieldOfView);
        }
        else
        {
            if (_noScreenshotPresetsError)
            {
                NoScreenshotPresetsError();
            }
            else
            {
                GUILayout.BeginHorizontal();

                //Selecciono el preset que quiero usar
                EditorGUILayout.LabelField("Preset");
                _selectedPreset = EditorGUILayout.Popup(_selectedPreset, _presets);
                GUILayout.EndHorizontal();

                //Aplico los valores al Render Texture.
                if (GUILayout.Button("Apply Change"))
                    ChangeRenderTextureValues(_camScreenshot.presets[_selectedPreset].renderTextureHeight, _camScreenshot.presets[_selectedPreset].renderTextureWidth, _camScreenshot.presets[_selectedPreset].camFieldOfView);
            }
            
        }
        GUILayout.BeginHorizontal();

        //Muestro los formatos.
        EditorGUILayout.LabelField("Format");
        _selectedFormat = EditorGUILayout.Popup(_selectedFormat, _formats);
        GUILayout.EndHorizontal();
        
        //Nombre del archivo y path.
        _fileName = EditorGUILayout.TextField("File Name", _fileName);

        _useCustomPath = EditorGUILayout.Toggle("Use Custom Path", _useCustomPath);
        if (_useCustomPath)
            _savePath = EditorGUILayout.TextField("Save Path", _savePath);
        else
        {
            if (_noDefaultPathsError)
            {
                DefaultPathsError();
            }
            else
            {
                _noDefaultPathsError = false;
                // Selecciono el path que quiero usar
                EditorGUILayout.LabelField("Path");
                _selectedPath = EditorGUILayout.Popup(_selectedPath, _defaultPaths);
                _savePath = _defaultPaths[_selectedPath];
            }
            
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Preview will be available if render texture", _importantStyle);
        EditorGUILayout.LabelField("resolution is equal or less than screen", _importantStyle);
        EditorGUILayout.LabelField("resolution.", _importantStyle);

        //Abro la ventana de preview para ver que se va a capturar.
        if (_camScreenshot.cam.targetTexture.height <= Screen.currentResolution.height && _camScreenshot.cam.targetTexture.width <= Screen.currentResolution.width)
            if (GUILayout.Button("Preview"))
                OpenPreview();

        //Hago la captura.    
        if (GUILayout.Button("Screenshot"))
            TakeScreenshot();

        if (_noRenderTextureError)
            NoRenderTextureError();
        if (_noFileOrPathNameError)
            NoFileOrPathNameError();
        if (_invalidSavePathError)
            InvalidSavePathError();
    }

    void TakeScreenshot()
    {
        //Me fijo si hay Render Texture para mostrar errores.
        if (CheckRenderTexture())
            return;
        else
        {
            //Me fijo que el nombre y path no sean vacíos para mostrar errores.
            if (CheckNameAndPath())
                return;
            else
            {
                _noFileOrPathNameError = false;
                _noRenderTextureError = false;
                _invalidSavePathError = false;
                var rt = RenderTexture.active;

                //El Render Texture de la cámara seleccionada se vuelve el Render Texture activo.
                RenderTexture.active = _camScreenshot.cam.targetTexture;
                _camScreenshot.cam.Render();

                //Creo una textura en la que voy a guardar lo que renderice la cámara.
                Texture2D texture = new Texture2D(_camScreenshot.cam.targetTexture.width, _camScreenshot.cam.targetTexture.height, TextureFormat.ARGB32, false);

                //Leo los píxeles que renderizó la cámara y los aplico en la textura.
                texture.ReadPixels(new Rect(0, 0, _camScreenshot.cam.targetTexture.width, _camScreenshot.cam.targetTexture.height), 0, 0);
                texture.Apply();
                RenderTexture.active = rt;

                //Selecciono el formato en el cual se va a guardar el "screenshot".
                switch (_selectedFormat)
                {
                    case 0:
                        var png = ImageConversion.EncodeToPNG(texture);
                        File.WriteAllBytes(_savePath + "/" + _fileName + ".png", png);
                        break;

                    case 1:
                        var jpg = ImageConversion.EncodeToJPG(texture);
                        File.WriteAllBytes(_savePath + "/" + _fileName + ".jpg", jpg);
                        break;
                }
            }
        }
    }
    
    private void ChangeRenderTextureValues(int height, int width, float camFOV)
    {
        _camScreenshot.renderTexture.Release();
        _camScreenshot.renderTextureHeight = height;
        _camScreenshot.renderTextureWidth = width;
        _camScreenshot.renderTexture.height = height;
        _camScreenshot.renderTexture.width = width;
        _camScreenshot.cam.targetTexture = _camScreenshot.renderTexture;
        _camScreenshot.cam.fieldOfView = camFOV;
    }

    private bool CheckRenderTexture()
    {
        if (_camScreenshot.cam.targetTexture == null)
        {
            _noRenderTextureError = true;
            _noFileOrPathNameError = false;
            _invalidSavePathError = false;
            return true;
        }
        else return false;
    }

    private void NoRenderTextureError()
    {
        EditorGUILayout.HelpBox("There's no Render Texture attached to this camera.", MessageType.Error);
    }

    private bool CheckNameAndPath()
    {
        if (string.IsNullOrEmpty(_savePath) || string.IsNullOrEmpty(_fileName))
        {
            _noFileOrPathNameError = true;
            _noRenderTextureError = false;
            _invalidSavePathError = false;
            return true;
        }
        else if (Directory.Exists(_savePath))
        {
            return false;
        }
        else
        {
            _invalidSavePathError = true;
            _noFileOrPathNameError = false;
            _noRenderTextureError = false;
            return true;
        }
    }

    private void NoFileOrPathNameError()
    {
        EditorGUILayout.HelpBox("Name or path can't be empty.", MessageType.Error);
    }

    private void DefaultPathsError()
    {
        EditorGUILayout.HelpBox("There are no default paths found.", MessageType.Error);
    }

    private void InvalidSavePathError()
    {
        EditorGUILayout.HelpBox("Path is invalid or inexistent.", MessageType.Error);
    }

    private void NoScreenshotPresetsError()
    {
        EditorGUILayout.HelpBox("No presets found.", MessageType.Error);
    }

    void OpenPreview()
    {
        var window = EditorWindow.GetWindow<PreviewWindow>();
        window.cam = _camScreenshot.cam;
        window.BeginWindows();
    }

    private void GUIHandles()
    {
        Handles.BeginGUI();
        var rect = _sceneCamera.pixelRect;
        var p = Camera.current.WorldToScreenPoint(_pos);
        GUILayout.BeginArea(new Rect(p.x, rect.height - p.y, rect.width / 8, rect.height));
        Rect r = new Rect(p.x, rect.height - p.y, rect.width / 8, rect.height);
        if (GUILayout.Button("Take Screenshot"))
            TakeScreenshot();

        if (_noDefaultPathsError && _useCustomPath == false)
            DefaultPathsError();

        if (_noRenderTextureError)
            NoRenderTextureError();

        if (_noFileOrPathNameError)
            NoFileOrPathNameError();

        if (_invalidSavePathError)
            InvalidSavePathError();
        GUILayout.EndArea();
        Handles.EndGUI();
    }
}
