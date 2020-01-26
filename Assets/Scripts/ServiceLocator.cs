using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ServiceLocator
{
    private static readonly Dictionary<Type, object> Services = new Dictionary<Type, object>();

    public static T GetService<T>()
    {
        return (T)Services[typeof(T)];
    }
    public static void Register<T>(T service)
    {
        Services[typeof(T)] = service;
    }
    public static void Reset()
    {
        Services.Clear();
    }
}
