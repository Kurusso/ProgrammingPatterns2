using Microsoft.AspNetCore.Mvc;

namespace UserSettings.Controllers;

[ApiController]
[Route("api/hiddenAccount")]
public class HiddenAccountController : ControllerBase
{
    [HttpGet("Accounts")]
    public ActionResult GetAccountsVisibilities()
    {
        
        return Ok();
    }
    
    [HttpPut]
    public ActionResult ChangeAccountVisibility()
    {
        return Ok();
    }
}