using Microsoft.Extensions.Options;
using Refit;
using ToDo.Business.Interface;
using ToDo.Data.Models;
using ToDo.Services.Interface;

namespace ToDo.Business.Implementation;

public class TaskList : ITaskListService
{
    private readonly MicrosoftGraphSettings? _microsoftGraphSettings;
    private readonly RefitSettings _refitSettings;
    private ITaskListEndpoint _apiRequest;

    public TaskList(IOptions<MicrosoftGraphSettings> microsoftGraphSettings)
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

    public async Task<ResponseService<HttpResponseMessage>> CreateAsync(TaskListRequestData todoList, CancellationToken ct)
    {
        try
        {
            var response = await _apiRequest.CreateAsync(todoList, ct);
            return GetResponseService<HttpResponseMessage>.GetResponse(System.Net.HttpStatusCode.OK, response);
        }
        catch (ApiException ex)
        {
            return GetResponseService<HttpResponseMessage>.GetResponse(ex.StatusCode);
        }
    }

    public async Task<ResponseService<HttpResponseMessage>> DeleteAsync(string todoTaskListId, CancellationToken ct)
    {
        try
        {
            var response = await _apiRequest.DeleteAsync(todoTaskListId, ct);
            return GetResponseService<HttpResponseMessage>.GetResponse(System.Net.HttpStatusCode.OK, response);
        }
        catch (ApiException ex)
        {
            return GetResponseService<HttpResponseMessage>.GetResponse(ex.StatusCode);
        }
    }

    public async Task<ResponseService<TaskListData>> GetAsync(string todoTaskListId, CancellationToken ct)
    {
        try
        {
            var response = await _apiRequest.GetAsync(todoTaskListId, ct);
            return GetResponseService<TaskListData>.GetResponse(System.Net.HttpStatusCode.OK, response);
        }
        catch (ApiException ex)
        {
            return GetResponseService<TaskListData>.GetResponse(ex.StatusCode);
        }
    }

    public async Task<ResponseService<List<TaskListData>>> GetAllAsync(CancellationToken ct)
    {
        try
        {
            var response = await _apiRequest.GetAllAsync(ct);
            return GetResponseService<List<TaskListData>>.GetResponse(System.Net.HttpStatusCode.OK, response.Value);
        }
        catch (ApiException ex)
        {
            return GetResponseService<List<TaskListData>>.GetResponse(ex.StatusCode);
        }
    }

    public async Task<ResponseService<HttpResponseMessage>> UpdateAsync(string todoTaskListId, TaskListRequestData todo, CancellationToken ct)
    {
        try
        {
            var response = await _apiRequest.UpdateAsync(todoTaskListId, todo, ct);
            return GetResponseService<HttpResponseMessage>.GetResponse(System.Net.HttpStatusCode.OK, response);
        }
        catch (ApiException ex)
        {
            return GetResponseService<HttpResponseMessage>.GetResponse(ex.StatusCode);
        }
    }

    public async Task<ResponseService<List<TaskData>?>> GetTasksAsync(string todoTaskListId, CancellationToken ct)
    {
        try
        {
            var response = await _apiRequest.GetTasksAsync(todoTaskListId, ct);
            return GetResponseService<List<TaskData>?>.GetResponse(System.Net.HttpStatusCode.OK, response.Value);
        }
        catch (ApiException ex)
        {
            return GetResponseService<List<TaskData>?>.GetResponse(ex.StatusCode);
        }
    }
}
