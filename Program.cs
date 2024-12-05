var builder = WebApplication.CreateBuilder(args);

// Configurando o CORS (permitir qualquer origem, método e cabeçalho)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configurando o Kestrel para escutar a porta dinâmica fornecida pelo Azure (ou 5000 localmente)
builder.WebHost.ConfigureKestrel(options =>
{
    // Usando a variável de ambiente PORT (Azure fornece essa variável para definir a porta)
    var port = Environment.GetEnvironmentVariable("PORT") ?? "5000"; // 5000 como valor padrão
    options.Listen(System.Net.IPAddress.Any, int.Parse(port)); // Configura a porta dinâmica
});

var app = builder.Build();

// Ativando o Swagger apenas em ambiente de desenvolvimento
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Habilitar CORS
app.UseCors("AllowAll");

// Servir arquivos estáticos da pasta wwwroot (para index.html e outros arquivos estáticos)
app.UseStaticFiles();

// Servir o arquivo index.html como página inicial
app.UseDefaultFiles(); // Middleware que serve o index.html automaticamente quando acessado a URL base

// Mapear os controladores
app.MapControllers();

// Iniciar a aplicação
app.Run();
