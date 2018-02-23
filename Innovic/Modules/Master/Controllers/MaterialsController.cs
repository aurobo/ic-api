using Innovic.App;
using Innovic.Modules.Master.Models;
using Innovic.Modules.Master.Options;
using Microsoft.AspNet.Identity;
using Red.Wine;
using Red.Wine.Picker;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace Innovic.Modules.Master.Controllers
{
    [RoutePrefix("api/Materials")]
    public class MaterialsController : ApiController
    {
        private readonly InnovicContext _context;
        private readonly string _userId;
        private readonly GenericRepository<Material> _materialRepository;

        public MaterialsController()
        {
            _context = new InnovicContext();
            _userId = RequestContext.Principal.Identity.GetUserId();
            _materialRepository = new GenericRepository<Material>(_context, _userId);
        }

        [Route("")]
        public IHttpActionResult Get()
        {
            return Ok(_materialRepository.Get().ToPickDictionaryCollection(new PickConfig(true, true)));
        }

        [Route("{id}")]
        public IHttpActionResult Get(string id)
        {
            Material material = _materialRepository.GetByID(id);
            if (material == null)
            {
                return NotFound();
            }

            return Ok(material.ToPickDictionary(new PickConfig(true, true)));
        }

        [Route("{id}")]
        public IHttpActionResult Put(string id, MaterialUpdateOptions options)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != options.Id)
            {
                return BadRequest();
            }

            Material existingMaterial = _materialRepository.GetByID(id);
            Material updatedMaterial = _materialRepository.UpdateExistingWineModel(existingMaterial, options);
            _context.Materials.Attach(updatedMaterial);

            // Service calls go here

            try
            {
                _context.UpdateContextWithDefaultValues(_userId);
                _context.SaveChanges();
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
        public IHttpActionResult Post(MaterialInsertOptions options)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Material material = _materialRepository.CreateNewWineModel(options);
            _context.Materials.Add(material);

            // Service calls go here

            try
            {
                _context.UpdateContextWithDefaultValues(_userId);
                _context.SaveChanges();
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

            return Ok(material.ToPickDictionary(new PickConfig(true, true)));
        }

        [Route("{id}")]
        public IHttpActionResult Delete(string id)
        {
            Material material = _materialRepository.GetByID(id);
            if (material == null)
            {
                return NotFound();
            }

            _context.Materials.Remove(material);
            _context.SaveChanges();

            return Ok(material.ToPickDictionary(new PickConfig(true, true)));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool MaterialExists(string id)
        {
            return _context.Materials.Count(e => e.Id == id) > 0;
        }
    }
}
