using System.Reflection;
using Academix.WebAPI.Common;

namespace Academix.WebAPI.Common
{
    public static class EndpointExtensions
    {
        /// <summary>
        /// Discovers and maps all endpoints that implement IEndpoint in the assembly
        /// </summary>
        public static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder app, Assembly assembly)
        {
            var endpointTypes = assembly.GetTypes()
                .Where(t => t.IsAssignableTo(typeof(IEndpoint)) && !t.IsAbstract && !t.IsInterface)
                .ToList();

            foreach (var endpointType in endpointTypes)
            {
                var endpoint = Activator.CreateInstance(endpointType) as IEndpoint;
                endpoint?.MapEndpoint(app);
            }

            return app;
        }

        /// <summary>
        /// Maps all endpoints in the current assembly
        /// </summary>
        public static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder app)
        {
            return app.MapEndpoints(Assembly.GetCallingAssembly());
        }

        /// <summary>
        /// Maps endpoints from a specific feature namespace
        /// </summary>
        public static IEndpointRouteBuilder MapFeatureEndpoints<TFeature>(this IEndpointRouteBuilder app)
        {
            var featureNamespace = typeof(TFeature).Namespace;
            var assembly = typeof(TFeature).Assembly;

            var endpointTypes = assembly.GetTypes()
                .Where(t => t.IsAssignableTo(typeof(IEndpoint)) 
                    && !t.IsAbstract 
                    && !t.IsInterface
                    && t.Namespace?.StartsWith(featureNamespace!) == true)
                .ToList();

            foreach (var endpointType in endpointTypes)
            {
                var endpoint = Activator.CreateInstance(endpointType) as IEndpoint;
                endpoint?.MapEndpoint(app);
            }

            return app;
        }
    }
} 