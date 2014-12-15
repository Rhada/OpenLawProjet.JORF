using Splayce.JORF.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Splayce.JORF.Parser
{
    /// <summary>
    /// public interface for extracting JORFText from xml files
    /// </summary>
    public class Manager
    {
        
        public List<JORFText> GetJORFTextFromUpdate(String path)
        {
            List<JORFText> texts = new List<JORFText>();
            if (System.IO.Directory.Exists(path))
            {
                List<String> xmlConts = GetConteneurXMLFiles(path + "\\conteneur");
                foreach (String xmlCont in xmlConts)
                {
                    List<JORFText> subResult = Builder.BuildJorfText(xmlCont, path);
                    if (subResult.Count > 0)
                    {
                        texts.AddRange(subResult);
                    }
                }
            }
            return texts;
        }
        public List<String> GetConteneurXMLFiles(String path)
        {
            List<String> result = new List<string>();
            if (System.IO.Directory.Exists(path))
            {
                String[] files = System.IO.Directory.GetFiles(path, "JORFCONT*.xml");
                if (files.Length > 0)
                {
                    result.AddRange(files.ToList());
                }
                String[] dirs = System.IO.Directory.GetDirectories(path);
                if (dirs.Length > 0)
                {
                    foreach (String dirpath in dirs)
                    {
                        List<String> subResult = GetConteneurXMLFiles(dirpath);
                        result.AddRange(subResult);
                    }
                }
            }
            return result;
        }
    }
}
