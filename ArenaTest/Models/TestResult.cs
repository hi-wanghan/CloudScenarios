using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArenaTest.Models
{
public class TestResult : TableEntity
    {

        public double Result { get; set; }

        public TestResult(string testId, string testRunId, string platform, string testname, string method)
        {
            PartitionKey = testId + "-" + testRunId;
            RowKey = platform + "-" + testname + "-" + method;
        }

        public TestResult() { }

    }
}