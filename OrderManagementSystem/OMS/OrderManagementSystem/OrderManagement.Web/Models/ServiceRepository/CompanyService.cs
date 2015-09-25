using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OrderManagement.Web.Models.Repository;

namespace OrderManagement.Web.Models.ServiceRepository
{

    public interface ICompanyService
    {
        IList<Company> GetAll();

    }

    public class CompanyService : ICompanyService
    {
        private ICompanyRepository _repository;
         public CompanyService(ICompanyRepository repository)
        {
            _repository = repository;
           
        }

        public IList<Company> GetAll()
        {
            _repository.SelectAll();
            return null;
        }
    }
}