using System;

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
        public static void CheckArgumentNull(this object obj, string paramName)
        {
            if (obj == null)
                throw new ArgumentNullException(paramName);
        }
    }
}
