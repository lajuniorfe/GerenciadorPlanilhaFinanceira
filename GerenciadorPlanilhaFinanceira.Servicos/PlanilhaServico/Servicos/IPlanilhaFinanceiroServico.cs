using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GerenciadorPlanilhaFinanceira.Servicos.PlanilhaServico.Servicos
{
    public interface IPlanilhaFinanceiroServico
    {
        public void TratarMensagemDespesaRecebida(string mensagem);
        Task TratarMensagemPersistenciaRecebidaAsync(string mensagem);
    }
}
