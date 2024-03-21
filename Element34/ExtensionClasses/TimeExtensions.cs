using System;

namespace Element34.ExtensionClasses
{
    public static class TimeExtensions
    {
        private const int SecondsInDay = 86400;

        public static TimeSpan ParseTimespan(string sValue)
        {
            decimal decimalValue;
            DateTime dateTime;
            TimeSpan timeOfDay = TimeSpan.Zero;

            if (DateTime.TryParse(sValue, out dateTime))
            {
                timeOfDay = TimeSpan.Parse(string.Format("{0:HH:mm:ss}", dateTime));
            }
            else if (Decimal.TryParse(sValue, out decimalValue))
            {
                // Ensure the decimal value is within the range [0, 1]
                decimal normalizedValue = decimalValue % 1;
                if (normalizedValue < 0)
                    normalizedValue += 1;

                // Calculate the number of seconds elapsed
                int elapsedSeconds = (int)Math.Round(normalizedValue * SecondsInDay);

                // Convert seconds to TimeSpan
                timeOfDay = TimeSpan.FromSeconds(elapsedSeconds);
            }
            else if (TimeSpan.TryParse(sValue, out timeOfDay))
            { }
            else
                throw new ArgumentException("Invalid time format.", nameof(sValue));

            return timeOfDay;
        }
    }
}
