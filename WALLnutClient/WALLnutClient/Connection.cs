﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WALLnutClient
{
    class Connection
    {
        public static readonly HttpClient client = new HttpClient();
        public static string access_token = string.Empty;

        static Connection()
        {
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public static async Task<String> PostRequest(string url, Dictionary<string, string> body)
        {
            try
            {
                body.Add("access_token", access_token);
                StringContent content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(Properties.Settings.Default.SERVER_URL + url, content);

                return await response.Content.ReadAsStringAsync();
            }
            catch
            {
                MessageBox.Show("Network Err...");
                return await Task.FromResult<string>("");
            }
        }
    }
}
