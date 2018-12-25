namespace Arcadia.Assistant.InboxEmail.Abstractions
{
    public class InboxEmailEventBus
    {
        public InboxEmailEventBus(Email email)
        {
            this.Email = email;
        }

        public Email Email { get;  }
    }
}