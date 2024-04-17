using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[RequireComponent(typeof(RectTransform))]
[DisallowMultipleComponent]
public class UIFrame : MonoSingleton<UIFrame>
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
    /// UI画布
    /// </summary>
    public static Canvas Canvas { get; private set; }

    /// <summary>
    /// UI相机
    /// </summary>
    public static Camera Camera { get; private set; }

    /// <summary>
    /// 当加载UI超过这个时间（单位：秒）时，检测为卡住
    /// </summary>
    public static float StuckTime = 1;

    /// <summary>
    /// 当前显示的Panel
    /// </summary>
    public static UIBase CurrentPanel
    {
        get
        {
            if (panelStack.Count <= 0) 
            {
                return null;
            }

            if (panelStack.Peek().type == null)
            {
                return null;
            }
          
            if (instances.TryGetValue(panelStack.Peek().type, out var instance))
            {
                return instance.GetComponent<UIBase>();
            }
            return null;
        }
    }

    protected override void Awake()
    {
        base.Awake();

        if (canvas == null)
        {
            throw new Exception("UIFrame初始化失败，请设置Canvas");
        }

        if(canvas.worldCamera == null)
        {
            throw new Exception("UIFrame初始化失败，请给Canvas设置worldCamera");
        }

        if (layers == null)
        {
            throw new Exception("UIFrame初始化失败，请设置layers");
        }

        Canvas = canvas;
        Camera = canvas.worldCamera;

        layerTransform = layers;
        layerTransform.anchoredPosition = Vector2.zero;
        layerTransform.localScale = Vector2.one;
        layerTransform.localRotation = Quaternion.identity;

    }
}
