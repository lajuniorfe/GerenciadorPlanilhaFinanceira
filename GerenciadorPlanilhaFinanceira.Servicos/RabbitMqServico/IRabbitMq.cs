using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GerenciadorPlanilhaFinanceira.Servicos.RabbitMqServico
{
    public interface IRabbitMq
    {
        Task OuvirFila();
    }
}
