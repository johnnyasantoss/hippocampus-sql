using System;
using System.Collections.Generic;
using System.Linq;

namespace HippocampusSql.Utils
{
    public static class Error
    {
        /// <summary>
        /// Throws a <see cref="ArgumentNullException"/> if the param <paramref name="obj"/> is null.
        /// </summary>
        /// <param name="obj">Object to check</param>
        /// <param name="paramName">Name of the object</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static T CheckNull<T>(this T obj, string paramName)
        {
            if (obj == null)
                throw new ArgumentNullException(paramName);
            return obj;
        }

        public static string CheckEmpty(this string obj, string paramName)
        {
            if (obj == null)
                return null;

            if (string.IsNullOrWhiteSpace(obj))
                throw new ArgumentNullException(paramName, "The argument must not be empty.");

            return obj;
        }

        public static IEnumerable<T> CheckEmpty<T>(this IEnumerable<T> obj, string paramName)
        {
            if (obj == null)
                return null;

            if (obj.Count() == 0)
                throw new ArgumentNullException(paramName, "The argument must not be empty.");

            return obj;
        }
    }
}
