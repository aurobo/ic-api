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
    [RoutePrefix("api/Invoices")]
    [Authorize]
    public class InvoicesController : ApiController
    {
        private readonly InnovicContext _context;
        private readonly string _userId;
        private readonly BaseRepository<Invoice> _invoiceRepository;

        public InvoicesController()
        {
            _userId = RequestContext.Principal.Identity.GetUserId();
            _context = new InnovicContext(_userId);
            _invoiceRepository = new BaseRepository<Invoice>(_context, _userId);
        }

        [Route("")]
        public IHttpActionResult Get()
        {
            return Ok(_invoiceRepository.Get().ToPickDictionaryCollection(PickConfigurations.Default));
        }

        [Route("{id}")]
        public IHttpActionResult Get(string id)
        {
            Invoice invoice = _invoiceRepository.GetByID(id);
            if (invoice == null)
            {
                return NotFound();
            }

            return Ok(invoice.ToPickDictionary(PickConfigurations.Default));
        }

        [Route("{id}")]
        public IHttpActionResult Put(string id, InvoiceUpdateOptions options)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != options.Id)
            {
                return BadRequest();
            }

            Invoice existingInvoice = _invoiceRepository.GetByID(id);
            Invoice updatedInvoice = _invoiceRepository.UpdateExistingWineModel(existingInvoice, options);

            InvoiceService.Process(updatedInvoice, InvoiceFlow.Update);

            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InvoiceExists(id))
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
        public IHttpActionResult Post(InvoiceInsertOptions options)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Invoice invoice = _invoiceRepository.CreateNewWineModel(options);

            InvoiceService.Process(invoice, InvoiceFlow.Insert);

            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (InvoiceExists(invoice.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return Ok(invoice.ToPickDictionary(new PickConfig(true, true)));
        }

        [Route("{id}")]
        public IHttpActionResult Delete(string id)
        {
            Invoice invoice = _invoiceRepository.GetByID(id);
            if (invoice == null)
            {
                return NotFound();
            }

            _context.Invoices.Remove(invoice);
            _context.SaveChanges();

            return Ok(invoice.ToPickDictionary(new PickConfig(true, true)));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool InvoiceExists(string id)
        {
            return _context.Invoices.Count(e => e.Id == id) > 0;
        }
    }
}
