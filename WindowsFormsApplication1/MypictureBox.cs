using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class MypictureBox : PictureBox
    {
        public MypictureBox()
        {
            InitializeComponent();
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }

        public MypictureBox(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
    }
}
