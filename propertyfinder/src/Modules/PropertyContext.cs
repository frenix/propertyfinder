/*
 * Created by Fuego, Inc. 
 * File  :   PropertyContext.cs
 * Author:    Efren Duran
 * Date: 3/20/2015
 * Time: 9:26 AM
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OHWebService.Models;

namespace OHWebService.Modules
{
    /// <summary>
    /// Class to handle manipulating Property details to db
    /// </summary>
    public class PropertyContext
    {
        public PropertyContext()
        {
        }
        
        internal IList<PropertyModel> Get(int top, int from, string filter)
		{
			// TODO: acknowledge parameter values.
			String sql = "select * from listing where SoldFlag = 0 order by ListingId ";
			return CommonModule.GetDatabase().Query<PropertyModel>(sql).ToList();
		}

        public PropertyModel GetById(int listingId)
        {
            String sql = "select * from listing where ListingId =" + listingId.ToString();
            return CommonModule.GetDatabase().FirstOrDefault<PropertyModel>(sql);
        }

        public IList<PropertyModel> GetByAgentId(long agentId, bool inclSold)
        {
             String sql;
            if (inclSold == false ) 
            {
                // do not include sold properties in the list
                sql = "select * from listing where SoldFlag = 0 And AgentId =" + agentId.ToString();
            }
            else 
            {
                // include all properties, sold or unsold
                sql = "select * from listing where AgentId =" + agentId.ToString();
            }
            return CommonModule.GetDatabase().Query<PropertyModel>(sql).ToList();
        }

        public IList<PropertyModel> GetByAddress (string location)
        {
        	String sql = "select * from listing where SoldFlag = 0 And Address LIKE '%" + location + "%'";
    	 	return CommonModule.GetDatabase().Query<PropertyModel>(sql).ToList();
        }
        public void Add(PropertyModel listing)
		{
			CommonModule.GetDatabase().Insert(listing);
		}
        
                
        internal void update(PropertyModel listing)
		{
			CommonModule.GetDatabase().Update(listing);
		}
        
        internal void delete(PropertyModel listing)
		{
			CommonModule.GetDatabase().Delete(listing);
		}
        
    }
}
