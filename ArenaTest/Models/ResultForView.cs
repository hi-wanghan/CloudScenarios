using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

namespace ArenaTest.Models
{

    public  class ResultForView
    {
        public  string PKey;

        public  string TestName { get; set; }

        public string AllTdName { get; set; }

        public string AllValue { get; set; }


        public  Dictionary<string, double> Results { get; set; }

        public  ResultForView(string value)
        {
            PKey = value;

            TestName = "ServiceBusPerformance";

            if (value.StartsWith("7"))
            {
                TestName = "BatchSendMsg";
            }else if(value.StartsWith("1007")){
                TestName = "StorageComparison";
            }

            Results = new Dictionary<string, double>();

        }

        public  ResultForView()
        {
            TestName = "ServiceBusPerformance";
            Results = new Dictionary<string, double>();
        }

        public void ComposeTableData()
        {
            char[] delimiterChars = { '-', ',', '.', ':', '\t' };

            StringBuilder sb = new StringBuilder();

            string Firstpart = Results.Keys.First().Split(delimiterChars)[0];
            string LastPart = Results.Keys.Last().Split(delimiterChars)[0];

            //Method comparison on the same platform
            if(Firstpart.Equals(LastPart)){

                foreach(string key in Results.Keys)
                    sb.Append(key.Split(delimiterChars)[2]).Append('-');

            }else{ //Platform comparison

                foreach (string key in Results.Keys)
                    sb.Append(key.Split(delimiterChars)[0]).Append('-');
            }

            AllTdName = sb.ToString();

            foreach (double value in Results.Values)
            {
                AllValue += value.ToString() + '-';
            }

        }

        //public static string ToJson(this Object obj)
        //{
        //    return new JavaScriptSerializer().Serialize(obj);
        //}


    }
}