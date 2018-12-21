using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ElectricPowerDebuger.Common
{
    public class MultiLanguageForm : Form
    {
        public MultiLanguageForm()
        {
            if (!System.Threading.Thread.CurrentThread.CurrentUICulture.Name.Equals("zh-Hans", StringComparison.OrdinalIgnoreCase)) //如果是简体，则无需转换
            {
                base.TextChanged += MultiLanguageFormBase_TextChanged;
                base.Load += new System.EventHandler(this.MultiLanguageFormBase_Shown);
            }
        }

        private void MultiLanguageFormBase_TextChanged(object sender, EventArgs e)
        {
            this.Text = MultiLanguage.GetCurrentText(this.Text);
        }

        private void MultiLanguageFormBase_Shown(object sender, EventArgs e)
        {
            MultiLanguage.SetControlLanguageText(this);
            base.ControlAdded += MultiLanguageFormBase_ControlAdded;
        }

        private void MultiLanguageFormBase_ControlAdded(object sender, ControlEventArgs e)
        {
            MultiLanguage.SetControlLanguageText(e.Control);
        }

        /// <summary>
        /// 强制通知子控件改变消息
        /// </summary>
        /// <param name="target"></param>
        protected virtual void PerformChildrenChange(Control target)
        {
            MultiLanguage.SetControlLanguageText(target);
        }

        /// <summary>
        /// 弹出消息框
        /// </summary>
        /// <param name="text"></param>
        /// <param name="caption"></param>
        /// <param name="buttons"></param>
        /// <param name="icon"></param>
        /// <param name="defaultButton"></param>
        /// <returns></returns>
        protected DialogResult MessageBoxShow(string text, string caption, MessageBoxButtons buttons = MessageBoxButtons.OK, MessageBoxIcon icon = MessageBoxIcon.None, MessageBoxDefaultButton defaultButton = MessageBoxDefaultButton.Button1)
        {
            return MessageBox.Show(MultiLanguage.GetCurrentText(text), MultiLanguage.GetCurrentText(caption), buttons, icon, defaultButton);
        }

    }

    public class MultiLanguage
    {
        public static string CurrentLanguage;
        public static Dictionary<string, string> CurrentResource;
        private static Dictionary<Object, string> DefaultResource = new Dictionary<Object, string>();
        /// <summary>
        /// 初始化语言
        /// </summary>
        public static void InitLanguage(Control control)
        {
            System.Threading.Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.GetCultureInfo("zh-CN");
            
            if (CurrentResource == null)
            {
                CurrentResource = LangDict.dictEN;
            }

            //使用递归的方式对控件及其子控件进行处理
            SetControlLanguageText(control);

            //工具栏或者菜单动态构建窗体或者控件的时候，重新对子控件进行处理
            //control.ControlAdded += (sender, e) =>
            //{
            //    SetControlLanguageText(e.Control);
            //};
        }
        /// <summary>
        /// 内容的语言转化
        /// </summary>
        /// <param name="parent"></param>
        public static void SetControlLanguageText(System.Windows.Forms.Control parent)
        {
            CurrentLanguage = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;

            if (parent.HasChildren)
            {
                parent.Text = GetObjectText(parent);
                foreach (System.Windows.Forms.Control ctrl in parent.Controls)
                {
                    SetContainerLanguage(ctrl);
                }
            }
            else
            {
                SetLanguage(parent);
            }
        }

        /// <summary>
        /// 根据语言标识符得到转换后的值
        /// </summary>
        /// <param name="languageFlag"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetCurrentText(string key)
        {
            string strRet = "";

            string language = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;
            if (string.IsNullOrWhiteSpace(key) || language.Equals("zh-CN", StringComparison.OrdinalIgnoreCase))
            {
                strRet = key;
            }
            else
            {
                if (CurrentResource == null)
                {
                    switch (language)
                    {
                        case "en":
                        case "en-US":
                        default:
                            CurrentResource = LangDict.dictEN;
                            break;
                    }
                }

                strRet = CurrentResource.FirstOrDefault(q => q.Key == key).Value;
                if(strRet == null)
                {
                    strRet = key;
                }
            }

            return strRet;
        }

        public static string GetDefaultText(string value)
        {
            string strRet = "";

            string language = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;
            if (string.IsNullOrWhiteSpace(value) || language.Equals("zh-CN", StringComparison.OrdinalIgnoreCase))
            {
                strRet = value;
            }
            else
            {
                if (CurrentResource == null)
                {
                    switch (language)
                    {
                        case "en":
                        case "en-US":
                        default:
                            CurrentResource = LangDict.dictEN;
                            break;
                    }
                }

                strRet = CurrentResource.FirstOrDefault(q => q.Value == value).Key;
                if (strRet == null)
                {
                    strRet = value;
                }
            }

            return strRet;
        }
        /// <summary>
        /// 获取控件默认语言
        /// </summary>
        /// <param name="ctrl"></param>
        /// <returns></returns>
        public static string GetControlDefaultText(Control ctrl)
        {
            string strRet = "";
            Object obj = ctrl as Object;

            if (DefaultResource.ContainsKey(obj))
            {
                strRet = DefaultResource[obj];
            }
            else
            {
                string objText = "";

                if (obj is ComboBox)
                {
                    ComboBox comboBox = (ComboBox)obj;
                    for (int n = 0; n < comboBox.Items.Count; n++)
                    {
                        objText += comboBox.Items[n].ToString() + "/";
                    }
                    objText = objText.Remove(objText.Length - 1);
                }
                else if (obj is CheckedListBox)
                {
                    CheckedListBox chklistBox = (CheckedListBox)obj;
                    for (int n = 0; n < chklistBox.Items.Count; n++)
                    {
                        objText += chklistBox.Items[n].ToString() + "/";
                    }
                    objText = objText.Remove(objText.Length - 1);
                }
                else if (obj is DataGridViewColumn)
                {
                    objText = ((DataGridViewColumn)obj).HeaderText;
                }
                else if (obj is Control)
                {
                    objText = ((Control)obj).Text;
                }

                string language = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;
                if (language.Equals("zh-CN", StringComparison.OrdinalIgnoreCase))
                {
                    strRet = objText;
                }
                else
                {
                    if (CurrentResource == null)
                    {
                        switch (language)
                        {
                            case "en":
                            case "en-US":
                            default:
                                CurrentResource = LangDict.dictEN;
                                break;
                        }
                    }

                    strRet = CurrentResource.FirstOrDefault(q => q.Key == objText).Value;
                    if (strRet == null)
                    {
                        strRet = objText;
                    }
                }
            }

            return strRet;
        }

        /// <summary>
        /// 更新控件默认语言
        /// </summary>
        /// <param name="obj"></param>
        public static void UpdateControlDefaultText(Control ctrl)
        {
            string objText = "";
            Object obj = ctrl as Object;

            if (obj is ComboBox)
            {
                ComboBox comboBox = (ComboBox)obj;
                for (int n = 0; n < comboBox.Items.Count; n++)
                {
                    objText += comboBox.Items[n].ToString() + "/";
                }
                objText = objText.Remove(objText.Length - 1);
            }
            else if (obj is CheckedListBox)
            {
                CheckedListBox chklistBox = (CheckedListBox)obj;
                for (int n = 0; n < chklistBox.Items.Count; n++)
                {
                    objText += chklistBox.Items[n].ToString() + "/";
                }
                objText = objText.Remove(objText.Length - 1);
            }
            else if (obj is DataGridViewColumn)
            {
                objText = ((DataGridViewColumn)obj).HeaderText;
            }
            else if (obj is Control)
            {
                objText = ((Control)obj).Text;
            }

            /*
            if (obj is ToolStripItem)
            {
                objText = ((ToolStripItem)obj).Text;
            }
            else if (obj is ToolStripMenuItem)
            {
                objText = ((ToolStripMenuItem)obj).Text;
            }
            else if (obj is TreeNode)
            {
                objText = ((TreeNode)obj).Text;
            }
            else if (obj is TabPage)
            {
                objText = ((TabPage)obj).Text;
            }
            */


            if (DefaultResource.ContainsKey(obj))
            {
                DefaultResource[obj] = objText;
            }
            else
            {
                DefaultResource.Add(obj, objText);
            }

            SetControlLanguageText(ctrl);
        }

        #region 设置控件语言
        /// <summary>
        /// 设置容器类控件的语言
        /// </summary>
        /// <param name="ctrl"></param>
        /// <returns></returns>
        private static void SetContainerLanguage(System.Windows.Forms.Control ctrl)
        {
            if (ctrl is DataGridView)
            {
                try
                {
                    DataGridView dataGridView = (DataGridView)ctrl;
                    foreach (DataGridViewColumn dgvc in dataGridView.Columns)
                    {
                        try
                        {
                            if (dgvc.HeaderText.ToString() != "" && dgvc.Visible)
                            {
                                dgvc.HeaderText = GetObjectText(dgvc);
                            }
                        }
                        catch
                        {
                        }
                    }
                }
                catch (Exception)
                { }
            }
            if (ctrl is MenuStrip)
            {
                MenuStrip menuStrip = (MenuStrip)ctrl;
                foreach (ToolStripMenuItem toolItem in menuStrip.Items)
                {
                    try
                    {
                        toolItem.Text = GetObjectText(toolItem);
                    }
                    catch (Exception)
                    {
                    }
                    finally
                    {
                        if (toolItem.DropDownItems.Count > 0)
                        {
                            GetItemText(toolItem);
                        }
                    }
                }
            }
            else if (ctrl is TreeView)
            {
                TreeView treeView = (TreeView)ctrl;
                foreach (TreeNode node in treeView.Nodes)
                {
                    try
                    {
                        node.Text = GetObjectText(node);
                    }
                    catch (Exception)
                    {
                    }
                    finally
                    {
                        if (node.Nodes.Count > 0)
                        {
                            GetNodeText(node);
                        }
                    }
                }
            }
            else if (ctrl is TabControl)
            {
                TabControl tabCtrl = (TabControl)ctrl;
                try
                {
                    foreach (TabPage tabPage in tabCtrl.TabPages)
                    {
                        tabPage.Text = GetObjectText(tabPage);
                    }
                }
                catch (Exception)
                {
                }
            }
            else if (ctrl is StatusStrip)
            {
                StatusStrip statusStrip = (StatusStrip)ctrl;
                foreach (ToolStripItem toolItem in statusStrip.Items)
                {
                    try
                    {
                        toolItem.Text = GetObjectText(toolItem);
                    }
                    catch (Exception)
                    {
                    }
                    finally
                    {
                        ToolStripDropDownButton tsDDBtn = toolItem as ToolStripDropDownButton;
                        if (tsDDBtn != null && tsDDBtn.DropDownItems.Count > 0)
                        {
                            GetItemText(tsDDBtn);
                        }
                    }
                }
            }
            else if (ctrl is ToolStrip)
            {
                ToolStrip statusStrip = (ToolStrip)ctrl;
                foreach (ToolStripItem toolItem in statusStrip.Items)
                {
                    try
                    {
                        toolItem.Text = GetObjectText(toolItem);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            else if (ctrl is CheckedListBox)
            {
                CheckedListBox chkListBox = (CheckedListBox)ctrl;
                try
                {
                    if(chkListBox.Items.Count > 0)
                    {
                        GetObjectText(chkListBox);
                    }
                }
                catch (Exception)
                { }
            }
            else if (ctrl is ComboBox)
            {
                ComboBox comboBox = (ComboBox)ctrl;
                try
                {
                    if (comboBox.Items.Count > 0)
                    {
                        GetObjectText(comboBox);
                    }
                }
                catch (Exception)
                { }
            }
            else if (ctrl is GroupBox)
            {
                GroupBox grpBox = (GroupBox)ctrl;
                try
                {
                    grpBox.Text = GetObjectText(grpBox);
                }
                catch (Exception)
                {
                }
            }


            if (ctrl.HasChildren)
            {
                foreach (System.Windows.Forms.Control c in ctrl.Controls)
                {
                    SetContainerLanguage(c);
                }
            }
            else
            {
                SetLanguage(ctrl);
            }

        }
        /// <summary>
        /// 设置普通控件的语言
        /// </summary>
        /// <param name="ctrl"></param>
        /// <returns></returns>
        private static void SetLanguage(System.Windows.Forms.Control ctrl)
        {
            if (true)
            {
                if (ctrl is CheckBox)
                {
                    CheckBox checkBox = (CheckBox)ctrl;
                    try
                    {
                        checkBox.Text = GetObjectText(checkBox);
                    }
                    catch (Exception)
                    {
                    }
                }
                else if (ctrl is Label)
                {
                    Label label = (Label)ctrl;
                    try
                    {
                        label.Text = GetObjectText(label);
                    }
                    catch (Exception)
                    {
                    }
                }
                else if (ctrl is Button)
                {
                    Button button = (Button)ctrl;
                    try
                    {
                        button.Text = GetObjectText(button);
                    }
                    catch (Exception)
                    {
                    }
                }
                else if (ctrl is GroupBox)
                {
                    GroupBox groupBox = (GroupBox)ctrl;
                    try
                    {
                        groupBox.Text = GetObjectText(groupBox);
                    }
                    catch (Exception)
                    {
                    }
                }
                else if (ctrl is RadioButton)
                {
                    RadioButton radioButton = (RadioButton)ctrl;
                    try
                    {
                        radioButton.Text = GetObjectText(radioButton);
                    }
                    catch (Exception)
                    {
                    }
                }
                else if (ctrl is CheckedListBox)
                {
                    CheckedListBox chkListBox = (CheckedListBox)ctrl;
                    try
                    {
                        if (chkListBox.Items.Count > 0)
                        {
                            GetObjectText(chkListBox);
                        }
                    }
                    catch (Exception)
                    { }
                }
                else if (ctrl is ComboBox)
                {
                    ComboBox comboBox = (ComboBox)ctrl;
                    try
                    {
                        if (comboBox.Items.Count > 0)
                        {
                            GetObjectText(comboBox);
                        }
                    }
                    catch (Exception)
                    { }
                }
            }

        }
        /// <summary>
        /// 递归转化菜单
        /// </summary>
        /// <param name="menuItem"></param>
        private static void GetItemText(ToolStripDropDownItem menuItem)
        {
            foreach (ToolStripItem toolItem in menuItem.DropDownItems)
            {
                try
                {
                    toolItem.Text = GetObjectText(toolItem);
                }
                catch (Exception)
                {
                }
                finally
                {
                    if (toolItem is ToolStripDropDownItem)
                    {
                        ToolStripDropDownItem subMenuStrip = (ToolStripDropDownItem)toolItem;
                        if (subMenuStrip.DropDownItems.Count > 0)
                        {
                            GetItemText(subMenuStrip);
                        }
                    }
                }

            }
        }
        /// <summary>
        /// 递归转化树
        /// </summary>
        /// <param name="menuItem"></param>
        private static void GetNodeText(TreeNode node)
        {

            foreach (TreeNode treeNode in node.Nodes)
            {
                try
                {
                    treeNode.Text = GetObjectText(treeNode);
                }
                catch (Exception)
                {
                }
                finally
                {
                    if (treeNode.Nodes.Count > 0)
                    {
                        GetNodeText(treeNode);
                    }
                }
            }
        }

        private static string GetObjectText(Object obj)
        {
            string objText = "";

            string language = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;
            if (language.Equals("zh-CN", StringComparison.OrdinalIgnoreCase))
            {
                // default language
                if (DefaultResource.ContainsKey(obj))
                {
                    objText = DefaultResource[obj];

                    if (obj is ComboBox)
                    {
                        ComboBox comboBox = (ComboBox)obj;
                        string[] items = objText.Split('/');
                        for (int n = 0; n < comboBox.Items.Count && items.Length == comboBox.Items.Count; n++)
                        {
                            comboBox.Items[n] = items[n];
                        }
                    }
                    else if (obj is CheckedListBox)
                    {
                        CheckedListBox chklistBox = (CheckedListBox)obj;
                        string[] items = objText.Split('/');
                        for (int n = 0; n < chklistBox.Items.Count && items.Length == chklistBox.Items.Count; n++)
                        {
                            chklistBox.Items[n] = items[n];
                        }
                    }
                    else
                    {
                        
                    }
                }
                else
                {
                    /*
                    if (obj is ToolStripItem)
                    {
                        objText = ((ToolStripItem)obj).Text;
                    }
                    else if (obj is ToolStripMenuItem)
                    {
                        objText = ((ToolStripMenuItem)obj).Text;
                    }
                    else if (obj is TreeNode)
                    {
                        objText = ((TreeNode)obj).Text;
                    }
                    else if (obj is TabPage)
                    {
                        objText = ((TabPage)obj).Text;
                    }
                    //else 
                     */
                    if (obj is ComboBox)
                    {
                        ComboBox comboBox = (ComboBox)obj;
                        for (int n = 0; n < comboBox.Items.Count; n++)
                        {
                            objText += comboBox.Items[n].ToString() + "/";
                        }
                        objText = objText.Remove(objText.Length - 1);
                    }
                    else if (obj is CheckedListBox)
                    {
                        CheckedListBox chklistBox = (CheckedListBox)obj;
                        for (int n = 0; n < chklistBox.Items.Count; n++)
                        {
                            objText += chklistBox.Items[n].ToString() + "/";
                        }
                        objText = objText.Remove(objText.Length - 1);
                    }
                    else if (obj is DataGridViewColumn)
                    {
                        objText = ((DataGridViewColumn)obj).HeaderText;
                    }
                    else if (obj is Control)
                    {
                        objText = ((Control)obj).Text;
                    }
                    else if (obj is object)
                    {
                        objText = ((object)obj).ToString();
                    }

                    DefaultResource.Add(obj, objText);
                }
            }
            else
            {
                // other language
                if (DefaultResource.ContainsKey(obj) == true)
                {
                    objText = DefaultResource[obj];

                    if (obj is ComboBox)
                    {
                        ComboBox comboBox = (ComboBox)obj;
                        //int selectIndex = comboBox.SelectedIndex;
                        string[] items = objText.Split('/');
                        for (int n = 0; n < comboBox.Items.Count && items.Length == comboBox.Items.Count; n++)
                        {
                            comboBox.Items[n] = GetCurrentText(items[n]);
                        }
                        //comboBox.SelectedIndex = selectIndex;
                    }
                    else if (obj is CheckedListBox)
                    {
                        CheckedListBox chklistBox = (CheckedListBox)obj;
                        //int selectIndex = chklistBox.SelectedIndex;
                        string[] items = objText.Split('/');
                        for (int n = 0; n < chklistBox.Items.Count && items.Length == chklistBox.Items.Count; n++)
                        {
                            chklistBox.Items[n] = GetCurrentText(items[n]);
                        }
                        //chklistBox.SelectedIndex = selectIndex;
                    }
                    else
                    {
                        objText = GetCurrentText(objText);
                    }

                }
            }

            return objText;
        }
        #endregion
    }
}
