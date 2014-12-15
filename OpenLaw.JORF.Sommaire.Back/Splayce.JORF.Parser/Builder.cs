using Splayce.JORF.Business;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;


namespace Splayce.JORF.Parser
{
    /// <summary>
    /// Exposes methods and function for extracting JORFTexts from XML files
    /// </summary>
    internal class Builder
    {
        /// <summary>
        /// Entry Point for extracting JORFTexts from an update folder
        /// </summary>
        /// <param name="path"></param>
        /// <param name="rootFolder"></param>
        /// <returns></returns>
        public static List<JORFText> BuildJorfText(String path, String rootFolder)
        {
            int ordre = 1;
            List<JORFText> result = new List<JORFText>();
            XDocument xDoc = XDocument.Load(new StreamReader(path));
            String dateJOStr = xDoc.XPathSelectElement("//JO/META/META_SPEC/META_CONTENEUR/DATE_PUBLI").Value;
            DateTime dateJO = DateTime.ParseExact(dateJOStr, "yyyy-MM-dd", CultureInfo.CurrentCulture);

            var rub1 = xDoc.XPathSelectElement("//JO/STRUCTURE_TXT/TM[@niv='1']/TITRE_TM");
            var liens_txtrub1 = rub1.XPathSelectElements("LIEN_TXT");
            foreach (var lien in liens_txtrub1)
            {
                JORFText text = new JORFText()
                {
                    Rubrique1 = rub1.Value.ToString(),
                    Ordre = ordre
                };

                SetJORFTextContent(rootFolder, dateJO, lien, text);
                result.Add(text);
                ordre++;
            }
            var rub2 = xDoc.XPathSelectElements("//JO/STRUCTURE_TXT/TM[@niv='1']/TM[@niv='2']");


            //var liens_txt = xDoc.XPathSelectElements("//JO/STRUCTURE_TXT/TM[@niv='1']/TM[@niv='2']/TM[@niv='3']/TM[@niv='4']/LIEN_TXT");
            foreach (var rubitem2 in rub2)
            {
                XElement xtitlerub2 = rubitem2.Elements().Where(x => x.Name == "TITRE_TM").First();
                String titlerub2 = xtitlerub2.Value;
                if (titlerub2 == "LOIS")
                {
                    titlerub2 = "Parlement";
                }
                var liens_txtrub2 = rubitem2.XPathSelectElements("LIEN_TXT");
                foreach (var lien in liens_txtrub2)
                {
                    JORFText text = new JORFText()
                    {
                        Rubrique1 = rub1.Value.ToString(),
                        Rubrique2 = titlerub2,
                        AutoriteEmettrice = titlerub2,
                        Ordre = ordre
                    };
                    SetJORFTextContent(rootFolder, dateJO, lien, text);
                    result.Add(text);
                    ordre++;
                }

                var rub3 = rubitem2.XPathSelectElements("TM[@niv='3']");
                foreach (var rubitem3 in rub3)
                {
                    XElement xtitlecat3 = rubitem3.Elements().Where(x => x.Name == "TITRE_TM").First();
                    String titlecat3 = xtitlecat3.Value;
                    var rub4 = rubitem3.XPathSelectElements("TM[@niv='4']");
                    foreach (XElement rubitem4 in rub4)
                    {
                        XElement autority = rubitem4.Elements().Where(x => x.Name == "TITRE_TM").First();
                        var liens_txt = rubitem4.XPathSelectElements("LIEN_TXT");
                        foreach (var lien in liens_txt)
                        {
                            JORFText text = new JORFText()
                            {
                                Rubrique1 = rub1.Value.ToString(),
                                Rubrique2 = titlerub2,
                                Rubrique3 = titlecat3,
                                AutoriteEmettrice = autority.Value,
                                Ordre = ordre
                            };
                            SetJORFTextContent(rootFolder, dateJO, lien, text);
                            result.Add(text);
                            ordre++;
                        }
                    }
                }
            }
            return result;
        }
        /// <summary>
        /// Set properties for a JORFText
        /// </summary>
        /// <param name="rootFolder"></param>
        /// <param name="dateJO"></param>
        /// <param name="lien"></param>
        /// <param name="text"></param>
        private static void SetJORFTextContent(String rootFolder, DateTime dateJO, XElement lien, JORFText text)
        {
            text.DateJO = dateJO;
            text.IdText = lien.Attribute("idtxt").Value.ToString();
            text.Titre = lien.Attribute("titretxt").Value.ToString();
            CompleteWithContent(text, rootFolder, text.IdText);
        }
        /// <summary>
        /// Set additionnal properties for a JORFText
        /// </summary>
        /// <param name="text"></param>
        /// <param name="rootFolder"></param>
        /// <param name="id"></param>
        private static void CompleteWithContent(JORFText text, string rootFolder, string id)
        {
            try
            {
                String structPath = rootFolder + @"\texte\struct\JORF\TEXT\";
                String shortid = id.Replace("JORFTEXT", "");
                var parts = shortid.SplitInParts(2).ToArray();
                structPath = ConstructPath(structPath, parts);
                String versionPath = rootFolder + @"\texte\version\JORF\TEXT\";
                versionPath = ConstructPath(versionPath, parts);

                if (System.IO.Directory.Exists(structPath))
                {
                    string textPath = structPath + id + ".xml";
                    ParseStructText(textPath, text);
                    text.Articles = GetArticles(rootFolder, textPath);
                    text.Sections = GetSections(rootFolder, textPath);
                    var modifs = from p in text.Articles where p.Type == "MODIFICATEUR" select p;
                    if (modifs.Count() > 0)
                    {
                        text.Type = "MODIFICATEUR";
                    }
                    else
                    {
                        text.Type = "AUTONOME";
                    }
                }
                if (System.IO.Directory.Exists(versionPath))
                {
                    ParseVersionText(versionPath + id + ".xml", text);
                }
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// Get Section (Titre, Chapitre, etc) for a JORFText
        /// </summary>
        /// <param name="rootFolder"></param>
        /// <param name="textPath"></param>
        /// <returns></returns>
        private static List<Section> GetSections(string rootFolder, string textPath)
        {
            List<Section> sections = new List<Section>();
            XDocument xDoc = XDocument.Load(new StreamReader(textPath));
            var lienSections = xDoc.XPathSelectElements("//TEXTELR/STRUCT/LIEN_SECTION_TA");
            foreach (var lien in lienSections)
            {
                string sctapath = rootFolder + "\\section_ta" + lien.Attribute("url").Value.ToString().Replace("/", "\\");
                Section sec = new Section();
                sec.Title = lien.Value;
                GetListArticleFromSectionFile(rootFolder, sctapath, sec);
                sections.Add(sec);
            }
            return sections;
        }
        /// <summary>
        /// Get a list of JORFArticle for a JORFText
        /// </summary>
        /// <param name="rootFolder"></param>
        /// <param name="sctapath"></param>
        /// <param name="section"></param>
        private static void GetListArticleFromSectionFile(string rootFolder, string sctapath, Section section)
        {
            XDocument xDoc = XDocument.Load(new StreamReader(sctapath));
            var lienSections = xDoc.XPathSelectElements("//SECTION_TA/STRUCTURE_TA/LIEN_SECTION_TA");
            foreach (var lien in lienSections)
            {
                Section sect = new Section();
                string sctapath2 = rootFolder + "\\section_ta" + lien.Attribute("url").Value.ToString().Replace("/", "\\");
                sect.Title = lien.Value;
                GetListArticleFromSectionFile(rootFolder, sctapath2, sect);
                section.Sections.Add(sect);
            }
            List<JORFArticle> arts = GetArticles(rootFolder, sctapath, "//SECTION_TA/STRUCTURE_TA/LIEN_ART");
            section.Articles = arts;
        }
        /// <summary>
        /// Extract information for the version folder of the update
        /// </summary>
        /// <param name="versionPath"></param>
        /// <param name="text"></param>
        private static void ParseVersionText(string versionPath, JORFText text)
        {
            XDocument xDoc = XDocument.Load(new StreamReader(versionPath));
            String content = xDoc.XPathSelectElement("//TEXTE_VERSION/VISAS/CONTENU").Value;
            if (content.Length > 100)
            {
                content = content.Substring(0, 100) + " ...";
            }
            text.ShortContent = content;
            text.TexteIntegral = xDoc.XPathSelectElement("//TEXTE_VERSION/VISAS/CONTENU").ToString().Replace("<CONTENU>", "").Replace("</CONTENU>", "");
            text.Titre = xDoc.XPathSelectElement("//TEXTE_VERSION/META/META_SPEC/META_TEXTE_VERSION/TITRE").Value;
            text.TitreFull = xDoc.XPathSelectElement("//TEXTE_VERSION/META/META_SPEC/META_TEXTE_VERSION/TITREFULL").Value;
            text.Signataires = xDoc.XPathSelectElement("//TEXTE_VERSION/SIGNATAIRES/CONTENU").ToString().Replace("<CONTENU>", "").Replace("</CONTENU>", "");
            text.LienActions = ParseLienAction(versionPath, text);
            if (text.LienActions.FindAll(x => x.TypeAction == "CREE" || x.TypeAction == "MODIFIE" || x.TypeAction == "ABROGE").Count > 0)
            {
                text.Type = "MODIFICATEUR";
            }
        }
        /// <summary>
        /// Extract action link for a JORFText
        /// </summary>
        /// <param name="versionPath"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        private static List<LienAction> ParseLienAction(string versionPath, JORFText text)
        {
            List<LienAction> result = new List<LienAction>();
            XDocument xDoc = XDocument.Load(new StreamReader(versionPath));
            var liens = xDoc.XPathSelectElements("//TEXTE_VERSION/META/META_SPEC/META_TEXTE_VERSION/LIENS/LIEN");
            foreach (var lien in liens)
            {
                LienAction la = new LienAction();
                String typeLien = lien.Attribute("typelien").Value.ToString();
                la.TextModificateur = text.TitreFull;
                la.IdTextModificateur = text.IdText;
                la.TypeAction = typeLien;
                la.Nature = lien.Attribute("naturetexte").Value.ToString();
                la.IdTexteSujet = lien.Attribute("cidtexte").Value.ToString();
                if (la.IdTexteSujet != la.IdTextModificateur)
                {
                    switch (typeLien)
                    {
                        case "CITATION":
                            la.TexteSujet = lien.Value;
                            la.ArticleSujet = lien.Attribute("num").Value.ToString();
                            break;
                        case "CREE":
                        case "MODIFIE":
                        case "ABROGE":
                            la.TexteSujet = lien.Attribute("nomtexte").Value.ToString();
                            la.ArticleSujet = lien.Value;
                            break;
                    }
                    result.Add(la);
                }
            }
            return result;
        }
        /// <summary>
        /// Extract information for the Struct folder of the update
        /// </summary>
        /// <param name="structPath"></param>
        /// <param name="text"></param>
        private static void ParseStructText(string structPath, JORFText text)
        {
            XDocument xDoc = XDocument.Load(new StreamReader(structPath));
            text.Nature = xDoc.XPathSelectElement("//TEXTELR/META/META_COMMUN/NATURE").Value;
            text.NumeroJO = xDoc.XPathSelectElement("//META_SPEC/META_TEXTE_CHRONICLE/NUM").Value;
            text.NOR = xDoc.XPathSelectElement("//META_SPEC/META_TEXTE_CHRONICLE/NOR").Value;
            text.DatePublication = DateTime.ParseExact(xDoc.XPathSelectElement("//META_SPEC/META_TEXTE_CHRONICLE/DATE_PUBLI").Value, "yyyy-MM-dd", CultureInfo.CurrentCulture);
            text.DateTexte = DateTime.ParseExact(xDoc.XPathSelectElement("//META_SPEC/META_TEXTE_CHRONICLE/DATE_TEXTE").Value, "yyyy-MM-dd", CultureInfo.CurrentCulture);

        }
        /// <summary>
        /// Construct path with a two digits array
        /// </summary>
        /// <param name="structPath"></param>
        /// <param name="parts"></param>
        /// <returns></returns>
        private static string ConstructPath(String structPath, string[] parts)
        {
            for (int x = 0; x < 5; x++)
            {
                structPath += parts[x] + "\\";
            }
            return structPath;
        }
        /// <summary>
        /// Get JORFArticles from xmls inside a folder
        /// </summary>
        /// <param name="root"></param>
        /// <param name="structPath"></param>
        /// <param name="xpath"></param>
        /// <returns></returns>
        private static List<JORFArticle> GetArticles(string root, string structPath, String xpath = "")
        {
            List<JORFArticle> articles = new List<JORFArticle>();
            XDocument xDoc = XDocument.Load(new StreamReader(structPath));
            String xpath2 = String.IsNullOrEmpty(xpath) ? "//TEXTELR/STRUCT/LIEN_ART" : xpath;
            var liens = xDoc.XPathSelectElements(xpath2);
            foreach (var lien in liens)
            {
                JORFArticle article = new JORFArticle();

                article.Id = lien.Attribute("id").Value.ToString();
                article.NumeroArticle = lien.Attribute("num").Value.ToString();
                String shortid = article.Id.Replace("JORFARTI", "");
                String articlePath = root + @"\article\JORF\ARTI\";
                var parts = shortid.SplitInParts(2).ToArray();
                articlePath = ConstructPath(articlePath, parts);
                CompleteArticle(articlePath + "\\" + article.Id + ".xml", article);
                articles.Add(article);
            }
            return articles;
        }
        /// <summary>
        /// Extract additional infos for a JORFArticle from the article folder
        /// </summary>
        /// <param name="articlePath"></param>
        /// <param name="article"></param>
        private static void CompleteArticle(string articlePath, JORFArticle article)
        {
            XDocument xDoc = XDocument.Load(new StreamReader(articlePath));
            article.Nature = xDoc.XPathSelectElement("//ARTICLE/META/META_COMMUN/NATURE").Value;
            article.Texte = xDoc.XPathSelectElement("//ARTICLE/BLOC_TEXTUEL").ToString().Replace("<BLOC_TEXTUEL>", "").Replace("</BLOC_TEXTUEL>", "");
            article.Type = xDoc.XPathSelectElement("//ARTICLE/META/META_SPEC/META_ARTICLE/TYPE").Value;
        }

    }
}
