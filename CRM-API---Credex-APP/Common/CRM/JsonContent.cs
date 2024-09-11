using System.Net.Http;
using System.Text;


namespace CRM_API___Credex.Common
{
    internal class JsonContent : StringContent
    {
        internal JsonContent(string content)
          : this(content, Encoding.UTF8)
        {
        }

        internal JsonContent(string content, Encoding encoding)
          : base(content, encoding, "application/json")
        {
        }
    }
}