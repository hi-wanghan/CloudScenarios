using System.Collections.Generic;
using System.Web.Mvc;
using ArenaTest.Models;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace ArenaTest.Controllers
{
    public class TestResultController : Controller
    {

        //Initiate the TestResult Query Page
        public ActionResult Index(string testId, string testRunId, string testName)
        {
            string Pkey = testId + "-" + testRunId;

            ResultForView rv = new ResultForView(Pkey);
            rv.TestName = testName.Split('-')[1];


            return View(rv);
        }

        //Query from Table Storage
        public ActionResult Query(string testId, string testRunId)
        {
            var results = new List<TestResult>();
            //check if the TestResult is ready, but how long should I wait?
            string ConnectionString = CloudConfigurationManager.GetSetting("ArenaTestStorageLocation");

            // Retrieve the storage account from the connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConnectionString);
            Microsoft.WindowsAzure.Storage.Table.CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("TestResults");
            TableQuery<TestResult> query = null;
            string Pkey = null;

            if (!string.IsNullOrEmpty(testId) && !string.IsNullOrEmpty(testRunId))
            {
                Pkey = testId + "-" + testRunId;

                query = new TableQuery<TestResult>()
                    .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, Pkey));

            }
            else if (!string.IsNullOrEmpty(testId) && string.IsNullOrEmpty(testRunId))
            {
                Pkey = testId;
                query = new TableQuery<TestResult>()
                .Where(TableQuery.CombineFilters(
                TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.GreaterThanOrEqual, Pkey),
                TableOperators.And,
                TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.LessThan, testId + 1)));

            }

            ResultForView rv = new ResultForView(Pkey);

            if (query != null)
            {
                foreach (TestResult entity in table.ExecuteQuery(query))
                {

                    ComposeDataForView(rv, entity);
                }
            }

            rv.ComposeTableData();

            //create the /TestResult/Query.cshtml
            return View(rv);
        }

        public void ComposeDataForView(ResultForView rv, TestResult tr)
        {
            rv.Results.Add(tr.RowKey, tr.Result);

        }

        public ActionResult QueryFromSQL(string searchString)
        {
            ArenaTestModelsContainer db = new ArenaTestModelsContainer();

            //var movies = from m in db
            //             select m;

            //if (!String.IsNullOrEmpty(searchString))
            //{
            //    movies = movies.Where(s => s.Title.Contains(searchString));
            //}

            return View();
        }
    }

}