﻿using System;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Power_of_God_Lib.BibPlan;
using Power_of_God_Lib.GUI.Controls;
using Power_of_God_Lib.GUI.DialogBox;
using Power_of_God_Lib.NetBible;
using Power_of_God_Lib.NetBible.Books;
using Power_of_God_Lib.pSystem;
using Power_of_God_Lib.Plugins;
using Power_of_God_Lib.Properties;
using Power_of_God_Lib.Utilities;
using Button = Power_of_God_Lib.GUI.Controls.Button;
using Settings = Power_of_God_Lib.pSystem.Settings;

namespace Power_of_God_Lib.GUI
{

    public partial class MainForm : Form
    {
        private readonly FlowLayoutPanel _fdefault;
        public MainForm()
        {
            if (!Directory.Exists("power.of.god"))
            {
                Directory.CreateDirectory("power.of.god");
            }
            InitializeComponent();
            _fdefault = flowLayoutPanel2;
            SetEventHandlers();
            Settings.LoadDefault();
            Settings.LoadFromJson();
            Bible.SetLocation("power.of.god/" + Settings.UserSettings.scriptver + ".xml");
            UpdatePluginPanel();

            var ver = Settings.UserSettings.scriptver;
            var file = "power.of.god/" + ver + ".xml";        
        }
        private void MainForm_Load(object sender, EventArgs e)
        {
            var x = PluginReader.PluginList;
            foreach (var c in x)
            {
                PluginReader.AppLoad(c);
            }
            Content.SetTitle(Network.LatestStable(false));
        }
        private bool IsRunningOnMono()
        {
            return Type.GetType("Mono.Runtime") != null;
        }
        private void UpdatePluginPanel()
        {
            PluginReader.LoadPlugins();
            try
            {
                foreach (var pl in PluginReader.PluginList)
                {
                    var myButton = new Button(pl, true);
                    myButton.SetText(pl.Name);
                    myButton.Width = flowLayoutPanel1.Width - 5;
                    myButton.Click += myButtonClick;
                    flowLayoutPanel1.Controls.Add(myButton);
                }
            }
            catch (Exception ex)
            {
                ErrorLogging.Write(ex);
            }
        }

        private void myButtonClick(object sender, EventArgs e)
        {
            var btnObject = (Button) sender;
            MessageBox.Show(btnObject.Text);
        }

        private void SetEventHandlers()
        {
            //webBrowser1.Navigated += ChangedTitle;
            Parser.PlanDays.CollectionChanged += CheckChanged;
            headerpanel.MouseDown += panel1_MouseDown;
            headerpanel.MouseUp += panel1_MouseUp;
            headerpanel.MouseMove += panel1_MouseMove;
            picMain.MouseDown += panel1_MouseDown;
            picMain.MouseUp += panel1_MouseUp;
            picMain.MouseMove += panel1_MouseMove;
            Content.ObservedList.CollectionChanged += CollectionChanged;
            settingsBtn.MouseHover += Hovered;
            settingsBtn.MouseLeave += LeftButton;
        }

        private void LeftButton(object sender, EventArgs e)
        {
            settingsBtn.Image = Resources.button_regular;
        }

        private void Hovered(object sender, EventArgs e)
        {
            settingsBtn.Image = Resources.button_inverted;
        }

        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            lboListOfItems.Items.Clear();
            foreach (var item in Content.ObservedList)
            {
                lboListOfItems.Items.Add(item);
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
        

        public sealed override string Text
        {
            get { return base.Text; }
            set { base.Text = value; }
        }


        private void picExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void picMinimize_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }
        

        private void lboListOfItems_SelectedIndexChanged(object sender, EventArgs e)
        {
            PluginReader.ActivatePluginListMethod(lboListOfItems.SelectedIndex);
        }

        private PluginFrame _alreadyClearedFrame;
        private string _alreadyClearedTitle;

        private void PluginUpdateTimer_Tick(object sender, EventArgs e)
        {
            flowLayoutPanel2 = _fdefault;
            // Update Content
            if (PluginReader.CurrentFrame != PluginReader.OldFrame)
            {

                if (_alreadyClearedFrame == null || _alreadyClearedFrame != PluginReader.CurrentFrame)
                {
                    flowLayoutPanel2.Controls.Clear();
                    _alreadyClearedFrame = PluginReader.CurrentFrame;
                    flowLayoutPanel2.Controls.Add(PluginReader.CurrentFrame);
                }
            }
            if (PluginReader.CurrentFrame == PluginReader.OldFrame)
            {
                flowLayoutPanel2.Controls.Add(PluginReader.CurrentFrame);
            }
            // Update Text
            if (Content.CurrentTitle != Content.OldTitle)
            {
                if (_alreadyClearedTitle == null || _alreadyClearedTitle != Content.CurrentTitle)
                {
                    lblName.Text = "";
                    _alreadyClearedTitle = Content.CurrentTitle;
                    lblName.Text = _alreadyClearedTitle;
                }
            }
            if (Content.CurrentTitle == Content.OldTitle)
            {
                lblName.Text = Content.CurrentTitle;
            }
        }

        private void SettingsButtonClick(object sender, EventArgs e)
        {
            var settingsForm = new SettingsForm();
            settingsForm.ShowDialog();
        }
        
        private void settingsBtn_Click(object sender, EventArgs e)
        {
            settingsBtn.Image = Resources.button_clicked;
            SettingsButtonClick(sender, e);
        }

        private void btnReadBible_Click(object sender, EventArgs e)
        {
            if (!IsRunningOnMono())
            {
                new BibleReaderForm().Show();
            }
            else
            {
                new DialogBox.DialogBox("Sorry",
                    "This feature is unavailable on Linux/Mac. It is only available on Windows.").Show();
            }
        }


        private void flowLayoutPanel2_Paint(object sender, PaintEventArgs e)
        {
            
        }

        private void lblName_Click(object sender, EventArgs e)
        {

        }

        private void headerpanel_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnCredits_Click(object sender, EventArgs e)
        {
            new CreditsForm().ShowDialog();
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
