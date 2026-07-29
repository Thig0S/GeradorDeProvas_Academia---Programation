using FizzWare.NBuilder;
using GeradorDeProvas.Dominio.Modulos.ModuloDisciplina;
using GeradorDeProvas.Dominio.Modulos.ModuloMateria;
using GeradorDeProvas.Infra.Compartilhado.Orm;
using GeradorDeProvas.Infra.Modulos.ModuloDisciplina;
using GeradorDeProvas.Infra.Modulos.ModuloMateria;
using GeradorDeProvas.Infra.Modulos.ModuloProva;
using GeradorDeProvas.Testes.Integracao.Identity;
using Microsoft.EntityFrameworkCore;

namespace GeradorDeProvas.Testes.Integracao.Compartilhado;

public abstract class RepositorioOrmTestBase
{
    //inicializa o repositorio da classe
    protected GeradorDeProvasDbContext dbContext = null!;
    protected RepositorioDisciplinaEmOrm repositorioDisciplina = null!;
    protected RepositorioMateriaEmOrm repositorioMateria = null!;

    //classe para a inicializacao dos testes
    [TestInitialize]
    public void InicializarContexto()
    {
        //atribui o teste para o atributo da classe

        dbContext = CriarDbContext(Guid.NewGuid());
        repositorioDisciplina = new RepositorioDisciplinaEmOrm(dbContext);
        repositorioMateria = new RepositorioMateriaEmOrm(dbContext);

        //Metodo Persist = agora tera a ação de cadastrar no banco de dados, como se fosse um override
        BuilderSetup.SetCreatePersistenceMethod<IList<Disciplina>>(disciplinas =>
        {
            foreach (Disciplina d in disciplinas)
                repositorioDisciplina.Cadastrar(d);

            dbContext.ChangeTracker.Clear();
        });
        BuilderSetup.SetCreatePersistenceMethod<Disciplina>(disciplina =>
        {
            repositorioDisciplina.Cadastrar(disciplina);
        });

        //Metodo persist do repositoMateria

        BuilderSetup.SetCreatePersistenceMethod<IList<Materia>>(materias =>
        {
            foreach (Materia d in materias)
                repositorioMateria.Cadastrar(d);

            dbContext.ChangeTracker.Clear();
        });
        BuilderSetup.SetCreatePersistenceMethod<Materia>(Materia =>
        {
            repositorioMateria.Cadastrar(Materia);
        });
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
