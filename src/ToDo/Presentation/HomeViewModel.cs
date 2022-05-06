namespace ToDo.Presentation;

[ReactiveBindable]
public partial class HomeViewModel
{
	private readonly INavigator _navigator;
	private readonly IAuthenticationService _authSvc;
	private readonly IStringLocalizer _localizer;
	private readonly ITaskListService _listSvc;
	private readonly IUserProfilePictureService _userSvc;

	private HomeViewModel(
		INavigator navigator,
		IStringLocalizer localizer,
		IAuthenticationService authSvc,
		ITaskListService listSvc,
		IUserProfilePictureService userSvc,
		IMessenger messenger)
	{
		_navigator = navigator;
		_localizer = localizer;
		_navigator = navigator;
		_authSvc = authSvc;
		_listSvc = listSvc;
		_userSvc = userSvc;

		Lists.Observe(messenger, list => list.Id);

		WellKnownLists = new TaskList[]
		{
			new(TaskList.WellknownListNames.Important, _localizer["HomePage_ImportantTaskListLabel"]),
			new(TaskList.WellknownListNames.Tasks, _localizer["HomePage_CommonTaskListLabel"]),
		};
	}

	public IFeed<UserContext?> CurrentUser => Feed<UserContext?>.Async(async ct =>
	{
		var user = await _authSvc.GetCurrentUserAsync();
		if ( user != default && user.ProfilePicture is null )
		{
			var profilePictureContent = await _userSvc.GetAsync(ct);
			_authSvc.SetProfilePicture(profilePictureContent);
			return await _authSvc.GetCurrentUserAsync();
		}
		return user;
	});

	private IListState<TaskList> Lists => ListState<TaskList>.Async(this, _listSvc.GetAllAsync);

	public TaskList[] WellKnownLists { get; }

	public IListFeed<TaskList> CustomLists => Lists.Where(list => list is { WellknownListName: null or "none" });

	public ICommand CreateTaskList => Command.Async(DoCreateTaskList);
	private async ValueTask DoCreateTaskList(CancellationToken ct)
	{
		var listName = await _navigator.GetDataAsync<AddListViewModel, string>(this, qualifier: Qualifiers.Dialog, cancellation: ct);

		if (listName is not null)
		{
			await _listSvc.CreateAsync(listName, ct);
		}
	}
}
