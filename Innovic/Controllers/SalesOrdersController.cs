using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Http.Description;
using Innovic.Models;
using Innovic.Models.Sales;
using Innovic.Services;

namespace Innovic.Controllers
{
    [RoutePrefix("api/salesorders")]
    public class SalesOrdersController : ApiController
    {
        private InnovicContext _db = new InnovicContext();

        [Route("")]
        public IQueryable<SalesOrder> Get()
        {
            return _db.SalesOrders;
        }

        [Route("{id}")]
        public IHttpActionResult Get(string id)
        {
            SalesOrder salesOrder = _db.SalesOrders.Find(id);
            if (salesOrder == null)
            {
                return NotFound();
            }

            return Ok(salesOrder);
        }

        [Route("{id}")]
        public IHttpActionResult Put(string id, SalesOrder salesOrder)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != salesOrder.Id)
            {
                return BadRequest();
            }

            _db.Entry(salesOrder).State = EntityState.Modified;

            try
            {
                _db.SaveChanges();
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
        public IHttpActionResult Post(SalesOrder salesOrder)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _db.SalesOrders.Add(salesOrder);
            
            try
            {
                _db.SaveChanges();
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

            return Ok(salesOrder);
        }

        [Route("{id}")]
        public IHttpActionResult Delete(string id)
        {
            SalesOrder salesOrder = _db.SalesOrders.Find(id);
            if (salesOrder == null)
            {
                return NotFound();
            }

            _db.SalesOrders.Remove(salesOrder);
            _db.SaveChanges();

            return Ok(salesOrder);
        }

        // POST: api/SalesOrders/Upload
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

                ExcelService excelService = new ExcelService();

                var salesOrder = excelService.ToSalesOrder(provider.FileData[0].LocalFileName);

                _db.SalesOrders.Add(salesOrder);

                try
                {
                    _db.SaveChanges();
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
                _db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool SalesOrderExists(string id)
        {
            return _db.SalesOrders.Count(e => e.Id == id) > 0;
        }
    }
}