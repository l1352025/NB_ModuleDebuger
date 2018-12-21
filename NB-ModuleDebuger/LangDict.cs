using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ElectricPowerDebuger.Common
{
    class LangDict
    {
        public static Dictionary<string, string> dictEN = new Dictionary<string, string>()
        {
            // 界面
            {"NB模组调试软件",                "NB-Module_Debuger"},
            {"上海桑锐电子科技股份有限公司",  "Shanghai Sunray Electronics Technology Co., Ltd."},

            {"界面语言",            "UI Language"},
            {"简体中文(zh-CN)",     "简体中文(zh-CN)"},
            {"English (en-US)",     "English (en-US)"},
            {"模组选择",            "Module Select"},
            {"型号",                "Model"},
            
            {"连接设备",            "Connect Device"},
            {"端口号",              "Port"},
            {"打开串口",            "Open"},
            {"关闭串口",            "Close"},
            
            {"信息查询",            "Information Query"},
            {"版本号",              "Version"},
            {"IMEI号码",            "IMEI"},
            {"BAND值",              "Band"},
            {"SIM卡ID",             "IMSI"},
            {"温度、电池电压",      "Temp、Vbat"},

            {"参数设置",            "Parameter Setting"},
            {"BAND设置",            "Set Band "},
            
            {"网络连接",            "Network Connection"},
            {"入网",                "Join Network "},
            {"网络状态信息",        "Query UEStatus"},
            {"云平台",              "Server"},
            {"CDP服务器",           "CDP Server"},
            {"UDP服务器",           "UDP Server"},
            {"OneNet平台",          "CMCC OneNet"},
            {"建立连接",            "Connect "},
            {"断开连接",            "Disconnect "},
            {"数据\r\n上传",        "Data\r\nUpload"},
            {"查看接收数据",        "Check Receive"},
            {"入网状态：离线",      "NetState：Offline"},
            {"入网状态：成功",      "NetState：Online"},

            {"通信记录",            "Message Log"},
            {"清空记录",            "Clear Log"},
            {"保存记录",            "Save Log"},
            {"显示时间",            "Show Time"},
            {"发送Ctrl-Z",          "Send Ctrl-Z"},
            {"发送ESC",             "Send ESC"},
            {"  AT 指 令 ：",       "AT Command:"},
            {"发送",                "Send"},
            {"命令执行中...",       "Command Executing ..."},

            // 提示信息
            {"入网启动中...",                    "Join Network Starting ..."},
            {"请入网后再连接平台",               "Please Join Network Before Connect Server"},
            {"输入的Ip或Port无效：",             "The Input Ip Or Port Is Invalid : "},
            {"请连接平台后再上传",               "Please Connect Server Before Upload Data"},
            {"请输入数据后再上传",               "Please Input Data Before Upload"},
            {"请连接平台后再接收",               "Please Connect Server Before Check Receive"},
            
            {"[ 串口连接已断开 ]",               "[ SerialPort Disconnected ]"},
            {"打开串口失败",                     "Open SerialPort Failed"},
            {"关闭串口失败",                     "Close SerialPort Failed"},
            {"模组检测中...",                    "Module Check Stating ..."},
            {"连接的可能不是",                   "Current Connect Device Maybe Not"},
            {"模组",                             "Module"},
            {"请至少选择一个Band值进行设置",     "Please choose at least one Band to set"},
            {"成功",                             "Success"},
            {"失败",                             "Failed"},

            // 接收解析
            {"收到数据",                "Received Data"},
            {"网络状态",                "UEStatus"},
            {"软件版本",                "Software Version"},
            {"硬件版本",                "Hardware Version"},
            {"温度",                    "Temperature"},
            {"电池电压",                "Battery Voltage"},

            // AT指令名
            {"模组检测",                "Module Check "},
            {"连接平台",                "Connect Server "},
            {"数据上传",                "Upload Data "},
            {"查看接收缓存",            "Check Receive Buffer "},

            {"查询版本号",            "Query Version "},
            {"查询软件版本",          "Query Software Version "},
            {"查询硬件版本",          "Query Hardware Version "},
            {"查询BAND",              "Query Band "},
            {"查询IMEI",              "Query IMEI "},
            {"查询SIM卡ID",           "Query IMSI "},
            {"查询温度和电压",        "Query Temperature and Battery Voltage "},
            {"查询入网状态",          "Query NetState "},
            {"查询网络状态信息",      "Query UEStatus "},

            {"关闭SIM卡",             "Close Sim Function "},
            {"设置BAND",              "Set Band "},
            {"打开SIM卡",             "Open Sim Function "},
            {"模组复位及入网",        "Module Reset And AutoRegister "},
            {"激活PDP上下文",         "Active PDP Context "},
            {"模组复位",              "Module Reset "},
            {"模组配置",              "Module Configure-0 "},
            {"模组配置1",             "Module Configure-1 "},
            {"模组配置2",             "Module Configure-2 "},
            {"入网激活",              "Attach to Packet Domain Service "},
            
            {"设置COAP协议IP",        "Configure CDP Server "},
            {"打开接收上报",          "Enable New Message Indication "},
            {"创建通信Socket",        "Create Socket "},
            {"创建通信套件",          "Create Communication Suite "},
            {"添加设备对象",          "Add Device Object "},
            {"注册设备",              "Open OneNet Connection "},
            {"观察对象应答",          "Response To Observe Object "},
            {"发现对象资源应答",      "Response To Discovery Resource "},
            
            {"发送数据",              "Send Data "},
            {"发送UDP数据",           "Send UDP Data "},
            {"上报对象资源",          "Report Object Resource "},
            {"接收Socket数据",        "Receive Socket Data "},

            {"关闭CDP连接",           "Close CDP Connection "},
            {"关闭通信Socket",        "Close Socket "},
            {"注销设备",              "Close OneNet Connection "},
            {"删除通信套件",          "Delete Communication Suite "},

            {"自定义AT指令",          "Send Custom AT Command "},
        };
    }
}
