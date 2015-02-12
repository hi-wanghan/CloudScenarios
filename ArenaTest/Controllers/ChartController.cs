//http://weblogs.asp.net/imranbaloch/archive/2011/03/21/chart-helper-in-asp-net-mvc-3-0-with-transparent-background.aspx

using ArenaTest.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace ArenaTest.Controllers
{
    public class ChartController : Controller
    {

        //[HttpPost]   can't invoke the method by <img src> if it's marked as HttpPost
        //public ActionResult DrawChart(string rvString)
        public ActionResult DrawChart(string chartKey, string name, string allTdName, string allValue)
        {
            Type type = new ResultForView().GetType();

            var cachedChart = Chart.GetFromCache(key: chartKey);

            if (cachedChart == null)
            {
                cachedChart = new Chart(600, 400, theme: ChartTheme.Vanilla);
                cachedChart.AddTitle(name + "  "  + DateTime.Now);
                cachedChart.AddSeries(
                name: "Cloud Arena",
                axisLabel: "axisLabel",
                xValue: allTdName.Split('-'),
                yValues: allValue.Split('-'), yFields: "ms",
                legend:""
                );

                cachedChart.SaveToCache(key: chartKey,
                minutesToCache: 2,
                slidingExpiration: false);
            }
            Chart.WriteFromCache(chartKey);

            var bytes = cachedChart.GetBytes("png");

            return File(bytes, "image/png");
        }
    }
}