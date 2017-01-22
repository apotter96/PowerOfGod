﻿using System;
using System.Collections.Generic;
using System.Linq;
using Power_of_God_Lib.GUI.Controls;
using Power_of_God_Lib.Plugins;
using Purpose.Frames;

namespace Purpose
{
    public class Plugin : Power_of_God_Lib.Plugins.Plugin
    {
        public new int Priority = 5;
        public sealed override string[] PluginVersion { get; set; }

        public Plugin(string name, string dev, Category cat, bool act) : base(name, dev, cat, act)
        {

        }
        public override void AppLoad()
        {
            // Add any needed plugin start up content
            PerformStartAction(); /* If your start action trigers any events, it needs to be called 
            * first in the constructor and 2nd in the
            * Button click */
        }
        public Plugin()
        {
            PluginVersion = new[] { "1", "0" };

        }
        public override void PerformStartAction()
        {
            
        }

        // Put main frame id in spot 0... ALWAYS
        private readonly List<PluginFrame> _frames = new List<PluginFrame>
        {
            new PurposeFrame(),
            null
        };
        public override List<PluginFrame> FrameIdList()
        {
            return _frames;
        }

        public override PluginFrame GetFrame(string idStr)
        {
            foreach (var frame in FrameIdList().Where(frame => frame.FrameID == idStr))
            {
                return frame;
            }
            return new PluginFrame();
        }
    }
}