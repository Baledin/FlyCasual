using System;
using System.ComponentModel;
using System.Reflection;

namespace Assets.Scripts.Model.Tools.Enums
{
    public static class EnumExtensions
    {
        public static string GetDescription(this Enum value)
        {
            FieldInfo fieldInfo = value.GetType().GetField(value.ToString());
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : value.ToString();
        }

        public static T GetEnumValueFromDescription<T>(string description) where T : Enum
        {
            foreach (FieldInfo field in typeof(T).GetFields())
            {
                DescriptionAttribute attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));
                if (attribute != null && attribute.Description == description)
                {
                    return (T)field.GetValue(null);
                }
            }
            throw new ArgumentException($"No enum value with description '{description}' found in {typeof(T).Name}.");
        }
    }
}
