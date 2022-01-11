using System;
using System.Reflection;
using UnityEngine;

namespace RadialUI.Extensions
{
    /// <summary>
    /// Reflections specifically to continue original RadialUI Methods to maintain the invocation
    /// of methods and run patches. 
    /// </summary>
    public static class Reflections
    {
        private const BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;


        public static Action<MapMenuItem, object> GetMenuItemActions(string method, CreatureMenuBoardTool tool)
        {
            return (t, u) =>
            {
                if (RadialUIPlugin.SelectedCreatures.Count > 0)
                {
                    foreach (var selected in RadialUIPlugin.SelectedCreatures)
                    {
                        tool.SetCreature(selected,0f);
                        Console.WriteLine("Invoking");
                        var a = CreateReusableAction<CreatureMenuBoardTool, MapMenuItem, object>(method, tool);
                        a(t, u);
                    }
                }
                else
                {
                    var a = CreateReusableAction<CreatureMenuBoardTool, MapMenuItem, object>(method, tool);
                    a.Invoke(t, u);
                }
            };
        }

        public static Action<MapMenu, object> GetMenuActions(string method, CreatureMenuBoardTool tool)
        {
            return (t, u) =>
            {
                if (RadialUIPlugin.SelectedCreatures.Count > 0)
                {
                    foreach (var selected in RadialUIPlugin.SelectedCreatures)
                    {
                        tool.SetCreature(selected, 0f);
                        Console.WriteLine("Invoking");
                        var a = CreateReusableAction<CreatureMenuBoardTool, MapMenu, object>(method, tool);
                        a(t, u);
                    }
                }
                else
                {
                    var a = CreateReusableAction<CreatureMenuBoardTool, MapMenu, object>(method, tool);
                    a.Invoke(t, u);
                }
            };
        }

        public static Action<MapMenu,object> GetMenuAction<Tclass>(string method,Tclass o)
        {
            return CreateReusableAction<Tclass,MapMenu,object>(method, o); 
        }

        public static Action<MapMenuItem, object> GetMenuItemAction<Tclass>(string method, Tclass o)
        {
            return CreateReusableAction<Tclass, MapMenuItem, object>(method, o);
        }

        public static T CallMethod<T,TClass>(string methodName, TClass instance, object[] param = null)
        {
            if (param == null) param = new object[0];
            var method = typeof(TClass).GetMethod(methodName, bindFlags);
            var result = (T) method.Invoke(instance, param);
            return result;
        }

        public static void CallMethod<TClass>(string methodName, TClass instance, object[] param = null)
        {
            if (param == null) param = new object[0];
            var method = typeof(TClass).GetMethod(methodName, bindFlags);
            method.Invoke(instance, param);
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
