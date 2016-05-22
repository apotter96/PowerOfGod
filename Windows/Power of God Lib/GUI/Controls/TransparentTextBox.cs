﻿using System.Drawing;
using System.Windows.Forms;

namespace Power_of_God_Lib.GUI.Controls
{
    public partial class TransparentTextBox : TextBox
    {
        public TransparentTextBox()
        {
            InitializeComponent();
            SetStyle(ControlStyles.SupportsTransparentBackColor |
                 ControlStyles.OptimizedDoubleBuffer |
                 ControlStyles.AllPaintingInWmPaint |
                 ControlStyles.ResizeRedraw |
                 ControlStyles.UserPaint, true);
            BackColor = Color.Transparent;
        }

        public sealed override Color BackColor
        {
            get { return base.BackColor; }
            set { base.BackColor = value; }
        }
    }
}