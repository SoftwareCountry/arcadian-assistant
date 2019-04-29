namespace Arcadia.Assistant.CSP.Cache
{
    using System;
    using System.Threading.Tasks;

    using Akka.Actor;
    using Akka.Event;

    using Microsoft.Extensions.Caching.Memory;

    public abstract class CachedActor : UntypedActor, ILogReceive, IWithUnboundedStash
    {
        private readonly IMemoryCache memoryCache;
        private readonly TimeSpan cachePeriod;

        private readonly ILoggingAdapter logger = Context.GetLogger();

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
                    this.Become(this.OnRefreshReceive);

                    this.LoadValue().PipeTo(
                        this.Self,
                        success: value =>
                        {
                            this.SetToCache(value);
                            return Refresh.Success.Instance;
                        },
                        failure: err => new Refresh.Failure(err));
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

        private void OnRefreshReceive(object message)
        {
            switch (message)
            {
                case Refresh.Success _:
                    this.BecomeDefault();
                    break;

                case Refresh.Failure msg:
                    this.logger.Warning($"Exception thrown on cache refresh in {this.Self.Path}: {msg.Exception}");
                    this.BecomeDefault();
                    break;

                default:
                    this.Stash.Stash();
                    break;
            }
        }

        private void BecomeDefault()
        {
            this.Stash.UnstashAll();
            this.Become(this.OnReceive);
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