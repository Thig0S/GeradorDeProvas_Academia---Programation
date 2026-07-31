using GeradorDeProvas.Infra.Modulos.ModuloDisciplina;
using GeradorDeProvas.Testes.Integracao.Compartilhado;
using FizzWare.NBuilder;
using GeradorDeProvas.Dominio.Modulos.ModuloDisciplina;
using GeradorDeProvas.Dominio.Compartilhado;
using GeradorDeProvas.Testes.Integracao.Compartilhado.Orm;

namespace GeradorDeProvas.Testes.Integracao.ModuloDisciplina;

[TestClass]
public class RepositorioDisciplinaOrmTests : RepositorioBaseEmOrmTests
{
    [TestMethod]
    public void CadastrarESelecionarPorId_CarregaRegistro()
    {
        //arranjo
        Disciplina disciplina = Builder<Disciplina>.CreateNew()
        .With(d => d.UserId = Guid.Empty)
        .Build();

        //acao

        repositorioDisciplina.Cadastrar(disciplina);
        dbContext.ChangeTracker.Clear();

        Disciplina? disciplinaSelecionada = repositorioDisciplina.SelecionarPorId(disciplina.Id);

        //assercao  

        Assert.IsNotNull(disciplinaSelecionada);
    }

    [TestMethod]
    public void Editar_AtualizaRegistroExistente()
    {
        //arranjo
        Disciplina disciplina = Builder<Disciplina>.CreateNew()
        .With(d => d.UserId = Guid.Empty)
        .Persist();

        Disciplina disciplinaAtualizada = Builder<Disciplina>.CreateNew()
        .With(d => d.Nome = "Nome Atualizado")
        .With(d => d.UserId = Guid.Empty)
        .Build();

        dbContext.ChangeTracker.Clear();

        //acao
        bool conseguiuEditar = repositorioDisciplina.Editar(disciplina.Id, disciplinaAtualizada);
        dbContext.ChangeTracker.Clear();

        Disciplina? disciplinaSelecionada = repositorioDisciplina.SelecionarPorId(disciplina.Id);

        //assercao
        Assert.IsTrue(conseguiuEditar);
        Assert.IsNotNull(disciplinaAtualizada);
        Assert.AreEqual("Nome Atualizado", disciplinaAtualizada.Nome);

    }

    [TestMethod]
    public void Excluir_RemoveRegistroExistente()
    {
        //arranjo
        Disciplina disciplina = Builder<Disciplina>.CreateNew()
        .With(d => d.UserId = Guid.Empty)
        .Persist();
        dbContext.ChangeTracker.Clear();

        //acao
        bool conseguiuExcluir = repositorioDisciplina.Excluir(disciplina.Id);
        Disciplina? disciplinaBuscada = repositorioDisciplina.SelecionarPorId(disciplina.Id);
        //assercao

        Assert.IsTrue(conseguiuExcluir);
        Assert.IsNull(disciplinaBuscada);
    }

    [TestMethod]
    public void SelecionarTodos_CarregaRegistro()
    {
        //arranjo
        IList<Disciplina> disciplina = Builder<Disciplina>
        .CreateListOfSize(5)
        .All()
        .With(d => d.UserId = Guid.Empty)
        .Persist();

        dbContext.ChangeTracker.Clear();

        //assercao
        Assert.HasCount(5, repositorioDisciplina.SelecionarTodos());
    }
}
