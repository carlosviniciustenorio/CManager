using System.Net;

namespace BusinessMotors.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnunciosController : ControllerBase
    {
        private readonly IMediator _mediatr;
        
        public AnunciosController(IMediator mediatr)
        {
            _mediatr = mediatr;
        }

        /// <summary>
        /// Cadastra um anúncio
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <returns></returns>
        /// <response code="201">Criado</response>
        /// <response code="500">Retorna erros caso ocorram</response>
        [ProducesResponseType(typeof(Unit), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [HttpPost]
        [Authorize]
        public async Task<HttpStatusCode> Create([FromForm]AddAnuncioCommand.Command command)
        {
            if (string.IsNullOrEmpty(command.usuarioId))
            {
                var userId = User.Claims.FirstOrDefault(d => d.Type.Contains("nameidentifier"))?.Value; 
                command.usuarioId = userId;
            }

            await _mediatr.Send(command);    
            return HttpStatusCode.Created;
        } 

        /// <summary>
        /// Retorna todos os anuncios cadastrados
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <returns></returns>
        /// <response code="200">Retorna todas os anuncios</response>
        /// <response code="500">Retorna erros caso ocorram</response>
        [ProducesResponseType(typeof(List<AnunciosResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [HttpGet]
        public async Task<List<AnunciosResponse>> Get([FromQuery]GetAnunciosQuery.Anuncios command) => await _mediatr.Send(command);    
        
        /// <summary>
        /// Retorna um anúncio com o ID fornecido.
        /// </summary>
        /// <remarks>
        /// Esta operação retorna um único anúncio com o ID fornecido, se existir.
        /// </remarks>
        /// <param name="id">O ID único do anúncio.</param>
        /// <returns>O objeto AnuncioResponse que representa o anúncio solicitado.</returns>
        /// <response code="200">Retorna o anúncio com o ID fornecido.</response>
        /// <response code="404">Se o anúncio com o ID fornecido não for encontrado.</response>
        /// <response code="500">Se ocorrer um erro interno do servidor.</response>
        [ProducesResponseType(typeof(AnuncioResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [HttpGet("{id}")]
        public async Task<AnuncioResponse> Get([FromRoute]GetAnuncioQuery.Anuncio command) => await _mediatr.Send(command);    
    }
}