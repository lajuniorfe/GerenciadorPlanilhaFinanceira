using GerenciadorPlanilhaFinanceira.Aplicacao.Messageria.RabbitMqAppServico.Consumer.Interface;
using GerenciadorPlanilhaFinanceira.Servicos.RabbitMqServico;
using Microsoft.Extensions.Hosting;

namespace GerenciadorPlanilhaFinanceira.Servicos.Worker
{
    public class QueueConsumerWorker : BackgroundService
    {
        private readonly IRabbitConsumerApp _rabbitMq;

        public QueueConsumerWorker(IRabbitConsumerApp rabbitMq)
        {
            _rabbitMq = rabbitMq;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _rabbitMq.OuvirFilaPlanilhaFinanceiro(stoppingToken);
            await _rabbitMq.OuvirFilaPersistencia(stoppingToken);
        }
    }
}
