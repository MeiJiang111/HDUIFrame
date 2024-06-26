using Async.UIFramework;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginData : UIData
{
    public string user;
    public string password;
}


[PanelLayer]
public class LoginPanel : UIComponent<LoginData>
{
    public InputField inputUser;
    public InputField inputPassword;
    public Button btnLogin;
   
    protected override UniTask OnCreate()
    {
        return UniTask.CompletedTask;
    }

    protected override UniTask OnRefresh()
    {
        //Debug.Log("LoginPanel -- OnRefresh");
        return UniTask.CompletedTask;
    }

    protected override void OnBind()
    {
        btnLogin.onClick.AddListener(() =>
        {
            VerifyLogin();
        });
    }

    protected override void OnUnbind() 
    {
        btnLogin.onClick.RemoveAllListeners();
    }

    protected override void OnShow()
    {

    }

    protected override void OnHide()
    {

    }

    private void VerifyLogin()
    {
        string user = inputUser.text;
        string password = inputPassword.text;

        if(string.Equals(user, this.Data.user) && string.Equals(password, this.Data.password))
        {
            LogUtil.Log("ddd -- µÇÂ¼³É¹¦ ...");
            UIFrame.Show<LoadingPanel>();
        }
        else
        {
            LogUtil.Log("ddd -- µÇÂ¼Ê§°Ü -- ÕËºÅ»òÃÜÂë´íÎó ...");
        }
    }
}
