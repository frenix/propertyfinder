/*
 * Created by Fuego, Inc. 
 * File  :   ${FILE}
 * Author:    Efren Duran
 * Date: 3/20/2015
 * Time: 9:26 AM
 * 
 */
using System;
using System.Collections.Generic;

namespace OHWebService.Models
{
    /// <summary>
    /// Class that defines Property and map it to db
    /// </summary>

    [PetaPoco.TableName("listing")]
	[PetaPoco.PrimaryKey("ListingId")]
    public partial class PropertyModel
    {
        [PetaPoco.Column("ListingId")]
		public Int64 ListingId { get; set; }
		[PetaPoco.Column("Title")]
		public String Title { get; set; }
		[PetaPoco.Column("HouseType")]
		public String HouseType { get; set; }
		[PetaPoco.Column("Price")]
		public String Price { get; set; }
		[PetaPoco.Column("Description")]
		public String Description { get; set; }
		[PetaPoco.Column("Address")]
		public String Address { get; set; }
		[PetaPoco.Column("Latitude")]
		public String Latitude { get; set; }
		[PetaPoco.Column("Longitude")]
		public String Longitude { get; set; }
		[PetaPoco.Column("SoldFlag")]
		public Int32 SoldFlag { get; set; }
		[PetaPoco.Column("AgentId")]
		public Int64 AgentId { get; set; }  
		[PetaPoco.Column("CreatedDate")]
		public DateTime CreatedDate { get; set; }  
    }

    [PetaPoco.TableName("listing_images")]
    [PetaPoco.PrimaryKey("ImageId")]
    public partial class PropertyImgModel
    {
        [PetaPoco.Column("ListingId")]
        public Int64 ListingId { get; set; }
        [PetaPoco.Column("Filename")]
        public String Filename { get; set; }
        [PetaPoco.Column("Url")]
        public String Url { get; set; }
    }

    public class ListingResp
    {
        public PropertyModel Property { get; set; }
        public IList<PropertyImgModel> Property_Images { get; set; }
    }
}
