using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace CRM_API___Credex.MessageHandler
{
    public abstract class LoggingMessageHandler : DelegatingHandler
    {
        public static string fileName = DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
        public static string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", "Traffic", fileName);
        public System.IO.StreamWriter logFile = new System.IO.StreamWriter(filePath, true);
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var corrId = string.Format("{0}{1}", DateTime.Now.Ticks, Thread.CurrentThread.ManagedThreadId);
            var requestInfo = string.Format("{0} {1}", request.Method, request.RequestUri);
            logFile.AutoFlush = true;
            var requestMessage = await request.Content.ReadAsByteArrayAsync();



            await IncommingMessageAsync(corrId, requestInfo, requestMessage);



            var response = await base.SendAsync(request, cancellationToken);



            byte[] responseMessage;



            if (response.IsSuccessStatusCode)
                responseMessage = await response.Content.ReadAsByteArrayAsync();
            else
                responseMessage = Encoding.UTF8.GetBytes(response.ReasonPhrase);



            await OutgoingMessageAsync(corrId, requestInfo, responseMessage);



            return response;
        }
        protected abstract Task IncommingMessageAsync(string correlationId, string requestInfo, byte[] message);
        protected abstract Task OutgoingMessageAsync(string correlationId, string requestInfo, byte[] message);
    }
    public class MessageLoggingHandler : LoggingMessageHandler
    {
        protected override async Task IncommingMessageAsync(string correlationId, string requestInfo, byte[] message)
        {
            await Task.Run(() =>
                logFile.WriteLine(string.Format("[{3}]{0} - Request: {1}\r\n{2}", correlationId, requestInfo, Encoding.UTF8.GetString(message), DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"))));
        }




        protected override async Task OutgoingMessageAsync(string correlationId, string requestInfo, byte[] message)
        {
            await Task.Run(() =>
                logFile.WriteLine(string.Format("[{3}]{0} - Response: {1}\r\n{2}", correlationId, requestInfo, Encoding.UTF8.GetString(message), DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"))));
        }
    }
}