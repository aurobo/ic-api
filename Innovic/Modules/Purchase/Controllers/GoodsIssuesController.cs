using Innovic.App;
using Innovic.Modules.Purchase.Models;
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
            var goodsIssues = _goodsIssueRepository.Get().ToList();
            if (goodsIssues.Count > 0)
            {
                GoodsIssueService.Process(goodsIssues, GoodsIssueItemFlow.AddRemainingQuantity);
                GoodsIssueService.Process(goodsIssues, GoodsIssueItemFlow.CanCreateGoodsReceipt);
            }
            return Ok(goodsIssues.ToPickDictionaryCollection(PickConfigurations.GoodsIssues));
        }
    }
}