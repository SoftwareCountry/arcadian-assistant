namespace Arcadia.Assistant.ExternalStorages.SharepointOnline.Contracts
{
    using System.Collections.Generic;

    using Arcadia.Assistant.ExternalStorages.Abstractions;

    using Microsoft.SharePoint.Client;

    public interface ISharepointCamlBuilder
    {
        CamlQuery GetCamlQuery(IEnumerable<ICondition> conditions = null);
    }
}