using DotNet.Utilities;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;


namespace GoogleImgCrawler
{

    class Program
    {

        public static Queue<string> contentQueue = new Queue<string>();
        static void Main(string[] args)
        {

            #region 存储线程

            Thread saveThread = new Thread(() =>
                {
                    while (true)
                    {
                        if (contentQueue.Count > 0)
                        {
                            File.AppendAllText("D://googleImg.txt", contentQueue.Dequeue());
                        }
                        else
                        {
                            Thread.Sleep(1000);
                        }
                    }

                });
            saveThread.Start();

            #endregion
            while (true)
            {               
                #region 测试1000值
                Console.WriteLine("请输入关键词：");
                String keyWord = Console.ReadLine().Trim();
                keyWord= keyWord.Replace(" ", "+").Replace(",", "+").Replace("、", "+");//谷歌搜索，搜索条件之间用"+"连接                
                #endregion
                if (!string.IsNullOrEmpty(keyWord.Trim()))
                {
                    //           
                    Thread t = new Thread(() =>
                    {
                        #region 抓取任务开始
                        String start = "";
                        String endMd5 = MD5Helper.MD5Helper.ComputeMd5String("今天天气好晴朗，又是刮风又是下雨");//因为这是一段矛盾的话，其MD5与返回值MD5一致的概率几乎为0
                        string taskKeyWord = keyWord;
                        //string fileName = Guid.NewGuid().ToString();
                        HttpHelper http = new HttpHelper();
                        //每一次抓取
                        //大于totalItems/48时停止
                        Int32 count = 0;
                        Random random = new Random();
                        while (true)
                        {
                            start = (100 * count).ToString();
                            HttpItem item = new HttpItem()
                            {
                                URL = count > 0 ? "https://www.google.com.hk/search?q=" + taskKeyWord + "&newwindow=1&safe=strict&biw=1920&bih=995&site=imghp&tbm=isch&ijn=" + count + "&ei=NxThVtLqNKbImAXJt5n4Aw&start=" + start + "&ved=0ahUKEwiS4oX2vrXLAhUmJKYKHclbBj8QuT0IGSgB&vet=10ahUKEwiS4oX2vrXLAhUmJKYKHclbBj8QuT0IGSgB.NxThVtLqNKbImAXJt5n4Aw.i" : "https://www.google.com.hk/search?newwindow=1&safe=strict&site=imghp&tbm=isch&source=hp&biw=1920&bih=995&q=" + taskKeyWord + "&gs_l=img.3..0j0i24l9.6815.13285.0.13701.29.19.4.0.0.0.416.2385.0j1j8j0j1.10.0....0...1ac.1j4.64.img..16.12.2001.DlZwjPhbRD0",
                                // URL = count > 0 ? "https://www.google.com.hk/search?q=" + taskKeyWord + "&newwindow=1&safe=strict&biw=800&bih=60&site=imghp&tbm=isch&ijn=" + count + "&ei=NxThVtLqNKbImAXJt5n4Aw&start=" + start + "&ved=0ahUKEwiS4oX2vrXLAhUmJKYKHclbBj8QuT0IGSgB&vet=10ahUKEwiS4oX2vrXLAhUmJKYKHclbBj8QuT0IGSgB.NxThVtLqNKbImAXJt5n4Aw.i" : "https://www.google.com.hk/search?newwindow=1&safe=strict&site=imghp&tbm=isch&source=hp&biw=800&bih=60&q=" + taskKeyWord + "&gs_l=img.3..0j0i24l9.6815.13285.0.13701.29.19.4.0.0.0.416.2385.0j1j8j0j1.10.0....0...1ac.1j4.64.img..16.12.2001.DlZwjPhbRD0",
                                Method = "get",//URL     可选项 默认为Get   
                                IsToLower = false,//得到的HTML代码是否转成小写     可选项默认转小写   
                                Cookie = "NID=77=H_zf8nY0Q2oZyFBHQUdNb1R7FfhduoF49EpiIhLNNmLYoQKbapT-hxWSwnvLaTtb3EqJYSuCvBCKric-5PWFtWCyKBHGQN3WTsNm9TKdL7O710vB4WDxjjrw-cQcHWdskh25tnoRdQ",//字符串Cookie     可选项   
                                Referer = "https://www.google.com.hk/",//来源URL     可选项   
                                Postdata = "",//Post数据     可选项GET时不需要写   
                                Timeout = 100000,//连接超时时间     可选项默认为100000    
                                ReadWriteTimeout = 30000,//写入Post数据超时时间     可选项默认为30000   
                                UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:18.0) Gecko/20100101 Firefox/18.0",//用户的浏览器类型，版本，操作系统     可选项有默认值   
                                ContentType = "text/html",//返回类型    可选项有默认值   
                                Allowautoredirect = false,//是否根据301跳转     可选项   
                                                          //CerPath = "d:\123.cer",//证书绝对路径     可选项不需要证书时可以不写这个参数   
                                                          //Connectionlimit = 1024,//最大连接数     可选项 默认为1024    
                                ProxyIp = "192.168.210.143:1080",//代理服务器ID     可选项 不需要代理 时可以不设置这三个参数    
                                                                 //ProxyPwd = "123456",//代理服务器密码     可选项    
                                                                 //ProxyUserName = "administrator",//代理服务器账户名     可选项   
                                ResultType = ResultType.String
                            };
                            HttpResult result = http.GetHtml(item);
                            string html = result.Html;
                            //string cookie = result.Cookie;
                            //解析结果，-----→                            

                            //List<ImgModel> list = count > 0 ? GetJsonPlus(html) : GetJsonRaw(html);//Kiwi：plus
                            List<ImgModel> list = GetJsonPlus(html);
                            Int32 num = 0;
                            foreach (ImgModel model in list)
                            {                                
                                num++;
                                StringBuilder content = new StringBuilder();
                                content.Append("[" + taskKeyWord + "]-");
                                content.Append((count + 1).ToString() + "-" + num.ToString() + "->");
                                content.Append("→图片URL:" + model.ou);//图片地址
                                content.Append("→图片网站URL:" + model.ru);//网站地址
                                content.Append("→图片Title:" + model.pt + "\r\n");
                                contentQueue.Enqueue(content.ToString());
                                // File.AppendAllText("D:/"+ fileName + ".txt", content.ToString());
                            }

                            //解析结果，end-----→              

                            //停止条件：
                            //全网搜索                           
                            //2.数量不到上限，小于700时，返回空字符串
                            //结果：MD5一样时，停止
                            //锁定网站搜索==》site:
                            //1.数量上限是900，大于900时==》返回空字符串
                            //2.数量不到上限，小于900，如何停止？
                            //结果：返回空字符串时，停止
                            if (String.IsNullOrEmpty(html))
                            {
                                Console.WriteLine("结束时间：【" + DateTime.Now + "】-任务关键字：" + taskKeyWord);
                                break;
                            }
                            if (MD5Helper.MD5Helper.ComputeMd5String(html) == endMd5)
                            {
                                Console.WriteLine("结束时间：【" + DateTime.Now + "】-任务关键字：" + taskKeyWord);
                                break;
                            }
                            //下一轮
                            int span = random.Next(3000, 10000);
                            Thread.Sleep(span);
                            //Console.WriteLine(count);
                            count++;
                            endMd5 = MD5Helper.MD5Helper.ComputeMd5String(html);
                        }
                        #endregion
                    });
                    //threads.Add(t);
                    t.Start();
                    Console.WriteLine("开始时间：【" + DateTime.Now + "】-任务关键字：" + keyWord);

                }
            }
        }        
        public static List<ImgModel> GetJsonPlus(string source)
        {
            List<ImgModel> list = new List<ImgModel>();

            Int32 indexStart = 0;
            Int32 indexEnd = 0;
            while (source.IndexOf("<!--m-->") > 0)
            {
                indexStart = source.IndexOf(">{", source.IndexOf("<!--m-->")) + 1;
                indexEnd = source.IndexOf("}</div>", indexStart);
                string subJson = source.Substring(indexStart, indexEnd - indexStart + 1);
                System.Web.Script.Serialization.JavaScriptSerializer js = new System.Web.Script.Serialization.JavaScriptSerializer();
                ImgModel model = js.Deserialize<ImgModel>(subJson);
                list.Add(model);
                source = source.Substring(indexEnd);
            }
            return list;
        }


    }



}
