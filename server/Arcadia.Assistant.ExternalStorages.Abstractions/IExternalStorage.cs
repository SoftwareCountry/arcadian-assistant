namespace Arcadia.Assistant.ExternalStorages.Abstractions
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IExternalStorage : IDisposable
    {
        Task<IEnumerable<StorageItem>> GetItems(string list, IEnumerable<ICondition> conditions = null, CancellationToken cancellationToken = default(CancellationToken));

        Task<StorageItem> AddItem(string list, StorageItem item, CancellationToken cancellationToken = default(CancellationToken));

        Task UpdateItem(string list, StorageItem item, CancellationToken cancellationToken = default(CancellationToken));

        Task DeleteItem(string list, string itemId, CancellationToken cancellationToken = default(CancellationToken));
    }
}