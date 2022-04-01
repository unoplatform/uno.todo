using Microsoft.Extensions.Options;
using Refit;
using ToDo.Business.Interface;
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

        public async Task<ResponseService<TaskData>> CreateAsync(string listId, TaskData newTask, CancellationToken ct)
        {
            try
            {
                var response = await _apiRequest.CreateAsync(listId, newTask, ct);
                return GetResponseService<TaskData>.GetResponse(System.Net.HttpStatusCode.OK, response);
            }
            catch (ApiException ex)
            {
                return GetResponseService<TaskData>.GetResponse(ex.StatusCode);

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

        public async Task<ResponseService<TaskData>> GetAsync(string listId, string taskId, CancellationToken ct)
        {
            try
            {
                var response = await _apiRequest.GetAsync(listId, taskId, ct);
                return GetResponseService<TaskData>.GetResponse(System.Net.HttpStatusCode.OK, response);
            }
            catch (ApiException ex)
            {
                return GetResponseService<TaskData>.GetResponse(ex.StatusCode);
            }
        }

        public async Task<ResponseService<TaskData>> UpdateAsync(string listId, string taskId, TaskData updatedTask, CancellationToken ct)
        {
            try
            {
                var response = await _apiRequest.UpdateAsync(listId, taskId, updatedTask, ct);
                return GetResponseService<TaskData>.GetResponse(System.Net.HttpStatusCode.OK, response);
            }
            catch (ApiException ex)
            {
                return GetResponseService<TaskData>.GetResponse(ex.StatusCode);
            }
        }
    }
}
