using Innovic.App;
using Innovic.Modules.Sales.Models;
using Innovic.Modules.Sales.Options;
using Innovic.Modules.Sales.ProcessFlows;
using Innovic.Modules.Sales.Services;
using Microsoft.AspNet.Identity;
using Red.Wine;
using Red.Wine.Picker;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace Innovic.Modules.Sales.Controllers
{
    [RoutePrefix("api/Customers")]
    public class CustomersController : ApiController
    {
        private readonly InnovicContext _context;
        private readonly string _userId;
        private readonly GenericRepository<Customer> _customerRepository;

        public CustomersController()
        {
            _context = new InnovicContext();
            _userId = RequestContext.Principal.Identity.GetUserId();
            _customerRepository = new GenericRepository<Customer>(_context, _userId);
        }

        [Route("")]
        public IHttpActionResult Get()
        {
            return Ok(_customerRepository.Get().ToPickDictionaryCollection(new PickConfig(true, true)));
        }

        [Route("{id}")]
        public IHttpActionResult Get(string id)
        {
            Customer customer = _customerRepository.GetByID(id);
            if (customer == null)
            {
                return NotFound();
            }

            return Ok(customer.ToPickDictionary(new PickConfig(true, true)));
        }

        [Route("{id}")]
        public IHttpActionResult Put(string id, CustomerUpdateOptions options)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != options.Id)
            {
                return BadRequest();
            }

            Customer existingCustomer = _customerRepository.GetByID(id);
            Customer updatedCustomer = _customerRepository.UpdateExistingWineModel(existingCustomer, options);
            _context.Customers.Attach(updatedCustomer);

            CustomerService.Process(updatedCustomer, CustomerFlow.Update);

            try
            {
                _context.UpdateContextWithDefaultValues(_userId);
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        [Route("")]
        public IHttpActionResult Post(CustomerInsertOptions options)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Customer customer = _customerRepository.CreateNewWineModel(options);
            _context.Customers.Add(customer);

            CustomerService.Process(customer, CustomerFlow.Insert);

            try
            {
                _context.UpdateContextWithDefaultValues(_userId);
                _context.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (CustomerExists(customer.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return Ok(customer.ToPickDictionary(new PickConfig(true, true)));
        }

        [Route("{id}")]
        public IHttpActionResult Delete(string id)
        {
            Customer customer = _customerRepository.GetByID(id);
            if (customer == null)
            {
                return NotFound();
            }

            _context.Customers.Remove(customer);
            _context.SaveChanges();

            return Ok(customer.ToPickDictionary(new PickConfig(true, true)));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CustomerExists(string id)
        {
            return _context.Customers.Count(e => e.Id == id) > 0;
        }
    }
}
