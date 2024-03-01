using Sereno.Identity.Entities.Models;

namespace Sereno.Identity.Contracts
{
    public interface ICompanyRepository
    {
        IEnumerable<Company> GetAllCompanies(bool trackChanges);
    }
}
