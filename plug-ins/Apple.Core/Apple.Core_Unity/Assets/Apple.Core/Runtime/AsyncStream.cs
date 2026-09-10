using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AOT;
using Apple.Core.Runtime;
using UnityEngine;

namespace Apple.Core
{
    public interface IAsyncSequence<T> : IAsyncEnumerable<T>
    {
        long Id { get; }
    }
    public static class AsyncStreamInterop
    {
        private static Dictionary<long, object> _streamMap = new Dictionary<long, object>();
        private static long _streamId = long.MinValue;
        private static object _lock = new object();

        public static IAsyncSequence<T> Create<T>(Func<IntPtr, T> create)
            where T: class
        {
            Debug.Log("AsyncStreamInterop.Create");
            lock(_lock)
            {
                if (_streamId == long.MaxValue)
                    _streamId = long.MinValue;

                var id = _streamId++;
                var stream = new PrivateStream<T>(id, create);
                _streamMap.Add(id, stream);
                return stream;
            }
        }

        [MonoPInvokeCallback(typeof(SuccessTaskCallback<IntPtr>))]
        public static void AddValue(long id, IntPtr value)
        {
            Debug.Log("AsyncStreamInterop.AddValue");
            IPrivateStream stream = null;

            lock (_lock)
            {
                if (!_streamMap.TryGetValue(id, out var obj))
                {
                    Debug.Log("Stream map did not have id: " + id);
                    return;
                }

                stream = (IPrivateStream)obj;
            }

            stream?.AddValue(value);
        }

        [MonoPInvokeCallback(typeof(SuccessTaskCallback))]
        public static void Completed(long id)
        {
            Debug.Log("AsyncStreamInterop.Completed");
            IPrivateStream stream = null;

            lock (_lock)
            {
                if (!_streamMap.TryGetValue(id, out var obj))
                {
                    return;
                }

                stream = (IPrivateStream)obj;
            }

            stream?.Completed();
        }

        private static void Remove(long id)
        {
            lock(_lock)
            {
                if (_streamMap.ContainsKey(id))
                {
                    _streamMap.Remove(id);
                }
                else
                {
                    Debug.Log("Stream map did not have id: " + id);
                }
            }
        }

        // Type erasure for generics
        private interface IPrivateStream
        {
            void AddValue(IntPtr value);
            void Completed();
        }

        private sealed class PrivateStream<T> : IAsyncSequence<T>, IPrivateStream
            where T: class
        {
            private readonly Func<IntPtr, T> _create;
            private List<T> _items = new();
            private volatile bool _completed = false;
            private ReaderWriterLockSlim _rwLock = new ReaderWriterLockSlim();

            public long Id { get; }

            public PrivateStream(long id, Func<IntPtr, T> create)
            {
                Id = id;
                _create = create;
            }

            public void AddValue(IntPtr ptr)
            {
                Debug.Log("PrivateStream.AddValue");
                var item = _create(ptr);

                _rwLock.EnterWriteLock();
                try 
                {
                    _items.Add(item);
                    Debug.Log("PrivateStream.AddValue: Added item, total count: " + _items.Count);
                }
                finally 
                {
                    _rwLock.ExitWriteLock();
                }
            }

            public void Completed()
            {
                Debug.Log("PrivateStream.Completed");
                _completed = true;

                // The static map no longer needs to hold onto this. At this point an enumerator will
                // keep this alive as long as there is a consumer for it.
                Debug.Log($"PrivateStream.Completed: Removing {Id} from map");
                AsyncStreamInterop.Remove(Id);
            }

            // Async method causes state machine building, so use synchronous first to avoid 
            // emitting a state machine when not needed
            public ValueTask<(bool, T)> TryGetValue(int index, CancellationToken cancellationToken)
            {
                Debug.Log("PrivateStream.TryGetValue: Attempting to get item at index " + index);
                // Synchronous attempt
                _rwLock.EnterReadLock();
                try 
                {
                    if (_items.Count > index)
                    {
                        Debug.Log("PrivateStream.TryGetValue: Found item at index " + index);
                        var result = (true, _items[index]);
                        return new ValueTask<(bool, T)>(result);
                    }
                }
                finally
                {
                    _rwLock.ExitReadLock();
                }

                return TryGetValueAsync(index, cancellationToken);
            }

            private async ValueTask<(bool, T)> TryGetValueAsync(int index, CancellationToken cancellationToken)
            {
                // Loop until copmletion with a delay
                Debug.Log("PrivateStream.TryGetValueAsync: Waiting for item at index " + index);
                while(!_completed)
                {
                    await Task.Delay(100 /*ms*/, cancellationToken);

                    _rwLock.EnterReadLock();
                    try 
                    {
                        if (_items.Count > index)
                        {
                            return (true, _items[index]);
                        }
                    }
                    finally
                    {
                        _rwLock.ExitReadLock();
                    }
                }

                // One last try after completed just to make sure it wasn't added after the lock
                _rwLock.EnterReadLock();
                try 
                {
                    if (_items.Count > index)
                    {
                        return (true, _items[index]);
                    }
                }
                finally
                {
                    _rwLock.ExitReadLock();
                }

                return (false, null);
            }

            public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
            {
                return new AsyncEnumerator<T>(this, cancellationToken);
            }
        }

        private sealed class AsyncEnumerator<T> : IAsyncEnumerator<T>
            where T: class
        {
            private readonly PrivateStream<T> _stream;
            private readonly CancellationTokenSource _cts;
            private int _index = -1; // Incremented on every MoveNextAsync

            public AsyncEnumerator(PrivateStream<T> stream, CancellationToken cancellationToken)
            {
                _stream = stream;
                _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            }

            public T Current { get; private set; }

            public ValueTask DisposeAsync()
            {
                Debug.Log("AsyncEnumerator.DisposeAsync");
                _cts.Cancel();
                _cts.Dispose();
                
                return default;
            }

            public async ValueTask<bool> MoveNextAsync()
            {
                Debug.Log("AsyncEnumerator.MoveNextAsync");
                if (_cts.IsCancellationRequested)
                {
                    Debug.Log("AsyncEnumerator.MoveNextAsync: Cancellation requested");
                    return false;
                }

                var index = Interlocked.Increment(ref _index);
                Debug.Log("AsyncEnumerator.MoveNextAsync: Moving to index " + index);
                var (success, value) = await _stream.TryGetValue(index, _cts.Token);
                Debug.Log("AsyncEnumerator.MoveNextAsync: TryGetValue returned success=" + success);
                Current = value;

                return success;
            }
        }
    }
}