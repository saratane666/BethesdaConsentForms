﻿using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Web;
using System.Web.Configuration;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.Win32.TaskScheduler;
using WindowsCEConsentForms.ConsentFormSvc;
using Configuration = System.Configuration.Configuration;

namespace WindowsCEConsentForms.Administration
{
    public partial class Setup : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //string checkIfExist = ConfigurationManager.AppSettings["DBSetupStatus"];
            //if (checkIfExist == "1")
            //{
            //    PnlDBConfiguration.Visible = false;
            //}
            try
            {
                if (!IsPostBack)
                {
                    RdoSqlServerAuthentication.Checked = true;
                    RdoSqlServerAuthenticationExternal.Checked = true;
                    Reset();
                }
            }
            catch (Exception ex)
            {
                var client = Utilities.GetConsentFormSvcClient();
                client.CreateLog(Utilities.GetUsername(Session), LogType.E, GetType().Name + "-" + new StackTrace().GetFrame(0).GetMethod().ToString(),
                                 ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }

        protected void BtnReset_Click(object sender, EventArgs e)
        {
            Reset();
        }

        private void Reset()
        {
            try
            {
                TxtDatabasename.Text = string.Empty;
                TxtServerName.Text = string.Empty;
                TxtUsername.Text = string.Empty;
                TxtPassword.Text = string.Empty;

                TxtDatabasenameExternal.Text = string.Empty;
                TxtServerNameExternal.Text = string.Empty;
                TxtUsernameExternal.Text = string.Empty;
                TxtPasswordExternal.Text = string.Empty;

                TxtServerName.Focus();

                // loading WCF Service URL from web.config
                Configuration config =
                    WebConfigurationManager.OpenWebConfiguration(HttpContext.Current.Request.ApplicationPath);
                TxtServiceURL.Text = config.AppSettings.Settings["ServiceURL"].Value;

                // Getting export paths and display in boxes
                var endpoint = new EndpointAddress(new Uri(TxtServiceURL.Text.Trim()));
                var formHandlerServices = new ConsentFormSvcClient(new BasicHttpBinding(), endpoint);

                // setting Database fields value from connection string
                try
                {
                    var sqlConnectionStringBuilder = new SqlConnectionStringBuilder(ConfigurationManager.AppSettings["ConnectionString"]);
                    TxtServerName.Text = sqlConnectionStringBuilder.DataSource;
                    TxtDatabasename.Text = sqlConnectionStringBuilder.InitialCatalog;
                    if (!string.IsNullOrEmpty(sqlConnectionStringBuilder.UserID))
                    {
                        RdoSqlServerAuthentication.Checked = true;
                        TxtUsername.Text = sqlConnectionStringBuilder.UserID;
                        TxtPassword.Text = sqlConnectionStringBuilder.Password;
                    }
                    else
                    {
                        RdoWindowsAuthentication.Checked = true;
                    }
                }
                catch (Exception)
                {
                }

                // setting Database fields value from connection string
                try
                {
                    var sqlConnectionStringBuilder = new SqlConnectionStringBuilder(ConfigurationManager.AppSettings["BethesdaConnectionString"]);
                    TxtServerNameExternal.Text = sqlConnectionStringBuilder.DataSource;
                    TxtDatabasenameExternal.Text = sqlConnectionStringBuilder.InitialCatalog;
                    if (!string.IsNullOrEmpty(sqlConnectionStringBuilder.UserID))
                    {
                        RdoSqlServerAuthenticationExternal.Checked = true;
                        TxtUsernameExternal.Text = sqlConnectionStringBuilder.UserID;
                        TxtPasswordExternal.Text = sqlConnectionStringBuilder.Password;
                    }
                    else
                    {
                        RdoWindowsAuthentication.Checked = true;
                    }
                }
                catch (Exception)
                {
                }

                SetCredentialPanel();
            }
            catch (Exception ex)
            {
                var client = Utilities.GetConsentFormSvcClient();
                client.CreateLog(Utilities.GetUsername(Session), LogType.E, GetType().Name + "-" + new StackTrace().GetFrame(0).GetMethod().ToString(),
                                 ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }

        protected void BtnCompleted_Click(object sender, EventArgs e)
        {
            try
            {
                LblError.Text = string.Empty;
                Configuration config = WebConfigurationManager.OpenWebConfiguration(HttpContext.Current.Request.ApplicationPath);

                if (PnlDBConfiguration.Visible) // right now it is always true
                {
                    if (!string.IsNullOrEmpty(TxtServiceURL.Text))
                    {
                        // testing the given WCF url with a sample method
                        var endpoint = new EndpointAddress(new Uri(TxtServiceURL.Text.Trim()));
                        var consentFormSvcClient = new ConsentFormSvcClient(new BasicHttpBinding(), endpoint);

                        // it may throw error if configuration is wrong
                        config.AppSettings.Settings.Remove("ServiceURL");
                        config.AppSettings.Settings.Add("ServiceURL", TxtServiceURL.Text.Trim());
                        config.Save(ConfigurationSaveMode.Modified);
                        ConfigurationManager.RefreshSection("appSettings");

                        LblError.Text += "<br /> WCF Service URL configured successfully";

                        try
                        {
                            // calling an sample method to validate WCF service and database
                            //consentFormSvcClient.IsValidEmployee("test");
                        }
                        catch (Exception ex)
                        {
                            LblError.Text += "Please input valid wcf service URL and then try.";
                            return; // return the function if the wcf is not able to connect
                        }

                        try
                        {
                            if (!string.IsNullOrEmpty(TxtServerName.Text.Trim()) &&
                                !string.IsNullOrEmpty(TxtDatabasename.Text.Trim()) ||
                                (RdoSqlServerAuthentication.Checked && !string.IsNullOrEmpty(TxtUsername.Text.Trim()) &&
                                 !string.IsNullOrEmpty(TxtPassword.Text.Trim())))
                            {
                                //string checkIfExist = ConfigurationManager.AppSettings["DBSetupStatus"];
                                //if (checkIfExist == "0")
                                //{
                                string connectionString;
                                if (RdoSqlServerAuthentication.Checked)
                                    connectionString = @"server=" + TxtServerName.Text.Trim() + ";database=" + TxtDatabasename.Text + ";uid=" + TxtUsername.Text.Trim() + ";pwd=" + TxtPassword.Text.Trim();
                                else
                                    connectionString = "Server=" + TxtServerName.Text.Trim() + ";Database=" + TxtDatabasename.Text.Trim() + ";Trusted_Connection=True;";

                                config.AppSettings.Settings.Remove("ConnectionString");
                                config.AppSettings.Settings.Add("ConnectionString", connectionString);
                                config.Save(ConfigurationSaveMode.Modified);
                                ConfigurationManager.RefreshSection("appSettings");

                                LblError.Text += "<br /> Database Connection string saved successfully";

                                config.AppSettings.Settings.Remove("DBSetupStatus");
                                config.AppSettings.Settings.Add("DBSetupStatus", "1");

                                string masterConnectionString = connectionString.Replace(connectionString.Split('=')[2], "master;uid");

                                // Setting connection in WCF application for configured DB connection
                                consentFormSvcClient.SetDbConnection(connectionString);
                                LblError.Text += "<br /> Database connection string configured in WCF successfully";

                                // start configuring WCF Service URL
                                try
                                {
                                    // create database if not exists
                                    if (CreateDatabase(TxtDatabasename.Text.Trim(), masterConnectionString))
                                    {
                                        LblError.Text += "<br /> Database Created succefully";
                                    }
                                }
                                catch (Exception ex)
                                {
                                    LblError.Text += "<br /> Unable to create database due to [" + ex.Message + "]";
                                }
                            }
                            else
                                LblError.Text += "<br /> Please input required fields and submit.";
                        }
                        catch (Exception ex)
                        {
                            LblError.Text += "<br /> Unable to save settings due to [" + ex.Message + "]";
                        }

                        try
                        {
                            if (!string.IsNullOrEmpty(TxtServerNameExternal.Text.Trim()) &&
                                !string.IsNullOrEmpty(TxtDatabasenameExternal.Text.Trim()) ||

                                (RdoSqlServerAuthenticationExternal.Checked &&
                                 !string.IsNullOrEmpty(TxtUsernameExternal.Text.Trim()) &&
                                 !string.IsNullOrEmpty(TxtPasswordExternal.Text.Trim())))
                            {
                                string connectionString;
                                if (RdoSqlServerAuthenticationExternal.Checked)
                                    connectionString = @"server=" + TxtServerNameExternal.Text.Trim() +
                                                       ";database=" + TxtDatabasename.Text.Trim() + ";uid=" +
                                                       TxtUsernameExternal.Text.Trim() + ";pwd=" +
                                                       TxtPasswordExternal.Text.Trim();
                                else
                                    connectionString = "Server=" + TxtServerNameExternal.Text.Trim() + ";Database=" +
                                                       TxtDatabasenameExternal.Text.Trim() + ";Trusted_Connection=True;";
                                config.AppSettings.Settings.Remove("BethesdaConnectionString");
                                config.AppSettings.Settings.Add("BethesdaConnectionString", connectionString);
                                config.Save(ConfigurationSaveMode.Modified);
                                ConfigurationManager.RefreshSection("appSettings");

                                consentFormSvcClient.SetBethesdaDbConnection(connectionString);
                            }
                            else
                                LblError.Text += "Please input required fields and submit.";
                        }
                        catch (Exception ex)
                        {
                            LblError.Text += "Unable to save external database settings due to [" + ex.Message + "]";
                        }
                    }
                    else
                        LblError.Text += "<br /> Please input wcf service URL and try again.";

                    try
                    {
                        // Get the service on the local machine
                        using (var ts = new TaskService())
                        {
                            if (ts.RootFolder.GetTasks().Any(task => task.Name == "BethedaContentSync"))
                            {
                                ts.RootFolder.DeleteTask("BethedaContentSync");
                            }

                            // Create a new task definition and assign properties
                            TaskDefinition td = ts.NewTask();
                            td.RegistrationInfo.Description = "Bethesda Employee, Patient and Physician import task.";

                            // Create a trigger that will fire the task at this time every day
                            td.Triggers.Add(new DailyTrigger { StartBoundary = DateTime.Now.AddHours(-DateTime.Now.Hour), DaysInterval = 1, Enabled = true });

                            string path = @"C:\Program Files\Internet Explorer\iexplore.exe";

                            // Create an action that will launch Notepad whenever the trigger fires
                            td.Actions.Add(new ExecAction(Request.Url.OriginalString.Replace(Request.Url.Query, string.Empty) + "/Administration/", "", null));

                            // Register the task in the root folder
                            ts.RootFolder.RegisterTaskDefinition(@"BethedaContentSync", td);
                        }
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            var client = Utilities.GetConsentFormSvcClient();
                            client.CreateLog(Utilities.GetUsername(Session), LogType.E, GetType().Name + "-" + new StackTrace().GetFrame(0).GetMethod().ToString(),
                                             ex.Message + Environment.NewLine + ex.StackTrace);
                        }
                        catch (Exception)
                        {
                        }
                    }
                }

                try
                {
                    // Get the service on the local machine
                    using (var ts = new TaskService())
                    {
                        if (ts.RootFolder.GetTasks().Any(task => task.Name == "BethedaContentSync"))
                        {
                            ts.RootFolder.DeleteTask("BethedaContentSync");
                        }

                        // Create a new task definition and assign properties
                        TaskDefinition td = ts.NewTask();
                        td.RegistrationInfo.Description = "Bethesda Employee, Patient and Physician import task.";

                        // Create a trigger that will fire the task at this time every day
                        td.Triggers.Add(new DailyTrigger { StartBoundary = DateTime.Now.AddHours(-DateTime.Now.Hour), DaysInterval = 1, Enabled = true });

                        // Create an action that will launch Notepad whenever the trigger fires
                        td.Actions.Add(new ExecAction(Request.Url.Host, "", null));

                        // Register the task in the root folder
                        ts.RootFolder.RegisterTaskDefinition(@"BethedaContentSync", td);
                    }
                }
                catch (Exception ex)
                {
                    try
                    {
                        var client = Utilities.GetConsentFormSvcClient();
                        client.CreateLog(Utilities.GetUsername(Session), LogType.E, GetType().Name + "-" + new StackTrace().GetFrame(0).GetMethod().ToString(),
                                         ex.Message + Environment.NewLine + ex.StackTrace);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                try
                {
                    var client = Utilities.GetConsentFormSvcClient();
                    client.CreateLog(Utilities.GetUsername(Session), LogType.E, GetType().Name + "-" + new StackTrace().GetFrame(0).GetMethod().ToString(),
                                     ex.Message + Environment.NewLine + ex.StackTrace);
                }
                catch (Exception)
                {
                }
            }
        }

        public bool CreateDatabase(string databaseName, string connectionString)
        {
            try
            {
                using (var connData = new SqlConnection(connectionString))
                {
                    if (File.Exists(Server.MapPath("/Administration/ConsentDB.sql")))
                    {
                        var file = new FileInfo(Server.MapPath("/Administration/ConsentDB.sql"));
                        string script = file.OpenText().ReadToEnd();
                        var streamReader = new StreamReader(Server.MapPath("/Administration/ConsentDB.sql"));
                        while (!streamReader.EndOfStream)
                        {
                            var line = streamReader.ReadLine();
                            if (line != null && line.Split(' ')[0].ToLower() == "use")
                            {
                                script = script.Replace(line.Split('[')[1], databaseName + "]");
                                break;
                            }
                        }
                        streamReader.Dispose();
                        var server = new Server(new ServerConnection(connData));
                        server.ConnectionContext.ExecuteNonQuery("create database [" + databaseName + "]");
                        server.ConnectionContext.ExecuteNonQuery(script);
                        file.OpenText().Close();
                        connData.Close();
                        return true;
                    }
                    else
                    {
                        LblError.Text += " <br /> Not able to create database due to SQL script file not found.";
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                var client = Utilities.GetConsentFormSvcClient();
                client.CreateLog(Utilities.GetUsername(Session), LogType.E, GetType().Name + "-" + new StackTrace().GetFrame(0).GetMethod().ToString(),
                                 ex.Message + Environment.NewLine + ex.StackTrace);
                throw ex;
            }
        }

        protected void RdoSqlServerAuthentication_CheckedChanged(object sender, EventArgs e)
        {
            SetCredentialPanel();
        }

        protected void RdoWindowsAuthentication_CheckedChanged(object sender, EventArgs e)
        {
            SetCredentialPanel();
        }

        protected void RdoWindowsAuthenticationExternal_CheckedChanged(object sender, EventArgs e)
        {
            SetCredentialPanelExternal();
        }

        private void SetCredentialPanel()
        {
            PnlCredentials1.Visible = RdoSqlServerAuthentication.Checked;
            PnlCredentials2.Visible = RdoSqlServerAuthentication.Checked;
        }

        private void SetCredentialPanelExternal()
        {
            PnlCredentialsExternal1.Visible = RdoSqlServerAuthenticationExternal.Checked;
            PnlCredentialsExternal2.Visible = RdoSqlServerAuthenticationExternal.Checked;
        }

        private bool CheckConnectionString(string connectionString, string errorText)
        {
            try
            {
                using (var connection = new SqlConnection())
                {
                    connection.Open();
                }
                return true;
            }
            catch (Exception)
            {
                LblError.Text = errorText;
                return false;
            }
        }
    }
} ;