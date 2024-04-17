using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using testSwaggerUIError.Services;
using Unchase.Swashbuckle.AspNetCore.Extensions.Extensions;

namespace testSwaggerUIError
{
    public class ConfigureSwaggerOptions : IConfigureNamedOptions<SwaggerGenOptions>
    {
        private readonly IApiVersionDescriptionProvider provider;

        public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider)
        {
            this.provider = provider;
        }

        public void Configure(SwaggerGenOptions options)
        {
            // add swagger document for every API version discovered
            foreach (var description in provider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(
                    description.GroupName,
                    CreateVersionInfo(description));
            }

            // SwaggerDocFilter removes 'internal API' controllers and methods from swagger json
            options.DocumentFilter<SwaggerDocFilter>();
            options.OperationFilter<SwaggerDefaultValues>();

            options.MapType<TimeSpan>(() => new OpenApiSchema
            {
                Type = "string",
                Example = new OpenApiString("02:00:00"),
                Format = "time-span"
            });
            options.CustomOperationIds(api =>
            {
                var group = api.GroupBySwaggerGroupAttribute();
                var controller = group != null && group.Length > 0
                    ? group[0]
                    : api.ActionDescriptor.RouteValues["controller"];
                var action = api.ActionDescriptor.RouteValues["action"];
                var operationId = $"{controller}_{action}";
                return operationId;
            });

            // GroupBySwaggerGroupAttribute replaces standard tagging by controller name
            // with custom grouping by GroupName of a SwaggerGroupAttribute
            options.TagActionsBy(api => api.GroupBySwaggerGroupAttribute());

            options.UseAllOfToExtendReferenceSchemas();
            options.AddEnumsWithValuesFixFilters();

            // Set the comments path for the Swagger JSON and UI.
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            options.IncludeXmlComments(xmlPath);
        }

        public void Configure(string name, SwaggerGenOptions options)
        {
            Configure(options);
        }

        private OpenApiInfo CreateVersionInfo(ApiVersionDescription description)
        {
            var info = new OpenApiInfo()
            {
                Title = "kBridge.WebApi",
                Version = description.ApiVersion.ToString()
            };

            if (description.IsDeprecated)
            {
                info.Description += " This API version has been deprecated.";
            }

            return info;
        }
    }
}