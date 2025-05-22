using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GerenciadorPlanilhaFinanceira.Aplicacao.Messageria.RabbitMqAppServico.Producter.Interface
{
    public interface IRabbitProducterApp
    {
        Task DispararMensagemPersistencia(string mensagem, CancellationToken cancellation = default);
    }
}
