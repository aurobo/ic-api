using Innovic.App;
using Innovic.Modules.Purchase.Models;
using Innovic.Modules.Purchase.ProcessFlows;
using Innovic.Modules.Purchase.Services;
using Innovic.Modules.Sales.Options;
using Microsoft.AspNet.Identity;
using Red.Wine.Picker;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Innovic.Modules.Purchase.Controllers
{
    [RoutePrefix("api/Suppliers")]
    [Authorize]
    public class SuppliersController : ApiController
    {
        private readonly InnovicContext _context;
        private readonly string _userId;
        private readonly BaseRepository<Supplier> _customerRepository;

        public SuppliersController()
        {
            _userId = RequestContext.Principal.Identity.GetUserId();
            _context = new InnovicContext(_userId);
            _customerRepository = new BaseRepository<Supplier>(_context, _userId);
        }

        [Route("")]
        public IHttpActionResult Get()
        {
            return Ok(_customerRepository.Get().ToPickDictionaryCollection(PickConfigurations.Default));
        }

        [Route("{id}")]
        public IHttpActionResult Get(string id)
        {
            Supplier supplier = _customerRepository.GetByID(id);
            if (supplier == null)
            {
                return NotFound();
            }

            return Ok(supplier.ToPickDictionary(new PickConfig(true, true)));
        }

        [Route("{id}")]
        public IHttpActionResult Put(string id, SupplierUpdateOptions options)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != options.Id)
            {
                return BadRequest();
            }

            Supplier existingSupplier = _customerRepository.GetByID(id);
            Supplier updatedSupplier = _customerRepository.UpdateExistingWineModel(existingSupplier, options);

            SupplierService.Process(updatedSupplier, SupplierFlow.Update);

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

            Supplier supplier = _customerRepository.CreateNewWineModel(options);

            SupplierService.Process(supplier, SupplierFlow.Insert);

            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (CustomerExists(supplier.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return Ok(supplier.ToPickDictionary(new PickConfig(true, true)));
        }

        [Route("{id}")]
        public IHttpActionResult Delete(string id)
        {
            Supplier supplier = _customerRepository.GetByID(id);
            if (supplier == null)
            {
                return NotFound();
            }

            _context.Suppliers.Remove(supplier);
            _context.SaveChanges();

            return Ok(supplier.ToPickDictionary(new PickConfig(true, true)));
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
