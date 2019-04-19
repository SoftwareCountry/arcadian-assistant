namespace Arcadia.Assistant.ExternalStorages.SharepointOnline.Contracts
{
    public class SharepointField
    {
        public SharepointField(string name, string valueType)
        {
            this.Name = name;
            this.ValueType = valueType;
        }

        public string Name { get; }

        public string ValueType { get; }
    }
}