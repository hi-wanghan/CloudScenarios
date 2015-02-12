using ArenaTest.Models;
using Microsoft.ServiceBus;
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
    public class TestController : Controller
    {
        static bool initialized = false;

        string ConnectionString =
        CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ArenaTest");

        string TopicName = CloudConfigurationManager.GetSetting("TopicName");

        string SubscriptionName = CloudConfigurationManager.GetSetting("SubscriptionName");

        private ArenaTestModelsContainer db = new ArenaTestModelsContainer();

        private static TestView testView = new TestView();

        [HttpPost]
        public ActionResult GetDescription(string name)
        {
            string value = "";
            testView.TestDescriptions.TryGetValue(name, out value);

            return Json(new {desc = value});
        }

        // GET: /Test/
        public ActionResult Index()
        {

            InitializeServiceBus();

            return View(testView);
        }

        public ActionResult Angular()
        {
            return View(testView);
        }

        public ActionResult BootStrap()
        {
            return View(testView);
        }

        public void InitializeServiceBus()
        {
            if (!initialized)
            {
                SbCreateTopic();
                sbCreateSubscription();
                initialized = true;
            }

        }

        public void SbCreateTopic()
        {
            // Create a new Topic with custom settings

            var namespaceManager =
                NamespaceManager.CreateFromConnectionString(ConnectionString);

            if (!namespaceManager.TopicExists(TopicName))
            {
                // Configure Topic Settings
                TopicDescription td = new TopicDescription(TopicName);
                td.MaxSizeInMegabytes = 5120;
                td.DefaultMessageTimeToLive = new TimeSpan(0, 1, 0);
                namespaceManager.CreateTopic(td);
            }

        }

        //agents should use this method to subscribe topics on my cloudArena service bus 
        //Once is enough, so check it everytime before invoking the method
        //han: Agent read the SubscriptionList from App.config file

        public void sbCreateSubscription()
        {

            var namespaceManager =
                NamespaceManager.CreateFromConnectionString(ConnectionString);

            string[] Split = SubscriptionName.Split(',');

            for (int i = 0; i < Split.Length; i++)
            {
                if (!namespaceManager.SubscriptionExists(TopicName, Split[i]))
                {
                    namespaceManager.CreateSubscription(TopicName, Split[i]);
                }
            }

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
