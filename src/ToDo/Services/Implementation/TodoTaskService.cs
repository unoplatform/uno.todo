using Microsoft.Extensions.Options;
using Refit;
using ToDo.Models;
using ToDo.Models.DataModels;
using ToDo.Services.Interface;

namespace ToDo.Services.Implementation
{
    public class TodoTaskService : ITodoTaskService
    {
        private readonly MicrosoftGraphSettings _microsoftGraphSettings;
        private readonly RefitSettings _refitSettings;
        private readonly ITodoTaskServiceApi _todoTaskService;

        public TodoTaskService(IOptions<MicrosoftGraphSettings> microsoftGraphSettings)
        {
            _microsoftGraphSettings = microsoftGraphSettings.Value;
            _refitSettings = new RefitSettings
            {
                AuthorizationHeaderValueGetter = GetAccessToken
            };
            _todoTaskService = RestService.For<ITodoTaskServiceApi>(_microsoftGraphSettings.Endpoint, _refitSettings);
        }

        private Task<string> GetAccessToken()
        {
            return Task.FromResult(_microsoftGraphSettings.AccessToken);
        }

        public async Task<TodoTask> CreateTask(string listId, object newTask, CancellationToken ct)
        {
            try
            {
                return await _todoTaskService.CreateTask(listId, newTask, ct);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task DeleteTask(string listId, string taskId, CancellationToken ct)
        {
            try
            {
                await _todoTaskService.DeleteTask(listId, taskId, ct);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Task<TodoTask> GetTask(string listId, string taskId, CancellationToken ct)
        {
            try
            {
                return _todoTaskService.GetTask(listId, taskId, ct);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Task<TodoTask> UpdateTask(string listId, string taskId, object updatedTask, CancellationToken ct)
        {
            try
            {
                return _todoTaskService.UpdateTask(listId, taskId, updatedTask, ct);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
