using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace OHWebService.Modules
{
    public class FileController
    {
        public FileController()
        {
            //Cloudinary cloudinary = new Cloudinary(System.Configuration.ConfigurationManager.AppSettings.Get("CLOUDINARY_URL"));
        }
        
        public static String UploadFile(string filename, string filepath)
        {
            Cloudinary cloudinary = new Cloudinary(System.Configuration.ConfigurationManager.AppSettings.Get("CLOUDINARY_URL"));

            CloudinaryDotNet.Actions.ImageUploadParams uploadParams = new CloudinaryDotNet.Actions.ImageUploadParams()
            {
                //File = new CloudinaryDotNet.Actions.FileDescription(@"c:\mypicture.jpg"),
                File = new CloudinaryDotNet.Actions.FileDescription(@filepath),
                PublicId = filename
            };

            CloudinaryDotNet.Actions.ImageUploadResult uploadResult = cloudinary.Upload(uploadParams);
            string url = cloudinary.Api.UrlImgUp.BuildUrl(String.Format("{0}.{1}", uploadResult.PublicId, uploadResult.Format));
            return url;
        }
    }
}