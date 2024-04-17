using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using testSwaggerUIError;
using testSwaggerUIError.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(3, 0);
    options.ReportApiVersions = true;
            
    options.ApiVersionReader = ApiVersionReader.Combine(
        new HeaderApiVersionReader(SwaggerDefaultValues.ApiVersionParameterName),
        // removed the one below as it adds the api-version: x.y; to the Content-Type, which breaks the locust performance tester
        // new MediaTypeApiVersionReader(SwaggerDefaultValues.ApiVersionParameterName),
        new QueryStringApiVersionReader(SwaggerDefaultValues.ApiVersionParameterName));
});
builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV"; //  TODO: try VVVV
});

builder.Services.AddSwaggerGen();
builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();


var app = builder.Build();

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

// app.MapControllers();

app.UseApiVersioning();
app.UseSwagger();

var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
app.UseSwaggerUI(c =>
{
    foreach (var description in provider.ApiVersionDescriptions)
    {
        c.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
    }
});


app.Run();