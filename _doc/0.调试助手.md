# 手把手教你从入门到精通C# Socket通信

![在这里插入图片描述](.\source\e164fd67832b4de68a180b2924eb3955.png)

## 前言：

Socket通信（包含Tcp/Udp通信）在工业领域用途非常广泛，作者在自动化领域耕耘多年，做过的Tcp/Udp通信的项目大大小小也有几百个，公司项目+兼职项目，可以说只要是Tcp/Udp的项目，没有我做不了的，毕竟让我徒手撸一个市面上你见到的Tcp/Udp调试助手对我而言也不在话下，比如上图你看到的TCP/UDP1.0调试助手就是我写的。古人云读万卷书不如行万里路，行万里路不如名师指路，所以入门最快的方法一定是找个前辈模仿他，年轻的时候一定要投资自己！买书、买课程学习都是你提升自己最快的方法，成为高手别无他法，努力学习+模仿高手，他日一定有所成就！一本书、一个课程几十块钱，但是在未来为你赚到的钱一定是这些成本的几十倍、几百倍、上千倍，甚至更多！如果你是一个普通人，没有强大的背景，一定要持续学习，持续学习是普通人逆袭唯一的方法！如果想精通TCP/UDP编程，C#基础的语法知识必不可少，订阅我的专栏[《 C#Socket通信从入门到精通 》](https://blog.csdn.net/qq_34059233/category_12416831.html)就送《C#本质论》、《C#图解教程》电子书。其次是Tcp客户端编程、Tcp服务器编程、Udp客户端编程、Udp服务器编程、Udp广播编程、Udp多播编程，这些知识我都会在我开设的专栏[《 C#Socket通信从入门到精通 》](https://blog.csdn.net/qq_34059233/category_12416831.html)中进行详细讲解，总之，只要跟我学了Tcp/Udp通信，一定能打变Tcp/Udp无敌手，由于Tcp/Udp使用的特别多，尤其是工业自动化领域和仪器的通信，比如和仪器、PLC、机械手的通信、文件的传输等，我先写个Tcp/Udp通信的调试工具软件给大伙尝尝鲜，帮助大家更好的理解Tcp/Udp，当然能写一个Tcp/Udp通信助手真的不算什么，在实际项目中，还有更复杂的功能，比如文件的传输等，当然这些知识我都会在我的专栏[《 C#Socket通信从入门到精通 》](https://blog.csdn.net/qq_34059233/category_12416831.html)中进行详细介绍，只有系统掌握了各种Tcp/Udp通信的知识以及应用，才算是一个高手，不然怎么轻松拿30万+年薪。  
**说了那么多，大家肯定好奇，我花了钱学你这个专栏我到底能获得什么？总结下：  
1、掌握TCP客户端编程；  
2、掌握TCP服务器编程；  
3、掌握UDP客户端编程；  
4、掌握UDP服务器编程；  
5、掌握UDP组播编程；  
6、掌握任意数量的客户端、服务器编程；  
7、掌握文件传输；  
8、掌握项目实战编程方法**  
**学完本专栏，以后遇到的任何关于TCP、UDP编程的问题都不是问题！什么ModbusTCP、与PLC的通信、与各种仪器的通信都是手到擒来！雷军曾说你会发现你生命中遇到的问题百分之九十的问题别人都遇到过，你需要做的就是找个人问一下，我就是那个你需要问的人。**

## 一、[《 C#Socket通信从入门到精通 》](https://blog.csdn.net/qq_34059233/category_12416831.html)专栏文章目录

点击下面的文章链接即可转到该文章：  
1、[C# Socket通信从入门到精通（1）——单个同步TCP客户端C#代码实现](https://blog.csdn.net/qq_34059233/article/details/133935166)  
2、[C# Socket通信从入门到精通（2）——多个同步TCP客户端C#代码实现](https://blog.csdn.net/qq_34059233/article/details/133969687)  
3、[C# Socket通信从入门到精通（3）——单个异步TCP客户端C#代码实现](https://blog.csdn.net/qq_34059233/article/details/133999615)  
4、[C# Socket通信从入门到精通（4）——多个异步TCP客户端C#代码实现](https://blog.csdn.net/qq_34059233/article/details/134085021)  
5、[C# Socket通信从入门到精通（5）——单个同步TCP服务器监听一个客户端C#代码实现](https://blog.csdn.net/qq_34059233/article/details/134101129)  
6、[C# Socket通信从入门到精通（6）——单个同步TCP服务器监听多个客户端C#代码实现](https://blog.csdn.net/qq_34059233/article/details/134108466)  
7、[C# Socket通信从入门到精通（7）——单个异步TCP服务器监听单个客户端C#代码实现](https://blog.csdn.net/qq_34059233/article/details/134341658)  
8、[C# Socket通信从入门到精通（8）——单个异步TCP服务器监听多个客户端C#代码实现](https://blog.csdn.net/qq_34059233/article/details/134365951)  
9、[C# Socket通信从入门到精通（9）——如何设置本机Ip地址](https://blog.csdn.net/qq_34059233/article/details/134426570)  
10、[C# Socket通信从入门到精通（10）——如何检测两台电脑之间的网络是否通畅](https://blog.csdn.net/qq_34059233/article/details/134426951)  
11、[C# Socket通信从入门到精通（11）——单个同步UDP客户端C#代码实现](https://blog.csdn.net/qq_34059233/article/details/134684132)  
12、[C# Socket通信从入门到精通（12）——多个同步UDP客户端C#代码实现](https://blog.csdn.net/qq_34059233/article/details/134688111)  
13、[C# Socket通信从入门到精通（13）——单个异步UDP客户端C#代码实现](https://blog.csdn.net/qq_34059233/article/details/134958493)  
14、[C# Socket通信从入门到精通（14）——多个异步UDP客户端C#代码实现](https://blog.csdn.net/qq_34059233/article/details/134960261)  
15、[C# Socket通信从入门到精通（15）——单个同步UDP服务器监听一个客户端C#代码实现](https://blog.csdn.net/qq_34059233/article/details/135022430)  
16、[C# Socket通信从入门到精通（16）——单个同步UDP服务器监听多个客户端C#代码实现](https://blog.csdn.net/qq_34059233/article/details/135736002)  
17、[C# Socket通信从入门到精通（17）——单个异步UDP服务器监听一个客户端C#代码实现](https://blog.csdn.net/qq_34059233/article/details/135736665)  
18、[C# Socket通信从入门到精通（18）——单个异步UDP服务器监听多个客户端C#代码实现](https://blog.csdn.net/qq_34059233/article/details/135892480)  
19、[C# Socket通信从入门到精通（19）——UDP广播C#代码实现](https://blog.csdn.net/qq_34059233/article/details/133842703)  
20、[C# Socket通信从入门到精通（20）——UDP组播（多播）C#代码实现](https://blog.csdn.net/qq_34059233/article/details/133847436)  
21、[C# Socket通信从入门到精通（21）——TCP发送文件与接收文件 C#代码实现](https://blog.csdn.net/qq_34059233/article/details/136414863)

## 二、TCP/UDP调试助手1.0介绍

TCP/UDP调试助手1.0是我开发的一个TCP/UDP助手，主要帮助学习[《 C#Socket通信从入门到精通 》](https://blog.csdn.net/qq_34059233/category_12416831.html)专栏的学员更好的理解socket通信，当然我的专栏的内容是远远多余这个通信助手的，TCP/UDP调试助手1.0是锦上添花，TCP/UDP调试助手1.0的功能如下：

### 2.1 TCP Server测试

鼠标点击“TCP Server”  
点击“创建”  
输入需要监控的Tcp“端口”，比如50  
点击确定，如下图：  
![在这里插入图片描述](.\source\69b6899b27d34cb28f2c4633dd5036ec.png)

![在这里插入图片描述](.\source\f6b1d37de38a467abadb7dae9f3592bf.png)  
![在这里插入图片描述](.\source\e6859e8425b343bf8ac8af755aa11848.png)  
![在这里插入图片描述](.\source\d4e54a8630d041c49fc872996acea68e.png)  
经过以上操作以后，我们就开启了一个Tcp服务器这个服务器监听的端口号是50，只要我们使用TCP Client去连接这个端口为50的服务器即可连接服务器，关于如何连接TCP服务器，在2.2节 TCP Client测试中我将讲解。

### 2.2 TCP Client测试

#### 2.2.1 连接TCP服务器

我们再次打开一个TCP/UDP调试助手1.0，这样电脑上就有两个TCP/UDP调试助手1.0被运行，左边是TCP Server、右边是TCP Client如下：  
![在这里插入图片描述](.\source\98508e68800345fc80fd32e3545cc11a.png)  
我们在新打开的TCP/UDP调试助手1.0上创建TcpClient也就是TCP客户端，操作如下：  
鼠标选择“TCP Client”  
点击“创建”  
输入“IP”、“端口”，这里的Ip和端口都是服务器的IP和端口。  
点击“确定”  
点击“连接”  
经过以上操作，客户端就能连接上服务器，连上以后就能发送数据给服务器、从服务器接收数据。

![在这里插入图片描述](.\source\606eda7d2115448a8222717c2473e18b.png)  
![在这里插入图片描述](.\source\25348528a51c4e59856699868c3014f1.png)  
![在这里插入图片描述](.\source\ad9fed9cd7124fbab84649c21a5ce618.png)  
![在这里插入图片描述](.\source\32d3f2923d6346fe81cf37c8490268ec.png)

#### 2.2.2 发送数据到TCP服务器

在TCP Client所在的“数据发送区”输入“111”，然后点击“发送”，就能在TCP Server所在的“数据接收区”看到接收的数据，说明服务器成功接收到了客户端发送的数据。

![在这里插入图片描述](.\source\37e41793435744a694ae0b95cf8db0dc.png)  
![在这里插入图片描述](.\source\3885e6f9fa6c46b0b9d9dfc126cbc661.png)

#### 2.2.3 从TCP服务器接收数据

要实现从TCP服务器接收数据，就要使用TCP服务器发送数据给客户端，如下图：  
在服务器的“数据发送区”输入“i am server”，然后点击“发送”，可以看到TCP Client的“数据接收区”收到了“i am server”。  
![在这里插入图片描述](.\source\9e2075a4d6bd4713a5f56ba9fa66f5c3.png)

![在这里插入图片描述](.\source\401b436661534372972f0246f84c13e5.png)

### 2.3 UDP Server测试

鼠标点击“UDP Server”  
点击“创建”  
输入需要监控的UDP“端口”，比如60  
点击确定，如下图：  
![在这里插入图片描述](.\source\4e7d3e339166451b880d0b507bc41558.png)  
![在这里插入图片描述](.\source\ae31ed4af18941e2aa79be0d8fb01566.png)  
![在这里插入图片描述](.\source\8f3afe5cba6244c788628473325c2701.png)  
经过以上操作以后，我们就开启了一个UDP服务器这个服务器监听的端口号是60，这样我们使用UDP Client去和这个服务器进行数据的收发操作，关于如何与UDP服务器进行交互，我将在2.4节 UDP Client测试中讲解。

### 2.4 UDP Client测试

我们再次打开一个TCP/UDP调试助手1.0，这样电脑上就有两个TCP/UDP调试助手1.0被运行，左边是UDPServer、右边是UDP Client如下：  
![在这里插入图片描述](.\source\42e17856b37a4a18a644e0f9bff5123e.png)  
我们在新打开的TCP/UDP调试助手1.0上创建UdpClient也就是UDP客户端，操作如下：  
鼠标选择“UDP Client”  
点击“创建”  
输入“对方IP”、“对方端口”、“本地端口”，这里的Ip和端口都是服务器的IP和端口。  
点击“确定”  
经过以上操作，客户端就能连接上服务器，连上以后就能发送数据给服务器、从服务器接收数据。  
![在这里插入图片描述](.\source\459feaa0bb564b168888e151e1f098ba.png)  
![在这里插入图片描述](.\source\da2e95f47c4e46cfaf48974773609c39.png)  
![在这里插入图片描述](.\source\69e65db116c54c6d92fe200fc6129be1.png)

#### 2.4.1 发送数据到udp服务器

在UDP Client所在的“数据发送区”输入“ttt”，然后点击“发送”，就能在UDP Server所在的“数据接收区”看到接收的数据，说明服务器成功接收到了客户端发送的数据。  
![在这里插入图片描述](.\source\53c946c39694460d837a50b5fbe30ef0.png)![在这里插入图片描述](.\source\31b3da099f79441db0454b6f16f4ce92.png)

#### 2.4.2 从udp服务器接收数据

在UDP Server所在的“数据发送区”输入“rr”，然后点击“发送”，就能在UDP Client所在的“数据接收区”看到接收的数据，说明客户端成功接收到了服务器发送的数据。  
![在这里插入图片描述](.\source\b3dbb1df26224cfe97e99dca95ee5ce2.png)  
![在这里插入图片描述](.\source\206efdf72cc645b09e3a9f2b8dbb2d32.png)

### 2.5 UDP Group测试

首先打开TCP/UDP调试助手1.0，然后加入UDP多播组，组播Ip为224.0.0.3，端口为65000；  
然后在新打开的TCP/UDP调试助手1.0上创建UdpClient也就是UDP客户端，操作如下：  
鼠标选择“UDP Client”  
点击“创建”  
输入“对方IP”、“对方端口”、“本地端口”，这里的Ip和端口都是服务器的IP和端口。  
点击“确定”。  
最后在数据发送区输入“rrr”，然后发送，就会发现UDP组播地址能收到客户端收到的数据。  
![在这里插入图片描述](.\source\ad8fa0decc7a413e99a7ca706b64e107.png)

![在这里插入图片描述](.\source\58f51e32b9824e09b452c7c4ca116178.png)

![在这里插入图片描述](.\source\8db1c0bb1601440ea69f5a75c74d13b6.png)

![在这里插入图片描述](.\source\a7eaa251dc6d4ac494b1a1c34d460fb5.png)

![在这里插入图片描述](.\source\4eeaacab1cf34f7eaec87856e8703b61.png)  
![在这里插入图片描述](.\source\60f7a255da744dbda44d3828c8fa6cf9.png)

## 三、UDP广播

电脑1的Ip为192.168.0.3  
电脑2的Ip为192.168.0.1  
电脑3的Ip为192.168.0.2  
此时三台电脑同属于192.168.0网段，然后在电脑1的Ip为192.168.0.3开启一个UDP客户端，如下：  
![在这里插入图片描述](.\source\b41bd1d764de46ef80104db27a2f3b9f.png)  
![在这里插入图片描述](.\source\725b1ed73d4449be8dc1af8872ba5eb2.png)

这里要注意如果想实现广播的效果，那么发送的IP一定是255网段（IP最后一位是255），点击发送以后电脑2和电脑3创建的Udp服务器都能收到电脑1发送的数据，电脑2和电脑3的创建的UDP服务器如下（服务器监听的端口号是100）：  
![在这里插入图片描述](.\source\e1d9f2216e0d440d9353875f252f459e.png)  
![在这里插入图片描述](.\source\8eeade027cc34c22963ae7165238bc86.png)

## 四、源码下载

### 4.1 如何获取TCP/UDP调试助手1.0版本的源码

本文介绍的TCP/UDP调试助手1.0版本的源码订阅专栏[《 C#Socket通信从入门到精通 》](https://blog.csdn.net/qq_34059233/category_12416831.html)后，私信联系本人或者加入文章最下面的群可获取（提供博客名）

