namespace ToDo.Presentation.Dialogs;

public static class DialogResults
{
	public static object Affirmative = nameof(Affirmative);
	public static object Negative = nameof(Negative);
}

public static class Dialog
{
	public static string ConfirmDeleteList = nameof(ConfirmDeleteList);
	public static string ConfirmDeleteTask = nameof(ConfirmDeleteTask);
	public static string ConfirmSignOut = nameof(ConfirmSignOut);
}

public static class ResourceKeys
{
	public static string WideMinWindowWidth = nameof(WideMinWindowWidth);
	public static string DialogContent = "Content";
	public static string DialogTitle = "Title";
	public static string DeleteButton = "./Dialog_Common_Delete";
	public static string CancelButton = "./Dialog_Common_Cancel";
	public static string SignOutButton = "SignOut";
}
