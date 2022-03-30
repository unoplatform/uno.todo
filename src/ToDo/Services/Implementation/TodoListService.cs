using Microsoft.Extensions.Options;
using Refit;
using ToDo.Models;
using ToDo.Services.Interface;

namespace ToDo.Services.Implementation;

public class TodoListService : ITodoListService
{
    private readonly MicrosoftGraphSettings? _microsoftGraphSettings;
    private readonly RefitSettings _refitSettings;

    public TodoListService(IOptions<MicrosoftGraphSettings> microsoftGraphSettings)
    {
        _microsoftGraphSettings = microsoftGraphSettings?.Value;
        _refitSettings = new RefitSettings();
        _refitSettings.AuthorizationHeaderValueGetter = GetAccessToken;
    }

    public Task<string> GetAccessToken()
    {
        return Task.FromResult(_microsoftGraphSettings?.AccessToken ?? "");
    }

    public async Task<HttpResponseMessage> CreateTodoListAsync(CreateUpdateTodoListObj todoList, CancellationToken ct)
    {
        var apiResponse = RestService.For<ITodoListServiceApi>(_microsoftGraphSettings?.Endpoint ?? "", _refitSettings);
        var response = await apiResponse.CreateTodoListAsync(todoList, ct);

        return response;
    }

    public async Task<HttpResponseMessage> DeleteTodoListAsync(string todoTaskListId, CancellationToken ct)
    {
        var apiResponse = RestService.For<ITodoListServiceApi>(_microsoftGraphSettings?.Endpoint ?? "", _refitSettings);
        var response = await apiResponse.DeleteTodoListAsync(todoTaskListId, ct);
        return response;

    }

    public async Task<ResponseService<TodoList>> GetTodoListAsync(string todoTaskListId, CancellationToken ct)
    {
        try
        {
            var apiResponse = RestService.For<ITodoListServiceApi>(_microsoftGraphSettings?.Endpoint ?? "", _refitSettings);
            var response = await apiResponse.GetTodoListAsync(todoTaskListId, ct);
            return GetResponseService<TodoList>.GetResponse(System.Net.HttpStatusCode.OK, response);
        }
        catch (ApiException ex)
        {
            return GetResponseService<TodoList>.GetResponse(ex.StatusCode);
        }
    }

    public async Task<ResponseService<List<TodoList>>> GetTodoListsAsync(CancellationToken ct)
    {
        try
        {
            var apiResponse = RestService.For<ITodoListServiceApi>(_microsoftGraphSettings?.Endpoint ?? "", _refitSettings);
            var response = await apiResponse.GetTodoListsAsync(ct);
            return GetResponseService<List<TodoList>>.GetResponse(System.Net.HttpStatusCode.OK, response.Value);
        }
        catch (ApiException ex)
        {
            return GetResponseService<List<TodoList>>.GetResponse(ex.StatusCode);
        }
    }

    public async Task<HttpResponseMessage> UpdateTodoListAsync(string todoTaskListId, CreateUpdateTodoListObj todo, CancellationToken ct)
    {
        var apiResponse = RestService.For<ITodoListServiceApi>(_microsoftGraphSettings?.Endpoint ?? "", _refitSettings);
        var response = await apiResponse.UpdateTodoListAsync(todoTaskListId, todo, ct);
        return response;
    }
}
