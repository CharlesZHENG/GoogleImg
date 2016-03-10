using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GoogleImgCrawler
{
    public class ImgModel
    {
        public string id { get; set; }//id值
        public string isu { get; set; }//来源网址域名
        public string ity { get; set; }//图片格式
        public int oh { get; set; }//图片高度
        public string ou { get; set; }//图片URL
        public int ow { get; set; }//图片宽度
        public string pt { get; set; }//图片描述
        public string rid { get; set; }//未知
        public string ru { get; set; }//来源网址
        public string s { get; set; }//不可靠描述，不建议使用
        public int sc { get; set; }//未知
        public int th { get; set; }//未知
        public string tu { get; set; }//缩略图URL
        public int tw { get; set; }//未知
    }
}
