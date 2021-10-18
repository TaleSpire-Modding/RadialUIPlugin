using System;

namespace RadialUI.Reflection_Extensions
{
    public static class Reflections
    {
        // To Do
        public static Action<MapMenuItem,object> GetMenuAction<Tclass>(string method)
        {
            return CreateReusableAction<Tclass,MapMenuItem,object>(method); 
        }

        public static Action<Tp1, Tp2> CreateReusableAction<TClass, Tp1, Tp2>(string methodName)
        {
            var method = typeof(TClass).GetMethod(methodName);
            var del = Delegate.CreateDelegate(typeof(Action<TClass>), method);
            Action<Tp1, Tp2> caller = (instance, param) => del.DynamicInvoke(instance, param);
            return caller;
        }

    }
}
