using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GerenciadorPlanilhaFinanceira.Aplicacao.Messageria.RabbitMqAppServico.Consumer.Interface
{
    public interface IRabbitConsumerApp
    {
        Task OuvirFilaPlanilhaFinanceiro(CancellationToken cancellationToken);
        Task OuvirFilaPersistencia(CancellationToken cancellationToken);
    }
}
