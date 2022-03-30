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
            var accessToken = "eyJ0eXAiOiJKV1QiLCJub25jZSI6IkhMeVM3RkZxSTBjVTRDMlB1UjdHMFdPWGFRVk5XN0JpM21Zb25nWFJRZFUiLCJhbGciOiJSUzI1NiIsIng1dCI6ImpTMVhvMU9XRGpfNTJ2YndHTmd2UU8yVnpNYyIsImtpZCI6ImpTMVhvMU9XRGpfNTJ2YndHTmd2UU8yVnpNYyJ9.eyJhdWQiOiIwMDAwMDAwMy0wMDAwLTAwMDAtYzAwMC0wMDAwMDAwMDAwMDAiLCJpc3MiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC9mM2VlZDljMi02OGU0LTQ0ZmQtOWJlOS05YjVjYzc1NzI2YjgvIiwiaWF0IjoxNjQ4NjE5MDc2LCJuYmYiOjE2NDg2MTkwNzYsImV4cCI6MTY0ODYyMzkyMywiYWNjdCI6MCwiYWNyIjoiMSIsImFpbyI6IkFTUUEyLzhUQUFBQXRNVlg2RnBObjVWT1RJb25QcVNtT3N6NUhVSHIrUm5EN016QkNRUUJ3SEE9IiwiYW1yIjpbInB3ZCJdLCJhcHBfZGlzcGxheW5hbWUiOiJHcmFwaCBFeHBsb3JlciIsImFwcGlkIjoiZGU4YmM4YjUtZDlmOS00OGIxLWE4YWQtYjc0OGRhNzI1MDY0IiwiYXBwaWRhY3IiOiIwIiwiZmFtaWx5X25hbWUiOiJZYWh1aXJhIiwiZ2l2ZW5fbmFtZSI6IkNhcmzDrW4iLCJpZHR5cCI6InVzZXIiLCJpcGFkZHIiOiIxOTAuMjMyLjEwNi4xMSIsIm5hbWUiOiJDYXJsw61uIFlhaHVpcmEiLCJvaWQiOiIwZTJkYmE4Zi1kMmMxLTRhYjItYTIwYS03YmUxOTc1ZjMxMWYiLCJwbGF0ZiI6IjMiLCJwdWlkIjoiMTAwMzIwMDE1Njg1M0E5NiIsInB3ZF9leHAiOiIzMTUyNzU0NTIiLCJwd2RfdXJsIjoiaHR0cHM6Ly9wcm9kdWN0aXZpdHkuc2VjdXJlc2VydmVyLm5ldC9taWNyb3NvZnQ_bWFya2V0aWQ9ZW4tVVNcdTAwMjZlbWFpbD1jYXJsaW4ueWFodWlyYSU0MGhvcnVzLmNvbS51eVx1MDAyNnNvdXJjZT1WaWV3VXNlcnNcdTAwMjZhY3Rpb249UmVzZXRQYXNzd29yZCIsInJoIjoiMC5BU1lBd3RudTgtUm9fVVNiNlp0Y3gxY211QU1BQUFBQUFBQUF3QUFBQUFBQUFBQW1BUGsuIiwic2NwIjoib3BlbmlkIHByb2ZpbGUgVGFza3MuUmVhZFdyaXRlIFVzZXIuUmVhZCBlbWFpbCIsInNpZ25pbl9zdGF0ZSI6WyJrbXNpIl0sInN1YiI6IjVFNExvUFpRbWVZQVZic3hnMGJLbEc3RlpOeGtIckZic1JnRVlwM0twSkUiLCJ0ZW5hbnRfcmVnaW9uX3Njb3BlIjoiU0EiLCJ0aWQiOiJmM2VlZDljMi02OGU0LTQ0ZmQtOWJlOS05YjVjYzc1NzI2YjgiLCJ1bmlxdWVfbmFtZSI6ImNhcmxpbi55YWh1aXJhQGhvcnVzLmNvbS51eSIsInVwbiI6ImNhcmxpbi55YWh1aXJhQGhvcnVzLmNvbS51eSIsInV0aSI6Ik1McVdueGJFajBLWHBycUt1SThhQUEiLCJ2ZXIiOiIxLjAiLCJ3aWRzIjpbImI3OWZiZjRkLTNlZjktNDY4OS04MTQzLTc2YjE5NGU4NTUwOSJdLCJ4bXNfc3QiOnsic3ViIjoiTUEzRmJpbTR2R1VLemY2cTZsb2w5SVN2eTllMGxMNGd4MXZIaG93ZzRqTSJ9LCJ4bXNfdGNkdCI6MTUyMTA2Njg4N30.O7XGlTETkAW2Qm4scoujvxAaBrp6I0l-SYobNazpsFgsBON5-M4upIoqU5xCsTb04_3EsN7ZO8LcO8m_AjBWifHoPVjvBJqSrMqsOSE2D5zKiIvTBnr_R38rbIJHiHJ7WM0-RtMDIMD_s-yOS8YE47fkVaWMkc_U7j3GogIwJ5rfBoyfSMAbtGfDwGB5O0tkTp7Xd4dvwzxEHVTLmef8GOg0gXqMwhjxet3ysdEy8Tr5afG8tiT0nB9UBNzPqGCnGvSLgIWgcv7MFuS6yz1WMewshlYCLjO1M0Zq0m5f72h9D5s9zSpjzM4NwGhLQlPGTny7CGyMHba0zIWduI_e1Q";
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
