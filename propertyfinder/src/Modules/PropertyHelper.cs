/*
 * Created by SharpDevelop.
 * User: Frenix
 * Date: 5/9/2015
 * Time: 9:36 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using OHWebService.Models;
using OHWebService.Modules;

namespace OHWebService.Modules
{
	/// <summary>
	/// Description of PropertyHelper.
	/// </summary>
	public class PropertyHelper
	{
		public PropertyHelper()
		{
		}
		
		//not yet used
		//TODO: make this function common to /property and /public
		public static IList<ListingResp> GetListingByAgent(long agentId)
        {
    		IList<PropertyImgModel> listingImg;
    		ListingResp resp = new ListingResp();
    		
			IList<ListingResp> properties = new List<ListingResp>();
    	
			AgentContext ctxAgent = new AgentContext();
            AgentModel agent = ctxAgent.GetById(agentId);
            
    		// create a connection to the PetaPoco orm and try to fetch and object with the given Id
           	PropertyContext ctx = new PropertyContext();
           	// Get Listing by AgentId
            IList<PropertyModel> listings = ctx.GetByAgentId(agentId);

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
            
            return properties;
        }
		
	} //end of class
}
