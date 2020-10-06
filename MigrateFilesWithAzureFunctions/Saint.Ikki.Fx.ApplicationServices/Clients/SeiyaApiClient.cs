using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using Saint.Ikki.Fx.ApplicationServices.Abstract;
using Saint.Ikki.Fx.ApplicationServices.Abstract.Clients;
using Saint.Ikki.Fx.Shared.Models;
using Saint.Ikki.Fx.Shared.Models.Infrastructure;
using Saint.Ikki.Fx.Shared.Models.Seiya;
using Saint.Ikki.Fx.Shared.Models.Seiya.Exceptions;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace Saint.Ikki.Fx.ApplicationServices.Clients
{
    public class SeiyaApiClient : ISeiyaApiClient, IAsyncInitialization
    {
        private readonly HttpClient httpClient;
        private readonly AppSettings appSettings;

        public Task InitializationTask { get; private set; }

        public SeiyaApiClient(HttpClient httpClient, IOptions<AppSettings> options)
        {
            this.httpClient = httpClient; 
            this.appSettings = options.Value;
            InitializationTask = InitializeClient();
        }

        public async Task<bool> DeleteDocumentAsync(DeleteRequestModel requestModel)
        {
            using (var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri($"{httpClient.BaseAddress}api/document/delete"),
                Content = new StringContent(
                    JsonConvert.SerializeObject(requestModel),
                    Encoding.UTF8,
                    MediaTypeNames.Application.Json)
            })
            using (var response = await httpClient.SendAsync(httpRequestMessage))
            {
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<bool>();
                }

                throw await HandleError(response);
            }
        }

        public async Task<ResponseModel> IndexBinaryDocumentAsync(IndexBinaryRequestModel requestModel, Stream fileStream, string fileName)
        {
            using var formDataContent = new MultipartFormDataContent();
            var streamContent = new StreamContent(fileStream);
            streamContent.Headers.ContentType = new MediaTypeHeaderValue(MimeTypes.GetMimeType(fileName));

            formDataContent.Add(
                new StringContent(
                    JsonConvert.SerializeObject(requestModel),
                    Encoding.UTF8,
                    MediaTypeNames.Application.Json),
                "Metadata");
            formDataContent.Add(streamContent, "File", fileName);

            using var response = await httpClient.PostAsync("api/document/indexbinary", formDataContent);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<ResponseModel>();
            }

            throw await HandleError(response);
        }
 
        public async Task<UriResponseModel> GetDocumentUriAsync(string documentId, string documentClassId, string user)
        {
            var url = $"document/uri?id={documentId}&documentclassid={documentClassId}&user={user}";
            using (var response = await httpClient.GetAsync(url))
            {
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var objectResult = JsonConvert.DeserializeObject<UriResponseModel>(responseContent);
                    return objectResult;
                }

                throw await HandleError(response);
            }
        }

        public async Task<bool> RevertDeleteDocumentAsync(DeleteRequestModel requestModel)
        {
            using (var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri($"{httpClient.BaseAddress}api/document/revertDelete"),
                Content = new StringContent(
                    JsonConvert.SerializeObject(requestModel),
                    Encoding.UTF8,
                    MediaTypeNames.Application.Json)
            })
            using (var response = await httpClient.SendAsync(httpRequestMessage))
            {
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<bool>();
                }

                throw await HandleError(response);
            }
        }
        private async Task<AuthenticationResult> GetTokenAsync()
        {
            var authenticationContext = new AuthenticationContext(appSettings.SeiyaApiClientConfig.AzureAd.Authority, false);
            var authenticationResult = await authenticationContext.AcquireTokenAsync(
                                        appSettings.SeiyaApiClientConfig.AzureAd.Resource,
                                        new ClientCredential(appSettings.SeiyaApiClientConfig.AzureAd.ClientId, appSettings.SeiyaApiClientConfig.AzureAd.ClientSecret));
            return authenticationResult;
        }

        private async Task InitializeClient()
        {
            httpClient.BaseAddress = appSettings.SeiyaApiClientConfig.HttpClientOptions.BaseAddress;
            httpClient.Timeout = appSettings.SeiyaApiClientConfig.HttpClientOptions.Timeout;

            const string X_USER_ID_HEADER = "x-user-name"; 
            var authenticationResult = await GetTokenAsync();
            
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                                                                    authenticationResult.AccessTokenType,
                                                                    authenticationResult.AccessToken);
            httpClient.DefaultRequestHeaders.Add(X_USER_ID_HEADER, appSettings.SeiyaApiClientConfig.UserName);
        }

        private async Task<Exception> HandleError(HttpResponseMessage response)
        {
            if ((int)response.StatusCode == StatusCodes.Status401Unauthorized)
            {
                return new SeiyaBaseException(new HttpResponseMessage(HttpStatusCode.Unauthorized));
            }

            return new SeiyaBaseException(JsonConvert.DeserializeObject<ProblemDetails>(await response.Content.ReadAsStringAsync()));
        }

        public async Task<bool> UpdateProcessAsync(UpdateStatusRequest requestModel)
        {
            using (var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"{httpClient.BaseAddress}api/document/updateProcess"),
                Content = new StringContent(
                    JsonConvert.SerializeObject(requestModel),
                    Encoding.UTF8,
                    MediaTypeNames.Application.Json)
            })
            using (var response = await httpClient.SendAsync(httpRequestMessage))
            {
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<bool>();
                }

                throw await HandleError(response);
            }
        }
    }
}
