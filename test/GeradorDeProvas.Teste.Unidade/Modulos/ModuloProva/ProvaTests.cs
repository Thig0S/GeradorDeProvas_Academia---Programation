using GeradorDeProvas.Dominio.Modulos.ModuloDisciplina;
using GeradorDeProvas.Dominio.Modulos.ModuloMateria;
using GeradorDeProvas.Dominio.Modulos.ModuloProva;

namespace GeradorDeProvas.Teste.Unidade.Modulos.ModuloProva;

[TestClass]
public class ProvaTests
{
    [TestMethod]
    public void Validar_SemTitulo_DeveRetornar_ErroCorrespondente()
    {
        Disciplina disciplina = new("Matematica");
        Materia materia = new("Algebra", 8, disciplina);

        Prova prova = new(string.Empty, disciplina, materia, 8, 9, false);

        List<string> erros = prova.Validar();

        Assert.HasCount(1, erros);
        Assert.AreEqual("O Campo \"Título\" é obrigatório.",
        erros.First());
    }
    [TestMethod]
    public void Validar_TamanhoMaxTitulo_DeveRetornar_ErroCorrespondente()
    {
        Disciplina disciplina = new("Matematica");
        Materia materia = new("Algebra", 8, disciplina);

        Prova prova = new(new string('a', 101), disciplina, materia, 8, 9, false);

        List<string> erros = prova.Validar();

        Assert.HasCount(1, erros);
        Assert.AreEqual("O campo \"Título\" deve conter entre 2 à 100 caracteres.",
        erros.First());
    }
    [TestMethod]
    public void Validar_SemDisciplina_DeveRetornar_ErroCorrespondente()
    {
        Materia materia = new("Algebra", 8, null);

        Prova prova = new("Prova de Matematica 8a serie", null!, materia, 8, 9, false);

        List<string> erros = prova.Validar();

        Assert.HasCount(1, erros);
        Assert.AreEqual("O campo \"Disciplina\" deve ser preenchido.",
        erros.First());
    }
    [TestMethod]
    public void Validar_ComSerieEMateria_DeveRetornar_ErroCorrespondente()
    {
        Disciplina disciplina = new("Matematica");
        Materia materia = new("Algebra", 8, disciplina);

        Prova prova = new("Prova de Matematica 8a serie", disciplina, materia, 9, 9, false);

        List<string> erros = prova.Validar();

        Assert.HasCount(1, erros);
        Assert.AreEqual("O campo \"Série\" deve alinhar com a série da Materia.",
        erros.First());
    }
    [TestMethod]
    public void Validar_RecuperacaoComMateria_DeveRetornar_ErroCorrespondente()
    {
        Disciplina disciplina = new("Matematica");
        Materia materia = new("Algebra", 8, disciplina);

        Prova prova = new("Prova de Matematica 8a serie", disciplina, materia, 9, 9, true);

        List<string> erros = prova.Validar();

        Assert.HasCount(1, erros);
        Assert.AreEqual("O campo \"Materia\" não deve ser preenchido em uma prova de recuperação.",
        erros.First());
    }
    [TestMethod]
    public void Validar_ComSerieZeroOuAbaixo_DeveRetornar_ErroCorrespondente()
    {
        Disciplina disciplina = new("Matematica");
        Materia materia = new("Algebra", 0, disciplina);

        Prova prova = new("Prova de Matematica 8a serie", disciplina, materia, 0, 9, false);

        List<string> erros = prova.Validar();

        Assert.HasCount(1, erros);
        Assert.AreEqual("O campo \"Série\" deve ser maior que 0.",
        erros.First());
    }

    [TestMethod]
    public void Validar_QuantidadeDeQuestoesAbaxidoDeUm_DeveRetornar_ErroCorrespondente()
    {
        Disciplina disciplina = new("Matematica");
        Materia materia = new("Algebra", 4, disciplina);

        Prova prova = new("Prova de Matematica 8a serie", disciplina, materia, 4, 0, false);

        List<string> erros = prova.Validar();

        Assert.HasCount(1, erros);
        Assert.AreEqual("O campo \"Quantidade De Questões\" deve ser maior que 0.",
        erros.First());
    }
    [TestMethod]
    public void Validar_MateriaFora_DaDisciplina_DeveRetornar_ErroCorrespondente()
    {
        Disciplina disciplina = new("Matematica");
        Disciplina disciplinaForaDaMateria = new("Portugues");

        Materia materia = new("Algebra", 7, disciplina);

        Prova prova = new("Prova de Matematica 8a serie", disciplinaForaDaMateria, materia, 7, 2, false);

        List<string> erros = prova.Validar();

        Assert.HasCount(1, erros);
        Assert.AreEqual("O campo \"Disciplina\" deve alinhar com a Disciplina da Materia.",
        erros.First());
    }
}
