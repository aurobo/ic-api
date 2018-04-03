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
using System.Web;
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
            return Ok(_goodsIssueRepository.Get().ToPickDictionaryCollection(PickConfigurations.GoodsIssues));
        }

        [Route("{id}")]
        public IHttpActionResult Get(string id)
        {
            GoodsIssue goodsIssue = _goodsIssueRepository.GetByID(id);

            if (goodsIssue == null)
            {
                return NotFound();
            }

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
                if (goodsIssueExists(goodsIssue.Id))
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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool goodsIssueExists(string id)
        {
            return _context.GoodsReceipts.Count(e => e.Id == id) > 0;
        }
    }
}