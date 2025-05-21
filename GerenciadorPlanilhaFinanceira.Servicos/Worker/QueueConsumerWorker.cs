using GerenciadorPlanilhaFinanceira.Servicos.RabbitMqServico;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            await _rabbitMq.OuvirFila();
        }
    }
}
