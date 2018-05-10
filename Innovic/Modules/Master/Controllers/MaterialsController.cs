using Innovic.App;
using Innovic.Infrastructure;
using Innovic.Modules.Master.Models;
using Innovic.Modules.Master.Options;
using Innovic.Modules.Master.Services;
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

namespace Innovic.Modules.Master.Controllers
{
    [RoutePrefix("api/Materials")]
    [Authorize]
    public class MaterialsController : ApiController
    {
        private readonly InnovicContext _context;
        private readonly string _userId;
        private readonly BaseRepository<Material> _materialRepository;

        public MaterialsController()
        {
            _userId = RequestContext.Principal.Identity.GetUserId();
            _context = new InnovicContext(_userId);
            _materialRepository = new BaseRepository<Material>(_context, _userId);
        }

        [Route("")]
        public IHttpActionResult Get()
        {
            return Ok(_materialRepository.Get().ToPickDictionaryCollection(PickConfigurations.Default));
        }

        [Route("{id}")]
        public IHttpActionResult Get(string id)
        {
            Material material = _materialRepository.GetByID(id);
            if (material == null)
            {
                return NotFound();
            }

            return Ok(material.ToPickDictionary(PickConfigurations.Default));
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

            // Service calls go here

            try
            {
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

            // Service calls go here

            try
            {
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

            return Ok(material.ToPickDictionary(PickConfigurations.Default));
        }

        [Route("{id}")]
        public IHttpActionResult Delete(string id)
        {
            Material material = _materialRepository.GetByID(id);
            if (material == null)
            {
                return NotFound();
            }

            if(!material.IsDeletable())
            {
                return Conflict();
            }

            _context.Materials.Remove(material);
            _context.SaveChanges();

            return Ok(material.ToPickDictionary(PickConfigurations.Default));
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

                excelManager.ToMaterials(provider.FileData[0].LocalFileName);

                try
                {
                    _context.SaveChanges();
                }
                catch (DbUpdateException e)
                {
                    throw e;
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

        private bool MaterialExists(string id)
        {
            return _context.Materials.Count(e => e.Id == id) > 0;
        }
    }
}
