using GerenciadorPlanilhaFinanceira.Aplicacao.PlanilhaAppServico.Interface;
using GerenciadorPlanilhaFinanceira.Aplicacao.ProducterMessageria.RabbitMqAppServico.Interface;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace GerenciadorPlanilhaFinanceira.Aplicacao.ProducterMessageria.RabbitMqAppServico
{
    public class RabbitMqAppServico : IRabbitMqAppServico
    {
        private readonly ConnectionFactory _connectionFactory;
        private readonly IConfiguration _configuration;
        private readonly IGerenciamentoAppServico gerenciamentoAppServico;

        public RabbitMqAppServico(IConfiguration configuration, IGerenciamentoAppServico gerenciamentoAppServico)
        {
            _configuration = configuration;

            _connectionFactory = new ConnectionFactory()
            {
                HostName = _configuration["RabbitMQ:Host"],
                Port = int.Parse(_configuration["RabbitMQ:Port"] ?? "5672"),
                UserName = _configuration["RabbitMQ:Username"],
                Password = _configuration["RabbitMQ:Password"]
            };

            this.gerenciamentoAppServico = gerenciamentoAppServico;
        }

        public async Task OuvirFilaPlanilhaFinanceiro()
        {
            var reset = new ManualResetEventSlim(false);

            using var connection = await _connectionFactory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            string queueName = "planilha-financeiro";

            await channel
                .QueueDeclareAsync(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                await gerenciamentoAppServico.TratarMensagemDespesaRecebida(message);

                await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
            };

            await channel.BasicConsumeAsync(queue: queueName, autoAck: false, consumer: consumer);

            reset.Wait();
        }

        public async Task OuvirFilaPersistencia()
        {
            var reset = new ManualResetEventSlim(false);
            using var connection = await _connectionFactory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            string queueName = "persistencia-financeiro";


            await channel.QueueDeclareAsync(queue: queueName,
                              durable: true,
                              exclusive: false,
                              autoDelete: false,
                              arguments: null
            );


            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                await gerenciamentoAppServico.TratarMensagemPersistenciaRecebidaAsync(message);

                await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
            };

            await channel.BasicConsumeAsync(queue: queueName,
                                  autoAck: false,
                                  consumer: consumer);

            reset.Wait();

        }
    }
}
