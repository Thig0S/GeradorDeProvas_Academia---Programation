using System.Security.Cryptography.X509Certificates;
using GeradorDeProvas.Dominio.Modulos.ModuloDisciplina;
using GeradorDeProvas.Dominio.Modulos.ModuloMateria;

namespace GeradorDeProvas.Teste.Unidade.Modulos.ModuloMateria;

[TestClass]
public sealed class MateriaTests
{
    [TestMethod]
    public void Validar_SemNome_RetornaErrosCorrespondentes()
    {
        Disciplina disciplina = new("Matematica");
        Materia materia = new(string.Empty, 5, disciplina);

        List<string> erros = materia.Validar();

        Assert.HasCount(1, erros);
        Assert.AreEqual("O campo \"Nome\" deve conter entre 2 e 100 caracteres.", erros.First());
    }
    [TestMethod]
    public void Validar_SemSerie_RetornaErrosCorrespondentes()
    {
        Disciplina disciplina = new("Matematica");
        Materia materia = new("Matematica", 0, disciplina);

        List<string> erros = materia.Validar();

        Assert.HasCount(1, erros);
        Assert.AreEqual("O campo \"Série\" deve ser preenchido.", erros.First());
    }
    [TestMethod]
    public void Validar_SemDisciplina_RetornaErrosCorrespondentes()
    {
        Materia materia = new("Matematica", 1, null);

        List<string> erros = materia.Validar();

        Assert.HasCount(1, erros);
        Assert.AreEqual("O campo \"Disciplina\" deve ser preenchido.", erros.First());
    }
    [TestMethod]
    public void Atualiza_DeveAtualizar_Nome_Serie_Disciplina()
    {
        Disciplina disciplina = new("Matematica");
        Materia materia = new("Operações basicas", 5, disciplina);

        Disciplina disciplinaAtualizada = new("Historia");
        Materia materiaAtualizada = new("Independencia do Brasil", 7, disciplinaAtualizada);

        materia.Atualizar(materiaAtualizada);

        Assert.AreEqual("Independencia do Brasil", materia.Nome);
        Assert.AreEqual(7, materia.Serie);
        Assert.AreSame(disciplinaAtualizada, materia.Disciplina);
    }
}
