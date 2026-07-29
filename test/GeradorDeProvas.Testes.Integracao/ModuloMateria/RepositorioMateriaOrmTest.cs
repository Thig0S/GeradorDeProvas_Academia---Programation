using FizzWare.NBuilder;
using GeradorDeProvas.Dominio.Modulos.ModuloDisciplina;
using GeradorDeProvas.Dominio.Modulos.ModuloMateria;
using GeradorDeProvas.Testes.Integracao.Compartilhado;

namespace GeradorDeProvas.Testes.Integracao.ModuloMateria;

[TestClass]
public class RepositorioMateriaOrmTest : RepositorioOrmTestBase
{
    [TestMethod]
    public void Cadastrar_Materia_E_SelecionarPorId()
    {
        Disciplina disciplina = Builder<Disciplina>.CreateNew()
        .With(d => d.UserId = Guid.Empty).Persist();

        Materia materia = Builder<Materia>.CreateNew()
        .With(m => m.UserId = Guid.Empty)
        .With(m => m.Disciplina = disciplina).Build();

        repositorioMateria.Cadastrar(materia);
        dbContext.ChangeTracker.Clear();

        Assert.IsNotNull(repositorioMateria.SelecionarPorId(materia.Id));
    }

    [TestMethod]
    public void Editar_AtualizaMateriaExistente()
    {

        Disciplina disciplina = Builder<Disciplina>.CreateNew()
        .With(d => d.UserId = Guid.Empty).Persist();

        Materia materia = Builder<Materia>.CreateNew()
        .With(m => m.UserId = Guid.Empty)
        .With(m => m.Disciplina = disciplina).Persist();

        Materia materiaAtualizada = Builder<Materia>.CreateNew()
        .With(m => m.UserId = Guid.Empty)
        .With(m => m.Nome = "Matematica")
        .With(m => m.Disciplina = disciplina).Persist();

        bool conseguiuEditar = repositorioMateria.Editar(materia.Id, materiaAtualizada);
        dbContext.ChangeTracker.Clear();

        Assert.IsTrue(conseguiuEditar);
        Assert.AreEqual("Matematica", materia.Nome);

    }

    [TestMethod]
    public void Excluir_RemoveMateriaExistente()
    {
        Disciplina disciplina = Builder<Disciplina>.CreateNew()
        .With(d => d.UserId = Guid.Empty).Persist();

        Materia materia = Builder<Materia>.CreateNew()
        .With(m => m.UserId = Guid.Empty)
        .With(m => m.Disciplina = disciplina).Persist();

        bool consegiuExcluir = repositorioMateria.Excluir(materia.Id);
        dbContext.ChangeTracker.Clear();

        Assert.IsNull(repositorioMateria.SelecionarPorId(materia.Id));
        Assert.IsTrue(consegiuExcluir);
    }

    [TestMethod]
    public void SelecionarTodos_RetornaMateriaComRelacionamentos()
    {
        // Arranjo (Arrange)
        Disciplina disciplina = new Disciplina("Nome1");
        Materia materia = new Materia("Nome1", 1, disciplina);

        repositorioMateria.Cadastrar(materia);
        dbContext.ChangeTracker.Clear();

        // Ação (Act)
        List<Materia> materias = repositorioMateria.SelecionarTodos();
        Materia materiaSelecionada = materias.First(); // Ou o índice correspondente ao item selecionado

        // Asserção (Assert)
        Assert.HasCount(1, materias);
        Assert.AreEqual("Nome1", materiaSelecionada.Nome);
        Assert.AreEqual(1, materiaSelecionada.Serie);
        Assert.AreEqual(disciplina.Nome, materiaSelecionada.Disciplina.Nome);
    }
}
