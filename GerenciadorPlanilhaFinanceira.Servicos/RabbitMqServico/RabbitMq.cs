using GerenciadorPlanilhaFinanceira.Servicos.EmailServico;
using GerenciadorPlanilhaFinanceira.Servicos.PlanilhaServico.Servicos;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

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

        public async Task OuvirFila()
        {
            var reset = new ManualResetEventSlim(false);

            using var connection = await _connectionFactory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            string queueName = "planilha-financeiro";

            await channel.QueueDeclareAsync(queue: queueName,
                              durable: true,
                              exclusive: false,
                              autoDelete: false,
             arguments: null);

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (model, ea) =>
           {
               var body = ea.Body.ToArray();
               var message = Encoding.UTF8.GetString(body);

               planilhaFinanceiroServico.TratarMensagemDespesaRecebida(message);

               // Confirma consumo da mensagem
               await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
           };

            await channel.BasicConsumeAsync(queue: queueName,
                                  autoAck: false,
                                  consumer: consumer);

            reset.Wait(); 
        }
    }
}
