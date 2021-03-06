using System;
using System.IO;
using System.Reflection;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace BuscaLinksDiscord
{
    class Program
    {
        static ChromeDriver driver;
        static StreamWriter arquivo;
        static string email;
        static string senha;
        static string servidor;
        static string topico;

        static void Main(string[] args)
        {
            Console.WriteLine("Por favor, digite seu login do discord");
            email = Console.ReadLine();

            Console.WriteLine("e agora, sua senha ");
            senha = Console.ReadLine();

            Console.WriteLine("o nome do servidor (respeitando as letras maiúsculas e a pontuação)");
            servidor = Console.ReadLine();

            Console.WriteLine("e finalmente o nome do topico aonde estão os links que deseja gravar no arquivo (também respeitando as letras maiúsculas e a pontuação)");
            topico = Console.ReadLine();

            Console.WriteLine("Certo. Agora peço por gentileza para aguardar um tempinho pois o processo é um pouco demorado.");

            if (!Directory.Exists("C://Links do Discord"))
            {
                Directory.CreateDirectory("C://Links do Discord");
            }

            arquivo = File.CreateText("C://Links do Discord//arquivoComOsLinks.txt");

            driver = new ChromeDriver(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));

            abrirJanelaEClicarNoBotaoEntrar(driver);
            fazerLogin(driver);
            buscarElementos(driver);

            Console.WriteLine("Hello World!");
        }

        static void abrirJanelaEClicarNoBotaoEntrar(ChromeDriver driver) {
            driver.Navigate().GoToUrl("https://discord.com/");
            var botaoDeEntrar = driver.FindElementByCssSelector("div.appButton-2wSXh-");
            botaoDeEntrar.Click();
        }

        static void fazerLogin(ChromeDriver driver)
        {
            if (driver.Url == "https://discord.com/login")
            {
                var campoEmail = driver.FindElementByCssSelector("input[name='email']");
                campoEmail.SendKeys(email);
                var campoSenha = driver.FindElementByCssSelector("input[name='password']");
                campoSenha.SendKeys(senha);
                var botaoEntrar = driver.FindElementByCssSelector("button[type='submit']");
                botaoEntrar.Click();
            }
        }

        static void buscarElementos(ChromeDriver driver)
        {
            while (driver.Url != "https://discord.com/channels/@me")
            {
                Thread.Sleep(5000);
            }

            Thread.Sleep(5000);

            var menuDoServidor = driver.FindElementByCssSelector("div[aria-label*='"+servidor+"']");
            menuDoServidor.Click();
            var menuDoTopico = driver.FindElementByCssSelector("a[aria-label*='"+topico+"']");
            menuDoTopico.Click();

            Thread.Sleep(5000);

            buscaComentarios(driver);

            Thread.Sleep(5000);

            for (int i = 0; i < 100; i++)
            {
                driver.ExecuteScript("document.getElementsByClassName('auto-Ge5KZx scrollerBase-289Jih')[0].scroll(0,document.getElementsByClassName('auto-Ge5KZx scrollerBase-289Jih')[0].heigth)");
                Thread.Sleep(5000);
                buscaComentarios(driver);
            }
        }

        static void buscaComentarios(ChromeDriver driver)
        {
            var listademsgs = driver.FindElementsByCssSelector("div[class='message-2qnXI6 cozyMessage-3V1Y8y groupStart-23k01U wrapper-2a6GCs cozy-3raOZG zalgo-jN1Ica']");
            string nome = null;
            string attr = null;
            string linkSemATag = null;

            if (listademsgs.Count > 0) {
                foreach (IWebElement msg in listademsgs)
                {
                    var conteudo = msg.FindElement(By.CssSelector(".contents-2mQqc9 .markup-2BOw-j.messageContent-2qWWxC"));
                    var Links = conteudo.FindElements(By.TagName("a"));
                    if (Links.Count > 0)
                    {
                        arquivo.WriteLine("-----------------------------------------------");
                        nome = msg.FindElement(By.CssSelector(".header-23xsNx .headerText-3Uvj1Y span")).Text;
                        arquivo.WriteLine(nome);
                        foreach (var taglink in Links)
                        {
                            attr = taglink.GetAttribute("href");
                            arquivo.WriteLine(attr);
                        }
                    }
                    else {
                        if (conteudo.Text.Contains("http")) {
                            arquivo.WriteLine("-----------------------------------------------");
                            nome = msg.FindElement(By.CssSelector(".header-23xsNx .headerText-3Uvj1Y span")).Text;
                            arquivo.WriteLine(nome);
                            linkSemATag = conteudo.Text.Split(new string[] { "http" }, StringSplitOptions.None)[1];
                            linkSemATag = linkSemATag.Replace("\r\n", " ");
                            arquivo.WriteLine("http" + linkSemATag.Split(" ")[0]);
                        }
                    }
                }
            }
        }
    }
}
