using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;


namespace CSGOChatGPT.Networking {
    public static class Requests {

        public static async Task<Res> PostJSON<Req, Res>(string endpoint, Req request, string bearerToken = null) {
            // Serialize the request object to JSON
            string json = JsonConvert.SerializeObject(request);

            // Create an instance of HttpClient
            using (HttpClient httpClient = new HttpClient()) {
                // Set the content type header to indicate that the request body is JSON
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                // Set the API key as a bearer token in the Authorization header, if provided
                if (!string.IsNullOrEmpty(bearerToken)) {
                    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearerToken);
                }

                // Create a StringContent object to represent the request body
                StringContent content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                // Send the POST request and await the response
                HttpResponseMessage response = await httpClient.PostAsync(endpoint, content);

                // Ensure the response was successful
                response.EnsureSuccessStatusCode();

                // Read the response body as a string
                string responseBody = await response.Content.ReadAsStringAsync();

                // Deserialize the response body from JSON to the specified result type
                Res result = JsonConvert.DeserializeObject<Res>(responseBody);

                // Return the deserialized result object
                return result;
            }
        }

    }
}
