using GeradorDeProvas.Dominio.Modulos.ModuloDisciplina;
using GeradorDeProvas.Dominio.Modulos.ModuloMateria;
using GeradorDeProvas.Dominio.Modulos.ModuloProva;
using GeradorDeProvas.Dominio.Modulos.ModuloQuestao;
using GeradorDeProvas.Infra.Compartilhado.Orm;
using GeradorDeProvas.Infra.Modulos.ModuloProva;
using GeradorDeProvas.Testes.Integracao.Identity;
using Microsoft.EntityFrameworkCore;

namespace GeradorDeProvas.Testes.Integracao.ModuloProva;

[TestClass]
public sealed class RepositorioProvaEmOrmTests
{
    //inicializa o repositorio da classe
    private RepositorioProvaEmOrm repositorio = null!;
    private GeradorDeProvasDbContext dbContext = null!;

    //classe para a inicializacao dos testes
    [TestInitialize]
    public void InicializarRepositorio()
    {
        //atribui o teste para o atributo da classe

        dbContext = CriarDbContext(Guid.NewGuid());
        repositorio = new RepositorioProvaEmOrm(dbContext);
    }
    //limpa o garbage collector depois de cada teste
    [TestCleanup]
    public void LimparContexto()
    {
        dbContext.Dispose();
    }
    [TestMethod]
    public void CadastrarESelecionarPorId_CarregaRelacionamentosDaProva()
    {
        //Arranjo

        Disciplina disciplina = new("Matematica");
        Materia materia = new("Algebra", 7, disciplina);

        Prova prova = new("Prova de Matematica 8a serie", disciplina, materia, 7, 2, false);

        List<Questao> questoesDisponiveis = Enumerable.Range(1, 5)
        .Select(indice => new Questao($"Questão {indice}", materia, [new Alternativa("4", false), new Alternativa("7", true)]))
        .ToList();

        prova.SortearQuestoes(questoesDisponiveis, new Random(70));

        repositorio.Cadastrar(prova);
        dbContext.ChangeTracker.Clear();

        Prova? provaSelecionada = repositorio.SelecionarPorId(prova.Id);

        // Asserção
        Assert.IsNotNull(provaSelecionada);
        Assert.AreEqual("Prova de Matematica 8a serie", provaSelecionada.Titulo);
        Assert.AreEqual(disciplina.Id, provaSelecionada.Disciplina.Id);
        Assert.AreEqual(materia.Id, provaSelecionada.Materia!.Id);
        Assert.HasCount(2, provaSelecionada.Questoes);
    }

    [TestMethod]
    public void Editar_AtualizaProvaExistente()
    {
        Disciplina disciplina = new("Matematica");
        Materia materia = new("Algebra", 7, disciplina);

        Prova prova = new("Prova de Matematica 8a serie", disciplina, materia, 7, 2, false);

        List<Questao> questoesDisponiveis = Enumerable.Range(1, 5)
        .Select(indice => new Questao($"Questão {indice}", materia, [new Alternativa("4", false), new Alternativa("7", true)]))
        .ToList();

        prova.SortearQuestoes(questoesDisponiveis, new Random(70));

        repositorio.Cadastrar(prova);

        Prova provaAtualizada = new("Prova de Algebra Linear", disciplina, null!, 9, 4, true);

        bool conseguiuEditar = repositorio.Editar(prova.Id, provaAtualizada);
        dbContext.ChangeTracker.Clear();

        Assert.IsTrue(conseguiuEditar);
        Assert.AreEqual("Prova de Algebra Linear", repositorio.SelecionarPorId(prova.Id)!.Titulo);


    }

    [TestMethod]
    public void Excluir_RemoveProvaExistente()
    {
        Disciplina disciplina = new("Matematica");
        Materia materia = new("Algebra", 7, disciplina);

        Prova prova = new("Prova de Matematica 8a serie", disciplina, materia, 7, 2, false);

        List<Questao> questoesDisponiveis = Enumerable.Range(1, 5)
        .Select(indice => new Questao($"Questão {indice}", materia, [new Alternativa("4", false), new Alternativa("7", true)]))
        .ToList();

        prova.SortearQuestoes(questoesDisponiveis, new Random(70));

        repositorio.Cadastrar(prova);
        dbContext.ChangeTracker.Clear();

        //act   
        bool consegiuExcluir = repositorio.Excluir(prova.Id);

        Prova? ProvaExcluida = repositorio.SelecionarPorId(prova.Id);

        Assert.IsTrue(consegiuExcluir);
        Assert.IsNull(ProvaExcluida);

    }

    [TestMethod]
    public void SelecionarTodos_RetornaProvasComRelacionamentos()
    {
        Disciplina disciplina = new("Matematica");
        Materia materia = new("Algebra", 7, disciplina);

        Prova prova = new("Prova de Matematica 8a serie", disciplina, materia, 7, 2, false);

        List<Questao> questoesDisponiveis = Enumerable.Range(1, 5)
        .Select(indice => new Questao($"Questão {indice}", materia, [new Alternativa("4", false), new Alternativa("7", true)]))
        .ToList();

        prova.SortearQuestoes(questoesDisponiveis, new Random(70));

        repositorio.Cadastrar(prova);
        dbContext.ChangeTracker.Clear();

        //act
        List<Prova> provas = repositorio.SelecionarTodos();

        Assert.HasCount(1, provas);
        Assert.AreEqual("Matematica", provas.First().Disciplina.Nome);
        Assert.AreEqual("Algebra", provas.First().Materia!.Nome);
        Assert.HasCount(2, provas.First().Questoes);
    }

    private GeradorDeProvasDbContext CriarDbContext(Guid userId)
    {
        DbContextOptions<GeradorDeProvasDbContext> options =
            new DbContextOptionsBuilder<GeradorDeProvasDbContext>()
            .UseInMemoryDatabase("GeradorDeProvasTestDb_Memory")
            .Options;

        return new GeradorDeProvasDbContext(options, new ProvedorDeUsuarioFake(userId));
    }

}
