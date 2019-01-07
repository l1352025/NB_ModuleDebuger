using ElectricPowerDebuger.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace NB_ModuleDebuger
{
    public partial class FormMain : Form
    {
        private string _configPath;
        private SerialCom _scom;
        private ConcurrentQueue<byte[]> _recvQueue;
        private ConcurrentQueue<Command> _sendQueue;
        private Thread _thrTransceiver;
        private bool _IsSendNewCmd;

        public FormMain()
        {
            InitializeComponent();
            this.Text = Application.ProductName + "_v" + Application.ProductVersion + "    " + Application.CompanyName;
            _configPath = Application.ExecutablePath.Substring(0, Application.ExecutablePath.Length - 4) + ".cfg";

            _scom = new SerialCom();
            _scom.DataReceivedEvent += serialPort_DataReceived;
            _scom.UnexpectedClosedEvent += serialPort_UnexpectedClosed;
            _recvQueue = new ConcurrentQueue<byte[]>();
            _sendQueue = new ConcurrentQueue<Command>();
            _thrTransceiver = new Thread(TransceiverHandle);
            _thrTransceiver.IsBackground = true; 
            _thrTransceiver.Start();

            MultiLanguage.InitLanguage(this);
            string title = LanguageDict.zh_CN_To_en_US[Application.ProductName] + "_v" + Application.ProductVersion + "    " + LanguageDict.zh_CN_To_en_US[Application.CompanyName];
            LanguageDict.zh_CN_To_en_US.Add(this.Text, title);
            if (Thread.CurrentThread.CurrentCulture.Name.StartsWith("zh-", StringComparison.OrdinalIgnoreCase))
            {
                combLanguage.SelectedIndex = 0;
            }
            else if (Thread.CurrentThread.CurrentCulture.Name.StartsWith("en-", StringComparison.OrdinalIgnoreCase))
            {
                combLanguage.SelectedIndex = 1;
            }

            combModel.Text = XmlHelper.GetNodeDefValue(_configPath, "/Config/Model", "NH01A");
            combPortNum.Text = XmlHelper.GetNodeDefValue(_configPath, "/Config/PortName", "");
            string strBand = XmlHelper.GetNodeDefValue(_configPath, "/Config/Band", "3,5,8");
            BandChkSet(strBand);
            chkTime.Checked = (XmlHelper.GetNodeDefValue(_configPath, "/Config/ShowTime", "") == "true" ? true : false);
            string strCustomAtCmds = XmlHelper.GetNodeDefValue(_configPath, "/Config/CustomAtCmds", "");
            combAtCmd.Items.AddRange(strCustomAtCmds.Split(new char[]{'/'}, StringSplitOptions.RemoveEmptyEntries));
            if(combModel.Text == "NH01A")
            {
                combModel.SelectedIndex = 0;
            }
            else
            {
                combModel.SelectedIndex = 1;
            }

            UiOperateDisable();
        }

        private delegate void CommandHandler(Command cmd);
        private class Command
        {
            public string Name;
            public byte[] TxBuf;
            public byte[] RxBuf;
            public List<string> Params;
            public int TimeWaitMS;
            public int RetryTimes;
            public bool IsEnable;
            public CommandHandler SendFunc;
            public CommandHandler RecvFunc;

            public Command()
                : this(null, null, null, 0, 3)
            {
            }
            public Command(string cmdName, CommandHandler sendFunc, CommandHandler recvFunc, int timeOut, int retryTimes)
            {
                Name = cmdName;
                TimeWaitMS = timeOut;
                RetryTimes = retryTimes;
                SendFunc = sendFunc;
                RecvFunc = recvFunc;
                Params = new List<string>();
            }
        };

        delegate void CallbackFunc(params object[] args);
        private CallbackFunc _cmdEndCallback;
        private object[] _argsEndCallback;
        private Command CurrentCmd = null;
        private string _strMsgBuf = "";
        private string _strMsgMain = "";
        private string _strMsgSub = "";
        

        #region AT 指令
        private readonly string[] AT_CmdTbl_NH01A = new string[]
        {
         /*--命令名            命令字符串  --*/
            // 入网流程
            "模组复位           AT+NRB                                       ",
            "模组配置           AT+NCONFIG=AUTOCONNECT,TRUE;+NCONFIG=CELL_RESELECTION,TRUE;+CMEE=1  ",
            "模组配置1          AT+NCONFIG=CR_0354_0338_SCRAMBLING,TRUE                             ",
            "模组配置2          AT+NCONFIG=CR_0859_SI_AVOID,TRUE                                    ",
            "打开SIM卡          AT+CFUN=1                                    ",
            "入网激活           AT+CGATT=1                                   ",
            "查询入网状态       AT+CGATT?                                    ",
            "查询网络状态信息   AT+NUESTATS                                  ",

            // 信息查询
            "查询模组型号       AT+SRMTYPE                                   ",
            "查询版本号         AT+CGMR                                      ",
            "查询IMEI           AT+CGSN=1                                    ",
            "查询SIM卡ID        AT+CIMI                                      ",
            "查询制造商         AT+CGMI                                      ",
            "查询COAP协议IP     AT+NCDP?                                     ",
            "查询BAND           AT+NBAND?                                    ",
            "查询设备IP         AT+CGPADDR                                   ",
            "查询发送/接受状态  AT+NSMI?;+NNMI?                              ",
            "查询温度和电压     AT+NCHIPINFO=ALL                             ",

            // 参数设置 
            "关闭SIM卡          AT+CFUN=0                                    ",
            "设置BAND           AT+NBAND=3,5,8,28                            ",
            "设置COAP协议IP     AT+NCDP=180.101.147.115,5683                 ",
            "打开接收上报       AT+NNMI=1                                    ",
            "发送数据           AT+NMGS=5,1122334455                         ",

            // tcp/udp通信
            "创建通信Socket     AT+NSOCR=<type>,<protocol>,<listen port>[,<receive control>[,<af_type>]]       ",
            "发送UDP数据        AT+NSOST=<socket>,<remote_addr>, <remote_port>,<length>,<data>[,<sequence>]        ",
            "发送(Flg)UDP数据   AT+NSOSTF=<socket>,<remote_addr>, <remote_port>,<flag>,<length>,<data>[,<sequence>]       ",
            "建立TCP连接        AT+NSOCO=<socket>,<remote_addr>,<remote_port>           ",
            "发送TCP数据        AT+NSOSD=<socket>,<length>,<data>[,<flag>[,sequence]]   ",
            "查询Socket发送缓存 AT+NQSOS=<socket>[,<socket> [,<socket>[...]]]           ",
            "收到Socket消息通知 AT+NSONMI:<socket>,<length>                             ",   // 应答仅有
            "接收Socket数据     AT+NSORF=<socket>,<length>                              ",
            "关闭通信Socket     AT+NSOCL=<socket>                                       ",
        };
        private readonly string[] AT_CmdTbl_NR01A = new string[]
        {
         /*--命令名            命令字符串  --*/
            // 入网流程
            "模组复位及入网     AT+NRB                                       ",
            "打开SIM卡          AT+CFUN=1                                    ",
            "入网激活           AT+CGATT=1                                   ",
            "查询入网状态       AT+CGATT?                                    ",
            "激活PDP上下文      AT+CGACT=1,1                                 ",
            "查询网络状态信息   AT+TUESTATS=\"ALL\"                          ",

            // 信息查询
            "查询模组型号       AT+SRMTYPE                                   ",
            "查询软件版本       AT+CGMR                                      ",
            "查询硬件版本       AT+SRHWVER                                   ",
            "查询IMEI           AT+CGSN=1                                    ",
            "查询SIM卡ID        AT+CIMI                                      ",
            "查询信号质量       AT+CSQ                                       ",
            "查询制造商         AT+CGMI                                      ",
            "查询COAP协议IP     AT+NCDP?                                     ",
            "查询BAND           AT+NVSETBAND?                                ",
            "查询设备IP地址     AT+CGPADDR                                   ",
            "查询发送/接受状态  AT+NSMI?;+NNMI?                              ",

            //参数设置
            "关闭SIM卡          AT+CFUN=0                                    ",
            "设置IMEI           AT+EGMR=1,7,\"862295040010228\"              ",
            "设置BAND           AT+NVSETBAND=3,5,8,28                        ",
            "设置COAP协议IP     AT+NCDPOPEN=\"180.101.147.115\",5683         ",
            "打开接收上报       AT+NNMI=1                                    ",
            "发送数据           AT+NMGS=5,1122334455                         ",
            "查看接收缓存       AT+NMGR                                      ",
            "关闭CDP连接        AT+NCDPCLOSE                                 ",

            //OneNet通信
            "查询通信套件版本   AT+MIPLVER?                                  ",
            "创建通信套件       AT+MIPLCREATE                                ",
            "添加设备对象       AT+MIPLADDOBJ=0,3,1,\"1\",2,0                ",
            "注册设备           AT+MIPLOPEN=0,1800,30                        ",
            "观察对象应答       AT+MIPLOBSERVERSP=0,216555,1                 ",
            "发现对象资源应答   AT+MIPLDISCOVERRSP=0,19944,1,4,\"3;10\"      ",
            "上报对象资源       AT+MIPLNOTIFY=0,151015,3,0,3,1,4,\"v1.0\",0,0          ",
            "读对象资源应答     AT+MIPLREADRSP=0,48162,1,3,0,3,1,4,\"v1.0\",0,0        ",
            "写对象资源应答     AT+MIPLWRITERSP=0,61344,2                              ",
            "写对象参数应答     AT+MIPLPARAMETERRSP=0,61346,2                          ",
            "执行对象资源应答   AT+MIPLEXECUTERSP=0,61345,2                            ",
            "更新注册信息       AT+MIPLUPDATE=0,0,0                          ",
            "删除设备对象       AT+MIPLDELOBJ=0,3                            ",
            "注销设备           AT+MIPLCLOSE=0                               ",
            "删除通信套件       AT+MIPLDELETE=0                              ",

            // udp通信
            "创建通信Socket     AT+TSOCR=<type>,<protocol>,<listen port>[,<receive control>]       ",
            "发送UDP数据        AT+TSOST=<socket>,<remote_addr>, <remote_port>,<length>,<data>[,<sequence>] ",
            "接收Socket数据     AT+TSORF=<socket>,<length>                              ",
            "关闭通信Socket     AT+TSOCL=<socket>                                       ",

        };
        #endregion


        #region 串口通信

        //串口选择
        private void combPortNum_Click(object sender, EventArgs e)
        {
            combPortNum.Items.Clear();
            combPortNum.Items.AddRange(SerialPort.GetPortNames());
        }

        //串口打开/关闭
        private void btPortCtrl_Click(object sender, EventArgs e)
        {
            if (combPortNum.Text == "")
            {
                return;
            }

            string ctrlText = MultiLanguage.GetControlDefaultText(btPortCtrl);
            if (ctrlText == "打开串口" && true == PortCtrl(true))
            {
                btPortCtrl.Text = "关闭串口";
                btPortCtrl.BackColor = Color.GreenYellow;
                combPortNum.Enabled = false;

                XmlHelper.SetNodeValue(_configPath, "/Config", "PortName", combPortNum.Text);

                ModuleCheck();
            }
            else
            {
                PortCtrl(false);
                btPortCtrl.Text = "打开串口";
                btPortCtrl.BackColor = Color.Silver;
                combPortNum.Enabled = true;

                UiOperateDisable();
                if (CurrentCmd != null)
                {
                    CurrentCmd.RetryTimes = 0;
                    CurrentCmd.TimeWaitMS = 0;
                }
                UpdateNetStatus("离线");
                UpdateCnctStatus("未连接");
            }

            MultiLanguage.UpdateControlDefaultText(btPortCtrl);
        }

        private bool PortCtrl(bool ctrl)
        {
            if (true == ctrl)
            {
                if (_scom.IsOpen == false)
                {
                    try
                    {
                        string strBaud = "9600";
                        if(combModel.Text == "NH01A")
                        {
                            strBaud = "9600";
                        }
                        else if (combModel.Text == "NR01A")
                        {
                            strBaud = "57600";
                        }

                        _scom.Config(combPortNum.Text, Convert.ToInt32(strBaud), "8N1");
                        _scom.Open();
                    }
                    catch (Exception ex)
                    {
                        ShowMsg(GetCurrentLang("打开串口失败") + "：" + ex.Message + "\r\n", Color.Red);
                        return false;
                    }
                }
            }
            else
            {
                try
                {
                    _scom.Close();
                }
                catch (System.Exception ex)
                {
                    ShowMsg(GetCurrentLang("关闭串口失败") + "：" + ex.Message + "\r\n", Color.Red);
                    return false;
                }
            }
            return true;
        }

        //串口发送
        private bool serialPort_SendData(byte[] buf)
        {
            bool ret = false;

            if (true == _scom.IsOpen)
            {
                try
                {
                    _scom.WritePort(buf, 0, buf.Length);
                    ret = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("serialPort_SendData error :" + ex.Message);
                }
            }

            return ret;
        }

        //串口接收
        private void serialPort_DataReceived(byte[] buf)
        {
            _recvQueue.Enqueue(buf);
        }

        //串口异常断开
        private void serialPort_UnexpectedClosed(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                Invoke(new EventHandler(serialPort_UnexpectedClosed), new object[]{ sender, e});
                return;
            }

            btPortCtrl_Click(null, null);

            ShowMsg(GetCurrentLang("[ 串口连接已断开 ]") + " \r\n\r\n", Color.Red);
        }
        #endregion

        #region 命令处理--发送、接收

        // 发送、接收处理
        private void TransceiverHandle()
        {
            Command cmd = null;
            TimeSpan timeWait = TimeSpan.MaxValue;
            DateTime lastSendTime = DateTime.Now;
            int sendTimes = 0;
            string msg = "";
            bool enableUI = true;

            while (Thread.CurrentThread.IsAlive)
            {
                if(MultiLanguage.CurrentLanguage != Thread.CurrentThread.CurrentUICulture.Name)
                {
                    Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.GetCultureInfo(MultiLanguage.CurrentLanguage);
                }

                // send a new command
                if (_IsSendNewCmd && _sendQueue.Count > 0 && _sendQueue.TryDequeue(out cmd))
                {
                    cmd.IsEnable = true;
                    timeWait = TimeSpan.MaxValue;
                    sendTimes = 0;
                    CurrentCmd = cmd;
                    UiOperateDisable("show executing");
                }

                // send and retry
                if (cmd != null && cmd.IsEnable && timeWait.TotalMilliseconds > cmd.TimeWaitMS)
                {
                    if (cmd.RetryTimes > 0)
                    {
                        cmd.SendFunc(cmd);
                        sendTimes++;
                        cmd.RetryTimes--;
                        lastSendTime = DateTime.Now;
                        _IsSendNewCmd = false;
                    }
                    else
                    {
                        cmd.IsEnable = false;
                        _IsSendNewCmd = true;
                        enableUI = true;

                        if(cmd.Params[0] != "")
                        {
                            msg = GetCurrentLang(cmd.Params[0]) + GetCurrentLang("失败");

                            if (cmd.Params[0].Contains("模组检测"))
                            {
                                enableUI = false;
                                string model = XmlHelper.GetNodeDefValue(_configPath, "/Config/Model", "NH01A");
                                msg += "：" + GetCurrentLang("连接的可能不是") + " " + model + " " + GetCurrentLang("模组");
                            }
                            ShowMsg(msg + "\r\n\r\n", Color.Red);
                        }
                        else if (cmd.Params.Count < 4 || cmd.Params[3] != "自定义")
                        {
                            msg = GetCurrentLang(cmd.Name) + GetCurrentLang("失败");
                            ShowMsg(msg + "\r\n\r\n", Color.Red);
                        }


                        while (_sendQueue.Count > 0)
                        {
                            Command ignoredCmd;
                            _sendQueue.TryDequeue(out ignoredCmd);
                        }
                        
                        _strMsgBuf = "";
                        _strMsgMain = "";
                        _strMsgSub = "";
                        cmd.Params[0] = "";
                        cmd.Params[2] = "";
                        cmd.Name = "Idle状态";

                        if (enableUI)
                        {
                            UiOperateEnable();
                        }
                        else
                        {
                            UiOperateDisable();
                        }

                        if (_cmdEndCallback != null)
                        {
                            _cmdEndCallback(_argsEndCallback);
                            _cmdEndCallback = null;
                        }
                    }
                }

                // receive
                if (cmd != null && _recvQueue.Count > 0 && _recvQueue.TryDequeue(out cmd.RxBuf))
                {
                    cmd.RecvFunc(cmd);

                    if (cmd.IsEnable == false)
                    {
                        if (_sendQueue.Count == 0)
                        {
                            if (cmd.Params[0] != "")
                            {
                                msg = GetCurrentLang(cmd.Params[0]) + GetCurrentLang("成功") + (_strMsgSub != "" ? "：" + _strMsgSub : "");
                                ShowMsg(msg + "\r\n\r\n", Color.Blue);

                                if (cmd.Params[0] == "连接平台")
                                {
                                    UpdateCnctStatus("已连接");
                                }
                                else if (cmd.Params[0] == "入网")
                                {
                                    UpdateNetStatus("在线");
                                }
                                
                            }
                            else if (_strMsgMain != "")
                            {
                                ShowMsg(_strMsgMain + "\r\n\r\n", Color.Blue);
                            }

                            _strMsgBuf = "";
                            _strMsgMain = "";
                            _strMsgSub = "";
                            cmd.Params[0] = "";
                            cmd.Params[2] = "";
                            cmd.Name = "Idle状态";
                            
                            UiOperateEnable();

                            if (_cmdEndCallback != null)
                            {
                                _cmdEndCallback(_argsEndCallback);
                                _cmdEndCallback = null;
                            }
                        }
                    }
                }

                // wait
                Thread.Sleep(50);

                timeWait = DateTime.Now - lastSendTime;
            }
        }
        #endregion

        #region 模组选择、检测
        private static int combModel_SelectedIndex_old = -1;
        private void combModel_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (combModel.SelectedIndex < 0 || combModel.SelectedIndex == combModel_SelectedIndex_old)
            {
                return;
            }
            combModel_SelectedIndex_old = combModel.SelectedIndex;

            string model = GetDefaultLang(combModel.Text);
            XmlHelper.SetNodeValue(_configPath, "/Config", "Model", model);

            if (model == "NH01A")
            {
                combCloudSvr.Items.Clear();
                combCloudSvr.Items.Add("CDP服务器");
                combCloudSvr.Items.Add("UDP服务器");
                btQryTempVbat.Visible = true;
            }
            else
            {
                combCloudSvr.Items.Clear();
                combCloudSvr.Items.Add("CDP服务器");
                combCloudSvr.Items.Add("UDP服务器");
                combCloudSvr.Items.Add("OneNet平台");
                btQryTempVbat.Visible = false;
            }
            combCloudSvr.SelectedIndex = 0;

            MultiLanguage.UpdateControlDefaultText(combCloudSvr);
        }
        private void ModuleCheck()
        {
            Command cmd;

            if (_sendQueue.Count > 0) return;

            string strModelType = XmlHelper.GetNodeDefValue(_configPath, "/Config/Model", "NH01A");

            cmd = new Command();
            cmd.Name = "查询模组型号";
            cmd.SendFunc = SendCmd;
            cmd.RecvFunc = RecvCmd;
            cmd.TimeWaitMS = 500;
            cmd.RetryTimes = 3;
            cmd.Params.Add("模组检测");
            _sendQueue.Enqueue(cmd);

            cmd = new Command();
            cmd.Name = "查询BAND";
            cmd.SendFunc = SendCmd;
            cmd.RecvFunc = RecvCmd;
            cmd.TimeWaitMS = 500;
            cmd.RetryTimes = 3;
            cmd.Params.Add("模组检测");
            _sendQueue.Enqueue(cmd);

            _IsSendNewCmd = true;

            ShowMsg(GetCurrentLang("模组检测中...") + "  \r\n", Color.Blue);
        }
        #endregion

        #region 多语言选择
        private void combLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (combLanguage.SelectedIndex < 0) return;

            string oldname = Thread.CurrentThread.CurrentUICulture.Name;

            if (combLanguage.SelectedIndex == 0)
            {
                Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.GetCultureInfo("zh-CN");
            }
            else if (combLanguage.SelectedIndex == 1)
            {
                Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.GetCultureInfo("en-US");
            }

            if (oldname != Thread.CurrentThread.CurrentUICulture.Name)
            {
                MultiLanguage.SetControlLanguageText(this);
                ShowMsg("Language Switch To ：" + combLanguage.Text + "\r\n", Color.Green);
            }
        }

        private string GetDefaultLang(string value)
        {
            return MultiLanguage.GetDefaultText(value);
        }
        private string GetCurrentLang(string value)
        {
            return MultiLanguage.GetCurrentText(value);
        }
        #endregion

        #region 信息查询
        private void btQryVer_Click(object sender, EventArgs e)
        {
            Command cmd;

            if (_sendQueue.Count > 0) return;

            string strModelType = XmlHelper.GetNodeDefValue(_configPath, "/Config/Model", "NH01A");

            if (strModelType == "NR01A")
            {
                cmd = new Command();
                cmd.Name = "查询软件版本";
                cmd.SendFunc = SendCmd;
                cmd.RecvFunc = RecvCmd;
                cmd.TimeWaitMS = 500;
                cmd.RetryTimes = 3;
                _sendQueue.Enqueue(cmd);

                cmd = new Command();
                cmd.Name = "查询硬件版本";
                cmd.SendFunc = SendCmd;
                cmd.RecvFunc = RecvCmd;
                cmd.TimeWaitMS = 500;
                cmd.RetryTimes = 3;
                _sendQueue.Enqueue(cmd);
            }
            else
            {
                cmd = new Command();
                cmd.Name = "查询版本号";
                cmd.SendFunc = SendCmd;
                cmd.RecvFunc = RecvCmd;
                cmd.TimeWaitMS = 500;
                cmd.RetryTimes = 3;
                _sendQueue.Enqueue(cmd);
            }

            _IsSendNewCmd = true;
            
        }

        private void btQryIMEI_Click(object sender, EventArgs e)
        {
            Command cmd;

            if (_sendQueue.Count > 0) return;

            cmd = new Command();
            cmd.Name = "查询IMEI";
            cmd.SendFunc = SendCmd;
            cmd.RecvFunc = RecvCmd;
            cmd.TimeWaitMS = 500;
            cmd.RetryTimes = 3;
            _sendQueue.Enqueue(cmd);

            _IsSendNewCmd = true;
        }

        private void btQryBand_Click(object sender, EventArgs e)
        {
            Command cmd;

            if (_sendQueue.Count > 0) return;

            cmd = new Command();
            cmd.Name = "查询BAND";
            cmd.SendFunc = SendCmd;
            cmd.RecvFunc = RecvCmd;
            cmd.TimeWaitMS = 500;
            cmd.RetryTimes = 3;
            _sendQueue.Enqueue(cmd);

            _IsSendNewCmd = true;
        }

        private void btQrySimId_Click(object sender, EventArgs e)
        {
            Command cmd;

            if (_sendQueue.Count > 0) return;

            cmd = new Command();
            cmd.Name = "查询SIM卡ID";
            cmd.SendFunc = SendCmd;
            cmd.RecvFunc = RecvCmd;
            cmd.TimeWaitMS = 500;
            cmd.RetryTimes = 3;
            _sendQueue.Enqueue(cmd);

            _IsSendNewCmd = true;
        }
        private void btQryTempVbat_Click(object sender, EventArgs e)
        {
            Command cmd;

            if (_sendQueue.Count > 0) return;

            cmd = new Command();
            cmd.Name = "查询温度和电压";
            cmd.SendFunc = SendCmd;
            cmd.RecvFunc = RecvCmd;
            cmd.TimeWaitMS = 500;
            cmd.RetryTimes = 3;
            _sendQueue.Enqueue(cmd);

            _IsSendNewCmd = true;
        }
        #endregion

        #region 参数设置
        private void BandChkSet(string str)
        {
            if (this.InvokeRequired)
            {
                Invoke(new UpdateUi(BandChkSet), str);
                return;
            }

            chkBand3.Checked = (str.Split(',').Contains(chkBand3.Text) ?  true : false);
            chkBand5.Checked = (str.Split(',').Contains(chkBand5.Text) ? true : false);
            chkBand8.Checked = (str.Split(',').Contains(chkBand8.Text) ? true : false);
            chkBand28.Checked = (str.Split(',').Contains(chkBand28.Text) ? true : false);
        }

        private string BandChkGet()
        {
            string strRet = "";

            strRet += (chkBand3.Checked ? (strRet == "" ? "" : ",") + chkBand3.Text : "");
            strRet += (chkBand5.Checked ? (strRet == "" ? "" : ",") + chkBand5.Text : "");
            strRet += (chkBand8.Checked ? (strRet == "" ? "" : ",") + chkBand8.Text : "");
            strRet += (chkBand28.Checked ? (strRet == "" ? "" : ",") + chkBand28.Text : "");

            return strRet;
        }
        private void btSetBand_Click(object sender, EventArgs e)
        {
            Command cmd;

            if (_sendQueue.Count > 0) return;

            string strBand = BandChkGet();
            if(strBand == "")
            {
                ShowMsg(GetCurrentLang("请至少选择一个Band值进行设置") + "\r\n\r\n", Color.Red);
                return;
            }

            XmlHelper.SetNodeValue(_configPath, "/Config", "Band", strBand);

            string strModelType = XmlHelper.GetNodeDefValue(_configPath, "/Config/Model", "NH01A");

            if (strModelType == "NR01A")
            {
                strBand = strBand.Split(',').Length + "," + strBand;
                cmd = new Command();
                cmd.Name = "设置BAND";
                cmd.SendFunc = SendCmd;
                cmd.RecvFunc = RecvCmd;
                cmd.TimeWaitMS = 500;
                cmd.RetryTimes = 3;
                cmd.Params.Add("BAND设置");
                cmd.Params.Add(strBand);
                _sendQueue.Enqueue(cmd);
            }
            else
            {
                cmd = new Command();
                cmd.Name = "关闭SIM卡";
                cmd.SendFunc = SendCmd;
                cmd.RecvFunc = RecvCmd;
                cmd.TimeWaitMS = 5000;
                cmd.RetryTimes = 3;
                cmd.Params.Add("BAND设置");
                _sendQueue.Enqueue(cmd);

                cmd = new Command();
                cmd.Name = "设置BAND";
                cmd.SendFunc = SendCmd;
                cmd.RecvFunc = RecvCmd;
                cmd.TimeWaitMS = 500;
                cmd.RetryTimes = 3;
                cmd.Params.Add("BAND设置");
                cmd.Params.Add(strBand);
                _sendQueue.Enqueue(cmd);

                cmd = new Command();
                cmd.Name = "打开SIM卡";
                cmd.SendFunc = SendCmd;
                cmd.RecvFunc = RecvCmd;
                cmd.TimeWaitMS = 5000;
                cmd.RetryTimes = 3;
                cmd.Params.Add("BAND设置");
                _sendQueue.Enqueue(cmd);
            }

            _IsSendNewCmd = true;
        }
        #endregion

        #region 网络连接

        private bool _IsCloudConnected = false; // 是否连上云平台
        private string _objMsgId = "";          // OneNet设备对象通信Id
        private string _ackMsgId = "";          // OneNet应答消息Id
        private string _currBand = "";
        private Random _randomPort = new Random((int)(DateTime.Now.ToBinary()));    // 随机端口
        private int _currSocket;       
        private string _serverIp = "";
        private string _listenPort = "";

        // 入网
        private void btJoinNwk_Click(object sender, EventArgs e)
        {
            Command cmd;

            if (_sendQueue.Count > 0) return;

            if (GetDefaultLang(btCnctSvr.Text) == "断开连接")
            {
                DisCnctSvr();
            }

            string strModelType = XmlHelper.GetNodeDefValue(_configPath, "/Config/Model", "NH01A");

            if (strModelType == "NR01A")
            {
                cmd = new Command();
                cmd.Name = "打开SIM卡";
                cmd.SendFunc = SendCmd;
                cmd.RecvFunc = RecvCmd;
                cmd.TimeWaitMS = 3000;
                cmd.RetryTimes = 3;
                cmd.Params.Add("连接平台");
                _sendQueue.Enqueue(cmd);

                cmd = new Command();
                cmd.Name = "模组复位及入网";
                cmd.SendFunc = SendCmd;
                cmd.RecvFunc = RecvCmd;
                cmd.TimeWaitMS = 40000;
                cmd.RetryTimes = 1;
                cmd.Params.Add("入网");
                _sendQueue.Enqueue(cmd); 

                cmd = new Command();
                cmd.Name = "查询入网状态";
                cmd.SendFunc = SendCmd;
                cmd.RecvFunc = RecvCmd;
                cmd.TimeWaitMS = 5000;
                cmd.RetryTimes = 10;
                cmd.Params.Add("入网");
                _sendQueue.Enqueue(cmd);

                cmd = new Command();
                cmd.Name = "激活PDP上下文";
                cmd.SendFunc = SendCmd;
                cmd.RecvFunc = RecvCmd;
                cmd.TimeWaitMS = 500;
                cmd.RetryTimes = 3;
                cmd.Params.Add("入网");
                _sendQueue.Enqueue(cmd);
            }
            else  // (strModelType == "NH01A")
            {
                cmd = new Command();
                cmd.Name = "模组复位";
                cmd.SendFunc = SendCmd;
                cmd.RecvFunc = RecvCmd;
                cmd.TimeWaitMS = 15000;
                cmd.RetryTimes = 1;
                cmd.Params.Add("入网");
                _sendQueue.Enqueue(cmd);

                cmd = new Command();
                cmd.Name = "模组配置";
                cmd.SendFunc = SendCmd;
                cmd.RecvFunc = RecvCmd;
                cmd.TimeWaitMS = 1000;
                cmd.RetryTimes = 3;
                cmd.Params.Add("入网");
                _sendQueue.Enqueue(cmd);

                cmd = new Command();
                cmd.Name = "模组配置1";
                cmd.SendFunc = SendCmd;
                cmd.RecvFunc = RecvCmd;
                cmd.TimeWaitMS = 1000;
                cmd.RetryTimes = 3;
                cmd.Params.Add("入网");
                _sendQueue.Enqueue(cmd);

                cmd = new Command();
                cmd.Name = "模组配置2";
                cmd.SendFunc = SendCmd;
                cmd.RecvFunc = RecvCmd;
                cmd.TimeWaitMS = 1000;
                cmd.RetryTimes = 3;
                cmd.Params.Add("入网");
                _sendQueue.Enqueue(cmd);

                cmd = new Command();
                cmd.Name = "打开SIM卡";
                cmd.SendFunc = SendCmd;
                cmd.RecvFunc = RecvCmd;
                cmd.TimeWaitMS = 3000;
                cmd.RetryTimes = 3;
                cmd.Params.Add("入网");
                _sendQueue.Enqueue(cmd);

                cmd = new Command();
                cmd.Name = "入网激活";
                cmd.SendFunc = SendCmd;
                cmd.RecvFunc = RecvCmd;
                cmd.TimeWaitMS = 3000;
                cmd.RetryTimes = 3;
                cmd.Params.Add("入网");
                _sendQueue.Enqueue(cmd);

                cmd = new Command();
                cmd.Name = "查询入网状态";
                cmd.SendFunc = SendCmd;
                cmd.RecvFunc = RecvCmd;
                cmd.TimeWaitMS = 5000;
                cmd.RetryTimes = 10;
                cmd.Params.Add("入网");
                _sendQueue.Enqueue(cmd);
            }

            _IsSendNewCmd = true;

            ShowMsg(GetCurrentLang("入网启动中...") + "  \r\n", Color.Blue);
        }

        // 入网状态查询
        private void btQryNwkStat_Click(object sender, EventArgs e)
        {
            Command cmd;

            if (_sendQueue.Count > 0) return;

            cmd = new Command();
            cmd.Name = "查询入网状态";
            cmd.SendFunc = SendCmd;
            cmd.RecvFunc = RecvCmd;
            cmd.TimeWaitMS = 1000;
            cmd.RetryTimes = 1;
            _sendQueue.Enqueue(cmd);

            string strModelType = XmlHelper.GetNodeDefValue(_configPath, "/Config/Model", "NH01A");

            if (strModelType == "NR01A"
                && false == GetDefaultLang(lbNetState.Text).Contains("离线"))
            {
                cmd = new Command();
                cmd.Name = "激活PDP上下文";
                cmd.SendFunc = SendCmd;
                cmd.RecvFunc = RecvCmd;
                cmd.TimeWaitMS = 500;
                cmd.RetryTimes = 3;
                cmd.Params.Add("入网");
                _sendQueue.Enqueue(cmd);
            }

            cmd = new Command();
            cmd.Name = "查询网络状态信息";
            cmd.SendFunc = SendCmd;
            cmd.RecvFunc = RecvCmd;
            cmd.TimeWaitMS = 1000;
            cmd.RetryTimes = 3;
            _sendQueue.Enqueue(cmd);

            _IsSendNewCmd = true;
        }

        // 选择云平台
        private static int combCloudSvr_SelectedIndex_old = -1;
        private void combCloudSvr_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (combCloudSvr.SelectedIndex < 0 || combCloudSvr.SelectedIndex == combCloudSvr_SelectedIndex_old)
            {
                return;
            }
            combCloudSvr_SelectedIndex_old = combCloudSvr.SelectedIndex;
                
            if(combCloudSvr.Text.Contains("OneNet"))
            {
                lbIp.Visible = false;
                txtIp.Visible = false;
                lbPort.Visible = false;
                txtPort.Visible = false;
                txtDataUpload.Location = new System.Drawing.Point(32, 80);
                btDataUpload.Location = new System.Drawing.Point(207, 80);
            }
            else if (combCloudSvr.Text.Contains("CDP"))
            {
                lbIp.Visible = true;
                txtIp.Visible = true;
                lbPort.Visible = true;
                txtPort.Visible = true;
                txtDataUpload.Location = new System.Drawing.Point(32, 108);
                btDataUpload.Location = new System.Drawing.Point(207, 108);
                txtIp.Text = XmlHelper.GetNodeDefValue(_configPath, "/Config/CdpSvrIp", "180.101.147.115");
                txtPort.Text = XmlHelper.GetNodeDefValue(_configPath, "/Config/CdpSvrPort", "5683");
            }
            else if (combCloudSvr.Text.Contains("UDP"))
            {
                lbIp.Visible = true;
                txtIp.Visible = true;
                lbPort.Visible = true;
                txtPort.Visible = true;
                txtDataUpload.Location = new System.Drawing.Point(32, 108);
                btDataUpload.Location = new System.Drawing.Point(207, 108);
                txtIp.Text = XmlHelper.GetNodeDefValue(_configPath, "/Config/UdpSvrIp", "47.98.243.161");
                txtPort.Text = XmlHelper.GetNodeDefValue(_configPath, "/Config/UdpSvrPort", "17728");
            }

            if (GetDefaultLang(combCloudSvr.Text) != XmlHelper.GetNodeValue(_configPath, "/Config/CloudSvrName"))
            {
                DisCnctSvr();
                XmlHelper.SetNodeValue(_configPath, "/Config", "CloudSvrName", GetDefaultLang(combCloudSvr.Text));
            }
        }

        // 建立连接
        private void btCnctSvr_Click(object sender, EventArgs e)
        {
            Command cmd;
            string strIp = "" , strPort = "";

            if (GetDefaultLang(btCnctSvr.Text) == "断开连接")
            {
                DisCnctSvr();
                return;
            }

            if (GetDefaultLang(lbNetState.Text).Contains("离线"))
            {
                ShowMsg(GetCurrentLang("请入网后再连接平台") + "\r\n\r\n", Color.Red);
                return;
            }

            try
            {
                strIp = txtIp.Text.Trim();
                strPort = txtPort.Text.Trim();
                if (strIp != "" && strIp.Split('.').Length != 4)
                {
                    throw new Exception();
                }
                Convert.ToByte(strIp.Split('.')[0]);
                Convert.ToByte(strIp.Split('.')[1]);
                Convert.ToByte(strIp.Split('.')[2]);
                Convert.ToByte(strIp.Split('.')[3]);
                Convert.ToUInt16(strPort);
            }
            catch (Exception ex)
            {
                ShowMsg(GetCurrentLang("输入的Ip或Port无效：") + ex.Message + "\r\n\r\n", Color.Red);
                return;
            }

            _serverIp = strIp;
            _listenPort = strPort;

            if (_sendQueue.Count > 0) return;

            string strModelType = XmlHelper.GetNodeDefValue(_configPath, "/Config/Model", "NH01A");
            string strCloudSvr = GetDefaultLang(combCloudSvr.Text);

            if (strCloudSvr == "CDP服务器")
            {
                XmlHelper.SetNodeValue(_configPath, "/Config", "CdpSvrIp", _serverIp);
                XmlHelper.SetNodeValue(_configPath, "/Config", "CdpSvrPort", _listenPort);
            }
            else if (strCloudSvr == "UDP服务器")
            {
                XmlHelper.SetNodeValue(_configPath, "/Config", "UdpSvrIp", _serverIp);
                XmlHelper.SetNodeValue(_configPath, "/Config", "UdpSvrPort", _listenPort);
            }

            if (strModelType == "NR01A")
            {
                if (strCloudSvr == "CDP服务器")
                {
                    if (false == _currBand.Contains("3") && false == _currBand.Contains("8"))
                    {
                        cmd = new Command();
                        cmd.Name = "设置BAND";
                        cmd.SendFunc = SendCmd;
                        cmd.RecvFunc = RecvCmd;
                        cmd.TimeWaitMS = 500;
                        cmd.RetryTimes = 3;
                        cmd.Params.Add("连接平台");
                        cmd.Params.Add("3,5,8");
                        _sendQueue.Enqueue(cmd);
                    }

                    cmd = new Command();
                    cmd.Name = "设置COAP协议IP";
                    cmd.SendFunc = SendCmd;
                    cmd.RecvFunc = RecvCmd;
                    cmd.TimeWaitMS = 1000;
                    cmd.RetryTimes = 3;
                    cmd.Params.Add("连接平台");
                    cmd.Params.Add("\"" + _serverIp + "\"," + _listenPort);
                    _sendQueue.Enqueue(cmd);

                    cmd = new Command();
                    cmd.Name = "打开接收上报";
                    cmd.SendFunc = SendCmd;
                    cmd.RecvFunc = RecvCmd;
                    cmd.TimeWaitMS = 500;
                    cmd.RetryTimes = 3;
                    cmd.Params.Add("连接平台");
                    _sendQueue.Enqueue(cmd);

                    cmd = new Command();
                    cmd.Name = "打开SIM卡";
                    cmd.SendFunc = SendCmd;
                    cmd.RecvFunc = RecvCmd;
                    cmd.TimeWaitMS = 5000;
                    cmd.RetryTimes = 3;
                    cmd.Params.Add("连接平台");
                    _sendQueue.Enqueue(cmd);

                    cmd = new Command();
                    cmd.Name = "入网激活";
                    cmd.SendFunc = SendCmd;
                    cmd.RecvFunc = RecvCmd;
                    cmd.TimeWaitMS = 3000;
                    cmd.RetryTimes = 3;
                    cmd.Params.Add("连接平台");
                    _sendQueue.Enqueue(cmd);

                    cmd = new Command();
                    cmd.Name = "查询入网状态";
                    cmd.SendFunc = SendCmd;
                    cmd.RecvFunc = RecvCmd;
                    cmd.TimeWaitMS = 5000;
                    cmd.RetryTimes = 10;
                    cmd.Params.Add("连接平台");
                    _sendQueue.Enqueue(cmd);
                }
                else if (strCloudSvr == "UDP服务器")
                {
                    
                    cmd = new Command();
                    cmd.Name = "创建通信Socket";
                    cmd.SendFunc = SendCmd;
                    cmd.RecvFunc = RecvCmd;
                    cmd.TimeWaitMS = 1000;
                    cmd.RetryTimes = 3;
                    cmd.Params.Add("连接平台");
                    cmd.Params.Add("\"DGRAM\",17," + _randomPort.Next(40000, 60000) + ",1");
                    _sendQueue.Enqueue(cmd);
                }
                else if (strCloudSvr == "OneNet平台")
                {
                    cmd = new Command();
                    cmd.Name = "创建通信套件";
                    cmd.SendFunc = SendCmd;
                    cmd.RecvFunc = RecvCmd;
                    cmd.TimeWaitMS = 1000;
                    cmd.RetryTimes = 3;
                    cmd.Params.Add("连接平台");
                    _sendQueue.Enqueue(cmd);

                    cmd = new Command();
                    cmd.Name = "添加设备对象";
                    cmd.SendFunc = SendCmd;
                    cmd.RecvFunc = RecvCmd;
                    cmd.TimeWaitMS = 500;
                    cmd.RetryTimes = 3;
                    cmd.Params.Add("连接平台");
                    cmd.Params.Add("0,3306,1,\"1\",1,0");
                    _sendQueue.Enqueue(cmd);

                    cmd = new Command();
                    cmd.Name = "注册设备";
                    cmd.SendFunc = SendCmd;
                    cmd.RecvFunc = RecvCmd;
                    cmd.TimeWaitMS = 30000;
                    cmd.RetryTimes = 1;
                    cmd.Params.Add("连接平台");
                    _sendQueue.Enqueue(cmd);

                    cmd = new Command();
                    cmd.Name = "观察对象应答";
                    cmd.SendFunc = SendCmd;
                    cmd.RecvFunc = RecvCmd;
                    cmd.TimeWaitMS = 1000;
                    cmd.RetryTimes = 3;
                    cmd.Params.Add("连接平台");
                    cmd.Params.Add(",1");
                    _sendQueue.Enqueue(cmd);

                    cmd = new Command();
                    cmd.Name = "发现对象资源应答";
                    cmd.SendFunc = SendCmd;
                    cmd.RecvFunc = RecvCmd;
                    cmd.TimeWaitMS = 1000;
                    cmd.RetryTimes = 3;
                    cmd.Params.Add("连接平台");
                    cmd.Params.Add(",1,4,\"5750\"");
                    _sendQueue.Enqueue(cmd);
                }
            }
            else if (strModelType == "NH01A")
            {
                if (strCloudSvr == "CDP服务器")
                {
                    cmd = new Command();
                    cmd.Name = "关闭SIM卡";
                    cmd.SendFunc = SendCmd;
                    cmd.RecvFunc = RecvCmd;
                    cmd.TimeWaitMS = 5000;
                    cmd.RetryTimes = 3;
                    cmd.Params.Add("连接平台");
                    _sendQueue.Enqueue(cmd);

                    if (false == _currBand.Contains("5"))
                    {
                        cmd = new Command();
                        cmd.Name = "设置BAND";
                        cmd.SendFunc = SendCmd;
                        cmd.RecvFunc = RecvCmd;
                        cmd.TimeWaitMS = 500;
                        cmd.RetryTimes = 3;
                        cmd.Params.Add("连接平台");
                        cmd.Params.Add("3,5,8");
                        _sendQueue.Enqueue(cmd);
                    }

                    cmd = new Command();
                    cmd.Name = "设置COAP协议IP";
                    cmd.SendFunc = SendCmd;
                    cmd.RecvFunc = RecvCmd;
                    cmd.TimeWaitMS = 1000;
                    cmd.RetryTimes = 3;
                    cmd.Params.Add("连接平台");
                    cmd.Params.Add(_serverIp + "," + _listenPort);
                    _sendQueue.Enqueue(cmd);

                    cmd = new Command();
                    cmd.Name = "打开接收上报";
                    cmd.SendFunc = SendCmd;
                    cmd.RecvFunc = RecvCmd;
                    cmd.TimeWaitMS = 500;
                    cmd.RetryTimes = 3;
                    cmd.Params.Add("连接平台");
                    _sendQueue.Enqueue(cmd);

                    cmd = new Command();
                    cmd.Name = "打开SIM卡";
                    cmd.SendFunc = SendCmd;
                    cmd.RecvFunc = RecvCmd;
                    cmd.TimeWaitMS = 5000;
                    cmd.RetryTimes = 3;
                    cmd.Params.Add("连接平台");
                    _sendQueue.Enqueue(cmd);

                    cmd = new Command();
                    cmd.Name = "入网激活";
                    cmd.SendFunc = SendCmd;
                    cmd.RecvFunc = RecvCmd;
                    cmd.TimeWaitMS = 3000;
                    cmd.RetryTimes = 3;
                    cmd.Params.Add("连接平台");
                    _sendQueue.Enqueue(cmd);

                    cmd = new Command();
                    cmd.Name = "查询入网状态";
                    cmd.SendFunc = SendCmd;
                    cmd.RecvFunc = RecvCmd;
                    cmd.TimeWaitMS = 5000;
                    cmd.RetryTimes = 10;
                    cmd.Params.Add("连接平台");
                    _sendQueue.Enqueue(cmd);
                }
                else if (strCloudSvr == "UDP服务器")
                {
                    cmd = new Command();
                    cmd.Name = "创建通信Socket";
                    cmd.SendFunc = SendCmd;
                    cmd.RecvFunc = RecvCmd;
                    cmd.TimeWaitMS = 1000;
                    cmd.RetryTimes = 3;
                    cmd.Params.Add("连接平台");
                    cmd.Params.Add("DGRAM,17," + _randomPort.Next(40000, 60000) + ",1,AF_INET");
                    _sendQueue.Enqueue(cmd);
                }
                else // 其他平台
                {

                }
            }

            _IsSendNewCmd = true;
        }

        // 数据上传
        private void btDataUpload_Click(object sender, EventArgs e)
        {
            Command cmd;

            if (false == _IsCloudConnected)
            {
                ShowMsg(GetCurrentLang("请连接平台后再上传") + " \r\n\r\n", Color.Red);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtDataUpload.Text))
            {
                ShowMsg(GetCurrentLang("请输入数据后再上传") + " \r\n\r\n", Color.Red);
                return;
            }

            if (_sendQueue.Count > 0) return;

            string msg = txtDataUpload.Text.Trim();
            byte[] buf;
            if (chkHex.Checked)
            {
                buf = Util.GetByteFromStringHex(msg);
                if(buf == null)
                {
                    ShowMsg("输入的数据不是Hex格式\r\n", Color.Red);
                    return;
                }
            }
            else
            {
                buf = Encoding.Default.GetBytes(msg);        // GB2312
                msg = Util.GetStringHexFromByte(buf, 0, buf.Length);
            }

            if (combCloudSvr.Text.Contains("CDP"))
            {
                string param = buf.Length + "," + msg;

                cmd = new Command();
                cmd.Name = "发送数据";
                cmd.SendFunc = SendCmd;
                cmd.RecvFunc = RecvCmd;
                cmd.TimeWaitMS = 2000;
                cmd.RetryTimes = 3;
                cmd.Params.Add("数据上传");
                cmd.Params.Add(param);
                _sendQueue.Enqueue(cmd);
            }
            else if (combCloudSvr.Text.Contains("UDP"))
            {
                string param = _currSocket + ",\"" + _serverIp + "\"," + _listenPort + "," + buf.Length + "," + msg + "";

                cmd = new Command();
                cmd.Name = "发送UDP数据";
                cmd.SendFunc = SendCmd;
                cmd.RecvFunc = RecvCmd;
                cmd.TimeWaitMS = 1000;
                cmd.RetryTimes = 3;
                cmd.Params.Add("数据上传");
                cmd.Params.Add(param);
                _sendQueue.Enqueue(cmd);
            }
            else if (combCloudSvr.Text.Contains("OneNet"))
            {
                msg = txtDataUpload.Text.Trim();
                string param = ",3306,0,5750,1," + msg.Length + ",\"" + msg + "\",0,0";

                cmd = new Command();
                cmd.Name = "上报对象资源";
                cmd.SendFunc = SendCmd;
                cmd.RecvFunc = RecvCmd;
                cmd.TimeWaitMS = 3000;
                cmd.RetryTimes = 3;
                cmd.Params.Add("数据上传");
                cmd.Params.Add(msg);
                _sendQueue.Enqueue(cmd);
            }

            _IsSendNewCmd = true;
        }

        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="args"> args[0] - "TCP,UDP,TCP/UDP,CDP", args[1] - socket num, args[2] - length </param>
        private void RecvSocketData(params object[] args)
        {
            Command cmd = null;

            if (args.Length != 3) return;

            if (_sendQueue.Count > 0) return;

            string server = (string)args[0];
            int socket = (int)args[1];
            int length = (int)args[2];

            length = (length <= 0 ? 512 : length);

            string strModelType = XmlHelper.GetNodeDefValue(_configPath, "/Config/Model", "NH01A");

            if (strModelType == "NH01A")
            {
                if (server.Contains("UDP"))
                {
                    cmd = new Command();
                    cmd.Name = "接收Socket数据";
                    cmd.SendFunc = SendCmd;
                    cmd.RecvFunc = RecvCmd;
                    cmd.TimeWaitMS = 1000;
                    cmd.RetryTimes = 1;
                    cmd.Params.Add("");
                    cmd.Params.Add(socket + "," + length);
                    _sendQueue.Enqueue(cmd);
                }
            }
            else if (strModelType == "NR01A")
            {
                if (server.Contains("CDP"))
                {
                    cmd = new Command();
                    cmd.Name = "查看接收缓存";
                    cmd.SendFunc = SendCmd;
                    cmd.RecvFunc = RecvCmd;
                    cmd.TimeWaitMS = 1000;
                    cmd.RetryTimes = 1;
                    _sendQueue.Enqueue(cmd);
                }
                else if (server.Contains("UDP"))
                {
                    cmd = new Command();
                    cmd.Name = "接收Socket数据";
                    cmd.SendFunc = SendCmd;
                    cmd.RecvFunc = RecvCmd;
                    cmd.TimeWaitMS = 5000;
                    cmd.RetryTimes = 1;
                    cmd.Params.Add("");
                    cmd.Params.Add(socket + "," + length + ",5");
                    _sendQueue.Enqueue(cmd);
                }
               
            }

            _IsSendNewCmd = true;
        }
        
        // 断开连接
        private void DisCnctSvr(params object[] args)
        {
            Command cmd;

            if (_sendQueue.Count > 0 || false == _IsCloudConnected) return;

            UpdateCnctStatus("未连接");

            string strModelType = XmlHelper.GetNodeDefValue(_configPath, "/Config/Model", "NH01A");
            string strCloudSvrName = XmlHelper.GetNodeDefValue(_configPath, "/Config/CloudSvrName", "");

            if (strModelType == "NR01A")
            {
                if (strCloudSvrName == "CDP服务器")
                {
                    cmd = new Command();
                    cmd.Name = "关闭CDP连接";
                    cmd.SendFunc = SendCmd;
                    cmd.RecvFunc = RecvCmd;
                    cmd.TimeWaitMS = 1000;
                    cmd.RetryTimes = 3;
                    cmd.Params.Add("断开连接");
                    _sendQueue.Enqueue(cmd);
                }
                else if (strCloudSvrName == "UDP服务器")
                {
                    cmd = new Command();
                    cmd.Name = "关闭通信Socket";
                    cmd.SendFunc = SendCmd;
                    cmd.RecvFunc = RecvCmd;
                    cmd.TimeWaitMS = 2000;
                    cmd.RetryTimes = 3;
                    cmd.Params.Add("断开连接");
                    cmd.Params.Add(_currSocket.ToString());
                    _sendQueue.Enqueue(cmd);
                }
                else if (strCloudSvrName == "OneNet平台")
                {
                    cmd = new Command();
                    cmd.Name = "注销设备";
                    cmd.SendFunc = SendCmd;
                    cmd.RecvFunc = RecvCmd;
                    cmd.TimeWaitMS = 1000;
                    cmd.RetryTimes = 3;
                    cmd.Params.Add("断开连接");
                    _sendQueue.Enqueue(cmd);

                    cmd = new Command();
                    cmd.Name = "删除通信套件";
                    cmd.SendFunc = SendCmd;
                    cmd.RecvFunc = RecvCmd;
                    cmd.TimeWaitMS = 1000;
                    cmd.RetryTimes = 3;
                    cmd.Params.Add("断开连接");
                    _sendQueue.Enqueue(cmd);
                }
            }
            else if (strModelType == "NH01A")
            {
                if (strCloudSvrName == "CDP服务器")
                {
                    cmd = new Command();
                    cmd.Name = "关闭SIM卡";
                    cmd.SendFunc = SendCmd;
                    cmd.RecvFunc = RecvCmd;
                    cmd.TimeWaitMS = 5000;
                    cmd.RetryTimes = 3;
                    cmd.Params.Add("断开连接");
                    _sendQueue.Enqueue(cmd);
                }
                else if (strCloudSvrName == "UDP服务器")
                {
                    cmd = new Command();
                    cmd.Name = "关闭通信Socket";
                    cmd.SendFunc = SendCmd;
                    cmd.RecvFunc = RecvCmd;
                    cmd.TimeWaitMS = 2000;
                    cmd.RetryTimes = 3;
                    cmd.Params.Add("断开连接");
                    cmd.Params.Add(_currSocket.ToString());
                    _sendQueue.Enqueue(cmd);
                }
                else
                {

                }
            }

            _IsSendNewCmd = true;
        }
        #endregion

        #region 自定义AT指令发送
        private void btSend_Click(object sender, EventArgs e)
        {
            Command cmd;
            string strAtCmd = combAtCmd.Text.Trim();
            int index = combAtCmd.Items.IndexOf(strAtCmd);
            string items = "";

            if (strAtCmd == "") return;

            if(index >= 0)
            {
                combAtCmd.Items.RemoveAt(index);
            }

            if (strAtCmd.StartsWith("AT", StringComparison.OrdinalIgnoreCase))
            { 
                combAtCmd.Items.Insert(0, strAtCmd);
            }

            for (int i = combAtCmd.Items.Count; combAtCmd.Items.Count > 20; i--)
            {
                combAtCmd.Items.RemoveAt(i - 1);
            }

            foreach (object str in combAtCmd.Items)
            {
                items += str + "/";
            }
            XmlHelper.SetNodeValue(_configPath, "/Config", "CustomAtCmds", items);

            if (_sendQueue.Count > 0) return;

            cmd = new Command();
            cmd.Name = "自定义AT指令";
            cmd.SendFunc = SendCmd;
            cmd.RecvFunc = RecvCmd;
            cmd.TimeWaitMS = 2000;
            cmd.RetryTimes = 1;
            cmd.Params.Add("");
            cmd.Params.Add(strAtCmd);
            cmd.Params.Add("");
            cmd.Params.Add("自定义AT指令");
            _sendQueue.Enqueue(cmd);

            _IsSendNewCmd = true;
        }
        #endregion

        #region 报文生成、解析
        // 发送报文
        private void SendCmd(Command cmd)
        {
            string[] AT_CmdTbl;
            string strAtCmd = "";
            bool isCmdFind = false;
            string strModelType = XmlHelper.GetNodeDefValue(_configPath, "/Config/Model", "NH01A");

            if (strModelType == "NR01A")
            {
                AT_CmdTbl = AT_CmdTbl_NR01A;
            }
            else
            {
                AT_CmdTbl = AT_CmdTbl_NH01A;
            }

            while (cmd.Params.Count < 3)    //Params0 - cmdName, Params1 - cmdParam, Params2 - cmdRecvMsg
            {
                cmd.Params.Add("");
            }

            if (cmd.Name == "自定义AT指令")
            {
                string AtCmdPrefix;

                strAtCmd = cmd.Params[1].ToUpper();
                AtCmdPrefix = (strAtCmd.IndexOf("=") > 0 ? strAtCmd.Substring(0, strAtCmd.IndexOf("=") + 1) : strAtCmd);
                foreach (string strCmd in AT_CmdTbl)
                {
                    if (strCmd.Substring(strCmd.IndexOf(" ")).Trim().StartsWith(strAtCmd))
                    {
                        cmd.Name = strCmd.Substring(0, strCmd.IndexOf(" ")).Trim();
                        isCmdFind = true;
                        break;
                    }
                }

                if (!isCmdFind)
                {
                    foreach (string strCmd in AT_CmdTbl)
                    {
                        if (strCmd.Substring(strCmd.IndexOf(" ")).Trim().StartsWith(AtCmdPrefix))
                        {
                            cmd.Name = strCmd.Substring(0, strCmd.IndexOf(" ")).Trim();
                            isCmdFind = true;
                            break;
                        }
                    }
                }
                strAtCmd = "";
            }
            else
            {
                foreach (string strCmd in AT_CmdTbl)
                {
                    if (strCmd.Contains(cmd.Name))
                    {
                        strAtCmd = strCmd.Substring(strCmd.IndexOf(" ")).Trim();
                        isCmdFind = true;
                        break;
                    }
                }
            }

            if (isCmdFind || cmd.Name == "自定义AT指令")
            {
                cmd.Params[2] = "";

                if (cmd.Params.Count > 1 && cmd.Params[1] != "")
                {
                    string strParams = cmd.Params[1].ToUpper();
                    if (cmd.Name.Contains("观察对象应答") || cmd.Name.Contains("上报对象资源"))
                    {
                        strParams = "0," + _objMsgId + strParams;
                    }
                    else if (cmd.Name.Contains("应答"))   // 其他应答
                    {
                        strParams = "0," + _ackMsgId + strParams;
                    }

                    strAtCmd = strAtCmd.Substring(0, strAtCmd.IndexOf('=') + 1) + strParams;
                }
                strAtCmd += "\r\n";

                char[] charsAtCmd = strAtCmd.ToCharArray();
                byte[] buf = new byte[charsAtCmd.Length];
                for (int i = 0; i < charsAtCmd.Length; i++)
                {
                    buf[i] = (byte)charsAtCmd[i];
                }

                if (false == serialPort_SendData(buf))
                {
                    UiOperateEnable();
                    cmd.RetryTimes = 0;
                    return;
                }

                ShowMsg("Tx：" + strAtCmd, Color.Black);
            }
        }

        // 接收报文
        private void RecvCmd(Command cmd)
        {
            string msg = "", strVal = "", strTmp;
            int index = 0, strLen = 0;

            if (cmd.RxBuf == null || cmd.RxBuf.Length == 0) return;

            byte[] buf = cmd.RxBuf;

            foreach (byte b in buf)
            {
                msg += (char)b;
            }

            cmd.Params[2] = (cmd.Params[2] + msg).Replace("\0", "\r\n");

            if (chkShowRxData.Checked)
            {
                ShowMsg("Rx：" + msg + "\r\n", Color.Green);
            }

            switch(cmd.Name)
            {
                case "Idle状态":
                    if (msg.Contains("+NNMI:"))
                    {
                        // NR01A CDP msg recved
                        index = msg.IndexOf("+NNMI:");
                        strLen = msg.IndexOf("\r\n", index + 6) - (index + 6);
                        string[] strs = msg.Substring((index + 6), strLen).Trim().Split(',');
                        if (strs.Length != 2)
                        {
                            return;
                        }

                        string strData = "";
                        try
                        {
                            strData = Encoding.Default.GetString(Util.GetByteFromStringHex(strs[1]));
                            strData = "(Str) " + strData;
                        }
                        catch (Exception)
                        {
                            strData = "(Hex) " + strs[1];
                        }
                        strVal += GetCurrentLang("收到数据") + "：" + GetCurrentLang("长度") + " " + strs[0] + " , " + GetCurrentLang("数据") + strData + "\r\n";
                    
                    }
                    else if (msg.Contains("+NSONMI:"))  
                    {
                        // NH01A socket msg recved
                        index = msg.IndexOf("+NSONMI:");
                        strLen = msg.IndexOf("\r\n", index + 8) - (index + 8);
                        string[] strs = msg.Substring((index + 8), strLen).Trim().Split(',');
                        if (strs.Length != 2)
                        {
                            return;
                        }

                        _cmdEndCallback = RecvSocketData;
                        _argsEndCallback = new object[] { "TCP/UDP", Convert.ToInt32(strs[0]), Convert.ToInt32(strs[1]) };

                    }
                    else if ( !msg.StartsWith("\r\n") && msg.EndsWith("\r\n"))      
                    {
                        // NR01A socket msg recved
                        foreach (string item in msg.Trim().Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            string[] strs = item.Trim().Split(',');
                            if (strs.Length != 5)
                            {
                                continue;
                            }

                            string strData = "";
                            try
                            {
                                strData = Encoding.Default.GetString(Util.GetByteFromStringHex(strs[4]));
                                strData = "(Str) " + strData;
                            }
                            catch (Exception)
                            {
                                strData = "(Hex) " + strs[4];
                            }
                            strVal += GetCurrentLang("收到数据") + "：socket " + strs[0] + " - " + GetCurrentLang("远程地址") + " " + strs[1] + ":" + strs[2] + "\r\n";
                            strVal += "\t\t\t" + GetCurrentLang("长度") + " " + strs[3] + " , " + GetCurrentLang("数据") + strData + "\r\n";
                        }
                    }
                    _strMsgMain = strVal;
                    break;

                case "自定义AT指令":
                    cmd.IsEnable = false;
                    _IsSendNewCmd = true;
                    break;

                case "模组复位":
                case "模组复位及入网":
                    if (msg.Contains("OK") || msg.Contains("CTZV:"))
                    {
                        cmd.IsEnable = false;
                        _IsSendNewCmd = true;
                    }
                    break;
                case "模组配置":
                case "模组配置1":
                case "模组配置2":
                case "打开SIM卡":
                case "入网激活":
                case "激活PDP上下文":
                case "设置COAP协议IP":
                case "打开接收上报":
                    if (msg.Contains("OK"))
                    {
                        cmd.IsEnable = false;
                        _IsSendNewCmd = true;
                    }
                    break;

                case "设置BAND":
                    if(msg.Contains("OK"))
                    {
                        cmd.IsEnable = false;
                        _IsSendNewCmd = true;
                        _currBand = BandChkGet();
                        if (cmd.Params[0] == "BAND设置")
                        {
                            _strMsgSub = _currBand;
                        }
                    }
                    break;

                case "创建通信Socket":
                    if (msg.Contains("OK"))
                    {
                        cmd.IsEnable = false;
                        _IsSendNewCmd = true;

                        if (cmd.Params[0] != "连接平台")
                        {
                            return;
                        }

                        if ((index = cmd.Params[2].IndexOf("\r\n")) >= 0)
                        {
                            strLen = cmd.Params[2].IndexOf("OK", index + 2) - (index + 2);
                            strVal = cmd.Params[2].Substring((index + 2), strLen).Trim();

                            string[] array = strVal.Split(new string[1]{"\r\n"}, 10, StringSplitOptions.RemoveEmptyEntries);
                            strVal = (array.Length > 0 ? array[array.Length - 1] : strVal);
                        }
                        
                        try
                        {
                            _currSocket = Convert.ToByte(strVal);
                        }
                        catch(Exception){ }
                    }
                    break;
                case "发送UDP数据":
                    if (msg.Contains("OK"))
                    {
                        cmd.IsEnable = false;
                        _IsSendNewCmd = true;
                    }
                    break;
                case "接收Socket数据":
                    if (msg.Contains("OK"))
                    {
                        cmd.IsEnable = false;
                        _IsSendNewCmd = true;

                        strLen = cmd.Params[2].IndexOf("OK");
                        string[] strs = cmd.Params[2].Substring(0, strLen).Trim().Split(',');
                        if(strs.Length == 6)
                        {
                            string strData = "";
                            try
                            {
                                strData = Encoding.Default.GetString(Util.GetByteFromStringHex(strs[4]));
                                strData = "(Str) " + strData;
                            }
                            catch(Exception)
                            {
                                strData = "(Hex) " + strs[4];
                            }
                            strVal = GetCurrentLang("收到数据") + "：socket " + strs[0] + " - " + GetCurrentLang("远程地址") + " " + strs[1] + ":" + strs[2] + "\r\n";
                            strVal += "\t\t\t" + GetCurrentLang("长度") + " " + strs[3] + " , " + GetCurrentLang("数据") + strData + "\r\n";
                            strVal += "\t\t\t" + GetCurrentLang("剩余缓存数据") + " " + strs[5] + "\r\n";

                            if(strs[5] != "0")
                            {
                                _cmdEndCallback = RecvSocketData;
                                _argsEndCallback = new object[] { "TCP/UDP", Convert.ToInt32(strs[0]), Convert.ToInt32(strs[5]) };
                            }
                        }
                    }
                    _strMsgMain = strVal;
                    break;
                case "关闭通信Socket":
                    if (msg.Contains("OK"))
                    {
                        cmd.IsEnable = false;
                        _IsSendNewCmd = true;

                        if (cmd.Params[0] != "断开连接")
                        {
                            return;
                        }
                        _currSocket = 0;
                    }
                    break;

                case "创建通信套件":
                case "添加设备对象":
                    if(msg.Contains("OK"))
                    {
                        cmd.IsEnable = false;
                        _IsSendNewCmd = true;
                    }
                    else if (msg.Contains("ERROR"))
                    {
                        cmd.RetryTimes = 0;
                        _cmdEndCallback = DisCnctSvr;
                    }
                    break;
                case "发现对象资源应答":
                case "更新注册信息":
                case "删除设备对象":
                case "注销设备":
                case "删除通信套件":
                    if(msg.Contains("OK"))
                    {
                        cmd.IsEnable = false;
                        _IsSendNewCmd = true;
                    }
                    break;

                case "注册设备":
                    if ((index = cmd.Params[2].IndexOf("MIPLOBSERVE:")) >= 0)
                    {
                        strLen = cmd.Params[2].IndexOf("\r\n", index + 12) - (index + 12);
                        strVal = cmd.Params[2].Substring((index + 12), strLen);
                        if (strVal.Split(',')[3] == "3306")
                        {
                            cmd.IsEnable = false;
                            _IsSendNewCmd = true;
                            _objMsgId = strVal.Split(',')[1];
                        }
                    }
                    break;

                case "观察对象应答":
                    if ((index = cmd.Params[2].IndexOf("MIPLDISCOVER:")) >= 0)
                    {
                        strLen = cmd.Params[2].IndexOf("\r\n", index + 13) - (index + 13);
                        strVal = cmd.Params[2].Substring((index + 13), strLen);
                        if (strVal.Split(',')[2] == "3306")
                        {
                            cmd.IsEnable = false;
                            _IsSendNewCmd = true;
                            _ackMsgId = strVal.Split(',')[1];
                        }
                    }
                    break;

                case "上报对象资源":
                    if (msg.Contains("OK"))
                    {
                        cmd.IsEnable = false;
                        _IsSendNewCmd = true;
                    }
                    else if (msg.Contains("ERROR"))
                    {
                        cmd.RetryTimes = 0;
                        _cmdEndCallback = DisCnctSvr;
                    }
                    break;

                case "关闭SIM卡":
                    if (msg.Contains("OK"))
                    {
                        cmd.IsEnable = false;
                        _IsSendNewCmd = true;
                        UpdateNetStatus("离线");
                        UpdateCnctStatus("未连接");
                    }
                    break;

                case "发送数据":
                    if (msg.Contains("OK"))
                    {
                        cmd.IsEnable = false;
                        _IsSendNewCmd = true;
                    }
                    else if (msg.Contains("ERROR"))
                    {
                        cmd.RetryTimes = 0; 
                        UpdateCnctStatus("未连接");
                    }
                    _strMsgMain = strVal;
                    break;

                case "查看接收缓存":
                    if (msg.Contains("OK"))
                    {
                        cmd.IsEnable = false;
                        _IsSendNewCmd = true;
                    }
                    break;

                case "查询入网状态":
                    if (msg.Contains("OK"))
                    {
                        cmd.IsEnable = false;
                        _IsSendNewCmd = true;

                        if (msg.Contains("+CGATT:1"))
                        {
                            UpdateNetStatus("在线");
                        }
                        else if (msg.Contains("+CGATT:0"))
                        {
                            UpdateNetStatus("离线");
                        }
                    }
                    break;

                case "查询网络状态信息":
                    if (msg.Contains("OK"))
                    {
                        cmd.IsEnable = false;
                        _IsSendNewCmd = true;

                        cmd.Params[2] = cmd.Params[2].Replace("\r\n\r\n", "\r\n");
                        if ((index = cmd.Params[2].IndexOf("\r\n")) >= 0)
                        {
                            int idx2 = cmd.Params[2].IndexOf("RSRQ", index + 2);
                            strLen = cmd.Params[2].IndexOf("\r\n", idx2 + 4) - index;
                            strVal = cmd.Params[2].Substring(index, strLen);
                            strVal = GetCurrentLang("网络状态") + "：" + strVal.Replace("\r\n", "\r\n        ").Replace("TUESTATS:RADIO,", "");
                        }
                        _strMsgMain = strVal;
                    }
                    break;

                case "查询模组型号":
                    if (msg.Contains("OK"))
                    {
                        cmd.IsEnable = false;
                        _IsSendNewCmd = true;

                        if ((index = cmd.Params[2].IndexOf("+MTYPE:")) >= 0)
                        {
                            strLen = cmd.Params[2].IndexOf("\r\n", index + 7) - (index + 7);
                            strVal = cmd.Params[2].Substring((index + 7), strLen);
                        }
                        else if ((index = cmd.Params[2].IndexOf("+SRMTYPE\r\n\r\n")) >= 0)
                        {
                            strLen = cmd.Params[2].IndexOf("\r\n", index + 12) - (index + 12);
                            strVal = cmd.Params[2].Substring((index + 12), strLen);
                        }

                        strVal = GetCurrentLang("模组型号") + "：" + strVal;

                        _strMsgMain = strVal;
                    }
                    break;

                case "查询版本号":
                    if (msg.Contains("OK"))
                    {
                        cmd.IsEnable = false;
                        _IsSendNewCmd = true;

                        if ((index = cmd.Params[2].IndexOf("SR")) < 0)
                        {
                            // 其他厂家模块
                            cmd.RetryTimes = 0;
                            strVal = "";
                        }
                        else
                        {
                            if ((index = cmd.Params[2].IndexOf("APPLICATION_A,")) >= 0)
                            {
                                strLen = cmd.Params[2].IndexOf("\r\n", index + 14) - (index + 14);
                                strVal = GetCurrentLang("软件版本") + "：" + cmd.Params[2].Substring((index + 14), strLen);
                            }
                            if ((index = cmd.Params[2].IndexOf("SSB,")) >= 0)
                            {
                                strLen = cmd.Params[2].IndexOf("\r\n", index + 4) - (index + 4);
                                strVal += " ， " + GetCurrentLang("硬件版本") + "：" + cmd.Params[2].Substring((index + 4), strLen);
                            }
                        }

                        _strMsgMain = strVal;
                    }
                    break;
                case "查询软件版本":
                     if (msg.Contains("OK"))
                    {
                        cmd.IsEnable = false;
                        _IsSendNewCmd = true;
                        _strMsgBuf = cmd.Params[2];

                        if ((index = cmd.Params[2].IndexOf("SR")) < 0)
                        {
                            // 其他厂家模块
                            cmd.RetryTimes = 0;
                        }
                    }
                    break;
                case "查询硬件版本":
                    if (msg.Contains("OK"))
                    {
                        cmd.IsEnable = false;
                        _IsSendNewCmd = true;
                        _strMsgBuf += cmd.Params[2];

                        if ((index = _strMsgBuf.IndexOf("SR")) >= 0)
                        {
                            strLen = _strMsgBuf.IndexOf("\r\n", index + 2) - (index + 2);
                            strVal = GetCurrentLang("软件版本") + "：" + _strMsgBuf.Substring((index + 2), strLen);
                        }
                        if ((index = _strMsgBuf.IndexOf("SRHWVER", index)) >= 0)
                        {
                            strLen = _strMsgBuf.IndexOf("\r\n", index + 11) - (index + 11);
                            strVal += " ， " + GetCurrentLang("硬件版本") + "：" + _strMsgBuf.Substring((index + 11), strLen);
                        }

                        _strMsgMain = strVal;
                    }
                    break;
                case "查询IMEI":
                    if (msg.Contains("OK"))
                    {
                        cmd.IsEnable = false;
                        _IsSendNewCmd = true;

                        if ((index = cmd.Params[2].IndexOf("CGSN:")) >= 0)
                        {
                            strLen = cmd.Params[2].IndexOf("\r\n", index + 5) - (index + 5);
                            strVal = GetCurrentLang("IMEI号码") + "：" + cmd.Params[2].Substring((index + 5), strLen).Trim();
                        }

                        _strMsgMain = strVal;
                    }
                    break;
                case "查询SIM卡ID":
                    if (msg.Contains("OK"))
                    {
                        cmd.IsEnable = false;
                        _IsSendNewCmd = true;

                        cmd.Params[2] = cmd.Params[2].Replace("\r\n\r\n", "\r\n");
                        if ((index = cmd.Params[2].IndexOf("\r\n")) >= 0)
                        {
                            strLen = cmd.Params[2].IndexOf("\r\n", index + 2) - (index + 2);
                            strVal = GetCurrentLang("SIM卡ID") + "：" + cmd.Params[2].Substring((index + 2), strLen);
                        }

                        _strMsgMain = strVal;
                    }
                    break;
                case "查询BAND":
                    if (msg.Contains("OK"))
                    {
                        cmd.IsEnable = false;
                        _IsSendNewCmd = true;

                        if ((index = cmd.Params[2].IndexOf("NBAND:")) >= 0
                            || (index = cmd.Params[2].IndexOf("total:")) >= 0)
                        {
                            strLen = cmd.Params[2].IndexOf("\r\n", index + 6) - (index + 6);
                            _currBand = cmd.Params[2].Substring((index + 6), strLen);
                            strVal = GetCurrentLang("BAND值") + "：" + _currBand;
                            BandChkSet(_currBand);
                        }

                        _strMsgMain = strVal;
                    }
                    break;

                case "查询COAP协议IP":
                    if (msg.Contains("+NCDP:"))
                    {
                        cmd.IsEnable = false;
                        _IsSendNewCmd = true;
                    }
                    break;

                case "查询温度和电压":
                    if (msg.Contains("OK"))
                    {
                        cmd.IsEnable = false;
                        _IsSendNewCmd = true;

                        if ((index = cmd.Params[2].IndexOf("TEMP,")) >= 0)
                        {
                            strLen = cmd.Params[2].IndexOf("\r\n", index + 5) - (index + 5);
                            strVal = GetCurrentLang("温度") + "：" + cmd.Params[2].Substring((index + 5), strLen) + " ℃";
                        }
                        if ((index = cmd.Params[2].IndexOf("VBAT,", index)) >= 0)
                        {
                            strLen = cmd.Params[2].IndexOf("\r\n", index + 5) - (index + 5);
                            strTmp = cmd.Params[2].Substring((index + 5), strLen);
                            strVal += " ， " + GetCurrentLang("电池电压") + "：" + strTmp.Substring(0, 1) + "." + strTmp.Substring(1) + " V";
                        }

                        _strMsgMain = strVal;
                    }
                    break;

                default:
                    {
                        cmd.IsEnable = false;
                        _IsSendNewCmd = true;
                    }
                    break;
            }
        }
        #endregion

        #region 通信记录、显示时间、发送Ctrl-Z / ESC

        private void ShowMsg(string msg, Color fgColor)
        {
            if (msg == "") return;

            if(chkTime.Checked && msg != "\r\n")
            {
                msg = "[" + DateTime.Now.ToString("HH:mm:ss.fff") + "] " + msg;
            }

            RichTextBoxAppand(rTxtMsg, msg, fgColor);
        }

        private delegate void UpdateRichTextCallback(RichTextBox rtb, string msg, Color fgColor);
        private void RichTextBoxAppand(RichTextBox rtb, string str, Color foreColor)
        {
            if (this.InvokeRequired)
            {
                Invoke(new UpdateRichTextCallback(RichTextBoxAppand), new object[] { rtb, str, foreColor });
                return;
            }

            int iStart = rtb.Text.Length;
            rtb.AppendText(str);
            rtb.Select(iStart, rtb.Text.Length);
            rtb.SelectionColor = foreColor;
            rtb.Select(rtb.Text.Length, 0);
            rtb.ScrollToCaret();
        }

        private void btClear_Click(object sender, EventArgs e)
        {
            rTxtMsg.Clear();
        }

        private void btSave_Click(object sender, EventArgs e)
        {
            string strDirectory;
            string strFileName;
            SaveFileDialog saveFileDlg = new SaveFileDialog();

            if (rTxtMsg.Text == "")
            {
                return;
            }

            strDirectory = XmlHelper.GetNodeDefValue(_configPath, "/Config/LogPath", Application.StartupPath);
            saveFileDlg.Filter = "*.txt(文本文件)|*.txt";
            saveFileDlg.DefaultExt = "txt";
            saveFileDlg.FileName = "";
            saveFileDlg.ShowDialog();

            strFileName = saveFileDlg.FileName;
            if (strFileName.Length == 0)
            {
                return;
            }

            if (strDirectory != Path.GetDirectoryName(strFileName))
            {
                strDirectory = Path.GetDirectoryName(strFileName);
                XmlHelper.SetNodeValue(_configPath, "/Config", "LogPath", strDirectory);
            }

            using (StreamWriter sw = new StreamWriter(strFileName, false, Encoding.UTF8))
            {
                sw.WriteLine(rTxtMsg.Text.Replace("\n", "\r\n"));
            }

            ShowMsg(GetCurrentLang("保存成功") + " ！\r\n\r\n", Color.Green);
        }

        private void chkTime_CheckedChanged(object sender, EventArgs e)
        {
            string value = (chkTime.Checked ? "true" : "false");
            XmlHelper.SetNodeValue(_configPath, "/Config", "ShowTime", value);
        }

        private void btSendCtrlZ_Click(object sender, EventArgs e)
        {
            serialPort_SendData(new byte[] { 0x1a });
        }

        private void btSendEsc_Click(object sender, EventArgs e)
        {
            serialPort_SendData(new byte[] { 0x1b });
        }
        #endregion

        #region UI更新
        delegate void UpdateUi(string msg);
        private void UiOperateEnable(string msg = "")
        {
            if(this.InvokeRequired)
            {
                Invoke(new UpdateUi(UiOperateEnable), msg);
                return;
            }

            lbCmdStatus.Visible = false;

            if (_scom.IsOpen)
            {
                grpDevType.Enabled = false;
                grpQuery.Enabled = true;
                grpParam.Enabled = true;
                grpNwk.Enabled = true;
                btSend.Enabled = true;
            }
            else
            {
                grpDevType.Enabled = true;
            }

            btQryBand.BackColor = Color.DarkKhaki;
            btQryIMEI.BackColor = Color.DarkKhaki;
            btQrySimId.BackColor = Color.DarkKhaki;
            btQryVer.BackColor = Color.DarkKhaki;
            btQryTempVbat.BackColor = Color.DarkKhaki;
            btSetBand.BackColor = Color.DarkKhaki;
            btJoinNwk.BackColor = Color.DarkKhaki;
            btQryNwkStat.BackColor = Color.DarkKhaki;
            btCnctSvr.BackColor = Color.DarkKhaki;
            btDataUpload.BackColor = Color.DarkKhaki;
            btSend.BackColor = Color.DarkKhaki;
        }
        private void UiOperateDisable(string msg = "")
        {
            if (this.InvokeRequired)
            {
                Invoke(new UpdateUi(UiOperateDisable), msg);
                return;
            }

            lbCmdStatus.Visible = (msg != "" ? true : false);

            grpDevType.Enabled = false;
            grpQuery.Enabled = false;
            grpParam.Enabled = false;
            grpNwk.Enabled = false;
            btSend.Enabled = false;

            if (_scom.IsOpen == false)
            {
                grpDevType.Enabled = true;
            }

            btQryBand.BackColor = Color.Silver;
            btQryIMEI.BackColor = Color.Silver;
            btQrySimId.BackColor = Color.Silver;
            btQryVer.BackColor = Color.Silver;
            btQryTempVbat.BackColor = Color.Silver;
            btSetBand.BackColor = Color.Silver;
            btJoinNwk.BackColor = Color.Silver;
            btQryNwkStat.BackColor = Color.Silver;
            btCnctSvr.BackColor = Color.Silver;
            btDataUpload.BackColor = Color.Silver;
            btSend.BackColor = Color.Silver;
        }

        private void UpdateNetStatus(string msg)
        {
            if (this.InvokeRequired)
            {
                Invoke(new UpdateUi(UpdateNetStatus), msg);
                return;
            }

            if(msg.Contains("在线"))
            {
                lbNetState.Text = "入网状态：成功";
                lbNetState.BackColor = Color.Lime;
            }
            else
            {
                lbNetState.Text = "入网状态：离线";
                lbNetState.BackColor = Color.Silver;
            }

            MultiLanguage.UpdateControlDefaultText(lbNetState);
        }
        private void UpdateCnctStatus(string msg)
        {
            if (this.InvokeRequired)
            {
                Invoke(new UpdateUi(UpdateCnctStatus), msg);
                return;
            }

            if (msg.Contains("已连接")) // 已连接
            {
                btCnctSvr.Text = "断开连接";
                btCnctSvr.ForeColor = Color.Red;
                _IsCloudConnected = true;
            }
            else                        // 未连接
            {
                btCnctSvr.Text = "建立连接";
                btCnctSvr.ForeColor = Color.Black;
                _IsCloudConnected = false;
            }

            MultiLanguage.UpdateControlDefaultText(btCnctSvr);
        }
        #endregion

        #region 窗口关闭
        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            _thrTransceiver.Abort();
            if (_scom.IsOpen)
            {
                _scom.Close();
            }
        }
        #endregion
    }
}
