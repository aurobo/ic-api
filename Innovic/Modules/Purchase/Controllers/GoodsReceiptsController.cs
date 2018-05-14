using Innovic.App;
using Innovic.Infrastructure;
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

        [Route("")]
        public IHttpActionResult Post(GoodsReceiptInsertOptions options)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            GoodsReceipt goodsReceipt = _goodsReceiptRepository.CreateNewWineModel(options);

            if (!goodsReceipt.IsInsertable())
            {
                return BadRequest("Can't insert the GoodsReceipt.");
            }

            goodsReceipt.AddMaterialQuantity();

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

        [HttpPost]
        [Route("upload")]
        public async Task<HttpResponseMessage> UploadAsync()
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            var provider = new MultipartFormDataStreamProvider(HostingEnvironment.MapPath("~/App_Data"));

            try
            {
                await Request.Content.ReadAsMultipartAsync(provider);

                ExcelManager excelManager = new ExcelManager(_context, _userId);

                var errors = excelManager.ValidateForGoodsReceipt(provider.FileData[0].LocalFileName);

                if (errors.Count > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, errors);
                }

                var goodsReceipt = excelManager.ToGoodsReceipt(provider.FileData[0].LocalFileName);

                goodsReceipt.AddMaterialQuantity();

                try
                {
                    _context.SaveChanges();
                }
                catch (DbUpdateException)
                {
                    if (goodsReceiptExists(goodsReceipt.Id))
                    {
                        return Request.CreateResponse(HttpStatusCode.Conflict);
                    }
                    else
                    {
                        throw;
                    }
                }

                if (System.IO.File.Exists(provider.FileData[0].LocalFileName))
                {
                    System.IO.File.Delete(provider.FileData[0].LocalFileName);
                }

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, e);
            }
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
