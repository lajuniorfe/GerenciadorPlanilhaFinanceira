using GerenciadorPlanilhaFinanceira.Servicos.RabbitMqServico;
using Microsoft.Extensions.Hosting;

namespace GerenciadorPlanilhaFinanceira.Servicos.Worker
{
    public class QueueConsumerWorker : BackgroundService
    {
        private readonly IRabbitMq _rabbitMq;

        public QueueConsumerWorker(IRabbitMq rabbitMq)
        {
            _rabbitMq = rabbitMq;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _rabbitMq.OuvirFilaPlanilhaFinanceiro();
           // await _rabbitMq.OuvirFilaPersistencia();
        }
    }
}
