namespace Arcadia.Assistant.ExternalStorages.SharepointOnline.Contracts
{
    using System.Collections.Generic;

    using Abstractions;

    public interface ISharepointConditionsCompiler
    {
        string? CompileConditions(IEnumerable<ICondition>? conditions = null);
    }
}