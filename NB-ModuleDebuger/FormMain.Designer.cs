namespace NB_ModuleDebuger
{
    partial class FormMain
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.grpPort = new System.Windows.Forms.GroupBox();
            this.btPortCtrl = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.combPortNum = new System.Windows.Forms.ComboBox();
            this.grpNwk = new System.Windows.Forms.GroupBox();
            this.lbPort = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.lbIp = new System.Windows.Forms.Label();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.txtIp = new System.Windows.Forms.TextBox();
            this.combCloudSvr = new System.Windows.Forms.ComboBox();
            this.txtDataUpload = new System.Windows.Forms.TextBox();
            this.lbNetState = new System.Windows.Forms.Label();
            this.btQryNwkStat = new System.Windows.Forms.Button();
            this.btCnctSvr = new System.Windows.Forms.Button();
            this.btDataUpload = new System.Windows.Forms.Button();
            this.btJoinNwk = new System.Windows.Forms.Button();
            this.grpMsg = new System.Windows.Forms.GroupBox();
            this.lbCmdStatus = new System.Windows.Forms.Label();
            this.lbLang = new System.Windows.Forms.Label();
            this.chkShowRxData = new System.Windows.Forms.CheckBox();
            this.chkTime = new System.Windows.Forms.CheckBox();
            this.rTxtMsg = new System.Windows.Forms.RichTextBox();
            this.combLanguage = new System.Windows.Forms.ComboBox();
            this.btSendEsc = new System.Windows.Forms.Button();
            this.btSendCtrlZ = new System.Windows.Forms.Button();
            this.btSave = new System.Windows.Forms.Button();
            this.btClear = new System.Windows.Forms.Button();
            this.grpDevType = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.combModel = new System.Windows.Forms.ComboBox();
            this.grpQuery = new System.Windows.Forms.GroupBox();
            this.btQrySimId = new System.Windows.Forms.Button();
            this.btQryTempVbat = new System.Windows.Forms.Button();
            this.btQryBand = new System.Windows.Forms.Button();
            this.btQryIMEI = new System.Windows.Forms.Button();
            this.btQryVer = new System.Windows.Forms.Button();
            this.grpParam = new System.Windows.Forms.GroupBox();
            this.chkBand28 = new System.Windows.Forms.CheckBox();
            this.chkBand8 = new System.Windows.Forms.CheckBox();
            this.chkBand5 = new System.Windows.Forms.CheckBox();
            this.chkBand3 = new System.Windows.Forms.CheckBox();
            this.btSetBand = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.btSend = new System.Windows.Forms.Button();
            this.combAtCmd = new System.Windows.Forms.ComboBox();
            this.chkHex = new System.Windows.Forms.CheckBox();
            this.grpPort.SuspendLayout();
            this.grpNwk.SuspendLayout();
            this.grpMsg.SuspendLayout();
            this.grpDevType.SuspendLayout();
            this.grpQuery.SuspendLayout();
            this.grpParam.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpPort
            // 
            this.grpPort.BackColor = System.Drawing.SystemColors.Control;
            this.grpPort.Controls.Add(this.btPortCtrl);
            this.grpPort.Controls.Add(this.label1);
            this.grpPort.Controls.Add(this.combPortNum);
            this.grpPort.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.grpPort.Location = new System.Drawing.Point(9, 72);
            this.grpPort.Name = "grpPort";
            this.grpPort.Size = new System.Drawing.Size(272, 59);
            this.grpPort.TabIndex = 0;
            this.grpPort.TabStop = false;
            this.grpPort.Text = "连接设备";
            // 
            // btPortCtrl
            // 
            this.btPortCtrl.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btPortCtrl.Location = new System.Drawing.Point(150, 25);
            this.btPortCtrl.Name = "btPortCtrl";
            this.btPortCtrl.Size = new System.Drawing.Size(109, 25);
            this.btPortCtrl.TabIndex = 2;
            this.btPortCtrl.Text = "打开串口";
            this.btPortCtrl.UseVisualStyleBackColor = true;
            this.btPortCtrl.Click += new System.EventHandler(this.btPortCtrl_Click);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(17, 31);
            this.label1.Name = "label1";
            this.label1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.label1.Size = new System.Drawing.Size(56, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "端口号";
            // 
            // combPortNum
            // 
            this.combPortNum.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.combPortNum.FormattingEnabled = true;
            this.combPortNum.Location = new System.Drawing.Point(77, 28);
            this.combPortNum.Name = "combPortNum";
            this.combPortNum.Size = new System.Drawing.Size(64, 20);
            this.combPortNum.TabIndex = 0;
            this.combPortNum.Click += new System.EventHandler(this.combPortNum_Click);
            // 
            // grpNwk
            // 
            this.grpNwk.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.grpNwk.BackColor = System.Drawing.SystemColors.Control;
            this.grpNwk.Controls.Add(this.chkHex);
            this.grpNwk.Controls.Add(this.lbPort);
            this.grpNwk.Controls.Add(this.label6);
            this.grpNwk.Controls.Add(this.lbIp);
            this.grpNwk.Controls.Add(this.txtPort);
            this.grpNwk.Controls.Add(this.txtIp);
            this.grpNwk.Controls.Add(this.combCloudSvr);
            this.grpNwk.Controls.Add(this.txtDataUpload);
            this.grpNwk.Controls.Add(this.lbNetState);
            this.grpNwk.Controls.Add(this.btQryNwkStat);
            this.grpNwk.Controls.Add(this.btCnctSvr);
            this.grpNwk.Controls.Add(this.btDataUpload);
            this.grpNwk.Controls.Add(this.btJoinNwk);
            this.grpNwk.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.grpNwk.Location = new System.Drawing.Point(9, 348);
            this.grpNwk.Name = "grpNwk";
            this.grpNwk.Size = new System.Drawing.Size(272, 203);
            this.grpNwk.TabIndex = 0;
            this.grpNwk.TabStop = false;
            this.grpNwk.Text = "网络连接";
            // 
            // lbPort
            // 
            this.lbPort.AutoSize = true;
            this.lbPort.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbPort.Location = new System.Drawing.Point(171, 79);
            this.lbPort.Name = "lbPort";
            this.lbPort.Size = new System.Drawing.Size(33, 17);
            this.lbPort.TabIndex = 6;
            this.lbPort.Text = "port";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label6.Location = new System.Drawing.Point(29, 55);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(44, 17);
            this.label6.TabIndex = 6;
            this.label6.Text = "云平台";
            // 
            // lbIp
            // 
            this.lbIp.AutoSize = true;
            this.lbIp.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbIp.Location = new System.Drawing.Point(29, 80);
            this.lbIp.Name = "lbIp";
            this.lbIp.Size = new System.Drawing.Size(19, 17);
            this.lbIp.TabIndex = 6;
            this.lbIp.Text = "ip";
            // 
            // txtPort
            // 
            this.txtPort.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtPort.Location = new System.Drawing.Point(207, 79);
            this.txtPort.Multiline = true;
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(52, 21);
            this.txtPort.TabIndex = 5;
            // 
            // txtIp
            // 
            this.txtIp.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtIp.Location = new System.Drawing.Point(53, 80);
            this.txtIp.Multiline = true;
            this.txtIp.Name = "txtIp";
            this.txtIp.Size = new System.Drawing.Size(114, 21);
            this.txtIp.TabIndex = 5;
            // 
            // combCloudSvr
            // 
            this.combCloudSvr.BackColor = System.Drawing.SystemColors.Control;
            this.combCloudSvr.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.combCloudSvr.FormattingEnabled = true;
            this.combCloudSvr.Items.AddRange(new object[] {
            "OneNet平台",
            "CDP服务器",
            "UDP服务器"});
            this.combCloudSvr.Location = new System.Drawing.Point(77, 55);
            this.combCloudSvr.Name = "combCloudSvr";
            this.combCloudSvr.Size = new System.Drawing.Size(90, 20);
            this.combCloudSvr.TabIndex = 4;
            this.combCloudSvr.SelectedIndexChanged += new System.EventHandler(this.combCloudSvr_SelectedIndexChanged);
            // 
            // txtDataUpload
            // 
            this.txtDataUpload.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.txtDataUpload.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtDataUpload.Location = new System.Drawing.Point(32, 108);
            this.txtDataUpload.Multiline = true;
            this.txtDataUpload.Name = "txtDataUpload";
            this.txtDataUpload.Size = new System.Drawing.Size(169, 64);
            this.txtDataUpload.TabIndex = 3;
            // 
            // lbNetState
            // 
            this.lbNetState.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lbNetState.AutoSize = true;
            this.lbNetState.BackColor = System.Drawing.Color.Silver;
            this.lbNetState.Location = new System.Drawing.Point(3, 180);
            this.lbNetState.Name = "lbNetState";
            this.lbNetState.Size = new System.Drawing.Size(107, 20);
            this.lbNetState.TabIndex = 2;
            this.lbNetState.Text = "入网状态：离线";
            // 
            // btQryNwkStat
            // 
            this.btQryNwkStat.BackColor = System.Drawing.Color.DarkKhaki;
            this.btQryNwkStat.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btQryNwkStat.Location = new System.Drawing.Point(150, 25);
            this.btQryNwkStat.Name = "btQryNwkStat";
            this.btQryNwkStat.Size = new System.Drawing.Size(109, 23);
            this.btQryNwkStat.TabIndex = 1;
            this.btQryNwkStat.Text = "查询网络状态";
            this.btQryNwkStat.UseVisualStyleBackColor = false;
            this.btQryNwkStat.Click += new System.EventHandler(this.btQryNwkStat_Click);
            // 
            // btCnctSvr
            // 
            this.btCnctSvr.BackColor = System.Drawing.Color.DarkKhaki;
            this.btCnctSvr.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btCnctSvr.Location = new System.Drawing.Point(173, 52);
            this.btCnctSvr.Name = "btCnctSvr";
            this.btCnctSvr.Size = new System.Drawing.Size(86, 23);
            this.btCnctSvr.TabIndex = 1;
            this.btCnctSvr.Text = "建立连接";
            this.btCnctSvr.UseVisualStyleBackColor = false;
            this.btCnctSvr.Click += new System.EventHandler(this.btCnctSvr_Click);
            // 
            // btDataUpload
            // 
            this.btDataUpload.BackColor = System.Drawing.Color.DarkKhaki;
            this.btDataUpload.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btDataUpload.Location = new System.Drawing.Point(207, 108);
            this.btDataUpload.Name = "btDataUpload";
            this.btDataUpload.Size = new System.Drawing.Size(52, 42);
            this.btDataUpload.TabIndex = 1;
            this.btDataUpload.Text = "数据\r\n上传";
            this.btDataUpload.UseVisualStyleBackColor = false;
            this.btDataUpload.Click += new System.EventHandler(this.btDataUpload_Click);
            // 
            // btJoinNwk
            // 
            this.btJoinNwk.BackColor = System.Drawing.Color.DarkKhaki;
            this.btJoinNwk.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btJoinNwk.Location = new System.Drawing.Point(32, 25);
            this.btJoinNwk.Name = "btJoinNwk";
            this.btJoinNwk.Size = new System.Drawing.Size(109, 23);
            this.btJoinNwk.TabIndex = 1;
            this.btJoinNwk.Text = "入网";
            this.btJoinNwk.UseVisualStyleBackColor = false;
            this.btJoinNwk.Click += new System.EventHandler(this.btJoinNwk_Click);
            // 
            // grpMsg
            // 
            this.grpMsg.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpMsg.Controls.Add(this.lbCmdStatus);
            this.grpMsg.Controls.Add(this.lbLang);
            this.grpMsg.Controls.Add(this.chkShowRxData);
            this.grpMsg.Controls.Add(this.chkTime);
            this.grpMsg.Controls.Add(this.rTxtMsg);
            this.grpMsg.Controls.Add(this.combLanguage);
            this.grpMsg.Controls.Add(this.btSendEsc);
            this.grpMsg.Controls.Add(this.btSendCtrlZ);
            this.grpMsg.Controls.Add(this.btSave);
            this.grpMsg.Controls.Add(this.btClear);
            this.grpMsg.Location = new System.Drawing.Point(287, 12);
            this.grpMsg.Name = "grpMsg";
            this.grpMsg.Size = new System.Drawing.Size(650, 508);
            this.grpMsg.TabIndex = 0;
            this.grpMsg.TabStop = false;
            this.grpMsg.Text = "通信记录";
            // 
            // lbCmdStatus
            // 
            this.lbCmdStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lbCmdStatus.BackColor = System.Drawing.Color.GreenYellow;
            this.lbCmdStatus.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbCmdStatus.Location = new System.Drawing.Point(5, 458);
            this.lbCmdStatus.Name = "lbCmdStatus";
            this.lbCmdStatus.Size = new System.Drawing.Size(644, 25);
            this.lbCmdStatus.TabIndex = 3;
            this.lbCmdStatus.Text = "命令执行中...";
            this.lbCmdStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbLang
            // 
            this.lbLang.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbLang.Location = new System.Drawing.Point(219, 2);
            this.lbLang.Name = "lbLang";
            this.lbLang.Size = new System.Drawing.Size(78, 12);
            this.lbLang.TabIndex = 0;
            this.lbLang.Text = "界面语言";
            this.lbLang.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // chkShowRxData
            // 
            this.chkShowRxData.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkShowRxData.AutoSize = true;
            this.chkShowRxData.Checked = true;
            this.chkShowRxData.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkShowRxData.Location = new System.Drawing.Point(276, 491);
            this.chkShowRxData.Name = "chkShowRxData";
            this.chkShowRxData.Size = new System.Drawing.Size(72, 16);
            this.chkShowRxData.TabIndex = 2;
            this.chkShowRxData.Text = "显示接收";
            this.chkShowRxData.UseVisualStyleBackColor = true;
            this.chkShowRxData.Visible = false;
            // 
            // chkTime
            // 
            this.chkTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkTime.AutoSize = true;
            this.chkTime.Checked = true;
            this.chkTime.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkTime.Location = new System.Drawing.Point(185, 491);
            this.chkTime.Name = "chkTime";
            this.chkTime.Size = new System.Drawing.Size(72, 16);
            this.chkTime.TabIndex = 2;
            this.chkTime.Text = "显示时间";
            this.chkTime.UseVisualStyleBackColor = true;
            this.chkTime.CheckedChanged += new System.EventHandler(this.chkTime_CheckedChanged);
            // 
            // rTxtMsg
            // 
            this.rTxtMsg.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rTxtMsg.Location = new System.Drawing.Point(3, 17);
            this.rTxtMsg.Name = "rTxtMsg";
            this.rTxtMsg.Size = new System.Drawing.Size(647, 466);
            this.rTxtMsg.TabIndex = 0;
            this.rTxtMsg.Text = "";
            // 
            // combLanguage
            // 
            this.combLanguage.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.combLanguage.FormattingEnabled = true;
            this.combLanguage.Items.AddRange(new object[] {
            "简体中文(zh-CN)",
            "English (en-US)"});
            this.combLanguage.Location = new System.Drawing.Point(303, 0);
            this.combLanguage.Name = "combLanguage";
            this.combLanguage.Size = new System.Drawing.Size(118, 20);
            this.combLanguage.TabIndex = 0;
            this.combLanguage.SelectedIndexChanged += new System.EventHandler(this.combLanguage_SelectedIndexChanged);
            // 
            // btSendEsc
            // 
            this.btSendEsc.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btSendEsc.Location = new System.Drawing.Point(488, 487);
            this.btSendEsc.Name = "btSendEsc";
            this.btSendEsc.Size = new System.Drawing.Size(76, 20);
            this.btSendEsc.TabIndex = 1;
            this.btSendEsc.Text = "发送ESC";
            this.btSendEsc.UseVisualStyleBackColor = true;
            this.btSendEsc.Click += new System.EventHandler(this.btSendEsc_Click);
            // 
            // btSendCtrlZ
            // 
            this.btSendCtrlZ.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btSendCtrlZ.Location = new System.Drawing.Point(389, 487);
            this.btSendCtrlZ.Name = "btSendCtrlZ";
            this.btSendCtrlZ.Size = new System.Drawing.Size(93, 20);
            this.btSendCtrlZ.TabIndex = 1;
            this.btSendCtrlZ.Text = "发送Ctrl-Z";
            this.btSendCtrlZ.UseVisualStyleBackColor = true;
            this.btSendCtrlZ.Click += new System.EventHandler(this.btSendCtrlZ_Click);
            // 
            // btSave
            // 
            this.btSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btSave.Location = new System.Drawing.Point(93, 488);
            this.btSave.Name = "btSave";
            this.btSave.Size = new System.Drawing.Size(70, 20);
            this.btSave.TabIndex = 1;
            this.btSave.Text = "保存记录";
            this.btSave.UseVisualStyleBackColor = true;
            this.btSave.Click += new System.EventHandler(this.btSave_Click);
            // 
            // btClear
            // 
            this.btClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btClear.Location = new System.Drawing.Point(4, 488);
            this.btClear.Name = "btClear";
            this.btClear.Size = new System.Drawing.Size(70, 20);
            this.btClear.TabIndex = 1;
            this.btClear.Text = "清空记录";
            this.btClear.UseVisualStyleBackColor = true;
            this.btClear.Click += new System.EventHandler(this.btClear_Click);
            // 
            // grpDevType
            // 
            this.grpDevType.BackColor = System.Drawing.SystemColors.Control;
            this.grpDevType.Controls.Add(this.label3);
            this.grpDevType.Controls.Add(this.combModel);
            this.grpDevType.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.grpDevType.Location = new System.Drawing.Point(9, 12);
            this.grpDevType.Name = "grpDevType";
            this.grpDevType.Size = new System.Drawing.Size(272, 54);
            this.grpDevType.TabIndex = 1;
            this.grpDevType.TabStop = false;
            this.grpDevType.Text = "模组选择";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(42, 28);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "型号";
            // 
            // combModel
            // 
            this.combModel.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.combModel.Items.AddRange(new object[] {
            "NH01A",
            "NR01A"});
            this.combModel.Location = new System.Drawing.Point(77, 25);
            this.combModel.Name = "combModel";
            this.combModel.Size = new System.Drawing.Size(64, 20);
            this.combModel.TabIndex = 0;
            this.combModel.SelectedIndexChanged += new System.EventHandler(this.combModel_SelectedIndexChanged);
            // 
            // grpQuery
            // 
            this.grpQuery.BackColor = System.Drawing.SystemColors.Control;
            this.grpQuery.Controls.Add(this.btQrySimId);
            this.grpQuery.Controls.Add(this.btQryTempVbat);
            this.grpQuery.Controls.Add(this.btQryBand);
            this.grpQuery.Controls.Add(this.btQryIMEI);
            this.grpQuery.Controls.Add(this.btQryVer);
            this.grpQuery.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.grpQuery.Location = new System.Drawing.Point(9, 141);
            this.grpQuery.Name = "grpQuery";
            this.grpQuery.Size = new System.Drawing.Size(272, 118);
            this.grpQuery.TabIndex = 0;
            this.grpQuery.TabStop = false;
            this.grpQuery.Text = "信息查询";
            // 
            // btQrySimId
            // 
            this.btQrySimId.BackColor = System.Drawing.Color.DarkKhaki;
            this.btQrySimId.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btQrySimId.Location = new System.Drawing.Point(150, 58);
            this.btQrySimId.Name = "btQrySimId";
            this.btQrySimId.Size = new System.Drawing.Size(109, 23);
            this.btQrySimId.TabIndex = 2;
            this.btQrySimId.Text = "SIM卡ID";
            this.btQrySimId.UseVisualStyleBackColor = false;
            this.btQrySimId.Click += new System.EventHandler(this.btQrySimId_Click);
            // 
            // btQryTempVbat
            // 
            this.btQryTempVbat.BackColor = System.Drawing.Color.DarkKhaki;
            this.btQryTempVbat.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btQryTempVbat.Location = new System.Drawing.Point(32, 89);
            this.btQryTempVbat.Name = "btQryTempVbat";
            this.btQryTempVbat.Size = new System.Drawing.Size(109, 23);
            this.btQryTempVbat.TabIndex = 1;
            this.btQryTempVbat.Text = "温度、电池电压";
            this.btQryTempVbat.UseVisualStyleBackColor = false;
            this.btQryTempVbat.Click += new System.EventHandler(this.btQryTempVbat_Click);
            // 
            // btQryBand
            // 
            this.btQryBand.BackColor = System.Drawing.Color.DarkKhaki;
            this.btQryBand.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btQryBand.Location = new System.Drawing.Point(32, 58);
            this.btQryBand.Name = "btQryBand";
            this.btQryBand.Size = new System.Drawing.Size(109, 23);
            this.btQryBand.TabIndex = 1;
            this.btQryBand.Text = "BAND值";
            this.btQryBand.UseVisualStyleBackColor = false;
            this.btQryBand.Click += new System.EventHandler(this.btQryBand_Click);
            // 
            // btQryIMEI
            // 
            this.btQryIMEI.BackColor = System.Drawing.Color.DarkKhaki;
            this.btQryIMEI.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btQryIMEI.Location = new System.Drawing.Point(150, 26);
            this.btQryIMEI.Name = "btQryIMEI";
            this.btQryIMEI.Size = new System.Drawing.Size(109, 23);
            this.btQryIMEI.TabIndex = 1;
            this.btQryIMEI.Text = "IMEI号码";
            this.btQryIMEI.UseVisualStyleBackColor = false;
            this.btQryIMEI.Click += new System.EventHandler(this.btQryIMEI_Click);
            // 
            // btQryVer
            // 
            this.btQryVer.BackColor = System.Drawing.Color.DarkKhaki;
            this.btQryVer.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btQryVer.Location = new System.Drawing.Point(32, 26);
            this.btQryVer.Name = "btQryVer";
            this.btQryVer.Size = new System.Drawing.Size(109, 23);
            this.btQryVer.TabIndex = 1;
            this.btQryVer.Text = "版本号";
            this.btQryVer.UseVisualStyleBackColor = false;
            this.btQryVer.Click += new System.EventHandler(this.btQryVer_Click);
            // 
            // grpParam
            // 
            this.grpParam.BackColor = System.Drawing.SystemColors.Control;
            this.grpParam.Controls.Add(this.chkBand28);
            this.grpParam.Controls.Add(this.chkBand8);
            this.grpParam.Controls.Add(this.chkBand5);
            this.grpParam.Controls.Add(this.chkBand3);
            this.grpParam.Controls.Add(this.btSetBand);
            this.grpParam.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.grpParam.Location = new System.Drawing.Point(9, 275);
            this.grpParam.Name = "grpParam";
            this.grpParam.Size = new System.Drawing.Size(272, 56);
            this.grpParam.TabIndex = 0;
            this.grpParam.TabStop = false;
            this.grpParam.Text = "参数设置";
            // 
            // chkBand28
            // 
            this.chkBand28.AutoSize = true;
            this.chkBand28.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.chkBand28.Location = new System.Drawing.Point(131, 29);
            this.chkBand28.Name = "chkBand28";
            this.chkBand28.Size = new System.Drawing.Size(36, 16);
            this.chkBand28.TabIndex = 4;
            this.chkBand28.Text = "28";
            this.chkBand28.UseVisualStyleBackColor = true;
            // 
            // chkBand8
            // 
            this.chkBand8.AutoSize = true;
            this.chkBand8.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.chkBand8.Location = new System.Drawing.Point(99, 29);
            this.chkBand8.Name = "chkBand8";
            this.chkBand8.Size = new System.Drawing.Size(30, 16);
            this.chkBand8.TabIndex = 4;
            this.chkBand8.Text = "8";
            this.chkBand8.UseVisualStyleBackColor = true;
            // 
            // chkBand5
            // 
            this.chkBand5.AutoSize = true;
            this.chkBand5.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.chkBand5.Location = new System.Drawing.Point(65, 29);
            this.chkBand5.Name = "chkBand5";
            this.chkBand5.Size = new System.Drawing.Size(30, 16);
            this.chkBand5.TabIndex = 4;
            this.chkBand5.Text = "5";
            this.chkBand5.UseVisualStyleBackColor = true;
            // 
            // chkBand3
            // 
            this.chkBand3.AutoSize = true;
            this.chkBand3.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.chkBand3.Location = new System.Drawing.Point(32, 29);
            this.chkBand3.Name = "chkBand3";
            this.chkBand3.Size = new System.Drawing.Size(30, 16);
            this.chkBand3.TabIndex = 4;
            this.chkBand3.Text = "3";
            this.chkBand3.UseVisualStyleBackColor = true;
            // 
            // btSetBand
            // 
            this.btSetBand.BackColor = System.Drawing.Color.DarkKhaki;
            this.btSetBand.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btSetBand.Location = new System.Drawing.Point(173, 25);
            this.btSetBand.Name = "btSetBand";
            this.btSetBand.Size = new System.Drawing.Size(86, 25);
            this.btSetBand.TabIndex = 1;
            this.btSetBand.Text = "BAND设置";
            this.btSetBand.UseVisualStyleBackColor = false;
            this.btSetBand.Click += new System.EventHandler(this.btSetBand_Click);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.SystemColors.Control;
            this.label2.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(288, 531);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(79, 17);
            this.label2.TabIndex = 2;
            this.label2.Text = "  AT 指 令 ：";
            // 
            // btSend
            // 
            this.btSend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btSend.BackColor = System.Drawing.Color.DarkKhaki;
            this.btSend.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btSend.Location = new System.Drawing.Point(857, 526);
            this.btSend.Name = "btSend";
            this.btSend.Size = new System.Drawing.Size(80, 25);
            this.btSend.TabIndex = 4;
            this.btSend.Text = "发送";
            this.btSend.UseVisualStyleBackColor = false;
            this.btSend.Click += new System.EventHandler(this.btSend_Click);
            // 
            // combAtCmd
            // 
            this.combAtCmd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.combAtCmd.FormattingEnabled = true;
            this.combAtCmd.Location = new System.Drawing.Point(380, 528);
            this.combAtCmd.Name = "combAtCmd";
            this.combAtCmd.Size = new System.Drawing.Size(471, 20);
            this.combAtCmd.TabIndex = 4;
            // 
            // chkHex
            // 
            this.chkHex.AutoSize = true;
            this.chkHex.Location = new System.Drawing.Point(207, 152);
            this.chkHex.Name = "chkHex";
            this.chkHex.Size = new System.Drawing.Size(54, 24);
            this.chkHex.TabIndex = 7;
            this.chkHex.Text = "Hex";
            this.chkHex.UseVisualStyleBackColor = true;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(949, 564);
            this.Controls.Add(this.combAtCmd);
            this.Controls.Add(this.btSend);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.grpDevType);
            this.Controls.Add(this.grpMsg);
            this.Controls.Add(this.grpParam);
            this.Controls.Add(this.grpQuery);
            this.Controls.Add(this.grpNwk);
            this.Controls.Add(this.grpPort);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormMain";
            this.Text = "NB模组调试软件";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormMain_FormClosed);
            this.grpPort.ResumeLayout(false);
            this.grpNwk.ResumeLayout(false);
            this.grpNwk.PerformLayout();
            this.grpMsg.ResumeLayout(false);
            this.grpMsg.PerformLayout();
            this.grpDevType.ResumeLayout(false);
            this.grpDevType.PerformLayout();
            this.grpQuery.ResumeLayout(false);
            this.grpParam.ResumeLayout(false);
            this.grpParam.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox grpPort;
        private System.Windows.Forms.GroupBox grpNwk;
        private System.Windows.Forms.GroupBox grpMsg;
        private System.Windows.Forms.RichTextBox rTxtMsg;
        private System.Windows.Forms.Button btPortCtrl;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox combPortNum;
        private System.Windows.Forms.Button btJoinNwk;
        private System.Windows.Forms.GroupBox grpDevType;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox combModel;
        private System.Windows.Forms.Button btQryNwkStat;
        private System.Windows.Forms.GroupBox grpQuery;
        private System.Windows.Forms.Button btQryIMEI;
        private System.Windows.Forms.Button btQryVer;
        private System.Windows.Forms.GroupBox grpParam;
        private System.Windows.Forms.Button btSetBand;
        private System.Windows.Forms.Button btQryBand;
        private System.Windows.Forms.Button btSave;
        private System.Windows.Forms.Button btClear;
        private System.Windows.Forms.Label lbNetState;
        private System.Windows.Forms.Button btQrySimId;
        private System.Windows.Forms.ComboBox combCloudSvr;
        private System.Windows.Forms.TextBox txtDataUpload;
        private System.Windows.Forms.Button btDataUpload;
        private System.Windows.Forms.CheckBox chkBand28;
        private System.Windows.Forms.CheckBox chkBand8;
        private System.Windows.Forms.CheckBox chkBand5;
        private System.Windows.Forms.CheckBox chkBand3;
        private System.Windows.Forms.CheckBox chkTime;
        private System.Windows.Forms.Button btQryTempVbat;
        private System.Windows.Forms.CheckBox chkShowRxData;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btSend;
        private System.Windows.Forms.Button btCnctSvr;
        private System.Windows.Forms.TextBox txtIp;
        private System.Windows.Forms.Label lbIp;
        private System.Windows.Forms.Label lbPort;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.Label lbLang;
        private System.Windows.Forms.ComboBox combLanguage;
        private System.Windows.Forms.Label lbCmdStatus;
        private System.Windows.Forms.Button btSendEsc;
        private System.Windows.Forms.Button btSendCtrlZ;
        private System.Windows.Forms.ComboBox combAtCmd;
        private System.Windows.Forms.CheckBox chkHex;
    }
}

