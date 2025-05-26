using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GerenciadorPlanilhaFinanceira.Servicos.PlanilhaServico.Entidades
{
    public class PersistenciaFinanceiro
    {
        public DateTime DataCriacao { get; set; }
        public string NomeDespesa { get; set; }
        public decimal Valor { get; set; }
        public string TipoDespesa { get; set; }
        public string Categoria { get; set; }
        public string FormaPagamento { get; set; }
        public int Parcela { get; set; }
        public string Responsavel { get; set; }
        public string MesRelacionado { get; set; }
        public string Identificador { get; set; }
    }
}
