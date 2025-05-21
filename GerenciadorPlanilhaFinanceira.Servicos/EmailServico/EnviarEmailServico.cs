using System.Net;
using System.Net.Mail;

namespace GerenciadorPlanilhaFinanceira.Servicos.EmailServico
{
    public class EnviarEmailServico : IEnviarEmailServico
    {
        public void EnviarEmailAsync(string para, string assunto, string corpo)
        {
            Console.WriteLine("cheguei aqui enviado!");

            var fromAddress = new MailAddress("lajunior@outlool.com.br", "Sistema Planilha");
            var toAddress = new MailAddress(para);
            const string fromPassword = "La130603!";

            using var smtp = new SmtpClient
            {
                Host = "smtp.office365.com",  // servidor SMTP do Outlook
                Port = 587,                   // porta TLS
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };

            using var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = assunto,
                Body = corpo
            };

            smtp.Send(message);

            Console.WriteLine("Email enviado!");
        }
    }
}
