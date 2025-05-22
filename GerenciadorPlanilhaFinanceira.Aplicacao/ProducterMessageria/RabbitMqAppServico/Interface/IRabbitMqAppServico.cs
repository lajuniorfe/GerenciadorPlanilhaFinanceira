using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GerenciadorPlanilhaFinanceira.Aplicacao.ProducterMessageria.RabbitMqAppServico.Interface
{
    public interface IRabbitMqAppServico
    {
        Task OuvirFilaPlanilhaFinanceiro();
        Task OuvirFilaPersistencia();
    }
}
