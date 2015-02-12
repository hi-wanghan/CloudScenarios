using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Amazon;
using Amazon.SQS.Model;
using Amazon.SQS;
//AWS SNS performance Test
namespace AwsPerformance
{
    class SqsOperations
    {
        const int NumberOfMessages = 20;
        const int MaxMessageInBatch = 10;
        static int NumberOfBatch = NumberOfMessages/10;
        private static string myQueueUrl; 
        private static Stopwatch Watch = new Stopwatch();

        public static double SendReceiveMsg()
        {
            double send = SyncSendMsg();
            double receive = ReceiveMsg();
            
            return send + receive;

        }

        public static double BatchSendMsg()
        {

            var sqs = AWSClientFactory.CreateAmazonSQSClient();
            var sqsRequest = new CreateQueueRequest { QueueName = "SoloQueue" + new Random().Next(0, 1000) };
            var createQueueResponse = sqs.CreateQueue(sqsRequest);
            myQueueUrl = createQueueResponse.QueueUrl;

            //Sending a message
            Console.WriteLine("BatchSending 200 message to AWS SQS service.\n");
            List<SendMessageBatchResult> results = new List<SendMessageBatchResult>();

            StartWatch("AWSAwsBatchSendMsg \n");

            int index = 0;

            for (int i = 1; i <= NumberOfBatch; i++)
            {

                List<SendMessageBatchRequestEntry> messages = new List<SendMessageBatchRequestEntry>();

                for (int n = 1; n <= MaxMessageInBatch; n++)
                {
                    index = n + (i - 1) * MaxMessageInBatch;
                    var sendMessageRequest = new SendMessageBatchRequestEntry
                    {
                        Id = n.ToString(),
                        MessageBody = "quick brown fox jumps over the lazy dog" + index
                    };
                    messages.Add(sendMessageRequest);

                    Console.WriteLine("adding message " + index);
                }
                Console.WriteLine("sending message " + index);

                SendMessageBatchResult result = sqs.SendMessageBatch(new SendMessageBatchRequest { QueueUrl = myQueueUrl, Entries = messages });

                if (result.Failed.Count > 0)
                {
                    Console.WriteLine("Google Cloud Failed to send all message ");
                    return 999999;
                }

                results.Add(result);
            }

            double elapse = StopWatch();

            sqs.DeleteQueue(new DeleteQueueRequest { QueueUrl = myQueueUrl });

            return elapse;
        }

        public static double BatchSendReceiveMsg()
        {

            var sqs = AWSClientFactory.CreateAmazonSQSClient();
            var sqsRequest = new CreateQueueRequest { QueueName = "SoloQueue" + new Random().Next(0, 1000) };
            var createQueueResponse = sqs.CreateQueue(sqsRequest);
            myQueueUrl = createQueueResponse.QueueUrl;

            //Sending a message
            Console.WriteLine("BatchSending 200 message to AWS SQS service.\n");
            List<SendMessageBatchResult> results = new List<SendMessageBatchResult>();

            StartWatch("AWSAwsBatchSendMsg \n");

            int index = 0;

            for (int i = 1; i <= NumberOfBatch; i++)
            {

                List<SendMessageBatchRequestEntry> messages = new List<SendMessageBatchRequestEntry>();

                for (int n = 1; n <= MaxMessageInBatch; n++)
                {
                    index = n + (i - 1) * MaxMessageInBatch;
                    var sendMessageRequest = new SendMessageBatchRequestEntry
                    {
                        Id = n.ToString(),
                        MessageBody = "quick brown fox jumps over the lazy dog" + index
                    };
                    messages.Add(sendMessageRequest);

                    Console.WriteLine("adding message " + sendMessageRequest.MessageBody);
                }
                Console.WriteLine("sending  " + index);

                SendMessageBatchResult result = sqs.SendMessageBatch(new SendMessageBatchRequest { QueueUrl = myQueueUrl, Entries = messages });

                if (result.Failed.Count > 0)
                {
                    Console.WriteLine("Google Cloud Failed to send all message ");
                    return 999999;
                }

                results.Add(result);
            }

            //receive msg
            ReceiveMessageRequest receiveMessageRequest = new ReceiveMessageRequest();
            receiveMessageRequest.MaxNumberOfMessages = 10;
            receiveMessageRequest.QueueUrl = myQueueUrl;

            ReceiveMessageResponse receiveMessageResponse = sqs.ReceiveMessage(receiveMessageRequest);
            int count = receiveMessageResponse.Messages.Count;
            for (int n = 0; n < count; n++)
            {
                Console.WriteLine("received message " + receiveMessageResponse.Messages.ElementAt(n).Body.ToString());
            }
            Console.WriteLine("first batch count is " + count);
            
            receiveMessageResponse = sqs.ReceiveMessage(receiveMessageRequest);

            count = receiveMessageResponse.Messages.Count;
            Console.WriteLine("second batch count is " + count);
            for (int n = 0; n < count; n++)
            {
                Console.WriteLine("received message " + receiveMessageResponse.Messages.ElementAt(n).Body.ToString());
            }

            double elapse = StopWatch();


            sqs.DeleteQueue(new DeleteQueueRequest { QueueUrl = myQueueUrl });

            return elapse;
        }

        public static double SyncSendMsg()
        {
            var sqs = AWSClientFactory.CreateAmazonSQSClient();
            var sqsRequest = new CreateQueueRequest { QueueName = "SoloQueue" + new Random().Next(0, 1000) };
            var createQueueResponse = sqs.CreateQueue(sqsRequest);
            myQueueUrl = createQueueResponse.QueueUrl;

            //Sending a message
            Console.WriteLine("Sending 200 message to MyQueue.\n");

            StartWatch("AwsSqsSendMsg\n");

            for (int n = 0; n < NumberOfMessages; n++)
            {
                var sendMessageRequest = new SendMessageRequest
                {
                    QueueUrl = myQueueUrl, //URL from initial queue creation
                    MessageBody = "quick brown fox jumps over the lazy dog" + n
                };
                sqs.SendMessage(sendMessageRequest);
            }

            return StopWatch();
        }

        public static double ReceiveMsg()
        {
            var sqs = AWSClientFactory.CreateAmazonSQSClient();

            var receiveMessageRequest = new ReceiveMessageRequest { QueueUrl = myQueueUrl };

            StartWatch("AwsSqsReceiveMsg");
            for (int n = 0; n < NumberOfMessages; n++)
            {
                var receiveMessageResponse = sqs.ReceiveMessage(receiveMessageRequest);
                var messageRecieptHandle = receiveMessageResponse.Messages[0].ReceiptHandle;
                //Deleting a message
                Console.WriteLine("receiving message " + n);
                Console.WriteLine("Deleting the message with index " + n);
                var deleteRequest = new DeleteMessageRequest { QueueUrl = myQueueUrl, ReceiptHandle = messageRecieptHandle };
                sqs.DeleteMessage(deleteRequest);
            }

            return StopWatch();

            //if (receiveMessageResponse.Messages != null)
            //{
            //    Console.WriteLine("Printing received message.\n");
            //    foreach (var message in receiveMessageResponse.Messages)
            //    {
            //        Console.WriteLine("  Message");
            //        if (!string.IsNullOrEmpty(message.MessageId))
            //        {
            //            Console.WriteLine("    MessageId: {0}", message.MessageId);
            //        }
            //        if (!string.IsNullOrEmpty(message.ReceiptHandle))
            //        {
            //            Console.WriteLine("    ReceiptHandle: {0}", message.ReceiptHandle);
            //        }
            //        if (!string.IsNullOrEmpty(message.MD5OfBody))
            //        {
            //            Console.WriteLine("    MD5OfBody: {0}", message.MD5OfBody);
            //        }
            //        if (!string.IsNullOrEmpty(message.Body))
            //        {
            //            Console.WriteLine("    Body: {0}", message.Body);
            //        }

            //        foreach (string attributeKey in message.Attributes.Keys)
            //        {
            //            Console.WriteLine("  Attribute");
            //            Console.WriteLine("    Name: {0}", attributeKey);
            //            var value = message.Attributes[attributeKey];
            //            Console.WriteLine("    Value: {0}", string.IsNullOrEmpty(value) ? "(no value)" : value);
            //        }
            //    }
            //}

            //var messageRecieptHandle = receiveMessageResponse.Messages[0].ReceiptHandle;
            ////Deleting a message
            //Console.WriteLine("Deleting the message.\n");
            //var deleteRequest = new DeleteMessageRequest { QueueUrl = myQueueUrl, ReceiptHandle = messageRecieptHandle };
            //sqs.DeleteMessage(deleteRequest);

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
