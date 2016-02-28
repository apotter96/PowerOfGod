﻿using System;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using NetBible.Books;
using Power_of_God.BibPlan;
using Power_of_God.pSystem;
using Power_of_God.User;
using Power_of_God_Lib.pSystem;
using Power_of_God_Lib.Plugins;
using PurposeVerses = Power_of_God.pSystem.PurposeVerses;
using Newtonsoft.Json;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Power_of_God_Lib.pSystem.DialogBox;

namespace Power_of_God
{
    
    public partial class MainForm : Form
    {
        public MainForm()
        {
            if (!Directory.Exists("power.of.god"))
            {
                Directory.CreateDirectory("power.of.god");
            }
            InitializeComponent();
            Text = "Power of God " + Updater.LatestStable();
            SetEventHandlers();
            //const string settingsFile = "power.of.god/settings.json";
            Settings.LoadDefault();
            Settings.LoadFromJson();
            SetDefaultContent();
            if (Updater.UpdateWord().ToLower() != "updated")
            {
                DialogBox dBox = new DialogBox("Update Notice for Power of God", Updater.UpdateNotice());
                dBox.Show();
            }
            Bible.SetLocation("power.of.god/" + Settings.UserSettings.scriptver + ".xml");
            UpdatePluginPanel();
        }
        
        private void UpdatePluginPanel()
        {
            PluginReader.LoadPlugins();
            foreach (var pl in PluginReader.PluginList)
            {
                var myButton = new Power_of_God_Lib.Plugins.Button(pl);
                myButton.SetText(pl.Name);
                myButton.Width = flowLayoutPanel1.Width - 5;
                myButton.Click += myButtonClick;
                flowLayoutPanel1.Controls.Add(myButton);
            }
        }

        private void myButtonClick(object sender, EventArgs e)
        {
            var btnObject = (Power_of_God_Lib.Plugins.Button) sender;
            MessageBox.Show(btnObject.Text);
        }

        private void SetEventHandlers()
        {
            webBrowser1.Navigated += ChangedTitle;
            Parser.PlanDays.CollectionChanged += CheckChanged;
            headerpanel.MouseDown += panel1_MouseDown;
            headerpanel.MouseUp += panel1_MouseUp;
            headerpanel.MouseMove += panel1_MouseMove;
            picMain.MouseDown += panel1_MouseDown;
            picMain.MouseUp += panel1_MouseUp;
            picMain.MouseMove += panel1_MouseMove;
            UpdateWeb.XBrowser.Navigated += ActivateUrl;
        }

        private void ActivateUrl(object sender, WebBrowserNavigatedEventArgs e)
        {
            webBrowser1.ScriptErrorsSuppressed = true;
            try
            {
                webBrowser1.DocumentText = UpdateWeb.XBrowser.DocumentText;
            }
            catch (COMException)
            {
                //webBrowser1.Navigate(UpdateWeb.XBrowser.Url);
            }
            
        }

        //Global variables;
        private bool _dragging;
        //private Point _offset;
        private Point _startPoint = new Point(0, 0);


        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            _dragging = true;  // _dragging is your variable flag
            _startPoint = new Point(e.X, e.Y);
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            _dragging = false;
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_dragging) return;
            var p = PointToScreen(e.Location);
            Location = new Point(p.X - _startPoint.X, p.Y - _startPoint.Y);
        }
        private void CheckChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            lboListOfItems.Items.Clear();
            foreach (var x in Parser.PlanDays)
            {
                lboListOfItems.Items.Add(x);
            }
        }

        private void ChangedTitle(object sender, WebBrowserNavigatedEventArgs e)
        {
            lblName.Text = webBrowser1.DocumentTitle;
        }

        public override sealed string Text
        {
            get { return base.Text; }
            set { base.Text = value; }
        }

        private bool _isTwice;
        private void MainForm_Load(object sender, EventArgs e)
        {
            if (Updater.LatestStable() != "Alpha 1.5")
            {
                picMain.Load("http://godispower.us/images/main.png");
            }
            var x = PluginReader.PluginList;
            foreach (var c in x)
            {
                PluginReader.AppLoad(c);
            }
        }

        private void SetRichText(RichTextMode rt)
        {
            switch (rt)
            {
                case RichTextMode.Purpose:
                    SetDefaultContent();
                    break;
            }
        }

        private void SetDefaultContent()
        {

        }

        /*private void SetCurrentContent(string x)
        {
            webBrowser1.DocumentText = "<html><body>" + x + "</body></html>";
        }
        */
        private void btnPurpose_Click(object sender, EventArgs e)
        {
            var x = PluginReader.PluginList;
            PluginReader.StartUp(x.ElementAt(0));
            PluginReader.StartUp(x.ElementAt(0));
        }

        private void btnLessons_Click(object sender, EventArgs e)
        {
            ListItems(RichTextMode.Lessons);
        }


        private void btnDailyVerses_Click(object sender, EventArgs e)
        {
            for (var x = 1; x <= 2; x++)
            {
                try
                {
                    webBrowser1.DocumentText = DailyScripture.GetDailyScripture();
                }
                catch (Exception)
                {
                    MessageBox.Show("Sorry, there is no verse(s) for today. Check back tomorrow!");
                }
            }
            ListItems(RichTextMode.DailyVerses);
        }


        private void picExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void picMinimize_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var settingsForm = new SettingsForm();
            settingsForm.ShowDialog();
        }

        private BibPlanDialog _bPlanDialog = new BibPlanDialog();
        private void KillPlans()
        {
            _bPlanDialog.Hide();
        }
        private RichTextMode _rtmSize;
        private void ListItems(RichTextMode rtm)
        {
            lboListOfItems.Items.Clear();
            _rtmSize = rtm;
            if (rtm != RichTextMode.BiblePlans) KillPlans();
            switch (rtm)
            {
                case RichTextMode.Lessons:
                    GetallFilesFromHttp.ListDiractory("http://godispower.us/Sundays/");
                    var intIndex = 0;
                    foreach (var x in GetallFilesFromHttp.Files)
                    {
                        if (intIndex > 1)
                        {
                            if (intIndex < GetallFilesFromHttp.Files.Count - 2)
                                try
                                {
                                    var i = x.Substring(1, x.Length - 7).Replace(".", "/");
                                    if (!(Convert.ToDateTime(i) > DateTime.Now))
                                    {
                                        lboListOfItems.Items.Add(i);
                                    }
                                }
                                catch (Exception)
                                {
                                    // Ignored - Occurs when "Lessons" button clicked twice
                                }
                        }
                        intIndex++;
                    }
                    
                    break;
                case RichTextMode.DailyVerses:
                    var currentScripture = Directory.GetFiles("Verses/").Length;
                    for (var x = 1; x <= currentScripture; x++)
                    {
                        lboListOfItems.Items.Add("Day #" + x);
                    }
                    break;
            }
        }
        
        private void btnBibPlans_Click(object sender, EventArgs e)
        {
            _bPlanDialog.Show();
            _rtmSize = RichTextMode.BiblePlans;
        }

        private void lboListOfItems_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (_rtmSize)
            {
                case RichTextMode.Lessons:
                    var webFile = "http://godispower.us/Sundays/" +
                                  lboListOfItems.SelectedItem.ToString().Replace("/", ".") + ".html";
                    webBrowser1.Navigate(webFile);
                    break;
                case RichTextMode.DailyVerses:
                    // D a y   # 1
                    // 0 1 2 3 4 5
                    webBrowser1.DocumentText =
                        DailyScripture.GetDailyScripture(int.Parse(lboListOfItems.SelectedItem.ToString().Substring(5)));
                    break;
                case RichTextMode.BiblePlans:
                    webBrowser1.DocumentText = Parser.HtmlText(BibPlanDialog.PlanFileName, lboListOfItems.SelectedIndex);
                    break;

            }
        }

        private void headerpanel_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button2_Load(object sender, EventArgs e)
        {

        }

        private void lblName_Click(object sender, EventArgs e)
        {

        }
    }

    public enum RichTextMode
    {
        Purpose,
        Lessons,
        DailyVerses,
        BiblePlans
    }

   
}

