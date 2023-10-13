using System;
using System.IO;
using System.Text;
using System.IO.Compression;

namespace Element34.ExtensionClasses
{
    /**
 * This class can be used to generate a minimal browser extension that
 * will force Chrome to send out an Authorize header on every request.
 *
 * This is the only reliable way that has worked for me with Selenium 3
 * to use basic auth.
 *
 * Alternatives to this approach:
 * - Add the basic auth credentials to the url (does not work in recent versions of FF and Chrome)
 * - Using a proxy that adds the header (LittleProxy). This should probably work,
 *   but https with proxies can be a pain in the ass. Also the last release of LittleProxy is from 2017.
 *   Launching an extra server for every test would also unnecessarily increase the complexity of tests.
 * - Using selenium as the user would by filling out the username, password text boxes and clicking on the login button.
 *   This works, but with login forms adding captcha and other deterrents to stop bots can be unreliable.
 * - Use a cookie for authentication and add that to the selenium browser. This works great if you can get
 *   a valid session cookie.
 *
 * Useful links:
 * - https://stackoverflow.com/a/35293026
 * - https://devopsqa.wordpress.com/2018/08/05/handle-basic-authentication-in-selenium-for-chrome-browser/
 * - https://stackoverflow.com/a/27936481
 * - https://sqa.stackexchange.com/questions/12892/how-to-send-basic-authentication-headers-in-selenium
 */

    public class SeleniumChromeAuthExtensionBuilder : IDisposable
    {
        private string headerJsCode = "";
        private string baseUrl = "<all_urls>";
        protected bool disposed;

        public SeleniumChromeAuthExtensionBuilder WithStaticHeader(string name, string value)
        {
            string nameEscaped = EscapeJavaScriptString(name);
            string valueEscaped = EscapeJavaScriptString(value);
            headerJsCode += $"headers.push({{name: \"{nameEscaped}\", value: \"{valueEscaped}\"}});\n";
            return this;
        }

        public SeleniumChromeAuthExtensionBuilder WithBasicAuth(string username, string password)
        {
            string encodeUserPass = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"));
            return WithStaticHeader("Authorization", "Basic " + encodeUserPass);
        }

        public SeleniumChromeAuthExtensionBuilder WithBaseUrl(string baseUrl)
        {
            this.baseUrl = baseUrl;
            return this;
        }

        public FileInfo Build()
        {
            string tempFileName = Path.GetTempFileName();
            using (var zipArchive = ZipFile.Open(tempFileName, ZipArchiveMode.Create))
            {
                zipArchive.CreateEntry("manifest.json").WriteContent(generateManifestJson(), Encoding.UTF8);
                zipArchive.CreateEntry("background.js").WriteContent(generateBackgroundJs(), Encoding.UTF8);
            }
            return new FileInfo(tempFileName);
        }

        private string generateBackgroundJs()
        {
            return "chrome.webRequest.onBeforeSendHeaders.addListener(\n" +
                    "    function(e) {\n" +
                    "        var headers = e.requestHeaders;\n" +
                    headerJsCode +
                    "        return { requestHeaders: headers };\n" +
                    "    },\n" +
                    $"    {{urls: [\"{EscapeJavaScriptString(baseUrl)}\"]}},\n" +
                    "    ['blocking', 'requestHeaders' , 'extraHeaders']\n" +
                    ");";
        }

        private string generateManifestJson()
        {
            return "{\n" +
                    "    \"manifest_version\": 2,\n" +
                    "    \"name\": \"Authentication for selenium tests\",\n" +
                    "    \"version\": \"1.0.0\",\n" +
                    "    \"permissions\": [\"<all_urls>\", \"webRequest\", \"webRequestBlocking\"],\n" +
                    "    \"background\": {\n" +
                    "      \"scripts\": [\"background.js\"]\n" +
                    "    }\n" +
                    "  }";
        }

        private static string EscapeJavaScriptString(string input)
        {
            return input.Replace("\"", "\\\"");
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                if (headerJsCode != null)
                    headerJsCode = null;

                if (baseUrl != null)
                    baseUrl = null;

                // delete temp file
            }

            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }

    public static class ZipArchiveEntryExtensions
    {
        public static void WriteContent(this ZipArchiveEntry entry, string content, Encoding encoding)
        {
            using (var streamWriter = new StreamWriter(entry.Open(), encoding))
            {
                streamWriter.Write(content);
            }
        }
    }
}

