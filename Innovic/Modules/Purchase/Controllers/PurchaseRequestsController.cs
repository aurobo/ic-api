using Innovic.App;
using Innovic.Infrastructure;
using Innovic.Modules.Purchase.Models;
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
using System.Web;
using System.Web.Hosting;
using System.Web.Http;

namespace Innovic.Modules.Purchase.Controllers
{
    [RoutePrefix("api/PurchaseRequests")]
    [Authorize]
    public class PurchaseRequestsController : ApiController
    {
        private readonly InnovicContext _context;
        private readonly string _userId;
        private readonly BaseRepository<PurchaseRequest> _purchaseRequestRepository;

        public PurchaseRequestsController()
        {
            _userId = RequestContext.Principal.Identity.GetUserId();
            _context = new InnovicContext(_userId);
            _purchaseRequestRepository = new BaseRepository<PurchaseRequest>(_context, _userId);
        }

        [Route("")]
        public IHttpActionResult Get()
        {
            return Ok(_purchaseRequestRepository.Get().ToPickDictionaryCollection(PickConfigurations.PurchaseRequests));
        }

        [Route("{id}")]
        public IHttpActionResult Get(string id)
        {
            PurchaseRequest purchaseRequest = _purchaseRequestRepository.GetByID(id);

            if (purchaseRequest == null)
            {
                return NotFound();
            }

            PurchaseRequestService.Process(purchaseRequest, PurchaseRequestFlow.AddRemainingQuantity);

            return Ok(purchaseRequest.ToPickDictionary(PickConfigurations.PurchaseRequests));
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

                var errors = excelManager.ValidateForPurchaseRequest(provider.FileData[0].LocalFileName);

                if (errors.Count > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, errors);
                }

                var purchaseRequest = excelManager.ToPurchaseRequest(provider.FileData[0].LocalFileName);

                try
                {
                    _context.SaveChanges();
                }
                catch (DbUpdateException)
                {
                    if (PurchaseRequestExists(purchaseRequest.Id))
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

        private bool PurchaseRequestExists(string id)
        {
            return _context.PurchaseRequests.Count(e => e.Id == id) > 0;
        }
    }
}