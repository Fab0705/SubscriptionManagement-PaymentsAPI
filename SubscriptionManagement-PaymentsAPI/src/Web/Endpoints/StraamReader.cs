namespace SubscriptionManagement_PaymentsAPI.Web.Endpoints;

internal class StraamReader
{
    private Stream _body;

    public StraamReader(Stream body)
    {
        _body = body;
    }
}