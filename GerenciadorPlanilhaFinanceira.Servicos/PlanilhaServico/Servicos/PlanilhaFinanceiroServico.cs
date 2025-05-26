using GerenciadorPlanilhaFinanceira.Servicos.PlanilhaServico.Entidades;
using GerenciadorPlanilhaFinanceira.Servicos.PlanilhaServico.Utils;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;

namespace GerenciadorPlanilhaFinanceira.Servicos.PlanilhaServico.Servicos
{
    public class PlanilhaFinanceiroServico : IPlanilhaFinanceiroServico
    {

        public async Task<PersistenciaFinanceiro> TrataDespesasNaoParceladas(PlanilhaFinanceiroRequest request)
        {
            if (Enum.TryParse<MesesEnum>(request.MesRelacionado, ignoreCase: true, out var mesEnum))
            {
                Console.WriteLine($"Compra será em {request.MesRelacionado}");

                PersistenciaFinanceiro persistencia = PopularDespesa(request, 0);

                await CriarDespesaPlanilha(request, 0, request.MesRelacionado);

                return persistencia;
            }

            return null;
        }

        public async Task<List<PersistenciaFinanceiro>> TratarDespesasParceladas(int parcelas, PlanilhaFinanceiroRequest request)
        {
            List<PersistenciaFinanceiro> listaPersistencia = new();

            for (var parcela = 0; parcela < parcelas; parcela++)
            {
                if (Enum.TryParse<MesesEnum>(request.MesRelacionado, ignoreCase: true, out var mesEnum))
                {
                    int mesAtual = ((int)mesEnum - 1 + parcela) % 12 + 1;

                    MesesEnum mesParcela = (MesesEnum)mesAtual;

                    Console.WriteLine($"Parcela {parcela + 1} será em {mesParcela}");

                    PersistenciaFinanceiro persistencia = PopularDespesa(request, parcela + 1);

                    listaPersistencia.Add(persistencia);

                    await CriarDespesaPlanilha(request, parcela + 1, mesParcela.ToString());
                }
            }

            return listaPersistencia;
        }
              
        private PersistenciaFinanceiro PopularDespesa(PlanilhaFinanceiroRequest request, int parcela)
        {
            PersistenciaFinanceiro persistencia = new();
            persistencia.NomeDespesa = request.NomeDespesa;
            persistencia.TipoDespesa = request.TipoDespesa;
            persistencia.MesRelacionado = request.MesRelacionado;
            persistencia.Responsavel = request.Responsavel;
            persistencia.Categoria = request.Categoria;
            persistencia.DataCriacao = request.DataCriacao;
            persistencia.FormaPagamento = request.FormaPagamento;
            persistencia.Identificador = request.Identificador;
            persistencia.Valor = request.Valor;
            persistencia.Parcela = parcela;

            return persistencia;
        }

        private async Task CriarDespesaPlanilha(PlanilhaFinanceiroRequest request, int parcela, string pagina)
        {
            try
            {
                var service = new SheetsService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = BuscarArquivoCredencial(),
                    ApplicationName = "PlanilhaFinanceira",
                });

                string spreadsheetId = "10lsLAVdVqRoRN9ezDKvyvIcycReIcXfNIzZZ-Jx9aoQ";
                string range = $"{pagina}!A1";

                var valueRange = new ValueRange
                {
                    Values = new List<IList<object>>
                    {
                        new List<object> {
                            request.NomeDespesa,
                            request.Valor,
                            request.TipoDespesa,
                            request.Categoria,
                            request.FormaPagamento,
                            request.CompraParcelada == true? "Sim" : "Não",
                            request.Parcela > 0 ? $"{parcela} de {request.Parcela}" : "",
                            request.Responsavel,
                            request.DataCriacao.Date
                        }
                    }
                };

                var appendRequest = service.Spreadsheets.Values.Append(valueRange, spreadsheetId, range);
                appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.RAW;

                var result = await appendRequest.ExecuteAsync();

            }
            catch (Exception ex)
            {
                Console.WriteLine("deu erro ao criar ", ex);
                throw;
            }
        }

        public async Task EditarSincronizacaoPlanilha(int linha, string pagina, string identificadoLinha)
        {
            try
            {
                var service = new SheetsService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = BuscarArquivoCredencial(),
                    ApplicationName = "PlanilhaFinanceira",
                });

                string spreadsheetId = "10lsLAVdVqRoRN9ezDKvyvIcycReIcXfNIzZZ-Jx9aoQ";
                string range = $"{pagina}!{identificadoLinha}{linha}";
                var valueRange = new ValueRange
                {
                    Values = new List<IList<object>> { new List<object> { "Sim" } }
                };

                var updateRequest = service.Spreadsheets.Values.Update(valueRange, spreadsheetId, range);
                updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;
                var result = await updateRequest.ExecuteAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("deu tudo errado ", ex);
                throw;
            }
        }

        private GoogleCredential BuscarArquivoCredencial()
        {
            var credPath = Path.Combine(AppContext.BaseDirectory, "careful-granite-442820-r3-c34c4c0a85eb.json");

            var credential = GoogleCredential
                .FromFile(credPath)
                .CreateScoped(SheetsService.Scope.Spreadsheets);

            return credential;
        }
    }
}
