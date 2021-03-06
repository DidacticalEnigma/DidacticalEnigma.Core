﻿using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using DidacticalEnigma.Core.Models.Formatting;
using Optional;

namespace DidacticalEnigma.Core.Models.DataSources
{
    // asynchronously initializing wrapper over a IDataSource
    // not implementing the IDataSource because it purposefully violates the contract
    public class AsyncDataSource
    {
        private readonly Task<IDataSource> dataSourceTask;

        private bool disposed;

        public enum InitializationState
        {
            InProgress,
            Success,
            Failure
        }

        public DataSourceDescriptor Descriptor { get; }

        public InitializationState State
        {
            get
            {
                if (dataSourceTask.IsFaulted)
                    return InitializationState.Failure;
                if (dataSourceTask.IsCanceled)
                    return InitializationState.Failure;
                if (dataSourceTask.IsCompleted)
                    return InitializationState.Success;
                return InitializationState.InProgress;
            }

        }

        public string Kind { get; }

        public async Task<Option<RichFormatting>> Answer(Request request, CancellationToken token)
        {
            var dataSource = await dataSourceTask.ConfigureAwait(false);
            var result = await dataSource.Answer(request, token);
            return result;
        }

        public async void Dispose()
        {
            if (disposed)
                return;
            disposed = true;
            if (State == InitializationState.Success)
            {
                dataSourceTask.Result.Dispose();
                return;
            }
            // if it's still initializing, then we wait for a while
            if (State == InitializationState.InProgress)
                await Task.Delay(5000);
            // then dispose it if it's initialized
            if (State == InitializationState.Success)
                dataSourceTask.Result.Dispose();
            // otherwise fuck it
        }

        public async Task<UpdateResult> UpdateLocalDataSource(CancellationToken cancellation = default)
        {
            var dataSource = await dataSourceTask.ConfigureAwait(false);
            return await dataSource.UpdateLocalDataSource(cancellation);
        }

        public AsyncDataSource(Task<IDataSource> dataSource, Type type, string kind)
        {
            if (type.GetProperty("Descriptor", BindingFlags.Static | BindingFlags.Public)?.GetValue(null) is DataSourceDescriptor descriptor)
            {
                Descriptor = descriptor;
                dataSourceTask = dataSource;
                Kind = kind ?? "";
            }
            else
            {
                dataSourceTask = Task.FromException<IDataSource>(new InvalidOperationException("the data source doesn't provide a descriptor"));
            }
        }
    }
}
