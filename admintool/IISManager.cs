using Microsoft.Web.Administration;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace admintool
{
    public class IISManager
    {
        public class SiteInfo
        {
            public string Name { get; set; }
            public string State { get; set; }
        }

        public static List<SiteInfo> GetListOfSites()
        {
            List<SiteInfo> sites = new List<SiteInfo>();

            try
            {
                using (ServerManager serverManager = new ServerManager())
                {
                    foreach (Site site in serverManager.Sites)
                    {
                        SiteInfo siteInfo = new SiteInfo
                        {
                            Name = site.Name,
                            State = site.State.ToString()
                        };

                        sites.Add(siteInfo);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при получении списка сайтов: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return sites;
        }

        public static void StartSite(string siteName)
        {
            using (ServerManager serverManager = new ServerManager())
            {
                Site site = serverManager.Sites[siteName];

                if (site != null)
                {
                    if (site.State == ObjectState.Stopped)
                    {
                        site.Start();
                        serverManager.CommitChanges();
                    }
                }
            }
        }

        public static void StopSite(string siteName)
        {
            using (ServerManager serverManager = new ServerManager())
            {
                Site site = serverManager.Sites[siteName];

                if (site != null)
                {
                    if (site.State == ObjectState.Started)
                    {
                        site.Stop();
                        serverManager.CommitChanges();
                    }
                }
                else
                {
                    MessageBox.Show($"Сайт с именем '{siteName}' не найден.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public static void CreateWebsite(string siteName, string physicalPath, int port)
        {
            try
            {
                using (ServerManager serverManager = new ServerManager())
                {
                    Site site = serverManager.Sites.Add(siteName, "http", $"*:{port}:", physicalPath);
                    serverManager.CommitChanges();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении сайта: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void DeleteWebsite(string siteName)
        {
            try
            {
                using (ServerManager serverManager = new ServerManager())
                {
                    Site site = serverManager.Sites[siteName];
                    if (site != null)
                    {
                        serverManager.Sites.Remove(site);
                        serverManager.CommitChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении сайта: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void ModifyWebsite(string currentSiteName, string newSiteName, string newPhysicalPath, int newPort)
        {
            try
            {
                using (ServerManager serverManager = new ServerManager())
                {
                    Site site = serverManager.Sites[currentSiteName];

                    if (site != null)
                    {
                        site.Name = newSiteName;
                        site.Applications[0].VirtualDirectories[0].PhysicalPath = newPhysicalPath;
                        site.Bindings[0].EndPoint.Port = newPort;
                        serverManager.CommitChanges();
                    }
                    else
                    {
                        MessageBox.Show($"Сайт с именем '{currentSiteName}' не найден.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при изменении сайта: {ex.Message}","Ошибка",MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
