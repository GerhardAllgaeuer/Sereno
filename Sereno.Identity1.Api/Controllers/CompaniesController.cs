using AutoMapper;
using Sereno.Identity.Contracts;
using Sereno.Identity.Entities.DataTransferObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Sereno.Identity.Controllers
{
    [Route("api/companies")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        private readonly IRepositoryManager _repository;
        private readonly IMapper _mapper;

        public CompaniesController(IRepositoryManager repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [Authorize]
        [HttpGet]
        public IActionResult GetCompanies()
        {
            try
            {
                var claims = User.Claims;

                var companies = _repository.Company.GetAllCompanies(trackChanges: false);

                var companiesDto = _mapper.Map<IEnumerable<CompanyDto>>(companies);

                return Ok(companiesDto);
            }
            catch
            {
                return StatusCode(500, "Internal server error");
            }
        }
    }
}