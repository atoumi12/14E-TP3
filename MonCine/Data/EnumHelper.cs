using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace MonCine.Data
{

    /// <summary>
    /// Classe permettant d'ajouter des méthodes à au type énumération
    /// </summary>
    public static class EnumHelper
    {
        /// <summary>
        /// Permet d'obtenir la description d'un type d'énumération
        /// </summary>
        /// <typeparam name="T">Le type d'énumération</typeparam>
        /// <param name="enumValue">L'énumération</param>
        /// <returns>La description de l'énumération</returns>
        public static string GetDesc<T>(this T enumValue) where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
                return null;

            var description = enumValue.ToString();
            var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());

            if (fieldInfo != null)
            {
                var attrs = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), true);
                if (attrs?.Length > 0)
                {
                    description = ((DescriptionAttribute)attrs[0]).Description;
                }
            }

            return description;
        }
    }
}
