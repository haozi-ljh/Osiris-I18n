using System;
using System.IO;
using System.Net;
using System.Text;
using Pathfinding.Ionic.Zip;
using Pathfinding.Ionic.Crc;

namespace Osiris_I18n
{
    public class I18nUpdate
    {

        public static string Check()
        {

            string gitsite = "https://raw.githubusercontent.com/haozi-ljh/Osiris-I18n-Info/main/VersionInfo";
            HttpWebRequest githttp = (HttpWebRequest)WebRequest.Create(gitsite);
            HttpWebRequest githttp_fast = (HttpWebRequest)WebRequest.Create("https://ghproxy.com/" + gitsite);

            githttp_fast.Method = githttp.Method = "GET";
            githttp_fast.Timeout = githttp.Timeout = 3500;
            githttp_fast.KeepAlive = githttp.KeepAlive = true;

            StringBuilder sb = new StringBuilder();
            string line = "";
            try
            {
                HttpWebResponse response = (HttpWebResponse)githttp.GetResponse();
                StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);

                sb.Clear();
                string text = "";
                DateTime startedtime = DateTime.Now;
                while ((line = sr.ReadLine()) != null)
                {
                    sb.AppendLine(line.Remove(line.IndexOf("]")).Substring(line.IndexOf(":") + 1));
                    text += line;
                }
                int usetime = DateTime.Now.Subtract(startedtime).Milliseconds;
                if (usetime > 0)
                {
                    int speed = text.Length * sizeof(char) / usetime * 1000 / 1024;
                    if (speed < 150)
                    {
                        sr.Close();
                        response.Close();
                        throw new Exception("speed too slow");
                    }
                }

                sr.Close();
                response.Close();
            }
            catch
            {
                try
                {
                    HttpWebResponse response = (HttpWebResponse)githttp_fast.GetResponse();
                    StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);

                    sb.Clear();
                    sb.AppendLine("fastmode");
                    while ((line = sr.ReadLine()) != null)
                    {
                        sb.AppendLine(line.Remove(line.IndexOf("]")).Substring(line.IndexOf(":") + 1));
                    }

                    sr.Close();
                    response.Close();
                }
                catch
                {
                    sb.Clear();
                }
            }

            return sb.ToString().Trim();

        }

        public static bool Download(bool fastmode, string version)
        {

            string gitsite = $"https://github.com/haozi-ljh/Osiris-I18n/releases/download/{version}/Osiris-I18n_{version}.zip";
            string temppath = I18n.gamePath + "\\update.temp_";
            string ufpath = I18n.gamePath + "\\update.zip";
            HttpWebRequest githttp = (HttpWebRequest)WebRequest.Create(gitsite);
            HttpWebRequest githttp_fast = (HttpWebRequest)WebRequest.Create("https://ghproxy.com/" + gitsite);

            githttp_fast.Method = githttp.Method = "GET";
            githttp_fast.Timeout = githttp.Timeout = 3000;
            githttp_fast.KeepAlive = githttp.KeepAlive = true;

            try
            {
                HttpWebResponse response = (HttpWebResponse)(fastmode ? githttp_fast.GetResponse() : githttp.GetResponse());
                Stream stream = response.GetResponseStream();

                byte[] read = new byte[4096];
                int size = stream.Read(read, 0, 4096);
                FileStream fs = new FileStream(temppath, FileMode.Create);
                while (size > 0)
                {
                    fs.Write(read, 0, size);
                    size = stream.Read(read, 0, 4096);
                }
                fs.Flush();
                stream.Flush();
                fs.Close();
                stream.Close();

                byte[] zip = new byte[] { 0x50, 0x4b, 0x03, 0x04 };
                long templength = new FileInfo(temppath).Length;
                if (templength > zip.Length && templength == response.ContentLength)
                {
                    byte[] tempbs = new byte[zip.Length];
                    FileStream tempfs = new FileStream(temppath, FileMode.Open);
                    tempfs.Read(tempbs, 0, zip.Length);
                    tempfs.Close();

                    for (int fi = 0; fi < zip.Length; fi++)
                    {
                        if (tempbs[fi] != zip[fi])
                        {
                            File.Delete(temppath);
                            throw new Exception();
                        }
                    }
                    if (new FileInfo(ufpath).Exists)
                        File.Delete(ufpath);

                    File.Move(temppath, ufpath);
                }
                else
                {
                    File.Delete(temppath);
                    response.Close();
                    throw new Exception();
                }
                response.Close();

                return true;
            }
            catch
            {
                File.Delete(temppath);
                return false;
            }

        }

        public static bool Install()
        {
            string installpath = I18n.pluginPath;
            string inputfile = I18n.gamePath + "\\update.zip";
            string tempout = I18n.gamePath + "\\Temp";

            if (!File.Exists(inputfile))
                return false;
            while (Directory.Exists(tempout))
            {
                tempout += DateTime.Now.Millisecond;
            }
            Directory.CreateDirectory(tempout);

            ZipFile zipFile = new ZipFile(inputfile, new GB2312Encoding());
            foreach (ZipEntry zipEntry in zipFile)
            {
                try
                {
                    if (zipEntry.IsDirectory)
                    {
                        Directory.CreateDirectory(tempout + "\\" + zipEntry.FileName);
                        continue;
                    }
                    FileStream fs = new FileStream(tempout + "\\" + zipEntry.FileName, FileMode.Create);
                    CrcCalculatorStream zipEntryRead = zipEntry.OpenReader();
                    byte[] bs = new byte[4096];
                    int length = zipEntryRead.Read(bs, 0, bs.Length);
                    while (length > 0)
                    {
                        fs.Write(bs, 0, length);
                        fs.Flush();
                        length = zipEntryRead.Read(bs, 0, bs.Length);
                    }
                    fs.Close();
                    zipEntryRead.Close();
                }
                catch
                {
                    zipFile.Dispose();
                    File.Delete(inputfile);
                    Directory.Delete(tempout, true);
                    return false;
                }
            }
            zipFile.Dispose();

            string getfilepath;
            if (Directory.Exists(tempout + "\\Osiris-I18n"))
                getfilepath = tempout + "\\Osiris-I18n\\BepInEx\\plugins";
            else if (Directory.Exists(tempout + "\\BepInEx"))
                getfilepath = tempout + "\\BepInEx\\plugins";
            else
            {
                File.Delete(inputfile);
                Directory.Delete(tempout, true);
                return false;
            }

            System.Diagnostics.Process.Start("cmd.exe", $"/c del /F /Q \"{I18n.dllPath}");
            DateTime starttime = DateTime.Now;
            bool end = false;
            while (DateTime.Now.Subtract(starttime).Milliseconds < 600)
            {
                if (!File.Exists($"{I18n.dllPath}"))
                {
                    end = true;
                    break;
                }
            }
            if (!end)
            {
                return false;
            }

            CopyFiles(getfilepath, installpath);

            File.Delete(inputfile);
            Directory.Delete(tempout, true);
            return true;
        }

        private static void CopyFiles(string inpath, string outpath)
        {
            foreach (FileSystemInfo fsi in new DirectoryInfo(inpath).GetFileSystemInfos())
            {
                string namepath = fsi.FullName.Replace(inpath, "");
                if (fsi.Attributes.Equals(FileAttributes.Directory))
                {
                    if (!Directory.Exists(outpath + "\\" + namepath))
                        Directory.CreateDirectory(outpath + "\\" + namepath);
                    CopyFiles(fsi.FullName, outpath + namepath);
                    continue;
                }
                File.Copy(fsi.FullName, outpath + namepath, true);
            }
        }

    }
}
