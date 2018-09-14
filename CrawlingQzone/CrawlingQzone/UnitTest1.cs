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
                //�л��﷨������,һ���Ǹ��������л�,����һ�ָ���iframe�����л�
                //��������ʹ��name�л�
                ITargetLocator tagetLocator = driver.SwitchTo();
                //tagetLocator.Frame(1);  //frame index.
                tagetLocator.Frame("login_frame");  //frame frame name.
                var switchLogin = driver.FindElementByCssSelector("#switcher_plogin");
                switchLogin.Click();
                var userName = driver.FindElementByXPath("//*[@id='u']");
                //�����userName�����û������ı���
                //�����û�����ֵ
                userName.SendKeys("123456");
                var pwd = driver.FindElementByXPath("//*[@id='p']");
                pwd.SendKeys("********");
                var btnLogin = driver.FindElementByXPath("//*[@id='login_button']");
                //�������жϵ�¼��ť�Ƿ�ɼ�,���Բ�д,ֱ�ӵ���click����
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
                //ҳ����ת,�л���˵˵ҳ���˵˵�б�Iframe
                ITargetLocator t = driver.SwitchTo();
                //�������ǻ�һ�ֻ�ȡԪ�ط���,ֱ��ʹ��css��ȡ
                //xpath //*[@id="app_container"]/iframe
                //css #app_container > iframe
                IWebElement frame = driver.FindElementByCssSelector("#app_container > iframe");
                t.Frame(frame);
                //����˵˵����
                var dataAll = new List<MessageInfo>();
                //��һҳ��ʼ,pageIndex Ĭ��0
                //���庺�ַ���Ϊ��ֱ����������,��Ҫ������Щϸ��
                ˵˵���ݻ�ȡ(driver, dataAll, 0);
            }
            finally
            {
                driver.Quit();
            }

        }

        private static void ˵˵���ݻ�ȡ(ChromeDriver driver, List<MessageInfo> dataAll, int pageIndex)
        {
            //��ҳ֮������3s��ֹ����û�м�����ɳ����Ҳ���Ԫ���쳣
            System.Threading.Thread.Sleep(3000);
            var msgListDom = driver.FindElementsByXPath("//*[@id='msgList']/li");
            //����������ѭ��ȡ����

            //��Ϊ����Ԫ����Ҫ�õ�����ֵ,�����������forѭ����ʽ
            for (var i = 1; i < msgListDom.Count + 1; i++)
            {
                var data = new MessageInfo();
                //�������ǲ���Ԫ��
                //��һ��
                //*[@id="msgList"]/li[1]/div[3]/div[2]/a ���Ի�ȡԪ�ؿ�������д
                var qqDom = driver.FindElementByXPath("//*[@id='msgList']/li[" + i + "]/div[3]/div[2]/a"); //*[@id="msgList"]/li[1]/div[3]/div[2]/a.content �ǳ� data-uin QQ���� 
                if (qqDom != null)
                {
                    //��ȡԪ������  data-uin��ӦQQ����
                    data.QQ = qqDom.GetAttribute("data-uin");
                }
                var preDom = driver.FindElementByXPath("//*[@id='msgList']/li[" + i + "]/div[3]/div[2]/pre"); //*[@id="msgList"]/li[1]/div[3]/div[2]/a.content �ǳ� data-uin QQ���� 
                if (preDom != null)
                {
                    //��ȡԪ��Text�ı�ֵ
                    data.Content = preDom.Text;
                }
                //��ȡʱ��ֵ,����ʹ��css��ʽ��ȡ,��ȻҲ����ʹ��xpath������������,����Ԫ�ط�����һƪ������
                //���ǻ�ȡ���Ǻ�ʱ���ͺ�ͬһ����Ԫ��,�еİ����ͺ�,�еĲ�����
                var times = driver.FindElementsByCssSelector("#msgList > li:nth-child(" + i + ") > div.box.bgr3 > div.ft > div.info > span");
                //һ����ʱ��,һ���ǻ���
                //2015��2��10�� ����iPhone
                if (times.Count > 1)
                {
                    var time = driver.FindElementByCssSelector("#msgList > li:nth-child(" + i + ") > div.box.bgr3 > div.ft > div.info > span:nth-child(1) > a");
                    data.MessageTime = Convert.ToDateTime(time.GetAttribute("title"));
                    //��ȡ�ֻ��ͺ�
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
                #region ͼƬ��Ƶ����
                //�������Ƶ���ȡ��Ƶ,�����ȡͼƬ
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
            //��һҳ
            //����ץȡ���֮������һҳ
            driver.FindElementByXPath("//*[@id='pager_next_" + pageIndex + "']").Click();
            pageIndex++;
            ˵˵���ݻ�ȡ(driver, dataAll, pageIndex);
            //dataAll ���ݴ�����ڴ�֮��,�������Ҫ�Ŀ��Ա��浽���ݿ�,����Ͳ���ϸ������
        }
    }
    public class MessageInfo
    {
        /// <summary>
        /// ����
        /// </summary>
        public string QQ { get; set; }
        /// <summary>
        /// ��������
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// ����ʱ��
        /// </summary>
        public DateTime? MessageTime { get; set; }
        /// <summary>
        /// ˵˵ͼƬ�б�
        /// </summary>
        public List<string> ImageList { get; set; }
        /// <summary>
        /// �ֻ��ͺ�
        /// </summary>
        public string MobileType { get; set; }
    }
}
