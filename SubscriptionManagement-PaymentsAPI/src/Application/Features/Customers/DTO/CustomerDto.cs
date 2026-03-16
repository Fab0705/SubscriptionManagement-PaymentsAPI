using System;
using System.Collections.Generic;
using System.Text;

namespace SubscriptionManagement_PaymentsAPI.Application.Features.Customers.DTO;

public class CustomerDto
{
    public Guid Id { get; set; }
    /*public Guid TenantId { get; set; }*/
    public string Email { get; set; } = null!;
    public string? StripeCustomerId { get; set; }
}
