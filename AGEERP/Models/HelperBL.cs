using System;
using System.Drawing;
using System.IO;

namespace AGEERP.Models
{
    public class HelperBL
    {
        /// <summary>
        /// Convert From Image Path to Base 64 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string ConvertFromPathToBase64(string Path, string ExceptionURL)
        {
            try
            {
                if (File.Exists(System.Web.HttpContext.Current.Server.MapPath(Path)))
                {
                    using (Image image = Image.FromFile(Path))
                    {
                        using (MemoryStream m = new MemoryStream())
                        {
                            image.Save(m, image.RawFormat);
                            byte[] imageBytes = m.ToArray();

                            string base64String = Convert.ToBase64String(imageBytes);
                            return base64String;
                        }
                    }
                }
                else
                {
                    string lFileName = System.Web.HttpContext.Current.Server.MapPath(ExceptionURL);
                    using (Image image = Image.FromFile(lFileName))
                    {
                        using (MemoryStream m = new MemoryStream())
                        {
                            image.Save(m, image.RawFormat);
                            byte[] imageBytes = m.ToArray();

                            string base64String = Convert.ToBase64String(imageBytes);
                            return base64String;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                return "";
            }
        }
    }
}