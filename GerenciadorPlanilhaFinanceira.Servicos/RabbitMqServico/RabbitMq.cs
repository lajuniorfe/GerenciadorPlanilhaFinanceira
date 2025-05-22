using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;


namespace GerenciadorPlanilhaFinanceira.Servicos.RabbitMqServico
{
    public class RabbitMq : IRabbitMq
    {
       

        public RabbitMq()
        {
           
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

