using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Gma.QrCodeNet.Encoding;
using System.IO;
using Gma.QrCodeNet.Encoding.Windows.Render;
using System.Drawing.Imaging;

namespace QrCodeTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string value = @"abc
                124";
            QrCode code = new QrEncoder().Encode(value);

            using (var stream = new MemoryStream())
            {
                new GraphicsRenderer(new FixedCodeSize(190, QuietZoneModules.Two)).WriteToStream(code.Matrix,
                                                                                                   ImageFormat.Png,
                                                                                                   stream);

                Bitmap bmpt = new Bitmap(stream); 
                pictureBox1.Image = bmpt;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
