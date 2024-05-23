using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoSingleton<CameraController>
{
    public Camera m_UICamera;

    protected override void Awake()
    {
        base.Awake();
    }
}
