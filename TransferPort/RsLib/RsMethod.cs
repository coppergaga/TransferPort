using System.Reflection;

namespace RsLib
{
    public class RsMethod
    {
        private static BindingFlags DefaultFlags =
            BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
        
        public static object Invoke(object instance, string method, params object[] args)
        {
            return instance.GetType().GetMethod(method, DefaultFlags).Invoke(instance, args);
        }
    }
}