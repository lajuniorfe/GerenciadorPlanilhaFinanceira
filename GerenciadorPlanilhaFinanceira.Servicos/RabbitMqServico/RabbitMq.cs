using GerenciadorPlanilhaFinanceira.Servicos.EmailServico;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace GerenciadorPlanilhaFinanceira.Servicos.RabbitMqServico
{
    public class RabbitMq : IRabbitMq
    {
        private readonly ConnectionFactory _connectionFactory;
        private readonly IEnviarEmailServico enviarEmailServico;

        public RabbitMq(IEnviarEmailServico enviarEmailServico)
        {
            _connectionFactory = new ConnectionFactory()
            {
                HostName = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "rabbitmq-fly.internal",
                Port = int.Parse(Environment.GetEnvironmentVariable("RABBITMQ_PORT") ?? "5672"),
                UserName = Environment.GetEnvironmentVariable("RABBITMQ_USER") ?? "admin",
                Password = Environment.GetEnvironmentVariable("RABBITMQ_PASS") ?? "FinanceiroHoot!23"
            };
            this.enviarEmailServico = enviarEmailServico;
        }

        public async Task OuvirFila()
        {

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
               Console.WriteLine($"Mensagem recebida: {message}");

               // Envia email
               enviarEmailServico.EnviarEmailAsync("destinatario@exemplo.com", "Evento na fila RabbitMQ", message);

               // Confirma consumo da mensagem
               await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
           };

            await channel.BasicConsumeAsync(queue: queueName,
                                  autoAck: false,
                                  consumer: consumer);

            Console.WriteLine("Esperando mensagens. Pressione [enter] para sair.");
            Console.ReadLine();
        }
    }
}
