namespace Arcadia.Assistant.Configuration
{
    using Microsoft.Extensions.Configuration;

    public static class HoconContentExtensions
    {
        public static IConfigurationBuilder AddHoconContent(this IConfigurationBuilder builder, string path, string configKey, bool optional, bool reloadOnChange)
        {
            var source = new HoconContentSource(configKey);
            source.Path = path;
            source.Optional = optional;
            source.ReloadOnChange = reloadOnChange;

            builder.Add(source);

            return builder;
        }
    }
}