using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using ToDo.Models;
using ToDo.Services.Implementation;
using ToDo.Services.Interface;
using NUnit.Framework;

namespace ToDo.Tests
{
    public class TodoListServiceTest
    {
        private readonly ITodoListService _todoListService;

        public TodoListServiceTest()
        {
            MicrosoftGraphSettings microsoftGraphSettings = new MicrosoftGraphSettings()
            {
                AccessToken = "eyJ0eXAiOiJKV1QiLCJub25jZSI6InJWa0ZsaDJ4bXJvdXdYc2JmUzJsQmRuMjJUYTlFcWl3OXVNMmFnZGtteFEiLCJhbGciOiJSUzI1NiIsIng1dCI6ImpTMVhvMU9XRGpfNTJ2YndHTmd2UU8yVnpNYyIsImtpZCI6ImpTMVhvMU9XRGpfNTJ2YndHTmd2UU8yVnpNYyJ9.eyJhdWQiOiIwMDAwMDAwMy0wMDAwLTAwMDAtYzAwMC0wMDAwMDAwMDAwMDAiLCJpc3MiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC9mM2VlZDljMi02OGU0LTQ0ZmQtOWJlOS05YjVjYzc1NzI2YjgvIiwiaWF0IjoxNjQ4NjQ4NTI1LCJuYmYiOjE2NDg2NDg1MjUsImV4cCI6MTY0ODY1MzkwNywiYWNjdCI6MCwiYWNyIjoiMSIsImFpbyI6IkUyWmdZSmdZZDFVNzFTYU9vWHpyaGVEdHEyWjdwbHVKc005Tis1ZHFJcHc3aDJ2NlJWa0EiLCJhbXIiOlsicHdkIl0sImFwcF9kaXNwbGF5bmFtZSI6IkdyYXBoIEV4cGxvcmVyIiwiYXBwaWQiOiJkZThiYzhiNS1kOWY5LTQ4YjEtYThhZC1iNzQ4ZGE3MjUwNjQiLCJhcHBpZGFjciI6IjAiLCJmYW1pbHlfbmFtZSI6IkFsZGFuYSIsImdpdmVuX25hbWUiOiJBbmRyw6lzIiwiaWR0eXAiOiJ1c2VyIiwiaXBhZGRyIjoiMTkwLjI2LjE2NS4zIiwibmFtZSI6IkFuZHLDqXMgQWxkYW5hIiwib2lkIjoiNWMxNTMxMmMtZTE5NC00ZGI3LWE5NWYtODllYWYwNWJjOTE1IiwicGxhdGYiOiIzIiwicHVpZCI6IjEwMDMyMDAxMzc2Q0FFODAiLCJwd2RfZXhwIjoiMzE0NjQ2NjE5IiwicHdkX3VybCI6Imh0dHBzOi8vcHJvZHVjdGl2aXR5LnNlY3VyZXNlcnZlci5uZXQvbWljcm9zb2Z0P21hcmtldGlkPWVuLVVTXHUwMDI2ZW1haWw9YW5kcmVzLmFsZGFuYSU0MGhvcnVzLmNvbS51eVx1MDAyNnNvdXJjZT1WaWV3VXNlcnNcdTAwMjZhY3Rpb249UmVzZXRQYXNzd29yZCIsInJoIjoiMC5BU1lBd3RudTgtUm9fVVNiNlp0Y3gxY211QU1BQUFBQUFBQUF3QUFBQUFBQUFBQW1BTXMuIiwic2NwIjoib3BlbmlkIHByb2ZpbGUgVGFza3MuUmVhZFdyaXRlIFVzZXIuUmVhZCBlbWFpbCIsInNpZ25pbl9zdGF0ZSI6WyJrbXNpIl0sInN1YiI6IjlCcTQxZFVPOTZCMG43VjFaZFM0UXpuNTVrbWNnLXRjSXItNDVfaXhwRUkiLCJ0ZW5hbnRfcmVnaW9uX3Njb3BlIjoiU0EiLCJ0aWQiOiJmM2VlZDljMi02OGU0LTQ0ZmQtOWJlOS05YjVjYzc1NzI2YjgiLCJ1bmlxdWVfbmFtZSI6ImFuZHJlcy5hbGRhbmFAaG9ydXMuY29tLnV5IiwidXBuIjoiYW5kcmVzLmFsZGFuYUBob3J1cy5jb20udXkiLCJ1dGkiOiJIZUJPRTluWjEwLXhKbUxIZGh3ZEFBIiwidmVyIjoiMS4wIiwid2lkcyI6WyJiNzlmYmY0ZC0zZWY5LTQ2ODktODE0My03NmIxOTRlODU1MDkiXSwieG1zX3N0Ijp7InN1YiI6ImZRdkNHdWxXSjZDanZ6YWNNdTRmZTlUcTk2WWE0NWhvRjB4S1A4ZjdELVEifSwieG1zX3RjZHQiOjE1MjEwNjY4ODd9.Exjl94y4y3qNYdzFMe8AkW424yh3w8IRCNHd5RPxLF2IPAG6_oX8thXa7_JrYoFp_3CA4maGfGLG0r8QbrK3Ao39e6brZZMId0Tlbsav6MZzU7T_uPBw9Vj2SAk7RpaE_V7u3MkcEOcJfGVkQBD4DIqihX3yEL0KHFbR4YQsXMivRfpo3wEeG0G5xkQsCkrNlH9o4WfOj8Wq7593E2W9FFgXA8eaFyA8No_v8X8QUKUuA094LSKGDESZrAiO9yUeoSkgf1KNEAr6YSOSO7P1NXt-ITaEUZndHo1vODaW8Z5V7gVsHdjOXl1qMQE87ECBXBST7MDA0zEC2f8fZxbewA",
                Endpoint = "https://graph.microsoft.com/v1.0/me"
            };
            IOptions<MicrosoftGraphSettings> options = Options.Create(microsoftGraphSettings);
            _todoListService = new TodoListService(options);
        }

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task Create_TodoList_ShouldReturn_Created()
        {
            //Arrange
            CreateUpdateTodoListObj todoList = new CreateUpdateTodoListObj()
            {
                DisplayName = "New Todo List"
            };

            //Act
            var result = await _todoListService.CreateTodoListAsync(todoList, System.Threading.CancellationToken.None);

            //Assert
            Assert.IsTrue(result.StatusCode == System.Net.HttpStatusCode.Created, "The ToDo List can't be created");
        }

        [Test]
        public async Task Create_TodoList_ShouldReturn_BadRequest()
        {
            //Arrange
            CreateUpdateTodoListObj todoList = new CreateUpdateTodoListObj();
            //Act
            var result = await _todoListService.CreateTodoListAsync(todoList, System.Threading.CancellationToken.None);

            //Assert
            Assert.IsTrue(result.StatusCode == System.Net.HttpStatusCode.BadRequest, "The ToDo List can't be created");
        }

        [Test]
        public async Task Delete_TodoList_ShouldReturn_NotFound()
        {
            //Arrange
            string idTodoList = "AAMkAGM0ZTZiY2IwLTliZWEtNDM5Zi1iMDBlLTUxZDQxNWNmY2IxNgAuAAAAAAAC8Egk03A8QrAy_y5u1QQAAQD-PT2STVFATpxIXsYfLHGvAADbMaF_ABA=";
            //Act
            var result = await _todoListService.DeleteTodoListAsync(idTodoList, System.Threading.CancellationToken.None);

            //Assert
            Assert.IsTrue(result.StatusCode == System.Net.HttpStatusCode.NotFound, "The ToDo List have an error");
        }


        [Test]
        public async Task Delete_TodoList_ShouldReturn_NoContent()
        {
            //Arrange
            string idTodoList = "AAMkAGM0ZTZiY2IwLTliZWEtNDM5Zi1iMDBlLTUxZDQxNWNmY2IxNgAuAAAAAAAC8Egk03A8QrAy_y5u1QQAAQD-PT2STVFATpxIXsYfLHGvAADcfLguAAA=";
            //Act
            var result = await _todoListService.DeleteTodoListAsync(idTodoList, System.Threading.CancellationToken.None);

            //Assert
            Assert.IsTrue(result.StatusCode == System.Net.HttpStatusCode.NoContent, "The ToDo List wasn't delete");
        }

        [Test]
        public async Task Get_TodoList_ShouldReturn_NotFound()
        {
            //Arrange
            string idTodoList = "AAMkAGM0ZTZiY2IwLTliZWEtNDM5Zi1iMDBlLTUxZDQxNWNmY2IxNgAuAAAAAAAC8Egk03A8QrAy_y5u1QQAAQD-PT2STVFATpxIXsYfLHGvAADbMaF_ABA=";
            //Act
            var result = await _todoListService.GetTodoListAsync(idTodoList, System.Threading.CancellationToken.None);

            //Assert
            Assert.IsTrue(result.StatusCode == System.Net.HttpStatusCode.NotFound && result.Data == default, "The ToDo List was find");
        }


        [Test]
        public async Task Get_TodoList_ShouldReturn_TodoList()
        {
            //Arrange
            string idTodoList = "AAMkAGM0ZTZiY2IwLTliZWEtNDM5Zi1iMDBlLTUxZDQxNWNmY2IxNgAuAAAAAAAC8Egk03A8QrAy_y5u1QQAAQD-PT2STVFATpxIXsYfLHGvAADWiFSuAAA=";
            //Act
            var result = await _todoListService.GetTodoListAsync(idTodoList, System.Threading.CancellationToken.None);

            //Assert
            Assert.IsTrue(result.StatusCode == System.Net.HttpStatusCode.OK && result.Data != default, "The ToDo List wasn't find");
        }

        [Test]
        public async Task Get_TodoLists_ShouldReturn_TodoLists()
        {
            //Arrange
            //Act
            var result = await _todoListService.GetTodoListsAsync(System.Threading.CancellationToken.None);

            //Assert
            Assert.IsTrue(result.StatusCode == System.Net.HttpStatusCode.OK && result.Data != default, "The ToDo List wasn't find");
        }

        [Test]
        public async Task Update_TodoList_ShouldReturn_NotFound()
        {
            //Arrange
            string idTodoList = "AAMkAGM0ZTZiY2IwLTliZWEtNDM5Zi1iMDBlLTUxZDQxNWNmY2IxNgAuAAAAAAAC8Egk03A8QrAy_y4u1QQAAQD-PT2STVFATpxIXsYfLHGvAADWiFSuAAA=";
            CreateUpdateTodoListObj todoList = new CreateUpdateTodoListObj()
            {
                DisplayName = "New Todo List Updated"
            };

            //Act
            var result = await _todoListService.UpdateTodoListAsync(idTodoList, todoList, System.Threading.CancellationToken.None);

            //Assert
            Assert.IsTrue(result.StatusCode == System.Net.HttpStatusCode.NotFound, "The ToDo List found");
        }

        [Test]
        public async Task Update_TodoLists_ShouldReturn_Ok()
        {
            //Arrange
            string idTodoList = "AAMkAGM0ZTZiY2IwLTliZWEtNDM5Zi1iMDBlLTUxZDQxNWNmY2IxNgAuAAAAAAAC8Egk03A8QrAy_y5u1QQAAQD-PT2STVFATpxIXsYfLHGvAADWiFSuAAA=";
            CreateUpdateTodoListObj todoList = new CreateUpdateTodoListObj()
            {
                DisplayName = "New Todo List Updated"
            };

            //Act
            var result = await _todoListService.UpdateTodoListAsync(idTodoList, todoList, System.Threading.CancellationToken.None);

            //Assert
            Assert.IsTrue(result.StatusCode == System.Net.HttpStatusCode.OK, "The ToDo List wasn't find");
        }


    }
}
