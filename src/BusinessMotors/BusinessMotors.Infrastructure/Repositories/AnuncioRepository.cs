using BusinessMotors.Application.Queries;

namespace BusinessMotors.Infrastructure.Repositories
{
    public class AnuncioRepository : Repository<Anuncio>, IAnuncioRepository
    {
        protected DbSet<Anuncio> _dbSet;
        protected BusinessMotorsDBContext _dbContext;
        private readonly UserManager<Usuario> _userManager;

        public AnuncioRepository(BusinessMotorsDBContext dbContext, UserManager<Usuario> userManager) 
            : base(dbContext) => (_dbSet, _dbContext, _userManager) = (dbContext.Set<Anuncio>(), dbContext, userManager);
        public async Task<List<Anuncio>> GetListByIdAsync(List<Guid> ids) => await _dbSet.Where(d => ids.Contains(d.Id)).ToListAsync();
        public async Task<Anuncio> GetByIdAsync(Guid id) => await _dbSet.IgnoreAutoIncludes()
                                                                        .Include(d => d.Caracteristicas)
                                                                        .Include(d => d.Opcionais)
                                                                        .Include(d => d.TiposCombustiveis)
                                                                        .Include(d => d.Modelo)
                                                                            .ThenInclude(d => d.Marca)
                                                                        .Include(d => d.Versao)
                                                                        .Include(d => d.ImagensS3)
                                                                        .FirstOrDefaultAsync(d => d.Id == id);

        public async Task<List<Anuncio>> GetAllAsync(GetAnunciosQuery.Anuncios querie)
        {
            List<Usuario> users = new List<Usuario>();
            if (!string.IsNullOrEmpty(querie.role))
            {
                users = _userManager.GetUsersInRoleAsync(querie.role).Result.ToList();
            }
            
            return await _dbSet.IgnoreAutoIncludes()
                                                                .Include(d => d.Modelo)
                                                                    .ThenInclude(d => d.Marca)
                                                                .Include(d => d.Versao)
                                                                .Include(d => d.ImagensS3)
                                                                .OrderByDescending(d => d.DataCriacao)
                                                                .Where(d => 
                                                                        (string.IsNullOrEmpty(querie.estado) || d.Estado.Equals(querie.estado)) && 
                                                                        (!querie.precoInicio.HasValue || d.Preco >= querie.precoInicio) &&
                                                                        (!querie.precoFim.HasValue || d.Preco <= querie.precoFim) &&
                                                                        (!querie.kmInicio.HasValue || Convert.ToInt32(d.Km) >= querie.kmInicio) &&
                                                                        (!querie.kmFim.HasValue || Convert.ToInt32(d.Km) <= querie.kmFim) &&
                                                                        (!querie.anoModeloInicio.HasValue || d.AnoVeiculo >= querie.anoModeloInicio) &&
                                                                        (!querie.anoModeloFim.HasValue || d.AnoVeiculo <= querie.anoModeloFim) &&
                                                                        (!querie.idModelo.HasValue || d.Modelo.Id >= querie.idModelo) &&
                                                                        (!querie.idMarca.HasValue || d.Modelo.Marca.Id == querie.idMarca) &&
                                                                        (!querie.idVersao.HasValue || d.Modelo.Versoes.Any(d => d.Id == querie.idVersao)) &&
                                                                        (!users.Any() || users.Contains(d.User))
                                                                      )
                                                                .Skip(querie.skip)
                                                                .Take(querie.take)
                                                                .ToListAsync();
        }
                                                    
    }
}