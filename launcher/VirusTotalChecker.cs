using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

public class VirusTotalChecker
{
    private static readonly HttpClient client = new HttpClient();

    #region Properties
    public bool VirusFree { get; private set; }
    #endregion

    /// <summary>
    /// Checks the specified file for viruses using the VirusTotal API.
    /// </summary>
    /// <param name="filePath">The path to the file to be checked.</param>
    /// <param name="apiKey">The API key for accessing the VirusTotal API.</param>
    /// <returns>The JSON response from the VirusTotal API.</returns>
    public async Task<string> CheckFileAsync(string filePath, string apiKey)
    {
        var url = "https://www.virustotal.com/api/v3/files";
        var request = new HttpRequestMessage(HttpMethod.Post, url);

        request.Headers.Add("x-apikey", apiKey);

        var multiContent = new MultipartFormDataContent();
        var fileBytes = File.ReadAllBytes(filePath);
        var byteArrayContent = new ByteArrayContent(fileBytes);
        byteArrayContent.Headers.Add("Content-Type", "application/octet-stream");
        multiContent.Add(byteArrayContent, "file", Path.GetFileName(filePath));

        request.Content = multiContent;

        var response = await client.SendAsync(request);
        var jsonResponse = await response.Content.ReadAsStringAsync();

        var jsonDocument = JsonDocument.Parse(jsonResponse);
        var root = jsonDocument.RootElement;
        var analysisId = root.GetProperty("data").GetProperty("id").GetString();

        // get the result from the VirusTotal API
        jsonResponse = await AnalyzeFileAsync(analysisId, apiKey);

        VirusFree = VirusDetected(jsonResponse);

        return jsonResponse;
    }

    /// <summary>
    /// Retrieves the scan results for the specified analysis ID using the VirusTotal API.
    ///This method is straightforward.It sends a GET request to the VirusTotal API and returns
    /// the JSON response

    /// </summary>
    /// <param name="analysisId">The ID of the analysis.</param>
    /// <param name="apiKey">The API key for accessing the VirusTotal API.</param>
    /// <returns>The JSON response from the VirusTotal API.</returns>
    private async Task<string> GetScanResultsAsync(string analysisId, string apiKey)
    {
        var url = $"https://www.virustotal.com/api/v3/analyses/{analysisId}";
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add("x-apikey", apiKey);
        var response = await client.SendAsync(request);
        var jsonResponse = await response.Content.ReadAsStringAsync();

        return jsonResponse;
    }

    /// <summary>
    /// Waits for the scan results for the specified analysis ID using the VirusTotal API.
    /// This method is a bit more complex.It repeatedly calls [`GetScanResultsAsync`] every
    /// minute until it no longer receives a "NotFoundError". This method requires a bit
    /// more understanding of the context to fully grasp its purpose.
    /// </summary>
    /// <param name="analysisId">The ID of the analysis.</param>
    /// <param name="apiKey">The API key for accessing the VirusTotal API.</param>
    /// <returns>The JSON response from the VirusTotal API.</returns>
    public async Task<string> WaitForScanResultsAsync(string analysisId, string apiKey)
    {
        string result;
        do
        {
            await Task.Delay(TimeSpan.FromMinutes(1)); // Wait for 1 minute
            result = await GetScanResultsAsync(analysisId, apiKey);
        }
        while (result.Contains("NotFoundError")); // Continue if the result is not found

        return result;
    }

    /// <summary>
    /// Determines whether the file is virus free based on the JSON response from the VirusTotal API.
    /// </summary>
    /// <param name="jsonResponse">The JSON response from the VirusTotal API.</param>
    /// <returns>True if the file is virus; otherwise, false.</returns>
    public bool IsFileVirusFree(string jsonResponse)
    {
        var jsonDocument = JsonDocument.Parse(jsonResponse);
        var root = jsonDocument.RootElement;
        var attributes = root.GetProperty("data").GetProperty("attributes");

        var status = attributes.GetProperty("status").GetString();

        // If the analysis is not completed yet, we can't determine whether the file is good
        if (status != "completed")
        {
            throw new InvalidOperationException("The analysis is not completed yet.");
        }

        var stats = attributes.GetProperty("stats");

        var malicious = stats.GetProperty("malicious").GetInt32();
        var suspicious = stats.GetProperty("suspicious").GetInt32();

        // If any engine detected the file as malicious or suspicious, we consider the file as not good
        return malicious == 0 && suspicious == 0;
    }

    /// <summary>
    /// Determines whether a virus is detected based on the JSON response from the VirusTotal API.
    /// </summary>
    /// <param name="jsonResponse">The JSON response from the VirusTotal API.</param>
    /// <param name="ignoreGridinsoft">Whether to ignore Gridinsoft detection.</param>
    /// <returns>True if no virus is detected; otherwise, false.</returns>
    public bool VirusDetected(string jsonResponse, bool ignoreGridinsoft = true)
    {
        var jsonDocument = JsonDocument.Parse(jsonResponse);
        var root = jsonDocument.RootElement;
        var attributes = root.GetProperty("data").GetProperty("attributes");

        var status = attributes.GetProperty("status").GetString();

        // If the analysis is not completed yet, we can't determine whether the file is good
        if (status != "completed")
        {
            throw new InvalidOperationException("The analysis is not completed yet.");
        }

        var results = attributes.GetProperty("results");
        var stats = attributes.GetProperty("stats");

        var malicious = stats.GetProperty("malicious").GetInt32();
        var suspicious = stats.GetProperty("suspicious").GetInt32();

        if (ignoreGridinsoft)
        {
            // If Gridinsoft is the only one that detected the file as malicious, ignore it
            if (malicious == 1 && results.TryGetProperty("Gridinsoft", out var gridinsoftResult))
            {
                var category = gridinsoftResult.GetProperty("category").GetString();
                if (category == "malicious")
                {
                    malicious = 0;
                }
            }
        }
        // If any engine detected the file as malicious or suspicious, we consider the file as not good
        return malicious == 0 && suspicious == 0;
    }

    /// <summary>
    /// Analyzes the file with the specified analysis ID using the VirusTotal API.
    /// </summary>
    /// <param name="analysisId">The ID of the analysis.</param>
    /// <param name="apiKey">The API key for accessing the VirusTotal API.</param>
    /// <returns>The JSON response from the VirusTotal API.</returns>
    public async Task<string> AnalyzeFileAsync(string analysisId, string apiKey)
    {
        string jsonResponse;
        string status = "";

        do
        {
            await Task.Delay(TimeSpan.FromMinutes(1)); // Wait for 1 minute

            jsonResponse = await GetScanResultsAsync(analysisId, apiKey);

            var jsonDocument = JsonDocument.Parse(jsonResponse);
            var root = jsonDocument.RootElement;

            // If the analysis is not found or not completed yet, continue waiting
            if (jsonResponse.Contains("NotFoundError"))
            {
                continue;
            }

            status = root.GetProperty("data").GetProperty("attributes").GetProperty("status").GetString();

        } while (status == "queued");

        // Now the analysis should be completed, so we can interpret the results
        return jsonResponse;
    }
}
