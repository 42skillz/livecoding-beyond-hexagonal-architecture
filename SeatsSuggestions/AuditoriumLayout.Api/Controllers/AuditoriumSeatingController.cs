using System.Threading.Tasks;
using ExternalDependencies;
using Microsoft.AspNetCore.Mvc;

namespace AuditoriumLayout.Api.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/data_for_auditoriumSeating")]
    public class AuditoriumSeatingController : ControllerBase
    {
        private readonly IProvideAuditoriumLayouts _provideAuditoriumLayouts;

        public AuditoriumSeatingController(IProvideAuditoriumLayouts provideAuditoriumLayouts)
        {
            _provideAuditoriumLayouts = provideAuditoriumLayouts;
        }

        // GET api/data_for_auditoriumSeating/5
        [HttpGet("{showId}")]
        public async Task<ActionResult<AuditoriumDto>> Get(string showId)
        {
            return await _provideAuditoriumLayouts.GetAuditoriumSeatingFor(showId);
        }
    }
}