
using GeradorDeProvas.Dominio.Modulos.ModuloDisciplina;
using GeradorDeProvas.Dominio.Modulos.ModuloMateria;
using GeradorDeProvas.Dominio.Modulos.ModuloQuestao;

namespace GeradorDeProvas.Teste.Unidade.Modulos.ModuloQuestao
{
    [TestClass]
    public class QuestaoTest
    {
        [TestMethod]
        public void Construtor_DeveVincular_CadaAlternativa_AQuestao()
        {
            // arranjo
            Materia materia = new("Algebra", 8, new Disciplina("Matematica"));

            List<Alternativa> alternativas = [new Alternativa("3", false), new Alternativa("4", true)];
            Questao questao = new("Quanto é 2 + 2?", materia, alternativas);

            // acao

            bool alternativasVinculadas = questao.Alternativas.All(a => ReferenceEquals(questao, a.Questao));
            // asserção
            Assert.IsTrue(alternativasVinculadas);

        }
        [TestMethod]
        public void Validar_SemEnunciado_DeveRetornar_ErroCorreposdente()
        {
            Materia materia = new("Algebra", 8, new Disciplina("Matematica"));

            List<Alternativa> alternativas = [new Alternativa("3", false), new Alternativa("4", true)];

            Questao questao = new(string.Empty, materia, alternativas);

            List<string> erros = questao.Validar();

            Assert.HasCount(1, erros);
            Assert.AreEqual("O campo \"Enunciado\" deve ser preenchido!", erros.First());
        }
        [TestMethod]
        public void Validar_SemMateria_DeveRetornar_ErroCorrespondente()
        {
            List<Alternativa> alternativas = [new Alternativa("3", false), new Alternativa("4", true)];

            Questao questao = new("Quanto é 2+2?", null, alternativas);

            List<string> erros = questao.Validar();

            Assert.HasCount(1, erros);
            Assert.AreEqual("O campo \"Matéria\" deve ser preenchido.", erros.First());
        }
        [TestMethod]
        public void Validar_SemAlternativas_DeveRetornar_ErroCorrespondente()
        {
            Materia materia = new("Algebra", 8, new Disciplina("Matematica"));

            Questao questao = new("Quanto é 2 + 2?", materia, []);

            List<string> erros = questao.Validar();

            List<string> errosEsperados = [
                "A questão deve possuir no mínimo duas alternativas.",
                "A questão deve possuir uma alternativa correta."
                ];
            Assert.HasCount(2, erros);
            //compara as coleções de Strings 
            CollectionAssert.AreEqual(
                erros,
                errosEsperados
            );
        }
        [TestMethod]
        public void Validar_PoucasAlternativas_DeveRetornar_ErroCorrespondente()
        {
            List<Alternativa> alternativas = [new Alternativa("4", true)];

            Materia materia = new("Algebra", 8, new Disciplina("Matematica"));

            Questao questao = new("Quanto é 2 + 2?", materia, alternativas);

            List<string> erros = questao.Validar();

            Assert.HasCount(1, erros);
            //compara as coleções de Strings 
            Assert.AreEqual(
                 "A questão deve possuir no mínimo duas alternativas.", erros.First()
            );
        }
        [TestMethod]
        public void Validar_ComMuitasAlternativas_DeveRetornar_ErroCorrespondente()
        {
            List<Alternativa> alternativas = [
                new Alternativa("4", true),
                new Alternativa("10", false),
                new Alternativa("98", false),
                new Alternativa("101", false),
                new Alternativa("2", false)
                ];

            Materia materia = new("Algebra", 8, new Disciplina("Matematica"));

            Questao questao = new("Quanto é 2 + 2?", materia, alternativas);

            List<string> erros = questao.Validar();

            Assert.HasCount(1, erros);

            Assert.AreEqual(
                "A questão deve possuir no máximo quatro alternativas.", erros.First()
            );
        }
        [TestMethod]
        public void Validar_SemAlternativaCorreta_DeveRetornarErro_Correspondente()
        {
            List<Alternativa> alternativas = [
                new Alternativa("4", false), new Alternativa("10", false),
                new Alternativa("98", false), new Alternativa("101", false),
                ];

            Materia materia = new("Algebra", 8, new Disciplina("Matematica"));

            Questao questao = new("Quanto é 2 + 2?", materia, alternativas);

            List<string> erros = questao.Validar();


            Assert.HasCount(1, erros);

            Assert.AreEqual(
                "A questão deve possuir uma alternativa correta.", erros.First()
            );
        }
        public void Validar_ComMuitasAlternativasCorretas_DeveRetornarErro_Correspondente()
        {
            List<Alternativa> alternativas = [
                new Alternativa("4", true), new Alternativa("10", false),
                new Alternativa("98", true), new Alternativa("101", false),
                ];

            Materia materia = new("Algebra", 8, new Disciplina("Matematica"));

            Questao questao = new("Quanto é 2 + 2?", materia, alternativas);

            List<string> erros = questao.Validar();


            Assert.HasCount(1, erros);

            Assert.AreEqual(
                "A questão deve possuir apenas uma alternativa correta.", erros.First()
            );
        }
    }
}