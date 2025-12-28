using NUnit.Framework;
using PocketMechanic.ViewModels;
using PocketMechanic.Services;

namespace PocketMechanic.Tests
{
    [TestFixture]
    public class GarageViewModelTests
    {
        private GarageViewModel _viewModel;
        private DatabaseService _databaseService;

        [SetUp]
        public void Setup()
        {
            _databaseService = new DatabaseService();
            _viewModel = new GarageViewModel(_databaseService);
        }

        [Test]
        public void Test_InitialState()
        {
            // Arrange & Act
            // Assert
            Assert.IsNotNull(_viewModel);
            Assert.IsNotNull(_viewModel.Vehicles);
            Assert.AreEqual(0, _viewModel.Vehicles.Count);
        }

        // Add more tests for GarageViewModel methods and properties
    }
}