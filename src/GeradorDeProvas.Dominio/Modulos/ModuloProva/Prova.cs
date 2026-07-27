using GeradorDeProvas.Dominio.Compartilhado;
using GeradorDeProvas.Dominio.Compartilhado.Identity;
using GeradorDeProvas.Dominio.Modulos.ModuloDisciplina;
using GeradorDeProvas.Dominio.Modulos.ModuloMateria;
using GeradorDeProvas.Dominio.Modulos.ModuloQuestao;

namespace GeradorDeProvas.Dominio.Modulos.ModuloProva;

public sealed class Prova : EntidadeBase<Prova>, IEntidadeDoUsuario
{
    public string Titulo { get; set; } = string.Empty;
    public Disciplina Disciplina { get; set; } = null!;
    public Materia? Materia { get; set; }
    public int Serie { get; set; }
    public int QuantidadeDeQuestoes { get; set; }
    public bool ProvaRecuperacao { get; set; }
    public List<Questao> Questoes { get; set; } = [];
    public Guid UserId { get; set; }

    public Prova(string titulo, Disciplina disciplina, Materia? materia, int serie, int quantidadeDeQuestoes, bool provaRecuperacao)
    {
        Titulo = titulo;
        Disciplina = disciplina;
        Materia = materia;
        Serie = serie;
        QuantidadeDeQuestoes = quantidadeDeQuestoes;
        ProvaRecuperacao = provaRecuperacao;
    }

    public override void Atualizar(Prova entidadeAtualizada)
    {
        throw new NotImplementedException();
    }

    public override List<string> Validar()
    {
        List<string> erros = [];

        if (String.IsNullOrWhiteSpace(Titulo))
            erros.Add("O Campo \"Título\" é obrigatório.");

        else if (Titulo.Length < 2 || Titulo.Length > 100)
            erros.Add("O campo \"Título\" deve conter entre 2 à 100 caracteres.");

        if (Disciplina is null)
            erros.Add("O campo \"Disciplina\" deve ser preenchido.");

        if (ProvaRecuperacao && Materia is not null)
            erros.Add("O campo \"Materia\" não deve ser preenchido em uma prova de recuperação.");

        else if (!ProvaRecuperacao && Materia is not null && !Equals(Materia.Serie, Serie))
            erros.Add("O campo \"Série\" deve alinhar com a série da Materia.");

        if (Serie <= 0)
            erros.Add("O campo \"Série\" deve ser maior que 0.");

        if (QuantidadeDeQuestoes <= 0)
            erros.Add("O campo \"Quantidade De Questões\" deve ser maior que 0.");

        if(Materia is not null && Materia.Disciplina != Disciplina)
            erros.Add("O campo \"Disciplina\" deve alinhar com a Disciplina da Materia.");
            
        return erros;
    }
}
