﻿

using Windows.Media.Capture.Frames;
using Windows.Media.Core;

namespace ToDo;

public static class StateExtensions
{
	public static IDisposable Observe<TEntity, TKey>(this IState<TEntity> state, IMessenger messenger, Func<TEntity, TKey> keySelector)
		=> AttachedProperty.GetOrCreate(state, keySelector, messenger, (s, ks, msg) => new Recipient<IState<TEntity>, TEntity, TKey>(s, msg, ks, UpdateAsync));

	public static IDisposable Observe<TEntity, TKey>(this IListState<TEntity> state, IMessenger messenger, Func<TEntity, TKey> keySelector)
		=> AttachedProperty.GetOrCreate(state, keySelector, messenger, (s, ks, msg) => new Recipient<IListState<TEntity>, TEntity, TKey>(s, msg, ks, UpdateAsync));

	public static IDisposable Observe<TOther, TEntity, TKey>(
		this IListState<TEntity> state,
		IMessenger messenger,
		IState<TOther> other,
		Func<TOther, TEntity, bool> predicate,
		Func<TEntity, TKey> keySelector)
	{
		return AttachedProperty.GetOrCreate(
			state,
			keySelector,
			(other, predicate, messenger),
			CreateRecipient);

		static Recipient<IListState<TEntity>, TEntity, TKey> CreateRecipient(
			IListState<TEntity> state,
			Func<TEntity, TKey> keySelector,
			(IState<TOther> other, Func<TOther, TEntity, bool> predicate, IMessenger messenger) config)
			=> new(state, config.messenger, keySelector, If<IListState<TEntity>, TOther, TEntity, TKey>(config.other, config.predicate, UpdateAsync));
	}

	public static async ValueTask UpdateAsync<TEntity, TKey>(this IState<TEntity> state, EntityMessage<TEntity> message, Func<TEntity, TKey> keySelector, CancellationToken ct)
	{
		switch (message.Change)
		{
			case EntityChange.Updated:
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
			case EntityChange.Created:
				await state.AddAsync(message.Value, ct);
				break;

			case EntityChange.Deleted:
				var removedItemKey = keySelector(message.Value);
				await state.RemoveAllAsync(item => AreKeyEquals(removedItemKey, keySelector(item)), ct);
				break;

			case EntityChange.Updated:
				var updatedItemKey = keySelector(message.Value);
				await state.UpdateAsync(item => AreKeyEquals(updatedItemKey, keySelector(item)), _ => message.Value, ct);
				break;
		}

		static bool AreKeyEquals(TKey left, TKey right)
			=> left?.Equals(right) ?? right is null;
	}


	private static AsyncAction<TState, EntityMessage<TEntity>, Func<TEntity, TKey>> If<TState, TOther, TEntity, TKey>(
		IState<TOther> other,
		Func<TOther, TEntity, bool> predicate,
		AsyncAction<TState, EntityMessage<TEntity>, Func<TEntity, TKey>> update)
		=> async (state, message, keySelector, ct) =>
		{
			using var _ = SourceContext.GetOrCreate(other).AsCurrent();

			if ((await other).IsSome(out var master) && predicate(master, message.Value))
			{
				await update(state, message, keySelector, ct);
			}
		};

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

	//private sealed class Recipient<TState, TMasterEntity, TMasterKey, TDetailEntity, TDetailKey> : IRecipient<EntityMessage<TDetailEntity>>, IDisposable
	//	where TState : class
	//{
	//	private readonly CancellationTokenSource _ct = new();
	//	private readonly TState _state;
	//	private readonly IMessenger _messenger;
	//	private readonly Func<TEntity, TKey> _keySelector;
	//	private readonly AsyncAction<TState, EntityMessage<TDetailEntity>, Func<TEntity, TKey>> _update;

	//	public Recipient(
	//		TState state,
	//		IMessenger messenger,
	//		Func<TEntity, TKey> keySelector,
	//		AsyncAction<TState, EntityMessage<TEntity>, Func<TEntity, TKey>> update)
	//	{
	//		_state = state;
	//		_messenger = messenger;
	//		_keySelector = keySelector;
	//		_update = update;

	//		messenger.Register(this);
	//	}

	//	/// <inheritdoc />
	//	public async void Receive(EntityMessage<TDetailEntity> msg)
	//	{
	//		try
	//		{
	//			await _update(_state, msg, _keySelector, _ct.Token);
	//		}
	//		catch (OperationCanceledException) when (_ct.IsCancellationRequested)
	//		{
	//		}
	//		catch (Exception e)
	//		{
	//			if (this.Log().IsEnabled(LogLevel.Error))
	//			{
	//				this.Log().LogError(e, "Failed to apply update message.");
	//			}
	//		}
	//	}

	//	/// <inheritdoc />
	//	public void Dispose()
	//	{
	//		_messenger.Unregister<EntityMessage<TEntity>>(this);
	//		_ct.Cancel(throwOnFirstException: false);
	//		_ct.Dispose();
	//	}
	//}
}
