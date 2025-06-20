namespace Academix.WebAPI.Common
{
    /// <summary>
    /// Represents an endpoint that can be mapped to the application
    /// </summary>
    public interface IEndpoint
    {
        /// <summary>
        /// Maps the endpoint to the application's route builder
        /// </summary>
        /// <param name="app">The endpoint route builder</param>
        void MapEndpoint(IEndpointRouteBuilder app);
    }
} 