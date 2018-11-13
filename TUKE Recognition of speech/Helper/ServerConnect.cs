using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace TUKE_Recognition_of_speech.Helper
{
    public static class ServerConnect
    {
        private static HttpWebRequest webRequest;
        public static string response;
        public static async Task SendAudioGetResponse(byte[] buffer, TUKEasr form)
        {

            try
            {
                webRequest = (HttpWebRequest)WebRequest.Create("http:google.ba");
                webRequest.Method = "POST";
                webRequest.ContentType = "application/x-www-form-urlencoded";
                Stream data = webRequest.GetRequestStream();
                data.Write(buffer, 0, buffer.Length);
                data.Close();

                WebResponse WebResponse = await webRequest.GetResponseAsync();


                StreamReader reader =  new StreamReader(WebResponse.GetResponseStream());
                form.WriteOutResponse(reader.ReadToEnd());
               
            }
            catch (WebException exception)
            {
                form.WriteOutResponse(exception.Message.ToString());
            }
        }        
        
    }
}
