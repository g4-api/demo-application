namespace DemoApplication.Api.Contracts
{
    /// <summary>
    /// Represents the shared success envelope returned by every <c>/api/v{version}</c> endpoint.
    /// </summary>
    /// <remarks>
    /// The envelope keeps a stable shape across API versions so clients can branch on <see cref="Succeeded"/>
    /// without inspecting the transport status code. Failure responses do not use this type; unhandled and
    /// validation failures are returned as RFC 7807 <c>ProblemDetails</c> documents produced by the shared
    /// exception handler and model-validation pipeline.
    /// </remarks>
    /// <typeparam name="T">Payload type carried by a successful response.</typeparam>
    public sealed record ApiResponse<T>
    {
        /// <summary>
        /// Gets a value indicating whether the request completed successfully.
        /// </summary>
        public bool Succeeded { get; init; }

        /// <summary>
        /// Gets the payload produced by a successful request, or <see langword="null"/> when the request produced no body.
        /// </summary>
        public T? Data { get; init; }

        /// <summary>
        /// Gets the human-readable messages that describe a non-fault failure, such as a rejected command.
        /// </summary>
        /// <remarks>
        /// Always populated with an empty collection on success so clients can enumerate the value without a null check.
        /// </remarks>
        public IReadOnlyCollection<string> Errors { get; init; } = [];

        /// <summary>
        /// Creates a successful envelope that carries the supplied payload.
        /// </summary>
        /// <param name="data">Payload returned to the caller.</param>
        /// <returns>A response whose <see cref="Succeeded"/> flag is <see langword="true"/>.</returns>
        public static ApiResponse<T> Success(T data)
        {
            // Return the payload with an empty error collection so the success contract stays uniform.
            return new ApiResponse<T>
            {
                Succeeded = true,
                Data = data,
                Errors = []
            };
        }

        /// <summary>
        /// Creates a failed envelope that carries the supplied messages and no payload.
        /// </summary>
        /// <param name="errors">Messages that explain why the request did not succeed.</param>
        /// <returns>A response whose <see cref="Succeeded"/> flag is <see langword="false"/>.</returns>
        public static ApiResponse<T> Failure(IReadOnlyCollection<string> errors)
        {
            // Guard the message collection so a failed envelope never advertises a null contract to clients.
            ArgumentNullException.ThrowIfNull(
                argument: errors,
                paramName: nameof(errors));

            // Return the messages without a payload so callers read failure detail from a single place.
            return new ApiResponse<T>
            {
                Succeeded = false,
                Data = default,
                Errors = errors
            };
        }
    }
}
