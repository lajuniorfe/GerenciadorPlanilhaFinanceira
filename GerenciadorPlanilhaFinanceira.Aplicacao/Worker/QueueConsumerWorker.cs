using GerenciadorPlanilhaFinanceira.Aplicacao.ProducterMessageria.RabbitMqAppServico.Interface;
using GerenciadorPlanilhaFinanceira.Servicos.RabbitMqServico;
using Microsoft.Extensions.Hosting;

namespace GerenciadorPlanilhaFinanceira.Servicos.Worker
{
    public class QueueConsumerWorker : BackgroundService
    {
        private readonly IRabbitMqAppServico _rabbitMq;

        public QueueConsumerWorker(IRabbitMqAppServico rabbitMq)
        {
            _rabbitMq = rabbitMq;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _rabbitMq.OuvirFilaPlanilhaFinanceiro();
            await _rabbitMq.OuvirFilaPersistencia();
        }
    }
}
