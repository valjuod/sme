using Moq;
using Sme.BankingApi.Commands.Account;
using Sme.BankingApi.Commands.Model;
using Sme.BankingApi.Data.Model;
using Sme.BankingApi.Services.Models;
using Sme.BankingApi.Services.Transfer;

namespace Sme.BankingApi.Commands.Tests
{
    public class DepositCommandTest
    {
        private readonly Mock<IDepositService> _depositService;
        private readonly DepositCommandHandler _handler;

        public DepositCommandTest()
        {
            _depositService = new Mock<IDepositService>();
            _handler = new DepositCommandHandler(_depositService.Object);
        }

        [SetUp]
        public void Setup()
        {
            _depositService.Reset();
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

            _depositService.Setup(x => x.DoTransfer(It.IsAny<TransferRequest>()));

            // Act
            var result = await _handler.Handle(new DepositCommand(request), CancellationToken.None);

            // Assert
            Assert.IsTrue(result.Success);
            _depositService.Verify(x => x.DoTransfer(It.Is<TransferRequest>(
                x => x.AccountNumber == request.AccountNumber
                && x.Currency == request.Currency
                && x.Amount == request.Amount
                && x.Date == request.Date
            )), Times.Once);
        }
    }
}