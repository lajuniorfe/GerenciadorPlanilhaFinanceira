using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GerenciadorPlanilhaFinanceira.Aplicacao.Messageria.RabbitMqAppServico.Producer.Interface
{
    public interface IRabbitProducerApp
    {
        Task DispararMensagemPersistencia(string mensagem, CancellationToken cancellation = default);
    }
}
