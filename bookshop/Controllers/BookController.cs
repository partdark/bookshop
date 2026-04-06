using Microsoft.AspNetCore.Mvc;

namespace bookshop.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BookController : ControllerBase
    {
        [HttpGet("test")]
       public async Task<IActionResult> test ()
        {
            return Ok();
        }
    }
}
