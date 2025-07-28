// See https://aka.ms/new-console-template for more information
using GerenciadorPlanilhaFinanceira.Aplicacao.Messageria.RabbitMqAppServico.Consumer;
using GerenciadorPlanilhaFinanceira.Aplicacao.Messageria.RabbitMqAppServico.Consumer.Interface;
using GerenciadorPlanilhaFinanceira.Aplicacao.Messageria.RabbitMqAppServico.Producer;
using GerenciadorPlanilhaFinanceira.Aplicacao.Messageria.RabbitMqAppServico.Producer.Interface;
using GerenciadorPlanilhaFinanceira.Aplicacao.PlanilhaAppServico;
using GerenciadorPlanilhaFinanceira.Aplicacao.PlanilhaAppServico.Interface;
using GerenciadorPlanilhaFinanceira.Aplicacao.Worker;
using GerenciadorPlanilhaFinanceira.Servicos.EmailServico;
using GerenciadorPlanilhaFinanceira.Servicos.PlanilhaServico.Servicos;
using GerenciadorPlanilhaFinanceira.Servicos.RabbitMqServico;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

string environment = "Production"; //"Production"; // ou "Development"

var host = Host.CreateDefaultBuilder(args)
     .ConfigureAppConfiguration((hostingContext, config) =>
     {
         // Limpa as fontes padrão (opcional)
         config.Sources.Clear();

         // Carrega os arquivos de configuração conforme a variável manual
         config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
               .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
               .AddUserSecrets<Program>();
     })
     .ConfigureServices((_, services) =>
     {
         // Registra os serviços necessários
         services.AddSingleton<IRabbitMq, RabbitMq>();
         services.AddSingleton<IEnviarEmailServico, EnviarEmailServico>();
         services.AddSingleton<IPlanilhaFinanceiroServico, PlanilhaFinanceiroServico>();
         services.AddSingleton<IGerenciamentoAppServico, GerenciamentoPlanilhaAppServico>();
         services.AddSingleton<IRabbitConsumerApp,RabbitConsumerApp>();
         services.AddSingleton<IRabbitProducerApp, RabbitProducerApp>();

         services.AddHostedService<QueueConsumerWorker>();

     })

    .Build();

await host.RunAsync();

