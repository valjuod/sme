using Moq;
using Sme.BankingApi.Commands.Customer;
using Sme.BankingApi.Data.Model;
using Sme.BankingApi.Data.Repository;

namespace Sme.BankingApi.Commands.Tests
{
    public class UpgradeCustomerToVipCommandTest
    {
        private readonly Mock<ICustomerRepository> _customerRepositoryMock;
        private readonly UpgradeCustomerToVipCommandHandler _handler;

        public UpgradeCustomerToVipCommandTest()
        {
            _customerRepositoryMock = new Mock<ICustomerRepository>();

            _handler = new UpgradeCustomerToVipCommandHandler(_customerRepositoryMock.Object);
        }

        [SetUp]
        public void Setup()
        {
            _customerRepositoryMock.Reset();
        }

        [Test]
        public async Task WhenCustomerDoesNotExist_ShouldFail()
        {
            // Arrange
            var customerId = 1;
            _customerRepositoryMock.Setup(x => x.Find(It.IsAny<int>())).Returns((CustomerModel?)null);

            // Act
            var result = await _handler.Handle(new UpgradeCustomerToVipCommand(customerId), CancellationToken.None);

            // Assert
            Assert.IsFalse(result.Success);
            Assert.True(result.Errors.Any(x => x.Code == "CUSTOMER_NOT_FOUND"));
        }

        [Test]
        public async Task WhenCustomerIsVip_ShouldFail()
        {
            // Arrange
            var customerId = 1;
            _customerRepositoryMock.Setup(x => x.Find(customerId)).Returns(new CustomerModel()
            {
                Id = customerId,
                IsVip = true
            });

            // Act
            var result = await _handler.Handle(new UpgradeCustomerToVipCommand(customerId), CancellationToken.None);

            Assert.IsFalse(result.Success);
            Assert.True(result.Errors.Any(x => x.Code == "INVALID_OPERATION"));
        }

        [Test]
        public async Task WhenCustomerIsNotVip_ShouldSucceed()
        {
            // Arrange
            var customerId = 1;
            _customerRepositoryMock.Setup(x => x.Find(customerId)).Returns(new CustomerModel()
            {
                Id = customerId,
                IsVip = false
            });

            // Act
            var result = await _handler.Handle(new UpgradeCustomerToVipCommand(customerId), CancellationToken.None);

            // Assert
            Assert.IsTrue(result.Success);
        }
    }
}