using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ArenaTest.Models;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

//Upload file with MVC http://www.codeproject.com/Articles/442515/Uploading-and-returning-files-in-ASP-NET-MVC
//Upload with MVC http://cpratt.co/file-uploads-in-asp-net-mvc-with-view-models/


namespace ArenaTest.Controllers
{
    public class TestManagerController : Controller
    {
        private ArenaTestModelsContainer db = new ArenaTestModelsContainer();

        private static string ConnectionString = CloudConfigurationManager.GetSetting("ArenaTestStorageLocation");

        private CloudStorageAccount storageAccount =
            Microsoft.WindowsAzure.Storage.CloudStorageAccount.Parse(ConnectionString);

        // GET: /TestManager/
        public ActionResult Index()
        {
            return View(db.Tests.ToList());
        }

        // GET: /TestManager/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Test test = db.Tests.Find(id);
            if (test == null)
            {
                return HttpNotFound();
            }
            return View(test);
        }

        public byte[] convertFile(HttpPostedFileBase mydata)
        {
            System.IO.Stream str;
            Int32 strLen, strRead;
            // Create a Stream object.
            str = mydata.InputStream;
            // Find number of bytes in stream.
            strLen = Convert.ToInt32(str.Length);
            // Create a byte array.
            byte[] strArr = new byte[strLen];
            // Read stream into byte array.
            strRead = str.Read(strArr, 0, strLen);

            return strArr;

        }

        // GET: /TestManager/Create
        public ActionResult Create()
        {
            return View(new TestManagerViewModel());
        }

        // POST: /TestManager/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TestName,Description")] TestManagerViewModel model)
        {

            char[] delimiterChars = { '/', '\\' };
            var validTypes = new string[]
            {
                "image/gif",
                "image/jpeg",
                "image/pjpeg",
                "image/png",
                "application/x-zip-compressed"
            };
            HttpPostedFileBase Resource = Request.Files["testresource"];
            string FileName = (string)Resource.FileName.Split(delimiterChars).Last<string>();
            string contextType = Resource.ContentType;
        http://cpratt.co/file-uploads-in-asp-net-mvc-with-view-models/

            if (!validTypes.Contains(Resource.ContentType))
            {
                ModelState.AddModelError("FileUpload", "Please choose either a GIF, JPG or PNG image.");
            }

            //upload file to blob
            if (Resource != null)
            {
                // Create the blob client.
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer container = blobClient.GetContainerReference("dllfiles");
                container.CreateIfNotExists();

                CloudBlockBlob blockBlob = container.GetBlockBlobReference(FileName);

                if (!blockBlob.Exists())
                    blockBlob.UploadFromStream(Resource.InputStream);
            }

            //initial test object with model

            if (ModelState.IsValid)
            {
                Test test = new Test
                {
                    TestName = model.TestName,
                    Description = model.Description
                };
                db.Tests.Add(test);
                db.SaveChanges();

            }
            
            return RedirectToAction("Index");
        }

        // GET: /TestManager/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Test test = db.Tests.Find(id);
            if (test == null)
            {
                return HttpNotFound();
            }
            return View(test);
        }

        // POST: /TestManager/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TestId,TestName,Description")] Test test)
        {
            if (ModelState.IsValid)
            {
                db.Entry(test).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(test);
        }

        // GET: /TestManager/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Test test = db.Tests.Find(id);
            if (test == null)
            {
                return HttpNotFound();
            }
            return View(test);
        }

        // POST: /TestManager/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Test test = db.Tests.Find(id);
            db.Tests.Remove(test);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
