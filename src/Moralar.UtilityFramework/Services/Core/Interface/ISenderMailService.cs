
namespace Moralar.UtilityFramework.Services.Core.Interface
{
    public interface ISenderMailService
    {
        void SendMessageEmail(string nome, string email, string body, string subject, string titleEvent = null, bool invite = false, string local = null, DateTime? inicio = null, DateTime? utc = null, DateTime? fim = null, bool recorrente = false, string pathFile = null, List<string> ccEmails = null, List<string> ccoEmails = null, List<string> replyTo = null);

        void SendMessageEmail(string nome, List<string> email, string body, string subject, string titleEvent = null, bool invite = false, string local = null, DateTime? inicio = null, DateTime? utc = null, DateTime? fim = null, bool recorrente = false, string pathFile = null, List<string> ccoEmails = null, List<string> replyTo = null);

        void SendMessageEmailAmazon(string nome, List<string> email, string body, string subject, string titleEvent = null, bool invite = false, string local = null, DateTime? inicio = null, DateTime? utc = null, DateTime? fim = null, bool recorrente = false, string pathFile = null, List<string> ccoEmails = null, List<string> replyTo = null);

        Task SendMessageEmailAsync(string nome, string email, string body, string subject, string titleEvent = null, bool invite = false, string local = null, DateTime? inicio = null, DateTime? utc = null, DateTime? fim = null, bool recorrente = false, string pathFile = null, List<string> ccEmails = null, List<string> ccoEmails = null, List<string> replyTo = null, bool configureAwait = false);

        Task SendMessageEmailAsync(string nome, List<string> email, string body, string subject, string titleEvent = null, bool invite = false, string local = null, DateTime? inicio = null, DateTime? utc = null, DateTime? fim = null, bool recorrente = false, string pathFile = null, List<string> ccoEmails = null, List<string> replyTo = null, bool configureAwait = false);

        Task SendMessageEmailAmazonAsync(string nome, List<string> email, string body, string subject, string titleEvent = null, bool invite = false, string local = null, DateTime? inicio = null, DateTime? utc = null, DateTime? fim = null, bool recorrente = false, string pathFile = null, List<string> ccoEmails = null, List<string> replyTo = null, bool configureAwait = false);

        string GerateBody(string filename, Dictionary<string, string> substituicao);
    }
}
