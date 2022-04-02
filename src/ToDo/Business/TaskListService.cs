using Microsoft.Extensions.Options;
using Refit;

namespace ToDo;

public class TaskListService : ITaskListService
{
    private readonly MicrosoftGraphSettings? _microsoftGraphSettings;
    private readonly RefitSettings _refitSettings;
    private ITaskListEndpoint _apiRequest;

    public TaskListService(IOptions<MicrosoftGraphSettings> microsoftGraphSettings)
    {
        _microsoftGraphSettings = microsoftGraphSettings?.Value;
        _refitSettings = new RefitSettings();
        _refitSettings.AuthorizationHeaderValueGetter = GetAccessToken;
        _apiRequest = RestService.For<ITaskListEndpoint>(_microsoftGraphSettings?.Endpoint ?? "", _refitSettings);
    }

    public Task<string> GetAccessToken()
    {
        return System.Threading.Tasks.Task.FromResult(_microsoftGraphSettings?.AccessToken ?? "");
    }

    public async Task<Response<HttpResponseMessage>> CreateAsync(TaskListRequestData todoList, CancellationToken ct)
    {
        try
        {
            var response = await _apiRequest.CreateAsync(todoList, ct);
            return ResponseData<HttpResponseMessage>.Get(System.Net.HttpStatusCode.OK, response);
        }
        catch (ApiException ex)
        {
            return ResponseData<HttpResponseMessage>.Get(ex.StatusCode);
        }
    }

    public async Task<Response<HttpResponseMessage>> DeleteAsync(string todoTaskListId, CancellationToken ct)
    {
        try
        {
            var response = await _apiRequest.DeleteAsync(todoTaskListId, ct);
            return ResponseData<HttpResponseMessage>.Get(System.Net.HttpStatusCode.OK, response);
        }
        catch (ApiException ex)
        {
            return ResponseData<HttpResponseMessage>.Get(ex.StatusCode);
        }
    }

    public async Task<Response<TaskListData>> GetAsync(string todoTaskListId, CancellationToken ct)
    {
        try
        {
            var response = await _apiRequest.GetAsync(todoTaskListId, ct);
            return ResponseData<TaskListData>.Get(System.Net.HttpStatusCode.OK, response);
        }
        catch (ApiException ex)
        {
            return ResponseData<TaskListData>.Get(ex.StatusCode);
        }
    }

    public async Task<Response<List<TaskListData>>> GetAllAsync(CancellationToken ct)
    {
        try
        {
            var response = await _apiRequest.GetAllAsync(ct);
            return ResponseData<List<TaskListData>>.Get(System.Net.HttpStatusCode.OK, response.Value);
        }
        catch (ApiException ex)
        {
            return ResponseData<List<TaskListData>>.Get(ex.StatusCode);
        }
    }

    public async Task<Response<HttpResponseMessage>> UpdateAsync(string todoTaskListId, TaskListRequestData todo, CancellationToken ct)
    {
        try
        {
            var response = await _apiRequest.UpdateAsync(todoTaskListId, todo, ct);
            return ResponseData<HttpResponseMessage>.Get(System.Net.HttpStatusCode.OK, response);
        }
        catch (ApiException ex)
        {
            return ResponseData<HttpResponseMessage>.Get(ex.StatusCode);
        }
    }

    public async Task<Response<List<TaskData>?>> GetTasksAsync(string todoTaskListId, CancellationToken ct)
    {
        try
        {
            var response = await _apiRequest.GetTasksAsync(todoTaskListId, ct);
            return ResponseData<List<TaskData>?>.Get(System.Net.HttpStatusCode.OK, response.Value);
        }
        catch (ApiException ex)
        {
            return ResponseData<List<TaskData>?>.Get(ex.StatusCode);
        }
    }
}
