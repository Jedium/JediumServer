using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using Common.Logging;
using DomainInternal;

namespace Server2.Web
{
    public class AssetsController : ApiController
    {
        private readonly ILog _logger;
        private readonly IDatabaseAgent _database;

        public AssetsController(IDatabaseAgent database)
        {
            _logger = LogManager.GetLogger("[Assets Controller]");

            _database = database;
        }

        [HttpGet]
        public string BundleHash(string id)
        {
            Guid bid;
            if (Guid.TryParse(id, out bid))
            {
                var bundle = _database.GetAssetBundle(bid).Result;

                if (bundle != null) return bundle.Hash;
            }

            return string.Empty;
        }

        [HttpGet]
        public HttpResponseMessage Bundle(string id)
        {
            Guid bid;

            if (Guid.TryParse(id, out bid))
            {
                var bundle = _database.GetAssetBundle(bid).Result;

                if (bundle != null)
                {
                    string fpath = Path.Combine(Directory.GetCurrentDirectory(), "assets", bundle.BundleFile);

                    if (File.Exists(fpath))
                    {
                        var bytes = File.ReadAllBytes(fpath);

                        var dataStream = new MemoryStream(bytes);

                        HttpResponseMessage resp = Request.CreateResponse(HttpStatusCode.OK);
                        resp.Content = new StreamContent(dataStream);
                        resp.Content.Headers.ContentDisposition =
                            new ContentDispositionHeaderValue("attachment");
                        resp.Content.Headers.ContentDisposition.FileName = bundle.BundleFile;
                        resp.Content.Headers.ContentType =
                            new MediaTypeHeaderValue("application/octet-stream");


                        return resp;
                    }

                    _logger.Warn($"Can't find bundle file: {bundle}");
                }
                else
                {
                    _logger.Warn($"Can't find bundle with id: {id}");


#if DEBUG
                    var blist = _database.GetAllBundles().Result;

                    _logger.Warn($"Total bundle count: {blist.Count}");
                    foreach (var bnd in blist) _logger.Warn($"Bundle: {bnd.BundleId},{bnd.BundleFile}");
#endif
                }
            }
            else
            {
                _logger.Warn($"Can't parse bundle id: {id}");
            }

            HttpResponseMessage fresp = Request.CreateResponse(HttpStatusCode.NotFound);
            return fresp;
        }

        // GET api/<controller>/5
        public HttpResponseMessage Get(string id)
        {
          


            string fpath = Path.Combine(Directory.GetCurrentDirectory(), "assets", id);

            if (File.Exists(fpath))
            {
                var bytes = File.ReadAllBytes(fpath);

                var dataStream = new MemoryStream(bytes);

                HttpResponseMessage fresp = Request.CreateResponse(HttpStatusCode.OK);
                fresp.Content = new StreamContent(dataStream);
                fresp.Content.Headers.ContentDisposition =
                    new ContentDispositionHeaderValue("attachment");
                fresp.Content.Headers.ContentDisposition.FileName = id;
                fresp.Content.Headers.ContentType =
                    new MediaTypeHeaderValue("application/octet-stream");


                return fresp;
            }
            else
            {
                HttpResponseMessage fresp = Request.CreateResponse(HttpStatusCode.NotFound);
                return fresp;
            }
        }

        // POST api/<controller>
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}