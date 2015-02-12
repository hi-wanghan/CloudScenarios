//git status
//git add --all
//git commit - m "comments"
//git push

using ArenaTest.Models;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace ArenaTest.Controllers
{
    public class TestRunController : Controller
    {

        private ArenaTestModelsContainer db = new ArenaTestModelsContainer();
        string ConnectionString =
                CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ArenaTest");

        string TopicName = CloudConfigurationManager.GetSetting("TopicName");

        [HttpPost]
        public ActionResult Run(TestView tv)
        {
            string[] split = tv.selectedTest.Split('-');

            int testid = 0;
            int.TryParse(split[0],out testid);
            string testName = split[1];

            Console.WriteLine("hi there");

            TestRun tr = new TestRun
            {
                //TestRunId is the primary key which will be generated automatically by system
                TestId = testid,
                Platform = "won't use it. will delete it in future"
            };

            if (ModelState.IsValid)
            {
                db.TestRuns.Add(tr);
                db.SaveChanges();
            }

            SbSendMsg(testid, tr.TestRunId, tv, testName);

            tv.TestId = testid.ToString();
            tv.TestRunId = tr.TestRunId.ToString();

            Console.WriteLine("ok, the TestRunController side is done");
            //return to Views.TestRun.Run.cshtml
            return View(tv);

        }

        /*
         * Han
         * 1. Upload the TestRun data to database
         * 2. put the test to Service Bus Queue
         * 3. Show a result web page
         */

        public ActionResult RunWork(int testid, string platform, string testName)
        {
            int newPrimaryKey = 0;

            TestRun tr = new TestRun
            {
                //TestRunId is the primary key which will be generated automatically by system
                TestId = testid,
                Platform = platform
            };

            if (ModelState.IsValid)
            {

                db.TestRuns.Add(tr);
                db.SaveChanges();
                newPrimaryKey = tr.TestRunId;
            }

            //SbSendMsg(testid, newPrimaryKey, platform, testName);

            Console.WriteLine("ok, the TestRunController side is done");

            return View(tr);

            //return RedirectToAction("Done", "Test");
        }


        //han send message to the service bus
        //han: Just use Message.Properties to pass the value
        public void SbSendMsg(int id, int testRunid, TestView tv, string testName)
        {

            TopicClient Client =
                TopicClient.CreateFromConnectionString(ConnectionString, TopicName);

            BrokeredMessage message = new BrokeredMessage();
            message.Properties["TestId"] = id;
            message.Properties["TestRunId"] = testRunid;
            message.Properties["TestName"] = testName;
            ComposePlatform(message, tv);

            Client.Send(message);

        }

        public void ComposePlatform(BrokeredMessage message, TestView tv)
        {
            if (tv.AzureChecked)
            {
                message.Properties["Azure"] = 1;
            }
            if (tv.AwsChecked)
            {
                message.Properties["Aws"] = 1;
            }
            if (tv.GoogleChecked)
            {
                message.Properties["Google"] = 1;
            }
        }


        // GET: /TestRun/
        public ActionResult Index(TestRun tr)
        {

            return View(db.TestRuns.ToList());
        }

        // GET: /TestRun/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TestRun testrun = db.TestRuns.Find(id);
            if (testrun == null)
            {
                return HttpNotFound();
            }
            return View(testrun);
        }

        // GET: /TestRun/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TestRun testrun = db.TestRuns.Find(id);
            if (testrun == null)
            {
                return HttpNotFound();
            }
            return View(testrun);
        }

        // POST: /TestRun/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TestRunId,Platform,TestId")] TestRun testrun)
        {
            if (ModelState.IsValid)
            {
                db.Entry(testrun).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(testrun);
        }

        // GET: /TestRun/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TestRun testrun = db.TestRuns.Find(id);
            if (testrun == null)
            {
                return HttpNotFound();
            }
            return View(testrun);
        }

        // POST: /TestRun/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TestRun testrun = db.TestRuns.Find(id);
            db.TestRuns.Remove(testrun);
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
