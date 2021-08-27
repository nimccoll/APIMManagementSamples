//===============================================================================
// Microsoft FastTrack for Azure
// APIM Management Samples
//===============================================================================
// Copyright © Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace MonitorAndDisableGateway
{
    public static class DisableGateway
    {

        [FunctionName("DisableGateway")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("DisableGateway function processed a request.");

            string responseMessage = string.Empty;

            if (req.Query.ContainsKey("disableGateway")
                && req.Query.ContainsKey("region"))
            {
                bool disableGateway = false;
                string region = string.Empty;
                disableGateway = Convert.ToBoolean(req.Query["disableGateway"]);
                region = req.Query["region"];
                string apimUrl = Environment.GetEnvironmentVariable("APIMUrl", EnvironmentVariableTarget.Process);
                string apiVersion = Environment.GetEnvironmentVariable("APIMAPIVersion", EnvironmentVariableTarget.Process);
                APIMGatewayService apimGatewayService = new APIMGatewayService(apimUrl, apiVersion, log);
                APIMGateway apimGateway = await apimGatewayService.UpdateAPIMGateway(disableGateway, region);

                if (apimGateway != null)
                {
                    responseMessage =
                        $"Region {region} of APIM Gateway {apimGateway.name} was successfully updated.";
                    log.LogInformation(responseMessage);
                    return new OkObjectResult(responseMessage);
                }
                else
                {
                    log.LogError($"Update of {region} of APIM Gateway {apimGateway.name} failed. Please see logs for further details.");
                    return new StatusCodeResult(500);
                }
            }
            else
            {
                responseMessage = "You must pass a disableGateway boolean value and a region value to this API.";
                log.LogError(responseMessage);
                return new BadRequestObjectResult(responseMessage);
            }
        }
    }
}
