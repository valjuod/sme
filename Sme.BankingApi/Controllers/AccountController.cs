using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sme.BankingApi.Commands.Account;
using Sme.BankingApi.Commands.Common;
using Sme.BankingApi.Commands.Customer;
using Sme.BankingApi.Commands.Model;
using Sme.BankingApi.Data.Model;
using Sme.BankingApi.Extensions;

namespace Sme.BankingApi.Controllers
{
    

    [ApiController]
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AccountController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// API consumer can check Account status
        /// </summary>
        /// <param name="accountNumber"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AccountStatus))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(List<ResultError>))]
        public async Task<IActionResult> CheckStatus(string accountNumber)
        {
            var result = await _mediator.Send(new CheckStatusCommand(accountNumber));
            return this.CommandToHttpResult(result);
        }

        /// <summary>
        /// API consumer can perform Account balance inquiry
        /// </summary>
        /// <param name="accountNumber"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BalanceInquiryResult))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(List<ResultError>))]
        public async Task<IActionResult> BalanceInquiry(string accountNumber)
        {
            var result = await _mediator.Send(new BalanceInquiryCommand(accountNumber));
            return this.CommandToHttpResult(result);
        }

        /// <summary>
        /// API consumer can perform Account balance inquiry
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(List<ResultError>))]
        public async Task<IActionResult> Deposit([FromBody]TransactionRequest request)
        {
            var result = await _mediator.Send(new DepositCommand(request));
            return this.CommandToHttpResult(result);
        }

        /// <summary>
        /// API consumer can perform Account balance inquiry
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(List<ResultError>))]
        public async Task<IActionResult> Payment([FromBody] TransactionRequest request)
        {
            var result = await _mediator.Send(new PaymentCommand(request));
            return this.CommandToHttpResult(result);
        }
    }
}