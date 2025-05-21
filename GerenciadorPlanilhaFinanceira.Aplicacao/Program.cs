// See https://aka.ms/new-console-template for more information
using GerenciadorPlanilhaFinanceira.Servicos.EmailServico;
using GerenciadorPlanilhaFinanceira.Servicos.PlanilhaServico.Servicos;
using GerenciadorPlanilhaFinanceira.Servicos.RabbitMqServico;
using GerenciadorPlanilhaFinanceira.Servicos.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

string environment = "Development"; //"Production"; // ou "Development"

var host = Host.CreateDefaultBuilder(args)
     .ConfigureAppConfiguration((hostingContext, config) =>
     {
         // Limpa as fontes padrão (opcional)
         config.Sources.Clear();

         // Carrega os arquivos de configuração conforme a variável manual
         config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
               .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true);
     })
     .ConfigureServices((_, services) =>
     {
         // Registra os serviços necessários
         services.AddSingleton<IRabbitMq, RabbitMq>();
         services.AddSingleton<IEnviarEmailServico, EnviarEmailServico>();
         services.AddSingleton<IPlanilhaFinanceiroServico, PlanilhaFinanceiroServico>();

         services.AddHostedService<QueueConsumerWorker>();

     })
    .Build();

await host.RunAsync();

