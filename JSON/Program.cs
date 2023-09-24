using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.Remoting.Channels;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Policy;
using static Program;

class Program
{
    static async Task Main(string[] args)
    {
        string url = "https://googlechromelabs.github.io/chrome-for-testing/known-good-versions-with-downloads.json"; // Replace with your JSON endpoint URL

        using (HttpClient httpClient = new HttpClient())
        {
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    //Console.WriteLine(json);
                    Deserialise(json);
                    // Now you have the JSON string from the URL in the 'json' variable.
                }
                else
                {
                    Console.WriteLine($"HTTP request failed with status code {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }

    // Define classes to represent the JSON structure

    public class Download
    {
        public string platform { get; set; }
        public string url { get; set; }
    }

    public class Version
    {
        public string version { get; set; }
        public string revision { get; set; }
        public Dictionary<string, List<Download>> downloads { get; set; }
    }

    public class Root
    {
        public DateTime timestamp { get; set; }
        public List<Version> versions { get; set; }
    }


    static void Deserialise(string jsonString)
    {
        // Replace 'jsonString' with your JSON string

        // Deserialize JSON
        Root root = JsonConvert.DeserializeObject<Root>(jsonString);
        string targetVersion = "117";
        string targetPlatform = "win32"; // Specify the platform you want
        string targetDonwnload = "chromedriver"; //specify the download

        // Now you can access the data as C# objects
        Console.WriteLine($"Timestamp: {root.timestamp}");

        List<Version> matchingVersions = root.versions.Where(v => v.version.StartsWith(targetVersion) && v.downloads.ContainsKey(targetDonwnload)).ToList();

        if( matchingVersions.Count > 0 ) 
        {

            foreach (var version in matchingVersions)
            {
                Console.WriteLine($"Version: {version.version}");
                Console.WriteLine($"Revision: {version.revision}");

                // Check if "win32" download is available for this version
                if (version.downloads.ContainsKey("chromedriver"))
                {
                    List<Download> chromedriverDownloads = version.downloads[targetDonwnload];
                    var win32Download = chromedriverDownloads.FirstOrDefault(d => d.platform == targetPlatform);

                    if (win32Download != null)
                    {
                        Console.WriteLine($"Win32 Download URL: {win32Download.url}");
                    }
                    else
                    {
                        Console.WriteLine($"Win32 Download not available for this version.");
                    }
                }
                else
                {
                    Console.WriteLine($"No 'chromedriver' downloads available for this version.");
                }
            }
        }
        else
        {
            Console.Write("matching version not found");
        }
        
           Console.ReadLine();
    }
}       









