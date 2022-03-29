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

    public async Task<bool> CreateTodoListAsync(CreateUpdateTodoListObj todoList, CancellationToken ct)
    {
        var apiResponse = RestService.For<ITodoListServiceApi>(_microsoftGraphSettings?.Endpoint ?? "", _refitSettings);
        var response = await apiResponse.CreateTodoListAsync(todoList, ct);
        return response.StatusCode == System.Net.HttpStatusCode.Created;
    }

    public async Task<bool> DeleteTodoListAsync(string todoTaskListId, CancellationToken ct)
    {
        var apiResponse = RestService.For<ITodoListServiceApi>(_microsoftGraphSettings?.Endpoint ?? "", _refitSettings);
        var response = await apiResponse.DeleteTodoListAsync(todoTaskListId, ct);
        return response.StatusCode == System.Net.HttpStatusCode.NoContent;

    }

    public async Task<TodoList> GetTodoListAsync(string todoTaskListId, CancellationToken ct)
    {
        var apiResponse = RestService.For<ITodoListServiceApi>(_microsoftGraphSettings?.Endpoint ?? "", _refitSettings);
        var response = await apiResponse.GetTodoListAsync(todoTaskListId, ct);
        return response;
    }

    public async Task<List<TodoList>> GetTodoListsAsync(CancellationToken ct)
    {
        var apiResponse = RestService.For<ITodoListServiceApi>(_microsoftGraphSettings?.Endpoint ?? "", _refitSettings);
        var response = await apiResponse.GetTodoListsAsync(ct);
        return response?.Value ?? new List<TodoList>();
    }

    public async Task<bool> UpdateTodoListAsync(string todoTaskListId, CreateUpdateTodoListObj todo, CancellationToken ct)
    {
        var apiResponse = RestService.For<ITodoListServiceApi>(_microsoftGraphSettings?.Endpoint ?? "", _refitSettings);
        var response = await apiResponse.UpdateTodoListAsync(todoTaskListId, todo, ct);
        return response.StatusCode == System.Net.HttpStatusCode.OK;
    }
}
