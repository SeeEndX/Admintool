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
            this.label2 = new System.Windows.Forms.Label();
            this.btnExit = new System.Windows.Forms.Button();
            this.tabsCtrl = new System.Windows.Forms.TabControl();
            this.tabManageServer = new System.Windows.Forms.TabPage();
            this.button2 = new System.Windows.Forms.Button();
            this.tabManageSite = new System.Windows.Forms.TabPage();
            this.label1 = new System.Windows.Forms.Label();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnDeleteSite = new System.Windows.Forms.Button();
            this.btnEditSite = new System.Windows.Forms.Button();
            this.dgvSites = new System.Windows.Forms.DataGridView();
            this.btnAddSite = new System.Windows.Forms.Button();
            this.tabManagePool = new System.Windows.Forms.TabPage();
            this.btnStopPool = new System.Windows.Forms.Button();
            this.btnStartPool = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.btnDeletePool = new System.Windows.Forms.Button();
            this.btnEditPool = new System.Windows.Forms.Button();
            this.dgvPool = new System.Windows.Forms.DataGridView();
            this.btnAddPool = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.tabsCtrl.SuspendLayout();
            this.tabManageServer.SuspendLayout();
            this.tabManageSite.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSites)).BeginInit();
            this.tabManagePool.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPool)).BeginInit();
            this.SuspendLayout();
            // 
            // lbHello
            // 
            this.lbHello.AutoSize = true;
            this.lbHello.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbHello.Location = new System.Drawing.Point(30, 32);
            this.lbHello.Name = "lbHello";
            this.lbHello.Size = new System.Drawing.Size(170, 29);
            this.lbHello.TabIndex = 0;
            this.lbHello.Text = "Приветствую ";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(31, 73);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(315, 22);
            this.label2.TabIndex = 10;
            this.label2.Text = "Вам доступны следующие функции:";
            // 
            // btnExit
            // 
            this.btnExit.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnExit.Location = new System.Drawing.Point(653, 494);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(164, 35);
            this.btnExit.TabIndex = 11;
            this.btnExit.Text = "Выйти";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // tabsCtrl
            // 
            this.tabsCtrl.Controls.Add(this.tabManageServer);
            this.tabsCtrl.Controls.Add(this.tabManageSite);
            this.tabsCtrl.Controls.Add(this.tabManagePool);
            this.tabsCtrl.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tabsCtrl.Location = new System.Drawing.Point(31, 114);
            this.tabsCtrl.Name = "tabsCtrl";
            this.tabsCtrl.SelectedIndex = 0;
            this.tabsCtrl.Size = new System.Drawing.Size(790, 361);
            this.tabsCtrl.TabIndex = 12;
            // 
            // tabManageServer
            // 
            this.tabManageServer.Controls.Add(this.button2);
            this.tabManageServer.Location = new System.Drawing.Point(4, 34);
            this.tabManageServer.Name = "tabManageServer";
            this.tabManageServer.Size = new System.Drawing.Size(782, 323);
            this.tabManageServer.TabIndex = 2;
            this.tabManageServer.Text = "Конфигурация сервера";
            this.tabManageServer.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(510, 270);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(144, 50);
            this.button2.TabIndex = 1;
            this.button2.Text = "Сервер";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // tabManageSite
            // 
            this.tabManageSite.Controls.Add(this.label1);
            this.tabManageSite.Controls.Add(this.btnStop);
            this.tabManageSite.Controls.Add(this.btnStart);
            this.tabManageSite.Controls.Add(this.btnDeleteSite);
            this.tabManageSite.Controls.Add(this.btnEditSite);
            this.tabManageSite.Controls.Add(this.dgvSites);
            this.tabManageSite.Controls.Add(this.btnAddSite);
            this.tabManageSite.Location = new System.Drawing.Point(4, 34);
            this.tabManageSite.Name = "tabManageSite";
            this.tabManageSite.Padding = new System.Windows.Forms.Padding(3);
            this.tabManageSite.Size = new System.Drawing.Size(782, 323);
            this.tabManageSite.TabIndex = 0;
            this.tabManageSite.Text = "Управление сайтами IIS";
            this.tabManageSite.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(33, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(78, 25);
            this.label1.TabIndex = 6;
            this.label1.Text = "Сайты:";
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(606, 244);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(142, 50);
            this.btnStop.TabIndex = 5;
            this.btnStop.Text = "Остановить";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(459, 244);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(141, 50);
            this.btnStart.TabIndex = 4;
            this.btnStart.Text = "Запустить";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnDeleteSite
            // 
            this.btnDeleteSite.Location = new System.Drawing.Point(295, 244);
            this.btnDeleteSite.Name = "btnDeleteSite";
            this.btnDeleteSite.Size = new System.Drawing.Size(130, 50);
            this.btnDeleteSite.TabIndex = 3;
            this.btnDeleteSite.Text = "Удалить";
            this.btnDeleteSite.UseVisualStyleBackColor = true;
            this.btnDeleteSite.Click += new System.EventHandler(this.btnDeleteSite_Click);
            // 
            // btnEditSite
            // 
            this.btnEditSite.Location = new System.Drawing.Point(159, 244);
            this.btnEditSite.Name = "btnEditSite";
            this.btnEditSite.Size = new System.Drawing.Size(130, 50);
            this.btnEditSite.TabIndex = 2;
            this.btnEditSite.Text = "Изменить";
            this.btnEditSite.UseVisualStyleBackColor = true;
            this.btnEditSite.Click += new System.EventHandler(this.btnEditSite_Click);
            // 
            // dgvSites
            // 
            this.dgvSites.AllowUserToAddRows = false;
            this.dgvSites.AllowUserToDeleteRows = false;
            this.dgvSites.BackgroundColor = System.Drawing.SystemColors.ButtonHighlight;
            this.dgvSites.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSites.Location = new System.Drawing.Point(22, 32);
            this.dgvSites.Name = "dgvSites";
            this.dgvSites.ReadOnly = true;
            this.dgvSites.RowHeadersWidth = 62;
            this.dgvSites.RowTemplate.Height = 28;
            this.dgvSites.Size = new System.Drawing.Size(726, 206);
            this.dgvSites.TabIndex = 1;
            // 
            // btnAddSite
            // 
            this.btnAddSite.Location = new System.Drawing.Point(22, 244);
            this.btnAddSite.Name = "btnAddSite";
            this.btnAddSite.Size = new System.Drawing.Size(131, 50);
            this.btnAddSite.TabIndex = 0;
            this.btnAddSite.Text = "Добавить";
            this.btnAddSite.UseVisualStyleBackColor = true;
            this.btnAddSite.Click += new System.EventHandler(this.btnAddSite_Click);
            // 
            // tabManagePool
            // 
            this.tabManagePool.Controls.Add(this.btnStopPool);
            this.tabManagePool.Controls.Add(this.btnStartPool);
            this.tabManagePool.Controls.Add(this.label4);
            this.tabManagePool.Controls.Add(this.btnDeletePool);
            this.tabManagePool.Controls.Add(this.btnEditPool);
            this.tabManagePool.Controls.Add(this.dgvPool);
            this.tabManagePool.Controls.Add(this.btnAddPool);
            this.tabManagePool.Location = new System.Drawing.Point(4, 34);
            this.tabManagePool.Name = "tabManagePool";
            this.tabManagePool.Size = new System.Drawing.Size(782, 323);
            this.tabManagePool.TabIndex = 1;
            this.tabManagePool.Text = "Управление пулами";
            this.tabManagePool.UseVisualStyleBackColor = true;
            // 
            // btnStopPool
            // 
            this.btnStopPool.Location = new System.Drawing.Point(606, 244);
            this.btnStopPool.Name = "btnStopPool";
            this.btnStopPool.Size = new System.Drawing.Size(142, 50);
            this.btnStopPool.TabIndex = 10;
            this.btnStopPool.Text = "Остановить";
            this.btnStopPool.UseVisualStyleBackColor = true;
            this.btnStopPool.Click += new System.EventHandler(this.btnStopPool_Click);
            // 
            // btnStartPool
            // 
            this.btnStartPool.Location = new System.Drawing.Point(459, 244);
            this.btnStartPool.Name = "btnStartPool";
            this.btnStartPool.Size = new System.Drawing.Size(141, 50);
            this.btnStartPool.TabIndex = 9;
            this.btnStartPool.Text = "Запустить";
            this.btnStartPool.UseVisualStyleBackColor = true;
            this.btnStartPool.Click += new System.EventHandler(this.btnStartPool_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(33, 7);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(189, 25);
            this.label4.TabIndex = 8;
            this.label4.Text = "Пулы приложений:";
            // 
            // btnDeletePool
            // 
            this.btnDeletePool.Location = new System.Drawing.Point(295, 244);
            this.btnDeletePool.Name = "btnDeletePool";
            this.btnDeletePool.Size = new System.Drawing.Size(130, 50);
            this.btnDeletePool.TabIndex = 7;
            this.btnDeletePool.Text = "Удалить";
            this.btnDeletePool.UseVisualStyleBackColor = true;
            this.btnDeletePool.Click += new System.EventHandler(this.btnDeletePool_Click);
            // 
            // btnEditPool
            // 
            this.btnEditPool.Location = new System.Drawing.Point(159, 244);
            this.btnEditPool.Name = "btnEditPool";
            this.btnEditPool.Size = new System.Drawing.Size(130, 50);
            this.btnEditPool.TabIndex = 6;
            this.btnEditPool.Text = "Изменить";
            this.btnEditPool.UseVisualStyleBackColor = true;
            this.btnEditPool.Click += new System.EventHandler(this.btnEditPool_Click);
            // 
            // dgvPool
            // 
            this.dgvPool.AllowUserToAddRows = false;
            this.dgvPool.AllowUserToDeleteRows = false;
            this.dgvPool.BackgroundColor = System.Drawing.SystemColors.ButtonHighlight;
            this.dgvPool.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPool.Location = new System.Drawing.Point(22, 32);
            this.dgvPool.Name = "dgvPool";
            this.dgvPool.ReadOnly = true;
            this.dgvPool.RowHeadersWidth = 62;
            this.dgvPool.RowTemplate.Height = 28;
            this.dgvPool.Size = new System.Drawing.Size(726, 198);
            this.dgvPool.TabIndex = 5;
            // 
            // btnAddPool
            // 
            this.btnAddPool.Location = new System.Drawing.Point(22, 244);
            this.btnAddPool.Name = "btnAddPool";
            this.btnAddPool.Size = new System.Drawing.Size(131, 50);
            this.btnAddPool.TabIndex = 4;
            this.btnAddPool.Text = "Добавить";
            this.btnAddPool.UseVisualStyleBackColor = true;
            this.btnAddPool.Click += new System.EventHandler(this.btnAddPool_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label3.Location = new System.Drawing.Point(30, 131);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(520, 58);
            this.label3.TabIndex = 13;
            this.label3.Text = "Вам не назначены функции:(\r\nПожалуйста, обратитесь к администратору.";
            this.label3.Visible = false;
            // 
            // ProgForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.ClientSize = new System.Drawing.Size(851, 545);
            this.Controls.Add(this.tabsCtrl);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lbHello);
            this.Name = "ProgForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Форма операций";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ProgForm_FormClosed);
            this.tabsCtrl.ResumeLayout(false);
            this.tabManageServer.ResumeLayout(false);
            this.tabManageSite.ResumeLayout(false);
            this.tabManageSite.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSites)).EndInit();
            this.tabManagePool.ResumeLayout(false);
            this.tabManagePool.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPool)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbHello;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.TabControl tabsCtrl;
        private System.Windows.Forms.TabPage tabManageSite;
        private System.Windows.Forms.TabPage tabManagePool;
        private System.Windows.Forms.TabPage tabManageServer;
        private System.Windows.Forms.Button btnAddSite;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DataGridView dgvSites;
        private System.Windows.Forms.Button btnDeleteSite;
        private System.Windows.Forms.Button btnEditSite;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnDeletePool;
        private System.Windows.Forms.Button btnEditPool;
        private System.Windows.Forms.DataGridView dgvPool;
        private System.Windows.Forms.Button btnAddPool;
        private System.Windows.Forms.Button btnStopPool;
        private System.Windows.Forms.Button btnStartPool;
    }
}