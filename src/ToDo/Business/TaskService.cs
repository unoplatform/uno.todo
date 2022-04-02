using Microsoft.Extensions.Options;
using Refit;

namespace ToDo;
public class TaskService : ITaskService
{
    private readonly MicrosoftGraphSettings _microsoftGraphSettings;
    private readonly RefitSettings _refitSettings;
    private readonly ITaskEndpoint _apiRequest;

    public TaskService(IOptions<MicrosoftGraphSettings> microsoftGraphSettings)
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

    public async Task<Response<TaskData>> CreateAsync(string listId, TaskData newTask, CancellationToken ct)
    {
        try
        {
            var response = await _apiRequest.CreateAsync(listId, newTask, ct);
            return ResponseData<TaskData>.Get(System.Net.HttpStatusCode.OK, response);
        }
        catch (ApiException ex)
        {
            return ResponseData<TaskData>.Get(ex.StatusCode);

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

    public async Task<Response<TaskData>> GetAsync(string listId, string taskId, CancellationToken ct)
    {
        try
        {
            var response = await _apiRequest.GetAsync(listId, taskId, ct);
            return ResponseData<TaskData>.Get(System.Net.HttpStatusCode.OK, response);
        }
        catch (ApiException ex)
        {
            return ResponseData<TaskData>.Get(ex.StatusCode);
        }
    }

    public async Task<Response<TaskData>> UpdateAsync(string listId, string taskId, TaskData updatedTask, CancellationToken ct)
    {
        try
        {
            var response = await _apiRequest.UpdateAsync(listId, taskId, updatedTask, ct);
            return ResponseData<TaskData>.Get(System.Net.HttpStatusCode.OK, response);
        }
        catch (ApiException ex)
        {
            return ResponseData<TaskData>.Get(ex.StatusCode);
        }
    }
}
