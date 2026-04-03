using SubscriptionManagement_PaymentsAPI.Application.Features.Webhooks.Commands;

namespace SubscriptionManagement_PaymentsAPI.Web.Endpoints;

public class Webhooks : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapPost(StripeWebhook, "stripe").AllowAnonymous();
    }
    public async Task<IResult> StripeWebhook(ISender sender, HttpRequest request)
    {
        using var reader = new StreamReader(request.Body);
        var jsonPayload = await reader.ReadToEndAsync();

        var stripeSignature = request.Headers["Stripe-Signature"].ToString();

        try
        {
            var command = new PorcessStripeWebhookCommand(jsonPayload, stripeSignature);
            await sender.Send(command);

            return Results.Ok();
        }
        catch (Stripe.StripeException e)
        {
            return Results.BadRequest(e.Message);
        }
    }
}
