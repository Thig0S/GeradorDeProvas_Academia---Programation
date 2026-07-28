using GeradorDeProvas.Dominio.Modulos.ModuloProva;
using GeradorDeProvas.Infra.Compartilhado.Orm;
using Microsoft.EntityFrameworkCore;

namespace GeradorDeProvas.Infra.Modulos.ModuloProva;

public class RepositorioProvaEmOrm(GeradorDeProvasDbContext dbContext) : RepositorioBaseEmOrm<Prova>(dbContext)
{
    public override Prova? SelecionarPorId(Guid idSelecionado)
    {
        return registros
        .Include(p => p.Disciplina)
        .Include(p => p.Materia)
        .Include(p => p.Questoes)
        .ThenInclude(q => q.Alternativas)
        .SingleOrDefault(p => p.Id == idSelecionado);
    }
    public override List<Prova> SelecionarTodos()
    {
        return registros
        .Include(p => p.Disciplina)
        .Include(p => p.Materia)
        .Include(p => p.Questoes)
            .ThenInclude(q => q.Alternativas)
            .ToList();
    }
}
