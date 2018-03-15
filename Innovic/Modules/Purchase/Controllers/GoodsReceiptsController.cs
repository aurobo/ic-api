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
using System.Web.Http;

namespace Innovic.Modules.Purchase.Controllers
{
    [RoutePrefix("api/GoodsReceipts")]
    [Authorize]
    public class GoodsReceiptsController : ApiController
    {
        private readonly InnovicContext _context;
        private readonly string _userId;
        private readonly BaseRepository<GoodsReceipt> _goodsReceiptRepository;

        public GoodsReceiptsController()
        {
            _userId = RequestContext.Principal.Identity.GetUserId();
            _context = new InnovicContext(_userId);
            _goodsReceiptRepository = new BaseRepository<GoodsReceipt>(_context, _userId);
        }

        [Route("")]
        public IHttpActionResult Get()
        {
            return Ok(_goodsReceiptRepository.Get().ToPickDictionaryCollection(PickConfigurations.GoodsReceipts));
        }

        [Route("{id}")]
        public IHttpActionResult Get(string id)
        {
            GoodsReceipt goodsReceipt = _goodsReceiptRepository.GetByID(id);

            if (goodsReceipt == null)
            {
                return NotFound();
            }

            return Ok(goodsReceipt.ToPickDictionary(PickConfigurations.GoodsReceipt));
        }

        [Route("{id}")]
        public IHttpActionResult Put(string id, GoodsReceiptUpdateOptions options)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != options.Id)
            {
                return BadRequest();
            }

            GoodsReceipt existinggoodsReceipt = _goodsReceiptRepository.GetByID(id);
            GoodsReceipt updatedgoodsReceipt = _goodsReceiptRepository.UpdateExistingWineModel(existinggoodsReceipt, options);

            GoodsReceiptService.Process(updatedgoodsReceipt, GoodsReceiptFlow.Update);

            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!goodsReceiptExists(id))
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
        public IHttpActionResult Post(GoodsReceiptInsertOptions options)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            GoodsReceipt goodsReceipt = _goodsReceiptRepository.CreateNewWineModel(options);
            GoodsReceiptService.Process(goodsReceipt, GoodsReceiptFlow.PopulateFromPurchaseOrder);
            GoodsReceiptService.Process(goodsReceipt,GoodsReceiptFlow.SetDefaultStatus); 

            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (goodsReceiptExists(goodsReceipt.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return Ok(goodsReceipt.ToPickDictionary(new PickConfig(true, true)));
        }

        [Route("{id}")]
        public IHttpActionResult Delete(string id)
        {
            GoodsReceipt goodsReceipt = _goodsReceiptRepository.GetByID(id);
            if (goodsReceipt == null)
            {
                return NotFound();
            }

            _context.GoodsReceipts.Remove(goodsReceipt);
            _context.SaveChanges();

            return Ok(goodsReceipt.ToPickDictionary(new PickConfig(true, true)));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool goodsReceiptExists(string id)
        {
            return _context.GoodsReceipts.Count(e => e.Id == id) > 0;
        }
    }
}
