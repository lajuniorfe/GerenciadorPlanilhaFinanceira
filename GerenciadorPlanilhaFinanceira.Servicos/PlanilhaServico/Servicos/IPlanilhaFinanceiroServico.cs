using GerenciadorPlanilhaFinanceira.Servicos.PlanilhaServico.Entidades;

namespace GerenciadorPlanilhaFinanceira.Servicos.PlanilhaServico.Servicos
{
    public interface IPlanilhaFinanceiroServico
    {
        Task<List<PersistenciaFinanceiro>> TratarDespesasParceladas(int parcelas, PlanilhaFinanceiroRequest request);
        Task EditarSincronizacaoPlanilha(int linha, string pagina, string identificadoLinha);
        Task<PersistenciaFinanceiro> TrataDespesasNaoParceladas(PlanilhaFinanceiroRequest request);
    }
}
