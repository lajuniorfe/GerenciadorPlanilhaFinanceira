using GerenciadorPlanilhaFinanceira.Aplicacao.Messageria.RabbitMqAppServico.Producter.Interface;
using GerenciadorPlanilhaFinanceira.Aplicacao.PlanilhaAppServico.Interface;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GerenciadorPlanilhaFinanceira.Aplicacao.Messageria.RabbitMqAppServico.Producter
{
    public class RabbitProducterApp : IRabbitProducterApp
    {
        private readonly ConnectionFactory _connectionFactory;
        private readonly IConfiguration _configuration;
       

        public RabbitProducterApp(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionFactory = new ConnectionFactory()
            {
                HostName = _configuration["RabbitMQ:Host"],
                Port = int.Parse(_configuration["RabbitMQ:Port"] ?? "5672"),
                UserName = _configuration["RabbitMQ:Username"],
                Password = _configuration["RabbitMQ:Password"]
            };
        }
        public async Task DispararMensagemPersistencia(string mensagem, CancellationToken cancellation = default)
        {
            try
            {
                string fila = "persistencia-dados-planilha";
                var connection = await _connectionFactory.CreateConnectionAsync();
                var channel = await connection.CreateChannelAsync();
                await channel.QueueDeclareAsync(queue: fila, durable: true, exclusive: false, autoDelete: false, arguments: null);
                var body = Encoding.UTF8.GetBytes(mensagem);
                await channel.BasicPublishAsync(exchange: "", routingKey: fila, body: body, cancellation);

                Console.WriteLine($"Mensagem publicada na fila '{fila}'.");

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
