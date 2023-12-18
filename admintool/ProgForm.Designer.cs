namespace admintool
{
    partial class ProgForm
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
            this.lbHello = new System.Windows.Forms.Label();
            this.funcLB = new System.Windows.Forms.ListBox();
            this.btnAct = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.btnExit = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lbHello
            // 
            this.lbHello.AutoSize = true;
            this.lbHello.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbHello.Location = new System.Drawing.Point(33, 27);
            this.lbHello.Name = "lbHello";
            this.lbHello.Size = new System.Drawing.Size(142, 25);
            this.lbHello.TabIndex = 0;
            this.lbHello.Text = "Приветствую ";
            // 
            // funcLB
            // 
            this.funcLB.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.funcLB.FormattingEnabled = true;
            this.funcLB.ItemHeight = 29;
            this.funcLB.Location = new System.Drawing.Point(38, 100);
            this.funcLB.Name = "funcLB";
            this.funcLB.Size = new System.Drawing.Size(365, 178);
            this.funcLB.TabIndex = 6;
            this.funcLB.SelectedIndexChanged += new System.EventHandler(this.funcLB_SelectedIndexChanged);
            // 
            // btnAct
            // 
            this.btnAct.Location = new System.Drawing.Point(38, 284);
            this.btnAct.Name = "btnAct";
            this.btnAct.Size = new System.Drawing.Size(164, 35);
            this.btnAct.TabIndex = 8;
            this.btnAct.Text = "Выполнить";
            this.btnAct.UseVisualStyleBackColor = true;
            this.btnAct.Click += new System.EventHandler(this.btnAct_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(34, 77);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(283, 20);
            this.label2.TabIndex = 10;
            this.label2.Text = "Вам доступны следующие функции:";
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(239, 284);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(164, 35);
            this.btnExit.TabIndex = 11;
            this.btnExit.Text = "Выйти";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // ProgForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.ClientSize = new System.Drawing.Size(441, 344);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnAct);
            this.Controls.Add(this.funcLB);
            this.Controls.Add(this.lbHello);
            this.Name = "ProgForm";
            this.Text = "ProgForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbHello;
        private System.Windows.Forms.ListBox funcLB;
        private System.Windows.Forms.Button btnAct;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnExit;
    }
}