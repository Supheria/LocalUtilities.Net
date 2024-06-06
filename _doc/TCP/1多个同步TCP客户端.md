# C# Socket通信从入门到精通（1）——单个同步TCP客户端C#代码实现（史上最全）

## 前言：

Socket通信中有tcp通信，并且tcp有客户端和服务器，本文介绍的就是如何使用c#去实现一个tcp客户端，并且由于c#中有多种方式可以实现tcp客户端的功能，本文介绍的是使用TcpClient类来实现tcp客户端功能（**如需源码，订阅专栏后加入文章底部qq群获取**），demo软件界面如下：

  ![在这里插入图片描述](.\source\ff4903f9125948a6817302b3dccaf5df.png)

### 1.1 同步连接服务器

，我们使用工具软件建立一个tcp服务器，服务器的Ip地址为127.0.0.1，端口号为60000，如下图：  
![在这里插入图片描述](.\source\7d196e6802c24b8590136d9fa2751013.png)  
然后我们使用我开发的demo软件，我们在IP这一栏输入服务器的IP地址127.0.0.1，在端口号这一栏输入60000，然后点击“连接”按钮，如下图：  
![在这里插入图片描述](.\source\bb0a2844de03409ba70855b79afdf1bd.png)  
如果连接服务器成功，则会出现“连接成功”的弹窗，并且“连接”按钮上的文本变成了“断开”  
![在这里插入图片描述](.\source\d000e0d1de57490d864ded9122e0f548.png)

![在这里插入图片描述](.\source\33b5e1552c984700b08644dacd8ea204.png)

### 1.2 同步发送数据

如果我们想实现将客户端的数据发送到服务器，则在“数据发送区”输入文本，比如“123”，然后点击“发送”按钮，则能在服务器软件上看到客户端发送的数据如下：  
![在这里插入图片描述](.\source\06b318c0aa4749a1a5c657fba994d8b6.png)  
![在这里插入图片描述](.\source\4b65d3d4b31446d2b8fc2bcde4c0a5a7.png)

### 1.3 同步接收数据

如果想实现客户端接收服务器发送过来的数据，则使用服务器工具软件，比如输入“456”，然后点击发送，则能在我写的demo软件的“数据接收区”看到服务器发送的文本。  
![在这里插入图片描述](.\source\15873e5f9a7a4370b212553fa74564a9.png)  
![在这里插入图片描述](.\source\8e793fa9922a422db32374b0ece75706.png)

### 1.4 断开服务器连接

如果想断开客户端与服务器的连接，则点击“断开”按钮，则就能断开与服务器的连接，并且“断开”按钮文本恢复成“连接”，并且服务器软件也恢复到了监听状态。  
![在这里插入图片描述](.\source\bd5acc82270840e0b1fd9b1b1a8897a3.png)  
![在这里插入图片描述](.\source\72e8f98cc07547d79d4db0aaf1bb9770.png)

## 2、c#代码实现

```csharp
 public partial class Form1 : Form
    {
        TcpClient tcpClient;
        bool isConnected = false;
        byte[] receiveData = new byte[1024];
        public Form1()
        {
            InitializeComponent();
            InitialUi();
        }

        private void button_Connect_Click(object sender, EventArgs e)
        {
            try
            {

                if (button_Connect.Text == "连接")
                {
                    tcpClient = new TcpClient();
                    string ip = tbx_ip.Text.Trim();
                    int port = Convert.ToInt32(numericUpDown_Port.Value);
                    IPAddress iPAddress = IPAddress.Parse(ip);
                    tcpClient.Connect(ip, port);
                    MessageBox.Show("连接成功");
                    isConnected = true;
                    button_Connect.Text = "断开";
                    Task.Run(new Action (ReceiveData ));
                }
                else
                {
                    button_Connect.Text = "连接";
                    isConnected = false;
                    tcpClient.Close();
                }
                    
            }
            catch (Exception ex)
            {
                MessageBox.Show($"连接失败：{ex.Message }");
            }

        }

        /// <summary>
        /// 初始化界面
        /// </summary>
        private void InitialUi()
        {
            numericUpDown_Port.Maximum = int.MaxValue;
        }

        private void btn_Send_Click(object sender, EventArgs e)
        {
            if (isConnected)
            {
                NetworkStream networkStream = tcpClient.GetStream();
                string sendStr = tbx_Send.Text.Trim();
                byte[] sendBytes = Encoding.ASCII.GetBytes(sendStr);
                networkStream.Write(sendBytes, 0, sendBytes.Length);

            }
            else
            {
                MessageBox.Show("服务器未连接");
            }
        }

        /// <summary>
        /// 接收服务器的数据线程
        /// </summary>
        private void ReceiveData()
        {

            while (true )
            {
                try
                {
                  NetworkStream networkStream=  tcpClient.GetStream();
                   int readByteNums= networkStream.Read(receiveData,0,receiveData.Length );
                    byte[] readReadBytes = new byte[readByteNums];
                    Array.Copy(receiveData,0, readReadBytes,0, readByteNums);
                    UpdateReceiveData(readReadBytes);
                }
                catch (Exception ex)
                {

                }
                if (isConnected == false)
                {
                    break;
                }
            }

        }

        /// <summary>
        /// 更新接收到的数据到界面
        /// </summary>
        /// <param name="bytes"></param>
        private void UpdateReceiveData(byte[] bytes)
        {
            string str = Encoding.ASCII.GetString(bytes);
            this.Invoke(new Action (()=> {
                rtbx_Receive.AppendText(str);
            }));
            
        }
    }
```

