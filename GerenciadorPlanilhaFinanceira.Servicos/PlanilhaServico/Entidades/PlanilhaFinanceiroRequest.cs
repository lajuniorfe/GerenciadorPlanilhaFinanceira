using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GerenciadorPlanilhaFinanceira.Servicos.PlanilhaServico.Entidades
{
    public class PlanilhaFinanceiroRequest
    {
        public DateTime DataCriaçao { get; set; }
        public string NomeDespesa { get; set; }
        public decimal Valor { get; set; }
        public string TipoDespesa { get; set; }
        public string Categoria { get; set; }
        public string FormaPagamento { get; set; }
        public bool CompraParcelada { get; set; }
        public int Parcela {  get; set; }
        public string Responsavel { get; set; }
        public string MesRelacionado { get; set; }
    }

    public class GoogleSheetFinanceiroRequest
    {
        public string Evento { get; set; } // "FORM_SUBMIT"
        public string Sheet { get; set; }  // Nome da aba
        public int Row { get; set; }       // Número da linha inserida
        public List<string> Values { get; set; } // Dados da linha
        public Dictionary<string, List<string>> Named { get; set; } // Pergunta → [resposta]
        public DateTime Time { get; set; }  // Timestamp ISO 8601
    }
}
						