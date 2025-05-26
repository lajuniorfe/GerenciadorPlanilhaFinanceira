using GerenciadorPlanilhaFinanceira.Aplicacao.Messageria.RabbitMqAppServico.Consumer.Interface;
using GerenciadorPlanilhaFinanceira.Aplicacao.PlanilhaAppServico.Interface;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace GerenciadorPlanilhaFinanceira.Aplicacao.Messageria.RabbitMqAppServico.Consumer
{
    public class RabbitConsumerApp : IRabbitConsumerApp
    {
        private readonly ConnectionFactory _connectionFactory;
        private readonly IConfiguration _configuration;
        private IConnection? _connection;
        private IChannel? _channel;
        private readonly IGerenciamentoAppServico gerenciamentoAppServico;

        public RabbitConsumerApp(IConfiguration configuration, IGerenciamentoAppServico gerenciamentoAppServico)
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

        private async Task AbrirConexao()
        {
            if (_connection == null || !_connection.IsOpen)
            {
                _connection = await _connectionFactory.CreateConnectionAsync();
            }

            if (_channel == null || !_channel.IsOpen)
            {
                _channel = await _connection.CreateChannelAsync();
            }
        }

        public async Task OuvirFilaPlanilhaFinanceiro(CancellationToken cancellationToken)
        {
            await AbrirConexao();
       
            string queueName = "planilha-financeiro";

            await _channel
                .QueueDeclareAsync(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                await gerenciamentoAppServico.TratarMensagemDespesaRecebida(message, cancellationToken);

                await _channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
            };

            await _channel.BasicConsumeAsync(queue: queueName, autoAck: false, consumer: consumer);

           
        }

        public async Task OuvirFilaPersistencia(CancellationToken cancellationToken)
        {
            await AbrirConexao();
            
            string queueName = "persistencia-financeiro";

            await _channel.QueueDeclareAsync(queue: queueName,
                              durable: true,
                              exclusive: false,
                              autoDelete: false,
                              arguments: null
            );


            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                await gerenciamentoAppServico.TratarMensagemPersistenciaRecebidaAsync(message, cancellationToken);

                await _channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
            };

            await _channel.BasicConsumeAsync(queue: queueName,
                                  autoAck: false,
                                  consumer: consumer);

           

        }
    }
}
