using GeradorDeProvas.Infra.Compartilhado.Orm;
using GeradorDeProvas.Infra.Modulos.ModuloProva;
using GeradorDeProvas.Testes.Integracao.Identity;
using Microsoft.EntityFrameworkCore;

namespace GeradorDeProvas.Testes.Integracao.Compartilhado;

public abstract class RepositorioOrmTestBase
{
    //inicializa o repositorio da classe
    protected GeradorDeProvasDbContext dbContext = null!;

    //classe para a inicializacao dos testes
    [TestInitialize]
    public void InicializarContexto()
    {
        //atribui o teste para o atributo da classe

        dbContext = CriarDbContext(Guid.NewGuid());
    }
    
    //limpa o garbage collector depois de cada teste
    [TestCleanup]
    public void DescartarContexto()
    {
        dbContext.Dispose();
    }
    protected GeradorDeProvasDbContext CriarDbContext(Guid userId)
    {
        DbContextOptions<GeradorDeProvasDbContext> options =
            new DbContextOptionsBuilder<GeradorDeProvasDbContext>()
            .UseInMemoryDatabase("GeradorDeProvasTestDb_Memory")
            .Options;

        return new GeradorDeProvasDbContext(options, new ProvedorDeUsuarioFake(userId));
    }
}
