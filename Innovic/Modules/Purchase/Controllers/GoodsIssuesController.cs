using Innovic.App;
using Innovic.Modules.Purchase.Models;
using Innovic.Modules.Purchase.Options;
using Innovic.Modules.Purchase.ProcessFlows;
using Innovic.Modules.Purchase.Services;
using Microsoft.AspNet.Identity;
using Red.Wine.Picker;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
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
            GoodsIssue goodsIsue = _goodsIssueRepository.GetByID(id);

            if (goodsIsue == null)
            {
                return NotFound();
            }
            
            return Ok(goodsIsue.ToPickDictionary(PickConfigurations.GoodsIssue));
        }


        [Route("")]
        public IHttpActionResult Post(GoodsIssueInsertOptions options)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            GoodsIssue goodsIssue = _goodsIssueRepository.CreateNewWineModel(options);
            GoodsIssueService.Process(goodsIssue, GoodsIssueFlow.Insert);

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
        public IHttpActionResult Put(string id, GoodsIssueUpdateOptions options)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != options.Id)
            {
                return BadRequest();
            }

            GoodsIssue existingGoodsIssue = _goodsIssueRepository.GetByID(id);
            GoodsIssue updatedGoodsIssue = _goodsIssueRepository.UpdateExistingWineModel(existingGoodsIssue, options);

            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GoodsIssueExists(id))
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


        [Route("{id}")]
        public IHttpActionResult Delete(string id)
        {
            GoodsIssue goodIssue = _goodsIssueRepository.GetByID(id);
            if (goodIssue == null)
            {
                return NotFound();
            }

            _context.GoodsIssues.Remove(goodIssue);
            _context.SaveChanges();

            return Ok(goodIssue.ToPickDictionary(new PickConfig(true, true)));
        }


        private bool GoodsIssueExists(string id)
        {
            return _context.GoodsIssues.Count(e => e.Id == id) > 0;
        }
    }
}