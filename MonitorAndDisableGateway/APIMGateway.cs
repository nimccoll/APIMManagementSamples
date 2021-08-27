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
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

public class Tags
{
}

public class HostnameConfiguration
{
    public string type { get; set; }
    public string hostName { get; set; }
    public object encodedCertificate { get; set; }
    public object keyVaultId { get; set; }
    public object certificatePassword { get; set; }
    public bool negotiateClientCertificate { get; set; }
    public object certificate { get; set; }
    public bool defaultSslBinding { get; set; }
}

public class Sku
{
    public string name { get; set; }
    public int capacity { get; set; }
}

public class AdditionalLocation
{
    public string location { get; set; }
    public Sku sku { get; set; }
    public List<string> publicIPAddresses { get; set; }
    public object privateIPAddresses { get; set; }
    public object virtualNetworkConfiguration { get; set; }
    public string gatewayRegionalUrl { get; set; }
    public bool disableGateway { get; set; }
}

public class CustomProperties
{
    [JsonProperty("Microsoft.WindowsAzure.ApiManagement.Gateway.Security.Protocols.Tls10")]
    public string MicrosoftWindowsAzureApiManagementGatewaySecurityProtocolsTls10 { get; set; }

    [JsonProperty("Microsoft.WindowsAzure.ApiManagement.Gateway.Security.Protocols.Tls11")]
    public string MicrosoftWindowsAzureApiManagementGatewaySecurityProtocolsTls11 { get; set; }

    [JsonProperty("Microsoft.WindowsAzure.ApiManagement.Gateway.Security.Protocols.Ssl30")]
    public string MicrosoftWindowsAzureApiManagementGatewaySecurityProtocolsSsl30 { get; set; }

    [JsonProperty("Microsoft.WindowsAzure.ApiManagement.Gateway.Security.Ciphers.TripleDes168")]
    public string MicrosoftWindowsAzureApiManagementGatewaySecurityCiphersTripleDes168 { get; set; }

    [JsonProperty("Microsoft.WindowsAzure.ApiManagement.Gateway.Security.Backend.Protocols.Tls10")]
    public string MicrosoftWindowsAzureApiManagementGatewaySecurityBackendProtocolsTls10 { get; set; }

    [JsonProperty("Microsoft.WindowsAzure.ApiManagement.Gateway.Security.Backend.Protocols.Tls11")]
    public string MicrosoftWindowsAzureApiManagementGatewaySecurityBackendProtocolsTls11 { get; set; }

    [JsonProperty("Microsoft.WindowsAzure.ApiManagement.Gateway.Security.Backend.Protocols.Ssl30")]
    public string MicrosoftWindowsAzureApiManagementGatewaySecurityBackendProtocolsSsl30 { get; set; }

    [JsonProperty("Microsoft.WindowsAzure.ApiManagement.Gateway.Protocols.Server.Http2")]
    public string MicrosoftWindowsAzureApiManagementGatewayProtocolsServerHttp2 { get; set; }
}

public class ApiVersionConstraint
{
    public object minApiVersion { get; set; }
}

public class Properties
{
    public string publisherEmail { get; set; }
    public string publisherName { get; set; }
    public string notificationSenderEmail { get; set; }
    public string provisioningState { get; set; }
    public string targetProvisioningState { get; set; }
    public DateTime createdAtUtc { get; set; }
    public string gatewayUrl { get; set; }
    public string gatewayRegionalUrl { get; set; }
    public string portalUrl { get; set; }
    public string developerPortalUrl { get; set; }
    public string managementApiUrl { get; set; }
    public string scmUrl { get; set; }
    public List<HostnameConfiguration> hostnameConfigurations { get; set; }
    public List<string> publicIPAddresses { get; set; }
    public object privateIPAddresses { get; set; }
    public List<AdditionalLocation> additionalLocations { get; set; }
    public object virtualNetworkConfiguration { get; set; }
    public CustomProperties customProperties { get; set; }
    public string virtualNetworkType { get; set; }
    public object certificates { get; set; }
    public bool disableGateway { get; set; }
    public ApiVersionConstraint apiVersionConstraint { get; set; }
}

public class APIMGateway
{
    public string id { get; set; }
    public string name { get; set; }
    public string type { get; set; }
    public Tags tags { get; set; }
    public string location { get; set; }
    public string etag { get; set; }
    public Properties properties { get; set; }
    public Sku sku { get; set; }
    public object identity { get; set; }
}