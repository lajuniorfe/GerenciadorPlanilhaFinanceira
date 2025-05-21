// See https://aka.ms/new-console-template for more information
using GerenciadorPlanilhaFinanceira.Servicos.EmailServico;
using GerenciadorPlanilhaFinanceira.Servicos.RabbitMqServico;
using GerenciadorPlanilhaFinanceira.Servicos.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args)
     .ConfigureServices((_, services) =>
     {
         // Registra os serviços necessários
         services.AddSingleton<IRabbitMq, RabbitMq>();
         services.AddSingleton<IEnviarEmailServico, EnviarEmailServico>();
         services.AddHostedService<QueueConsumerWorker>();

     })
    .Build();

await host.RunAsync();