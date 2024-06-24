using Async.UIFramework;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginData : UIData
{
    public string Title;
}


[PanelLayer]
public class LoginPanel : UIComponent<LoginData>
{
    protected override UniTask OnRefresh()
    {
        return UniTask.CompletedTask;
    }
}
