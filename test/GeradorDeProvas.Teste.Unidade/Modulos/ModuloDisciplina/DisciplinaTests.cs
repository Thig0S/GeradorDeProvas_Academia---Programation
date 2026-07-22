using GeradorDeProvas.Dominio.Modulos.ModuloDisciplina;

namespace GeradorDeProvas.Teste.Unidade.Modulos.ModuloDisciplina;

[TestClass] //Declara que é uma classe de teste
public sealed class DisciplinaTests
{
    #region Teste de validacao
    [TestMethod] //declara que é um metodo de teste
    public void Validar_Com_NomeVazio_DeveRetornarErro()
    {
        //arranjo [configura os dadinhos]
        Disciplina disciplina = new(string.Empty);

        //acao [executa a acao duh]
        List<string> erro = disciplina.Validar();
        //Assercao [checa o resultado comparando com o esperado]
        Assert.HasCount(1, erro);
        Assert.AreEqual("O campo \"Nome\" deve conter entre 2 e 100 caracteres.", erro.First());
    }
    [TestMethod]
    public void Validar_Com_NomeCurto_DeveRetornarErro()
    {
        Disciplina disciplina = new(new string('a', 1));

        //acao [executa a acao duh]
        List<string> erro = disciplina.Validar();
        //Assercao [checa o resultado comparando com o esperado]
        Assert.HasCount(1, erro);
        Assert.AreEqual("O campo \"Nome\" teve ter no mínimo 2 caracteres!", erro.First());
    }
    [TestMethod]
    public void Validar_Com_NomeLongo_DeveRetornarErro()
    {
        Disciplina disciplina = new(new string('a', 101));

        //acao [executa a acao duh]
        List<string> erro = disciplina.Validar();
        //Assercao [checa o resultado comparando com o esperado]
        Assert.HasCount(1, erro);
        Assert.AreEqual("O campo \"Nome\" teve ter no máximo 100 caracteres!", erro.First());
    }
    #endregion
}
