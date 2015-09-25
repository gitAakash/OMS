using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OrderManagement.Web.Models.Repository;

namespace OrderManagement.Web.Models.ServiceRepository
{
    public interface IContactService
    {
        IList<Contact> GetAll();

    }



    public class ContactService:IContactService
    {
        private ICompanyRepository _companyRepository;
        private IContactRepository _repository;
        public ContactService(IContactRepository repository)
        {
            _repository = repository;
            _companyRepository = new CompanyRepository();
        }


        public IList<Contact> GetAll()
        {
           return _repository.GetAll().ToList();
           
        }
    }
}