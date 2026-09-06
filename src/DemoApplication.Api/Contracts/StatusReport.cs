namespace DemoApplication.Api.Contracts
{
    /// <summary>
    /// Describes the payload returned by the versioned status endpoint used for smoke checks.
    /// </summary>
    /// <remarks>
    /// The type intentionally models only the fields a smoke check needs. It carries no infrastructure detail
    /// so the response stays safe to expose without authentication.
    /// </remarks>
    public sealed record StatusReport
    {
        /// <summary>
        /// Gets the coarse health word reported by a running API host. The current contract always returns <c>ok</c>.
        /// </summary>
        public required string Status { get; init; }

        /// <summary>
        /// Gets the API major version that served the response, formatted as <c>v{major}</c>.
        /// </summary>
        public required string ApiVersion { get; init; }
    }
}
