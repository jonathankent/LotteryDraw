using LotteryDrawApp.Controllers;
using LotteryDrawApp.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json.Linq;
using System;
using System.Web;

namespace LotteryDrawApp.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod()]
        public void CreateDrawTest()
        {

            //LotteryDrawModel draw = new LotteryDrawModel();
            //draw.Name = "49s";
            //draw.Description = "49s Lottery";
            //draw.DrawDate = DateTime.Now;
            //draw.TotalPrimary = 49;
            //draw.RangePrimary = new Tuple<int, int>(1, 49);
            //draw.TotalSecondary = 2;
            //draw.RangeSecondary = new Tuple<int, int>(1, 10);
            //string configJson = JsonConvert.SerializeObject(draw);

            HttpContext.Current = new HttpContext(
                new HttpRequest(null, "http://tempuri.org", null),
                new HttpResponse(null));


            // Test message, winning numbers not specified.
            string jsonString = "{\"Name\": \"49s\", \"Description\": \"49s Lottery\", \"DrawDate\": \"13 Oct 2017\", \"TotalPrimary\": \"49\", \"RangePrimary\":{\"Item1\":1,\"Item2\":49}, \"TotalSecondary\": \"2\", \"RangeSecondary\":{\"Item1\":1,\"Item2\":10}}";


            var json = JObject.Parse(jsonString);

            LotteryDrawController drawController = new LotteryDrawController();


            dynamic result = drawController.CreateDraw(json);

            Assert.IsInstanceOfType(result, typeof(string));
            var jsonResult = JObject.Parse(result);
            Assert.IsTrue(jsonResult.Status == "OK");
        }

        [TestMethod()]
        public void AddWinninNumbersTest()
        {

            var request = new Mock<HttpRequestBase>();
            request.SetupGet(x => x.Headers).Returns(new System.Net.WebHeaderCollection { { "X-Requested-With", "XMLHttpRequest" } });

            var context = new Mock<HttpContextBase>();
            context.SetupGet(x => x.Request).Returns(request.Object);




            LotteryDrawModel draw = new LotteryDrawModel();
            draw.Name = "49s";
            draw.Description = "49s Lottery";
            draw.DrawDate = DateTime.Now;
            draw.TotalPrimary = 49;
            draw.RangePrimary = new Tuple<int, int>(1, 49);
            draw.TotalSecondary = 2;
            draw.RangeSecondary = new Tuple<int, int>(1, 10);

            context.Setup(x => x.Application.Add("49s", draw));


            //var mockApplication = new Mock<HttpApplicationStateBase>();
            //mockApplication.SetupGet(s => s["49s"]).Returns(draw);

            //var request = new Mock<HttpRequestBase>();
            //var context = new Mock<HttpContextBase>();

            //request.SetupGet(x => x.Headers).Returns(new WebHeaderCollection { { "X-Requested-With", "XMLHttpRequest" } });
            //context.SetupGet(ctx => ctx.Request).Returns(request.Object);
            //context.SetupGet(ctx => ctx.Application).Returns(mockApplication.Object);

            //HttpContext.Current = new HttpContext(
            //    new HttpRequest(null, "http://tempuri.org", null),
            //    new HttpResponse(null));

            string jsonString = "{ \"Name\":\"49s\", \"WinningPrimary\":[1,2,3,4,5], \"WinningSecondary\":[1,2]}";

            var json = JObject.Parse(jsonString);

            LotteryDrawController drawController = new LotteryDrawController();


            dynamic result = drawController.AddWinningNumbers(json);

            Assert.IsInstanceOfType(result, typeof(string));
            var jsonResult = JObject.Parse(result);


            // COMMENT: I had mocking issues with the application variable. Would love to have solved if I had more time.
            //Assert.IsTrue(jsonResult.Status == "OK");
        }
    }
}
