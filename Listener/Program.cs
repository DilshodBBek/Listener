using System.Net;
using System.Text;

namespace Listener
{
    internal class Program
    {
        public static HttpClient _client;
        static void Main(string[] args)
        {
            Task.Factory.StartNew(() => Sender());
            Listener();


        }

        private static void Listener()
        {
            HttpListener _listener = new();
            _listener.Prefixes.Add("http://127.0.0.1:9519/");
            bool _enabled = false;
            while (!_enabled)
            {
                _listener.Start();
               // Console.WriteLine("Start listening ...");
                var context = _listener.GetContextAsync().Result;

                HttpListenerRequest req = context.Request;

                using Stream body = req.InputStream;
                using var reader = new StreamReader(body, req.ContentEncoding);
                string res1 = reader.ReadToEndAsync().Result;

                Console.WriteLine("Incoming Request:" + res1 + "\n");

                var res = context.Response;
                res.Cookies.Add(new CookieCollection() {
                    new Cookie("Username", "Dilshod"),
                    new Cookie("Password", "Psw")
                });
                string text = "Request successful handled";

                byte[] bytes = Encoding.UTF8.GetBytes(text);

                res.ContentLength64 = bytes.Length;
                res.OutputStream.WriteAsync(bytes);

                res.OutputStream.FlushAsync();
                //await Console.Out.WriteLineAsync("Dasturni tugatishni istaysizmi? 1 | 0 ");

                //Boolean.TryParse(Console.ReadLine(), out _enabled);

            }
            _listener.Stop();
            Console.WriteLine("Finished!");
        }
        private static void Sender()
        {
            while (true)
            {
                _client = new HttpClient();
                while (true)
                {
                    Console.WriteLine("Xabar kiriting!");
                    StringContent content = new(Console.ReadLine() ?? " no message!");
                    HttpResponseMessage response = _client.PostAsync("http://127.0.0.1:9587/", content).Result;

                    var cookie = new Cookie("Username", response.Headers.GetValues("Username").ToString());
                    var cookie1 = new Cookie("Password", response.Headers.GetValues("Password").ToString());
                    _client.DefaultRequestHeaders.Add(cookie.Name, cookie.Value);
                    _client.DefaultRequestHeaders.Add(cookie1.Name, cookie1.Value);
                    Console.WriteLine("So`rov natijasi:" + response.Content.ReadAsStringAsync());
                }
            }
        }
    }
}