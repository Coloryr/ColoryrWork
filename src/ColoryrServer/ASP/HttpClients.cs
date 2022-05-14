namespace ColoryrServer.ASP
{
    public interface IHttpClients
    {
        public HttpClient GetOne();
    }
    public class HttpClients : IHttpClients
    {
        private readonly IHttpClientFactory _clientFactory;

        public HttpClients(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public HttpClient GetOne()
        {
            return _clientFactory.CreateClient();
        }
    }
}
