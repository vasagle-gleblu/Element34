using System;

namespace Element34.ExtensionClasses
{
    /// <summary>
    /// Provides extension methods for the <see cref="TimeSpan"/> structure.
    /// </summary>
    public static class TimeSpanExtensions
    {
        private const int SecondsInDay = 86400;

        /// <summary>
        /// Parses a string into a <see cref="TimeSpan"/> object. The string can represent a time in the following formats:
        /// - A time of day (e.g., "13:45:30")
        /// - A decimal representing a fraction of a day (e.g., "0.5" for 12:00:00)
        /// - A <see cref="TimeSpan"/> string (e.g., "12:30:00")
        /// </summary>
        /// <param name="sValue">The string representation of the time.</param>
        /// <returns>A <see cref="TimeSpan"/> object representing the parsed time.</returns>
        /// <exception cref="ArgumentException">Thrown when the input string is not in a valid format.</exception>
        public static TimeSpan ParseTimespan(string sValue)
        {
            if (string.IsNullOrWhiteSpace(sValue))
                throw new ArgumentException("Input value cannot be null or empty.", nameof(sValue));

            if (DateTime.TryParse(sValue, out DateTime dateTime))
            {
                return dateTime.TimeOfDay;
            }

            if (decimal.TryParse(sValue, out decimal decimalValue))
            {
                decimal normalizedValue = decimalValue % 1;
                if (normalizedValue < 0)
                    normalizedValue += 1;

                int elapsedSeconds = (int)Math.Round(normalizedValue * SecondsInDay);
                return TimeSpan.FromSeconds(elapsedSeconds);
            }

            if (TimeSpan.TryParse(sValue, out TimeSpan timeOfDay))
            {
                return timeOfDay;
            }

            throw new ArgumentException("Invalid time format.", nameof(sValue));
        }
    }
}
