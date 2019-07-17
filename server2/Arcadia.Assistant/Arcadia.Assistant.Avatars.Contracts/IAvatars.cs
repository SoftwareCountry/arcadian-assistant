namespace Arcadia.Assistant.Avatars.Contracts
{
    using Employees.Contracts;

    public interface IAvatars
    {
        IAvatar Get(EmployeeId employeeId);
    }
}