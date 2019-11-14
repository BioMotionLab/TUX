using System;
using JetBrains.Annotations;

namespace bmlTUX.Scripts.Utilities.Extensions {
    public static class TypeExtension
    {
        /// <summary>
        /// Returns the last part of name. So System.Type will be shortened to Type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [PublicAPI]
        public static string LastPartOfTypeName(this Type type) {
            string[] typeNameFull = type.FullName?.Split('.');
            string typeNameLast = typeNameFull?[typeNameFull.Length - 1];
            return typeNameLast;
        }
    }
}
