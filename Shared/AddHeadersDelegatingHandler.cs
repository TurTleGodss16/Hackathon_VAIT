namespace Hackathon_VAIT_New.Shared;

public class AddHeadersDelegatingHandler : DelegatingHandler
{
    public AddHeadersDelegatingHandler() : base(new HttpClientHandler())
    {
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        request.Headers.Add("Authorization", "Bearer " + Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
        return base.SendAsync(request, cancellationToken);
    }
}