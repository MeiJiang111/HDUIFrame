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
    /// UI����
    /// </summary>
    public static Canvas Canvas { get; private set; }

    /// <summary>
    /// UI���
    /// </summary>
    public static Camera Camera { get; private set; }

    /// <summary>
    /// ������UI�������ʱ�䣨��λ���룩ʱ�����Ϊ��ס
    /// </summary>
    public static float StuckTime = 1;

    /// <summary>
    /// ��ǰ��ʾ��Panel
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
            throw new Exception("UIFrame��ʼ��ʧ�ܣ�������Canvas");
        }

        if(canvas.worldCamera == null)
        {
            throw new Exception("UIFrame��ʼ��ʧ�ܣ����Canvas����worldCamera");
        }

        if (layers == null)
        {
            throw new Exception("UIFrame��ʼ��ʧ�ܣ�������layers");
        }

        Canvas = canvas;
        Camera = canvas.worldCamera;

        layerTransform = layers;
        layerTransform.anchoredPosition = Vector2.zero;
        layerTransform.localScale = Vector2.one;
        layerTransform.localRotation = Quaternion.identity;

    }
}
