using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;



public class GameManager : MonoSingleton<GameManager>
{
    Dictionary<Type, GameSystem> systemManagers;

    protected override void Awake()
    {
        base.Awake();

        AddAllManagers();

        GameInitialize.Instance.GameInitEvent += OnGameInit;
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void OnGameInit()
    {

    }

    void AddAllManagers()
    {
        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (Type type in assembly.GetTypes())
            {
                if (typeof(GameSystem).IsAssignableFrom(type))
                {
                    if (type.IsClass && !type.IsAbstract)
                    {
                        systemManagers.Add(type, (GameSystem)assembly.CreateInstance(type.Name));
                    }
                }
            }
        }
    }
}
