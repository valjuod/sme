using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sme.BankingApi.Commands.Common;
using Sme.BankingApi.Commands.Customer;
using Sme.BankingApi.Extensions;
using System.Security.Principal;

namespace Sme.BankingApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CustomerController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// API consumer can make customer VIP
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(List<ResultError>))]
        public async Task<IActionResult> UpgradeToVip(int id)
        {
            var result = await _mediator.Send(new UpgradeCustomerToVipCommand(id));
            return this.CommandToHttpResult(result);
        }
    }

}