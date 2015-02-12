using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.StorageClient;
using System;
using System.IO;
using System.Collections.Generic;
using System.IO.Compression;
using System.Reflection;

namespace CloudArenaTestAgent
{
    class TableStorageClient
    {

        private static string ConnectionString = CloudConfigurationManager.GetSetting("ArenaTestResources");
        private static string TargetPlatform = CloudConfigurationManager.GetSetting("targetPlatform");
        private static string ExtractPath = CloudConfigurationManager.GetSetting("extractPath");
        private static string TestResourceName = "";

        // Retrieve the storage account from the connection string.
        private Microsoft.WindowsAzure.Storage.CloudStorageAccount storageAccount =
            Microsoft.WindowsAzure.Storage.CloudStorageAccount.Parse(ConnectionString);


        //hanwan Access Han's TestDetail Table in SQL Server
        public TestDetail IdentifyTestDetail(string testId)
        {

            Microsoft.WindowsAzure.Storage.Table.CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Check if the table exists
            CloudTable table = tableClient.GetTableReference("TestDetails");

            //testId is the primary key & row key in the TestDetails table
            TableOperation retrieveOperation = TableOperation.Retrieve<TestDetail>(testId, TargetPlatform);
            TableResult retrieveResult = table.Execute(retrieveOperation);

            return (TestDetail)retrieveResult.Result;

        }

        //Download resource from Azure Blob
        public void DownloadTestResource(TestDetail td)
        {


            string ResourceFileName = td.FileName;
            string DllContainerRef = td.Container;
            TestResourceName = ExtractPath + ResourceFileName;
            string DllName = TestResourceName.Split('.')[0] + ".dll";

            if (System.IO.File.Exists(DllName))
            {
                return;
            }

            //Blob Container location where the test resource is sitting
            string DllContainerPath = td.Source;

            // Retrieve storage account from connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(DllContainerPath);

            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve reference to a previously created container.
            CloudBlobContainer container = blobClient.GetContainerReference(DllContainerRef);

            // Retrieve reference to a blob named "sbsm".

            CloudBlockBlob blockBlob = container.GetBlockBlobReference(ResourceFileName);

            // Save blob contents to a file.
            using (var fileStream = System.IO.File.OpenWrite(TestResourceName))
            {
                blockBlob.DownloadToStream(fileStream);
            }

            ZipFile.ExtractToDirectory(TestResourceName, ExtractPath);
        }

        public double ExecuteTestMethod(string targetType, string methodName)
        {
            string DllName = TestResourceName.Split('.')[0] + ".dll";
            var assembly = Assembly.LoadFile(DllName);
            //hanwan Get the Type from TestDetails Table
            Type type = assembly.GetType(targetType);

            // create instance of class Calculator
            object testInstance = Activator.CreateInstance(type);

            double result = (double)type.InvokeMember(methodName,
                        BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.Public, null, null, null);

            return result;

        }


        public string WriteTestResult(TestResult tr)
        {

            var insertOperation = TableOperation.InsertOrMerge(tr);
            Microsoft.WindowsAzure.Storage.Table.CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Create the table if it doesn't exist.
            CloudTable table = tableClient.GetTableReference("TestResults");
            table.CreateIfNotExists();
            // Execute the insert operation.
            table.Execute(insertOperation);

            return tr.PartitionKey;
        }

    }


    public class TestDetail : TableEntity
    {
        public string Type { get; set; }

        public string Source { get; set; }

        public string Method { get; set; }

        public string Container { get; set; }

        public string FileName { get; set; }

        public TestDetail() { }

    }

    public class TestResult : TableEntity
    {

        public double Result { get; set; }

        public TestResult(string testId, string testRunId, string platform, string testname, string method)
        {
            PartitionKey = testId + "-" + testRunId;
            RowKey = platform + "-" + testname + "-" + method;
        }
    }

}
