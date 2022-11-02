using Moq;
using NUnit.Framework.Internal;
using Sme.BankingApi.Common;
using Sme.BankingApi.Data.Model;
using Sme.BankingApi.Data.Repository;
using Sme.BankingApi.Services.Exceptions;
using Sme.BankingApi.Services.Models;
using Sme.BankingApi.Services.Transfer;

namespace Sme.BankingApi.Services.Tests
{
    public class PaymentServiceTest
    {
        private Mock<IAccountRepository> _accountRepositoryMock;
        private Mock<ITransactionRepository> _transactionRepositoryMock;
        private readonly Mock<ICustomerRepository> _customerRepositoryMock;
        private readonly CustomerSettings _settings;
        private readonly string _accountNumber;
        private readonly PaymentService _service;

        public PaymentServiceTest()
        {
            _accountRepositoryMock = new Mock<IAccountRepository>();
            _transactionRepositoryMock = new Mock<ITransactionRepository>();
            _customerRepositoryMock = new Mock<ICustomerRepository>();
            _settings = new CustomerSettings()
            {
                VipRewardPercentage = 0
            };

            _accountNumber = "LT1233333333333";

            _service = new PaymentService(_accountRepositoryMock.Object, 
                _transactionRepositoryMock.Object, 
                _customerRepositoryMock.Object,
                _settings);
        }

        [SetUp]
        public void Setup()
        {
            _accountRepositoryMock.Reset();
            _transactionRepositoryMock.Reset();
            _settings.VipRewardPercentage = 0;
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
        public void WhenBalanceWouldDropBelowLimit_ShouldFail()
        {
            // Arrange
            var amount = 10;
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
            Assert.That(ex.Code == "ACCOUNT_BALANCE_TOO_LOW");
        }
      
   
        [TestCase(10, 10, 0, false, 0)]
        [TestCase(20, 10, 10, false, 0)]
        [TestCase(10, 10, 0, true, 0)]
        [TestCase(10, 10, 1, true, 10)]
        public void WhenPayAmount_TransactionShouldBeCreated(decimal oldBalance,
            decimal transactionAmount,
            decimal newBalance,
            bool isVipCustomer,
            decimal rewardPercentage)
        {
            // Arrange
            var currency = Currency.EUR;
            var date = DateTime.Now;
            var customerId = 1;
            var shouldHaveReward = isVipCustomer && rewardPercentage > 0;

            _accountRepositoryMock.Setup(x => x.Find(_accountNumber)).Returns(new AccountModel()
            {
                Number = _accountNumber,
                Status = AccountStatus.Opened,
                Currency = currency,
                Balance = oldBalance,
                CustomerId = customerId
            });

            _customerRepositoryMock.Setup(x => x.Find(customerId)).Returns(new CustomerModel()
            {
                Id = customerId,
                IsVip = isVipCustomer
            });

            _settings.VipRewardPercentage = rewardPercentage;

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
            Assert.True(createdTransactions.Count >= (shouldHaveReward ? 2 : 1));

            var paymentTransaction = createdTransactions.First();

            Assert.AreEqual(newBalance, account.Balance);
            Assert.AreEqual(oldBalance - transactionAmount, paymentTransaction.Balance);
            Assert.AreEqual(transactionAmount, paymentTransaction.Amount);
            Assert.AreEqual(currency, paymentTransaction.Currency);
            Assert.AreEqual(date, paymentTransaction.Timestamp);
        }
    }
}