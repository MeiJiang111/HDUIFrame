using UnityEngine;



[RequireComponent(typeof(RectTransform))]
[DisallowMultipleComponent]
public abstract class UIComponent<T> : UIBase where T : UIData
{
    public T Data { get; set; }
}
