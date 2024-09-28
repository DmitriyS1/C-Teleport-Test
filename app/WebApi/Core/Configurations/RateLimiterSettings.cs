namespace CTeleport.Weather.Api.Core.Configurations
{
    /// <summary>
    /// Rate limiter settings
    /// </summary>
    public record RateLimiterSettings
    {
        /// <summary>
        /// Amount of allowed requests per WindowInSeconds period. Must be greater than 0.
        /// </summary>
        public uint PermitLimit { get; set; }

        /// <summary>
        /// Window in seconds to evaluate the rate limit
        /// </summary>
        public uint WindowInSeconds { get; set; }

        /// <summary>
        /// Maximum cumulative permit count of queued acquisition requests
        /// </summary>
        public uint QueueLimit { get; set; }
    }
}