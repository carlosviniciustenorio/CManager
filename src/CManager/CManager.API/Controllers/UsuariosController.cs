﻿using CManager.API.Attributes;
using CManager.API.Shared;
using CManager.Infrastructure.Constants.Identity;
using CManager.Integration.Cache;
using ECommerceCT.Application.DTOs.Requests;
using ECommerceCT.Application.DTOs.Responses;
using System.Net;
using System.Security.Claims;

namespace CManager.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ApiControllerBase
    {
        private readonly IIdentityService _identityService;
        private readonly ILogger<UsuariosController> _logger;
        public UsuariosController(IIdentityService identityService, ILogger<UsuariosController> logger)
        {
            _identityService = identityService;
            _logger = logger;
        }

        /// <summary>
        /// Cadastro de usuário.
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="usuarioCadastro">Dados de cadastro do usuário</param>
        /// <returns></returns>
        /// <response code="200">Usuário criado com sucesso</response>
        /// <response code="400">Retorna erros de validação</response>
        /// <response code="500">Retorna erros da aplicação caso ocorram</response>
        [ProducesResponseType(typeof(UsuarioCadastroResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = Roles.Admin)]
        [ClaimsAuthorizeAttribute(Infrastructure.Constants.Identity.ClaimTypes.Usuarios, "Create")]
        [HttpPost]
        public async Task<ActionResult<UsuarioCadastroResponse>> Cadastrar([FromBody] UsuarioCadastroRequest usuarioCadastro)
        {
            _logger.LogInformation($"Tentativa de cadastro de usuário, request: {JsonConvert.SerializeObject(usuarioCadastro)}");
            if (!ModelState.IsValid)
                return BadRequest();
        
            UsuarioCadastroResponse resultado = await _identityService.CadastrarUsuario(usuarioCadastro);
            if (resultado.Sucesso)
                return Ok(resultado); 
            
            if (resultado.Erros.Any())
                return BadRequest(new CustomProblemDetails(HttpStatusCode.BadRequest, Request, errors: resultado.Erros));

            return StatusCode(StatusCodes.Status500InternalServerError);
        }
        
        /// <summary>
        /// Consultar telefone do usuário
        /// </summary>
        /// <param name="id">Identificador do usuário</param>
        /// <returns></returns>
        /// <response code="200">Telefone do Usuário retornado com sucesso</response>
        /// <response code="400">Retorna erros de validação</response>
        /// <response code="500">Retorna erros da aplicação caso ocorram</response>
        [Authorize]
        [ProducesResponseType(typeof(UsuarioTelefoneResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [HttpGet("{id}/phone")]
        public async Task<ActionResult<UsuarioTelefoneResponse>> Get([FromRoute] string id)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            return await _identityService.GetTelefoneUsuarioAsync(id);
        }

        /// <summary>
        /// Associa uma role a um usuário.
        /// </summary>
        /// <remarks>
        /// Esta operação permite associar uma role a um usuário específico com base nos dados fornecidos no corpo da solicitação.
        /// </remarks>
        /// <param name="user">Os dados do usuário e a role a ser associada.</param>
        /// <returns>Um código de status HTTP indicando o sucesso da operação.</returns>
        /// <response code="200">Retorna OK se a role for associada com sucesso.</response>
        /// <response code="400">Se os dados do usuário fornecidos forem inválidos.</response>
        /// <response code="401">Se o usuário não estiver autorizado a acessar o recurso.</response>
        [ProducesResponseType(typeof(Unit), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize(Roles = Roles.Admin)]
        [HttpPost("role")]
        public async Task<ActionResult> Role([FromBody] UsuarioCadastroRole user)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            await _identityService.CadastrarRole(user.Role, user.Email);

            return Ok();
        }

        // <summary>
        /// Realiza o login de um usuário.
        /// </summary>
        /// <remarks>
        /// Esta operação permite que um usuário faça login no sistema utilizando suas credenciais.
        /// </remarks>
        /// <param name="usuarioLogin">As credenciais de login do usuário.</param>
        /// <returns>Um objeto contendo as informações do usuário logado.</returns>
        /// <response code="200">Retorna OK com as informações do usuário logado se o login for bem-sucedido.</response>
        /// <response code="400">Se as credenciais de login fornecidas forem inválidas.</response>
        /// <response code="401">Se o usuário não estiver autorizado a acessar o recurso.</response>
        [ProducesResponseType(typeof(UsuarioCadastroResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpPost("login")]
        public async Task<ActionResult<UsuarioCadastroResponse>> Login(UsuarioLoginRequest usuarioLogin)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            UsuarioLoginResponse resultado = await _identityService.Login(usuarioLogin);
            if (resultado.Sucesso)
                return Ok(resultado);

            return Unauthorized(resultado);
        }

        /// <summary>
        /// Atualiza o token de acesso de um usuário.
        /// </summary>
        /// <remarks>
        /// Esta operação permite atualizar o token de acesso de um usuário com base em um refresh token válido.
        /// </remarks>
        /// <returns>Um objeto contendo as informações atualizadas do usuário.</returns>
        /// <response code="200">Retorna OK com as informações do usuário atualizadas se a atualização for bem-sucedida.</response>
        /// <response code="400">Se o ID do usuário não puder ser extraído da solicitação.</response>
        /// <response code="401">Se o usuário não estiver autorizado a acessar o recurso ou se o refresh token fornecido for inválido.</response>
        [ProducesResponseType(typeof(UsuarioCadastroResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpPost("refresh")]
        public async Task<ActionResult<UsuarioCadastroResponse>> Refresh()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var usuarioId = identity?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (usuarioId == null)
                return BadRequest();

            var resultado = await _identityService.LoginComRefreshToken(usuarioId);
            if (resultado.Sucesso)
                return Ok(resultado);

            return Unauthorized();
        }
    }
}
