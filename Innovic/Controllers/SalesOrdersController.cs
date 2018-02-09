using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Innovic.Models;
using Innovic.Models.Sales;

namespace Innovic.Controllers
{
    public class SalesOrdersController : ApiController
    {
        private InnovicContext db = new InnovicContext();

        // GET: api/SalesOrders
        public IQueryable<SalesOrder> GetSalesOrders()
        {
            return db.SalesOrders;
        }

        // GET: api/SalesOrders/5
        [ResponseType(typeof(SalesOrder))]
        public IHttpActionResult GetSalesOrder(string id)
        {
            SalesOrder salesOrder = db.SalesOrders.Find(id);
            if (salesOrder == null)
            {
                return NotFound();
            }

            return Ok(salesOrder);
        }

        // PUT: api/SalesOrders/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutSalesOrder(string id, SalesOrder salesOrder)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != salesOrder.Id)
            {
                return BadRequest();
            }

            db.Entry(salesOrder).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
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

        // POST: api/SalesOrders
        [ResponseType(typeof(SalesOrder))]
        public IHttpActionResult PostSalesOrder(SalesOrder salesOrder)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.SalesOrders.Add(salesOrder);
            
            try
            {
                db.SaveChanges();
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

        // DELETE: api/SalesOrders/5
        [ResponseType(typeof(SalesOrder))]
        public IHttpActionResult DeleteSalesOrder(string id)
        {
            SalesOrder salesOrder = db.SalesOrders.Find(id);
            if (salesOrder == null)
            {
                return NotFound();
            }

            db.SalesOrders.Remove(salesOrder);
            db.SaveChanges();

            return Ok(salesOrder);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool SalesOrderExists(string id)
        {
            return db.SalesOrders.Count(e => e.Id == id) > 0;
        }
    }
}