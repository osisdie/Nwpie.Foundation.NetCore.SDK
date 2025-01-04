﻿using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Auth.Models;
using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.Foundation.S3Proxy.Contract.Login;
using Nwpie.Foundation.S3Proxy.Contract.Upload;
using Newtonsoft.Json;
using ServiceStack;
using Xunit;

namespace Nwpie.IntegrationTest.Serverless
{
    public class S3Proxy_Test : TestBase
    {
        [Fact(Skip = "Won't test integration in unittests")]
        public async Task Flow_Test()
        {
            var login = Login_Test();
            await login;
            await Upload_Test();
        }

        private async Task Login_Test()
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var request = new AcctLogin_Request
                {
                    Data = new AcctLogin_RequestModel
                    {
                        Email = "dev@kevinw.net",
                        Password = "**"
                    }
                };

                var url = string.Concat($"{m_QCBaseUrl}", request.ToPostUrl());
                var content = new StringContent(JsonConvert.SerializeObject(request),
                    Encoding.UTF8, "application/json"
                );

                var message = await httpClient.PostAsync(url, content);
                var input = await message.Content.ReadAsStringAsync();
                Assert.NotNull(input);

                var response = JsonConvert.DeserializeObject<AcctLogin_Response>(input);
                Assert.NotNull(response);
                Assert.True(response.IsSuccess);
                Assert.NotNull(response.Data?.AccessToken);

                m_AccessToken = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new AuthRequestModel()
                {
                    Token = response.Data.AccessToken,
                    Ip = "", // ignore
                    Mac = "" // ignore
                })));
            }
        }

        private async Task Upload_Test()
        {
            var bucketName = "svc-todo-api";
            var bytes = Convert.FromBase64String("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");
            var path = $"results/test/fileKey_123-{DateTime.UtcNow:yyyyMMddHHmmss}.png";

            var request = new ImageUpload_Request
            {
                Bucket = bucketName,
                FileKey = path
            };

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation(CommonConst.AuthHeaderName, m_AccessToken);

                var byteArrayContent = new ByteArrayContent(bytes);
                byteArrayContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/png");

                var url = string.Concat($"{m_S3ProxyBaseUrl}", $"{request.ToPutUrl()}");
                var message = await httpClient.PutAsync(url, new MultipartFormDataContent
                {
                    {new StringContent(bucketName), "\"bucket\""},
                    {new StringContent(path), "\"fileKey\""},
                    {byteArrayContent, "\"file\"", "\"fileKey_123.png\""}
                });

                var input = await message.Content.ReadAsStringAsync();
                Assert.NotNull(input);

                var response = JsonConvert.DeserializeObject<ImageUpload_Response>(input);
                Assert.NotNull(response);
                Assert.True(response.IsSuccess);
            }
        }

        private readonly string m_QCBaseUrl = "https://api-dev.kevinw.net/todo";
        private readonly string m_S3ProxyBaseUrl = "https://api-dev.kevinw.net/qc-app";

        private string m_AccessToken = string.Empty;
    }
}
