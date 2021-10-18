using System;
using System.Reflection;
using UnityEngine;

namespace RadialUI.Reflection_Extensions
{
    public static class Reflections
    {
        private const BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
        // To Do
        public static Action<MapMenuItem,object> GetMenuAction<Tclass>(string method,Tclass o)
        {
            return CreateReusableAction<Tclass,MapMenuItem,object>(method, o); 
        }

        public static Action<Tp1, Tp2> CreateReusableAction<TClass, Tp1, Tp2>(string methodName, TClass instance)
        {
            var method = typeof(TClass).GetMethod(methodName,bindFlags);
            Action<Tp1, Tp2> Caller = (Tp1 param1, Tp2 param2) => method.Invoke(instance, new object[] { param1, param2 });
            Debug.Log(instance == null ? "Failed to get Instance" : "Instance Appended");
            return Caller;
        }

    }
}
