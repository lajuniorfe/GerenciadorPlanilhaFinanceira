var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();

/*
1 - Ao receber uma despesa da planilha eu quero armazenar no Banco de dados
2 - Quero editar a planilha para informar que foi sincronizada
3 - Quero criar na planilha e no banco de dados as despesas parceladas nos meses correspondentes ex: despesa janeiro parcelada em 2. criar em janeiro e fevereiro
*/