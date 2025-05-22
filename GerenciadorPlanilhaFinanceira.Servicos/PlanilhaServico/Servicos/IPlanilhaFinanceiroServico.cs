using GerenciadorPlanilhaFinanceira.Servicos.PlanilhaServico.Entidades;

namespace GerenciadorPlanilhaFinanceira.Servicos.PlanilhaServico.Servicos
{
    public interface IPlanilhaFinanceiroServico
    {
        Task TratarDespesasParceladas(int parcelas, PlanilhaFinanceiroRequest request);
        Task EditarSincronizacaoPlanilha(int linha, string pagina);
    }
}
