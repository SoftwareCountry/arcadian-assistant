namespace Arcadia.Assistant.ExternalStorages.SharepointOnline
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;

    using Arcadia.Assistant.ExternalStorages.Abstractions;
    using Arcadia.Assistant.ExternalStorages.SharepointOnline.Contracts;

    using Microsoft.SharePoint.Client;

    public class SharepointStorage : IExternalStorage
    {
        private readonly string serverUrl;
        private readonly ISharepointClientFactory clientFactory;
        private readonly ISharepointFieldsMapper fieldsMapper;
        private readonly ISharepointCamlBuilder camlBuilder;

        public SharepointStorage(
            string sharepointServerUrl,
            ISharepointClientFactory sharepointClientFactory,
            ISharepointFieldsMapper sharepointFieldsMapper,
            ISharepointCamlBuilder sharepointCamlBuilder)
        {
            this.serverUrl = sharepointServerUrl;
            this.clientFactory = sharepointClientFactory;
            this.fieldsMapper = sharepointFieldsMapper;
            this.camlBuilder = sharepointCamlBuilder;
        }

        public static void SetClientAuth(string clientId, string clientSecret)
        {
            TokenHelper.ClientId = clientId;
            TokenHelper.ClientSecret = clientSecret;
        }

        public async Task<IEnumerable<StorageItem>> GetItems(string list, IEnumerable<ICondition> conditions, CancellationToken cancellationToken)
        {
            using (var client = this.clientFactory.GetClient(this.serverUrl))
            {
                var fieldNames = this.GetFieldNames();
                var listItems = await this.GetListItems(client, list, conditions, fieldNames);

                return listItems
                    .Select(item => this.ListItemToStorageItem(item, fieldNames))
                    .ToList();
            }
        }

        public async Task<StorageItem> AddItem(string list, StorageItem item, CancellationToken cancellationToken)
        {
            using (var client = this.clientFactory.GetClient(this.serverUrl))
            {
                var calendarList = await this.GetCalendarList(client, list);
                var fieldNames = this.GetFieldNames();

                var newListItem = calendarList.AddItem(new ListItemCreationInformation());
                this.FillListItemFromStorageItem(newListItem, item, fieldNames);
                newListItem.Update();

                await client.ExecuteQueryAsync();

                // Crutch is needed because Sharepoint updates item fields in a strange manner
                newListItem[fieldNames.Id] = newListItem.Id;

                return this.ListItemToStorageItem(newListItem, fieldNames);
            }
        }

        public async Task<StorageItem> UpdateItem(string list, StorageItem item, IEnumerable<ICondition> conditions, CancellationToken cancellationToken)
        {
            using (var client = this.clientFactory.GetClient(this.serverUrl))
            {
                var fieldNames = this.GetFieldNames();

                var listItems = (await this.GetListItems(client, list, conditions, fieldNames)).ToArray();

                if (listItems.Length == 0)
                {
                    throw new ArgumentException("No items were found by specified conditions", nameof(conditions));
                }

                if (listItems.Length > 1)
                {
                    throw new ArgumentException("More than one item was found by specified conditions", nameof(conditions));
                }

                var existingListItem = listItems.First();
                this.FillListItemFromStorageItem(existingListItem, item, fieldNames);
                existingListItem.Update();

                await client.ExecuteQueryAsync();

                return this.ListItemToStorageItem(existingListItem, fieldNames);
            }
        }

        public async Task DeleteItem(string list, IEnumerable<ICondition> conditions = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var client = this.clientFactory.GetClient(this.serverUrl))
            {
                var fieldNames = this.GetFieldNames();

                var listItems = (await this.GetListItems(client, list, conditions, fieldNames)).ToArray();

                if (listItems.Length == 0)
                {
                    throw new ArgumentException("No items were found by specified conditions", nameof(conditions));
                }

                if (listItems.Length > 1)
                {
                    throw new ArgumentException("More than one item was found by specified conditions", nameof(conditions));
                }

                var existingListItem = listItems.First();
                existingListItem.DeleteObject();

                await client.ExecuteQueryAsync();
            }
        }

        private async Task<List> GetCalendarList(ClientContext client, string list)
        {
            var calendarList = client.Web.Lists.GetByTitle(list);
            await client.ExecuteQueryAsync();
            return calendarList;
        }

        private async Task<IEnumerable<ListItem>> GetListItems(
            ClientContext client,
            string list,
            IEnumerable<ICondition> conditions,
            (string Id, string Title, string Description, string StartDate, string EndDate, string Category, string CalendarEventId) fieldNames)
        {
            var calendarList = await this.GetCalendarList(client, list);

            var fieldsToRetrieve = new Expression<Func<ListItem, object>>[]
            {
                li => li[fieldNames.Id],
                li => li[fieldNames.Title],
                li => li[fieldNames.Description],
                li => li[fieldNames.StartDate],
                li => li[fieldNames.EndDate],
                li => li[fieldNames.Category],
                li => li[fieldNames.CalendarEventId]
            };

            var clientQuery = calendarList
                .GetItems(this.camlBuilder.GetCamlQuery(conditions))
                .Include(fieldsToRetrieve);

            var items = client.LoadQuery(
                clientQuery);
            await client.ExecuteQueryAsync();
            return items;
        }

        private StorageItem ListItemToStorageItem(
            ListItem item,
            (string Id, string Title, string Description, string StartDate, string EndDate, string Category, string CalendarEventId) fieldNames)
        {
            return new StorageItem
            {
                Id = item[fieldNames.Id]?.ToString(),
                Title = item[fieldNames.Title]?.ToString(),
                Description = item[fieldNames.Description]?.ToString(),
                StartDate = (DateTime?)item[fieldNames.StartDate],
                EndDate = (DateTime?)item[fieldNames.EndDate],
                Category = item[fieldNames.Category]?.ToString(),
                CalendarEventId = item[fieldNames.CalendarEventId]?.ToString()
            };
        }

        private void FillListItemFromStorageItem(
            ListItem newListItem,
            StorageItem item,
            (string Id, string Title, string Description, string StartDate, string EndDate, string Category, string CalendarEventId) fieldNames)
        {
            newListItem[fieldNames.Title] = item.Title;
            newListItem[fieldNames.Description] = item.Description;
            newListItem[fieldNames.StartDate] = item.StartDate;
            newListItem[fieldNames.EndDate] = item.EndDate;
            newListItem[fieldNames.Category] = item.Category;
            newListItem[fieldNames.CalendarEventId] = item.CalendarEventId;
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
            var idField = this.fieldsMapper.GetSharepointField(si => si.Id).Name;
            var titleField = this.fieldsMapper.GetSharepointField(si => si.Title).Name;
            var descriptionField = this.fieldsMapper.GetSharepointField(si => si.Description).Name;
            var startDateField = this.fieldsMapper.GetSharepointField(si => si.StartDate).Name;
            var endDateField = this.fieldsMapper.GetSharepointField(si => si.EndDate).Name;
            var categoryField = this.fieldsMapper.GetSharepointField(si => si.Category).Name;
            var calendarEventIdField = this.fieldsMapper.GetSharepointField(si => si.CalendarEventId).Name;
            return (idField, titleField, descriptionField, startDateField, endDateField, categoryField, calendarEventIdField);
        }
    }
}