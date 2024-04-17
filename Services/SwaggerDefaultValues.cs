using System.Text.Json;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace testSwaggerUIError.Services;

/// <summary>
/// Represents the OpenAPI/Swashbuckle operation filter used to document information provided, but not used.
/// </summary>
/// <remarks>This <see cref="IOperationFilter"/> is only required due to bugs in the <see cref="SwaggerGenerator"/>.
/// Once they are fixed and published, this class can be removed.</remarks>
public class SwaggerDefaultValues : IOperationFilter
{
    public static string ApiVersionParameterName = "api-version";

    public void Apply( OpenApiOperation operation, OperationFilterContext context )
    {
        var apiDescription = context.ApiDescription;

        // operation.Deprecated |= apiDescription.IsDeprecated();

        // [CT] code below commented out b/c it removes IEnumerable<T> return types from generated.cs
        // REF: https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/1752#issue-663991077
        //foreach ( var responseType in context.ApiDescription.SupportedResponseTypes )
        //{
        //    // REF: https://github.com/domaindrivendev/Swashbuckle.AspNetCore/blob/b7cf75e7905050305b115dd96640ddd6e74c7ac9/src/Swashbuckle.AspNetCore.SwaggerGen/SwaggerGenerator/SwaggerGenerator.cs#L383-L387
        //    var responseKey = responseType.IsDefaultResponse ? "default" : responseType.StatusCode.ToString();
        //    var response = operation.Responses[responseKey];

        //    foreach ( var contentType in response.Content.Keys )
        //    {
        //        if ( !responseType.ApiResponseFormats.Any( x => x.MediaType == contentType ) )
        //        {
        //            response.Content.Remove( contentType );
        //        }
        //    }
        //}

        if ( operation.Parameters == null )
        {
            return;
        }

        // now remove version-related parameters
        // api version is going to be passed 'automatically' via media type header value
        // it is tied to a version of api swagger json
        operation.Parameters = operation.Parameters.Where(p => p.Name != SwaggerDefaultValues.ApiVersionParameterName).ToList();

        // REF: https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/412
        // REF: https://github.com/domaindrivendev/Swashbuckle.AspNetCore/pull/413
        foreach ( var parameter in operation.Parameters )
        {
            var description = apiDescription.ParameterDescriptions.First( p => p.Name == parameter.Name );

            parameter.Description ??= description.ModelMetadata?.Description;
            parameter.Required |= description.IsRequired;

            if (parameter.Schema.Default == null &&
                description.DefaultValue != null &&
                description.DefaultValue is not System.DBNull &&
                description.ModelMetadata is ModelMetadata modelMetadata)
            {
                // REF: https://github.com/Microsoft/aspnet-api-versioning/issues/429#issuecomment-605402330
                var json = JsonSerializer.Serialize(description.DefaultValue, modelMetadata.ModelType);
                parameter.Schema.Default = OpenApiAnyFactory.CreateFromJson(json);
            }
        }
    }
}