//The Model is used for home page view

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ArenaTest.Models
{
    public class TestView
    {
        public List<SelectListItem> slist;

        public string TestId { get; set; }

        public string TestRunId { get; set; }

        public string selectedTest { get; set; }

        public Dictionary<string, string> TestDescriptions { get; set; }

        private ArenaTestModelsContainer db = new ArenaTestModelsContainer();

        [DisplayName("Azure")]
        public Boolean AzureChecked { get; set; }

        [DisplayName("AWS")]
        public Boolean AwsChecked { get; set; }

        [DisplayName("Google")]
        public Boolean GoogleChecked { get; set; }

        public void WriteTestList(string name, int id)
        {
            slist.Add(new SelectListItem() { Text = name, Value = id.ToString() + "-" + name });
        }

        public List<SelectListItem> GetAllTests()
        {
            return slist;
        }

        public void InitiateTestView()
        {
            slist = new List<SelectListItem>();
            TestDescriptions = new Dictionary<string, string>();
            //var TestNames = new List<String>();
            var Tests = db.Tests.ToList();

            for (int i = 0; i < Tests.Count; i++)
            {
                WriteTestList(Tests.ElementAt(i).TestName, Tests.ElementAt(i).TestId);
                TestDescriptions.Add(Tests.ElementAt(i).TestName, Tests.ElementAt(i).Description);
            }
        }

        public TestView()
        {
            AwsChecked = false;
            AzureChecked = true;
            GoogleChecked = false;
            InitiateTestView();
        }
    }
}