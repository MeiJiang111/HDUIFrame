using UnityEngine;

namespace Async.UIFramework
{
    [RequireComponent(typeof(RectTransform))]
    [DisallowMultipleComponent]
    public abstract class UIComponent<T> : UIBase where T : UIData
    {
        public T Data { get; set; }
    }
}