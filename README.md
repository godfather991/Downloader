# Downloader
7.16 Winform & WPF 课程资料
项目见Code
## 作业
- 小试身手（4分）

  - 创建一个TextBlock，实现下载状态的显示，要求使用绑定实现。

  - 没有下载任务或下载被取消时显示“Free”；下载中显示“Downloading...”；下载完成后显示“Complete”。

  - 允许对设计要求做出合理的调整或改进，但需要阐述理由

- 略有难度（4+1分）

  1. 使用TextBlock，固定传输速率单位，实现下载速度的显示，要求使用绑定实现（4分）

     > 一种可行的思路：实例化一个Timer定时器，每过一段时间扫描一次下载进度（对应已下载Byte数），统计本次扫描到上次扫描的下载字节数，除以间隔时间即得下载速率

  2. 实现下载速度单位的自适应（1分）

     自动选择适合的下载速度计量单位，要求取值合理（即显示的数值避免过小或过大，例如一定得小于1024）

- 您是大佬（1分）

  1. 利用课上所学知识，对界面设计向符合自身审美的方向进行改进，颜值至上（0.5分）
  2. 服务器实战：给定地址，自动下载目录下所有文件（0.5分）
  
- 受我一拜！（0分）

  - 创建一个Button，并使其具有暂停下载的功能（实际上是断点续传）
  
## References
https://www.cnblogs.com/zh7791/category/1528742.html （来自博客园大佬的教程，建议深度学习orz）
