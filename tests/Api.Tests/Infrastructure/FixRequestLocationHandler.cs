namespace Api.Tests.Infrastructure;

/// <summary>
/// The fix request location handler.
/// </summary>
public class FixRequestLocationHandler : DelegatingHandler
{
    private readonly int _portNumber;

    public FixRequestLocationHandler(int portNumber, HttpMessageHandler innerHandler)
        : base(innerHandler)
    {
        _portNumber = portNumber;
    }

    /// <summary>
    /// Override of the SendAsync method to allow for reconstruction of the request uri to point to the dynamic testcontainer
    /// port number. This needs to be done as otherwise it defaults back to 8081. I assume there is some hard coded port in
    /// the emulator somewhere. If this is not done then the requests are not successful.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        request.RequestUri = new Uri($"https://localhost:{_portNumber}{request.RequestUri.PathAndQuery}");
        HttpResponseMessage response = await base.SendAsync(request, cancellationToken);
        return response;
    }
}