using Microsoft.Extensions.Options;
using Refit;
using ToDo.Models;
using ToDo.Services.Interface;

namespace ToDo.Services.Implementation;

public class TodoListService : ITodoListService
{
    private readonly MicrosoftGraphSettings? _microsoftGraphSettings;
    private readonly RefitSettings _refitSettings;
    private ITodoListServiceApi apiRequest;

    public TodoListService(IOptions<MicrosoftGraphSettings> microsoftGraphSettings)
    {
        _microsoftGraphSettings = microsoftGraphSettings?.Value;
        _refitSettings = new RefitSettings();
        _refitSettings.AuthorizationHeaderValueGetter = GetAccessToken;
        apiRequest = RestService.For<ITodoListServiceApi>(_microsoftGraphSettings?.Endpoint ?? "", _refitSettings);
    }

    public Task<string> GetAccessToken()
    {
        return Task.FromResult(_microsoftGraphSettings?.AccessToken ?? "");
    }

    public async Task<HttpResponseMessage> CreateTodoListAsync(CreateUpdateTodoListObj todoList, CancellationToken ct)
    {
        var response = await apiRequest.CreateTodoListAsync(todoList, ct);
        return response;
    }

    public async Task<HttpResponseMessage> DeleteTodoListAsync(string todoTaskListId, CancellationToken ct)
    {
          var response = await apiRequest.DeleteTodoListAsync(todoTaskListId, ct);
        return response;

    }

    public async Task<ResponseService<TodoList>> GetTodoListAsync(string todoTaskListId, CancellationToken ct)
    {
        try
        {
            var response = await apiRequest.GetTodoListAsync(todoTaskListId, ct);
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
            var response = await apiRequest.GetTodoListsAsync(ct);
            return GetResponseService<List<TodoList>>.GetResponse(System.Net.HttpStatusCode.OK, response.Value);
        }
        catch (ApiException ex)
        {
            return GetResponseService<List<TodoList>>.GetResponse(ex.StatusCode);
        }
    }

    public async Task<HttpResponseMessage> UpdateTodoListAsync(string todoTaskListId, CreateUpdateTodoListObj todo, CancellationToken ct)
    {
        var response = await apiRequest.UpdateTodoListAsync(todoTaskListId, todo, ct);
        return response;
    }
}
