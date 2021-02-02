using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Screenshot Preset", menuName = "Create Screenshot Preset")]
public class ScreenshotsPresets : ScriptableObject
{
    public string presetName;
    public int renderTextureWidth;
    public int renderTextureHeight;
    public float camFieldOfView;
}
