/*
 * Created by SharpDevelop.
 * User: Frenix
 * Date: 5/9/2015
 * Time: 6:35 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using Nancy;
using Nancy.ModelBinding;
using Nancy.Responses;
using OHWebService.Models;

namespace OHWebService.Modules
{
	/// <summary>
	/// Public access for users who wants to access properties lists
	/// </summary>
	public class PublicModule : Nancy.NancyModule
	{
		
        public PublicModule() : base("/public")
		{
			// /Public           GET: Get All Available Properties (public) 
			Get["/"] = parameter => { return GetAll(); };


		}
		
		// -- IMPLEMENTATION PART --
		
		// Get all data
		private object GetAll()
		{	
			try
			{
				// create a connection to the PetaPoco orm and try to fetch and object with the given Id
				PropertyContext ctx = new PropertyContext();
				// Get all (or rather the first 999) listings
                IList<PropertyModel> listings = ctx.Get(999, 0, ""); // future development parameters are: top, from, filter
               	IList<ListingResp> properties = new List<ListingResp>();
                
               	IList<PropertyImgModel> listingImg;
	    		ListingResp resp = new ListingResp();
	    		
				
	    		PropertyContext propctx = new PropertyContext();
				AgentContext ctxAgent = new AgentContext();
					
				 IList<PropertyModel> listingsByAgent;
				 	
                // loop through each listings and then get related listing images
                foreach(PropertyModel listing in listings) 
                {
                	//properties.Add( PropertyHelper.GetListingByAgent(listing.AgentId));
		            AgentModel agent = ctxAgent.GetById(listing.AgentId);
		            
		           	// Get Listing by AgentId
		           	listingsByAgent = ctx.GetByAgentId(listing.AgentId);
		
		            PropertyImgContext ctxImg = new PropertyImgContext();
		
		            foreach (var listingperagent in listingsByAgent)
		            {
		                // Get Images associated in a listing 
		                listingImg = ctxImg.GetByListingId(999, 0, listingperagent.ListingId.ToString());
		
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
                }
                
				Nancy.Response response = new Nancy.Responses.JsonResponse< IList<ListingResp>>(properties, new DefaultJsonSerializer());
				response.StatusCode = HttpStatusCode.OK;
				return response;
			}
			catch (Exception e)
			{
				return CommonModule.HandleException(e, HttpStatusCode.OK, String.Format("PropertyModule.GetAll()"), "NG", this.Request);
			}
		}
	}
}
