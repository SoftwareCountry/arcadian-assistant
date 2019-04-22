namespace Arcadia.Assistant.ExternalStorages.SharepointOnline
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    using Arcadia.Assistant.ExternalStorages.Abstractions;
    using Arcadia.Assistant.ExternalStorages.SharepointOnline.Contracts;
    using Arcadia.Assistant.ExternalStorages.SharepointOnline.SharepointApiModels;

    using Newtonsoft.Json;

    public class SharepointStorage : IExternalStorage
    {
        private readonly ISharepointOnlineConfiguration configuration;
        private readonly ISharepointAuthTokenService authTokenService;
        private readonly ISharepointFieldsMapper fieldsMapper;
        private readonly ISharepointConditionsCompiler conditionsCompiler;

        private readonly HttpClient httpClient;
        private string accessToken;

        public SharepointStorage(
            ISharepointOnlineConfiguration configuration,
            ISharepointAuthTokenService sharepointAuthTokenService,
            ISharepointFieldsMapper sharepointFieldsMapper,
            ISharepointConditionsCompiler sharepointConditionsCompiler,
            IHttpClientFactory httpClientFactory)
        {
            this.configuration = configuration;
            this.authTokenService = sharepointAuthTokenService;
            this.fieldsMapper = sharepointFieldsMapper;
            this.conditionsCompiler = sharepointConditionsCompiler;

            this.httpClient = httpClientFactory.CreateClient();
        }

        public async Task<IEnumerable<StorageItem>> GetItems(string list, IEnumerable<ICondition> conditions, CancellationToken cancellationToken)
        {
            var listItems = await this.GetListItems(list, conditions, cancellationToken);

            return listItems
                .Select(this.ListItemToStorageItem)
                .ToArray();
        }

        public async Task<StorageItem> AddItem(string list, StorageItem item, CancellationToken cancellationToken)
        {
            var listItemType = await this.GetListItemType(list, cancellationToken);
            var requestData = this.StorageItemToListItemRequest(item, listItemType);

            var request = SharepointRequest
                .Create(HttpMethod.Post, this.GetListItemsUrl(list))
                .WithContent(requestData);

            var addedItem = await this.ExecuteSharepointRequest<SharepointListItem>(request, cancellationToken);

            return this.ListItemToStorageItem(addedItem);
        }

        public async Task UpdateItem(string list, string itemId, StorageItem item, CancellationToken cancellationToken)
        {
            var idConditions = new[] { new EqualCondition(x => x.Id, itemId) };
            var listItems = (await this.GetListItems(list, idConditions, cancellationToken)).ToArray();

            this.EnsureSingleItemReturned(listItems);

            var existingListItem = listItems.First();

            var updateItemUrl = this.GetListItemsUrl(list, false);
            updateItemUrl += $"({existingListItem.Id})";

            var listItemType = await this.GetListItemType(list, cancellationToken);

            var requestData = this.StorageItemToListItemRequest(item, listItemType);

            var request = SharepointRequest
                .Create(HttpMethod.Post, updateItemUrl)
                .WithContent(requestData)
                .WithIfMatchHeader()
                .WithXHttpMethodHeader("MERGE");

            await this.ExecuteSharepointRequest<SharepointListItem>(request, cancellationToken);
        }

        public async Task DeleteItem(string list, string itemId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var idConditions = new[] { new EqualCondition(x => x.Id, itemId) };
            var listItems = (await this.GetListItems(list, idConditions, cancellationToken)).ToArray();

            this.EnsureSingleItemReturned(listItems);

            var existingListItem = listItems.First();

            var deleteItemUrl = this.GetListItemsUrl(list, false);
            deleteItemUrl += $"({existingListItem.Id})";

            var request = SharepointRequest
                .Create(HttpMethod.Post, deleteItemUrl)
                .WithIfMatchHeader()
                .WithXHttpMethodHeader("DELETE");

            await this.ExecuteSharepointRequest(request, cancellationToken);
        }

        public void Dispose()
        {
            this.httpClient.Dispose();
        }

        private async Task<IEnumerable<SharepointListItem>> GetListItems(
            string list,
            IEnumerable<ICondition> conditions,
            CancellationToken cancellationToken)
        {
            var itemsUrl = this.GetListItemsUrl(list);

            var conditionsUrlPart = this.conditionsCompiler.CompileConditions(conditions);

            if (conditionsUrlPart != null)
            {
                itemsUrl += $"&{conditionsUrlPart}";
            }

            var request = SharepointRequest.Create(HttpMethod.Get, itemsUrl);
            var response = await this.ExecuteSharepointRequest<SharepointListItemsResponse>(request, cancellationToken);

            return response.Value;
        }

        private async Task<string> GetListItemType(string list, CancellationToken cancellationToken)
        {
            var listUrl = $"{this.GetListUrl(list)}?$select=ListItemEntityTypeFullName";
            var request = SharepointRequest.Create(HttpMethod.Get, listUrl);
            var response = await this.ExecuteSharepointRequest<SharepointListResponse>(request, cancellationToken);
            return response.ListItemEntityTypeFullName;
        }

        private string GetListUrl(string list)
        {
            return $"{this.configuration.ServerUrl}/_api/lists/GetByTitle('{list}')";
        }

        private string GetListItemsUrl(string list, bool includeSelectPart = true)
        {
            var baseUrl = $"{this.GetListUrl(list)}/items";

            var fieldNames = this.GetFieldNames();

            var selectUrlPart =
                $"$select={fieldNames.Id},{fieldNames.Title},{fieldNames.Description},{fieldNames.StartDate}," +
                $"{fieldNames.EndDate},{fieldNames.Category},{fieldNames.CalendarEventId}";

            return !includeSelectPart
                ? baseUrl
                : $"{baseUrl}?{selectUrlPart}";
        }

        private async Task<T> ExecuteSharepointRequest<T>(SharepointRequest request, CancellationToken cancellationToken)
        {
            var response = await this.ExecuteSharepointRequest(request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Request failed with {response.StatusCode} status code");
            }

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(content);
        }

        private async Task<HttpResponseMessage> ExecuteSharepointRequest(SharepointRequest request, CancellationToken cancellationToken)
        {
            // To cache access token for several Sharepoint requests in bounds of one request to storage
            if (this.accessToken == null)
            {
                this.accessToken = await this.authTokenService.GetAccessToken(this.configuration.ServerUrl, cancellationToken);
            }

            request = request
                .WithAcceptHeader("application/json;odata=nometadata")
                .WithBearerAuthorizationHeader(this.accessToken);

            return await this.httpClient.SendAsync(request.GetHttpRequest(), cancellationToken);
        }

        private StorageItem ListItemToStorageItem(SharepointListItem item)
        {
            return new StorageItem
            {
                Id = item.Id.ToString(),
                Title = item.Title,
                Description = item.Description,
                StartDate = item.EventDate,
                EndDate = item.EndDate,
                Category = item.Category,
                CalendarEventId = item.CalendarEventId
            };
        }

        private SharepointListItemRequest StorageItemToListItemRequest(StorageItem item, string listItemType)
        {
            return new SharepointListItemRequest
            {
                Metadata = new SharepointListItemRequest.MetadataRequest
                {
                    Type = listItemType
                },
                Title = item.Title,
                Description = item.Description,
                EventDate = item.StartDate,
                EndDate = item.EndDate,
                Category = item.Category,
                CalendarEventId = item.CalendarEventId
            };
        }

        private void EnsureSingleItemReturned(SharepointListItem[] listItems)
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

        private (
            string Id,
            string Title,
            string Description,
            string StartDate,
            string EndDate,
            string Category,
            string CalendarEventId
            ) GetFieldNames()
        {
            var idField = this.fieldsMapper.GetSharepointField(si => si.Id);
            var titleField = this.fieldsMapper.GetSharepointField(si => si.Title);
            var descriptionField = this.fieldsMapper.GetSharepointField(si => si.Description);
            var startDateField = this.fieldsMapper.GetSharepointField(si => si.StartDate);
            var endDateField = this.fieldsMapper.GetSharepointField(si => si.EndDate);
            var categoryField = this.fieldsMapper.GetSharepointField(si => si.Category);
            var calendarEventIdField = this.fieldsMapper.GetSharepointField(si => si.CalendarEventId);
            return (idField, titleField, descriptionField, startDateField, endDateField, categoryField, calendarEventIdField);
        }
    }
}