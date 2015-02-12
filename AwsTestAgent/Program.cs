/*
 * This agent should be put on a Windows Azure VM 
 * 
 * 
 *
 **/
//AWS Agent, same structure with other Agents 
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure;
using System;

namespace CloudArenaTestAgent
{
    class Program
    {
        static string ConnectionString = CloudConfigurationManager.GetSetting("cloudcomputing-wus");
        static string TopicName = CloudConfigurationManager.GetSetting("topicName");
        static string SubscriptionName = CloudConfigurationManager.GetSetting("subscriptionName");
        static string FilterString = CloudConfigurationManager.GetSetting("filterString");
        static string TargetPlatform = CloudConfigurationManager.GetSetting("TargetPlatform");

        public static void Main(string[] args)
        {
            try
            {


                //Console.WriteLine("Are you ready ? ");
                //Console.ReadLine();

                //hanwan here should be my service bus resource
                //Put filter here in future so the agent only accept Azure related test
                CreateSubscription(ConnectionString, TopicName, SubscriptionName, FilterString);
                //this subscription contains a filter already
                ReceiveMessages(ConnectionString, TopicName, SubscriptionName);
                
               }
               catch (Exception ex)
               {
                   Console.WriteLine("exception is " + ex.ToString());
                   Console.ReadLine();
                   // Exception handling here.
               }
        }

        //hanwan  in future we can locked down the subscription filter according to where the agent sits on
        //figure out how to configure it dynamically
        public static void CreateSubscription(string connectionString, string topicName, string subscriptionName,string filterString)
        {

            var namespaceManager =
                NamespaceManager.CreateFromConnectionString(connectionString);
            var Filter = new SqlFilter(filterString);

            if (!namespaceManager.SubscriptionExists(topicName, subscriptionName))
            {
                namespaceManager.CreateSubscription(topicName, subscriptionName, Filter);
            }

        }

        //hanwan what shall the method do after finishing test? just return/write test result
        //has same topicName/subscription name with the message sender, so it can receive the msg
        public static void ReceiveMessages(string connectionString, string topicName, string subscriptionName)
        {

            SubscriptionClient Client =
                SubscriptionClient.CreateFromConnectionString
                        (connectionString, topicName, subscriptionName);

            Console.WriteLine("dude, you will use Han's table storage to store Your Test Result ");

            TableStorageClient tsc = new TableStorageClient();


            // Continuously process messages received from the HighMessages subscription 
            while (true)
            {
                Console.WriteLine("Listening to the new message, topic is " + topicName + " SubscriptionName is " + subscriptionName);
                
                BrokeredMessage message = Client.Receive();

                if (message != null)
                {
                    try
                    {

                        Console.WriteLine("Body: " + message.GetBody<string>());
                        Console.WriteLine("MessageID: " + message.MessageId);
                        string TestId = message.Properties["TestId"].ToString();
                        string TestRunId = message.Properties["TestRunId"].ToString();
                        string TestName = message.Properties["TestName"].ToString();

                        Console.WriteLine("TestId is " + TestId + ", TestRunId " + TestRunId );

                        TestDetail td = tsc.IdentifyTestDetail(TestId);

                        Console.WriteLine("baby, let's rock the TestDetail " + td.Method + td.Source + td.Type);

                        //download the resource
                        tsc.DownloadTestResource(td);

                        // Remove message from subscription
                        message.Complete();

                        //get All methods in the Test
                        string[] AllMethods = td.Method.Split(';');
                        foreach(string method in AllMethods)
                        {
                            TestResult tr = new TestResult(TestId, TestRunId, TargetPlatform, TestName, method);

                            //"execute" the method of the Test
                            tr.Result = tsc.ExecuteTestMethod(td.Type, method);
                            //hanwan write it to the table storage
                            tsc.WriteTestResult(tr);
                        }

                    }
                    catch (Exception e)
                    {
                        // Indicate a problem, unlock message in subscription
                        Console.WriteLine(e.ToString());
                        message.Abandon();
                    }
                }
                Console.WriteLine("dude, I'm off work now. bye-bye!");

            } 
        }



    }
}
