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
using Azure.Core;
using Azure.Identity;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MonitorAndDisableGateway
{
    public class APIMGatewayService
    {
        private string _apimUrl = string.Empty;
        private string _apiVersion = string.Empty;
        private ILogger _log;
        private HttpClient _httpClient;
        private string _accessToken = string.Empty;

        private APIMGatewayService()
        {

        }

        public APIMGatewayService(string apimUrl, string apiVersion, ILogger log)
        {
            if (string.IsNullOrEmpty(apimUrl)) throw new ArgumentNullException("apimUrl");

            _apimUrl = apimUrl;
            _apiVersion = apiVersion;
            _log = log;
            _httpClient = new HttpClient();
        }

        public async Task<APIMGateway> GetAPIMGateway()
        {
            if (string.IsNullOrEmpty(_accessToken)) _accessToken = await GenerateAccessToken();

            APIMGateway apimGateway = null;

            // Retrieve APIM gateway details using the Azure Management API
            string url = $"{_apimUrl}?api-version={_apiVersion}";
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _accessToken);
            HttpResponseMessage response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                apimGateway = JsonConvert.DeserializeObject<APIMGateway>(responseContent);
            }
            else
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                _log.LogError(responseContent);
            }

            return apimGateway;
        }

        public async Task<APIMGateway> UpdateAPIMGateway(bool disableGateway, string region)
        {
            APIMGateway updatedAPIMGateway = null;
            if (string.IsNullOrEmpty(_accessToken)) _accessToken = await GenerateAccessToken();

            APIMGateway apimGateway = await GetAPIMGateway();
            if (apimGateway != null)
            {
                bool updateGateway = false;
                // Determine if we are updating the primary region or an additional region
                if (DisableGateway(region, apimGateway.properties.gatewayRegionalUrl))
                {
                    apimGateway.properties.disableGateway = disableGateway;
                    updateGateway = true;
                }
                else
                {
                    foreach (AdditionalLocation additionalLocation in apimGateway.properties.additionalLocations)
                    {
                        if (DisableGateway(region, additionalLocation.gatewayRegionalUrl))
                        {
                            additionalLocation.disableGateway = disableGateway;
                            updateGateway = true;
                            break;
                        }
                    }
                }
                if (updateGateway)
                {
                    // Enable / disable the specified regional gateway using the Azure Management API
                    string url = $"{_apimUrl}?api-version={_apiVersion}";
                    _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _accessToken);
                    string updateGatewayJSON = JsonConvert.SerializeObject(apimGateway);
                    HttpResponseMessage response = await _httpClient.PutAsync(url, new StringContent(updateGatewayJSON, Encoding.UTF8, "application/json"));
                    if (response.IsSuccessStatusCode)
                    {
                        string responseContent = await response.Content.ReadAsStringAsync();
                        updatedAPIMGateway = JsonConvert.DeserializeObject<APIMGateway>(responseContent);
                    }
                    else
                    {
                        string responseContent = await response.Content.ReadAsStringAsync();
                        _log.LogError(responseContent);
                    }
                }
            }

            return updatedAPIMGateway;
        }

        private bool DisableGateway(string region, string gatewayRegionalUrl)
        {
            bool disableGateway = false;

            // Is this the gateway we want to update?
            if (!string.IsNullOrEmpty(gatewayRegionalUrl))
            {
                region = $"-{region.ToLower()}-";
                Uri uri = new Uri(gatewayRegionalUrl);
                if (uri.Host.ToLower().Contains(region)) disableGateway = true;
            }

            return disableGateway;
        }

        private async Task<string> GenerateAccessToken()
        {
            // Obtain an access token for the Azure Management API using Managed Identity credentials
            string accessToken = string.Empty;

            DefaultAzureCredential credential = new DefaultAzureCredential();
            TokenRequestContext tokenRequestContext = new TokenRequestContext(new string[]
            {
                "https://management.azure.com"
            });
            AccessToken accessTokenResult = await credential.GetTokenAsync(tokenRequestContext);
            accessToken = accessTokenResult.Token;

            return accessToken;
        }
    }
}
