using Async.UIFramework;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[PanelLayer]
public class LoadingPanel : UIComponent<LoadingData>
{
    public Slider slider;
    public Text precetText;

    protected override UniTask OnCreate()
    {
        return UniTask.CompletedTask;
    }

    protected override UniTask OnRefresh()
    {
        return UniTask.CompletedTask;
    }

    protected override void OnBind()
    {
       
    }

    protected override void OnUnbind()
    {
       
    }

    protected override void OnShow()
    {

    }

    protected override void OnHide()
    {

    }
}
