using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoSingleton<CameraController>
{
    public Camera uiCamera;

    protected override void Awake()
    {
        base.Awake();
    }
}
