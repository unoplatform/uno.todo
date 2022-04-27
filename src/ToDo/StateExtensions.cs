using System;
using System.Linq;
using Uno.Extensions.Reactive.Utils;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace ToDo;

public static class StateExtensions
{
	public static IDisposable Observe<TEntity, TKey>(this IState<TEntity> state, IMessenger messenger, Func<TEntity, TKey> keySelector)
		=> AttachedProperty.GetOrCreate(state, keySelector, messenger, (s, ks, msg) => new Recipient<IState<TEntity>, TEntity, TKey>(s, msg, ks, UpdateAsync));

	public static IDisposable Observe<TEntity, TKey>(this IListState<TEntity> state, IMessenger messenger, Func<TEntity, TKey> keySelector)
		=> AttachedProperty.GetOrCreate(state, keySelector, messenger, (s, ks, msg) => new Recipient<IListState<TEntity>, TEntity, TKey>(s, msg, ks, UpdateAsync));

	public static async ValueTask UpdateAsync<TEntity, TKey>(this IState<TEntity> state, EntityMessage<TEntity> message, Func<TEntity, TKey> keySelector, CancellationToken ct)
	{
		switch (message.Change)
		{
			case EntityChange.Update:
				var updatedEntityKey = keySelector(message.Value);
				await state.UpdateValue(current => current.IsSome(out var entity) && AreKeyEquals(updatedEntityKey, keySelector(entity)) ? message.Value : current, ct);
				break;
		}

		static bool AreKeyEquals(TKey left, TKey right)
			=> left?.Equals(right) ?? right is null;
	}

	public static async ValueTask UpdateAsync<TEntity, TKey>(this IListState<TEntity> state, EntityMessage<TEntity> message, Func<TEntity, TKey> keySelector, CancellationToken ct)
	{
		switch (message.Change)
		{
			case EntityChange.Create:
				await state.AddAsync(message.Value, ct);
				break;

			case EntityChange.Delete:
				var removedItemKey = keySelector(message.Value);
				await state.RemoveAllAsync(item => AreKeyEquals(removedItemKey, keySelector(item)), ct);
				break;

			case EntityChange.Update:
				var updatedItemKey = keySelector(message.Value);
				await state.UpdateAsync(item => AreKeyEquals(updatedItemKey, keySelector(item)), _ => message.Value, ct);
				break;
		}

		static bool AreKeyEquals(TKey left, TKey right)
			=> left?.Equals(right) ?? right is null;
	}

	private sealed class Recipient<TState, TEntity, TKey> : IRecipient<EntityMessage<TEntity>>, IDisposable
		where TState : class
	{
		private readonly CancellationTokenSource _ct = new();
		private readonly TState _state;
		private readonly IMessenger _messenger;
		private readonly Func<TEntity, TKey> _keySelector;
		private readonly AsyncAction<TState, EntityMessage<TEntity>, Func<TEntity, TKey>> _update;

		public Recipient(
			TState state,
			IMessenger messenger,
			Func<TEntity, TKey> keySelector,
			AsyncAction<TState, EntityMessage<TEntity>, Func<TEntity, TKey>> update)
		{
			_state = state;
			_messenger = messenger;
			_keySelector = keySelector;
			_update = update;

			messenger.Register(this);
		}

		/// <inheritdoc />
		public async void Receive(EntityMessage<TEntity> msg)
		{
			try
			{
				await _update(_state, msg, _keySelector, _ct.Token);
			}
			catch (OperationCanceledException) when (_ct.IsCancellationRequested)
			{
			}
			catch (Exception e)
			{
				if (this.Log().IsEnabled(LogLevel.Error))
				{
					this.Log().LogError(e, "Failed to apply update message.");
				}
			}
		}

		/// <inheritdoc />
		public void Dispose()
		{
			_messenger.Unregister<EntityMessage<TEntity>>(this);
			_ct.Cancel(throwOnFirstException: false);
			_ct.Dispose();
		}
	}
}
