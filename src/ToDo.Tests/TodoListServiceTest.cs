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
                AccessToken = "",
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
