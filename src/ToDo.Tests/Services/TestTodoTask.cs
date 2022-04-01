using Microsoft.Extensions.Options;
using NUnit.Framework;
using System.Threading;
using System.Threading.Tasks;
using ToDo.Models;
using ToDo.Models.DataModels;
using ToDo.Services.Implementation;
using ToDo.Services.Interface;

namespace ToDo.Tests.Services
{
    internal class TestTodoTask
    {
        private readonly ITodoTaskService _todoTaskService;
        public TestTodoTask()
        {
            var accessToken = "";
            var endpoint = "https://graph.microsoft.com/v1.0/me";
            MicrosoftGraphSettings microsoftGraphSettings = new MicrosoftGraphSettings(endpoint, accessToken);
            IOptions<MicrosoftGraphSettings> options = Options.Create(microsoftGraphSettings);
            _todoTaskService = new TodoTaskService(options);
        }

        [SetUp]
        public void Setup() { }

        [Test]
        public async Task Create_TodoTask_ShouldReturn_NewTask()
        {
            //Arrange
            var listId = "AAMkAGFlMTMyOTVlLTg4MTYtNGNkYi05Y2I1LWIxNjQ3MjQzZGUwZgAuAAAAAABxiwJ7rbfvTL0IfGDSJ4lUAQAstIhkSEopRrR__AvQNI34AACzQA1BAAA=";
            var newTask = new { title = "new task" };
            //Act
            var result = await _todoTaskService.CreateTask(listId, newTask, CancellationToken.None);

            //Assert
            Assert.IsInstanceOf<TodoTask>(result);
        }

        [Test]
        public async Task Get_TodoTask_ShouldReturnTask()
        {
            //Arrange
            var listId = "AAMkAGFlMTMyOTVlLTg4MTYtNGNkYi05Y2I1LWIxNjQ3MjQzZGUwZgAuAAAAAABxiwJ7rbfvTL0IfGDSJ4lUAQAstIhkSEopRrR__AvQNI34AACzQA1BAAA=";
            var taskId = "AAMkAGFlMTMyOTVlLTg4MTYtNGNkYi05Y2I1LWIxNjQ3MjQzZGUwZgBGAAAAAABxiwJ7rbfvTL0IfGDSJ4lUBwAstIhkSEopRrR__AvQNI34AACzQA1BAAAstIhkSEopRrR__AvQNI34AACzQBkEAAA=";

            //Act
            var result = await _todoTaskService.GetTask(listId, taskId, CancellationToken.None);

            //Assert
            Assert.IsInstanceOf<TodoTask>(result);
        }
    }
}
