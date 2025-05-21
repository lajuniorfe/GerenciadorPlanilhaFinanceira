using GerenciadorPlanilhaFinanceira.Servicos.PlanilhaServico.Entidades;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using System.Text.Json;

namespace GerenciadorPlanilhaFinanceira.Servicos.PlanilhaServico.Servicos
{
    public class PlanilhaFinanceiroServico : IPlanilhaFinanceiroServico
    {
        public async void TratarMensagemDespesaRecebida(string mensagem)
        {
            GoogleSheetFinanceiroRequest jsonMensagem = JsonSerializer.Deserialize<GoogleSheetFinanceiroRequest>(mensagem);

            List<PlanilhaFinanceiroRequest> listaPlanilhaFinanceiroValores = new();

            PlanilhaFinanceiroRequest request = new PlanilhaFinanceiroRequest();

            request.DataCriaçao = Convert.ToDateTime(jsonMensagem.Values[0]);
            request.NomeDespesa = jsonMensagem.Values[1];
            request.Valor = Convert.ToDecimal(jsonMensagem.Values[2]);
            request.TipoDespesa = jsonMensagem.Values[3];
            request.Categoria = jsonMensagem.Values[4];
            request.FormaPagamento = jsonMensagem.Values[5];
            request.CompraParcelada = jsonMensagem.Values[6] == "Não" ? false : true;
            request.Parcela = jsonMensagem.Values[7] == "" ? 0 : Convert.ToInt32(jsonMensagem.Values[8]);
            request.Responsavel = jsonMensagem.Values[8];
            request.MesRelacionado = jsonMensagem.Values[9];


            // criar despesas parceladas para cada mes correspondente e gravar na planilha e no banco
            // Eu quero armazenar no banco de dados


            // alterar a planilha para marcar que foi sincronizada com o banco de dados
            await EditarPlanilha(jsonMensagem.Row, jsonMensagem.Sheet);

            Console.WriteLine("deu tudo certo");
        }

        public async Task EditarPlanilha(int linha, string pagina)
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

            Console.WriteLine("o caminho", credPath);
            return credential;
        }
    }
}
