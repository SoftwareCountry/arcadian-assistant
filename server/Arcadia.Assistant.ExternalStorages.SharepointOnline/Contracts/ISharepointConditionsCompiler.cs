namespace Arcadia.Assistant.ExternalStorages.SharepointOnline.Contracts
{
    using System.Collections.Generic;

    using Arcadia.Assistant.ExternalStorages.Abstractions;

    public interface ISharepointConditionsCompiler
    {
        string CompileConditions(IEnumerable<ICondition> conditions = null);
    }
}