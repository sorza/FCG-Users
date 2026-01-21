using FCG.Shared.Contracts.Results;
using FCG_Users.Application.Shared.Interfaces;
using FCG_Users.Application.Users.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FCG_Users.Api.Controllers
{
    [ApiController]
    [Route("api")]
    [Authorize]
    public class UserController(IAccountService service) : ControllerBase
    {
        /// <summary>
        /// Solicita o cadastro de um novo usuário no sistema.
        /// </summary>
        /// <param name="request">Dados necessários para o cadastro do usuário.</param>
        /// <param name="cancellation">Token para monitorar o cancelamento da requisição.</param>  
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [AllowAnonymous]
        [HttpPost]
        public async Task<IResult> CreateUserAsync(AccountRequest request, CancellationToken cancellation = default)
        {
            var correlationId = HttpContext.Items["CorrelationId"]?.ToString();            

            var result = await service.CreateAccountAsync(request, correlationId!, cancellation);            

            if (result.IsFailure)
            {
                return result.Error.Code switch
                {                   
                    "409" => TypedResults.Conflict(new Error("409", result.Error.Message)),
                    _ => TypedResults.BadRequest(new Error("400", result.Error.Message))
                };
            }

            return TypedResults.Accepted($"/users/{result.Value.Id}", new { User = result.Value, CorrelationId = correlationId });
        }

        /// <summary>
        /// Busca um usuário pelo id.
        /// </summary>
        /// <param name="id">Id do usuário</param>
        /// <param name="cancellation">Token de controle para monitorar cancelamento do processo.</param>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Admin")]
        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        public async Task<IResult> GetUserByIdAsync(Guid id, CancellationToken cancellation = default)
        {           
            var result = await service.GetUserAsync(id, cancellation);

            if (result.IsFailure)
            {
                return result.Error.Code switch
                {
                    "404" => TypedResults.NotFound(new Error("404", result.Error.Message)),                   
                    _ => TypedResults.BadRequest(new Error("400", result.Error.Message))
                };
            }

            return TypedResults.Ok(result.Value);

        }

        /// <summary>
        /// Busca todos os usuários cadastrados.
        /// </summary>
        /// <param name="cancellation">Token de controle para monitorar cancelamento do processo.</param>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IResult> GetAllUsersAsync(CancellationToken cancellation = default)
        {
            var result = await service.GetAllUsersAsync(cancellation);    

            return TypedResults.Ok(result.Value);

        }

        /// <summary>
        /// Autentica um usuário e retorna um token JWT.
        /// </summary>
        /// <param name="request">Informações necessárias para o login</param>
        /// <param name="cancellation">Token para monitorar o cancelamento da operação.</param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [AllowAnonymous]
        [HttpPost]
        [Route("auth")]
        public async Task<IResult> AuthAsync(AuthRequest request, CancellationToken cancellation = default)
        {            
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var device = HttpContext.Request.Headers["User-Agent"].ToString();
            var correlationId = HttpContext.Items["CorrelationId"]?.ToString();

            var result = await service.AuthAsync(request, ip, device, correlationId!, cancellation);

            if (result.IsFailure)
            {
                return result.Error.Code switch
                {
                    "401" => TypedResults.Unauthorized(),
                    "403" => TypedResults.Forbid(),
                    _ => TypedResults.BadRequest(new Error("400", result.Error.Message))
                };
            }
            return TypedResults.Ok(result.Value);          
        }

        /// <summary>
        /// Retorna as informações do usuário autenticado com base no token JWT fornecido.
        /// </summary>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpGet("auth/check")]
        [Authorize]
        public IActionResult Check()
        {
            if (!User.Identity?.IsAuthenticated ?? true)
                return Unauthorized();

            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var userId = User.FindFirst("UserId")?.Value;
            var name = User.FindFirst("Name")?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            return Ok(new
            {
                Id = userId,
                Email = email,
                Name = name,
                Role = role
            });
        }

        /// <summary>
        /// Solicita a exclusão de um cadastro de usuário do sistema
        /// </summary>
        /// <param name="id">Id do usuário</param>
        /// <param name="cancellationToken">Token para monitorar o cancelamento da operação.</param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:guid}")]
        public async Task<IResult> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
            var result = await service.RemoveUserAsync(id, correlationId!,cancellationToken);

            IResult response = result.IsSuccess
                ? TypedResults.Accepted($"/users/status/{correlationId}", new { UserId = id, CorrelationId = correlationId })
                : TypedResults.NotFound(new Error("404",result.Error.Message));

            return response;
        }

        /// <summary>
        /// Retorna 220 se a aplicação estiver saudável.
        /// </summary>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AllowAnonymous]
        [HttpGet("/health")]
        public async Task<IResult> Health() => TypedResults.Ok();
    }
}
