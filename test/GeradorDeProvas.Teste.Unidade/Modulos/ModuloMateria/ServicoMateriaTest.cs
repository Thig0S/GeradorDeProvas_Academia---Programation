using GeradorDeProvas.Aplicacao.Modulos.ModuloMateria;
using GeradorDeProvas.Dominio.Modulos.ModuloDisciplina;
using GeradorDeProvas.Dominio.Modulos.ModuloMateria;
using GeradorDeProvas.Dominio.Modulos.ModuloQuestao;
using Moq;
using FizzWare.NBuilder;
using FluentResults;
namespace GeradorDeProvas.Teste.Unidade.Modulos.ModuloMateria;

[TestClass]
public sealed class ServicoMateriaTest
{
    [TestMethod]
    public void Cadastrar_Dados_Validos_Persiste()
    {
        // Arrange

        Mock<IRepositorioMateria> repositorioMateria = new();
        Mock<IRepositorioDisciplina> repositorioDisciplina = new();
        Mock<IRepositorioQuestao> repositorioQuestao = new();

        repositorioMateria.Setup(r => r.SelecionarTodos()).Returns([]);

        ServicoMateria servicoMateria = new
            (repositorioMateria.Object,
            repositorioDisciplina.Object,
            repositorioQuestao.Object);

        Disciplina disciplina = Builder<Disciplina>.CreateNew()
            .With(e => e.UserId = Guid.Empty).Build();

        repositorioDisciplina.Setup(r => r.SelecionarPorId(disciplina.Id)).Returns(disciplina);

        Materia? materiaCadastrada = null!;

        repositorioMateria.Setup(r =>
            r.Cadastrar(It.IsAny<Materia>())).Callback<Materia>(materia => materiaCadastrada = materia);

        //Act
        Result resultado = servicoMateria.Cadastrar(new CadastrarMateriaDto("Matematica", 9, disciplina.Id));

        Assert.IsTrue(resultado.IsSuccess);
        Assert.IsNotNull(materiaCadastrada);

        repositorioMateria.Verify(r => r.Cadastrar(It.IsAny<Materia>()), Times.Once);
    }
    [TestMethod]
    public void Cadastrar_NomeDuplicado_RetornaErro()
    {
        Mock<IRepositorioMateria> repositorioMateria = new();
        Mock<IRepositorioDisciplina> repositorioDisciplina = new();
        Mock<IRepositorioQuestao> repositorioQuestao = new();

        Materia materiaNomeDuplicado = Builder<Materia>.CreateNew()
        .With(m => m.Nome = "Matematica").Build();

        repositorioMateria.Setup(r => r.SelecionarTodos()).Returns([materiaNomeDuplicado]);

        ServicoMateria servicoMateria = new
            (repositorioMateria.Object,
            repositorioDisciplina.Object,
            repositorioQuestao.Object);

        Disciplina disciplina = Builder<Disciplina>.CreateNew()
            .With(e => e.UserId = Guid.Empty).Build();

        repositorioDisciplina.Setup(r => r.SelecionarPorId(disciplina.Id)).Returns(disciplina);

        Materia? materiaCadastrada = null!;

        repositorioMateria.Setup(r =>
            r.Cadastrar(It.IsAny<Materia>())).Callback<Materia>(materia => materiaCadastrada = materia);

        //Act
        Result resultado = servicoMateria.Cadastrar(new CadastrarMateriaDto("Matematica", 9, disciplina.Id));

        Assert.IsTrue(resultado.IsFailed);
        Assert.Contains("Já existe", resultado.Errors.First().Message);

        repositorioMateria.Verify(r => r.Cadastrar(It.IsAny<Materia>()), Times.Never);
    }
    [TestMethod]
    public void ExcluirRegistro_Materia_Relacionadas()
    {
        Mock<IRepositorioMateria> repositorioMateria = new();
        Mock<IRepositorioDisciplina> repositorioDisciplina = new();
        Mock<IRepositorioQuestao> repositorioQuestao = new();

        ServicoMateria servicoMateria = new
            (repositorioMateria.Object,
            repositorioDisciplina.Object,
            repositorioQuestao.Object);

        Materia materia = Builder<Materia>.CreateNew().
        Build();

        repositorioMateria.Setup(r => r.SelecionarPorId(materia.Id)).Returns(materia);
        repositorioQuestao.Setup(r => r.SelecionarTodos()).Returns([]);

        Result resultado = servicoMateria.Excluir(materia.Id);

        Assert.IsTrue(resultado.IsSuccess);
        repositorioMateria.Verify(r => r.Excluir(materia.Id), Times.Once);
    }
    [TestMethod]
    public void ExcluirRegistro_ComQuestoes_Relacionadas()
    {
        Mock<IRepositorioMateria> repositorioMateria = new();
        Mock<IRepositorioDisciplina> repositorioDisciplina = new();
        Mock<IRepositorioQuestao> repositorioQuestao = new();

        ServicoMateria servicoMateria = new
            (repositorioMateria.Object,
            repositorioDisciplina.Object,
            repositorioQuestao.Object);

        Materia materia = Builder<Materia>.CreateNew().
        Build();
        Questao questaoVinculada = Builder<Questao>.CreateNew()
        .With(q => q.Materia = materia).
        Build();

        repositorioMateria.Setup(r => r.SelecionarPorId(materia.Id)).Returns(materia);
        repositorioQuestao.Setup(r => r.SelecionarTodos()).Returns([questaoVinculada]);

        Result resultado = servicoMateria.Excluir(materia.Id);

        Assert.IsTrue(resultado.IsFailed);
        Assert.Contains("vinculadas", resultado.Errors.First().Message);
        repositorioMateria.Verify(r => r.Excluir(materia.Id), Times.Never);
    }
    [TestMethod]
    public void Editar_ComDadosValidos_Persiste()
    {
        Mock<IRepositorioMateria> repositorioMateria = new();
        Mock<IRepositorioDisciplina> repositorioDisciplina = new();
        Mock<IRepositorioQuestao> repositorioQuestao = new();

        ServicoMateria servicoMateria = new
            (repositorioMateria.Object,
            repositorioDisciplina.Object,
            repositorioQuestao.Object);

        
    }
}
