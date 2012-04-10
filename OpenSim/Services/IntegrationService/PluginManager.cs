/*
 * Copyright (c) Contributors, http://opensimulator.org/
 * See CONTRIBUTORS.TXT for a full list of copyright holders.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 *     * Redistributions of source code must retain the above copyright
 *       notice, this list of conditions and the following disclaimer.
 *     * Redistributions in binary form must reproduce the above copyright
 *       notice, this list of conditions and the following disclaimer in the
 *       documentation and/or other materials provided with the distribution.
 *     * Neither the name of the OpenSimulator Project nor the
 *       names of its contributors may be used to endorse or promote products
 *       derived from this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE DEVELOPERS ``AS IS'' AND ANY
 * EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED. IN NO EVENT SHALL THE CONTRIBUTORS BE LIABLE FOR ANY
 * DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
 * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

using System;
using System.Text;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Mono.Addins;
using Mono.Addins.Setup;
using Mono.Addins.Description;
using OpenSim.Framework;

using Ux = OpenSim.Services.IntegrationService.IUtils;

namespace OpenSim.Services.IntegrationService
{
    // This will maintain the plugin repositories and plugins
    public class PluginManager : SetupService
    {
        protected AddinRegistry m_Registry;

        internal PluginManager( AddinRegistry r): base (r)
        {
            m_Registry = r;
            m_Registry.Update();
        }

        public string InstallPlugin(string[] args)
        {
            PackageCollection pack = new PackageCollection();
            PackageCollection toUninstall;
            DependencyCollection unresolved;

            IProgressStatus ps = new ConsoleProgressStatus(false);

            // m_Registry.Update(ps);

            string name = Addin.GetIdName(args[1]);
            string version = Addin.GetIdVersion(args[1]);


            AddinRepositoryEntry[] aentry = Repositories.GetAvailableAddin(name, version);

            foreach (AddinRepositoryEntry ae in aentry)
            {
                Package p = Package.FromRepository(ae);
                pack.Add(p);
            }

            ResolveDependencies(ps, pack, out toUninstall, out unresolved);

            if(Install(ps, pack) == true)
            {
                m_Registry.Update(ps);
                return "Install";
            }
            else
            {
                return "Bomb";
            }
        }

        // Remove plugin
        public void UnInstall(string[] args)
        {
            Addin[] addins = GetSortedAddinList("IntegrationPlugin");

            int n = Convert.ToInt16(args[1]);
            if (n > (addins.Length -1))
            {
                MainConsole.Instance.Output("Selection out of range");
                return;
            }

            Addin addin = addins[n];
            MainConsole.Instance.OutputFormat("Uninstalling plugin {0}", addin.Id);
            AddinManager.Registry.DisableAddin(addin.Id);
            addin.Enabled = false;
            IProgressStatus ps = new ConsoleProgressStatus(false);
            Uninstall(ps, addin.Id);
            return;
        }

        public string CheckInstalled()
        {
            return "CheckInstall";
        }

        // List instaled addins
        public void ListInstalledAddins()
        {
            Addin[] addins = GetSortedAddinList("IntegrationPlugin");

            MainConsole.Instance.Output("Installed Plugins");

            int count = 0;
            foreach (Addin addin in addins)
            {
                MainConsole.Instance.OutputFormat("{0}) {1} {2} rev. {3}", count.ToString(),
                                                      addin.Enabled == false ? "[X]" : "[ ]",
                                                      addin.Name, addin.Version );
                count++;
            }
            return;
        }

        // List compatible plugins in registered repositories
        public ArrayList ListAvailable()
        {
            AddinRepositoryEntry[] addins = Repositories.GetAvailableAddins ();
            ArrayList list = new ArrayList();

            foreach (AddinRepositoryEntry addin in addins)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(String.Format("{0} rev. {1}, repo {2}", addin.Addin.Id, addin.Addin.Version, addin.RepositoryUrl));
                list.Add(sb.ToString());
            }
            return list;
        }

        // List available updates
        public void ListUpdates()
        {
            IProgressStatus ps = new ConsoleProgressStatus(true);
            Console.WriteLine ("Looking for updates...");
            Repositories.UpdateAllRepositories (ps);
            Console.WriteLine ("Available add-in updates:");
            bool found = false;
            AddinRepositoryEntry[] entries = Repositories.GetAvailableUpdates ();

            foreach (AddinRepositoryEntry entry in entries)
            {
                Console.WriteLine(String.Format("{0}",entry.Addin.Id));
            }
        }

        // Sync to repositories
        public string Update()
        {
            IProgressStatus ps = new ConsoleProgressStatus(true);
            Repositories.UpdateAllRepositories (ps);
            return "Update";
        }

        // Register a repository
        public string AddRepository(string[] args)
        {
            Repositories.RegisterRepository(null, args[2].ToString(), true);
            return "AddRepository";
        }

        public void GetRepository()
        {
            Repositories.UpdateAllRepositories (new ConsoleProgressStatus (false));
        }

        // Remove a repository from the list
        public void RemoveRepository(string[] args)
        {
            AddinRepository[] reps = Repositories.GetRepositories();
            Array.Sort (reps, (r1,r2) => r1.Title.CompareTo(r2.Title));
            if (reps.Length == 0)
            {
                MainConsole.Instance.Output("No repositories have been registered.");
                return;
            }

            int n = Convert.ToInt16(args[2]);
            if (n > (reps.Length -1))
            {
                MainConsole.Instance.Output("Selection out of range");
                return;
            }

            AddinRepository rep = reps[n];
            Repositories.RemoveRepository (rep.Url);
            return;
        }

        // Enable repository
        public void EnableRepository(string[] args)
        {
            AddinRepository[] reps = Repositories.GetRepositories();
            Array.Sort (reps, (r1,r2) => r1.Title.CompareTo(r2.Title));
            if (reps.Length == 0)
            {
                MainConsole.Instance.Output("No repositories have been registered.");
                return;
            }

            int n = Convert.ToInt16(args[2]);
            if (n > (reps.Length -1))
            {
                MainConsole.Instance.Output("Selection out of range");
                return;
            }

            AddinRepository rep = reps[n];
            Repositories.SetRepositoryEnabled(rep.Url, true);
            return;
        }

        // Disable a repository
        public void DisableRepository(string[] args)
        {
            AddinRepository[] reps = Repositories.GetRepositories();
            Array.Sort (reps, (r1,r2) => r1.Title.CompareTo(r2.Title));
            if (reps.Length == 0)
            {
                MainConsole.Instance.Output("No repositories have been registered.");
                return;
            }

            int n = Convert.ToInt16(args[2]);
            if (n > (reps.Length -1))
            {
                MainConsole.Instance.Output("Selection out of range");
                return;
            }

            AddinRepository rep = reps[n];
            Repositories.SetRepositoryEnabled(rep.Url, false);
            return;
        }

        // List registered repositories
        public ArrayList ListRepositories()
        {
            AddinRepository[] reps = Repositories.GetRepositories();
            Array.Sort (reps, (r1,r2) => r1.Title.CompareTo(r2.Title));
            if (reps.Length == 0)
            {
                MainConsole.Instance.Output("No repositories have been registered.");
                return null;
            }

            ArrayList list = new ArrayList();

            int n = 0;
            foreach (AddinRepository rep in reps)
            {
                list.Add(String.Format("{0}) {1} {2} {3}",n.ToString(), rep.Enabled == true ? "[ ]" : "[X]", rep.Name, rep.Url));
                n++;
            }
            return list;
        }

        public void UpdateRegistry()
        {
            m_Registry.Update();
        }

        // Show plugin info
        public string AddinInfo(string[] args)
        {
            Addin[] addins = GetSortedAddinList("IntegrationPlugin");

            int n = Convert.ToInt16(args[2]);
            if (n > (addins.Length -1))
            {
                MainConsole.Instance.Output("Selection out of range");
                return "XXX";
            }

            Addin addin = addins[n];
            MainConsole.Instance.OutputFormat("Name: {0}\nURL: {1}\n{2}",
                                              addin.Name, addin.Description.Url,
                                              addin.Description.FileName);

            return "AddinInfo";
        }

        // Disable a plugin
        public void DisablePlugin(string[] args)
        {
            Addin[] addins = GetSortedAddinList("IntegrationPlugin");

            int n = Convert.ToInt16(args[2]);
            if (n > (addins.Length -1))
            {
                MainConsole.Instance.Output("Selection out of range");
                return;
            }

            Addin addin = addins[n];
            AddinManager.Registry.DisableAddin(addin.Id);
            addin.Enabled = false;
            return;
        }

        // Enable plugin
        public void EnablePlugin(string[] args)
        {
            Addin[] addins = GetSortedAddinList("IntegrationPlugin");

            int n = Convert.ToInt16(args[2]);
            if (n > (addins.Length -1))
            {
                MainConsole.Instance.Output("Selection out of range");
                return;
            }

            Addin addin = addins[n];

            addin.Enabled = true;
            AddinManager.Registry.EnableAddin(addin.Id);
            // AddinManager.Registry.Update();
            if(m_Registry.IsAddinEnabled(addin.Id))
            {
                ConsoleProgressStatus ps = new ConsoleProgressStatus(false);
                if (!AddinManager.AddinEngine.IsAddinLoaded(addin.Id))
                {
                    AddinManager.Registry.Rebuild(ps);
                    AddinManager.AddinEngine.LoadAddin(ps, addin.Id);
                }
            }
            else
            {
                MainConsole.Instance.OutputFormat("Not Enabled in this domain {0}", addin.Name);
            }
            return;
        }

        #region Util
        private void Testing()
        {
            Addin[] list = Registry.GetAddins();

            var addins = list.Where( a => a.Description.Category == "IntegrationPlugin");

            foreach (Addin addin in addins)
            {
                MainConsole.Instance.OutputFormat("Addin {0}", addin.Name);
            }
        }

        private Addin[] GetSortedAddinList(string category)
        {
            ArrayList list = new ArrayList();
            list.AddRange(m_Registry.GetAddins());
            ArrayList xlist = new ArrayList();



            foreach (Addin addin in list)
            {
                if (addin.Description.Category == category)
                    xlist.Add(addin);
            }

            Addin[] addins = xlist.ToArray(typeof(Addin)) as Addin[];
            Array.Sort(addins,(r1,r2) => r1.Id.CompareTo(r2.Id));

            return addins;
        }
        #endregion Util
    }
}