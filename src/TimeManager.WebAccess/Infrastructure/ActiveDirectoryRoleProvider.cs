namespace Infocom.TimeManager.WebAccess.Infrastructure
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.DirectoryServices;
    using System.Web.Configuration;
    using System.Web.Security;

    public class ActiveDirectoryRoleProvider : RoleProvider
    {
        #region Constants and Fields

        private string _connectionString = string.Empty;

        private string _userName = string.Empty;

        private string _userPassword = string.Empty;

        #endregion

        #region Constructors and Destructors

        public ActiveDirectoryRoleProvider()
        {
            this.ApplicationName = string.Empty;
        }

        #endregion

        #region Properties

        public override string ApplicationName { get; set; }

        #endregion

        #region Public Methods

        public override void Initialize(string name, NameValueCollection config)
        {
            this._connectionString = config["connectionStringName"];

            if (!string.IsNullOrEmpty(config["applicationName"]))
            {
                this.ApplicationName = config["applicationName"];
            }

            if (!string.IsNullOrEmpty(config["connectionUsername"]))
            {
                this._userName = config["connectionUsername"];
            }

            if (!string.IsNullOrEmpty(config["connectionPassword"]))
            {
                this._userPassword = config["connectionPassword"];
            }

            base.Initialize(name, config);
        }

        public override string[] GetRolesForUser(string userName)
        {
            userName = this.RemoveADGroup(userName);
            return this.GetUserRoles(userName);
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        public override string[] GetAllRoles()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        public override string[] GetUsersInRole(string roleName)
        {
            return this.GetGroupMembers(this.RemoveADGroup(roleName));
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            string[] ary = this.GetRolesForUser(username);
            foreach (string s in ary)
            {
                if (roleName.ToLower() == s.ToLower())
                {
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region Methods

        private string RemoveADGroup(string name)
        {
            string[] ary = name.Split(new[] { '\\' });
            return ary[ary.Length - 1];
        }

        private string[] GetUserRoles(string userName)
        {
            DirectoryEntry obEntry =
                new DirectoryEntry(
                    WebConfigurationManager.ConnectionStrings[this._connectionString].ConnectionString, 
                    this._userName, 
                    this._userPassword);
            DirectorySearcher srch = new DirectorySearcher(obEntry, "(sAMAccountName=" + userName + ")");
            SearchResult res = srch.FindOne();

            Dictionary<string, string> dictionary = new Dictionary<string, string>();

            if (null != res)
            {
                DirectoryEntry obUser = new DirectoryEntry(res.Path, this._userName, this._userPassword);

                string rootPath = WebConfigurationManager.ConnectionStrings[this._connectionString].ConnectionString;
                rootPath = rootPath.Substring(0, rootPath.LastIndexOf(@"/") + 1);

                this.GetMemberships(obUser, dictionary, rootPath);
            }

            string[] ary = new string[dictionary.Count];
            dictionary.Values.CopyTo(ary, 0);
            return ary;
        }

        private void GetMemberships(DirectoryEntry entry, Dictionary<string, string> dictionary, string rootPath)
        {
            List<DirectoryEntry> childrenToCheck = new List<DirectoryEntry>();
            PropertyValueCollection children = entry.Properties["memberOf"];
            foreach (string childDN in children)
            {
                if (!dictionary.ContainsKey(childDN))
                {
                    DirectoryEntry obGpEntry = new DirectoryEntry(
                        rootPath + childDN, this._userName, this._userPassword);
                    string groupName = obGpEntry.Properties["sAMAccountName"].Value.ToString();

                    dictionary.Add(childDN, groupName);
                    childrenToCheck.Add(obGpEntry);
                }
            }

            foreach (DirectoryEntry child in childrenToCheck)
            {
                this.GetMemberships(child, dictionary, rootPath);
            }
        }

        private string[] GetGroupMembers(string groupName)
        {
            DirectoryEntry obEntry =
                new DirectoryEntry(
                    WebConfigurationManager.ConnectionStrings[this._connectionString].ConnectionString, 
                    this._userName, 
                    this._userPassword);
            DirectorySearcher srch = new DirectorySearcher(obEntry, "(sAMAccountName=" + groupName + ")");
            SearchResult res = srch.FindOne();

            Dictionary<string, string> groups = new Dictionary<string, string>();
            Dictionary<string, string> members = new Dictionary<string, string>();

            if (null != res)
            {
                DirectoryEntry entry = new DirectoryEntry(res.Path, this._userName, this._userPassword);

                this.GetMembers(entry, groups, members);
            }

            string[] ary = new string[members.Count];
            members.Values.CopyTo(ary, 0);
            return ary;
        }

        private void GetMembers(
            DirectoryEntry entry, Dictionary<string, string> groups, Dictionary<string, string> members)
        {
            List<DirectoryEntry> childrenToCheck = new List<DirectoryEntry>();
            object children = entry.Invoke("Members", null);

            foreach (object childObject in (IEnumerable)children)
            {
                DirectoryEntry child = new DirectoryEntry(childObject);
                string type = this.getEntryType(child);
                if (type == "G" && !groups.ContainsKey(child.Path))
                {
                    childrenToCheck.Add(child);
                }
                else if (type == "P" && !members.ContainsKey(child.Path))
                {
                    members.Add(child.Path, child.Properties["sAMAccountName"].Value.ToString());
                }
            }

            foreach (DirectoryEntry child in childrenToCheck)
            {
                this.GetMembers(child, groups, members);
            }
        }

        private string getEntryType(DirectoryEntry inEntry)
        {
            if (inEntry.Properties.Contains("objectCategory"))
            {
                string fullValue = inEntry.Properties["objectCategory"].Value.ToString();
                if (fullValue.StartsWith("CN=Group"))
                {
                    return "G";
                }
                else if (fullValue.StartsWith("CN=Person"))
                {
                    return "P";
                }
            }

            return string.Empty;
        }

        #endregion
    }
}