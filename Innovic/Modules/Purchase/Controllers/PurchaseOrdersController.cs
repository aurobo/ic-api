using Innovic.App;
using Innovic.Modules.Purchase.Models;
using Innovic.Modules.Purchase.Options;
using Innovic.Modules.Purchase.ProcessFlows;
using Innovic.Modules.Purchase.Services;
using Microsoft.AspNet.Identity;
using Red.Wine.Picker;
using System.Data.Entity.Infrastructure;
using System.Linq;
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
            var purchaseOrders = _purchaseOrderRepository.Get();
            purchaseOrders.ToList().ForEach(po => po.AddMetaData(PurchaseOrderFlow.AddRemainingQuantity));
            purchaseOrders.ToList().ForEach(po => po.AddMetaData(PurchaseOrderFlow.TotalRemainingQuantity));
            return Ok(purchaseOrders.ToPickDictionaryCollection(PickConfigurations.PurchaseOrders));
        }

        [Route("{id}")]
        public IHttpActionResult Get(string id)
        {
            PurchaseOrder purchaseOrder = _purchaseOrderRepository.GetByID(id);

            if (purchaseOrder == null)
            {
                return NotFound();
            }

            purchaseOrder.AddMetaData(PurchaseOrderFlow.AddRemainingQuantity);
            purchaseOrder.AddMetaData(PurchaseOrderFlow.TotalRemainingQuantity);

            return Ok(purchaseOrder.ToPickDictionary(PickConfigurations.PurchaseOrder));
        }

        [Route("")]
        public IHttpActionResult Post(PurchaseOrderInsertOptions options)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            PurchaseOrder purchaseOrder = _purchaseOrderRepository.CreateNewWineModel(options);

            if(!purchaseOrder.IsInsertable())
            {
                return BadRequest("Can't insert the PurchaseOrder.");
            }

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
