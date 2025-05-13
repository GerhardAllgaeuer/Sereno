using Sereno.Identity.Contracts;
using Sereno.Identity.Entities;

namespace Sereno.Identity.Repository
{
    public class RepositoryManager : IRepositoryManager
    {
        private RepositoryContext _repositoryContext;
        private ICompanyRepository? _companyRepository;

        public RepositoryManager(RepositoryContext repositoryContext)
        {
            _repositoryContext = repositoryContext;
        }

        public ICompanyRepository Company
        {
            get
            {
                if (_companyRepository == null)
                    _companyRepository = new CompanyRepository(_repositoryContext);

                return _companyRepository;
            }
        }

        public void Save() => _repositoryContext.SaveChanges();
    }
}