using GerenciadorPlanilhaFinanceira.Aplicacao.Messageria.RabbitMqAppServico.Producer.Interface;
using GerenciadorPlanilhaFinanceira.Aplicacao.PlanilhaAppServico.Interface;
using GerenciadorPlanilhaFinanceira.Servicos.PlanilhaServico.Entidades;
using GerenciadorPlanilhaFinanceira.Servicos.PlanilhaServico.Servicos;
using System.Globalization;
using System.Text.Json;

namespace GerenciadorPlanilhaFinanceira.Aplicacao.PlanilhaAppServico
{
    public class GerenciamentoPlanilhaAppServico : IGerenciamentoAppServico
    {
        private readonly IPlanilhaFinanceiroServico planilhaFinanceiroServico;
        private readonly IRabbitProducerApp rabbitProducterApp;

        public GerenciamentoPlanilhaAppServico(IPlanilhaFinanceiroServico planilhaFinanceiroServico, IRabbitProducerApp rabbitProducterApp)
        {
            this.planilhaFinanceiroServico = planilhaFinanceiroServico;
            this.rabbitProducterApp = rabbitProducterApp;
        }

        public async Task TratarMensagemDespesaRecebida(string mensagem, CancellationToken cancellationToken)
        {
            GoogleSheetFinanceiroRequest jsonMensagem = JsonSerializer.Deserialize<GoogleSheetFinanceiroRequest>(mensagem);

            PlanilhaFinanceiroRequest request = new PlanilhaFinanceiroRequest();
            var culturaBR = new CultureInfo("pt-BR");
            var data = DateTime.Parse(jsonMensagem.Values[0], culturaBR);

            request.DataCriaçao = data;
            request.NomeDespesa = jsonMensagem.Values[1];
            request.Valor = Convert.ToDecimal(jsonMensagem.Values[2]);
            request.TipoDespesa = jsonMensagem.Values[3];
            request.Categoria = jsonMensagem.Values[4];
            request.FormaPagamento = jsonMensagem.Values[5];
            request.CompraParcelada = jsonMensagem.Values[6] == "Não" ? false : true;
            request.Parcela = jsonMensagem.Values[7] == "" ? 0 : Convert.ToInt32(jsonMensagem.Values[7]);
            request.Responsavel = jsonMensagem.Values[8];
            request.MesRelacionado = jsonMensagem.Values[9];
            request.Identificador = $"{jsonMensagem.Sheet}| {jsonMensagem.Row}";

            // criar despesas parceladas para cada mes correspondente e gravar na planilha e no banco
            if (request.CompraParcelada)
            {
                List<PersistenciaFinanceiro> retorno = await planilhaFinanceiroServico.TratarDespesasParceladas(request.Parcela, request);

                // publicar em fila de persitencia
                await rabbitProducterApp.DispararMensagemPersistencia(JsonSerializer.Serialize(retorno), cancellationToken);

            }
            else
            {
                PersistenciaFinanceiro retorno = await planilhaFinanceiroServico.TrataDespesasNaoParceladas(request);

                // publicar em fila de persitencia
                await rabbitProducterApp.DispararMensagemPersistencia(JsonSerializer.Serialize(retorno), cancellationToken);

            }

            Console.WriteLine("deu tudo certo");
        }

        public async Task TratarMensagemPersistenciaRecebidaAsync(string mensagem, CancellationToken cancellationToken)
        {
            List<PlanilhaFinanceiroRequest> jsonMensagem = JsonSerializer.Deserialize<List<PlanilhaFinanceiroRequest>>(mensagem);

            foreach(var i in jsonMensagem)
            {
                string[] partes = i.Identificador.Split('|');

                string pagina = partes[0];
                int linha = Convert.ToInt32(partes[1]);

                string identificadorLinha = pagina == "Respostas ao formulário 1" ? "K" : "J";

                await planilhaFinanceiroServico.EditarSincronizacaoPlanilha(linha, pagina, identificadorLinha);
            }
        }
    }
}
