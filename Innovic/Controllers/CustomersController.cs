using Innovic.Models;
using Innovic.Models.Sales;
using Red.Wine.Picker;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace Innovic.Controllers
{
    [RoutePrefix("api/customers")]
    public class CustomersController : ApiController
    {
        private InnovicContext _db = new InnovicContext();

        [Route("")]
        public IHttpActionResult Get()
        {
            return Ok(_db.Customers.ToPickDictionaryCollection(new PickConfig(true, true)));
        }

        [Route("{id}")]
        public IHttpActionResult Get(string id)
        {
            Customer customer = _db.Customers.Find(id);
            if (customer == null)
            {
                return NotFound();
            }

            return Ok(customer);
        }

        [Route("{id}")]
        public IHttpActionResult Put(string id, Customer customer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != customer.Id)
            {
                return BadRequest();
            }

            _db.Entry(customer).State = EntityState.Modified;

            try
            {
                _db.SaveChanges();
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
        public IHttpActionResult Post(Customer customer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _db.Customers.Add(customer);

            try
            {
                _db.SaveChanges();
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

            return Ok(customer);
        }

        [Route("{id}")]
        public IHttpActionResult Delete(string id)
        {
            Customer customer = _db.Customers.Find(id);
            if (customer == null)
            {
                return NotFound();
            }

            _db.Customers.Remove(customer);
            _db.SaveChanges();

            return Ok(customer);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CustomerExists(string id)
        {
            return _db.Customers.Count(e => e.Id == id) > 0;
        }
    }
}
