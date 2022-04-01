using Microsoft.Extensions.Options;
using NUnit.Framework;
using System.Threading;
using System.Threading.Tasks;
using ToDo.Business.Interface;
using ToDo.Data.Models;
using ToDo.Data.Models.DataModels;
using ToDo.Services.Interface;

namespace ToDo.Tests.Services
{
    internal class TestTodoTask
    {
        private readonly ITaskService _todoTaskService;
        public TestTodoTask()
        {
            MicrosoftGraphSettings microsoftGraphSettings = new MicrosoftGraphSettings()
            {
                AccessToken = "",
                Endpoint = "https://graph.microsoft.com/v1.0/me"
            };
            IOptions<MicrosoftGraphSettings> options = Options.Create(microsoftGraphSettings);
            _todoTaskService = new ToDo.Business.Implementation.Task(options);
        }

        [SetUp]
        public void Setup() { }

        [Test]
        public async System.Threading.Tasks.Task Create_TodoTask_ShouldReturn_NewTask()
        {
            //Arrange
            var listId = "AAMkAGFlMTMyOTVlLTg4MTYtNGNkYi05Y2I1LWIxNjQ3MjQzZGUwZgAuAAAAAABxiwJ7rbfvTL0IfGDSJ4lUAQAstIhkSEopRrR__AvQNI34AACzQA1BAAA=";
            var newTask = new { title = "new task" };
            //Act
            var result = await _todoTaskService.CreateAsync(listId, newTask, CancellationToken.None);

            //Assert
            Assert.IsInstanceOf<TaskData>(result);
        }

        [Test]
        public async System.Threading.Tasks.Task Get_TodoTask_ShouldReturnTask()
        {
            //Arrange
            var listId = "AAMkAGFlMTMyOTVlLTg4MTYtNGNkYi05Y2I1LWIxNjQ3MjQzZGUwZgAuAAAAAABxiwJ7rbfvTL0IfGDSJ4lUAQAstIhkSEopRrR__AvQNI34AACzQA1BAAA=";
            var taskId = "AAMkAGFlMTMyOTVlLTg4MTYtNGNkYi05Y2I1LWIxNjQ3MjQzZGUwZgBGAAAAAABxiwJ7rbfvTL0IfGDSJ4lUBwAstIhkSEopRrR__AvQNI34AACzQA1BAAAstIhkSEopRrR__AvQNI34AACzQBkEAAA=";

            //Act
            var result = await _todoTaskService.GetAsync(listId, taskId, CancellationToken.None);

            //Assert
            Assert.IsInstanceOf<TaskData>(result);
        }
    }
}
