using System;
using System.Reflection;
using UnityEngine;

namespace RadialUI.Extensions
{
    /// <summary>
    /// Reflections specifically to continue original RadialUI Methods to maintain the invocation
    /// of methods and run patches. 
    /// </summary>
    internal static class Reflections
    {
        private const BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
        
        internal static Action<MapMenu,object> GetMenuAction<Tclass>(string method,Tclass o)
        {
            return CreateReusableAction<Tclass,MapMenu,object>(method, o); 
        }

        internal static Action<MapMenuItem, object> GetMenuItemAction<Tclass>(string method, Tclass o)
        {
            return CreateReusableAction<Tclass, MapMenuItem, object>(method, o);
        }

        internal static T CallMethod<T,TClass>(string methodName, TClass instance, object[] param = null)
        {
            if (param == null) param = new object[0];
            var method = typeof(TClass).GetMethod(methodName, bindFlags);
            var result = (T) method.Invoke(instance, param);
            return result;
        }

        internal static void CallMethod<TClass>(string methodName, TClass instance, object[] param = null)
        {
            if (param == null) param = new object[0];
            var method = typeof(TClass).GetMethod(methodName, bindFlags);
            method.Invoke(instance, param);
        }

        internal static Action<Tp1, Tp2> CreateReusableAction<TClass, Tp1, Tp2>(string methodName, TClass instance)
        {
            var method = typeof(TClass).GetMethod(methodName,bindFlags);
            if (method == null)
            {
                Debug.Log($"Method {methodName} for {instance.GetType()} was not found.");
                return null;
            }
            void Caller(Tp1 param1, Tp2 param2) => method.Invoke(instance, new object[] { param1, param2 });
            return Caller;
        }

    }
}
