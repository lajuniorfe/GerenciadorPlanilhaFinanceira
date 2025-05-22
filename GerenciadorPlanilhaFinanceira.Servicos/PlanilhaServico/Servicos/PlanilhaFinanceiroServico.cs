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

        public PlanilhaFinanceiroServico()
        {
        }

        public async Task TratarDespesasParceladas(int parcelas, PlanilhaFinanceiroRequest request)
        {
            for (var parcela = 0; parcela < parcelas; parcela++)
            {
                if (Enum.TryParse<MesesEnum>(request.MesRelacionado, ignoreCase: true, out var mesEnum))
                {
                    int mesAtual = ((int)mesEnum - 1 + parcela) % 12 + 1;

                    MesesEnum mesParcela = (MesesEnum)mesAtual;

                    Console.WriteLine($"Parcela {parcela + 1} será em {mesParcela}");

                    await CriarDespesaParceladaPlanilha(request, parcela + 1, mesParcela.ToString());
                }
            }
        }
                  

        private async Task CriarDespesaParceladaPlanilha(PlanilhaFinanceiroRequest request, int parcela, string pagina)
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
                            request.DataCriaçao.Date

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

        public async Task EditarSincronizacaoPlanilha(int linha, string pagina)
        {
            try
            {
                var service = new SheetsService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = BuscarArquivoCredencial(),
                    ApplicationName = "PlanilhaFinanceira",
                });

                string spreadsheetId = "10lsLAVdVqRoRN9ezDKvyvIcycReIcXfNIzZZ-Jx9aoQ";
                string range = $"{pagina}!K{linha}";
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

            var credential = GoogleCredential.FromFile(credPath)
                                              .CreateScoped(SheetsService.Scope.Spreadsheets);
            return credential;
        }
    }
}
