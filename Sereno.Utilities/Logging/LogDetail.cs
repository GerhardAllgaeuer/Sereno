using System;

namespace Sereno.Logging
{
    /// <summary>
    /// Detail Information zu einem Logeintrag
    /// </summary>
    public class LogDetail
    {
        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="description">Beschreibung</param>
        /// <param name="value">Wert</param>
        public LogDetail(string description, object? value)
        {
            this.Description = description ?? throw new ArgumentNullException(nameof(description));
            this.Value = value?.ToString() ?? string.Empty;
        }

        /// <summary>
        /// Beschreibung des Details
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Wert für dieses Detail
        /// </summary>
        public string Value { get; set; } = string.Empty;
    }
}
