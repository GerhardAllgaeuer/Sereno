namespace Sereno.Identity.Contracts
{
    public interface IRepositoryManager
    {
        ICompanyRepository Company { get; }
        void Save();
    }
}
