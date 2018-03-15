using Innovic.App;
using Innovic.Modules.Purchase.Models;
using Innovic.Modules.Purchase.Options;
using Innovic.Modules.Purchase.ProcessFlows;
using Innovic.Modules.Purchase.Services;
using Microsoft.AspNet.Identity;
using Red.Wine.Picker;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Web.Http;

namespace Innovic.Modules.Purchase.Controllers
{
    [RoutePrefix("api/PurchaseOrders")]
    [Authorize]
    public class PurchaseOrdersController : ApiController
    {
        private readonly InnovicContext _context;
        private readonly string _userId;
        private readonly BaseRepository<PurchaseOrder> _purchaseOrderRepository;

        public PurchaseOrdersController()
        {
            _userId = RequestContext.Principal.Identity.GetUserId();
            _context = new InnovicContext(_userId);
            _purchaseOrderRepository = new BaseRepository<PurchaseOrder>(_context, _userId);
        }

        [Route("")]
        public IHttpActionResult Get()
        {
            return Ok(_purchaseOrderRepository.Get().ToPickDictionaryCollection(PickConfigurations.PurchaseOrders));
        }

        [Route("{id}")]
        public IHttpActionResult Get(string id)
        {
            PurchaseOrder purchaseOrder = _purchaseOrderRepository.GetByID(id);

            if (purchaseOrder == null)
            {
                return NotFound();
            }

            return Ok(purchaseOrder.ToPickDictionary(PickConfigurations.PurchaseOrder));
        }

        [Route("{id}")]
        public IHttpActionResult Put(string id, PurchaseOrderUpdateOptions options)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != options.Id)
            {
                return BadRequest();
            }

            PurchaseOrder existingPurchaseOrder = _purchaseOrderRepository.GetByID(id);
            PurchaseOrder updatedPurchaseOrder = _purchaseOrderRepository.UpdateExistingWineModel(existingPurchaseOrder, options);

            PurchaseOrderService.Process(updatedPurchaseOrder, PurchaseOrderFlow.Update);

            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PurchaseOrderExists(id))
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
        public IHttpActionResult Post(PurchaseOrderInsertOptions options)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            PurchaseOrder purchaseOrder = _purchaseOrderRepository.CreateNewWineModel(options);
            PurchaseOrderService.Process(purchaseOrder, PurchaseOrderFlow.PopulateItemsFromPurchaseRequests, false);
            PurchaseOrderService.Process(purchaseOrder, PurchaseOrderFlow.CalculateItemCost);

            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (PurchaseOrderExists(purchaseOrder.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return Ok(purchaseOrder.ToPickDictionary(new PickConfig(true, true)));
        }

        [Route("{id}")]
        public IHttpActionResult Delete(string id)
        {
            PurchaseOrder purchaseOrder = _purchaseOrderRepository.GetByID(id);
            if (purchaseOrder == null)
            {
                return NotFound();
            }

            _context.PurchaseOrders.Remove(purchaseOrder);
            _context.SaveChanges();

            return Ok(purchaseOrder.ToPickDictionary(new PickConfig(true, true)));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool PurchaseOrderExists(string id)
        {
            return _context.PurchaseOrders.Count(e => e.Id == id) > 0;
        }
    }
}
