//How to use Service Bus Queue under different scanrios: http://msdn.microsoft.com/en-us/library/hh528527.aspx
//Basic: http://azure.microsoft.com/en-us/documentation/articles/service-bus-dotnet-how-to-use-queues/
//Notice: Namespace & ServiceBus have different connection Strings

//hanwan Azure Service Bus Performance Comparing with AWS SNS to check the performance difference
//AWS secret key cannot be put on public website
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace AzurePerformance
{
    public class ServiceBusOperations
    {
        private static Stopwatch Watch = new Stopwatch();
        const String conString =
                "Endpoint=sb://cloudcomputing-wus.servicebus.windows.net/;SharedSecretIssuer=owner;SharedSecretValue=S5iCmZn8BHdzfvQedY+fPAiMEa4rgkI6QupzCmT11zc=";

        private static NamespaceManager namespaceManager = NamespaceManager.CreateFromConnectionString(conString);
        //write it into configuration file in future
        const int NumberOfMessages = 20;

        private static void CreateTestQueue(string name)
        {
            QueueDescription qd = new QueueDescription(name);
            qd.MaxSizeInMegabytes = 1024;
            qd.DefaultMessageTimeToLive = new TimeSpan(0, 1, 0);

            if (!namespaceManager.QueueExists(name))
                namespaceManager.CreateQueue(qd);

        }

        private static void DeleteTestQueue(string name)
        {

            if(namespaceManager.QueueExists(name))
                namespaceManager.DeleteQueue(name);

        }


        public static double SyncSendReceiveMsg()
        {
            string sbName = "sb.sendreceivemsg."+ new Random().Next(0,100);
            Console.WriteLine("Azure SyncSendReceiveMsg, Connecting to "+ sbName + " to start the test...");
            
            CreateTestQueue(sbName);

            QueueClient qClient = QueueClient.CreateFromConnectionString(conString, sbName);
            double send = SyncSend(qClient);
            double receive = SyncReceive(qClient);
            
            DeleteTestQueue(sbName);

            return send + receive;

        }

        public static double AsyncSendReceiveMsg()
        {
            string sbName = "sb.sendreceivemsg." + new Random().Next(0, 100);
            Console.WriteLine("Azure Service Bus ASyncSendReceiveMsg, Connecting to " + sbName + " to start the test...");

            CreateTestQueue(sbName);

            QueueClient qClient = QueueClient.CreateFromConnectionString(conString, sbName);
            double send = AsyncSend(qClient);
            double receive = AsyncReceive(qClient);

            DeleteTestQueue(sbName);

            return send + receive;

        }

        public static double BatchAsyncSendReceiveMsg()
        {
            string sbName = "sb.sendreceivemsg." + new Random().Next(0, 100);
            Console.WriteLine("Azure Service Bus BatchAsyncSendReceiveMsg, Connecting to " + sbName + " to start the test...");

            CreateTestQueue(sbName);

            QueueClient qClient = QueueClient.CreateFromConnectionString(conString, sbName);
            double send = BatchAsyncSend(qClient);
            double receive = BatchAsyncReceive(qClient);

            DeleteTestQueue(sbName);

            return send + receive;

        }

        private static double SyncSend(QueueClient qClient)
        {
            StartWatch("Azure SyncSending ");

            for (int n = 0; n < NumberOfMessages; n++)
            {
                qClient.Send(new BrokeredMessage("quick brown fox jumps over the lazy dog" + n));
            }

            return StopWatch();

        }

        private static double AsyncSend(QueueClient qClient)
        {
            StartWatch("Azure AsyncSending ");

            List<Task> allTasks = new List<Task>();

            for (int n = 0; n < NumberOfMessages; n++)
            {
                allTasks.Add(qClient.SendAsync(new BrokeredMessage("quick brown fox jumps over the lazy dog" + n)));
            }
            Task.WaitAll(allTasks.ToArray());

            return StopWatch();
        }

        private static double BatchAsyncSend(QueueClient qClient)
        {
            StartWatch("Azure BatchAsyncSend ");

            List<BrokeredMessage> allMessages = new List<BrokeredMessage>();

            for (int n = 0; n < NumberOfMessages; n++)
            {
                allMessages.Add(new BrokeredMessage("quick brown fox jumps over the lazy dog" + n));
            }
            qClient.SendBatchAsync(allMessages).Wait();

            return StopWatch();

        }

        private static double SyncReceive(QueueClient qClient)
        {
            StartWatch("Azure SyncReceiving ");

            for (int n = 0; n < NumberOfMessages; n++)
            {
                var message = qClient.Receive();
                message.Complete();
            }

            return StopWatch();

        }

        private static double AsyncReceive(QueueClient qClient)
        {
            StartWatch("Azure AsyncReceiving ");

            List<Task> allTasks = new List<Task>();

            for (int n = 0; n < NumberOfMessages; n++)
            {
                allTasks.Add(qClient.ReceiveAsync().ContinueWith((m) => { m.Result.Complete(); }));
            }
            Task.WaitAll(allTasks.ToArray());

            return StopWatch();
        }

        private static double BatchAsyncReceive(QueueClient qClient)
        {

            StartWatch("Azure BatchReceiving ");
            int n = 0;
            while (n < NumberOfMessages)
            {
                Task<IEnumerable<BrokeredMessage>> allMessages = qClient.ReceiveBatchAsync(NumberOfMessages);
                allMessages.Wait();
                List<Guid> allComplete = new List<Guid>();

                foreach (var msg in allMessages.Result)
                {
                    allComplete.Add(msg.LockToken);
                    n++;
                }

                qClient.CompleteBatchAsync(allComplete).Wait();
            }


            return StopWatch();

        }

        private static void StartWatch(String methodName)
        {

            Watch.Reset();
            Watch.Start();
            Console.WriteLine(methodName + NumberOfMessages + " message " + "...");
        }

        private static double StopWatch()
        {

            Watch.Stop();
            double total = Watch.ElapsedMilliseconds;
            Console.WriteLine("Time elapsed: " + total + " Milliseconds");
            return total;
        }

    }

}

