var builder = WebApplication.CreateBuilder(args);

// Configurando o CORS (Recomenda-se limitar as origens em produção)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        // Durante o desenvolvimento, você pode permitir qualquer origem
        // Em produção, é recomendado restringir as origens para maior segurança
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Adicionando serviços à aplicação
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configurando o Kestrel para escutar na porta configurada pelo Azure (ou 5000 localmente)
builder.WebHost.ConfigureKestrel(options =>
{
    // Converte a variável de ambiente "PORT" para um número inteiro, ou usa 5000 como padrão
    int port = int.TryParse(Environment.GetEnvironmentVariable("PORT"), out var parsedPort) ? parsedPort : 5000;
    options.Listen(System.Net.IPAddress.Any, port);
});

var app = builder.Build();

// Configurando middlewares para o ambiente de desenvolvimento
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Habilitar CORS
app.UseCors("AllowAll");

// Configurar mapeamento de controladores
app.MapControllers();

// Configurar o roteamento para arquivos estáticos da pasta wwwroot (para o index.html)
app.UseStaticFiles(); // Habilita o acesso aos arquivos estáticos

// Executar a aplicação
app.Run();
