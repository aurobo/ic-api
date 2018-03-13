using Innovic.App;
using Innovic.Modules.Purchase.Models;
using Innovic.Modules.Purchase.Options;
using Innovic.Modules.Purchase.ProcessFlows;
using Innovic.Modules.Purchase.Services;
using Microsoft.AspNet.Identity;
using Red.Wine.Picker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Innovic.Modules.Purchase.Controllers
{
    [RoutePrefix("api/GoodsIssue")]
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
            return Ok(_goodsIssueRepository.Get().ToPickDictionaryCollection(PickConfigurations.GoodsIssue));
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
    }
}