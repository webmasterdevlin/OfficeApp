﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeApp.Helpers;
using OfficeApp.Models;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;


namespace OfficeApp.Services
{
    public class UserService : IUserService
    {
        private readonly HttpClient _client = new HttpClient();

        public async Task<bool> SignupAsync(User user)
        {
            var content = JsonConvert.SerializeObject(user);

            var response = await _client.PostAsync(Constants.URLs.SetSignupUrl(), new StringContent(content, Encoding.UTF8, "application/json"));

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> LoginAsync(User user)
        {
            string content = JsonConvert.SerializeObject(user);

            using (var response = await _client.PostAsync(Constants.URLs.SetLoginUrl(),
                                                      new StringContent(content, Encoding.UTF8, "application/json")))
            {
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return false;
                }

                else if (response.IsSuccessStatusCode)
                {
                    var stringResponse = await response.Content.ReadAsStringAsync();

                    JObject jwtJObject = JsonConvert.DeserializeObject<dynamic>(stringResponse);

                    UserToken userToken = JsonConvert.DeserializeObject<UserToken>(stringResponse);
                    Settings.Jwt = userToken.Token;

                    //DateTime accessTokenExpiration = jwtJObject.Value<DateTime>(".expires"); // FIXME
                    //Settings.JwtExpirationDate = accessTokenExpiration;

                    return response.IsSuccessStatusCode;
                }
                return false;
            }
        }
    }

    // ReSharper disable once ArrangeTypeModifiers
    internal class UserToken
    {
        [JsonProperty("token")]
        public string Token { get; set; }
    }
}