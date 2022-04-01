using Microsoft.Extensions.Options;
using Refit;
using ToDo.Business.Interface;
using ToDo.Data.Models.DataModels;
using ToDo.Data.Models;
using ToDo.Services;

namespace ToDo.Business.Implementation
{
    public class Task : ITaskService
    {
        private readonly MicrosoftGraphSettings _microsoftGraphSettings;
        private readonly RefitSettings _refitSettings;
        private readonly ITaskEndpoint _apiRequest;

        public Task(IOptions<MicrosoftGraphSettings> microsoftGraphSettings)
        {
            _microsoftGraphSettings = microsoftGraphSettings.Value;
            _refitSettings = new RefitSettings
            {
                AuthorizationHeaderValueGetter = GetAccessToken
            };
            _apiRequest = RestService.For<ITaskEndpoint>(_microsoftGraphSettings.Endpoint ?? "", _refitSettings);
        }

        private Task<string> GetAccessToken()
        {
            return System.Threading.Tasks.Task.FromResult(_microsoftGraphSettings.AccessToken ?? "");
        }

        public async Task<TaskData> CreateAsync(string listId, object newTask, CancellationToken ct)
        {
            try
            {
                return await _apiRequest.CreateAsync(listId, newTask, ct);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async System.Threading.Tasks.Task DeleteAsync(string listId, string taskId, CancellationToken ct)
        {
            try
            {
                await _apiRequest.DeleteAsync(listId, taskId, ct);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Task<TaskData> GetAsync(string listId, string taskId, CancellationToken ct)
        {
            try
            {
                return _apiRequest.GetAsync(listId, taskId, ct);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Task<TaskData> UpdateAsync(string listId, string taskId, object updatedTask, CancellationToken ct)
        {
            try
            {
                return _apiRequest.UpdateAsync(listId, taskId, updatedTask, ct);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
