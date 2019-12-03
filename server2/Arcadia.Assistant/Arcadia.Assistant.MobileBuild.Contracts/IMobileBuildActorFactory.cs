namespace Arcadia.Assistant.MobileBuild.Contracts
{
    /// <summary>
    ///     This interface defines the methods exposed by an actor.
    ///     Clients use this interface to interact with the actor that implements it.
    /// </summary>
    public interface IMobileBuildActorFactory
    {
        IMobileBuildActor MobileBuild(string type);
    }
}