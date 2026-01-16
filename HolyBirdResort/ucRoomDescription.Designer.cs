namespace HolyBirdResort
{
    partial class ucRoomDescription
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.guna2Elipse1 = new Guna.UI2.WinForms.Guna2Elipse(this.components);
            this.guna2PictureBox1 = new Guna.UI2.WinForms.Guna2PictureBox();
            this.btnDetails = new Guna.UI2.WinForms.Guna2Button();
            this.btnBooking = new Guna.UI2.WinForms.Guna2Button();
            this.lblNoneChange = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.lblGiaPhong = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.lblThongTinKhac = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.lblTenPhong = new Guna.UI2.WinForms.Guna2HtmlLabel();
            ((System.ComponentModel.ISupportInitialize)(this.guna2PictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // guna2Elipse1
            // 
            this.guna2Elipse1.BorderRadius = 15;
            this.guna2Elipse1.TargetControl = this;
            // 
            // guna2PictureBox1
            // 
            this.guna2PictureBox1.BorderRadius = 10;
            this.guna2PictureBox1.Image = global::HolyBirdResort.Properties.Resources.P2;
            this.guna2PictureBox1.ImageRotate = 0F;
            this.guna2PictureBox1.Location = new System.Drawing.Point(26, 18);
            this.guna2PictureBox1.Name = "guna2PictureBox1";
            this.guna2PictureBox1.Size = new System.Drawing.Size(344, 127);
            this.guna2PictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.guna2PictureBox1.TabIndex = 1;
            this.guna2PictureBox1.TabStop = false;
            // 
            // btnDetails
            // 
            this.btnDetails.BackColor = System.Drawing.Color.Transparent;
            this.btnDetails.BorderRadius = 10;
            this.btnDetails.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnDetails.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnDetails.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnDetails.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnDetails.FillColor = System.Drawing.Color.SlateGray;
            this.btnDetails.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDetails.ForeColor = System.Drawing.Color.White;
            this.btnDetails.Location = new System.Drawing.Point(37, 202);
            this.btnDetails.Name = "btnDetails";
            this.btnDetails.Size = new System.Drawing.Size(123, 32);
            this.btnDetails.TabIndex = 30;
            this.btnDetails.Text = "XEM CHI TIẾT";
            this.btnDetails.Click += new System.EventHandler(this.btnDetails_Click);
            // 
            // btnBooking
            // 
            this.btnBooking.BackColor = System.Drawing.Color.Transparent;
            this.btnBooking.BorderRadius = 10;
            this.btnBooking.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnBooking.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnBooking.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnBooking.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnBooking.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(98)))), ((int)(((byte)(180)))));
            this.btnBooking.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBooking.ForeColor = System.Drawing.Color.White;
            this.btnBooking.Location = new System.Drawing.Point(188, 202);
            this.btnBooking.Name = "btnBooking";
            this.btnBooking.Size = new System.Drawing.Size(163, 32);
            this.btnBooking.TabIndex = 29;
            this.btnBooking.Text = "ĐẶT PHÒNG NGAY";
            this.btnBooking.Click += new System.EventHandler(this.btnBooking_Click);
            // 
            // lblNoneChange
            // 
            this.lblNoneChange.AutoSize = false;
            this.lblNoneChange.BackColor = System.Drawing.Color.Transparent;
            this.lblNoneChange.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNoneChange.ForeColor = System.Drawing.Color.DimGray;
            this.lblNoneChange.Location = new System.Drawing.Point(234, 172);
            this.lblNoneChange.Name = "lblNoneChange";
            this.lblNoneChange.Size = new System.Drawing.Size(136, 24);
            this.lblNoneChange.TabIndex = 28;
            this.lblNoneChange.Text = "/giờ";
            this.lblNoneChange.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblGiaPhong
            // 
            this.lblGiaPhong.AutoSize = false;
            this.lblGiaPhong.BackColor = System.Drawing.Color.Transparent;
            this.lblGiaPhong.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblGiaPhong.ForeColor = System.Drawing.Color.CadetBlue;
            this.lblGiaPhong.Location = new System.Drawing.Point(234, 151);
            this.lblGiaPhong.Name = "lblGiaPhong";
            this.lblGiaPhong.Size = new System.Drawing.Size(136, 24);
            this.lblGiaPhong.TabIndex = 27;
            this.lblGiaPhong.Text = "1,200,000 VNĐ";
            this.lblGiaPhong.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblThongTinKhac
            // 
            this.lblThongTinKhac.AutoSize = false;
            this.lblThongTinKhac.BackColor = System.Drawing.Color.Transparent;
            this.lblThongTinKhac.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblThongTinKhac.ForeColor = System.Drawing.Color.DimGray;
            this.lblThongTinKhac.Location = new System.Drawing.Point(26, 172);
            this.lblThongTinKhac.Name = "lblThongTinKhac";
            this.lblThongTinKhac.Size = new System.Drawing.Size(223, 24);
            this.lblThongTinKhac.TabIndex = 26;
            this.lblThongTinKhac.Text = "1 giường đôi";
            // 
            // lblTenPhong
            // 
            this.lblTenPhong.AutoSize = false;
            this.lblTenPhong.BackColor = System.Drawing.Color.Transparent;
            this.lblTenPhong.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTenPhong.Location = new System.Drawing.Point(26, 151);
            this.lblTenPhong.Name = "lblTenPhong";
            this.lblTenPhong.Size = new System.Drawing.Size(222, 24);
            this.lblTenPhong.TabIndex = 25;
            this.lblTenPhong.Text = "VIP - 1010 ";
            // 
            // ucRoomDescription
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(242)))), ((int)(((byte)(250)))));
            this.Controls.Add(this.btnDetails);
            this.Controls.Add(this.btnBooking);
            this.Controls.Add(this.lblNoneChange);
            this.Controls.Add(this.lblGiaPhong);
            this.Controls.Add(this.lblThongTinKhac);
            this.Controls.Add(this.lblTenPhong);
            this.Controls.Add(this.guna2PictureBox1);
            this.Name = "ucRoomDescription";
            this.Size = new System.Drawing.Size(398, 249);
            ((System.ComponentModel.ISupportInitialize)(this.guna2PictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Guna.UI2.WinForms.Guna2Elipse guna2Elipse1;
        private Guna.UI2.WinForms.Guna2PictureBox guna2PictureBox1;
        private Guna.UI2.WinForms.Guna2Button btnDetails;
        private Guna.UI2.WinForms.Guna2Button btnBooking;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblNoneChange;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblGiaPhong;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblThongTinKhac;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblTenPhong;
    }
}
