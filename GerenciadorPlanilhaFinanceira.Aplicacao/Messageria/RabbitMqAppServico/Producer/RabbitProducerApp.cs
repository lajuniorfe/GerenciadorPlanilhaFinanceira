using GerenciadorPlanilhaFinanceira.Aplicacao.Messageria.RabbitMqAppServico.Producer.Interface;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System.Text;

namespace GerenciadorPlanilhaFinanceira.Aplicacao.Messageria.RabbitMqAppServico.Producer
{
    public class RabbitProducerApp : IRabbitProducerApp
    {
        private readonly ConnectionFactory _connectionFactory;
        private readonly IConfiguration _configuration;
        private IConnection? _connection;
        private IChannel? _channel;

        public RabbitProducerApp(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionFactory = new ConnectionFactory()
            {
                Uri = new Uri(_configuration["RabittOnline:Url"])
            };
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
        public async Task DispararMensagemPersistencia(string mensagem, CancellationToken cancellation = default)
        {
            try
            {
                await AbrirConexao();

                string fila = "persistencia-dados-planilha";
               
                await _channel.QueueDeclareAsync(queue: fila, durable: true, exclusive: false, autoDelete: false, arguments: null);
                var body = Encoding.UTF8.GetBytes(mensagem);
                await _channel.BasicPublishAsync(exchange: "", routingKey: fila, body: body, cancellation);

                Console.WriteLine($"Mensagem publicada na fila '{fila}'.");

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
