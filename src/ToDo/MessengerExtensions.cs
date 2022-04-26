using System;
using System.Linq;
using System.Runtime.CompilerServices;
using ToDo.Presentation;
using Uno.Extensions.Reactive.Utils;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace ToDo;

public static class MessengerExtensions
{
	public static IDisposable Observe<TEntity, TKey>(this IState<TEntity> state, IMessenger messenger, Func<TEntity, TKey> keySelector)
		=> AttachedProperty.GetOrCreate(state, keySelector, messenger, (s, ks, msg) => new EntityRecipient<TEntity, TKey>(s, msg, ks));

	public static IDisposable Observe<TEntity, TKey>(this IListState<TEntity> state, IMessenger messenger, Func<TEntity, TKey> keySelector)
		=> AttachedProperty.GetOrCreate(state, keySelector, messenger, (s, ks, msg) => new ListRecipient<TEntity, TKey>(s, msg, ks));

	private abstract class RecipientBase<TState, TEntity, TKey> : IRecipient<EntityMessage<TEntity>>, IDisposable
		where TState : class
	{
		private readonly CancellationTokenSource _ct = new();
		private readonly IMessenger _messenger;
		private readonly Func<TEntity, TKey> _keySelector;

		public RecipientBase(TState state, IMessenger messenger, Func<TEntity, TKey> keySelector)
		{
			State = state;
			_messenger = messenger;
			_keySelector = keySelector;
			messenger.Register<EntityMessage<TEntity>>(this);

			ConditionalDisposable.Link(state, this);
		}

		protected TState State { get; }

		protected TKey GetKey(TEntity entity)
			=> _keySelector(entity);
		protected bool AreKeyEquals(TEntity left, TEntity right)
			=> AreKeyEquals(GetKey(left), GetKey(right));
		protected bool AreKeyEquals(TEntity left, TKey right)
			=> AreKeyEquals(GetKey(left), right);
		protected bool AreKeyEquals(TKey left, TEntity right)
			=> AreKeyEquals(left, GetKey(right));
		protected bool AreKeyEquals(TKey left, TKey right)
			=> left?.Equals(right) ?? right is null;

		/// <inheritdoc />
		public async void Receive(EntityMessage<TEntity> msg)
		{
			try
			{
				await Receive(msg, _ct.Token);
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

		protected abstract ValueTask Receive(EntityMessage<TEntity> msg, CancellationToken ct);

		/// <inheritdoc />
		public void Dispose()
		{
			_messenger.Unregister<EntityMessage<TEntity>>(this);
			_ct.Cancel(throwOnFirstException: false);
			_ct.Dispose();
		}
	}

	private class EntityRecipient<TEntity, TKey> : RecipientBase<IState<TEntity>, TEntity, TKey>
	{
		public EntityRecipient(IState<TEntity> state, IMessenger messenger, Func<TEntity, TKey> keySelector)
			: base(state, messenger, keySelector)
		{
		}

		protected override async ValueTask Receive(EntityMessage<TEntity> msg, CancellationToken ct)
		{
			switch (msg.Change)
			{
				case EntityChange.Update:
					var updatedEntityKey = GetKey(msg.Value);
					await State.UpdateValue(current => current.IsSome(out var entity) && AreKeyEquals(updatedEntityKey, entity)? msg.Value : current, ct);
					break;
			}
		}
	}

	private class ListRecipient<TEntity, TKey> : RecipientBase<IListState<TEntity>, TEntity, TKey>
	{
		public ListRecipient(IListState<TEntity> state, IMessenger messenger, Func<TEntity, TKey> keySelector)
			: base(state, messenger, keySelector)
		{
		}

		/// <inheritdoc />
		protected override async ValueTask Receive(EntityMessage<TEntity> msg, CancellationToken ct)
		{
			switch (msg.Change)
			{
				case EntityChange.Create:
					await State.AddAsync(msg.Value, ct);
					break;

				case EntityChange.Delete:
					var removedItemKey = GetKey(msg.Value);
					await State.RemoveAllAsync(item => AreKeyEquals(removedItemKey, item), ct);
					break;

				case EntityChange.Update:
					var updatedItemKey = GetKey(msg.Value);
					await State.UpdateAsync(item => AreKeyEquals(updatedItemKey, item), _ => msg.Value, ct);
					break;
			}
		}
	}

	private sealed class ConditionalDisposable
	{
		private static readonly ConditionalWeakTable<object, Handle> _stores = new();

		public static void Link(object owner, IDisposable disposable)
			=> _stores.GetOrCreateValue(owner).Add(disposable);

		public static void Link(object owner, IAsyncDisposable disposable)
			=> _stores.GetOrCreateValue(owner).Add(disposable);

		private class Handle
		{
			private readonly List<object> _disposables = new();

			public void Add(IDisposable disposable)
				=> _disposables.Add(disposable);

			public void Add(IAsyncDisposable disposable)
				=> _disposables.Add(disposable);

			~Handle()
			{
				foreach (var disposable in _disposables)
				{
					try
					{
						switch (disposable)
						{
							case IDisposable syncDisposable:
								syncDisposable.Dispose();
								break;
							case IAsyncDisposable asyncDisposable:
								asyncDisposable.DisposeAsync();
								break;
						}
					}
					catch (Exception error)
					{
						if (this.Log().IsEnabled(LogLevel.Error))
						{
							this.Log().LogError(error, "Got an exception in dispose.");
						}
					}
				}
			}
		}
	}
}
