using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Camera Preset", menuName = "Create Camera Preset")]
public class CameraPresets : ScriptableObject
{
    public string presetName;
    public Color background;
    public int selectedProjection;
    public float fieldOfView;
    public float nearClippingPlane;
    public float farClippingPlane;
    public float viewportRectX;
    public float viewportRectY;
    public float viewportRectW;
    public float viewportRectH;
    public float depth;
    public bool occlusionCulling;
    public bool allowDynamicResolution;
}
