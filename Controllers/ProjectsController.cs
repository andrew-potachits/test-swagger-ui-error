using System.Net;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Mvc;
using testSwaggerUIError.Dto;

namespace testSwaggerUIError.Controllers;

[ApiController]
[ResponseCache(NoStore = true, Duration = 0, Location = ResponseCacheLocation.None)]
[Route("api/projects")]
[Produces("application/json")]
[ProducesResponseType(typeof(WebException), 401)]
public class ProjectsController : ControllerBase
{
    [HttpPost("transfer")]
    [ProducesResponseType(typeof(OperationResultDto<string>), 200)]
    [ProducesResponseType(typeof(OperationResultDto), 400)]
    [ProducesResponseType(typeof(OperationResultDto), 403)]
    [ProducesResponseType(typeof(OperationResultDto), 404)]
    [ProducesResponseType(typeof(OperationResultDto), 500)]
    public async Task<IActionResult> TransferProjectOwnership([FromBody] TransferOwnershipModelDto model,
       [FromHeader(Name = HeaderVars.ForAdmin), Optional] bool forAdmin)
    {
        return Ok();
    }
}

public static class AuthPolicies
{
    public const string Public = "kBridge.WebApi.Public";
    public const string Internal = "kBridge.WebApi.Internal";
    public const string PublicPlusToken = "kBridge.WebApi.Public+Token";
}