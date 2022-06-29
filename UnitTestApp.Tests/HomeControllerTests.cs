using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Moq;
using UnitTestApp.Controllers;
using UnitTestApp.Models;
using Xunit;

namespace UnitTestApp.Tests
{
    public class HomeControllerTest
    {
        [Fact]
        public void IndexReturnsAViewResultWithAListOfUsers()
        {
            //Arrange
            var mock = new Mock<IRepository>();
            mock.Setup(repo => repo.GetAll()).Returns(GetTestUser());
            HomeController controller = new HomeController(mock.Object);

            //Act
            var result = controller.Index();

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<User>>(viewResult.Model);
            Assert.Equal(GetTestUser().Count, model.Count());
        }

        private List<User> GetTestUser()
        {
            var users = new List<User>
            {
                new User { Id = 1, Name = "Tom", Age = 35 },
                new User { Id = 2, Name = "Alice", Age = 29 },
                new User { Id = 3, Name = "John", Age = 32 },
                new User { Id = 4, Name = "David", Age = 35 }
            };

            return users;
        }

        [Fact]
        public void AddUserReturnsViewResultWithUserModel()
        {
            //Arrange
            var mock = new Mock<IRepository>();
            var controller = new HomeController(mock.Object);
            controller.ModelState.AddModelError("Name", "Required");
            User newUser = new User();

            //Act
            var result = controller.AddUser();

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(null,viewResult?.Model);
            // Assert.Equal(newUser, viewResult?.Model);
        }

        [Fact]
        public void AddUserReturnsARedirectAndAddUser()
        {
            //Arrange
            var mock = new Mock<IRepository>();
            var controller = new HomeController(mock.Object);
            var newUser = new User()
            {
                Name = "Anton"
            };

            //Act
            var result = controller.AddUser(newUser);

            //Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            mock.Verify(r => r.Create(newUser));
        }

        [Fact]
        public void GetUserReturnsBadRequestResultWhenIdIsNull()
        {
            //Arrange
            var mock = new Mock<IRepository>();
            var controller = new HomeController(mock.Object);

            // Act
            var result = controller.GetUser(null);

            //Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public void GetUserNotFoundResultWhenUserNotFound()
        {
            //Arrange
            var mock = new Mock<IRepository>();
            var controller = new HomeController(mock.Object);

            //Act
            var result = controller.GetUser(10);

            //Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void GetUserReturnsViewResultWithUser()
        {
            //Arrange
            int testUserId = 1;
            var mock = new Mock<IRepository>();
            mock.Setup(repo => repo.Get(testUserId)).Returns(GetTestUser().FirstOrDefault(x => x.Id == testUserId));
            var controller = new HomeController(mock.Object);

            //Act
            var result = controller.GetUser(testUserId);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<User>(viewResult.Model);
            Assert.Equal("Tom", model.Name);
            Assert.Equal(35, model.Age);
            Assert.Equal(testUserId, model.Id);
        }
    }
}