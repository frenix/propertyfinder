﻿/*
 * Created by SharpDevelop.
 * User: durane
 * Date: 2/20/2015
 * Time: 9:03 AM
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


namespace OHWebService.Modules
{
	/// <summary>
	/// The root for this service is http://<ip></ip>:<port></port>/Properties
	/// </summary>
	public class PropertyModule : Nancy.NancyModule
	{
	    //temp string to return
		const String PropertyPage = @"
                        <html><body>
                        <h1>Properties Page </h1>
                        </body></html>
                        ";
		
		CommonModule cmn = new CommonModule();
		public PropertyModule() : base ("/Properties")
		{
			// /Properties           GET: Get All Available Properties (public) 
			Get["/"] = parameter => { return GetAll(); };
			
			// /Properties           POST: Listing JSON in body
			Post["/"] = parameter => { return this.AddListing(); };
			
			// /Properties           DELETE: {ListingId}
			Delete["/{id}"] = parameter => { return this.DeleteListing(parameter.id); };
			
			// /Properties           UPDATE: 
		}
		
		// -- IMPLEMENTATION PART --
		
		// Get all data
		private object GetAll()
		{
			try
			{
				// create a connection to the PetaPoco orm and try to fetch and object with the given Id
				PropertyContext ctx = new PropertyContext();
				// Get all (or rather the first 999) objects
                IList<PropertyModel> res = ctx.Get(999, 0, ""); // future development parameters are: top, from, filter
				// Nancy will convert this into an array of JSON objects.
                return res;
			}
			catch (Exception e)
			{
				return CommonModule.HandleException(e, String.Format("PropertyModule.GetAll()"), "NG", this.Request);
			}
		}
		
		// Add property for a particular agent
		Nancy.Response AddListing()
		{
		    // capture actual string posted in case the bind fails (as it will if the JSON is bad)
			// need to do it now as the bind operation will remove the data
			//String rawBody = this.GetBodyRaw(); 
			String rawBody = CommonModule.GetBodyRaw(this.Request);
			
			PropertyModel listing = null;
			try
			{
				// bind the request body to the object via a Nancy module.
				listing = this.Bind<PropertyModel>();

				// check exists. Return 409 if it does
				if (listing.ListingId > 0)
				{
					return MsgBuilder.MsgResponse(this.Request.Url.ToString(), "POST", HttpStatusCode.NotAcceptable, "NG", String.Format("Listing information exists -> {0}", listing.ListingId));
				}

				// Connect to the database
				PropertyContext ctx = new PropertyContext();
				ctx.Add(listing);
				
				// 201 - created
				Nancy.Response response = new Nancy.Responses.JsonResponse<PropertyModel>(listing, new DefaultJsonSerializer());
				response.StatusCode = HttpStatusCode.Created;
				// uri
				string uri = this.Request.Url.SiteBase + this.Request.Path + "/" + listing.Description;
				response.Headers["Location"] = uri;

				return response;
			}
			catch (Exception e)
			{
				Console.WriteLine(rawBody);
				String operation = String.Format("PropertyModule.AddListing({0})", (listing == null) ? "No Model Data" : listing.Title);
				return CommonModule.HandleException(e, operation, "NG", this.Request);
			}	
		}
		
		// PUT /Properties/1
		// http://stackoverflow.com/questions/2342579/http-status-code-for-update-and-delete 
		Nancy.Response UpdateListing(int id)
		{
			PropertyModel listing = null;
			try
			{
				// bind the request body to the object
				listing = this.Bind<PropertyModel>();

				PropertyContext ctx = new PropertyContext();

				listing.ListingId = id;
				PropertyModel res = ctx.GetById(id);
				if (res == null)
				{
					return 404;
				}

				ctx.update(listing);
				return 204; // no content response
			}
			catch (Exception e)
			{
				String operation = String.Format("PropertyModule.UpdateListing({0})", (listing == null) ? "No Model Data" : listing.Title);
				return CommonModule.HandleException(e, operation, "NG", this.Request);
			}
		}
		
		// DELETE /Properties/99
		Nancy.Response DeleteListing(int id)
		{
			try
			{
				PropertyContext ctx = new PropertyContext();
				PropertyModel res = ctx.GetById(id);

				if (res == null)
				{
					return MsgBuilder.MsgResponse(this.Request.Url.ToString(), "DELETE", HttpStatusCode.NotFound, "NG", String.Format("Agent with Id = {0} does not exist", id));
				}
				PropertyModel ci = new PropertyModel();
				ci.ListingId = id;
				ctx.delete(ci);
				return 204;
			}
			catch (Exception e)
			{
				return CommonModule.HandleException(e, String.Format("\nPropertyModule.Delete({0})", id), "NG", this.Request);
			}
		}
		
		
	} //end class PropertyModule
}
