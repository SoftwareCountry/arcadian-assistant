﻿namespace Arcadia.Assistant.ExternalStorages.SharepointOnline
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    using Abstractions;

    using Contracts;

    public class SharepointFieldsMapper : ISharepointFieldsMapper
    {
        private static readonly SharepointFieldMapping CalendarEventIdMapping =
            CreateMapping(item => item.CalendarEventId, "CalendarEventId");

        private readonly Dictionary<string, string> fieldsByPropertyName;

        public SharepointFieldsMapper()
            : this(DefaultMappingWithCalendarEventId)
        {
        }

        public SharepointFieldsMapper(params SharepointFieldMapping[] fields)
        {
            this.fieldsByPropertyName = fields
                .ToDictionary(f => this.GetPropertyName(f.Property), f => f.Field);
        }

        public static IEnumerable<SharepointFieldMapping> DefaultMapping { get; } = new[]
        {
            CreateMapping(item => item.Id, "ID"),
            CreateMapping(item => item.Title, "Title"),
            CreateMapping(item => item.Description, "Description"),
            CreateMapping(item => item.StartDate, "EventDate"),
            CreateMapping(item => item.EndDate, "EndDate"),
            CreateMapping(item => item.Category, "Category"),
            CreateMapping(item => item.AllDayEvent, "fAllDayEvent")
        };

        private static SharepointFieldMapping[] DefaultMappingWithCalendarEventId { get; } = DefaultMapping.Union(new[] { CalendarEventIdMapping }).ToArray();

        public string GetSharepointField(Expression<Func<StorageItem, object>> property)
        {
            var propertyName = this.GetPropertyName(property);
            var fieldName = this.GetSharepointField(propertyName);

            if (fieldName == null)
            {
                throw new ArgumentException($"Mapping for property '{propertyName}' doesn't exist");
            }

            return fieldName;
        }

        private string? GetSharepointField(string propertyName)
        {
            return this.fieldsByPropertyName.TryGetValue(propertyName, out var field) ? field : null;
        }

        public static SharepointFieldMapping CreateMapping(Expression<Func<StorageItem, object>> property, string fieldName)
        {
            return new SharepointFieldMapping(property, fieldName);
        }

        private string GetPropertyName(Expression<Func<StorageItem, object>> property)
        {
            return new PropertyNameParser().GetName(property);
        }

        public struct SharepointFieldMapping
        {
            public SharepointFieldMapping(Expression<Func<StorageItem, object>> property, string field)
            {
                this.Property = property;
                this.Field = field;
            }

            public Expression<Func<StorageItem, object>> Property { get; }

            public string Field { get; }
        }
    }
}