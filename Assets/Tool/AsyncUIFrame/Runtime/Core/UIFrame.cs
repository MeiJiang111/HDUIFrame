using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using Async.Extensions;

namespace Async.UIFramework 
{
    [RequireComponent(typeof(RectTransform))]
    [DisallowMultipleComponent]
    public class UIFrame : MonoSingleton<UIFrame>
    {
        /// <summary>
        /// UI����
        /// </summary>
        private static readonly Dictionary<Type, GameObject> instances = new Dictionary<Type, GameObject>();

        /// <summary>
        /// UI��弯��
        /// </summary>
        private static readonly Stack<(Type type, UIData data)> panelStack = new Stack<(Type, UIData)>();

        /// <summary>
        /// UI�㼶
        /// </summary>
        private static readonly Dictionary<UILayer, RectTransform> uiLayers = new Dictionary<UILayer, RectTransform>();

        private static HashSet<UITimer> timers = new HashSet<UITimer>();
        private static HashSet<UITimer> timerRemoveSet = new HashSet<UITimer>();
        private static RectTransform layerTransform;

        [SerializeField] private RectTransform layers;
        [SerializeField] private Canvas canvas;

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

            Debug.Log("ddd -- UIFrame - Awake");

            if (canvas == null)
            {
                throw new Exception("UIFrame��ʼ��ʧ�ܣ�������Canvas");
            }

            if (canvas.worldCamera == null) 
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
            layerTransform.anchorMin = Vector2.zero;
            layerTransform.anchorMax = Vector2.one;
            layerTransform.offsetMin = Vector2.zero;
            layerTransform.offsetMax = Vector2.zero;
          
            //DontDestroyOnLoad(gameObject);
            AutoBindUITimer.Enable();
            AutoBindUGUIButtonEvent.Enable();
        }

        private void Update()
        {
            foreach (var item in timers)
            {
                item.Update();

                if (item.IsCancel)
                {
                    timerRemoveSet.Add(item);
                }
            }

            foreach (var item in timerRemoveSet)
            {
                timers.Remove(item);
            }
        }

        private void OnDestroy()
        {
            AutoBindUITimer.Disable();
            AutoBindUGUIButtonEvent.Disable();
        }


        // ---------------------------------- UI�¼� ----------------------------------
        #region �¼�
        /// <summary>
        /// ��ס��ʼʱ�������¼�
        /// </summary>
        public static event Action OnStuckStart;

        /// <summary>
        /// ��ס����ʱ�������¼�
        /// </summary>
        public static event Action OnStuckEnd;

        /// <summary>
        /// ��Դ����
        /// </summary>
        public static event Func<Type, UniTask<GameObject>> OnAssetRequest;

        /// <summary>
        /// ��Դ�ͷ�
        /// </summary>
        public static event Action<Type> OnAssetRelease;

        /// <summary>
        /// UI����ʱ����
        /// </summary>
        public static event Action<UIBase> OnCreate;

        /// <summary>
        /// UIˢ��ʱ����
        /// </summary>
        public static event Action<UIBase> OnRefresh;

        /// <summary>
        /// UI���¼�ʱ����
        /// </summary>
        public static event Action<UIBase> OnBind;

        /// <summary>
        /// UI����¼�ʱ����
        /// </summary>
        public static event Action<UIBase> OnUnbind;

        /// <summary>
        /// UI��ʾʱ����
        /// </summary>
        public static event Action<UIBase> OnShow;

        /// <summary>
        /// UI����ʱ����
        /// </summary>
        public static event Action<UIBase> OnHide;

        /// <summary>
        /// UI����ʱ����
        /// </summary>
        public static event Action<UIBase> OnDied;
        #endregion


        // ---------------------------------- UI��ʾ ----------------------------------
        #region ��ʾ
        /// <summary>
        /// ��ʾUI
        /// </summary>
        public static UniTask<UIBase> Show(UIBase ui, UIData data = null)
        {
            if (GetLayer(ui) != null && ui.Parent != null) 
            {
                throw new Exception("��UI����ʹ��UILayer����");
            }

            return ShowAsync(ui, data);
        }

        /// <summary>
        /// ��ʾPanel��Window
        /// </summary>
        public static UniTask<T> Show<T>(UIData data = null) where T : UIBase
        {
            return ShowAsync<T>(data);
        }

        /// <summary>
        /// ��ʾPanel��Window
        /// </summary>
        public static UniTask<UIBase> Show(Type type, UIData data = null)
        {
            if (GetLayer(type) == null) 
            {
                throw new Exception("��ʹ��[UILayer]�������࣬��ʾ��UI��ʹ��Show(UIBase ui)");
            }
        
            return ShowAsync(type, data);
        }
        #endregion


        // ---------------------------------- UI���� ----------------------------------
        #region ����
        /// <summary>
        /// ����Panel
        /// </summary>
        public static UniTask Hide(bool forceDestroy = false)
        {
            return HideAsync(forceDestroy);
        }

        /// <summary>
        /// ����Panel��Window
        /// </summary>
        public static UniTask Hide<T>(bool forceDestroy = false)
        {
            return Hide(typeof(T), forceDestroy);
        }

        /// <summary>
        /// ����Panel��Window
        /// </summary>
        public static UniTask Hide(Type type, bool forceDestroy = false)
        {
            if (GetLayer(type) is PanelLayer)
            {
                if (CurrentPanel != null && CurrentPanel.GetType() == type) 
                {
                    return Hide();
                }
           
                throw new Exception(type.ToString() + "���ǵ�ǰ������ʾ��Panel����ʹ��UIFrame.Hide()�����ص�ǰPanel");
            }
            else if (GetLayer(type) != null)
            {
                if (instances.TryGetValue(type, out var instance))
                {
                    var uibase = instance.GetComponent<UIBase>();
                    var uibases = uibase.BreadthTraversal().ToArray();
                    DoUnbind(uibases);
                    DoHide(uibases);
                    instance.SetActive(false);

                    if (uibase.AutoDestroy || forceDestroy) 
                    {
                        ReleaseInstance(type);
                    }
                }

                return UniTask.CompletedTask;
            }

            throw new Exception("����UIʧ�ܣ���ʹ��[UILayer]�������࣬������UI��ʹ��UIFrame.Hide(UIBase ui)");
        }

        /// <summary>
        /// ����UI��forceDestroy����UI��Ч��
        /// </summary>
        public static UniTask Hide(UIBase ui, bool forceDestroy = false)
        {
            if (GetLayer(ui) == null)
            {
                if (!ui.gameObject.activeSelf) 
                {
                    return UniTask.CompletedTask;
                }
               
                var uibases = ui.BreadthTraversal().ToArray();
                DoUnbind(uibases);
                DoHide(uibases);
                ui.gameObject.SetActive(false);
                return UniTask.CompletedTask;
            }

            return Hide(ui.GetType(), forceDestroy);
        }
        #endregion


        // ---------------------------------- UI��� ----------------------------------
        #region ���
        /// <summary>
        /// ����Ѿ�ʵ������UI
        /// </summary>
        public static UIBase Get(Type type)
        {
            if (instances.TryGetValue(type, out var instance))
            {
                return instance.GetComponent<UIBase>();
            }
            return null;
        }

        /// <summary>
        /// ����Ѿ�ʵ������UI
        /// </summary>
        public static UIBase Get<T>()
        {
            return Get(typeof(T));
        }

        /// <summary>
        /// ����Ѿ�ʵ������UI
        /// </summary>
        public static bool TryGet<T>(out UIBase ui)
        {
            ui = Get<T>();
            return ui != null;
        }

        /// <summary>
        /// ����Ѿ�ʵ������UI
        /// </summary>
        public static bool TryGet(Type type, out UIBase ui)
        {
            ui = Get(type);
            return ui != null;
        }

        /// <summary>
        /// ��������Ѿ�ʵ������UI
        /// </summary>
        public static IEnumerable<UIBase> GetAll(Func<Type, bool> predicate = null)
        {
            foreach (var item in instances)
            {
                if (predicate != null && !predicate.Invoke(item.Key)) 
                {
                    continue;
                }
           
                yield return item.Value.GetComponent<UIBase>();
            }
        }

        /// <summary>
        /// ���UILayer
        /// </summary>
        public static UILayer GetLayer(Type type)
        {
            if (type == null) 
            {
                return null;
            }
           
            var layer = type.GetCustomAttributes(typeof(UILayer), true).FirstOrDefault() as UILayer;
            return layer;
        }

        /// <summary>
        /// ���UILayer
        /// </summary>
        public static UILayer GetLayer(UIBase ui)
        {
            return GetLayer(ui.GetType());
        }

        /// <summary>
        /// ���UI��RectTransform
        /// </summary>
        public static RectTransform GetLayerTransform(Type type)
        {
            var layer = GetLayer(type);
            uiLayers.TryGetValue(layer, out var result);
            return result;
        }
        #endregion


        // ---------------------------------- UIˢ�� ----------------------------------
        #region ˢ��
        /// <summary>
        /// ˢ��UI��dataΪnullʱ������֮ǰ��dataˢ��
        /// </summary>
        public static UniTask Refresh<T>(UIData data = null)
        {
            return Refresh(typeof(T), data);
        }

        /// <summary>
        /// ˢ��UI��dataΪnullʱ����֮ǰ��dataˢ��
        /// </summary>
        public static UniTask Refresh(Type type, UIData data = null)
        {
            if (type != null && instances.TryGetValue(type, out var instance))
            {
                return Refresh(instance.GetComponent<UIBase>(), data);
            }
            return UniTask.CompletedTask;
        }

        /// <summary>
        /// ˢ��UI��dataΪnullʱ����֮ǰ��dataˢ��
        /// </summary>
        public static UniTask Refresh(UIBase ui, UIData data = null)
        {
            if (!ui.gameObject.activeInHierarchy) 
            {
                return UniTask.CompletedTask;
            }
           
            var uibases = ui.BreadthTraversal().ToArray();
            if (data != null) 
            {
                TrySetData(ui, data);
            }
           
            if (panelStack.Count > 0 && GetLayer(ui) is PanelLayer)
            {
                var (type, _) = panelStack.Pop();
                panelStack.Push((type, data));
            }
            return DoRefresh(uibases);
        }

        /// <summary>
        /// ˢ������UI
        /// </summary>
        public static async UniTask RefreshAll(Func<Type, bool> predicate = null)
        {
            foreach (var item in instances)
            {
                if (predicate != null && !predicate.Invoke(item.Key)) 
                {
                    continue;
                } 
                await Refresh(item.Value.GetComponent<UIBase>());
            }
        }
        #endregion


        // ---------------------------------- other core ----------------------------------
        #region [other core]
        /// <summary>
        /// ����UI GameObject
        /// </summary>
        public static UniTask<GameObject> Instantiate(GameObject prefab, Transform parent = null, UIData data = null)
        {
            return InstantiateAsync(prefab, parent, data);
        }

        /// <summary>
        /// ����UI GameObject
        /// </summary>
        public static void Destroy(GameObject instance)
        {
            var uibases = instance.transform.BreadthTraversal()
                .Where(item => item.GetComponent<UIBase>() != null)
                .Select(item => item.GetComponent<UIBase>())
                .ToArray();

            var parentUI = GetParent(uibases.FirstOrDefault());
            foreach (var item in uibases)
            {
                if (parentUI == null) 
                {
                    break;
                }

                if (GetParent(item) != parentUI) 
                {
                    break;
                }
                parentUI.Children.Remove(item);
            }

            foreach (var item in uibases)
            {
                try
                {
                    OnDied?.Invoke(item);
                    item.InnerOnDied();
                    item.CancelAllTimer();
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            }

            GameObject.Destroy(instance);
        }

        /// <summary>
        /// ��������UI GameObject
        /// </summary>
        public static void DestroyImmediate(GameObject instance)
        {
            var uibases = instance.transform.BreadthTraversal()
               .Where(item => item.GetComponent<UIBase>() != null)
               .Select(item => item.GetComponent<UIBase>())
               .ToArray();
            var parentUI = GetParent(uibases.FirstOrDefault());

            foreach (var item in uibases)
            {
                if (parentUI == null) 
                {
                    break;
                }

                if (GetParent(item) != parentUI) 
                {
                    break;
                }
          
                parentUI.Children.Remove(item);
            }

            foreach (var item in uibases)
            {
                try
                {
                    OnDied?.Invoke(item);
                    item.InnerOnDied();
                    item.CancelAllTimer();
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            }
            GameObject.DestroyImmediate(instance);
        }

        /// <summary>
        /// ǿ���ͷ��Ѿ��رյ�UI����ʹUI��AutoDestroyΪfalse����Ȼ�ͷŸ���Դ
        /// </summary>
        public static void Release()
        {
            var keys = new List<Type>();
            foreach (var item in instances)
            {
                if (item.Value != null && !item.Value.activeInHierarchy)
                {
                    UIFrame.Destroy(item.Value);
                    OnAssetRelease?.Invoke(item.Key);
                    keys.Add(item.Key);
                }
            }

            foreach (var item in keys)
            {
                instances.Remove(item);
            }
        }

        /// <summary>
        /// ������ʱ��
        /// </summary>
        /// <param name="delay">�ӳٶ������ִ��callback</param>
        /// <param name="callback">�ӳ�ִ�еķ���</param>
        /// <param name="isLoop">�Ƿ���ѭ����ʱ��</param>
        public static UITimer CreateTimer(float delay, Action callback, bool isLoop = false)
        {
            if (delay <= 0) throw new Exception("delay�������0");
            var timer = new UITimer(delay, callback, isLoop);
            timers.Add(timer);
            return timer;
        }

        private static async UniTask<GameObject> RequestInstance(Type type, UIData data)
        {
            if (type == null) throw new NullReferenceException();

            if (instances.TryGetValue(type, out var instance))
            {
                TrySetData(instance.GetComponent<UIBase>(), data);
                return instance;
            }

            GameObject refInstance = null;
            if (OnAssetRelease != null)
            {
                refInstance = await OnAssetRequest.Invoke(type);
            }

            var uibase = refInstance.GetComponent<UIBase>();
            if (uibase == null) 
            {
                throw new Exception("Ԥ����û�й��ؼ̳���UIBase�Ľű�");
            } 

            var parent = GetOrCreateLayerTransform(type);
            instance = await UIFrame.Instantiate(refInstance, parent, data);
            instances[type] = instance;
            return instance;
        }

        private static void ReleaseInstance(Type type)
        {
            if (type == null) return;

            if (instances.TryGetValue(type, out var instance))
            {
                var root = instance.GetComponent<UIBase>();
                UIFrame.Destroy(instance);
                OnAssetRelease?.Invoke(type);
                instances.Remove(type);
            }
        }

        private static async UniTask DoRefresh(IList<UIBase> uibases)
        {
            if (uibases == null) 
            {
                return;
            }
           
            for (int i = 0; i < uibases.Count; ++i)
            {
                if (uibases[i] == null) 
                {
                    continue;
                }
                
                if (i == 0 || uibases[i].gameObject.activeSelf)
                {
                    try
                    {
                        OnRefresh?.Invoke(uibases[i]);
                        await uibases[i].InnerOnRefresh();
                    }
                    catch (Exception ex)
                    {
                        Debug.LogException(ex);
                    }
                }
            }
        }

        private static void DoBind(IList<UIBase> uibases)
        {
            if (uibases == null) 
            {
                return;
            }
            
            for (int i = 0; i < uibases.Count; ++i)
            {
                if (uibases[i] == null)
                {
                    continue;
                }
               
                if (i == 0 || uibases[i].gameObject.activeSelf)
                {
                    try
                    {
                        OnBind?.Invoke(uibases[i]);
                        uibases[i].InnerOnBind();

                    }
                    catch (Exception ex)
                    {
                        Debug.LogException(ex);
                    }
                }
            }
        }

        private static void DoUnbind(IList<UIBase> uibases)
        {
            if (uibases == null) 
            {
                return;
            }
            
            for (int i = uibases.Count - 1; i >= 0; --i)
            {
                if (uibases[i] == null) 
                {
                    continue;
                }
                
                if (i == 0 || uibases[i].gameObject.activeSelf)
                {
                    try
                    {
                        OnUnbind?.Invoke(uibases[i]);
                        uibases[i].InnerOnUnbind();
                    }
                    catch (Exception ex)
                    {
                        Debug.LogException(ex);
                    }
                }
            }
        }

        private static void DoShow(IList<UIBase> uibases)
        {
            if (uibases == null) 
            {
                return;
            }
            
            for (int i = 0; i < uibases.Count; ++i)
            {
                if (uibases[i] == null) 
                {
                    continue;
                }
                
                if (i == 0 || uibases[i].gameObject.activeSelf)
                {
                    try
                    {
                        OnShow?.Invoke(uibases[i]);
                        uibases[i].InnerOnShow();
                    }
                    catch (Exception ex)
                    {
                        Debug.LogException(ex);
                    }
                }
            }
        }

        private static void DoHide(IList<UIBase> uibases)
        {
            if (uibases == null) 
            {
                return;
            }
           
            for (int i = uibases.Count - 1; i >= 0; --i)
            {
                if (uibases[i] == null) 
                {
                    continue;
                }
                
                if (i == 0 || uibases[i].gameObject.activeSelf)
                {
                    try
                    {
                        OnHide?.Invoke(uibases[i]);
                        uibases[i].InnerOnHide();
                    }
                    catch (Exception ex)
                    {
                        Debug.LogException(ex);
                    }
                }
            }
        }

        private static async UniTask<GameObject> InstantiateAsync(GameObject prefab, Transform parent, UIData data)
        {
            bool refActiveSelf = prefab.activeSelf;
            prefab.SetActive(false);
            var instance = GameObject.Instantiate(prefab, parent);
            prefab.SetActive(refActiveSelf);
            var uibase = instance.GetComponent<UIBase>();
            var uibases = instance.transform.BreadthTraversal()
                .Where(item => item.GetComponent<UIBase>() != null)
                .Select(item => item.GetComponent<UIBase>())
                .ToArray();

            TrySetData(instance.GetComponent<UIBase>(), data);
            foreach (var item in uibases)
            {
                item.Children.Clear();
            }

            foreach (var item in uibases)
            {
                var parentUI = GetParent(item);
                if (parentUI == null) 
                {
                    continue;
                }

                parentUI.Children.Add(item);
                item.Parent = parentUI;
            }

            foreach (var item in uibases)
            {
                try
                {
                    OnCreate?.Invoke(item);
                    await item.InnerOnCreate();
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            }
            if (GetLayer(uibase) == null)
            {
                await DoRefresh(uibases);
                instance.SetActive(true);
                DoBind(uibases);
                DoShow(uibases);
            }
            return instance;
        }

        private static async UniTask<UIBase> ShowAsync(UIBase ui, UIData data = null)
        {
            try
            {
                if (GetLayer(ui) == null)
                {
                    if (ui.gameObject.activeSelf) 
                    {
                        return ui;
                    }
                   
                    TrySetData(ui, data);
                    var timeout = new CancellationTokenSource();
                    bool isStuck = false;
                    UniTask.Delay(TimeSpan.FromSeconds(StuckTime)).GetAwaiter().OnCompleted(() =>
                    {
                        if (timeout.IsCancellationRequested) return;

                        OnStuckStart?.Invoke();
                        isStuck = true;
                    });

                    var parentUIBases = ui.Parent.BreadthTraversal().ToArray();
                    DoUnbind(parentUIBases);
                    var uibases = ui.BreadthTraversal().ToArray();
                    await DoRefresh(uibases);
                    
                    ui.gameObject.SetActive(true);
                    if (ui.Parent != null)
                    {
                        DoBind(parentUIBases);
                    }
                    else
                    {
                        DoBind(uibases);
                    }
                    DoShow(uibases);
                    timeout.Cancel();

                    if (isStuck) OnStuckEnd?.Invoke();

                    return ui;
                }
                return await Show(ui.GetType(), data);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                return null;
            }
        }

        private static async UniTask<T> ShowAsync<T>(UIData data = null) where T : UIBase
        {
            var result = await Show(typeof(T), data);
            return result as T;
        }

        private static async UniTask<UIBase> ShowAsync(Type type, UIData data = null)
        {
            try
            {
                var timeout = new CancellationTokenSource();
                UIBase result = null;
                bool isStuck = false;
                UniTask.Delay(TimeSpan.FromSeconds(StuckTime)).GetAwaiter().OnCompleted(() =>
                {
                    if (timeout.IsCancellationRequested) return;
                    OnStuckStart?.Invoke();
                    isStuck = true;
                });

                if (GetLayer(type) is PanelLayer)
                {
                    if (CurrentPanel != null && type == CurrentPanel.GetType()) return CurrentPanel;

                    UIBase[] currentUIBases = null;
                    if (CurrentPanel != null)
                    {
                        currentUIBases = CurrentPanel.BreadthTraversal().ToArray();
                        DoUnbind(currentUIBases);
                    }

                    var instance = await RequestInstance(type, data);
                    var uibases = instance.GetComponent<UIBase>().BreadthTraversal().ToArray();
                    if (data != null && CurrentPanel != null)
                    {
                        data.Sender = CurrentPanel.GetType();
                    }
                    await DoRefresh(uibases);
                    if (CurrentPanel != null)
                    {
                        DoHide(currentUIBases);
                        CurrentPanel.gameObject.SetActive(false);
                        if (CurrentPanel.AutoDestroy) ReleaseInstance(CurrentPanel.GetType());
                    }
                    instance.SetActive(true);
                    panelStack.Push((type, data));
                    DoBind(uibases);
                    DoShow(uibases);
                    result = instance.GetComponent<UIBase>();
                }
                else if (GetLayer(type) != null)
                {
                    var instance = await RequestInstance(type, data);
                    var uibases = instance.GetComponent<UIBase>().BreadthTraversal().ToArray();

                    if (data != null && CurrentPanel != null)
                    {
                        data.Sender = CurrentPanel.GetType();
                    }
                    await DoRefresh(uibases);
                    instance.SetActive(true);
                    instance.transform.SetAsLastSibling();
                    DoBind(uibases);
                    DoShow(uibases);
                    result = instance.GetComponent<UIBase>();
                }
                timeout.Cancel();
                if (isStuck) OnStuckEnd?.Invoke();
                return result;
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                return null;
            }
        }

        private static async UniTask HideAsync(bool forceDestroy)
        {
            try
            {
                var timeout = new CancellationTokenSource();

                bool isStuck = false;
                UniTask.Delay(TimeSpan.FromSeconds(StuckTime)).GetAwaiter().OnCompleted(() =>
                {
                    if (timeout.IsCancellationRequested) return;
                    OnStuckStart?.Invoke();
                    isStuck = true;
                });

                if (CurrentPanel == null)
                {
                    timeout.Cancel();
                    return;
                }

                var currentPanel = CurrentPanel;
                var currentUIBases = currentPanel.BreadthTraversal().ToArray();
                panelStack.Pop();
                DoUnbind(currentUIBases);
                if (panelStack.Count > 0)
                {
                    var data = panelStack.Peek().data;
                    if (data != null && currentPanel != null)
                    {
                        data.Sender = currentPanel.GetType();
                    }
                    var instance = await RequestInstance(panelStack.Peek().type, data);
                    var uibases = instance.GetComponent<UIBase>().BreadthTraversal().ToArray();
                    await DoRefresh(uibases);
                    currentPanel.gameObject.SetActive(false);
                    DoHide(currentUIBases);
                    if (currentPanel.AutoDestroy || forceDestroy) ReleaseInstance(currentPanel.GetType());
                    instance.SetActive(true);
                    instance.transform.SetAsLastSibling();
                    DoBind(uibases);
                    DoShow(uibases);
                }
                else
                {
                    currentPanel.gameObject.SetActive(false);
                    DoHide(currentUIBases);
                    if (currentPanel.AutoDestroy || forceDestroy) ReleaseInstance(currentPanel.GetType());
                }

                timeout.Cancel();
                if (isStuck) OnStuckEnd?.Invoke();
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        private static bool TrySetData(UIBase ui, UIData data)
        {
            if (ui == null) return false;
            var property = ui.GetType().GetProperty("Data", BindingFlags.Public | BindingFlags.Instance);
            if (property == null) return false;
            property.SetValue(ui, data);
            return true;
        }

        private static UIBase GetParent(UIBase ui)
        {
            if (ui == null) return null;

            var parent = ui.transform.parent;
            while (parent != null)
            {
                var uibase = parent.GetComponent<UIBase>();
                if (uibase != null) return uibase;
                parent = parent.parent;
            }
            return null;
        }

        private static RectTransform GetOrCreateLayerTransform(Type type)
        {
            var layer = GetLayer(type);
            if (!uiLayers.TryGetValue(layer, out var result))
            {
                var layerObject = new GameObject(layer.GetName());
                layerObject.transform.SetParent(layerTransform);
                result = layerObject.AddComponent<RectTransform>();
                result.anchorMin = Vector2.zero;
                result.anchorMax = Vector2.one;
                result.offsetMin = Vector2.zero;
                result.offsetMax = Vector2.zero;
                result.localScale = Vector3.one;
                result.localPosition = Vector3.zero;
                uiLayers[layer] = result;
                int index = 0;

                foreach (var item in uiLayers.OrderBy(i => i.Key.GetOrder()))
                {
                    item.Value.SetSiblingIndex(++index);
                }
            }
            return result;
        }

        #endregion
    }
}

