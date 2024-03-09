using Microsoft.AspNetCore.Mvc;

namespace client_bank_backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CreditController:ControllerBase
{
    private readonly HttpClient _coreClient = new();
}