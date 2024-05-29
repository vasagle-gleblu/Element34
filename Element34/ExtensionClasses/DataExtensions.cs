using System;
using System.Data;

namespace Element34.ExtensionClasses
{
    /// <summary>
    /// Provides utility methods for working with <see cref="DataRow"/> objects.
    /// </summary>
    public static class DataRowUtility
    {
        /// <summary>
        /// Determines whether all fields in the specified <see cref="DataRow"/> are empty.
        /// A field is considered empty if it is <see cref="DBNull.Value"/> or a whitespace string.
        /// </summary>
        /// <param name="row">The <see cref="DataRow"/> to check.</param>
        /// <returns><c>true</c> if all fields are empty; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="row"/> is null.</exception>
        public static bool AreAllFieldsEmpty(DataRow row)
        {
            if (row == null)
                return true;

            foreach (var item in row.ItemArray)
            {
                if (item != DBNull.Value && !string.IsNullOrWhiteSpace(item.ToString()))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
