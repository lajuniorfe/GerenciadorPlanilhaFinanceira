using GerenciadorPlanilhaFinanceira.Servicos.PlanilhaServico.Entidades;
using GerenciadorPlanilhaFinanceira.Servicos.PlanilhaServico.Utils;
using GerenciadorPlanilhaFinanceira.Servicos.RabbitMqServico;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using System.Globalization;
using System.Text.Json;

namespace GerenciadorPlanilhaFinanceira.Servicos.PlanilhaServico.Servicos
{
    public class PlanilhaFinanceiroServico : IPlanilhaFinanceiroServico
    {

        public PlanilhaFinanceiroServico()
        {
        }

        public async void TratarMensagemDespesaRecebida(string mensagem)
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
                await TratarDespesasParceladas(request.Parcela, request);
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

            await EditarSincronizacaoPlanilha(linha, pagina);
        }

        private async Task TratarDespesasParceladas(int parcelas, PlanilhaFinanceiroRequest request)
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

        private async Task EditarSincronizacaoPlanilha(int linha, string pagina)
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
