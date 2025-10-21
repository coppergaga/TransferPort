using System.Reflection;

namespace RsLib
{
    public class RsField
    {
        public static object GetValue(object target, string fieldName)
        {
            FieldInfo field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            return field.GetValue(target);
        }
        
        public static void SetValue(object target, string fieldName, object value)
        {
            FieldInfo field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            field.SetValue(target, value);
        }

        public static void Copy(object source, object target, BindingFlags targetBindingFlags = BindingFlags.Instance | BindingFlags.Public)
        {
            FieldInfo[] fields = target.GetType().GetFields(targetBindingFlags);
            foreach (FieldInfo field in fields)
            {
                object oldValue = GetValue(source, field.Name);
                field.SetValue(target, oldValue);
            }
        }
        
    }
}