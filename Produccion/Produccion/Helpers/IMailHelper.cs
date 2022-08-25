using Produccion.Common;

namespace Produccion.Helpers
{
    public interface IMailHelper
    {
        Response SendMail(string toName, string toEmail, string subject, string body);

    }
}
