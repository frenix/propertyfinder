/*
 * Created by Fuego, Inc. 
 * File  :   Agent.cs
 * Author:    Efren Duran
 * Date: 3/17/2015
 * Time: 10:56 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.IO;
using Nancy;
using Nancy.ModelBinding;
using Nancy.Responses;
using OHWebService.Models;
using OHWebService.Authentication;

using Newtonsoft.Json;
using System.Web;

namespace OHWebService.Modules
{
	/// <summary>
	/// The root for this service is http://<ip></ip>:<port></port>/Agent
	/// </summary>
	public class AgentModule : Nancy.NancyModule
	{
	    const String AgentPage = @"
                                <html><body>
                                <h1>Agent Page </h1>
                                </body></html>
                                ";
	    	

		
		public AgentModule(IRootPathProvider pathProvider) : base("/agents")
		{
		    // /agent
		    Get["/"] = parameter => { return GetAll(); };
		    
//		    Get["/upload"] = parameter => { return Upload(); };
				
			// /agent/99
			Get["/{id}"] = parameter => { return GetById(parameter.id); };			
							
			// /agent       POST: Agent JSON in body
			Post["/"] = parameter => { return this.AddAgent(); };
			
			// /agent       POST: Agent JSON in body
			Post["/upload"] = parameter => { return this.UploadProfileImg(pathProvider); };
			
			// /agent       POST: Agent JSON in body
			Post["/contact"] = parameter => { return this.ContactAgent(); };
			
			// /agent        DELETE: {AgentId}
			Delete["/{authkey}"] = parameter => { return this.DeleteAgent(parameter.authkey); };
			
			// /agent/		PUT: Token JSON in body
			Put["/"] = parameter => { return this.UpdateAgentByToken(); };
		}
		
		// -- IMPLEMENTATION PART --
		
		// GET /Agents/99
		private object GetById(int id)
		{
			try
			{
				// create a connection to the PetaPoco orm and try to fetch and object with the given Id
				AgentContext ctx = new AgentContext();
				AgentModel res = ctx.GetById(id);
				if (res == null)   // a null return means no object found
				{
					// return a reponse conforming to REST conventions: a 404 error
					return MsgBuilder.MsgResponse(this.Request.Url.ToString(), "GET", HttpStatusCode.NotFound, "NG", String.Format("Agent with Id = {0} does not exist", id));
				}
				else
				{
					// success. The Nancy server will automatically serialise this to JSON
					return res;
				}
			}
			// Please, please handle exceptions in a way that provides information about the context of the error.
			catch (Exception e)
			{
				return CommonModule.HandleException(e, HttpStatusCode.OK,String.Format("AgentModule.GetById({0})", id), "NG", this.Request);
			}
		}
		
		// Get all data
		private object GetAll()
		{
			try
			{
				// create a connection to the PetaPoco orm and try to fetch and object with the given Id
				AgentContext ctx = new AgentContext();
				// Get all (or rather the first 999) objects
                IList<AgentModel> res = ctx.Get(999, 0, ""); // future development parameters are: top, from, filter
				// Nancy will convert this into an array of JSON objects.
                return res;
			}
			catch (Exception e)
			{
				return CommonModule.HandleException(e, HttpStatusCode.OK, String.Format("AgentModule.GetAll()"), "NG", this.Request);
			}
		}
		
		// POST /agent/upload JSON (Filestream)
		public dynamic UploadProfileImg(IRootPathProvider pathProvider)
        {
	
				string p = pathProvider.GetRootPath();
				var file = this.Request.Files.FirstOrDefault();
				if (file == null) 
				{
					return MsgBuilder.MsgResponse(this.Request.Url.ToString(), "POST", HttpStatusCode.BadRequest, "NG", "File is empty!");
				}
                var userName = this.Request.Form["username"];
                var agentId = this.Request.Form["agentId"];
                            
				var fileDetails = string.Format("{3} - {0} ({1}) {2}bytes", file.Name, file.ContentType, file.Value.Length, file.Key);
                var filename = Path.Combine(p, "App_Data", userName + ".jpeg");
      
                
             	using (var fileStream = new FileStream(filename, FileMode.Create))
                {
                    file.Value.CopyTo(fileStream);
                }
             	
             	string urlFile = FileController.UploadFile(userName, filename, agentId);
             	
             	//need to update dbase with url of profilepic given the email add 
         		AgentContext ctx = new AgentContext();
         		AgentModel agent = ctx.GetById(agentId);
             	if (agent == null)
				{
					return MsgBuilder.MsgResponse(this.Request.Url.ToString(), "POST", HttpStatusCode.NotFound, "NG", String.Format("Agent with username = {0} does not exist", userName));
				}
             	
             	agent.ProfileFileName = userName;
             	agent.ProfileUrl = urlFile;
             	
             	ctx.update(agent);
             	
             	return MsgBuilder.MsgResponse(this.Request.Url.ToString(), "POST", HttpStatusCode.OK, "OK", urlFile);
        }
	
		Nancy.Response ContactAgent()
		{
			// debug code only
			// capture actual string posted in case the bind fails (as it will if the JSON is bad)
			// need to do it now as the bind operation will remove the data
			String rawBody = CommonModule.GetBodyRaw(this.Request);
			
			AgentContactInfo contact = new AgentContactInfo();
			try
			{
				// bind the request body to the object via a Nancy module.
				contact = this.Bind<AgentContactInfo>();
				
				// check exists. Return 409 if it does
				if ((contact.SenderEmail.Length == 0) && (contact.AgentId == 0))
				{
					return MsgBuilder.MsgResponse(this.Request.Url.ToString(), "POST", HttpStatusCode.NotAcceptable, "NG", String.Format("Entries cannot be empty!"));
				}
				
				// retrieve agent information
				AgentContext ctx = new AgentContext();
				AgentModel agent = ctx.GetById((int)contact.AgentId);
				string agentname = agent.FirstName + " " + agent.LastName;
				// setup for email
				SendMail.ContactAgent(contact.SenderName,contact.SenderEmail, contact.Subject, contact.Message, agentname, agent.EmailAddress);
				return MsgBuilder.MsgResponse(this.Request.Url.ToString(), "POST", HttpStatusCode.OK, "OK", "Agent successfully emailed!"); ;
			}
			catch (Exception e)
			{
				Console.WriteLine(rawBody);
				String operation = String.Format("AgentModule.ContactAgent({0})", (contact == null) ? "No Model Data" : contact.SenderEmail);
				return CommonModule.HandleException(e, HttpStatusCode.OK, operation, "NG", this.Request);
			}	
		}
		// POST /Agent
		Nancy.Response AddAgent() 
		{
			// debug code only
			// capture actual string posted in case the bind fails (as it will if the JSON is bad)
			// need to do it now as the bind operation will remove the data
			String rawBody = CommonModule.GetBodyRaw(this.Request);
			
			//setup GUID for this user
			Guid uuid = GuidCreator.New();
			string fullName;
			
			AgentModel profile = null;
			try
			{
				// bind the request body to the object via a Nancy module.
				profile = this.Bind<AgentModel>();

				// check exists. Return 409 if it does
				if ((profile.EmailAddress.Length == 0) && (profile.Password.Length == 0))
				{
					return MsgBuilder.MsgResponse(this.Request.Url.ToString(), "POST", HttpStatusCode.NotAcceptable, "NG", String.Format("Please update your email address-> {0}", profile.EmailAddress));
				}
				
				//update AuthKey with GUID
				profile.AuthKey = uuid.ToString();
				//create fullname of the agent
				fullName = profile.FirstName + " " + profile.LastName;
                //Set ConfirmFlg to 0, this means user has not yet confirmed account via email
                profile.ConfirmFlag = "0";

				// Connect to the database
				AgentContext ctx = new AgentContext();
				ctx.Add(profile);
					
				//send email for confirmation
				// this is to update confirmedFlag in db
				SendMail.Send(fullName , profile.EmailAddress, uuid.ToString());
				return MsgBuilder.MsgResponse(this.Request.Url.ToString(), "POST", HttpStatusCode.Created, "OK", profile.AgentId.ToString()); //  "Profile created successfully!");
			}
			catch (Exception e)
			{
				Console.WriteLine(rawBody);
				String operation = String.Format("AgentModule.AddAgent({0})", (profile == null) ? "No Model Data" : profile.EmailAddress);
				return CommonModule.HandleException(e, HttpStatusCode.OK, operation, "NG", this.Request);
			}	
			
		}
		
		// DELETE /Agent/99
		Nancy.Response DeleteAgent(string authkey)
		{
			try
			{
				AgentContext ctx = new AgentContext();
				AgentModel res = ctx.GetByToken(authkey);

				if (res == null)
				{
					return MsgBuilder.MsgResponse(this.Request.Url.ToString(), "DELETE", HttpStatusCode.NotFound, "NG", String.Format("Agent with Id = {0} does not exist", authkey));
				}
				AgentModel ci = new AgentModel();
				ci.AgentId = res.AgentId;
				ctx.delete(ci);

				return MsgBuilder.MsgResponse(this.Request.Url.ToString(),"DELETE", HttpStatusCode.NoContent, "OK", String.Format("{0} deleted successfully!", res.EmailAddress));
			}
			catch (Exception e)
			{
				return CommonModule.HandleException(e, HttpStatusCode.OK, String.Format("\nAgentModule.Delete({0})", authkey), "NG", this.Request);
			}
		}
		
		//PUT /agents
		Nancy.Response UpdateAgentByToken()
		{
			// debug code only
			// capture actual string posted in case the bind fails (as it will if the JSON is bad)
			// need to do it now as the bind operation will remove the data
			String rawBody = CommonModule.GetBodyRaw(this.Request);
			
			AgentModelToken agentauthkey = null;
			try
			{
				// bind the request body to the object
				agentauthkey= this.Bind<AgentModelToken>();

				AgentContext ctx = new AgentContext();

				AgentModel agent = ctx.GetByToken(agentauthkey.token);
				
				if (agent == null)
				{
					return MsgBuilder.MsgResponse(this.Request.Url.ToString(), "PUT", HttpStatusCode.NotFound, "NG", String.Format("Agent with email  {0} does not exist", agent.EmailAddress));
				}

				ctx.update(agent);
                return MsgBuilder.MsgResponse(this.Request.Url.ToString(), "PUT", HttpStatusCode.NoContent, "OK", String.Format("{0} updated successfully!", agent.EmailAddress));
			}
			catch (Exception e)
			{
				String operation = String.Format("AgentModule.UpdateAgentByToken({0})", (agentauthkey == null) ? "No Model Data" : agentauthkey.token);
				return CommonModule.HandleException(e, HttpStatusCode.OK, operation,"NG",this.Request);
			}
		}
		
		
	} //end Class: Agent
}
