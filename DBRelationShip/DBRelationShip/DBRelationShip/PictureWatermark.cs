using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace Pub.Class
{
    /// <summary>
    /// 添加水印类 只支持添加图片水印
    /// </summary>
    public class Watermark
    {
        #region 私有成员
        private string modifyImagePath = null;
        private string drawedImagePath = null;
        private int rightSpace;
        private int bottoamSpace;
        private int lucencyPercent = 70;
        private string outPath = null;
        #endregion

        #region 构造器
        /// <summary>
        /// 构造函数
        /// </summary>
        public Watermark() { }
        #endregion

        #region 属性
        /// <summary>
        /// 获取或设置要修改的图像路径
        /// </summary>
        public string ModifyImagePath
        {
            get { return this.modifyImagePath; }
            set { this.modifyImagePath = value; }
        }
        /// <summary>
        /// 获取或设置在画的图片路径(水印图片)
        /// </summary>
        public string DrawedImagePath
        {
            get { return this.drawedImagePath; }
            set { this.drawedImagePath = value; }
        }
        /// <summary>
        /// 获取或设置水印在修改图片中的右边距
        /// </summary>
        public int RightSpace
        {
            get { return this.rightSpace; }
            set { this.rightSpace = value; }
        }
        /// <summary>
        /// 获取或设置水印在修改图片中距底部的高度
        /// </summary>
        public int BottoamSpace
        {
            get { return this.bottoamSpace; }
            set { this.bottoamSpace = value; }
        }
        /// <summary>
        /// 获取或设置要绘制水印的透明度,注意是原来图片透明度的百分比
        /// </summary>
        public int LucencyPercent
        {
            get { return this.lucencyPercent; }
            set { if (value >= 0 && value <= 100) this.lucencyPercent = value; }
        }
        /// <summary>
        /// 获取或设置要输出图像的路径
        /// </summary>
        public string OutPath
        {
            get { return this.outPath; }
            set { this.outPath = value; }
        }
        #endregion
        
        private static void textWatermark(object sender, EventArgs e)
        {
            System.Drawing.Image image = System.Drawing.Image.FromFile("d:\\b.jpg");
            Graphics g = Graphics.FromImage(image);
            g.DrawImage(image, 0, 0, image.Width, image.Height);
            Font f = new Font("Verdana", 32);
            Brush b = new SolidBrush(Color.Red);
            g.DrawString("Leonchen", f, b, 10, 10);
            g.Dispose();
            string spath = "d:\\" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".jpg";
            image.Save(spath);
            image.Dispose();

            if (File.Exists("d:\\b.jpg"))
            {
                File.Delete("d:\\b.jpg");
            }
        } 

        #region 开始绘制水印 DrawImage
        /// <summary>
        /// 开始绘制水印
        /// </summary>
        /// <example>
        /// <code>
        ///     Watermark wm = new Watermark();
        ///     wm.DrawedImagePath= Server.MapPath("") + "/upfile/" + "backlogo.gif";
        ///     wm.ModifyImagePath=path; 
        ///     wm.RightSpace=184;
        ///     wm.BottoamSpace=81;
        ///     wm.LucencyPercent=50;
        ///     wm.OutPath=Server.MapPath("") + "/upfile/" + fileName + "_new" + extension;
        ///     wm.DrawImage();
        ///     
        ///     //保存加水印过后的图片,删除原始图片 
        ///     mFileName=fileName + "_new" + extension;
        ///     if(File.Exists(path)) {  File.Delete(path); } 
        /// </code>
        /// </example>
        public void DrawImage()
        {
            Image modifyImage = null;
            Image drawedImage = null;
            Graphics g = null;
            try
            {
                modifyImage = Image.FromFile(this.ModifyImagePath);//建立图形对象
                drawedImage = Image.FromFile(this.DrawedImagePath);
                g = Graphics.FromImage(modifyImage);

                int x = modifyImage.Width - this.rightSpace;//获取要绘制图形坐标
                int y = modifyImage.Height - this.BottoamSpace;

                float[][] matrixItems ={//设置颜色矩阵
            new float[] {1, 0, 0, 0, 0},
            new float[] {0, 1, 0, 0, 0},
            new float[] {0, 0, 1, 0, 0},
            new float[] {0, 0, 0, (float)this.LucencyPercent/100f, 0},
            new float[] {0, 0, 0, 0, 1}};

                ColorMatrix colorMatrix = new ColorMatrix(matrixItems);
                ImageAttributes imgAttr = new ImageAttributes();
                imgAttr.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                g.DrawImage(//绘制阴影图像
                 drawedImage,
                 new Rectangle(x, y, drawedImage.Width, drawedImage.Height),
                 0, 0, drawedImage.Width, drawedImage.Height,
                 GraphicsUnit.Pixel, imgAttr);

                string[] allowImageType = { ".jpg", ".gif", ".png", ".bmp", ".tiff", ".wmf", ".ico" };//保存文件
                FileInfo file = new FileInfo(this.ModifyImagePath);
                ImageFormat imageType = ImageFormat.Gif;
                switch (file.Extension.ToLower())
                {
                    case ".jpg": imageType = ImageFormat.Jpeg; break;
                    case ".gif": imageType = ImageFormat.Gif; break;
                    case ".png": imageType = ImageFormat.Png; break;
                    case ".bmp": imageType = ImageFormat.Bmp; break;
                    case ".tif": imageType = ImageFormat.Tiff; break;
                    case ".wmf": imageType = ImageFormat.Wmf; break;
                    case ".ico": imageType = ImageFormat.Icon; break;
                    default: break;
                }
                MemoryStream ms = new MemoryStream();
                modifyImage.Save(ms, imageType);
                byte[] imgData = ms.ToArray();
                modifyImage.Dispose();
                drawedImage.Dispose();
                g.Dispose();
                FileStream fs = null;
                if (this.OutPath == null || this.OutPath == "")
                {
                    File.Delete(this.ModifyImagePath);
                    fs = new FileStream(this.ModifyImagePath, FileMode.Create, FileAccess.Write);
                }
                else
                {
                    fs = new FileStream(this.OutPath, FileMode.Create, FileAccess.Write);
                }
                if (fs != null)
                {
                    fs.Write(imgData, 0, imgData.Length);
                    fs.Close();
                }
            }
            finally
            {
                try
                {
                    drawedImage.Dispose();
                    modifyImage.Dispose();
                    g.Dispose();
                }
                catch { }
            }
        }
        #endregion
    }
}
