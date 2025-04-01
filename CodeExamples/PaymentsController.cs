[Route("api/[controller]")]
[ApiController]
public class PaymentsController : ControllerBase
{
    private readonly StripeSettings _stripeSettings;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public PaymentsController(IOptions<StripeSettings> stripeSettings, ApplicationDbContext context, 
        UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        _stripeSettings = stripeSettings.Value;
        _context = context;
        _userManager = userManager;
        _signInManager = signInManager;
        StripeConfiguration.ApiKey = Environment.GetEnvironmentVariable("STRIPE_SECRET_KEY"); 
    }

    [HttpPost("create-checkout-session")]
    public async Task<ActionResult> CreateCheckoutSession()
    {
        if (!User.Identity.IsAuthenticated)
        {
            return Unauthorized(new { message = "User not authenticated" });
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }

        var options = new SessionCreateOptions
        {
            PaymentMethodTypes = new List<string> { "card" },
            LineItems = new List<SessionLineItemOptions>
            {
                new SessionLineItemOptions
                {
                    Price = _stripeSettings.PriceCode,
                    Quantity = 1,
                },
            },
            AutomaticTax = new SessionAutomaticTaxOptions { Enabled = true },
            BillingAddressCollection = "required",
            Mode = "payment",
            SuccessUrl = $"{Request.Scheme}://{Request.Host}/success",
            CancelUrl = $"{Request.Scheme}://{Request.Host}/cancel",
            CustomerEmail = user.Email,
            ConsentCollection = new SessionConsentCollectionOptions { TermsOfService = "required" }
        };

        var service = new SessionService();
        Session session = await service.CreateAsync(options);

        return Ok(new { sessionId = session.Id, checkoutUrl = session.Url });
    }

    [HttpPost("webhook")]
    public async Task<IActionResult> Webhook()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        try
        {
            var stripeEvent = EventUtility.ConstructEvent(json, 
                Request.Headers["Stripe-Signature"], 
                Environment.GetEnvironmentVariable("STRIPE_WEBHOOK_SECRET")); 

            if (stripeEvent.Type == Events.CheckoutSessionCompleted)
            {
                var session = stripeEvent.Data.Object as Session;
                var user = await _userManager.FindByEmailAsync(session.CustomerEmail);

                if (user != null)
                {
                    user.IsPremium = true;

                    var purchase = new Purchases
                    {
                        ApplicationUserId = user.Id,
                        PurchaseDate = DateTime.UtcNow,
                        Amount = (session.AmountTotal ?? 0) / 100M,
                    };

                    _context.Purchases.Add(purchase);
                    await _context.SaveChangesAsync();
                }
            }

            return Ok();
        }
        catch (StripeException e)
        {
            return BadRequest(new { error = e.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }
}
