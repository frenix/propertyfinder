using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OHWebService.Models;


namespace OHWebService.Modules
{
    public class PropertyImgContext
    {
        internal IList<PropertyImgModel> Get(int top, int from, string filter)
        {
            // TODO: acknowledge parameter values.
            String sql = "select * from listing_images where SoldFlag = 0 order by ImageId ";
            return CommonModule.GetDatabase().Query<PropertyImgModel>(sql).ToList();
        }

        internal IList<PropertyImgModel> GetByListingId(int top, int from, string filter)
        {
            // TODO: acknowledge parameter values.
            String sql = "select * from listing_images where (SoldFlag = 0 And AgentId=" + filter +" order by ImageId ";
            return CommonModule.GetDatabase().Query<PropertyImgModel>(sql).ToList();
        }

        public void Add(PropertyImgModel listingImg)
        {
            CommonModule.GetDatabase().Insert(listingImg);
        }

        public PropertyImgModel GetById(int id)
        {
            String sql = "select * from listing_images where ListingId =" + id.ToString();
            return CommonModule.GetDatabase().FirstOrDefault<PropertyImgModel>(sql);
        }

        internal void update(PropertyImgModel listingImg)
        {
            CommonModule.GetDatabase().Update(listingImg);
        }

        internal void delete(PropertyImgModel listingImg)
        {
            CommonModule.GetDatabase().Delete(listingImg);
        }
    }

    
}