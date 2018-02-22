using Innovic.App;
using Innovic.Modules.Master.Models;
using Red.Wine.Picker;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace Innovic.Modules.Master.Controllers
{
    [RoutePrefix("api/materials")]
    public class MaterialsController : ApiController
    {
        private InnovicContext _db = new InnovicContext();

        [Route("")]
        public IHttpActionResult Get()
        {
            return Ok(_db.Materials.ToPickDictionaryCollection(new PickConfig(true, true)));
        }

        [Route("{id}")]
        public IHttpActionResult Get(string id)
        {
            Material material = _db.Materials.Find(id);
            if (material == null)
            {
                return NotFound();
            }

            return Ok(material);
        }

        [Route("{id}")]
        public IHttpActionResult Put(string id, Material material)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != material.Id)
            {
                return BadRequest();
            }

            _db.Entry(material).State = EntityState.Modified;

            try
            {
                _db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MaterialExists(id))
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
        public IHttpActionResult Post(Material material)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _db.Materials.Add(material);

            try
            {
                _db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (MaterialExists(material.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return Ok(material);
        }

        [Route("{id}")]
        public IHttpActionResult Delete(string id)
        {
            Material material = _db.Materials.Find(id);
            if (material == null)
            {
                return NotFound();
            }

            _db.Materials.Remove(material);
            _db.SaveChanges();

            return Ok(material);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool MaterialExists(string id)
        {
            return _db.Materials.Count(e => e.Id == id) > 0;
        }
    }
}
