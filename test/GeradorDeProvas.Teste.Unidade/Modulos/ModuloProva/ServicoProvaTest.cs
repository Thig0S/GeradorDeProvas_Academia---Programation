using FluentResults;
using GeradorDeProvas.Aplicacao.Modulos.ModuloProva;
using GeradorDeProvas.Dominio.Modulos.ModuloDisciplina;
using GeradorDeProvas.Dominio.Modulos.ModuloMateria;
using GeradorDeProvas.Dominio.Modulos.ModuloProva;
using GeradorDeProvas.Dominio.Modulos.ModuloQuestao;
using Moq;

namespace GeradorDeProvas.Teste.Unidade.Modulos.ModuloProva;

[TestClass]
public class ServicoProvaTest
{
    [TestMethod]
    public void Cadastrar_ConfigracaoValida_CadastraProvaComQuestoesSelecionadas()
    {
        Disciplina disciplina = new("Matematica");
        Materia materia = new("Álgebra", 7, disciplina);

        Questao primeiraQuestao = CriarQuestao(materia, "Quanto é 2 + 2?");
        Questao segundaQuestao = CriarQuestao(materia, "Quanto é 3 + 3?");

        Mock<IRepositorioProva> repositorioProva = new();
        Mock<IRepositorioMateria> repositorioMateria = new();
        Mock<IRepositorioDisciplina> repositorioDisciplina = new();
        Mock<IRepositorioQuestao> repositorioQuestao = new();

        ServicoProva servicoProva = new(
            repositorioProva.Object,
            repositorioDisciplina.Object,
            repositorioMateria.Object,
            repositorioQuestao.Object);

        repositorioProva.Setup(r => r.SelecionarTodos()).Returns([]);
        repositorioDisciplina.Setup(r => r.SelecionarPorId(disciplina.Id)).Returns(disciplina);
        repositorioMateria.Setup(r => r.SelecionarPorId(materia.Id)).Returns(materia);
        repositorioMateria.Setup(r => r.SelecionarTodos()).Returns([materia]);
        repositorioQuestao.Setup(r => r.SelecionarTodos()).Returns([primeiraQuestao, segundaQuestao]);

        Prova? provaCadastrada = null;

        repositorioProva.Setup(r => r.Cadastrar(It.IsAny<Prova>())).Callback<Prova>(prova => provaCadastrada = prova);

        CadastrarProvaDto dto = new("Avaliacao", disciplina.Id, materia.Id, 7, 2, false);
        Result resultado = servicoProva.Cadastrar(dto, [primeiraQuestao.Id, segundaQuestao.Id]);

        Assert.IsTrue(resultado.IsSuccess);
        Assert.IsNotNull(provaCadastrada);
        Assert.AreEqual("Avaliacao", provaCadastrada.Titulo);
        Assert.HasCount(2, provaCadastrada.Questoes);
        repositorioProva.Verify(r => r.Cadastrar(It.IsAny<Prova>()), Times.Once);
    }
    [TestMethod]
    public void Cadastar_TituloDuplicado_RetornaFalha()
    {
        Disciplina disciplina = new("Matematica");

        Mock<IRepositorioProva> repositorioProva = new();
        Mock<IRepositorioMateria> repositorioMateria = new();
        Mock<IRepositorioDisciplina> repositorioDisciplina = new();
        Mock<IRepositorioQuestao> repositorioQuestao = new();

        ServicoProva servicoProva = new(
            repositorioProva.Object,
            repositorioDisciplina.Object,
            repositorioMateria.Object,
            repositorioQuestao.Object);

        repositorioProva.Setup(r => r.SelecionarTodos()).Returns([new Prova("Avaliacao", disciplina, null, 7, 1, true)]);

        Result resultado = servicoProva.Cadastrar(new CadastrarProvaDto("Avaliacao", disciplina.Id, null, 7, 10, true));

        Assert.IsTrue(resultado.IsFailed);
        Assert.Contains("Já existe", resultado.Errors.First().Message);
    }
    [TestMethod]
    public void Cadastrar_MateriaDeOutraDisciplina_RetornaFalha()
    {
        // Arrange
        Disciplina disciplina = new("Matemática");
        Disciplina outraDisciplina = new("História");

        Materia materia = new("Álgebra", 7, outraDisciplina);

        Mock<IRepositorioProva> repositorioProva = new();
        Mock<IRepositorioDisciplina> repositorioDisciplina = new();
        Mock<IRepositorioMateria> repositorioMateria = new();
        Mock<IRepositorioQuestao> repositorioQuestao = new();

        repositorioProva.Setup(r => r.SelecionarTodos()).Returns([]);

        repositorioDisciplina.Setup(r => r.SelecionarPorId(disciplina.Id)).Returns(disciplina);
        repositorioMateria.Setup(r => r.SelecionarPorId(materia.Id)).Returns(materia);

        ServicoProva servico = new(
            repositorioProva.Object,
            repositorioDisciplina.Object,
            repositorioMateria.Object,
            repositorioQuestao.Object
        );

        // Act
        Result resultado = servico.Cadastrar(
            new CadastrarProvaDto("Avaliação", disciplina.Id, materia.Id, 7, 1, false)
        );

        // Assert
        Assert.IsTrue(resultado.IsFailed);
        Assert.Contains("não pertence à disciplina", resultado.Errors.Single().Message);
        repositorioProva.Verify(r => r.Cadastrar(It.IsAny<Prova>()), Times.Never);
    }

    [TestMethod]
    public void Cadastrar_QuestoesForaDaConfiguracao_RetornaFalha()
    {
        // Arrange
        Disciplina disciplina = new("Matemática");

        Materia materia = new("Álgebra", 7, disciplina);
        Materia outraMateria = new("Geometria", 8, disciplina);

        Questao questaoDeOutraMateria = CriarQuestao(outraMateria, "Questão fora da matéria");

        Mock<IRepositorioProva> repositorioProva = new();
        Mock<IRepositorioDisciplina> repositorioDisciplina = new();
        Mock<IRepositorioMateria> repositorioMateria = new();
        Mock<IRepositorioQuestao> repositorioQuestao = new();

        ServicoProva servico = new(
            repositorioProva.Object,
            repositorioDisciplina.Object,
            repositorioMateria.Object,
            repositorioQuestao.Object
        );

        repositorioProva.Setup(r => r.SelecionarTodos()).Returns([]);

        repositorioDisciplina.Setup(r => r.SelecionarPorId(disciplina.Id)).Returns(disciplina);

        repositorioMateria.Setup(r => r.SelecionarPorId(materia.Id)).Returns(materia);
        repositorioMateria.Setup(r => r.SelecionarTodos()).Returns([materia]);

        repositorioQuestao.Setup(r => r.SelecionarTodos()).Returns([questaoDeOutraMateria]);

        // Act
        Result resultado = servico.Cadastrar(
            new CadastrarProvaDto("Avaliação", disciplina.Id, materia.Id, 7, 1, false),
            [questaoDeOutraMateria.Id]
        );

        // Assert
        Assert.IsTrue(resultado.IsFailed);
        Assert.Contains("não pertencem à configuração", resultado.Errors.Single().Message);
        repositorioProva.Verify(r => r.Cadastrar(It.IsAny<Prova>()), Times.Never);
    }

    [TestMethod]
    public void Duplicar_ProvaExistente_CadastraCopiaComNovoTitulo()
    {
        // Arrange
        Disciplina disciplina = new("Matemática");
        Materia materia = new("Álgebra", 7, disciplina);

        Prova provaOriginal = new("Avaliação original", disciplina, materia, 7, 2, false);

        Mock<IRepositorioProva> repositorioProva = new();
        Mock<IRepositorioDisciplina> repositorioDisciplina = new();
        Mock<IRepositorioMateria> repositorioMateria = new();
        Mock<IRepositorioQuestao> repositorioQuestao = new();

        repositorioProva.Setup(r => r.SelecionarTodos()).Returns([]);
        repositorioProva.Setup(r => r.SelecionarPorId(provaOriginal.Id)).Returns(provaOriginal);

        Prova? provaDuplicada = null;

        repositorioProva
            .Setup(r => r.Cadastrar(It.IsAny<Prova>()))
            .Callback<Prova>(prova => provaDuplicada = prova);

        ServicoProva servico = new(
            repositorioProva.Object,
            repositorioDisciplina.Object,
            repositorioMateria.Object,
            repositorioQuestao.Object
        );

        // Act
        Result resultado = servico.Duplicar(new DuplicarProvaDto(provaOriginal.Id, "Avaliação cópia"));

        // Assert
        Assert.IsTrue(resultado.IsSuccess);
        Assert.IsNotNull(provaDuplicada);
        Assert.AreEqual("Avaliação cópia", provaDuplicada.Titulo);
        Assert.AreSame(disciplina, provaDuplicada.Disciplina);
        Assert.AreSame(materia, provaDuplicada.Materia);
        Assert.AreEqual(provaOriginal.Serie, provaDuplicada.Serie);
        Assert.AreEqual(provaOriginal.QuantidadeDeQuestoes, provaDuplicada.QuantidadeDeQuestoes);
        repositorioProva.Verify(r => r.Cadastrar(It.IsAny<Prova>()), Times.Once);
    }

    [TestMethod]
    public void Excluir_ProvaInexistente_RetornaFalha()
    {
        // Arrange
        Guid provaId = Guid.CreateVersion7();

        Mock<IRepositorioProva> repositorioProva = new();
        Mock<IRepositorioDisciplina> repositorioDisciplina = new();
        Mock<IRepositorioMateria> repositorioMateria = new();
        Mock<IRepositorioQuestao> repositorioQuestao = new();

        ServicoProva servico = new(
            repositorioProva.Object,
            repositorioDisciplina.Object,
            repositorioMateria.Object,
            repositorioQuestao.Object
        );

        // Act
        Result resultado = servico.Excluir(provaId);

        // Assert
        Assert.IsTrue(resultado.IsFailed);
        Assert.Contains("Prova não encontrada", resultado.Errors.Single().Message);

        repositorioProva.Verify(r => r.Excluir(It.IsAny<Guid>()), Times.Once);
    }

    private static Questao CriarQuestao(Materia materia, string enunciado)
    {
        return new Questao(
            enunciado,
            materia,
            [new Alternativa("Resposta correta", true), new Alternativa("Resposta errada", false)]
        );
    }
}
