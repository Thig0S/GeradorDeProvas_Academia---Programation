using System.Text.RegularExpressions;
using Microsoft.Playwright;
using Microsoft.Playwright.MSTest;

namespace GeradorDeProvas.Test.E2E;

[TestClass]
public sealed class AutenticacaoE2ETests : PageTest
{
    private string urlBase = "http://localhost:8001";

    [TestMethod]
    public async Task Deve_Exibir_TelaDeLogin_Para_UsuarioAnonimo()
    {
        //arrange

        //act
        await Page.GotoAsync($"{urlBase}");

        //assert
        await Expect(Page).ToHaveTitleAsync(new Regex("Entrar"));
    }
    [TestMethod]
    public async Task Deve_RegistrarEAutenticar_Usuario()
    {
        //arrange
        const string email = "novo.usuario@teste.local";
        const string senha = "senha123!";

        await Page.GotoAsync($"{urlBase}/Autenticacao/Registrar");

        //act
        await Page.GetByLabel("E-Mail").FillAsync(email);
        //diz que precisar ser o campo exato
        await Page.GetByLabel("Senha", new() { Exact = true }).FillAsync(senha);
        await Page.GetByLabel("Confirmar Senha").FillAsync(senha);

        //selecionar pelo papel do Botão com o nome Criar Conta
        await Page.GetByRole(AriaRole.Button, new() { Name = "Criar Conta" }).ClickAsync();

        //assert
        string rotaAbsoluta = new Uri(Page.Url).AbsolutePath;
        Assert.AreEqual("/", rotaAbsoluta);
    }
}
