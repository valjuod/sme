using Moq;
using Sme.BankingApi.Commands.Account;
using Sme.BankingApi.Commands.Model;
using Sme.BankingApi.Data.Model;
using Sme.BankingApi.Services.Models;
using Sme.BankingApi.Services.Transfer;

namespace Sme.BankingApi.Commands.Tests
{
    public class PaymentCommandTest
    {
        private readonly Mock<IPaymentService> _paymentService;
        private readonly PaymentCommandHandler _handler;

        public PaymentCommandTest()
        {
            _paymentService = new Mock<IPaymentService>();
            _handler = new PaymentCommandHandler(_paymentService.Object);
        }

        [SetUp]
        public void Setup()
        {
            _paymentService.Reset();
        }

        [Test]
        public async Task WhenParametersValid_ShouldSucceed()
        {
            // Arrange
            var request = new TransactionRequest()
            {
                AccountNumber = "LT1233333333333",
                Amount = 100,
                Currency = Currency.EUR,
                Date = DateTime.Now
            };

            _paymentService.Setup(x => x.DoTransfer(It.IsAny<TransferRequest>()));

            // Act
            var result = await _handler.Handle(new PaymentCommand(request), CancellationToken.None);

            // Assert
            Assert.IsTrue(result.Success);
            _paymentService.Verify(x => x.DoTransfer(It.Is<TransferRequest>(
                x => x.AccountNumber == request.AccountNumber
                && x.Currency == request.Currency
                && x.Amount == request.Amount
                && x.Date == request.Date
            )), Times.Once);
        }
    }
}