using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSP.WebPinger.Common
{
    public class SettingsManager : IDisposable
    {
        private Common.Settings _dsSettings = null;
        private string _settingsFilePath = null;

        private SettingsManager(string filePath)
        {
            this._dsSettings = new Settings();

            if (System.IO.File.Exists(filePath))
            {
                this._dsSettings.Websites.ReadXml(filePath);
            }
            this._settingsFilePath = filePath;
        }

        private static SettingsManager _SettingsManager = null;

        public static SettingsManager Instance()
        {
            string filePath = GetSettingFilePath();

            if (_SettingsManager == null)
            {
                _SettingsManager = new SettingsManager(filePath);
            }

            return _SettingsManager;
        }

        private static string GetSettingFilePath()
        {
            string filePath = string.Empty;

            filePath = System.IO.Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CSPSolutions");

            if (!System.IO.Directory.Exists(filePath))
            {
                System.IO.Directory.CreateDirectory(filePath);
            }

            filePath = System.IO.Path.Combine(filePath, "csp_pinger_settings.xml");

            return filePath;
        }


        public List<string> GetWebsites()
        {
            var query = from p in this._dsSettings.Websites

                        select p.Url;

            return query.ToList();
        }

        public void AddWebsite(string url)
        {
            var query = from p in this._dsSettings.Websites
                        where (string.Compare(p.Url, url, true) == 0)
                        select p;

            if (query.Count() == 0)
            {
                this._dsSettings.Websites.AddWebsitesRow(url);
            }

            this.SaveChanges();
        }

        public void RemoveWebsite(string url)
        {
            var query = from p in this._dsSettings.Websites
                        where (string.Compare(p.Url, url, true) == 0)
                        select p;
            var row = query.SingleOrDefault();

            if (row != null)
            {
                this._dsSettings.Websites.Rows.Remove(row);
            }

            this.SaveChanges();
        }

        public void SaveChanges()
        {
            this._dsSettings.AcceptChanges();

            this._dsSettings.WriteXml(this._settingsFilePath);
        }

        public void Dispose()
        {
            SaveChanges();
        }

        internal void Clear()
        {
            this._dsSettings.Websites.Clear();

            SaveChanges();
        }
    }
}
