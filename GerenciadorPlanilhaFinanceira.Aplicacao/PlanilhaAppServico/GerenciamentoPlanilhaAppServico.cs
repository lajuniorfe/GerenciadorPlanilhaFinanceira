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

        public GerenciamentoPlanilhaAppServico(IPlanilhaFinanceiroServico planilhaFinanceiroServico)
        {
            this.planilhaFinanceiroServico = planilhaFinanceiroServico;
        }

        public async Task TratarMensagemDespesaRecebida(string mensagem)
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
                await planilhaFinanceiroServico.TratarDespesasParceladas(request.Parcela, request);
            }
            else
            {
                //compa sem parcela
            }

            //dispara mensagem para persistencia em banco de dados
            //string jsonRequest = JsonSerializer.Serialize(request);
            // await rabbitMq.DispararMensagemPersistencia(jsonRequest);

            Console.WriteLine("deu tudo certo");
        }

        public async Task TratarMensagemPersistenciaRecebidaAsync(string mensagem)
        {
            PlanilhaFinanceiroRequest jsonMensagem = JsonSerializer.Deserialize<PlanilhaFinanceiroRequest>(mensagem);

            string[] partes = jsonMensagem.Identificador.Split('|');

            string pagina = partes[0];
            int linha = Convert.ToInt32(partes[1]);

            await planilhaFinanceiroServico.EditarSincronizacaoPlanilha(linha, pagina);
        }
    }
}
