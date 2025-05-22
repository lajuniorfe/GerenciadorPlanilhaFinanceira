using GerenciadorPlanilhaFinanceira.Servicos.PlanilhaServico.Servicos;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace GerenciadorPlanilhaFinanceira.Servicos.RabbitMqServico
{
    public class RabbitMq : IRabbitMq
    {
        private readonly ConnectionFactory _connectionFactory;
        private readonly IConfiguration _configuration;
        private readonly IPlanilhaFinanceiroServico planilhaFinanceiroServico;

        public RabbitMq(IConfiguration configuration, IPlanilhaFinanceiroServico planilhaFinanceiroServico)
        {
            _configuration = configuration;

            _connectionFactory = new ConnectionFactory()
            {
                HostName = _configuration["RabbitMQ:Host"],
                Port = int.Parse(_configuration["RabbitMQ:Port"] ?? "5672"),
                UserName = _configuration["RabbitMQ:Username"],
                Password = _configuration["RabbitMQ:Password"]
            };
            this.planilhaFinanceiroServico = planilhaFinanceiroServico;
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

                planilhaFinanceiroServico.TratarMensagemDespesaRecebida(message);

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

                await planilhaFinanceiroServico.TratarMensagemPersistenciaRecebidaAsync(message);

                await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
            };

            await channel.BasicConsumeAsync(queue: queueName,
                                  autoAck: false,
                                  consumer: consumer);

            reset.Wait();

        }
        //public async Task DispararMensagemPersistencia(string mensagem, CancellationToken cancellation = default)
        //{
        //    try
        //    {
        //        string fila = "persistencia-dados-planilha";
        //        var connection = await _connectionFactory.CreateConnectionAsync();
        //        var channel = await connection.CreateChannelAsync();
        //        await channel.QueueDeclareAsync(queue: fila, durable: true, exclusive: false, autoDelete: false, arguments: null);
        //        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(mensagem));
        //        await channel.BasicPublishAsync(exchange: "", routingKey: fila, body: body, cancellation);

        //        Console.WriteLine($"Mensagem publicada na fila '{fila}'.");

        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
    }
}

