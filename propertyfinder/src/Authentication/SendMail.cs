/*
 * Created by SharpDevelop.
 * User: Frenix
 * Date: 3/26/2015
 * Time: 10:23 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Net.Mail;

namespace OHWebService.Authentication
{
	/// <summary>
	/// Description of SendMail.
	/// </summary>
	public class SendMail
	{
		public SendMail()
		{
		}
		
		public static int Send(string fullname, string email, string uuid)
		{
			try
            {
                MailMessage mail = new MailMessage();
                //SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
                SmtpClient SmtpServer = new SmtpClient("smtp.sendgrid.net");
                               
                //mail.From = new MailAddress("proprtyfindr@gmail.com");
                mail.From = new MailAddress("45241146-f24f-4109-b9fe-23df0d1bd2fb@apphb.com");
                //mail.To.Add("efren.duranjr@gmail.com");
                //mail.To.Add("proprtyfindr@gmail.com");
                mail.To.Add(email);
                
                mail.Subject = "[Ownhome.com] Email Confirmation ";
                mail.Body += " <html>";
                mail.Body += "<body>";
                mail.Body += "<table>";
                
				//http://localhost:8000/app/#/signup-success/authenticationkeyhere

	
//                mail.Body += "<tr>";
//                mail.Body += "<td> <h1><img src=" + D:\ownhome\backend\OwnWebService\src\Images\logo-text.png + "></h1></td>";
//                mail.Body += "</tr>";
                
				mail.Body += "<tr>";
                mail.Body += "<td>Hi " +  fullname  + ",</td>";
                mail.Body += "</tr>";
				
                mail.Body += "<tr><td></td></tr>";
                mail.Body += "<tr><td></td></tr>";
                 
                mail.Body += "<tr>";
                mail.Body += "<td>You just signed up for Ownhome.com. Please follow this link to confirm that this is your e-mail address. </td>";
                mail.Body += "</tr>";

                mail.Body += "<tr>";
                mail.Body += "<td>http://www.ownhome.comeze.com/app/#/signup-success/" + uuid + " </td>";
                mail.Body += "</tr>";
				
                mail.Body += "<tr><td></td></tr>";
                
                mail.Body += "<tr>";
                mail.Body += "<td>Thanks, </td>";
                mail.Body += "</tr>";
                
                mail.Body += "<tr><td></td></tr>";
                mail.Body += "<tr><td></td></tr>";
                
                mail.Body += "<tr>";
                mail.Body += "<td>The Ownhome Team </td>";
                mail.Body += "</tr>";
                
                mail.Body += "<tr><td></td></tr>";
                mail.Body += "<tr><td></td></tr>";
                
                 mail.Body += "<tr>";
                mail.Body += "<td><i>Rent. Own. Stay. Enjoy! </i></td>";
                mail.Body += "</tr>";
                	
                mail.Body += "</table>";
                mail.Body += "</body>";
                mail.Body += "</html>";

                mail.IsBodyHtml = true;

                ////System.Net.Mail.Attachment attachment;
                ////attachment = new System.Net.Mail.Attachment(@"D:\bkup\krishna.mdb");
                ////mail.Attachments.Add(attachment);

                SmtpServer.Port = 587;
                SmtpServer.Credentials = new      
                //System.Net.NetworkCredential("proprtyfindr@gmail.com", "395Excel04");
                System.Net.NetworkCredential("45241146-f24f-4109-b9fe-23df0d1bd2fb@apphb.com", "kpmxuiwm8724");
                SmtpServer.EnableSsl = true;

                SmtpServer.Send(mail);

                return 1;              

            }
            catch (Exception err)
            {
            	Console.WriteLine(err.ToString());
            	return 0;
           
            }
		}
		
		public static int ContactAgent(string sendername, string sendermail, string subject, string message,string destname, string destmail)
		{
			try
			{
				MailMessage mail = new MailMessage();
                //SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
                SmtpClient SmtpServer = new SmtpClient("smtp.sendgrid.net");
                               
                //mail.From = new MailAddress("proprtyfindr@gmail.com");
                mail.From = new MailAddress(sendermail);
                mail.To.Add(destmail);
                
                mail.Subject = subject;
                mail.Body += " <html>";
                mail.Body += "<body>";
                mail.Body += "<table>";
                
                
				mail.Body += "<tr>";
                mail.Body += "<td>Hi " +  destname  + ",</td>";
                mail.Body += "</tr>";
				
                mail.Body += "<tr><td></td></tr>";
                mail.Body += "<tr><td></td></tr>";
                 
                mail.Body += "<tr>";
                mail.Body += "<td>" + message + " </td>";
                mail.Body += "</tr>";

            	mail.Body += "<tr><td></td></tr>";					
                mail.Body += "<tr><td></td></tr>";
                
                mail.Body += "<tr>";
                mail.Body += "<td>Thanks, </td>";
                mail.Body += "</tr>";
                
                mail.Body += "<tr><td></td></tr>";
                mail.Body += "<tr><td></td></tr>";
                
                mail.Body += "<tr>";
                mail.Body += "<td>" + sendername + " </td>";
                mail.Body += "</tr>";
                
                mail.Body += "<tr><td></td></tr>";
                mail.Body += "<tr><td></td></tr>";
                
                mail.Body += "<tr><td></td></tr>";
                	
                mail.Body += "</table>";
                mail.Body += "</body>";
                mail.Body += "</html>";

                mail.IsBodyHtml = true;

                ////System.Net.Mail.Attachment attachment;
                ////attachment = new System.Net.Mail.Attachment(@"D:\bkup\krishna.mdb");
                ////mail.Attachments.Add(attachment);

                SmtpServer.Port = 587;
                SmtpServer.Credentials = new      
                //System.Net.NetworkCredential("proprtyfindr@gmail.com", "395Excel04");
                System.Net.NetworkCredential("45241146-f24f-4109-b9fe-23df0d1bd2fb@apphb.com", "kpmxuiwm8724");
                SmtpServer.EnableSsl = true;

                SmtpServer.Send(mail);

				return 1;
			}
			catch (Exception err)
            {
            	Console.WriteLine(err.ToString());
            	return 0;
           
            }
		}
		
		public static int ConfirmationEmail(string sendername, string sendermail)
		{
			string message = "Well done! Agent successfully emailed!";
			try
			{
				MailMessage mail = new MailMessage();
                //SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
                SmtpClient SmtpServer = new SmtpClient("smtp.sendgrid.net");
                               
                mail.From = new MailAddress("45241146-f24f-4109-b9fe-23df0d1bd2fb@apphb.com");
                mail.To.Add(sendermail);
                
                mail.Subject = "[Ownhome] Confirmation Email";
                mail.Body += " <html>";
                mail.Body += "<body>";
                mail.Body += "<table>";
                
                
				mail.Body += "<tr>";
                mail.Body += "<td>Hi " +  sendername  + ",</td>";
                mail.Body += "</tr>";
				
                mail.Body += "<tr><td></td></tr>";
                mail.Body += "<tr><td></td></tr>";
                 
                mail.Body += "<tr>";
                mail.Body += "<td>" + message + " </td>";
                mail.Body += "</tr>";

            	mail.Body += "<tr><td></td></tr>";
                
                mail.Body += "<tr>";
                mail.Body += "<td>Thanks, </td>";
                mail.Body += "</tr>";
                
                mail.Body += "<tr><td></td></tr>";
                mail.Body += "<tr><td></td></tr>";
                
                mail.Body += "<tr>";
                mail.Body += "<td>The Ownhome Team </td>";
                mail.Body += "</tr>";
                
                mail.Body += "<tr><td></td></tr>";
                mail.Body += "<tr><td></td></tr>";
                
                 mail.Body += "<tr>";
                mail.Body += "<td><i>Rent. Own. Stay. Enjoy! </i></td>";
                mail.Body += "</tr>";
                	
                mail.Body += "</table>";
                mail.Body += "</body>";
                mail.Body += "</html>";

                mail.IsBodyHtml = true;

                SmtpServer.Port = 587;
                SmtpServer.Credentials = new      

                System.Net.NetworkCredential("45241146-f24f-4109-b9fe-23df0d1bd2fb@apphb.com", "kpmxuiwm8724");
                SmtpServer.EnableSsl = true;

                SmtpServer.Send(mail);

				return 1;
			}
			catch (Exception err)
            {
            	Console.WriteLine(err.ToString());
            	return 0;
           
            }
		}
		
		
	} //end of class
}
