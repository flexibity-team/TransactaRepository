namespace TestStart200K
{
    partial class Form1
    {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnStart = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.tbAnsver = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnSeans = new System.Windows.Forms.Button();
            this.Shift = new System.Windows.Forms.Button();
            this.ShiftCloseS = new System.Windows.Forms.Button();
            this.GetBuff = new System.Windows.Forms.Button();
            this.GetFloat = new System.Windows.Forms.Button();
            this.GetInt = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(103, 54);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 0;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(103, 189);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(75, 23);
            this.btnStop.TabIndex = 1;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // tbAnsver
            // 
            this.tbAnsver.Location = new System.Drawing.Point(103, 28);
            this.tbAnsver.Name = "tbAnsver";
            this.tbAnsver.Size = new System.Drawing.Size(394, 20);
            this.tbAnsver.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(55, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(42, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Answer";
            // 
            // btnSeans
            // 
            this.btnSeans.Location = new System.Drawing.Point(103, 83);
            this.btnSeans.Name = "btnSeans";
            this.btnSeans.Size = new System.Drawing.Size(75, 23);
            this.btnSeans.TabIndex = 4;
            this.btnSeans.Text = "Seans";
            this.btnSeans.UseVisualStyleBackColor = true;
            this.btnSeans.Click += new System.EventHandler(this.btnSeans_Click);
            // 
            // Shift
            // 
            this.Shift.Location = new System.Drawing.Point(103, 112);
            this.Shift.Name = "Shift";
            this.Shift.Size = new System.Drawing.Size(75, 23);
            this.Shift.TabIndex = 5;
            this.Shift.Text = "ShiftOpen";
            this.Shift.UseVisualStyleBackColor = true;
            this.Shift.Click += new System.EventHandler(this.Shift_Click);
            // 
            // ShiftCloseS
            // 
            this.ShiftCloseS.Location = new System.Drawing.Point(103, 151);
            this.ShiftCloseS.Name = "ShiftCloseS";
            this.ShiftCloseS.Size = new System.Drawing.Size(75, 23);
            this.ShiftCloseS.TabIndex = 6;
            this.ShiftCloseS.Text = "ShiftClose";
            this.ShiftCloseS.UseVisualStyleBackColor = true;
            this.ShiftCloseS.Click += new System.EventHandler(this.ShiftClose_Click);
            // 
            // GetBuff
            // 
            this.GetBuff.Location = new System.Drawing.Point(206, 54);
            this.GetBuff.Name = "GetBuff";
            this.GetBuff.Size = new System.Drawing.Size(75, 23);
            this.GetBuff.TabIndex = 7;
            this.GetBuff.Text = "GetAnswer";
            this.GetBuff.UseVisualStyleBackColor = true;
            this.GetBuff.Click += new System.EventHandler(this.GetBuff_Click);
            // 
            // GetFloat
            // 
            this.GetFloat.Location = new System.Drawing.Point(206, 83);
            this.GetFloat.Name = "GetFloat";
            this.GetFloat.Size = new System.Drawing.Size(75, 23);
            this.GetFloat.TabIndex = 8;
            this.GetFloat.Text = "GetFloat";
            this.GetFloat.UseVisualStyleBackColor = true;
            // 
            // GetInt
            // 
            this.GetInt.Location = new System.Drawing.Point(205, 111);
            this.GetInt.Name = "GetInt";
            this.GetInt.Size = new System.Drawing.Size(75, 23);
            this.GetInt.TabIndex = 9;
            this.GetInt.Text = "GetInt";
            this.GetInt.UseVisualStyleBackColor = true;
            this.GetInt.Click += new System.EventHandler(this.GetInt_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(509, 262);
            this.Controls.Add(this.GetInt);
            this.Controls.Add(this.GetFloat);
            this.Controls.Add(this.GetBuff);
            this.Controls.Add(this.ShiftCloseS);
            this.Controls.Add(this.Shift);
            this.Controls.Add(this.btnSeans);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbAnsver);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnStart);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.TextBox tbAnsver;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnSeans;
        private System.Windows.Forms.Button Shift;
        private System.Windows.Forms.Button ShiftCloseS;
        private System.Windows.Forms.Button GetBuff;
        private System.Windows.Forms.Button GetFloat;
        private System.Windows.Forms.Button GetInt;
    }
}

