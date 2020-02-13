namespace Arcadia.Assistant.ExternalStorages.SharepointOnline
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    using Abstractions;

    using Contracts;

    using SharepointApiModels;

    public class SharepointStorage : IExternalStorage
    {
        private readonly ISharepointConditionsCompiler conditionsCompiler;
        private readonly ISharepointOnlineConfiguration configuration;
        private readonly ISharepointRequestExecutor requestExecutor;

        public SharepointStorage(
            ISharepointOnlineConfiguration configuration,
            ISharepointRequestExecutor requestExecutor,
            ISharepointFieldsMapper sharePointFieldsMapper,
            ISharepointConditionsCompiler sharePointConditionsCompiler)
        {
            this.configuration = configuration;
            this.requestExecutor = requestExecutor;
            this.FieldsMapper = sharePointFieldsMapper;
            this.conditionsCompiler = sharePointConditionsCompiler;
            this.StorageItemJsonConvertor = new StorageItemJsonConvertor(this.FieldsMapper);
        }

        private ISharepointFieldsMapper FieldsMapper { get; }

        private StorageItemJsonConvertor StorageItemJsonConvertor { get; }

        public async Task<IEnumerable<StorageItem>> GetItems(string list, IEnumerable<ICondition>? conditions, CancellationToken cancellationToken)
        {
            var listItems = await this.GetListItems(list, conditions, cancellationToken);

            return listItems?.ToArray() ?? new StorageItem[0];
        }

        public async Task<StorageItem> AddItem(string list, StorageItem item, CancellationToken cancellationToken)
        {
            var listItemType = await this.GetListItemType(list, cancellationToken);
            var requestData = this.StorageItemJsonConvertor.StorageItemToRequestJson(item, listItemType);

            var request = SharepointRequest
                .Create(HttpMethod.Post, this.GetListItemsUrl(list, false))
                .WithAcceptHeader("application/json;odata=verbose")
                .WithContentString(requestData);

            return await this.requestExecutor.ExecuteSharepointRequest<StorageItem>(request, cancellationToken);
        }

        public async Task UpdateItem(string list, StorageItem item, CancellationToken cancellationToken)
        {
            var idConditions = new[] { new EqualCondition(x => x.Id, item.Id) };
            var listItems = (await this.GetListItems(list, idConditions, cancellationToken)).ToArray();

            this.EnsureSingleItemReturned(listItems);

            var existingListItem = listItems.First();

            var updateItemUrl = $"{this.GetListItemsUrl(list, false)}({existingListItem.Id})";

            var listItemType = await this.GetListItemType(list, cancellationToken);

            var requestData = this.StorageItemJsonConvertor.StorageItemToRequestJson(item, listItemType);

            var request = SharepointRequest
                .Create(HttpMethod.Post, updateItemUrl)
                .WithAcceptHeader("application/json;odata=verbose")
                .WithContentString(requestData)
                .WithIfMatchHeader()
                .WithXHttpMethodHeader("MERGE");

            await this.requestExecutor.ExecuteSharepointRequest<StorageItem>(request, cancellationToken);
        }

        public async Task DeleteItem(string list, string itemId, CancellationToken cancellationToken)
        {
            var idConditions = new[] { new EqualCondition(x => x.Id, itemId) };
            var listItems = (await this.GetListItems(list, idConditions, cancellationToken)).ToArray();

            this.EnsureSingleItemReturned(listItems);

            var existingListItem = listItems.First();

            var deleteItemUrl = this.GetListItemsUrl(list, false);
            deleteItemUrl += $"({existingListItem.Id})";

            var request = SharepointRequest
                .Create(HttpMethod.Post, deleteItemUrl)
                .WithAcceptHeader("application/json;odata=verbose")
                .WithIfMatchHeader()
                .WithXHttpMethodHeader("DELETE");

            await this.requestExecutor.ExecuteSharepointRequest(request, cancellationToken);
        }

        public void Dispose()
        {
        }

        private async Task<IEnumerable<StorageItem>?> GetListItems(
            string list,
            IEnumerable<ICondition>? conditions,
            CancellationToken cancellationToken)
        {
            var itemsUrl = this.GetListItemsUrl(list);

            var conditionsUrlPart = this.conditionsCompiler.CompileConditions(conditions);

            if (conditionsUrlPart != null)
            {
                itemsUrl += $"&{conditionsUrlPart}";
            }

            var request = SharepointRequest.Create(HttpMethod.Get, itemsUrl);
            var response = await this.requestExecutor.ExecuteSharepointRequest<SharepointListItemsResponse>(request, cancellationToken);
            return response.Value.Select(this.StorageItemJsonConvertor.JsonToStorageItem);
        }

        private async Task<string?> GetListItemType(string list, CancellationToken cancellationToken)
        {
            var listUrl = $"{this.GetListUrl(list)}?$select=ListItemEntityTypeFullName";
            var request = SharepointRequest.Create(HttpMethod.Get, listUrl);
            var response = await this.requestExecutor.ExecuteSharepointRequest<SharepointListResponse>(request, cancellationToken);
            return response.ListItemEntityTypeFullName;
        }

        private string GetListUrl(string list)
        {
            return $"{this.configuration.ServerUrl}/_api/web/lists/getByTitle('{list}')";
        }

        private string GetListItemsUrl(string list, bool includeSelectPart = true)
        {
            var baseUrl = $"{this.GetListUrl(list)}/items";

            var fieldNames = this.StorageItemJsonConvertor.GetFieldNames();

            var selectUrlPart =
                $"$select={fieldNames.Id},{fieldNames.Title},{fieldNames.Description},{fieldNames.StartDate}," +
                $"{fieldNames.EndDate},{fieldNames.Category},{fieldNames.AllDayEvent},{fieldNames.CalendarEventId}";

            return !includeSelectPart
                ? baseUrl
                : $"{baseUrl}?{selectUrlPart}";
        }

        private void EnsureSingleItemReturned(StorageItem[] listItems)
        {
            if (listItems.Length == 0)
            {
                throw new ArgumentException("No items were found by specified conditions");
            }

            if (listItems.Length > 1)
            {
                throw new ArgumentException("More than one item was found by specified conditions");
            }
        }
    }
}