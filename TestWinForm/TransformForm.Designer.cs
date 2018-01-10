namespace TestWinForm
{
    partial class TransformForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.txtInputXml20000 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtInputXsl = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtOutputXml20000 = new System.Windows.Forms.TextBox();
            this.btnTransform = new System.Windows.Forms.Button();
            this.txtOutputXml30000 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtInputXml30000 = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtMessage = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(95, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "Input xml 20000";
            // 
            // txtInputXml20000
            // 
            this.txtInputXml20000.Location = new System.Drawing.Point(100, 9);
            this.txtInputXml20000.Name = "txtInputXml20000";
            this.txtInputXml20000.Size = new System.Drawing.Size(172, 21);
            this.txtInputXml20000.TabIndex = 1;
            this.txtInputXml20000.Text = "input20000.xml";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 120);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "Input xsl";
            // 
            // txtInputXsl
            // 
            this.txtInputXsl.Location = new System.Drawing.Point(100, 118);
            this.txtInputXsl.Name = "txtInputXsl";
            this.txtInputXsl.Size = new System.Drawing.Size(172, 21);
            this.txtInputXsl.TabIndex = 3;
            this.txtInputXsl.Text = "input.xsl";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 35);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(101, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "Output Xml 20000";
            // 
            // txtOutputXml20000
            // 
            this.txtOutputXml20000.Location = new System.Drawing.Point(100, 32);
            this.txtOutputXml20000.Name = "txtOutputXml20000";
            this.txtOutputXml20000.Size = new System.Drawing.Size(172, 21);
            this.txtOutputXml20000.TabIndex = 5;
            this.txtOutputXml20000.Text = "output20000.xml";
            // 
            // btnTransform
            // 
            this.btnTransform.Location = new System.Drawing.Point(197, 197);
            this.btnTransform.Name = "btnTransform";
            this.btnTransform.Size = new System.Drawing.Size(75, 21);
            this.btnTransform.TabIndex = 6;
            this.btnTransform.Text = "Transform";
            this.btnTransform.UseVisualStyleBackColor = true;
            this.btnTransform.Click += new System.EventHandler(this.btnTransform_Click);
            // 
            // txtOutputXml30000
            // 
            this.txtOutputXml30000.Location = new System.Drawing.Point(100, 88);
            this.txtOutputXml30000.Name = "txtOutputXml30000";
            this.txtOutputXml30000.Size = new System.Drawing.Size(172, 21);
            this.txtOutputXml30000.TabIndex = 12;
            this.txtOutputXml30000.Text = "output30000.xml";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 90);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(101, 12);
            this.label4.TabIndex = 11;
            this.label4.Text = "Output Xml 30000";
            // 
            // txtInputXml30000
            // 
            this.txtInputXml30000.Location = new System.Drawing.Point(100, 65);
            this.txtInputXml30000.Name = "txtInputXml30000";
            this.txtInputXml30000.Size = new System.Drawing.Size(172, 21);
            this.txtInputXml30000.TabIndex = 8;
            this.txtInputXml30000.Text = "input30000.xml";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(13, 67);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(95, 12);
            this.label6.TabIndex = 7;
            this.label6.Text = "Input xml 30000";
            // 
            // txtMessage
            // 
            this.txtMessage.Location = new System.Drawing.Point(6, 142);
            this.txtMessage.Multiline = true;
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.ReadOnly = true;
            this.txtMessage.Size = new System.Drawing.Size(266, 49);
            this.txtMessage.TabIndex = 13;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(367, 83);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(41, 12);
            this.label5.TabIndex = 15;
            this.label5.Text = "label5";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(333, 29);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(63, 24);
            this.button1.TabIndex = 14;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(333, 120);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 16;
            this.button2.Text = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(333, 168);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 17;
            this.button3.Text = "ThreadDic";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(442, 29);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(75, 23);
            this.button4.TabIndex = 18;
            this.button4.Text = "Thread";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(442, 120);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(75, 23);
            this.button5.TabIndex = 19;
            this.button5.Text = "Continue";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(442, 67);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 21);
            this.textBox1.TabIndex = 20;
            // 
            // TransformForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(569, 222);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.txtMessage);
            this.Controls.Add(this.txtOutputXml30000);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtInputXml30000);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.btnTransform);
            this.Controls.Add(this.txtOutputXml20000);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtInputXsl);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtInputXml20000);
            this.Controls.Add(this.label1);
            this.Name = "TransformForm";
            this.Text = "TransformForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtInputXml20000;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtInputXsl;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtOutputXml20000;
        private System.Windows.Forms.Button btnTransform;
        private System.Windows.Forms.TextBox txtOutputXml30000;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtInputXml30000;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtMessage;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.TextBox textBox1;
    }
}