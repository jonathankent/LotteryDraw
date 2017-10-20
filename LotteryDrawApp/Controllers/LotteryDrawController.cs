using LotteryDrawApp.Models;
using LotteryDrawApp.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;

namespace LotteryDrawApp.Controllers
{
    public class LotteryDrawController : ApiController
    {
        #region "Constants"
        private const string routePath = "api/lotterydraw/";
        Logger Log;
        #endregion

        #region "Constructor"
        public LotteryDrawController()
        {
            Log = new Logger();
        }
        #endregion

        #region "Actions"

        [HttpPost]
        public string CreateDraw([FromBody]JObject json)
        {
            Log.WriteInformation($"CreateDraw {json}");

            JObject responseJson = new JObject();
            HttpStatusCode returnStatusCode = HttpStatusCode.BadRequest;
            string returnMessage = "Bad Request. The request could not be understood by the server. The exact error is unknown";

            try
            {
                Log.WriteInformation($"Saving Draw for json {json}");

                //Extract the draw from the request.
                JObject jsonRequestObject = json;
                LotteryDrawModel lotteryDraw = JsonConvert.DeserializeObject<LotteryDrawModel>(jsonRequestObject.ToString());

                if (lotteryDraw == null)
                {
                    Log.WriteWarning($"DeserializeObject<LotteryDrawModel> failed for json {json}");
                    returnMessage = "CreateDraw failure : DeserializeObject<LotteryDrawModel> failed.";
                }
                else
                {
                    // Persist the Draw (This would normally be persisted to database).
                    // Name is a unique identifer for the lottery so we don't need to include the date.
                    string drawIdentifier = lotteryDraw.Name;
                    System.Web.HttpContext.Current.Application[drawIdentifier] = lotteryDraw;
                    returnStatusCode = HttpStatusCode.OK;
                    returnMessage = $"Draw[{drawIdentifier}] saved";
                    Log.WriteInformation($"Draw[{drawIdentifier}] saved");
                }
                responseJson.Add("Status", returnStatusCode.ToString());
                responseJson.Add("Message", returnMessage);
            }
            catch (Exception ex)
            {
                Log.WriteException(ex);

                // Return error message in generic response object
                returnStatusCode = HttpStatusCode.InternalServerError;
                responseJson.Add("Status", returnStatusCode.ToString());
                responseJson.Add("Message", ex.Message);
            }

            return responseJson.ToString();
        }


        [HttpPost]
        [Route(routePath + "AddWinningNumbers")]
        public string AddWinningNumbers([FromBody]JObject json)
        {
            Log.WriteInformation($"AddWinningNumbers {json}");

            JObject responseJson = new JObject();
            HttpStatusCode returnStatusCode = HttpStatusCode.BadRequest;
            string returnMessage = "Bad Request. The request could not be understood by the server. The exact error is unknown";

            try
            {
                Log.WriteInformation($"Saving WinningNumbers for json {json}");

                //Extract the WinningNumbers from the request.
                JObject jsonRequestObject = json;
                LotteryDrawModel lotteryNumbers = JsonConvert.DeserializeObject<LotteryDrawModel>(jsonRequestObject.ToString());

                if (lotteryNumbers == null)
                {
                    Log.WriteWarning($"DeserializeObject<lotteryNumbersModel> failed for json {json}");
                    returnMessage = "AddWinningNumbers failure : DeserializeObject<lotteryNumbersModel> failed.";
                }
                else
                {
                    // Extract the draw details.
                    string drawIdentifier = lotteryNumbers.Name;
                    Tuple<int, int, int, int, int> winningPrimary = lotteryNumbers.WinningPrimary;
                    Tuple<int, int> winningSecondary = lotteryNumbers.WinningSecondary;


                    // Get the persisted draw.           
                    LotteryDrawModel draw = System.Web.HttpContext.Current.Application[drawIdentifier] as LotteryDrawModel;
                    if (draw == null)
                    {
                        throw new ArgumentException($"AddWinningNumbers failure for Draw[{drawIdentifier}] : Draw[{drawIdentifier}] not found.");
                    }

                    // Add draw data.
                    lotteryNumbers.Description = draw.Description;
                    lotteryNumbers.DrawDate = draw.DrawDate;
                    lotteryNumbers.TotalPrimary = draw.TotalPrimary;
                    lotteryNumbers.RangePrimary = draw.RangePrimary;
                    lotteryNumbers.TotalSecondary = draw.TotalSecondary;
                    lotteryNumbers.RangeSecondary = draw.RangeSecondary;

                    // Validate the Range.
                    Tuple<int, int> rangePrimary = draw.RangePrimary;
                    Tuple<int, int> rangeSecondary = draw.RangeSecondary;

                    int minPrimaryRange = rangePrimary.Item1;
                    int maxPrimaryRange = rangePrimary.Item2;

                    if (minPrimaryRange > maxPrimaryRange)
                    {
                        int swap = minPrimaryRange;
                        minPrimaryRange = maxPrimaryRange;
                        maxPrimaryRange = swap;
                    }

                    int minSecondaryRange = rangeSecondary.Item1;
                    int maxSecondaryRange = rangeSecondary.Item2;

                    if (minSecondaryRange > maxSecondaryRange)
                    {
                        int swap = minSecondaryRange;
                        minSecondaryRange = maxSecondaryRange;
                        maxSecondaryRange = swap;
                    }

                    // Validate the WinningNumbers.
                    int[] arrWinningPrimary = new int[] { winningPrimary.Item1, winningPrimary.Item2, winningPrimary.Item3, winningPrimary.Item4, winningPrimary.Item5 };
                    foreach (int n in arrWinningPrimary)
                    {
                        if (n < minPrimaryRange || n > maxPrimaryRange)
                        {
                            throw new ArgumentOutOfRangeException($"AddWinningNumbers failure for Draw[{drawIdentifier}] : WinningPrimary[{n}] out of range.");
                        }
                    }

                    int[] arrWinningSecondary = new int[] { winningSecondary.Item1, winningSecondary.Item2 };
                    foreach (int n in arrWinningSecondary)
                    {
                        if (n < minSecondaryRange || n > maxSecondaryRange)
                        {
                            throw new ArgumentOutOfRangeException($"AddWinningNumbers failure for Draw[{drawIdentifier}] : WinningSecondary[{n}] out of range.");
                        }
                    }

                    // Persist WinningNumbers.
                    System.Web.HttpContext.Current.Application[drawIdentifier] = lotteryNumbers;

                    returnStatusCode = HttpStatusCode.OK;
                    returnMessage = $"Draw[{lotteryNumbers.Name}] saved with winning numbers";
                    Log.WriteInformation($"Draw[{drawIdentifier}] saved with winning numbers");
                }
                responseJson.Add("Status", returnStatusCode.ToString());
                responseJson.Add("Message", returnMessage);
            }
            catch (Exception ex)
            {
                Log.WriteException(ex);

                // Return error message in generic response object
                returnStatusCode = HttpStatusCode.InternalServerError;
                responseJson.Add("Status", returnStatusCode.ToString());
                responseJson.Add("Message", ex.Message);
            }

            return responseJson.ToString();
        }

        [HttpPost]
        [Route(routePath + "SearchLotter")]
        public string SearchLottery([FromBody]JObject json)
        {
            Log.WriteInformation($"SearchLottery {json}");

            JObject responseJson = new JObject();
            HttpStatusCode returnStatusCode = HttpStatusCode.BadRequest;
            string returnMessage = "Bad Request. The request could not be understood by the server. The exact error is unknown";

            try
            {
                Log.WriteInformation($"Searching lottery for json {json}");

                //Extract the draw from the request.
                JObject jsonRequestObject = json;
                LotteryDrawModel lotteryDraw = JsonConvert.DeserializeObject<LotteryDrawModel>(jsonRequestObject.ToString());

                if (lotteryDraw == null)
                {
                    Log.WriteWarning($"DeserializeObject<LotteryDrawModel> failed for json {json}");
                    returnMessage = "SearchLottery failure : DeserializeObject<LotteryDrawModel> failed.";
                }
                else
                {

                    // Extract the date.
                    DateTime drawDate = lotteryDraw.DrawDate;

                    List<LotteryDrawModel> lotteryDraws = new List<LotteryDrawModel>();

                    // Searching for draws by date. (This would normally be done with a database).
                    foreach (string key in System.Web.HttpContext.Current.Application.Keys)
                    {
                        if (System.Web.HttpContext.Current.Application[key] != null)
                        {
                            LotteryDrawModel draw = System.Web.HttpContext.Current.Application[key] as LotteryDrawModel;
                            lotteryDraws.Add(draw);
                        }
                    }

                    // Method could return the list to UI if I had more time. 

                    returnStatusCode = HttpStatusCode.OK;
                    returnMessage = $"{lotteryDraws.Count} found";
                    Log.WriteInformation($"{lotteryDraws.Count} found");
                }
                responseJson.Add("Status", returnStatusCode.ToString());
                responseJson.Add("Message", returnMessage);
            }
            catch (Exception ex)
            {
                Log.WriteException(ex);

                // Return error message in generic response object
                returnStatusCode = HttpStatusCode.InternalServerError;
                responseJson.Add("Status", returnStatusCode.ToString());
                responseJson.Add("Message", ex.Message);
            }

            return responseJson.ToString();
        }
        #endregion
    }
}
