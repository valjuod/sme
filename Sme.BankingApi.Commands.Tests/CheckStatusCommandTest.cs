using Moq;
using Sme.BankingApi.Commands.Account;
using Sme.BankingApi.Commands.Customer;
using Sme.BankingApi.Data.Model;
using Sme.BankingApi.Data.Repository;

namespace Sme.BankingApi.Commands.Tests
{
    public class CheckStatusCommandTest
    {
        private readonly Mock<IAccountRepository> _accountRepository;
        private readonly string _accountNumber;
        private readonly CheckStatusCommandHandler _handler;

        public CheckStatusCommandTest()
        {
            _accountRepository = new Mock<IAccountRepository>();
            _accountNumber = "LT1233333333333";

            _handler = new CheckStatusCommandHandler(_accountRepository.Object);
        }

        [SetUp]
        public void Setup()
        {
            _accountRepository.Reset();
        }

        [Test]
        public async Task WhenAccountDoesNotExist_ShouldFail()
        {
            // Arrange
            _accountRepository.Setup(x => x.Find(It.IsAny<string>())).Returns((AccountModel?)null);

            // Act
            var result = await _handler.Handle(new CheckStatusCommand(_accountNumber), CancellationToken.None);

            // Assert
            Assert.IsFalse(result.Success);
            Assert.True(result.Errors.Any(x => x.Code == "ACCOUNT_NOT_FOUND"));
        }

        [TestCase(AccountStatus.Opened)]
        [TestCase(AccountStatus.Closed)]
        public async Task WhenAccountExists_ShouldSucceed(AccountStatus status)
        {
            // Arrange
            _accountRepository.Setup(x => x.Find(_accountNumber)).Returns(new AccountModel()
            {
                Number = _accountNumber,
                Status = status
            });

            // Act
            var result = await _handler.Handle(new CheckStatusCommand(_accountNumber), CancellationToken.None);

            // Assert
            Assert.IsTrue(result.Success);
            Assert.AreEqual(status, result.Data);
        }

    }
}