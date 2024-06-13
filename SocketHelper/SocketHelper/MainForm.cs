using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SocketHelper
{
    public partial class MainForm : Form
    {
        Dictionary<string, UserControl_TcpClient> dic_UserControl_TcpClient = new Dictionary<string, UserControl_TcpClient>();
        Dictionary<string, UserControl_UdpClient> dic_UserControl_UdpClient = new Dictionary<string, UserControl_UdpClient>();
        Dictionary<string, UserControl_UdpServer> dic_UserControl_UdpServer = new Dictionary<string, UserControl_UdpServer>();
        Dictionary<string, UserControl_UdpGroup> dic_UserControl_UdpGroup = new Dictionary<string, UserControl_UdpGroup>();
        Dictionary<string, UserControl_TcpServer> dic_UserControl_TcpServer = new Dictionary<string, UserControl_TcpServer>();
        Dictionary<string, UserControl_TcpServer_Communication> dic_UserControl_TcpServer_Communication = new Dictionary<string, UserControl_TcpServer_Communication>();
        int tcpClientIndex = 0;
        int tcpServerMonitorIndex = 0;

        int udpClientIndex = 0;
        int udpServertIndex = 0;
        int udpGroupIndex = 0;
        public MainForm()
        {
            InitializeComponent();
            GlobalDelegate.AddTcpServerClientDelegate = AddServerClient;
            GlobalDelegate.deleteTcpServerCommunicationDelegate = deleteTcpServerCommunication;
            GlobalDelegate.updateTcpServerReceiveStrDelegate = UpdateStr;

            Icon = Icons.MainIcon;
            treeViewSocket.ImageList = new();
            treeViewSocket.ImageList.Images.AddRange([
                Icons.SocketTool,
                Icons.Cloud,
                Icons.Client,
                Icons.Group,
                ]);
            //treeViewSocket.ImageIndex = 0;
            //treeViewSocket.SelectedImageIndex = 0;
        }

        private void toolStripMenuItem_Create_Click(object sender, EventArgs e)
        {
            if (treeViewSocket.SelectedNode.Text == "TCP Client")
            {
                TcpClientCreateForm tcpClientCreateForm = new TcpClientCreateForm();
                if (tcpClientCreateForm.ShowDialog() == DialogResult.OK)
                {
                    tcpClientIndex++;
                    UserControl_TcpClient userControl_TcpClient = new UserControl_TcpClient(tcpClientCreateForm.Ip, tcpClientCreateForm.Port);
                    dic_UserControl_TcpClient.Add(tcpClientIndex.ToString(), userControl_TcpClient);
                    Control control = tableLayoutPanel1.GetControlFromPosition(1, 0);
                    tableLayoutPanel1.Controls.Remove(control);
                    tableLayoutPanel1.Controls.Add(userControl_TcpClient, 1, 0);




                    string nodeText = $"{tcpClientCreateForm.Ip}:{tcpClientCreateForm.Port }";
                    TreeNode treeNode = new TreeNode(nodeText);
                    treeNode.ImageIndex = 2;
                    treeNode.SelectedImageIndex  = 2;
                    treeNode.Tag = tcpClientIndex;
                    treeViewSocket.SelectedNode.Nodes.Add(treeNode);
                    treeViewSocket.SelectedNode.ExpandAll();
                }
            }
            else if (treeViewSocket.SelectedNode.Text == "TCP Server")
            {
                TcpServerCreateForm tcpServerCreateForm = new TcpServerCreateForm();
                if (tcpServerCreateForm.ShowDialog() == DialogResult.OK)
                {

                    bool isPortExist = false;
                    foreach (var item in dic_UserControl_TcpServer)
                    {
                        if (item.Value.Port == tcpServerCreateForm.Port)
                        {
                            isPortExist = true;
                            break;
                        }
                    }
                    if (isPortExist)
                    {
                        MessageBox.Show($"{dic_UserControl_TcpServer[tcpServerMonitorIndex.ToString()].Port}已经被创建");
                    }
                    else
                    {
                        tcpServerMonitorIndex++;

                        UserControl_TcpServer serControl_TcpServer = new UserControl_TcpServer(tcpServerCreateForm.Port);
                        dic_UserControl_TcpServer.Add(tcpServerMonitorIndex.ToString(), serControl_TcpServer);
                        Control control = tableLayoutPanel1.GetControlFromPosition(1, 0);
                        tableLayoutPanel1.Controls.Remove(control);
                        tableLayoutPanel1.Controls.Add(serControl_TcpServer, 1, 0);




                        string nodeText = $"{tcpServerCreateForm.Port }";
                        TreeNode treeNode = new TreeNode(nodeText);
                        treeNode.ImageIndex = 1;
                        treeNode.SelectedImageIndex  = 1;
                        treeNode.Tag = tcpServerMonitorIndex;
                        treeViewSocket.SelectedNode.Nodes.Add(treeNode);
                        treeViewSocket.SelectedNode.ExpandAll();
                    }
                }
            }
            else if (treeViewSocket.SelectedNode.Text == "UDP Client")
            {
                UdpClientCreateForm udpClientCreateForm = new UdpClientCreateForm();
                if (udpClientCreateForm.ShowDialog() == DialogResult.OK)
                {
                    udpClientIndex++;
                    UserControl_UdpClient userControl_UdpClient = new UserControl_UdpClient(udpClientCreateForm.Ip, udpClientCreateForm.RemotePort, udpClientCreateForm.LocalPort);
                    dic_UserControl_UdpClient.Add(udpClientIndex.ToString(), userControl_UdpClient);
                    Control control = tableLayoutPanel1.GetControlFromPosition(1, 0);
                    tableLayoutPanel1.Controls.Remove(control);
                    tableLayoutPanel1.Controls.Add(userControl_UdpClient, 1, 0);




                    string nodeText = $"{udpClientCreateForm.Ip}:{udpClientCreateForm.RemotePort  }";
                    TreeNode treeNode = new TreeNode(nodeText);
                    treeNode.Tag = udpClientIndex;
                    treeViewSocket.SelectedNode.Nodes.Add(treeNode);
                    treeViewSocket.SelectedNode.ExpandAll();
                }

            }
            else if (treeViewSocket.SelectedNode.Text == "UDP Server")
            {
                UdpServerCreateForm udpServerCreateForm = new UdpServerCreateForm();
                if (udpServerCreateForm.ShowDialog() == DialogResult.OK)
                {
                    bool isPortExist = false;
                    foreach (var item in dic_UserControl_UdpServer)
                    {
                        if (item.Value.LocalPort == udpServerCreateForm.Port)
                        {
                            isPortExist = true;
                            break;
                        }
                    }
                    if (isPortExist)
                    {
                        MessageBox.Show($"{dic_UserControl_UdpServer[udpServertIndex.ToString()].LocalPort}已经被创建");
                    }
                    else
                    {

                        udpServertIndex++;
                        UserControl_UdpServer userControl_UdpServer = new UserControl_UdpServer(udpServerCreateForm.Port);
                        dic_UserControl_UdpServer.Add(udpServertIndex.ToString(), userControl_UdpServer);
                        Control control = tableLayoutPanel1.GetControlFromPosition(1, 0);
                        tableLayoutPanel1.Controls.Remove(control);
                        tableLayoutPanel1.Controls.Add(userControl_UdpServer, 1, 0);




                        string nodeText = $"{udpServerCreateForm.Port  }";
                        TreeNode treeNode = new TreeNode(nodeText);
                        treeNode.Tag = udpServertIndex;
                        treeViewSocket.SelectedNode.Nodes.Add(treeNode);
                        treeViewSocket.SelectedNode.ExpandAll();
                    }
                }

            }
            else if (treeViewSocket.SelectedNode.Text == "UDP Group")
            {
                UdpGroupCreateForm udpGroupCreateForm = new UdpGroupCreateForm();
                if (udpGroupCreateForm.ShowDialog() == DialogResult.OK)
                {
                    udpGroupIndex++;
                    UserControl_UdpGroup userControl_UdpGroup = new UserControl_UdpGroup(udpGroupCreateForm.Ip, udpGroupCreateForm.Port);
                    dic_UserControl_UdpGroup.Add(udpGroupIndex.ToString(), userControl_UdpGroup);
                    Control control = tableLayoutPanel1.GetControlFromPosition(1, 0);
                    tableLayoutPanel1.Controls.Remove(control);
                    tableLayoutPanel1.Controls.Add(userControl_UdpGroup, 1, 0);




                    string nodeText = $"{udpGroupCreateForm.Port  }";
                    TreeNode treeNode = new TreeNode(nodeText);
                    treeNode.ImageIndex  = 3;
                    treeNode.Tag = udpGroupIndex;
                    treeViewSocket.SelectedNode.Nodes.Add(treeNode);
                    treeViewSocket.SelectedNode.ExpandAll();

                }

            }
        }

        private void treeViewSocket_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Parent != null && e.Node.Parent.Text == "TCP Client")
            {
                string key = e.Node.Tag.ToString();
                UserControl_TcpClient userControl_TcpClient = dic_UserControl_TcpClient[key];
                Control control = tableLayoutPanel1.GetControlFromPosition(1, 0);
                tableLayoutPanel1.Controls.Remove(control);
                tableLayoutPanel1.Controls.Add(userControl_TcpClient, 1, 0);

            }
            else if (e.Node.Parent != null && e.Node.Parent.Text == "UDP Client")
            {
                string key = e.Node.Tag.ToString();
                UserControl_UdpClient userControl_UdpClient = dic_UserControl_UdpClient[key];
                Control control = tableLayoutPanel1.GetControlFromPosition(1, 0);
                tableLayoutPanel1.Controls.Remove(control);
                tableLayoutPanel1.Controls.Add(userControl_UdpClient, 1, 0);

            }
            else if (e.Node.Parent != null && e.Node.Parent.Text == "TCP Server")
            {
                string key = e.Node.Tag.ToString();
                UserControl_TcpServer userControl_TcpServer = dic_UserControl_TcpServer[key];
                Control control = tableLayoutPanel1.GetControlFromPosition(1, 0);
                tableLayoutPanel1.Controls.Remove(control);
                tableLayoutPanel1.Controls.Add(userControl_TcpServer, 1, 0);
            }
            else if (e.Node.Parent != null && e.Node.Parent.Parent != null && e.Node.Parent.Parent.Text == "TCP Server")
            {
                string key = e.Node.Text.ToString();
                UserControl_TcpServer_Communication userControl_TcpServer_Communication = dic_UserControl_TcpServer_Communication[key];
                Control control = tableLayoutPanel1.GetControlFromPosition(1, 0);
                tableLayoutPanel1.Controls.Remove(control);
                tableLayoutPanel1.Controls.Add(userControl_TcpServer_Communication, 1, 0);
            }
            else if (e.Node.Parent != null && e.Node.Parent.Text == "UDP Server")
            {
                string key = e.Node.Tag.ToString();
                UserControl_UdpServer userControl_UdpServer = dic_UserControl_UdpServer[key];
                Control control = tableLayoutPanel1.GetControlFromPosition(1, 0);
                tableLayoutPanel1.Controls.Remove(control);
                tableLayoutPanel1.Controls.Add(userControl_UdpServer, 1, 0);
            }
            else if (e.Node.Parent != null && e.Node.Parent.Text == "UDP Group")
            {
                string key = e.Node.Tag.ToString();
                UserControl_UdpGroup userControl_UdpGroup = dic_UserControl_UdpGroup[key];
                Control control = tableLayoutPanel1.GetControlFromPosition(1, 0);
                tableLayoutPanel1.Controls.Remove(control);
                tableLayoutPanel1.Controls.Add(userControl_UdpGroup, 1, 0);
            }
        }

        private void toolStripMenuItem_Delete_Click(object sender, EventArgs e)
        {
            TreeNode treeNode = treeViewSocket.SelectedNode;
            if (treeNode.Parent != null && treeNode.Parent.Text == "TCP Client")
            {

                string key = treeNode.Tag.ToString();
                Control control = tableLayoutPanel1.GetControlFromPosition(1, 0);
                tableLayoutPanel1.Controls.Remove(control);
                dic_UserControl_TcpClient.Remove(key);
                string nextKey = (Convert.ToInt32(key) + 11).ToString();
                string preKey = (Convert.ToInt32(key) - 1).ToString();
                if (dic_UserControl_TcpClient.ContainsKey(preKey))
                {
                    UserControl_TcpClient userControl_TcpClient_Show = dic_UserControl_TcpClient[preKey];
                    tableLayoutPanel1.Controls.Add(userControl_TcpClient_Show, 1, 0);
                }
                else if (dic_UserControl_TcpClient.ContainsKey(nextKey))
                {
                    UserControl_TcpClient userControl_TcpClient_Show = dic_UserControl_TcpClient[nextKey];
                    tableLayoutPanel1.Controls.Add(userControl_TcpClient_Show, 1, 0);
                }
                treeViewSocket.Nodes.Remove(treeNode);
            }
            else if (treeNode.Parent != null && treeNode.Parent.Text == "UDP Client")
            {

                string key = treeNode.Tag.ToString();
                Control control = tableLayoutPanel1.GetControlFromPosition(1, 0);
                tableLayoutPanel1.Controls.Remove(control);
                dic_UserControl_UdpClient.Remove(key);
                string nextKey = (Convert.ToInt32(key) + 11).ToString();
                string preKey = (Convert.ToInt32(key) - 1).ToString();
                if (dic_UserControl_UdpClient.ContainsKey(preKey))
                {
                    UserControl_UdpClient userControl_UdpClient_Show = dic_UserControl_UdpClient[preKey];
                    tableLayoutPanel1.Controls.Add(userControl_UdpClient_Show, 1, 0);
                }
                else if (dic_UserControl_UdpClient.ContainsKey(nextKey))
                {
                    UserControl_UdpClient userControl_UdpClient_Show = dic_UserControl_UdpClient[nextKey];
                    tableLayoutPanel1.Controls.Add(userControl_UdpClient_Show, 1, 0);
                }
                treeViewSocket.Nodes.Remove(treeNode);
            }

            else if (treeNode.Parent != null && treeNode.Parent.Parent == null && treeNode.Parent.Text == "TCP Server")
            {
                string key = treeNode.Tag .ToString();
                Control control = tableLayoutPanel1.GetControlFromPosition(1, 0);
                tableLayoutPanel1.Controls.Remove(control);
                dic_UserControl_TcpServer.Remove(key);
                string nextKey = treeNode.NextNode == null ? "" : treeNode.NextNode.Tag .ToString ();
                string preKey = treeNode.PrevNode == null ? "" : treeNode.PrevNode.Tag .ToString ();
                if (dic_UserControl_TcpServer.ContainsKey(preKey))
                {
                    UserControl_TcpServer userControl_TcpServer_Show = dic_UserControl_TcpServer[preKey];
                    tableLayoutPanel1.Controls.Add(userControl_TcpServer_Show, 1, 0);
                }
                else if (dic_UserControl_TcpServer.ContainsKey(nextKey))
                {
                    UserControl_TcpServer userControl_TcpServer_Show = dic_UserControl_TcpServer[nextKey];
                    tableLayoutPanel1.Controls.Add(userControl_TcpServer_Show, 1, 0);
                }
                treeNode.Remove();
            }
            else if (treeNode.Parent != null && treeNode.Parent.Parent != null && treeNode.Parent.Parent.Text == "TCP Server")
            {
                string key = treeNode.Text.ToString();
                Control control = tableLayoutPanel1.GetControlFromPosition(1, 0);
                tableLayoutPanel1.Controls.Remove(control);
                dic_UserControl_TcpServer_Communication.Remove(key);
                string nextKey = treeNode.NextNode == null ? "" : treeNode.NextNode.Text;
                string preKey = treeNode.PrevNode == null ? "" : treeNode.PrevNode.Text;
                if (dic_UserControl_TcpClient.ContainsKey(preKey))
                {
                    UserControl_TcpClient userControl_TcpClient_Show = dic_UserControl_TcpClient[preKey];
                    tableLayoutPanel1.Controls.Add(userControl_TcpClient_Show, 1, 0);
                }
                else if (dic_UserControl_TcpClient.ContainsKey(nextKey))
                {
                    UserControl_TcpClient userControl_TcpClient_Show = dic_UserControl_TcpClient[nextKey];
                    tableLayoutPanel1.Controls.Add(userControl_TcpClient_Show, 1, 0);
                }
                treeNode.Remove();
            }
            else if (treeNode.Parent != null && treeNode.Parent.Text == "UDP Server")
            {

                string key = treeNode.Tag.ToString();
                Control control = tableLayoutPanel1.GetControlFromPosition(1, 0);
                tableLayoutPanel1.Controls.Remove(control);
                dic_UserControl_UdpServer.Remove(key);
                string nextKey = (Convert.ToInt32(key) + 11).ToString();
                string preKey = (Convert.ToInt32(key) - 1).ToString();
                if (dic_UserControl_UdpServer.ContainsKey(preKey))
                {
                    UserControl_UdpServer userControl_UdpServer = dic_UserControl_UdpServer[preKey];
                    tableLayoutPanel1.Controls.Add(userControl_UdpServer, 1, 0);
                }
                else if (dic_UserControl_UdpServer.ContainsKey(nextKey))
                {
                    UserControl_UdpServer userControl_UdpServer = dic_UserControl_UdpServer[nextKey];
                    tableLayoutPanel1.Controls.Add(userControl_UdpServer, 1, 0);
                }
                treeViewSocket.Nodes.Remove(treeNode);
            }
            else if (treeNode.Parent != null && treeNode.Parent.Text == "UDP Group")
            {

                string key = treeNode.Tag.ToString();
                Control control = tableLayoutPanel1.GetControlFromPosition(1, 0);
                tableLayoutPanel1.Controls.Remove(control);
                dic_UserControl_UdpGroup.Remove(key);
                string nextKey = (Convert.ToInt32(key) + 11).ToString();
                string preKey = (Convert.ToInt32(key) - 1).ToString();
                if (dic_UserControl_UdpGroup.ContainsKey(preKey))
                {
                    UserControl_UdpGroup userControl_UdpGroup = dic_UserControl_UdpGroup[preKey];
                    tableLayoutPanel1.Controls.Add(userControl_UdpGroup, 1, 0);
                }
                else if (dic_UserControl_UdpGroup.ContainsKey(nextKey))
                {
                    UserControl_UdpGroup userControl_UdpGroup = dic_UserControl_UdpGroup[nextKey];
                    tableLayoutPanel1.Controls.Add(userControl_UdpGroup, 1, 0);
                }
                treeViewSocket.Nodes.Remove(treeNode);
            }
        }





        private void deleteTcpServerCommunication(string remoteInfo)
        {

            int port = dic_UserControl_TcpServer_Communication[remoteInfo].Port;
            TreeNode treeNode = FindParentNode(port);
            TreeNodeCollection subTreenodes = treeNode.Nodes;
            int nodeIndex = -1;

            foreach (TreeNode item in subTreenodes)
            {
                nodeIndex++;
                if (item.Text == remoteInfo)
                {
                    item.Remove();//移除当前节点
                    break;
                }

            }

            int preNodeIndex = nodeIndex - 1;
            int nextNodeIndex = nodeIndex + 1;
            int showNodeIndex = -1;
            if (preNodeIndex >= 0)
            {
                showNodeIndex = preNodeIndex;
            }
            else if (nextNodeIndex > 0)
            {
                showNodeIndex = nextNodeIndex;
            }
            TreeNode showNode = null;
            for (int i = 0; i < subTreenodes.Count; i++)
            {
                if (i == showNodeIndex)
                {
                    showNode = subTreenodes[showNodeIndex];
                    break;
                }
            }



            Control control = tableLayoutPanel1.GetControlFromPosition(1, 0);
            tableLayoutPanel1.Controls.Remove(control);
            dic_UserControl_TcpServer_Communication.Remove(remoteInfo);
            tableLayoutPanel1.Controls.Add(dic_UserControl_TcpServer_Communication[showNode.Text], 1, 0);
        }

        private void AddServerClient(Socket socket, string remoteInfo, int port)
        {
            this.Invoke(new Action(() =>
            {

                Control control = tableLayoutPanel1.GetControlFromPosition(1, 0);
                tableLayoutPanel1.Controls.Remove(control);

                UserControl_TcpServer_Communication userControl_TcpServer_Communication = new UserControl_TcpServer_Communication(remoteInfo, port, socket);
                tableLayoutPanel1.Controls.Add(userControl_TcpServer_Communication, 1, 0);
                dic_UserControl_TcpServer_Communication.Add(remoteInfo, userControl_TcpServer_Communication);


                TreeNode treeNode = new TreeNode(remoteInfo);
                TreeNode parentNode = FindParentNode(port);
                parentNode.Nodes.Add(treeNode);
                parentNode.ExpandAll();
            }));
        }


        private void UpdateStr(string key, string strInfo)
        {
            dic_UserControl_TcpServer_Communication[key].updateReceiveStr(strInfo);
        }


        private TreeNode FindParentNode(int port)
        {
            TreeNodeCollection treeNodeCollection = treeViewSocket.Nodes;
            TreeNode node = null;
            foreach (TreeNode item in treeNodeCollection)
            {
                if (item.Text == "TCP Server")
                {
                    TreeNodeCollection treeNode = item.Nodes;
                    foreach (TreeNode subItem in treeNode)
                    {
                        if (subItem.Text == port.ToString())
                        {
                            node = subItem;
                            return node;
                        }
                    }

                }
            }
            return null;
        }
    }
}
