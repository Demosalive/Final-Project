﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace MagicTheGatheringFinal.Models
{
    public class ScryfallDAL
    {
        public HttpClient GetClient()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://api.scryfall.com");
            //URI - uniform resource identifier
            return client;
        }

        public async Task<Cardobject> GetCard(string input)
        {
            var client = GetClient(); //calls the method that gives the API the general information needed to 
            //receive data from the API 
            var inputQuery = MakeQuery(input);
            var response = await client.GetAsync($"/named?fuzzy={inputQuery}"); //uses the client (HTTPClient) to receive 
            //data from the API based off of a certain endpoint.
            Cardobject card = await response.Content.ReadAsAsync<Cardobject>();
            //install-package Microsoft.AspNet.WebAPI.Client
            //response has a property called Content and Content has a method that reads the JSON and plugs it into a specified
            //obect. If the JSON does not fit within the object we get an Internal Deserialization error
            return card;
        }

        public string MakeQuery(string input)
        {
            //splits query into words separated by +, as this is the format for the api queries.
            string[] inputs = input.Split(" ");
            string query = "";
            foreach(string word in inputs)
            {
                query += word + "+";
            }

            //final interation of for loop leaves + on end of string, so returning a substring without the last +
            query = query.Substring(0, query.Length - 1);
            return query;
        }

        public static List<KeyValuePair<string, string>> BuildApiArguments(params string[] pairs)
        {
            if (pairs.Length % 2 == 1)
            {
                return null;
            }

            var ret = new List<KeyValuePair<string, string>>();

            for (int i = 0; i < pairs.Length; i += 2)
            {
                ret.Add(new KeyValuePair<string, string>(pairs[i], pairs[i + 1]));
            }

            return ret;
        }
        public static async Task<List<T>> GetApiResponseList<T>(string controller, string action, string baseUrl, string name,
    params KeyValuePair<string, string>[] options) where T : new()
        {
            string url = $"{baseUrl}/" +
                         $"{controller}/" +
                         $"{action}/" +
                         $"{name}";

            bool first = true;
            foreach (KeyValuePair<string, string> argument in options)
            {
                url += first ? "?" : "&";
                url += $"{argument.Key}={Uri.EscapeDataString(argument.Value)}";
                first = false;
            }

            HttpWebRequest request = WebRequest.CreateHttp(url);
            HttpWebResponse response;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException)
            {
                return null;
            }

            Stream s = response.GetResponseStream();
            if (s == null)
            {
                return null;
            }

            StreamReader rd = new StreamReader(s);

            string output = await rd.ReadToEndAsync();
            List<T> ret;
            try
            {
                ret = JsonConvert.DeserializeObject<List<T>>(output);
            }
            catch (JsonSerializationException)
            {
                ret = new List<T> { JsonConvert.DeserializeObject<T>(output) };
            }

            return ret;
        }
        public static async Task<T> GetApiResponse<T>(string controller, string action, string baseUrl, string SpeciesName)
    where T : new()
        {
            return (await GetApiResponseList<T>(controller, action, baseUrl, SpeciesName,
                (new List<KeyValuePair<string, string>>()).ToArray())).FirstOrDefault();
        }

    }
}