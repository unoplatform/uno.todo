using Microsoft.Extensions.Options;
using Task = System.Threading.Tasks.Task;
using NUnit.Framework;

namespace ToDo.Tests.Services
{
    internal class TaskListEndpointTests: BaseEndpointTests<IToDoTaskListEndpoint>
    {

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task Create_TodoList_ShouldReturn_Created()
        {
            //Arrange
            ToDoTaskListRequestData todoList = new ToDoTaskListRequestData()
            {
                DisplayName = "New Todo List"
            };

            //Act
            var result = await service.CreateAsync(todoList, System.Threading.CancellationToken.None);

            //Assert
            Assert.IsNotNull(result, "The ToDo List can't be created");
        }

        [Test]
        public async Task Create_TodoList_ShouldReturn_BadRequest()
        {
            //Arrange
            ToDoTaskListRequestData todoList = new ToDoTaskListRequestData();
            //Act
            var result = await service.CreateAsync(todoList, System.Threading.CancellationToken.None);

			//Assert
			Assert.IsNotNull(result, "The ToDo List can't be created");
        }

        [Test]
        public async Task Delete_TodoList_ShouldReturn_NotFound()
        {
            //Arrange
            string idTodoList = "AAMkAGM0ZTZiY2IwLTliZWEtNDM5Zi1iMDBlLTUxZDQxNWNmY2IxNgAuAAAAAAAC8Egk03A8QrAy_y5u1QQAAQD-PT2STVFATpxIXsYfLHGvAADbMaF_ABA=";
            //Act
            var result = await service.DeleteAsync(idTodoList, System.Threading.CancellationToken.None);

            //Assert
            Assert.IsTrue(result.StatusCode == System.Net.HttpStatusCode.NotFound, "The ToDo List have an error");
        }


        [Test]
        public async Task Delete_TodoList_ShouldReturn_NoContent()
        {
            //Arrange
            string idTodoList = "AAMkAGM0ZTZiY2IwLTliZWEtNDM5Zi1iMDBlLTUxZDQxNWNmY2IxNgAuAAAAAAAC8Egk03A8QrAy_y5u1QQAAQD-PT2STVFATpxIXsYfLHGvAADcfLguAAA=";
            //Act
            var result = await service.DeleteAsync(idTodoList, System.Threading.CancellationToken.None);

            //Assert
            Assert.IsTrue(result.StatusCode == System.Net.HttpStatusCode.NoContent, "The ToDo List wasn't delete");
        }

        [Test]
        public async Task Get_TodoList_ShouldReturn_NotFound()
        {
            //Arrange
            string idTodoList = "AAMkAGM0ZTZiY2IwLTliZWEtNDM5Zi1iMDBlLTUxZDQxNWNmY2IxNgAuAAAAAAAC8Egk03A8QrAy_y5u1QQAAQD-PT2STVFATpxIXsYfLHGvAADbMaF_ABA=";
            //Act
            var result = await service.GetAsync(idTodoList, System.Threading.CancellationToken.None);

            //Assert
            Assert.IsTrue(result == default, "The ToDo List was find");
        }


        [Test]
        public async Task Get_TodoList_ShouldReturn_TodoList()
        {
            //Arrange
            string idTodoList = "AAMkAGM0ZTZiY2IwLTliZWEtNDM5Zi1iMDBlLTUxZDQxNWNmY2IxNgAuAAAAAAAC8Egk03A8QrAy_y5u1QQAAQD-PT2STVFATpxIXsYfLHGvAADWiFSuAAA=";
            //Act
            var result = await service.GetAsync(idTodoList, System.Threading.CancellationToken.None);

            //Assert
            Assert.IsTrue(result != default, "The ToDo List wasn't find");
        }

        [Test]
        public async Task Get_TodoLists_ShouldReturn_TodoLists()
        {
            //Arrange
            //Act
            var result = await service.GetAllAsync(System.Threading.CancellationToken.None);

            //Assert
            Assert.IsTrue(result != default, "The ToDo List wasn't find");
        }

        [Test]
        public async Task Update_TodoList_ShouldReturn_NotFound()
        {
            //Arrange
            string idTodoList = "AAMkAGM0ZTZiY2IwLTliZWEtNDM5Zi1iMDBlLTUxZDQxNWNmY2IxNgAuAAAAAAAC8Egk03A8QrAy_y4u1QQAAQD-PT2STVFATpxIXsYfLHGvAADWiFSuAAA=";
            ToDoTaskListRequestData todoList = new ToDoTaskListRequestData()
            {
                DisplayName = "New Todo List Updated"
            };

            //Act
            var result = await service.UpdateAsync(idTodoList, todoList, System.Threading.CancellationToken.None);

			//Assert
			Assert.IsNotNull(result, "The ToDo List found");
        }

        [Test]
        public async Task Update_TodoLists_ShouldReturn_Ok()
        {
            //Arrange
            string idTodoList = "AAMkAGM0ZTZiY2IwLTliZWEtNDM5Zi1iMDBlLTUxZDQxNWNmY2IxNgAuAAAAAAAC8Egk03A8QrAy_y5u1QQAAQD-PT2STVFATpxIXsYfLHGvAADWiFSuAAA=";
            ToDoTaskListRequestData todoList = new ToDoTaskListRequestData()
            {
                DisplayName = "New Todo List Updated"
            };

            //Act
            var result = await service.UpdateAsync(idTodoList, todoList, System.Threading.CancellationToken.None);

			//Assert
			Assert.IsNotNull(result, "The ToDo List wasn't find");
        }
    }
}
