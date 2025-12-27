using NUnit.Framework;
using PocketMechanic.ViewModels;

namespace PocketMechanic.Tests
{
    [TestFixture]
    public class MainViewModelTests
    {
        private MainViewModel _viewModel;

        [SetUp]
        public void Setup()
        {
            _viewModel = new MainViewModel();
        }

        [Test]
        public void Test_InitialState()
        {
            // Arrange

            // Act

            // Assert
            Assert.IsNotNull(_viewModel);
            // Add more assertions based on the initial state of MainViewModel
        }

        // Add more tests for MainViewModel methods and properties
    }
}