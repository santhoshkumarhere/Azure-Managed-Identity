using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Net;
using System.Collections.Generic;
using Microsoft.Azure.Services.AppAuthentication;
using Azure.Identity;

namespace MIClient
{
    public static class MIClient
    {
        [FunctionName("MIClient")]
        public static async Task<object> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
           //  var tk = GetToken();
            var token = await GetMSIToken();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://mifuncapi.azurewebsites.net/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // New code:
                var response = await client.GetAsync("api/MIFuncAPI");
                if (response.IsSuccessStatusCode)
                {
                    return "Success" + await response.Content.ReadAsStringAsync();
                }
                else
                {
                    var res = await response.Content.ReadAsStringAsync();
                    return token + " res";
                }
            }

            return "No working";
        }

        private static async Task<string> GetMSIToken()
        {
            //var clientID = "ce74b7e8-3b26-4057-b703-f7b34065f2b5"; 
            var azureServiceTokenProvider = new AzureServiceTokenProvider("RunAs=App;AppId=ab0e941f-48e9-4739-b698-caee8c781f9c"); //User identity client Id
            string accessToken = await azureServiceTokenProvider.GetAccessTokenAsync("https://mifuncapi.azurewebsites.net");
            return accessToken;

            //var credential = new DefaultAzureCredential();
            //var token = await credential.GetTokenAsync(new 
            //              Azure.Core.TokenRequestContext(new[] { "https://managedidentityfunctionapi.azurewebsites.net" }));
            //return token.Token;
        }

        //private static string GetToken()
        //{
        //    var context = new AuthenticationContext("https://login.microsoftonline.com/77085693-d25a-4da4-b794-1390a6ca3936");

        //    var clientCredential = new ClientCredential("50b65b52-1c52-4e75-9b10-75a160fe262f", "-N83Ts~abK_IV5V~T0Be2aU1q7.z6B_Jg4");
        //    var authenticationResult = context.AcquireTokenAsync("https://managedidentityfunctionapi.azurewebsites.net", clientCredential).Result;
        //    var token = authenticationResult.AccessToken;
        //    return token;
        //}
    }
}
