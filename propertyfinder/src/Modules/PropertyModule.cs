/*
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
        public PropertyModule(IRootPathProvider pathProvider) : base("/properties")
		{
			// /Properties           GET: Get All Available Properties (public) 
			Get["/"] = parameter => { return GetAll(); };

            // /Properties           GET: Get All Available Properties (public) by AgentId
            Get["/agent/{id}"] = parameter => { return GetAllListingByAgent(parameter.id); };

			
			// /Properties           POST: Listing JSON in body
			Post["/"] = parameter => { return this.AddListing(); };

            // /Properties           POST: Listing JSON in body
            Post["/upload"] = parameter => { return this.UploadListingImg(pathProvider); };
			
			// /Properties           DELETE: {ListingId}
			Delete["/{id}"] = parameter => { return this.DeleteListing(parameter.id); };
			
			// /Properties           UPDATE:  Token JSON in body
            Put["/"] = parameter => { return this.UpdateListing(); };
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
				return CommonModule.HandleException(e, HttpStatusCode.OK, String.Format("PropertyModule.GetAll()"), "NG", this.Request);
			}
		}

		public Nancy.Response GetAllListingByAgent(long agentId)
        {
            IList<PropertyImgModel> listingImg;
            ListingResp resp = new ListingResp();
            //PropertyList properties = new PropertyList();
            IList<ListingResp> properties = new List<ListingResp>();
         
            AgentContext ctxAgent = new AgentContext();
            AgentModel agent = ctxAgent.GetById(agentId);
            
            try
            {
               // create a connection to the PetaPoco orm and try to fetch and object with the given Id
               PropertyContext ctx = new PropertyContext();
               // Get Listing by AgentId
                IList<PropertyModel> listings = ctx.GetByAgentId(agentId, true);

                PropertyImgContext ctxImg = new PropertyImgContext();

                foreach (var listing in listings)
                {
                    // Get Images associated in a listing 
                    listingImg = ctxImg.GetByListingId(999, 0, listing.ListingId.ToString());

                    // return this info
                    resp = new ListingResp
                    {
                        Property = listing,
                        Images = listingImg,
                        Agent = agent
                    };

                    //add in a list of properties
                    //we want to have a json format in this way
                    // { [listing1 [listing2images], listing2 [listing2images], ....}
                    properties.Add(resp);
                }
                // Nancy will convert this into an array of JSON objects
                Nancy.Response response = new Nancy.Responses.JsonResponse< IList<ListingResp>>(properties, new DefaultJsonSerializer());
				response.StatusCode = HttpStatusCode.OK;
				return response;

            }
            catch (Exception e)
            {
                return CommonModule.HandleException(e, HttpStatusCode.OK, String.Format("PropertyModule.GetAll()"), "NG", this.Request);
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
				//Get current datetime
				listing.CreatedDate = DateTime.Now;
				ctx.Add(listing);
				
//				// 201 - created
//				Nancy.Response response = new Nancy.Responses.JsonResponse<PropertyModel>(listing, new DefaultJsonSerializer());
//				response.StatusCode = HttpStatusCode.Created;
//				// uri
//				string uri = this.Request.Url.SiteBase + this.Request.Path + "/" + listing.Description;
//				response.Headers["Location"] = uri;

				return MsgBuilder.MsgResponse(this.Request.Url.ToString(), "POST", HttpStatusCode.Created, "OK", listing.ListingId.ToString()); //  "Profile created successfully!");
			}
			catch (Exception e)
			{
				Console.WriteLine(rawBody);
				String operation = String.Format("PropertyModule.AddListing({0})", (listing == null) ? "No Model Data" : listing.Title);
				return CommonModule.HandleException(e, HttpStatusCode.OK, operation, "NG", this.Request);
			}	
		}
		

        // POST /upload
        public dynamic UploadListingImg(IRootPathProvider pathProvider)
        {
        	PropertyImgContext ctxImg = new PropertyImgContext();
            PropertyImgModel listingImg = new PropertyImgModel();
		            
        	// We assume this listing id is existing in the db
            var listingId = this.Request.Form["ListingId"];
            string urlFile;
            int idx=1;
			try
			{
	            // Delete data first from cloudinary if it exists
	            // set listingId as the tag name
	            FileController.DeleteFileByTag(listingId);
	            
	            //delete entries in listing_images db also
	            ctxImg.deletebylistingid(listingId);
	            
	            //store images to cloud and then only url will be saved in db
	            string p = pathProvider.GetRootPath();
//	            var file = this.Request.Files.FirstOrDefault();
//	            if (file == null)
//	            {
//	                return MsgBuilder.MsgResponse(this.Request.Url.ToString(), "POST", HttpStatusCode.BadRequest, "NG", "File is empty!");
//	            }
	
	            var uploadDirectory = Path.Combine(p, "App_Data","Content", "uploads");
	            if (!Directory.Exists(uploadDirectory))
				{
				    Directory.CreateDirectory(uploadDirectory);
				}
	            foreach (var file in Request.Files)
				{
	            	         
	          		var filename = 	String.Format("img_{0}{1}", listingId, idx.ToString());
	            	var filepath = Path.Combine(uploadDirectory, filename + ".jpeg");
		            using (FileStream fileStream = new FileStream(filepath, FileMode.Create))
		            {
		                file.Value.CopyTo(fileStream);
		            }
				
		            // set listingId as the tag name
		            urlFile = FileController.UploadFile(filename, filepath, listingId);
		
		            //add this data to listing_images db
		            
		
		            listingImg.Filename = filename;
		            listingImg.ListingId = listingId;
		            listingImg.Url = urlFile;
		
		            ctxImg.Add(listingImg);
		            idx++;
	            }
	
	            return MsgBuilder.MsgResponse(this.Request.Url.ToString(), "POST", HttpStatusCode.OK, "OK", "Listings uploaded successfully!");
			}
			catch (Exception e)
			{
				String operation = String.Format("PropertyModule.UploadListingImg({0})", (listingId == null) ? "No Model Data" : listingId);
				return CommonModule.HandleException(e, HttpStatusCode.OK, operation, "NG", this.Request);
			}	
        }


		// PUT /Properties/1
		// http://stackoverflow.com/questions/2342579/http-status-code-for-update-and-delete 
		Nancy.Response UpdateListing()
		{
			PropertyModel listing = null;
			int id;
			
			try
			{
				// bind the request body to the object
				listing = this.Bind<PropertyModel>();

				PropertyContext ctx = new PropertyContext();

				id = (int)listing.ListingId;
				PropertyModel res = ctx.GetById(id);
				if (res == null)
				{
					return MsgBuilder.MsgResponse(this.Request.Url.ToString(), "PUT", HttpStatusCode.NotFound, "NG", "Property not found");
				}
				
				//update CreatedDate to Date of Update
				listing.CreatedDate = DateTime.Now;
				
				ctx.update(listing);
				return MsgBuilder.MsgResponse(this.Request.Url.ToString(), "PUT", HttpStatusCode.Created, "OK", "Updated successfully!"); 
			}
			catch (Exception e)
			{
				String operation = String.Format("PropertyModule.UpdateListing({0})", (listing == null) ? "No Model Data" : listing.Title);
				return CommonModule.HandleException(e, HttpStatusCode.OK, operation, "NG", this.Request);
			}
		}
		
		// DELETE /Properties/99
		Nancy.Response DeleteListing(int id)
		{
			try
			{
				PropertyContext ctx = new PropertyContext();
				PropertyModel res = ctx.GetById(id);
                
				PropertyImgContext ctxImg = new PropertyImgContext();
				if (res == null)
				{
					return MsgBuilder.MsgResponse(this.Request.Url.ToString(), "DELETE", HttpStatusCode.NotFound, "NG", String.Format("Agent with Id = {0} does not exist", id));
				}
				PropertyModel ci = new PropertyModel();
				ci.ListingId = id;
				// delete images from listing images table
				ctxImg.deletebylistingid(id);
				// now delete listings table
				ctx.delete(ci);
				return MsgBuilder.MsgResponse(this.Request.Url.ToString(), "DELETE", HttpStatusCode.OK, "OK", "Deleted successfully!"); 
			}
			catch (Exception e)
			{
				return CommonModule.HandleException(e, HttpStatusCode.OK, String.Format("\nPropertyModule.Delete({0})", id), "NG", this.Request);
			}
		}
		
		
	} //end class PropertyModule
}
