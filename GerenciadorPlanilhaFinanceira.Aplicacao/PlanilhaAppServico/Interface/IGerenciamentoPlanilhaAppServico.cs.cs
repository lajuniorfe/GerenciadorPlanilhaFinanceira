using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GerenciadorPlanilhaFinanceira.Aplicacao.PlanilhaAppServico.Interface
{
    public interface IGerenciamentoAppServico
    {
        Task TratarMensagemDespesaRecebida(string mensagem);
        Task TratarMensagemPersistenciaRecebidaAsync(string mensagem);
    }
}
