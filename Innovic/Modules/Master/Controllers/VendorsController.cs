using Innovic.App;
using Innovic.Infrastructure;
using Innovic.Modules.Master.Models;
using Innovic.Modules.Master.Options;
using Microsoft.AspNet.Identity;
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
    [RoutePrefix("api/Vendors")]
    [Authorize]
    public class VendorsController : ApiController
    {
        private readonly InnovicContext _context;
        private readonly string _userId;
        private readonly BaseRepository<Vendor> _vendorRepository;

        public VendorsController()
        {
            _userId = RequestContext.Principal.Identity.GetUserId();
            _context = new InnovicContext(_userId);
            _vendorRepository = new BaseRepository<Vendor>(_context, _userId);
        }

        [Route("")]
        public IHttpActionResult Get()
        {
            return Ok(_vendorRepository.Get().ToPickDictionaryCollection(PickConfigurations.Default));
        }

        [Route("{id}")]
        public IHttpActionResult Get(string id)
        {
            Vendor vendor = _vendorRepository.GetByID(id);
            if (vendor == null)
            {
                return NotFound();
            }

            return Ok(vendor.ToPickDictionary(PickConfigurations.Default));
        }

        [Route("{id}")]
        public IHttpActionResult Put(string id, VendorUpdateOptions options)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Vendor existingVendor = _vendorRepository.GetByID(id);
            Vendor updatedVendor = _vendorRepository.UpdateExistingWineModel(existingVendor, options);

            // Service calls go here

            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VendorExists(id))
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
        public IHttpActionResult Post(VendorInsertOptions options)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Vendor vendor = _vendorRepository.CreateNewWineModel(options);

            // Service calls go here

            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (VendorExists(vendor.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return Ok(vendor.ToPickDictionary(PickConfigurations.Default));
        }

        [Route("{id}")]
        public IHttpActionResult Delete(string id)
        {
            Vendor vendor = _vendorRepository.GetByID(id);

            if (vendor == null)
            {
                return NotFound();
            }

            _context.Vendors.Remove(vendor);
            _context.SaveChanges();

            return Ok(vendor.ToPickDictionary(PickConfigurations.Default));
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

                excelManager.ToVendors(provider.FileData[0].LocalFileName);

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

        private bool VendorExists(string id)
        {
            return _context.Vendors.Count(e => e.Id == id) > 0;
        }
    }
}
