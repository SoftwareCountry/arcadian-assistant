namespace Arcadia.Assistant.CSP.Cache
{
    using System;
    using System.Threading.Tasks;

    using Akka.Actor;

    using Microsoft.Extensions.Caching.Memory;

    public abstract class CachedActor : UntypedActor, ILogReceive, IWithUnboundedStash
    {
        private readonly IMemoryCache memoryCache;
        private readonly TimeSpan cachePeriod;

        protected CachedActor(IMemoryCache memoryCache, TimeSpan cachePeriod)
        {
            this.memoryCache = memoryCache;
            this.cachePeriod = cachePeriod;

            Context.System.Scheduler.ScheduleTellRepeatedly(
                TimeSpan.Zero,
                cachePeriod,
                this.Self,
                Refresh.Instance,
                this.Self);
        }

        public IStash Stash { get; set; }

        protected abstract string CacheKey { get; }

        protected abstract Task<object> LoadValue();

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case Refresh _:
                    this.LoadValue().PipeTo(
                        this.Self,
                        success: value =>
                        {
                            this.SetToCache(value);
                            return Refresh.Success.Instance;
                        },
                        failure: err => new Refresh.Failure(err));
                    break;

                case Refresh.Success _:
                    break;

                case Refresh.Failure _:
                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }

        protected bool CanReceive(object message)
        {
            return message is Refresh || message is Refresh.Success || message is Refresh.Failure;
        }

        protected object GetFromCache()
        {
            if (this.memoryCache.TryGetValue<object>(this.CacheKey, out var value))
            {
                return value;
            }

            return null;
        }

        protected void SetToCache(object value)
        {
            this.memoryCache.Set(this.CacheKey, value, this.cachePeriod);
        }

        protected class Refresh
        {
            public static readonly Refresh Instance = new Refresh();

            public class Success
            {
                public static readonly Success Instance = new Success();
            }

            public class Failure
            {
                public Failure(Exception exception)
                {
                    this.Exception = exception;
                }

                public Exception Exception { get; }
            }
        }
    }
}