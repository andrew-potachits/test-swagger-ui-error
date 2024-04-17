using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using NSwag.Annotations;
using Swashbuckle.AspNetCore.SwaggerGen;
using testSwaggerUIError.Controllers;

namespace testSwaggerUIError.Services
{
    public class SwaggerDocFilter : IDocumentFilter
    {
        /// <summary>
        /// Filter out [Authorize('Scope=kBridge.WebApi.Internal')] and [OpenApiIgnore] methods and controllers from swagger doc/json
        /// </summary>
        /// <param name="swaggerDoc"></param>
        /// <param name="context"></param>
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            foreach (var description in context.ApiDescriptions)
            {
                var filter = description.RelativePath?[0] == '/' ? description.RelativePath : '/' + description.RelativePath;
                var attributes1 = description.ActionDescriptor.EndpointMetadata.OfType<AuthorizeAttribute>().ToList();
                var openApiIgnored = description.ActionDescriptor.EndpointMetadata.OfType<OpenApiIgnoreAttribute>();
                var hiddenPublicApi = description.ActionDescriptor.EndpointMetadata.OfType<HidePublicApiAttribute>();
                var allowed = !attributes1.Any() || attributes1.Any(a => a.Policy == AuthPolicies.Public);
                var ignore = hiddenPublicApi.Any() || openApiIgnored.Any();
                if (!allowed || ignore)
                {
                    var toBeRemoved = swaggerDoc.Paths
                        .Where(x => x.Key.Equals(filter, System.StringComparison.InvariantCultureIgnoreCase));
                    foreach (var item in toBeRemoved)
                    {
                        if (item.Value.Operations.Count() > 1)
                        {
                            // here we remove just some api operations
                            // example: GET /api/list is public, but POST /api/list is hidden
                            foreach (var op in item.Value.Operations)
                            {
                                if (op.Key.ToString().Equals(
                                    description.HttpMethod, System.StringComparison.InvariantCultureIgnoreCase))
                                {
                                    item.Value.Operations.Remove(op.Key);
                                }
                            }
                        }
                        else
                        {
                            // remove api entry completely if there is just single operation
                            swaggerDoc.Paths.Remove(item.Key);
                        }
                    }
                }
            }
        }
    }
}
