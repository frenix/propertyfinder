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
        
        public static String UploadFile(string filename, string filepath, string tags)
        {
            Cloudinary cloudinary = new Cloudinary(System.Configuration.ConfigurationManager.AppSettings.Get("CLOUDINARY_URL"));

            CloudinaryDotNet.Actions.ImageUploadParams uploadParams = new CloudinaryDotNet.Actions.ImageUploadParams()
            {
                //File = new CloudinaryDotNet.Actions.FileDescription(@"c:\mypicture.jpg"),
                File = new CloudinaryDotNet.Actions.FileDescription(@filepath),
                Transformation = new Transformation().Width(1000).Height(1000).Crop("limit"),
                PublicId = filename,
                Overwrite = true,
                Invalidate = true,
                Tags = tags
            };

            CloudinaryDotNet.Actions.ImageUploadResult uploadResult = cloudinary.Upload(uploadParams);
            string url = cloudinary.Api.UrlImgUp.BuildUrl(String.Format("v{0}/{1}.{2}", uploadResult.Version, uploadResult.PublicId, uploadResult.Format));
            return url;
        }
        
    	public static String DeleteFileByTag(string tags)
        {
           Cloudinary cloudinary = new Cloudinary(System.Configuration.ConfigurationManager.AppSettings.Get("CLOUDINARY_URL"));

           DelResResult delResult = cloudinary.DeleteResourcesByTag(tags);
	
           return delResult.ToString();
        }
    }
}