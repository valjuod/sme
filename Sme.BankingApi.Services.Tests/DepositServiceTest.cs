using Moq;
using NUnit.Framework.Internal;
using Sme.BankingApi.Data.Model;
using Sme.BankingApi.Data.Repository;
using Sme.BankingApi.Services.Exceptions;
using Sme.BankingApi.Services.Models;
using Sme.BankingApi.Services.Transfer;

namespace Sme.BankingApi.Services.Tests
{
    public class DepositServiceTest
    {
        private Mock<IAccountRepository> _accountRepositoryMock;
        private Mock<ITransactionRepository> _transactionRepositoryMock;
        private readonly string _accountNumber;
        private readonly DepositService _service;

        public DepositServiceTest()
        {
            _accountRepositoryMock = new Mock<IAccountRepository>();
            _transactionRepositoryMock = new Mock<ITransactionRepository>();
            _accountNumber = "LT1233333333333";

            _service = new DepositService(_accountRepositoryMock.Object, _transactionRepositoryMock.Object);
        }

        [SetUp]
        public void Setup()
        {
            _accountRepositoryMock.Reset();
            _transactionRepositoryMock.Reset();
        }

        [Test]
        public void WhenAccountClosed_ShouldFail()
        {
            // Arrange
            var amount = 10;

            _accountRepositoryMock.Setup(x => x.Find(_accountNumber)).Returns(new AccountModel()
            {
                Number = _accountNumber,
                Status = AccountStatus.Closed
            });

            // Act
            TestDelegate method = () =>_service.DoTransfer(new TransferRequest(_accountNumber, amount, Currency.EUR, DateTime.Now));

            // Assert
            var ex = Assert.Throws<TransferException>(method);
            Assert.That(ex.Code == "ACCOUNT_NOT_OPENED");
        }

        [Test]
        public void WhenAccountDoesNotExist_ShouldFail()
        {
            // Arrange
            var amount = 10;

            _accountRepositoryMock.Setup(x => x.Find(_accountNumber)).Returns((AccountModel)null);

            // Act
            TestDelegate method = () => _service.DoTransfer(new TransferRequest(_accountNumber, amount, Currency.EUR, DateTime.Now));

            // Assert
            var ex = Assert.Throws<TransferException>(method);
            Assert.That(ex.Code == "ACCOUNT_NOT_FOUND");
        }

        [Test]
        public void WhenAccountCurrencyDoesNotMatch_ShouldFail()
        {
            // Arrange
            var amount = 10;

            _accountRepositoryMock.Setup(x => x.Find(_accountNumber)).Returns(new AccountModel()
            {
                Number = _accountNumber,
                Status = AccountStatus.Opened,
                Currency = Currency.USD,
            });

            // Act
            TestDelegate method = () => _service.DoTransfer(new TransferRequest(_accountNumber, amount, Currency.EUR, DateTime.Now));

            // Assert
            var ex = Assert.Throws<TransferException>(method);
            Assert.That(ex.Code == "ACCOUNT_CURRENCY_MISMATCH");
        }

        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(0.001)]
        public void WhenTransactionAmountIsInvalid_ShouldFail(decimal amount)
        {
            // Arrange
            var currency = Currency.EUR;

            _accountRepositoryMock.Setup(x => x.Find(_accountNumber)).Returns(new AccountModel()
            {
                Number = _accountNumber,
                Status = AccountStatus.Opened,
                Currency = currency,
            });

            // Act
            TestDelegate method = () => _service.DoTransfer(new TransferRequest(_accountNumber, amount, currency, DateTime.Now));

            // Assert
            var ex = Assert.Throws<TransferException>(method);
            Assert.That(ex.Code == "TRANSACTION_MIN_AMOUNT");
        }

        [Test]
        public void WhenBalanceWouldGoAboveLimit_ShouldFail()
        {
            // Arrange
            var amount = 10000000000000;
            var balance = 5;
            var currency = Currency.EUR;

            _accountRepositoryMock.Setup(x => x.Find(_accountNumber)).Returns(new AccountModel()
            {
                Number = _accountNumber,
                Status = AccountStatus.Opened,
                Currency = currency,
                Balance = balance,
            });

            // Act
            TestDelegate method = () => _service.DoTransfer(new TransferRequest(_accountNumber, amount, currency, DateTime.Now));

            // Assert
            var ex = Assert.Throws<TransferException>(method);
            Assert.That(ex.Code == "ACCOUNT_BALANCE_REACHED");
        }

        [TestCase(0d, 10, 10)]
        [TestCase(10, 10, 20)]
        public void WhenDepositAmount_TransactionShouldBeCreated(decimal oldBalance,
            decimal transactionAmount,
            decimal newBalance)
        {
            // Arrange
            var currency = Currency.EUR;
            var date = DateTime.Now;

            _accountRepositoryMock.Setup(x => x.Find(_accountNumber)).Returns(new AccountModel()
            {
                Number = _accountNumber,
                Status = AccountStatus.Opened,
                Currency = currency,
                Balance = oldBalance,
            });

            AccountModel? account = null;
            List<TransactionModel> createdTransactions = new List<TransactionModel>();

            _transactionRepositoryMock.Setup(x => x.Insert(It.IsAny<TransactionModel>())).Callback((TransactionModel obj) =>
            {
                createdTransactions.Add(obj);
            });

            _accountRepositoryMock.Setup(x => x.Update(It.IsAny<AccountModel>())).Callback((AccountModel obj) =>
            {
                account = obj;
            });

            // Act
            _service.DoTransfer(new TransferRequest(_accountNumber, transactionAmount, currency, date));

            // Assert
            Assert.True(createdTransactions.Count == 1);

            var transaction = createdTransactions.First();

            Assert.AreEqual(newBalance, account.Balance);
            Assert.AreEqual(newBalance, transaction.Balance);
            Assert.AreEqual(transactionAmount, transaction.Amount);
            Assert.AreEqual(currency, transaction.Currency);
            Assert.AreEqual(date, transaction.Timestamp);
        }
    }
}