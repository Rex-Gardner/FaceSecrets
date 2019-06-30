using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace API.Services
{
    public static class PhotolabService
    {
        public static async Task<string> UploadImage(byte[] image, string fileName)
        {
            using (var client = new HttpClient())
            {
                using (var content = new MultipartFormDataContent())
                {
                    content.Headers.Add("Content-Length", image.Length.ToString());
                    const string key = "file1";
                    content.Add(new StreamContent(new MemoryStream(image)), key, fileName);
                    
                    var parameters = new Dictionary<string, string> {{"no_resize", "1"}};
                    var dictionaryItems = new FormUrlEncodedContent(parameters);
                    content.Add(dictionaryItems);

                    using (
                        var message =
                            await client.PostAsync("http://upload-soft.photolab.me/upload.php", content))
                    {
                        var input = await message.Content.ReadAsStringAsync();

                        if (!string.IsNullOrWhiteSpace(input) && input.Contains("http"))
                        {
                            return input;
                        }

                        return null;
                    }
                }
            }
        }
        
        public static async Task<string> PostMultipartFormData(string imgUrl, string templateName)
        {
            HttpContent imgUrlContent = new StringContent(imgUrl);
            HttpContent templateNameContent = new StringContent(templateName);

            using (var client = new HttpClient())
            {
                using (var formData = new MultipartFormDataContent())
                {
                    formData.Add(imgUrlContent, "image_url[1]");
                    formData.Add(templateNameContent, "template_name");
                   
                    using (
                        var message =
                            await client.PostAsync("http://api-soft.photolab.me/template_process.php", formData))
                    {
                        var input = await message.Content.ReadAsStringAsync();

                        if (!string.IsNullOrWhiteSpace(input) && input.Contains("http"))
                        {
                            return input;
                        }

                        return null;
                    }
                }
            }
        }
    }
}