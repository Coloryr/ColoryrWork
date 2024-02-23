namespace ColoryrServer.ASP;

public interface IHttpClients
{
    public HttpClient GetOne();
}

public class HttpClients(IHttpClientFactory clientFactory) : IHttpClients
{
    public HttpClient GetOne()
    {
        return clientFactory.CreateClient();
    }
}
