using Microsoft.AspNetCore.Mvc;

namespace FCG_Users.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController
    {
        /// <summary>
        /// Cadastra um novo usuário no sistema.
        /// </summary>
        /// <param name="request">Dados necessários para o cadastro do usuário.</param>
        /// <param name="cancellation">Token para monitorar o cancelamento da requisição.</param>       
        [HttpPost]
        public async Task<IResult> CreateGameAsync(CancellationToken cancellation = default)
        {
            return Results.Ok();
        }
    }
}
