using FluentResults;
using GeradorDeProvas.Aplicacao.Modulos.ModuloDisciplina;
using GeradorDeProvas.Dominio.Modulos.ModuloDisciplina;
using GeradorDeProvas.Dominio.Modulos.ModuloMateria;
using Moq;

namespace GeradorDeProvas.Teste.Unidade.Modulos.ModuloDisciplina;

[TestClass]
public class ServicoDisciplinaTest
{
    [TestMethod]
    public void CadastrarDadosValidos_Persiste()
    {
        // Arrange
        Mock<IRepositorioDisciplina> repositorioDisciplina = new Mock<IRepositorioDisciplina>();
        Mock<IRepositorioMateria> repositorioMateria = new Mock<IRepositorioMateria>();

        repositorioDisciplina.Setup(r => r.SelecionarTodos()).Returns([]);

        Disciplina? disciplinaCadastrada = null;

        repositorioDisciplina.Setup(r => r.Cadastrar(It.IsAny<Disciplina>())).Callback<Disciplina>(disciplina => disciplinaCadastrada = disciplina);

        ServicoDisciplina servicoDisciplina = new(repositorioDisciplina.Object, repositorioMateria.Object);

        // Act
        Result resultado = servicoDisciplina.Cadastrar(new CadastrarDisciplinaDto("Matematica"));

        // Assert
        Assert.IsTrue(resultado.IsSuccess);
        Assert.IsNotNull(disciplinaCadastrada);
        Assert.AreEqual("Matematica", disciplinaCadastrada.Nome);

        repositorioDisciplina.Verify(r => r.Cadastrar(It.IsAny<Disciplina>()), Times.Once);
    }
    [TestMethod]
    public void Cadastrar_NomeDuplicado_RetornaErro()
    {
        Mock<IRepositorioDisciplina> repositorioDisciplina = new Mock<IRepositorioDisciplina>();
        Mock<IRepositorioMateria> repositorioMateria = new Mock<IRepositorioMateria>();

        repositorioDisciplina.Setup(r => r.SelecionarTodos()).Returns([new Disciplina("Matematica")]);

        ServicoDisciplina servicoDisciplina = new(repositorioDisciplina.Object, repositorioMateria.Object);


        Result resultado =
            servicoDisciplina.Cadastrar(new CadastrarDisciplinaDto("Matematica"));

        Assert.IsTrue(resultado.IsFailed);
        Assert.AreEqual("Nome", resultado.Errors.Single().Metadata["Campo"]);
        Assert.Contains("Já existe", resultado.Errors.Single().Message);

        repositorioDisciplina.Verify(r => r.Cadastrar(It.IsAny<Disciplina>()), Times.Never);

    }
    [TestMethod]
    public void ExcluirDisciplina_RemoveRegistro()
    {
        Mock<IRepositorioDisciplina> repositorioDisciplina = new Mock<IRepositorioDisciplina>();
        Mock<IRepositorioMateria> repositorioMateria = new Mock<IRepositorioMateria>();

        Disciplina disciplina = new("Matematica");

        repositorioDisciplina.Setup(r => r.SelecionarPorId(disciplina.Id)).Returns(disciplina);

        repositorioMateria.Setup(r => r.SelecionarTodos()).Returns([]);

        ServicoDisciplina servicoDisciplina = new(repositorioDisciplina.Object, repositorioMateria.Object);

        Result resultado =
            servicoDisciplina.Excluir(disciplina.Id);

        Assert.IsTrue(resultado.IsSuccess);

        repositorioDisciplina.Verify(r => r.Excluir(disciplina.Id), Times.Once);
    }


    [TestMethod]
    public void ExcluirDisciplina_ComMateriasVinculadas_RetornaFalha()
    {
        Mock<IRepositorioDisciplina> repositorioDisciplina = new Mock<IRepositorioDisciplina>();
        Mock<IRepositorioMateria> repositorioMateria = new Mock<IRepositorioMateria>();

        Disciplina disciplina = new("Matematica");

        repositorioDisciplina.Setup(r => r.SelecionarPorId(disciplina.Id)).Returns(disciplina);

        repositorioMateria.Setup(r => r.SelecionarTodos()).Returns([new Materia("Algebra", 7, disciplina)]);

        ServicoDisciplina servicoDisciplina = new(repositorioDisciplina.Object, repositorioMateria.Object);

        Result resultado =
            servicoDisciplina.Excluir(disciplina.Id);

        Assert.IsTrue(resultado.IsFailed);
        Assert.Contains("matérias vinculadas", resultado.Errors.First().Message);

        repositorioDisciplina.Verify(r => r.Excluir(disciplina.Id), Times.Never);
    }
}
