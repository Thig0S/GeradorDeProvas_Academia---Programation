using GeradorDeProvas.Dominio.Modulos.ModuloDisciplina;

namespace GeradorDeProvas.Teste.Unidade.Modulos.ModuloDisciplina;

[TestClass]
public sealed class DisciplinaTests
{
    #region Teste de validacao
    [TestMethod]
    public void Validar_Com_NomeVazio_DeveRetornarErro()
    {
        //arranjo [configura os dadinhos]
        Disciplina disciplina = new(string.Empty);

        //acao [executa a acao duh]
        List<string> erro = disciplina.Validar();
        //Assercao [checa o resultado comparando com o esperado]
        Assert.HasCount(1, erro);
    }
    [TestMethod]
    public void Validar_Com_NomeCurto_DeveRetornarErro()
    {
        Disciplina disciplina = new(new string('a', 1));

        //acao [executa a acao duh]
        List<string> erro = disciplina.Validar();
        //Assercao [checa o resultado comparando com o esperado]
        Assert.HasCount(1, erro);
    }
    [TestMethod]
    public void Validar_Com_NomeLongo_DeveRetornarErro()
    {
        Disciplina disciplina = new(new string('a', 101));

        //acao [executa a acao duh]
        List<string> erro = disciplina.Validar();
        //Assercao [checa o resultado comparando com o esperado]
        Assert.HasCount(1, erro);
    }
    #endregion
}
