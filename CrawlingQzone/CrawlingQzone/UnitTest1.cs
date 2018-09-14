using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace CrawlingQzone
{
    public class UnitTest1
    {
        [Fact]
        public void QQLogin()
        {
            dynamic type = (new UnitTest1()).GetType();
            string currentDirectory = Path.GetDirectoryName(type.Assembly.Location);
            var driver = new ChromeDriver(currentDirectory);
            driver.Url = "https://qzone.qq.com/";
            try
            {
                //切换语法有两种,一种是根据索引切换,另外一种根据iframe名称切换
                //这里我们使用name切换
                ITargetLocator tagetLocator = driver.SwitchTo();
                //tagetLocator.Frame(1);  //frame index.
                tagetLocator.Frame("login_frame");  //frame frame name.
                var switchLogin = driver.FindElementByCssSelector("#switcher_plogin");
                switchLogin.Click();
                var userName = driver.FindElementByXPath("//*[@id='u']");
                //这里的userName就是用户名的文本框
                //设置用户名的值
                userName.SendKeys("123456");
                var pwd = driver.FindElementByXPath("//*[@id='p']");
                pwd.SendKeys("********");
                var btnLogin = driver.FindElementByXPath("//*[@id='login_button']");
                //这里是判断登录按钮是否可见,可以不写,直接调用click方法
                if (btnLogin != null && btnLogin.Displayed == true)
                {
                    btnLogin.Click();
                }
                System.Threading.Thread.Sleep(3000);
                //*[@id="menuContainer"]/div/ul/li[5]/a
                var msgDom = driver.FindElementByXPath("//*[@id='menuContainer']/div/ul/li[5]/a");
                if (msgDom != null && msgDom.Displayed == true)
                {
                    msgDom.Click();
                }
                //页面跳转,切换到说说页面的说说列表Iframe
                ITargetLocator t = driver.SwitchTo();
                //这里我们换一种获取元素方法,直接使用css获取
                //xpath //*[@id="app_container"]/iframe
                //css #app_container > iframe
                IWebElement frame = driver.FindElementByCssSelector("#app_container > iframe");
                t.Frame(frame);
                //定义说说集合
                var dataAll = new List<MessageInfo>();
                //第一页开始,pageIndex 默认0
                //定义汉字方法为了直观描述功能,不要在意这些细节
                说说内容获取(driver, dataAll, 0);
            }
            finally
            {
                driver.Quit();
            }

        }

        private static void 说说内容获取(ChromeDriver driver, List<MessageInfo> dataAll, int pageIndex)
        {
            //翻页之后休眠3s防止数据没有加载完成出现找不到元素异常
            System.Threading.Thread.Sleep(3000);
            var msgListDom = driver.FindElementsByXPath("//*[@id='msgList']/li");
            //接下来就是循环取数了

            //因为查找元素需要用到索引值,所以这里就用for循环方式
            for (var i = 1; i < msgListDom.Count + 1; i++)
            {
                var data = new MessageInfo();
                //接下来是查找元素
                //第一个
                //*[@id="msgList"]/li[1]/div[3]/div[2]/a 所以获取元素可以这样写
                var qqDom = driver.FindElementByXPath("//*[@id='msgList']/li[" + i + "]/div[3]/div[2]/a"); //*[@id="msgList"]/li[1]/div[3]/div[2]/a.content 昵称 data-uin QQ号码 
                if (qqDom != null)
                {
                    //获取元素属性  data-uin对应QQ号码
                    data.QQ = qqDom.GetAttribute("data-uin");
                }
                var preDom = driver.FindElementByXPath("//*[@id='msgList']/li[" + i + "]/div[3]/div[2]/pre"); //*[@id="msgList"]/li[1]/div[3]/div[2]/a.content 昵称 data-uin QQ号码 
                if (preDom != null)
                {
                    //获取元素Text文本值
                    data.Content = preDom.Text;
                }
                //获取时间值,我们使用css方式获取,当然也可以使用xpath或者其他方法,查找元素方法第一篇讲过了
                //这是获取的是和时间型号同一级的元素,有的包含型号,有的不包含
                var times = driver.FindElementsByCssSelector("#msgList > li:nth-child(" + i + ") > div.box.bgr3 > div.ft > div.info > span");
                //一个是时间,一个是机型
                //2015年2月10日 来自iPhone
                if (times.Count > 1)
                {
                    var time = driver.FindElementByCssSelector("#msgList > li:nth-child(" + i + ") > div.box.bgr3 > div.ft > div.info > span:nth-child(1) > a");
                    data.MessageTime = Convert.ToDateTime(time.GetAttribute("title"));
                    //获取手机型号
                    var mobileDom = driver.FindElementsByCssSelector("#msgList > li:nth-child(" + i + ") > div.box.bgr3 > div.ft > div.info > span:nth-child(2) > span");
                    if (mobileDom.Count > 0)
                    {
                        var mobile = driver.FindElementByCssSelector("#msgList > li:nth-child(" + i + ") > div.box.bgr3 > div.ft > div.info > span:nth-child(2) > span");
                        data.MobileType = mobile.Text;
                    }
                }
                else
                {
                    var time = driver.FindElementByCssSelector("#msgList > li:nth-child(" + i + ") > div.box.bgr3 > div.ft > div.info > span > a");
                    data.MessageTime = Convert.ToDateTime(time.GetAttribute("title"));
                }
                #region 图片视频区域
                //如果有视频则获取视频,否则获取图片
                var videos = driver.FindElementsByCssSelector("#msgList > li:nth-child(" + i + ") > div.box.bgr3 > div.md > div.video");
                if (videos.Count > 0) { }
                else
                {
                    var imageList = new List<string>();
                    var images = driver.FindElementsByXPath("//*[@id='msgList']/li[" + i + "]/div[3]/div[3]/div[1]/div/a");
                    if (images != null && images.Count > 0)
                    {
                        //*[@id="msgList"]/li[1]/div[3]/div[3]/div[1]/div/a[2]
                        for (int m = 0; m < images.Count; m++)
                        {
                            var imageDom = driver.FindElementByXPath("//*[@id='msgList']/li[" + i + "]/div[3]/div[3]/div[1]/div/a[" + (m + 1) + "]");
                            if (imageDom != null)
                            {
                                imageList.Add(imageDom.GetAttribute("href"));
                            }
                        }
                    }
                    data.ImageList = imageList;
                }
                #endregion
                dataAll.Add(data);
            }
            //下一页
            //数据抓取完成之后点击下一页
            driver.FindElementByXPath("//*[@id='pager_next_" + pageIndex + "']").Click();
            pageIndex++;
            说说内容获取(driver, dataAll, pageIndex);
            //dataAll 数据存放于内存之中,如果有需要的可以保存到数据库,这里就不详细介绍了
        }
    }
    public class MessageInfo
    {
        /// <summary>
        /// 号码
        /// </summary>
        public string QQ { get; set; }
        /// <summary>
        /// 发布内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 发布时间
        /// </summary>
        public DateTime? MessageTime { get; set; }
        /// <summary>
        /// 说说图片列表
        /// </summary>
        public List<string> ImageList { get; set; }
        /// <summary>
        /// 手机型号
        /// </summary>
        public string MobileType { get; set; }
    }
}
