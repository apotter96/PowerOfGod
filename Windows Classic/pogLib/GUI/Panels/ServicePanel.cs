﻿using NetBible.Bible;
using pogLib.Utils;
using System;
using System.Collections.Generic;
using pogLib.Sermons;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.Diagnostics;

namespace pogLib.GUI.Panels
{
    public class ServicePanel : DefaultPanel
    {
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnLive;
        private System.Windows.Forms.Timer timer1;
        private System.ComponentModel.IContainer components;
        private YoutubePlayer.Player player1;
        private System.Windows.Forms.Panel panel1;

        public ServicePanel()
        {
            InitializeComponent();
            ServicePanel_Load(this, new EventArgs());
        }

        private void LoadVideo(string id)
        {
            //geckoWebBrowser1.Navigate($"https://godispower.us/Sermons/VideoPlayer.php?id={id}");
            player1.VideoID = id;
        }


        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.panel1 = new System.Windows.Forms.Panel();
            this.listView1 = new System.Windows.Forms.ListView();
            this.label1 = new System.Windows.Forms.Label();
            this.btnLive = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.player1 = new YoutubePlayer.Player();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.player1);
            this.panel1.Location = new System.Drawing.Point(254, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(620, 487);
            this.panel1.TabIndex = 0;
            // 
            // listView1
            // 
            this.listView1.Location = new System.Drawing.Point(12, 76);
            this.listView1.MultiSelect = false;
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(236, 423);
            this.listView1.TabIndex = 1;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(8, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(240, 23);
            this.label1.TabIndex = 2;
            this.label1.Text = "Service Selector";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnLive
            // 
            this.btnLive.Location = new System.Drawing.Point(12, 38);
            this.btnLive.Name = "btnLive";
            this.btnLive.Size = new System.Drawing.Size(236, 23);
            this.btnLive.TabIndex = 3;
            this.btnLive.Text = "Watch Live";
            this.btnLive.UseVisualStyleBackColor = true;
            this.btnLive.Click += new System.EventHandler(this.btnLive_Click);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // player1
            // 
            this.player1.Location = new System.Drawing.Point(22, 34);
            this.player1.Name = "player1";
            this.player1.Size = new System.Drawing.Size(577, 419);
            this.player1.TabIndex = 0;
            this.player1.VideoID = "NpEaa2P7qZI";
            // 
            // ServicePanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.ClientSize = new System.Drawing.Size(886, 511);
            this.Controls.Add(this.btnLive);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.panel1);
            this.Name = "ServicePanel";
            this.Load += new System.EventHandler(this.ServicePanel_Load);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            btnLive.Enabled = Network.IsLive();
            btnLive.Text = Network.IsLive() ? "Watch LIVE!" : "Not On Air";
        }

        private void ServicePanel_Load(object sender, EventArgs e)
        {
            int[] years = new int[] { 2018, 2017, 2016 };
            
            foreach (int year in years)
            {
                List<Sermon> current = SermonRetriever.GetSermons(year);
                current.Reverse();
                foreach (Sermon x in current)
                {
                    listView1.Items.Add(x.Name + " (" + x.AirDate + ")");
                    SermonRetriever.AllSermons.Add(x);
                }
            }

            LoadVideo(SermonRetriever.AllSermons[0].ID);
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int sel = listView1.SelectedItems.Count;
            if (sel > 0)
            {
                LoadVideo(SermonRetriever.AllSermons[listView1.SelectedIndices[0]].ID);
            }
        }

        private void btnLive_Click(object sender, EventArgs e)
        {
            string json = Network.GetUrlSource("http://godispower.us/Application/live_status.json");
            LiveStatus ls = JsonConvert.DeserializeObject<LiveStatus>(json);
            Process.Start($"https://www.youtube.com/watch?v={ls.video_id}");
        }
    }
}
