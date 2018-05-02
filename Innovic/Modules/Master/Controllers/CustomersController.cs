using Innovic.App;
using Innovic.Modules.Master.Models;
using Innovic.Modules.Master.Options;
using Innovic.Modules.Sales.Options;
using Innovic.Modules.Sales.ProcessFlows;
using Innovic.Modules.Sales.Services;
using Microsoft.AspNet.Identity;
using Red.Wine.Picker;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace Innovic.Modules.Master.Controllers
{
    [RoutePrefix("api/Customers")]
    [Authorize]
    public class CustomersController : ApiController
    {
        private readonly InnovicContext _context;
        private readonly string _userId;
        private readonly BaseRepository<Customer> _customerRepository;

        public CustomersController()
        {
            _userId = RequestContext.Principal.Identity.GetUserId();
            _context = new InnovicContext(_userId);
            _customerRepository = new BaseRepository<Customer>(_context, _userId);
        }

        [Route("")]
        public IHttpActionResult Get()
        {
            return Ok(_customerRepository.Get().ToPickDictionaryCollection(PickConfigurations.Default));
        }

        [Route("{id}")]
        public IHttpActionResult Get(string id)
        {
            Customer customer = _customerRepository.GetByID(id);
            if (customer == null)
            {
                return NotFound();
            }

            return Ok(customer.ToPickDictionary(PickConfigurations.Default));
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

            try
            {
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

            try
            {
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

            return Ok(customer.ToPickDictionary(PickConfigurations.Default));
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

            return Ok(customer.ToPickDictionary(PickConfigurations.Default));
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
