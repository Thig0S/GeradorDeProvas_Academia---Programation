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
}
