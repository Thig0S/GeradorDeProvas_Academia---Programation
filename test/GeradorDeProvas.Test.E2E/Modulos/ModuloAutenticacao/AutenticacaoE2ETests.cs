using System.Text.RegularExpressions;
using GeradorDeProvas.Test.E2E.Compartilhado;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Client;
using Microsoft.Playwright;
using Microsoft.Playwright.MSTest;

namespace GeradorDeProvas.Test.E2E;

[TestClass]
public sealed class AutenticacaoE2ETests : PageTest
{
    private TestApplicationFactory Aplicacao = null!;
    private string UrlBase { get; set; } = string.Empty;

    [TestInitialize]
    public async Task InicializarAplicacao()
    {
        Aplicacao = new TestApplicationFactory();

        UrlBase = Aplicacao.UrlBase!;
    }
    [TestCleanup]
    public async Task LiberarAplicacao()
    {
        try
        {
            if (Aplicacao is not null)
                await Aplicacao.DisposeAsync();

        }
        finally
        {
            Aplicacao = null!;
        }
    }
    [TestMethod]
    public async Task Deve_Exibir_TelaDeLogin_Para_UsuarioAnonimo()
    {
        //arrange

        //act
        await Page.GotoAsync($"{UrlBase}");

        //assert
        await Expect(Page).ToHaveTitleAsync(new Regex("Entrar"));
    }
    [TestMethod]
    public async Task Deve_RegistrarEAutenticar_Usuario()
    {
        //arrange
        const string email = "novo.usuario@teste.local";
        const string senha = "senha123!";

        await Page.GotoAsync($"{UrlBase}/Autenticacao/Registrar");

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
    [TestMethod]
    public async Task Deve_EntrarEAutentiacr_Usuario_Valido()
    {
        //arrange
        const string email = "novo.usuario@teste.local";
        const string senha = "senha123!";

        await RegistrarEAutenticarUsuario(email, senha);

        //act
        await Page.GotoAsync($"{UrlBase}/Autenticacao/Entrar");

        await Page.GetByLabel("E-Mail").FillAsync(email);
        await Page.GetByLabel("Senha").FillAsync(senha);

        await Page.GetByRole(AriaRole.Button, new() { Name = "Entrar" }).ClickAsync();

        string rotaAbsoluta = new Uri(Page.Url).AbsolutePath;
        Assert.AreEqual("/", rotaAbsoluta);
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = email })).ToBeVisibleAsync();
    }
    private async Task RegistrarEAutenticarUsuario(string email, string senha)
    {
        using IServiceScope scope = Aplicacao.Services.CreateScope();

        UserManager<IdentityUser<Guid>> userManager =
            scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser<Guid>>>();

        IdentityUser<Guid> user = new IdentityUser<Guid>()
        {
            Id = Guid.CreateVersion7(),
            UserName = email,
            Email = email
        };

        IdentityResult resultado = await userManager.CreateAsync(user, senha);

        Assert.IsTrue(
            resultado.Succeeded, string.Join("; ", resultado.Errors.Select(erro => erro.Description))
        );
    }
}
