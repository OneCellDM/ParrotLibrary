using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ParrotParserConsole
{

    internal class Program
    {
        private static void Main(string[] args)
        {
           
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            List<(Dictionary<string, string>, Dictionary<string, string>)> Res = new List<(Dictionary<string, string>, Dictionary<string, string>)>();
            foreach (var p in GetParrotsUrls())
            {
                try
                {
                    
                    Task.Run(() =>
                    {
                        //var автоматически определит тип который вернет функция
                        //var a=124 равносильно int a=124

                        var pg = PageDataParser("https://ru.wikipedia.org" + p);// промежуточные данные

                        var gs = GenJson(pg); //функция вернет данные типа string(строка)

                        //создаем экземпляр файла равносильно File *fp=fopen("");
                        FileInfo f = new FileInfo(pg.Item1["Title"] + ".json");

                        //создаем поток для записи перед этим создав файл при помощи функции create
                        StreamWriter streamWriter = new StreamWriter(f.Create());
                        //записываем данные
                        streamWriter.Write(gs);
                        //закрываем поток соотвественно и файл
                        streamWriter.Close();
                    });      
                }
                catch(Exception EX) {
                    Console.WriteLine(EX.Message);
                    
                }
            }
                    
            Console.Read();
        }

        private static string GenJson((Dictionary<string, string>, Dictionary<string, string>, Dictionary<string, string>) Data)
        {
            ArticleDataModel articleDataModel = new ArticleDataModel();
            articleDataModel.Data = GenDocument(Data);
            articleDataModel.Gender = Data.Item1["Gender"];
            articleDataModel.Title = Data.Item1["Title"];
            string output = JsonConvert.SerializeObject(articleDataModel);
            return output;
        }

        private static string GenDocument((Dictionary<string, string>, Dictionary<string, string>, Dictionary<string, string>) Data)
        {
            string Ds = @"<FlowDocument xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>

                       <Section>
                         <Paragraph>
                           <Expander Header='Информация о роде: "+Data.Item1["Gender"]+@"' FontWeight = 'ExtraBold' Foreground = '#336048' FontSize = '20'>
                            <StackPanel Margin='10'>";
            for (int i = 0; i < Data.Item3.Keys.Count; i++)
            {
                Ds += @"
                <TextBlock FontWeight = 'ExtraBold' FontSize = '18' Foreground = '#336048'>" +
                     Data.Item3.ElementAt(i).Key + @"
                 </TextBlock>

                  <TextBlock TextWrapping='Wrap'  FontWeight = 'Normal' FontSize = '16' Foreground = 'Black' TextAlignment = 'Justify' Margin = '0,10,20,10'>" +
                    Data.Item3.ElementAt(i).Value + @"
                     </TextBlock>";
            };
             Ds+= @"
                    </StackPanel>
                    </Expander>
                   
                    </Paragraph>
                    <Paragraph TextAlignment = 'Center' FontWeight = 'ExtraBold' FontSize = '25' Foreground = '#336048'> 
                        Информация о папугае
                  </Paragraph>
                   </Section>
                <Section>";
            for (int i = 0; i < Data.Item2.Keys.Count; i++)
            {
                Ds += @"
             <Paragraph FontWeight = 'ExtraBold' FontSize = '25' Foreground = '#336048'>" +
                     Data.Item2.ElementAt(i).Key + @"
                  </Paragraph>

                  <Paragraph TextAlignment = 'Justify' Margin = '20,0,20,10'>" +
                   Data.Item2.ElementAt(i).Value + @"
                         </Paragraph>";
            }
            Ds += @"</Section>
            </FlowDocument>";
            
            return Ds;
        }
        

        private static (Dictionary<string, string>, Dictionary<string, string>, Dictionary<string, string>) PageDataParser(string url)
        {
            Dictionary<string, string> DataDictionary = new Dictionary<string, string>();
            List<string> Hlist = new List<string>();
            Dictionary<string, string> Infodict = new Dictionary<string, string>();
            Dictionary<string, string> GenderDataList = null;
            HtmlDocument html = new HtmlDocument();
            html.LoadHtml(GetRequestData(url));
            DataDictionary.Add("Title", html.GetElementbyId("firstHeading")?.InnerText);
            var trnodes = html.DocumentNode.SelectNodes("//table[@class='infobox']/tbody/tr/td/div/div/table");
            foreach (var trnode in trnodes)
            {

                if (trnode.InnerText.Contains("Род"))
                {
                    try
                    {
                        GenderDataList = GenderInfoParser(GetRequestData("https://ru.wikipedia.org/" + trnode?.SelectSingleNode("tbody//td[2]/a")?.Attributes["href"].Value));
                        DataDictionary.Add("Gender", trnode.InnerText.Split(':')[1]);
                    }
                    catch(Exception EX) { }
                }
            }
            var nd = html.DocumentNode.SelectNodes("//h2");
            var pd = html.DocumentNode.SelectNodes("//p");
            for (int i = 1; i < nd.Count; i++)
            {
                if (nd[i].InnerText.Contains("Классификация")) break;
                else
                {
                    if (nd[i].InnerText.IndexOf("[") > 0)
                         Hlist.Add(nd[i].InnerText.Substring(0, nd[i].InnerText.IndexOf("[")));
                    else Hlist.Add(nd[i].InnerText);
                }
            }
            int m = 0;
            for (int i = 1; i < pd.Count; i++)
            {
                try
                {
                    if (pd[i].InnerText.Contains("Вид включает в себя")) break;
                    else
                    {
                        Infodict.Add(Hlist[m], pd[i].InnerText);
                        m++;
                    }
                }
                catch (Exception EX)
                { m++; }
            }

            return (DataDictionary, Infodict,GenderDataList);
        }

        private static List<string> GetParrotsUrls()
        {
            List<string> Urls = new List<string>();

            HtmlDocument html = new HtmlDocument();
            html.LoadHtml(GetRequestData("https://ru.wikipedia.org/wiki/%D0%A2%D0%B0%D0%BA%D1%81%D0%BE%D0%BD%D0%BE%D0%BC%D0%B8%D1%87%D0%B5%D1%81%D0%BA%D0%B8%D0%B9_%D1%81%D0%BF%D0%B8%D1%81%D0%BE%D0%BA_%D0%BF%D0%BE%D0%BF%D1%83%D0%B3%D0%B0%D0%B5%D0%B2%D1%8B%D1%85"));
            var UlCollection = html.GetElementbyId("mw-content-text").SelectSingleNode("div").SelectNodes("ul");
            foreach (var ul in UlCollection)
            {
                var liCollection = ul.SelectNodes("li");
                foreach (var li in liCollection)
                    if (li.SelectSingleNode("a")?.Attributes["href"].Value != null)
                        Urls.Add(li.SelectSingleNode("a").Attributes["href"].Value);
            }
            return Urls;
        }

        public static Dictionary<string, string> GenderInfoParser(string Data)
        {
            Dictionary<String, string> Infodict = new Dictionary<string, string>();
            List<string> Hlist = new System.Collections.Generic.List<string>();
            HtmlDocument html = new HtmlDocument();
            html.LoadHtml(Data);
            string s = string.Empty;
            s =  html.DocumentNode.SelectSingleNode("//div[@class='mw-parser-output']")?.SelectSingleNode("//p")?.InnerText;
            Infodict.Add("", s);
            var nd = html.DocumentNode.SelectNodes("//h2");
            var pd = html.DocumentNode.SelectNodes("//p");
            for (int i = 1; i < nd.Count; i++)
            {
                if (nd[i].InnerText.Contains("Классификация")) break;
                else
                {
                    if (nd[i].InnerText.IndexOf("[") > 0)
                        Hlist.Add(nd[i].InnerText.Substring(0, nd[i].InnerText.IndexOf("[")));
                    else Hlist.Add(nd[i].InnerText);
                }
            }
            int m = 0;
            for (int i = 1; i < pd.Count; i++)
            {
                try
                {
                    if (pd[i].InnerText.Contains("Вид включает в себя")) break;
                    else
                    {
                        Infodict.Add(Hlist[m], pd[i].InnerText);
                        m++;
                    }
                }
                catch (Exception EX)
                { m++; }
            }
            return Infodict;
        }

        private static string GetRequestData(string url)
        {
            WebRequest webRequest = WebRequest.Create(url);
            return new StreamReader(webRequest.GetResponse().GetResponseStream()).ReadToEnd();
        }
    }
}