using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace testSwaggerUIError.Services
{
    public static class ApiDescriptionExtensions
    {
        public static string[] GroupBySwaggerGroupAttribute(this ApiDescription api)
        {
            var actionDescriptor = api.GetProperty<ControllerActionDescriptor>();

            if (actionDescriptor == null)
            {
                actionDescriptor = api.ActionDescriptor as ControllerActionDescriptor;
                api.SetProperty(actionDescriptor);
            }

            var attribute = actionDescriptor?.EndpointMetadata.OfType<SwaggerGroupAttribute>().FirstOrDefault();

            return new string[]
            {
                attribute?.GroupName ?? actionDescriptor?.ControllerName
            };
        }
    }
}