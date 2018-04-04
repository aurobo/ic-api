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
using System.Web;
using System.Web.Hosting;
using System.Web.Http;

namespace Innovic.Modules.Purchase.Controllers
{
    [RoutePrefix("api/GoodsIssues")]
    [Authorize]
    public class GoodsIssuesController : ApiController
    {
        private readonly InnovicContext _context;
        private readonly string _userId;
        private readonly BaseRepository<GoodsIssue> _goodsIssueRepository;

        public GoodsIssuesController()
        {
            _userId = RequestContext.Principal.Identity.GetUserId();
            _context = new InnovicContext(_userId);
            _goodsIssueRepository = new BaseRepository<GoodsIssue>(_context, _userId);
        }

        [Route("")]
        public IHttpActionResult Get()
        {
            var goodsIssues = _goodsIssueRepository.Get();
            goodsIssues.ToList().ForEach(x => x.AddMetaData(GoodsIssueFlow.AddRemainingQuantity));
            goodsIssues.ToList().ForEach(x => x.AddMetaData(GoodsIssueFlow.TotalRemainingQuantity));
            return Ok(goodsIssues.ToPickDictionaryCollection(PickConfigurations.GoodsIssues));
        }

        [Route("{id}")]
        public IHttpActionResult Get(string id)
        {
            GoodsIssue goodsIssue = _goodsIssueRepository.GetByID(id);

            if (goodsIssue == null)
            {
                return NotFound();
            }

            goodsIssue.AddMetaData(GoodsIssueFlow.AddRemainingQuantity);
            goodsIssue.AddMetaData(GoodsIssueFlow.TotalRemainingQuantity);

            return Ok(goodsIssue.ToPickDictionary(PickConfigurations.GoodsIssue));
        }

        [Route("")]
        public IHttpActionResult Post(GoodsIssueInsertOptions options)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            GoodsIssue goodsIssue = _goodsIssueRepository.CreateNewWineModel(options);

            if (!goodsIssue.IsInsertable())
            {
                return BadRequest("Can't insert the GoodsIssue.");
            }

            goodsIssue.SubtractMaterialQuantity();

            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (GoodsIssueExists(goodsIssue.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return Ok(goodsIssue.ToPickDictionary(new PickConfig(true, true)));
        }

        [Route("{id}")]
        public IHttpActionResult Delete(string id)
        {
            GoodsIssue goodsIssue = _goodsIssueRepository.GetByID(id);
            if (goodsIssue == null)
            {
                return NotFound();
            }

            _context.GoodsIssues.Remove(goodsIssue);
            _context.SaveChanges();

            return Ok(goodsIssue.ToPickDictionary(new PickConfig(true, true)));
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

                var errors = excelManager.ValidateForGoodsIssue(provider.FileData[0].LocalFileName);

                if (errors.Count > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, errors);
                }

                var goodsIssue = excelManager.ToGoodsIssue(provider.FileData[0].LocalFileName);

                try
                {
                    _context.SaveChanges();
                }
                catch (DbUpdateException)
                {
                    if (GoodsIssueExists(goodsIssue.Id))
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

        private bool GoodsIssueExists(string id)
        {
            return _context.GoodsReceipts.Count(e => e.Id == id) > 0;
        }
    }
}