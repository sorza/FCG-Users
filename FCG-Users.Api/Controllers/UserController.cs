using FCG_Users.Application.Shared.Results;
using FCG_Users.Application.Users.Interfaces;
using FCG_Users.Application.Users.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FCG_Users.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController(IAccountService service) : ControllerBase
    {
        /// <summary>
        /// Cadastra um novo usuário no sistema.
        /// </summary>
        /// <param name="request">Dados necessários para o cadastro do usuário.</param>
        /// <param name="cancellation">Token para monitorar o cancelamento da requisição.</param>  
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost]
        public async Task<IResult> CreateUserAsync(AccountRequest request, CancellationToken cancellation = default)
        {
            try
            {
                var result = await service.CreateAccountAsync(request, cancellation);

                IResult response = result.IsSuccess
                    ? TypedResults.Created($"/user/{result.Value.Id}", result.Value)
                    : TypedResults.Conflict(new Error("409", result.Error.Message));

                return response;
            }
            catch (Exception ex)
            {
                return TypedResults.BadRequest(new Error("400", ex.Message));
            }
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
        [HttpPost]
        [Route("auth")]
        public async Task<IResult> AuthAsync(AuthRequest request, CancellationToken cancellation = default)
        {
            try
            {
                var result = await service.AuthAsync(request, cancellation);

                IResult response = result.IsSuccess
                    ? TypedResults.Ok(result.Value)
                    : result.Error.Code switch
                    {
                        "401" => TypedResults.Unauthorized(),
                        _ => TypedResults.Forbid()
                    };

                return response;

            }
            catch (Exception ex)
            {
                return TypedResults.BadRequest(new Error("400", ex.Message));
            }
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

    }
}
