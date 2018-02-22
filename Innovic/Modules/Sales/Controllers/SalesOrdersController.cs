using Innovic.App;
using Innovic.Infrastructure;
using Innovic.Modules.Sales.Models;
using Innovic.Modules.Sales.Options;
using Innovic.Modules.Sales.ProcessFlows;
using Innovic.Modules.Sales.Services;
using Microsoft.AspNet.Identity;
using Red.Wine;
using Red.Wine.Picker;
using System;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Web.Http;

namespace Innovic.Modules.Sales.Controllers
{
    [RoutePrefix("api/salesorders")]
    public class SalesOrdersController : ApiController
    {
        private readonly InnovicContext _context;
        private readonly string _userId;
        private readonly GenericRepository<SalesOrder> _salesOrderRepository;

        public SalesOrdersController()
        {
            _context = new InnovicContext();
            _userId = RequestContext.Principal.Identity.GetUserId();
            _salesOrderRepository = new GenericRepository<SalesOrder>(_context, _userId);
        }

        [Route("")]
        public IHttpActionResult Get()
        {
            return Ok(_salesOrderRepository.Get().ToPickDictionaryCollection(new PickConfig(true, true)));
        }

        [Route("{id}")]
        public IHttpActionResult Get(string id)
        {
            SalesOrder salesOrder = _salesOrderRepository.GetByID(id);
            if (salesOrder == null)
            {
                return NotFound();
            }

            return Ok(salesOrder.ToPickDictionary(new PickConfig(true, true)));
        }

        [Route("{id}")]
        public IHttpActionResult Put(string id, SalesOrderUpdateOptions options)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != options.Id)
            {
                return BadRequest();
            }

            SalesOrder existingSalesOrder = _salesOrderRepository.GetByID(id);
            SalesOrder updatedSalesOrder = _salesOrderRepository.UpdateExistingWineModel(existingSalesOrder, options);
            _context.SalesOrders.Attach(updatedSalesOrder);

            SalesOrderService.Process(updatedSalesOrder, SalesOrderFlow.Update);

            try
            {
                _context.UpdateContextWithDefaultValues(_userId);
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SalesOrderExists(id))
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
        public IHttpActionResult Post(SalesOrderInsertOptions options)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            SalesOrder salesOrder = _salesOrderRepository.CreateNewWineModel(options);
            _context.SalesOrders.Add(salesOrder);

            SalesOrderService.Process(salesOrder, SalesOrderFlow.Insert);

            try
            {
                _context.UpdateContextWithDefaultValues(_userId);
                _context.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (SalesOrderExists(salesOrder.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return Ok(salesOrder.ToPickDictionary(new PickConfig(true, true)));
        }

        [Route("{id}")]
        public IHttpActionResult Delete(string id)
        {
            SalesOrder salesOrder = _salesOrderRepository.GetByID(id);
            if (salesOrder == null)
            {
                return NotFound();
            }

            _context.SalesOrders.Remove(salesOrder);
            _context.SaveChanges();

            return Ok(salesOrder.ToPickDictionary(new PickConfig(true, true)));
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

                var salesOrder = excelManager.ToSalesOrder(provider.FileData[0].LocalFileName);
                _context.SalesOrders.Add(salesOrder);

                SalesOrderService.Process(salesOrder, SalesOrderFlow.ImportExcel);

                try
                {
                    _context.UpdateContextWithDefaultValues(_userId);
                    _context.SaveChanges();
                }
                catch (DbUpdateException)
                {
                    if (SalesOrderExists(salesOrder.Id))
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

        private bool SalesOrderExists(string id)
        {
            return _context.SalesOrders.Count(e => e.Id == id) > 0;
        }
    }
}
