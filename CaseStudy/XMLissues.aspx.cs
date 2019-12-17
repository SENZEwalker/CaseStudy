using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

namespace CaseStudy
{
    public partial class XMLissues : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        //XmlDocument物件的版本
        public void XMLissue()
        {
            string pathName="";
            XmlDocument xmlDoc = new XmlDocument(); //XML物件
            xmlDoc.Load(pathName); //Load方法，可以放WS位址 或 檔案位址
            XmlNamespaceManager nameSpace = new XmlNamespaceManager(xmlDoc.NameTable); //nameSpace物件，用來定義XML文件中的nameSpace
            nameSpace.AddNamespace("wsdl", "http://schemas.xmlsoap.org/wsdl/"); //WSDL是通用格式，網址應也都會一樣
            XmlElement root = xmlDoc.CreateElement("wsdl", "definitions"); //創建元素節點，第一個創進去得會變成根。 此方法有多型，兩個參數表是同時指定nameSpace
            xmlDoc.AppendChild(root); //要Append才會正式進去! 不然你只會得到空文件
            XmlNode mainLevel = xmlDoc.SelectSingleNode("/wsdl:definitions", nameSpace); //篩選單一節點，若無nameSpace就不用
            XmlNodeList subNode = xmlDoc.SelectNodes("/wsdl:definitions/wsdl:message", nameSpace); //篩選該節點下符合名稱的所有節點

            //跨文件塞節點的情境
            XmlDocument xmlDocNew = new XmlDocument();
            xmlDoc.Load(pathName);
            XmlElement root2 = xmlDocNew.CreateElement("Root");
            xmlDocNew.AppendChild(root2);
            XmlNode myNode = xmlDoc.SelectSingleNode("//NodeName");
            XmlNode tempNode = xmlDocNew.ImportNode(myNode, true); //true為深層置入，連帶所有詳細資訊以及子節點全部搬
            xmlDocNew.DocumentElement.AppendChild(tempNode); //一樣記得Append

            mainLevel.RemoveChild(subNode[0]); //針對某節點刪除其下的特定節點

            //儲存成Stream型別的方法
            MemoryStream xmlStream = new MemoryStream();
            xmlDoc.Save(xmlStream);
            xmlStream.Flush();
            xmlStream.Position = 0;

        }

        /// 留存一下還帶有追蹤用變數的版本
        /// <summary>
        /// 移除WSDL文件中message,operation下重複名稱結點，以Stream型別回傳整理後的XML，若有問題回傳NULL
        /// </summary>
        ///<param name="pathName">檔名路徑或WS路徑</param>
        public Stream RemoveDuplicateNodesinXML(string pathName)
        {
            //string resultPath="";
            try
            {
                List<string> listTitle = new List<string>();
                List<string> listRest = new List<string>();
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(pathName);
                XmlNamespaceManager nameSpace = new XmlNamespaceManager(xmlDoc.NameTable);
                nameSpace.AddNamespace("wsdl", "http://schemas.xmlsoap.org/wsdl/");

                XmlDocument xmlDocNew = new XmlDocument();
                XmlElement root = xmlDocNew.CreateElement("wsdl", "definitions");
                xmlDocNew.AppendChild(root);

                //清除<definitions>下重複name的 <message>
                XmlNode mainLevel = xmlDoc.SelectSingleNode("/wsdl:definitions", nameSpace);
                XmlNodeList subNode = xmlDoc.SelectNodes("/wsdl:definitions/wsdl:message", nameSpace);
                foreach (XmlNode m_Node in subNode)
                {
                    string innerText = m_Node.OuterXml.Split('"')[1];

                    if (false == listTitle.Contains(innerText))
                    {
                        listTitle.Add(innerText);
                        XmlNode tempNode = xmlDocNew.ImportNode(m_Node, true);
                        xmlDocNew.DocumentElement.AppendChild(tempNode);
                    }
                    else
                    {
                        mainLevel.RemoveChild(m_Node);
                        listRest.Add(innerText);
                    }
                }

                //清除<definitions/portType>下重複name的 <operation>
                listTitle.Clear();
                listRest.Clear();
                mainLevel = xmlDoc.SelectSingleNode("/wsdl:definitions/wsdl:portType", nameSpace);
                subNode = xmlDoc.SelectNodes("/wsdl:definitions/wsdl:portType/wsdl:operation", nameSpace);
                foreach (XmlNode m_Node in subNode)
                {
                    string innerText = m_Node.OuterXml.Split('"')[1];

                    if (false == listTitle.Contains(innerText))
                    {
                        listTitle.Add(innerText);
                        XmlNode tempNode = xmlDocNew.ImportNode(m_Node, true);
                        xmlDocNew.DocumentElement.AppendChild(tempNode);
                    }
                    else
                    {
                        mainLevel.RemoveChild(m_Node);
                        listRest.Add(innerText);
                    }
                }
                //resultPath = pathName.Replace(".xml","") + "_cleared.xml";                
                //xmlDocNew.Save(resultPath);
                MemoryStream xmlStream = new MemoryStream();
                xmlDoc.Save(xmlStream);
                xmlStream.Flush();
                xmlStream.Position = 0;
                return xmlStream;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}