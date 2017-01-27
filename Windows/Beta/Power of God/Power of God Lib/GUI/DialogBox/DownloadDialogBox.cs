﻿using System;
using System.ComponentModel;
using System.Drawing;
using System.Net;
using System.Windows.Forms;

namespace Power_of_God_Lib.GUI.DialogBox
{
    public partial class DownloadDialogBox : Form
    {
        public DownloadDialogBox()
        {
            InitializeComponent();
            throw new NotImplementedException(); // Do not use default constructor
        }

        private readonly string _downloadLocation;
        private readonly string _localLocation;
        public DownloadDialogBox(string title, string message, string downloadLocation, string localLocation)
        {
            InitializeComponent();
            SetTitle(title);
            SetMessage(message);
            SetButtonText("Ok");
            _downloadLocation = downloadLocation;
            _localLocation = localLocation;
            SetInitHandlers();
        }
        private void SetInitHandlers()
        {
            picMain.MouseDown += panel1_MouseDown;
            picMain.MouseUp += panel1_MouseUp;
            picMain.MouseMove += panel1_MouseMove;
            lblName.MouseDown += panel1_MouseDown;
            lblName.MouseUp += panel1_MouseUp;
            lblName.MouseMove += panel1_MouseMove;
            MessageLabel.MouseDown += panel1_MouseDown;
            MessageLabel.MouseUp += panel1_MouseUp;
            MessageLabel.MouseMove += panel1_MouseMove;
            MouseDown += panel1_MouseDown;
            MouseUp += panel1_MouseUp;
            MouseMove += panel1_MouseMove;
        }
        //Global variables;
        private bool _dragging = false;
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
            Location = new Point(p.X - this._startPoint.X, p.Y - this._startPoint.Y);
        }
        private void SetButtonText(string text)
        {
            button1.Text = text;
        }

        public new void Show()
        {
            ShowDialog();
        }
        private void SetTitle(string text)
        {
            lblName.Text = text;
        }

        private void SetMessage(string text)
        {
            MessageLabel.Text = text;
        }
        private void DialogBox_Load(object sender, EventArgs e)
        {
            var client = new WebClient();
            client.DownloadProgressChanged += client_DownloadProgressChanged;
            client.DownloadFileCompleted += client_DownloadFileCompleted;
            var uri =
                new Uri(_downloadLocation
                    );
            client.DownloadFileAsync(uri, _localLocation);
        }

        private void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            MessageBox.Show("The download is finished.", "Complete");
        }

        private void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBar1.Maximum = (int)e.TotalBytesToReceive / 100;
            progressBar1.Value = (int)e.BytesReceived / 100;
        }

        private void ClickMethod(object sender, EventArgs e)
        {
            Hide();
            Close();
        }
    }
}