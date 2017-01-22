﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Power_of_God_Lib.GUI;
using Power_of_God_Lib.GUI.DialogBox;

namespace Power_of_God_Lib.Utilities
{
    
    public class Network
    {
        public static bool DailyScriptureAlreadyPulled = false;
        public const bool IsPreRelease = false;
        public static string VersionPrefix = "Beta";
        ///
        public static void DownloadScripture()
        {
            var dlForm = new DownloadDialogBox("Downloading Scripture",
                "Most features of Power of God require that you have the Scriptures downloaded.\n\nSelected Version: " + Settings.UserSettings.scriptver,
                "http://powerofgodonline.net/Application/Bibles/" + Settings.UserSettings.scriptver + ".xml", 
                "power.of.god/Bibles/" + Settings.UserSettings.scriptver + ".xml");
            dlForm.ShowDialog();
        }
        public static string VersionLetter()
        {
            var x = VersionPrefix.ToLower().Substring(0, 1);
            return x;
        }
        public const int ReleaseNumber = 7;
        

        private static readonly string[] CurrentVersion = {
            "1", "0", null, null
        };
        /// <summary>
        /// Gets current installed version
        /// </summary>
        /// <param name="catchingPreRelease">True if Pre-Release</param>
        /// <returns></returns>
        public static string LatestStable(bool catchingPreRelease)
        {
            if (!IsPreRelease || catchingPreRelease)
            {
                var returnValue = VersionPrefix + " " + CurrentVersion[0] + "." +
                 CurrentVersion[1];
                if (CurrentVersion[2] == null) return returnValue;
                returnValue += "." + CurrentVersion[2];
                if (CurrentVersion[3] != null)
                {
                    returnValue += "." + CurrentVersion[3];
                }
                return returnValue;
            }
            return "Pre-Release " + PreRelease.GetString();
        }

        private static int CurrentPrefixInt()
        {
            switch (VersionPrefix)
            {
                case "Alpha":
                    return 0;
                case "Beta":
                    return 1;
                default:
                    return 2;
            }
        }

        public static string ServerResponse = "Nothing Yet";

        public static string UpdateNotice()
        {
            switch (UpdateWord())
            {
                case "Updated":
                    return "You are currently updated to the most recent version.";
                case "Outdated":
                    return "You do not have the most recent version. Consider updating to " +
                           LatestOnline();
                default:
                    if (CurrentPrefixInt() == 5000)
                    {
                        return
                            "You are using a Pre-Release. Please note that not all features will be functional.\n\n" +
                            "(" + LatestStable(false) + ")";
                    }
                    else
                    {
                        return "You are using an unsupported version of Power of God. \nPlease consider " +
                            "using the current version, " + LatestOnline() +
                            ", \nbecause your version could possibly be unstable.";
                    }
            }
        }

        public static string UpdateWord()
        {
            try

            {

                int onlinePrefix;
                switch (GetUrlSource(Url).ElementAt(0).Substring(1))
                {
                    case "Alpha":
                        onlinePrefix = 0;
                        break;
                    case "Beta":
                        onlinePrefix = 1;
                        break;
                    default:
                        onlinePrefix = 2;
                        break;
                }
                GetUrlSource(Url);
                var itemcontext = GetUrlSource(Url).ElementAt(1);
                var item1 = int.Parse(itemcontext);
                var item2 = int.Parse(GetUrlSource(Url).ElementAt(2));
                int item3; // If these values stay as -1 then the app will know to not read them
                var item4 = -1;
                try
                {
                    item3 = int.Parse(GetUrlSource(Url).ElementAt(3));
                    try
                    {
                        item4 = int.Parse(GetUrlSource(Url).ElementAt(4));
                    }
                    catch (Exception)
                    {
                        item4 = -1;
                    }
                }
                catch (Exception)
                {
                    item3 = -1;
                }
                if (onlinePrefix > CurrentPrefixInt()) return "Outdated";
                if (onlinePrefix < CurrentPrefixInt()) return "Unsupported";
                if (item1 > CurrentVersionInt().ElementAt(0)) return "Outdated";
                if (item1 < CurrentVersionInt().ElementAt(0)) return "Unsupported";
                if (item2 > CurrentVersionInt().ElementAt(1)) return "Outdated";
                if (item2 < CurrentVersionInt().ElementAt(1)) return "Unsupported";
                if (item3 > -1)
                {
                    var local3 = CurrentVersionInt().ElementAt(2);
                    if (item3 > local3) return "Outdated";
                    if (item3 < local3) return "Unsupported";
                    if (item4 > -1)
                    {
                        var local4 = CurrentVersionInt().ElementAt(3);
                        if (item4 > local4) return "Outdated";
                        if (item4 < local4) return "Unsupported";
                    }
                    else
                    {
                        var localHere = CurrentVersion[3];
                        if (localHere != null) return "Unsupported";
                    }
                }
                else
                {
                    var localHere = CurrentVersion[2];
                    if (localHere != null) return "Unsupported";
                }
                return "Updated"; // If it gets to this point - the user has passed all checks. They are updated!
            }
            catch (Exception ex)
            {
                ErrorLogging.Write(ex);
                return "Updated"; // Exception caught. Let the user continue.
            }
        }

        private static IEnumerable<int> CurrentVersionInt()
        {
            var xList = new List<int>();
            foreach (var x in CurrentVersion)
            {
                try
                {
                    xList.Add(int.Parse(x));
                }
                catch (Exception)
                {
                    xList.Add(-1);
                }
            }
            return xList;
        }

        private const string Url = "http://godispower.us/Application/Updates.txt";
        /// <summary>
        /// Gets latest version
        /// </summary>
        /// <returns></returns>
        public static string LatestOnline()
        {
            var x = GetUrlSource(Url).ElementAt(0) + " ";
            var versionStoppedAt = 0;
            for (var x2 = 1; x2 <= 4; x2++)
            {
                if (x2 >= 5) continue;
                if (!GetUrlSource(Url).ElementAt(x2).Equals("NR"))
                {
                    if (x2 < 4) x += "" + GetUrlSource(Url).ElementAt(x2) + ".";
                    else if (x2 == 4) x += "" + GetUrlSource(Url).ElementAt(x2);
                }
                else
                {
                    versionStoppedAt = x2;
                }
            }
            if (versionStoppedAt == 3 || versionStoppedAt == 4)
            {
                return x.Substring(0, x.Length - 1);
            }
            return x;
        }
        /// <summary>
        /// Obtain source of webpage
        /// </summary>
        /// <param name="urlF">The url needed</param>
        /// <returns>source of page</returns>
        public static List<string> GetUrlSource(string urlF)
        {
            if (urlF.Contains("0201"))
            {
                urlF = urlF.Replace("0201", "2016");
            }
            var temp = "power.of.god/check_file.txt";
            var c = File.CreateText(temp);

            c.Close();
            using (var client = new WebClient())
            {
                try
                {
                    client.DownloadFile(urlF, temp);
                }
                catch (Exception e)
                {
                    ErrorLogging.Write(e);
                }

            }
            return File.ReadAllLines(temp).ToList();
        }

        public static WebBrowser XBrowser = new WebBrowser();
        public static void UpdateWebContent(string html)
        {
            XBrowser.DocumentText = html;
        }

        public static void Navigate(string url)
        {
            XBrowser.Navigate(url);
        }
        /// <summary>
        /// Gets mediafire download of mediafire link
        /// </summary>
        /// <param name="download"></param>
        /// <returns></returns>
        public static string Mediafire(string download)
        {
            var req = (HttpWebRequest)WebRequest.Create(download);
            var res = (HttpWebResponse)req.GetResponse();
            // ReSharper disable once AssignNullToNotNullAttribute
            var str = new StreamReader(res.GetResponseStream()).ReadToEnd();
            var indexurl = str.IndexOf("http://download", StringComparison.Ordinal);
            var indexend = GetNextIndexOf('"', str, indexurl);
            var direct = str.Substring(indexurl, indexend - indexurl);
            return direct;
        }

        private static int GetNextIndexOf(char c, string source, int start)
        {
            if (start < 0 || start > source.Length - 1)
            {
                throw new ArgumentOutOfRangeException();
            }
            for (var i = start; i < source.Length; i++)
            {
                if (source[i] == c)
                {
                    return i;
                }
            }
            return -1;
        }

        public static void DownloadFile(string internetLocation, string localLocation)
        {
            using (var client = new WebClient())
            {
                client.DownloadFile(internetLocation, localLocation);
            }
        }

        private static string GetDirectoryListingRegexForUrl(string url)
        {
            return "\\\"([^\"]*)\\\"";
        }

        private static readonly List<string> Files = new List<string>();

        private static void Write()
        {
            var fw = File.CreateText("tempradio.txt");
            fw.Write(Files.Aggregate("", (current, f) => current + ("\n" + f)));
            fw.Flush();
            fw.Close();

        }
        public static List<string> GetWebDirectory(string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            using (var response = (HttpWebResponse)request.GetResponse())
            {
                // ReSharper disable once AssignNullToNotNullAttribute
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    var html = reader.ReadToEnd();

                    var regex = new Regex(GetDirectoryListingRegexForUrl(url));
                    var matches = regex.Matches(html);
                    if (matches.Count > 0)
                    {
                        foreach (var match in matches.Cast<Match>().Where(match => match.Success))
                        {
                            Files.Add(match.ToString());
                        }
                    }
                }
                Write();
            }
            return Files;
        }

        /// <summary>
        /// Gets the title of a webpage
        /// </summary>
        /// <param name="url">the webpage</param>
        /// <returns>title of webpage</returns>
        public static string GetPageTitle(string url)
        {
            var title = "";
            try
            {
                var request = (WebRequest.Create(url) as HttpWebRequest);
                if (request == null) return title;
                var response = (request.GetResponse() as HttpWebResponse);

                if (response == null) return title;
                using (var stream = response.GetResponseStream())
                {
                    // compiled regex to check for <title></title> block
                    var titleCheck = new Regex(@"<title>\s*(.+?)\s*</title>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                    const int bytesToRead = 8092;
                    var buffer = new byte[bytesToRead];
                    var contents = "";
                    int length;
                    while (stream != null && (length = stream.Read(buffer, 0, bytesToRead)) > 0)
                    {
                        // convert the byte-array to a string and add it to the rest of the
                        // contents that have been downloaded so far
                        contents += Encoding.UTF8.GetString(buffer, 0, length);

                        var m = titleCheck.Match(contents);
                        if (m.Success)
                        {
                            // we found a <title></title> match =]
                            title = m.Groups[1].Value;
                            break;
                        }
                        else if (contents.Contains("</head>"))
                        {
                            // reached end of head-block; no title found =[
                            break;
                        }
                    }
                }
                return title;
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR " + e);
                ErrorLogging.Write(e);
                return "Error";
            }
        }
    }

    
}