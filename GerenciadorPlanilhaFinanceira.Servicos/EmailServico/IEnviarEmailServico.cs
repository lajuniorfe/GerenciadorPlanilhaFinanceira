using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GerenciadorPlanilhaFinanceira.Servicos.EmailServico
{
    public interface IEnviarEmailServico
    {
        void EnviarEmailAsync(string para, string assunto, string corpo);
    }
}
