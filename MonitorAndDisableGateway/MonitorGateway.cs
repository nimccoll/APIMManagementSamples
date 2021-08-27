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
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace MonitorAndDisableGateway
{
    public static class MonitorGateway
    {
        [FunctionName("MonitorGateway")]
        public static async void Run([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            List<string> gatewaysToDisable = new List<string>();
            string apimUrl = Environment.GetEnvironmentVariable("APIMUrl", EnvironmentVariableTarget.Process);
            string apiVersion = Environment.GetEnvironmentVariable("APIMAPIVersion", EnvironmentVariableTarget.Process);
            APIMGatewayService apimGatewayService = new APIMGatewayService(apimUrl, apiVersion, log);
            APIMGateway apimGateway = await apimGatewayService.GetAPIMGateway();
            if (apimGateway != null)
            {
                // Call the health check endpoint on each regional gateway to verify its health
                string subscriptionKey = Environment.GetEnvironmentVariable("SubscriptionKey", EnvironmentVariableTarget.Process);
                string healthCheckEndpoint = Environment.GetEnvironmentVariable("HealthCheckEndpoint", EnvironmentVariableTarget.Process);
                if (!string.IsNullOrEmpty(apimGateway.properties.gatewayRegionalUrl)
                    && !apimGateway.properties.disableGateway)
                {
                    bool isGatewayHealthy = await CheckGatewayHealth(healthCheckEndpoint, subscriptionKey, apimGateway.properties.gatewayRegionalUrl);
                    if (!isGatewayHealthy) gatewaysToDisable.Add(apimGateway.properties.gatewayRegionalUrl);
                    log.LogInformation($"Regional Gateway {apimGateway.properties.gatewayRegionalUrl} is healthy = {isGatewayHealthy}");
                }
                else
                {
                    log.LogInformation($"Regional Gateway {apimGateway.properties.gatewayRegionalUrl} is currently disabled.");
                }
                if (apimGateway.properties.additionalLocations != null
                    && apimGateway.properties.additionalLocations.Count > 0)
                {
                    foreach (AdditionalLocation additionalLocation in apimGateway.properties.additionalLocations)
                    {
                        if (!string.IsNullOrEmpty(additionalLocation.gatewayRegionalUrl)
                            && !additionalLocation.disableGateway)
                        {
                            bool isGatewayHealthy = await CheckGatewayHealth(healthCheckEndpoint, subscriptionKey, additionalLocation.gatewayRegionalUrl);
                            if (!isGatewayHealthy) gatewaysToDisable.Add(additionalLocation.gatewayRegionalUrl);
                            log.LogInformation($"Regional Gateway {additionalLocation.gatewayRegionalUrl} is healthy = {isGatewayHealthy}");
                        }
                        else
                        {
                            log.LogInformation($"Regional Gateway {additionalLocation.gatewayRegionalUrl} is currently disabled.");
                        }

                    }
                }
                if (gatewaysToDisable.Count > 0)
                {
                    // If any regional gateways are unhealthy, disable them
                    foreach (string gatewayRegionalUrl in gatewaysToDisable)
                    {
                        Uri uri = new Uri(gatewayRegionalUrl);
                        string[] hostParts = uri.Host.Split(".");
                        string[] serviceNameParts = hostParts[0].Split("-");
                        string region = serviceNameParts[serviceNameParts.Length - 2];
                        bool isGatewayDisabled = await DisableGateway(region);
                        if (isGatewayDisabled)
                        {
                            log.LogInformation($"Region {region} of APIM Gateway {apimGateway.name} has been disabled.");
                        }
                        else
                        {
                            log.LogError($"Failed to disable region {region} of APIM Gateway {apimGateway.name}. See logs for further details.");
                        }
                    }
                }
            }
        }

        private static async Task<bool> CheckGatewayHealth(string healthCheckEndpoint, string subscriptionKey, string gatewayUrl)
        {
            bool isGatewayHealthy = false;
            HttpClient httpClient = new HttpClient();

            for (int i = 0; i < 5; i++)
            {
                string url = $"{gatewayUrl}{healthCheckEndpoint}?subscription-key={subscriptionKey}";
                HttpResponseMessage response = await httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    isGatewayHealthy = true;
                    break;
                }
                else
                {
                    int delay = (i + 1) * 1000;
                    await Task.Delay(delay);
                }
            }

            return isGatewayHealthy;
        }

        private static async Task<bool> DisableGateway(string region)
        {
            string disableGatewayUrl = Environment.GetEnvironmentVariable("DisableGatewayUrl", EnvironmentVariableTarget.Process);
            string disableGatewayKey = Environment.GetEnvironmentVariable("DisableGatewayKey", EnvironmentVariableTarget.Process);
            string url = $"{disableGatewayUrl}?disableGateway=true&region={region}";
            if (!string.IsNullOrEmpty(disableGatewayKey))
            {
                url = $"{url}&code={disableGatewayKey}";
            }
            HttpClient httpClient = new HttpClient();
            HttpResponseMessage response = await httpClient.GetAsync(url);
            return response.IsSuccessStatusCode;
        }
    }
}
