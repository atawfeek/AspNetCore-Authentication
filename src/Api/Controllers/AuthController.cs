using AspNetCore.Authentication.Identity.Token.Interfaces;
using AspNetCore.Authentication.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCore.Authentication.Web.Controllers
{
    [Route("login")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ITokenIssuer _issuer;
        public AuthController(ITokenIssuer issuer)
        {
            _issuer = issuer;
        }
        // GET api/values
        [HttpPost, Route("login")]
        public IActionResult Login([FromBody] LoginModel user)
        {
            if (user == null)
            {
                return BadRequest("Invalid client request");
            }
            if (user.UserName == "johndoe" && user.Password == "def@123")
            {   
                return Ok(new { Token = _issuer.IssueAccessToken() });
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}
