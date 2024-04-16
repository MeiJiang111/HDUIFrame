using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[RequireComponent(typeof(RectTransform))]
[DisallowMultipleComponent]
public class UIFrame : MonoBehaviour
{
    private static readonly Dictionary<Type, GameObject> instances = new Dictionary<Type, GameObject>();
    private static readonly Stack<(Type type, UIData data)> panelStack = new Stack<(Type, UIData)>();
    private static readonly Dictionary<UILayer, RectTransform> uiLayers = new Dictionary<UILayer, RectTransform>();
    private static HashSet<UITimer> timers = new HashSet<UITimer>();
    private static HashSet<UITimer> timerRemoveSet = new HashSet<UITimer>();
    private static RectTransform layerTransform;

    [SerializeField] 
    private RectTransform layers;

    [SerializeField] 
    private Canvas canvas;

    /// <summary>
    /// UI»­²¼
    /// </summary>
    public static Canvas Canvas { get; private set; }

    /// <summary>
    /// UIÏà»ú
    /// </summary>
    public static Camera Camera { get; private set; }

}
