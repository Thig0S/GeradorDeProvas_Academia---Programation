using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace GeradorDeProvas.Aplicacao.Modulos.ModuloProva;

public sealed class GeradorDePdf
{
    public byte[] Gerar(DetalhesProvaDto prova, bool incluirGabarito)
    {
        // GerenetePdf retorna bytes
        return CriarDocumento(prova, incluirGabarito).GeneratePdf();
    }
    private static IDocument CriarDocumento(DetalhesProvaDto prova, bool incluirGabarito)
    {
        return Document.Create(d =>
        {
            //Formatacao da pagina padrao
            d.Page(pagina =>
            {
                pagina.Size(PageSizes.A4);
                pagina.Margin(2, Unit.Centimetre);
                pagina.PageColor(Colors.White);
                pagina.DefaultTextStyle(style => style.FontSize(12));

                //Cabecalho
                pagina.Header()
                    .Column(header =>
                    {
                        header.Item().Text(prova.Titulo)
                        .Bold()
                        .FontSize(18)
                        .FontColor(Colors.Blue.Darken2);

                        header.Item()
                            .PaddingTop(4)
                            .Text(texto =>
                        {
                            texto.Span($"Disciplina: {prova.NomeDisciplina}     ");
                            texto.Span(prova.ProvaRecuperacao ? "Prova de Recuperação" : $"Matéria: {prova.NomeMateria}     ");
                            texto.Span($"Série: {prova.Serie}");
                        });

                        header.Item()
                            .PaddingTop(8)
                            .LineHorizontal(1)
                            .LineColor(Colors.Grey.Lighten1);
                    });

                //Conteudo central da pagina, questoes e alternativas
                pagina.Content()
                    .PaddingVertical(15)
                    .Column(conteudo =>
                    {
                        conteudo.Spacing(12);

                        for (int i = 0; i < prova.Questoes.Count; i++)
                        {
                            QuestaoProvaDto questaoDto = prova.Questoes[i];

                            conteudo.Item()
                                .PreventPageBreak()
                                .Column(questao =>
                                {
                                    questao.Spacing(5);

                                    questao.Item().Text(texto =>
                                    {
                                        texto.Span($"{i + 1}").Bold();
                                        texto.Span(questaoDto.Enunciado);
                                    });

                                    foreach (AlternativaProvaDto alternativa in questaoDto.Alternativas)
                                    {
                                        string marcador = incluirGabarito && alternativa.Correta ? "[X]" : "[ ]";

                                        questao.Item().PaddingLeft(15)
                                        .Text($"{marcador} {alternativa.Texto}");
                                    }
                                });
                        }
                    });

                //Rodape
                pagina.Footer()
                    .AlignCenter()
                    .Text(texto =>
                    {
                        texto.Span("Página ");
                        texto.CurrentPageNumber();
                        texto.Span(" de ");
                        texto.TotalPages();
                    });
            });
        });
    }
}
