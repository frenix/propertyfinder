﻿/*
 * Created by Fuego, Inc. 
 * File  :   AgentModel.cs
 * Author:    Efren Duran
 * Date: 3/17/2015
 * Time: 12:50 PM
 * 
 */
using System;

namespace OHWebService.Models
{
    /// <summary>
    /// Class that defines Agent and map it to db
    /// </summary>
    
    [PetaPoco.TableName("property_agent")]
	[PetaPoco.PrimaryKey("AgentId")]
    public partial class AgentModel
    {
        [PetaPoco.Column("AgentId")]
		public Int64 AgentId { get; set; }
		[PetaPoco.Column("FirstName")]
		public String FirstName { get; set; }
		[PetaPoco.Column("LastName")]
		public String LastName { get; set; }
		[PetaPoco.Column("EmailAddress")]
		public String EmailAddress { get; set; }
		[PetaPoco.Column("Password")]
		public String Password { get; set; }
		[PetaPoco.Column("AuthKey")]
		public String AuthKey { get; set; }
		[PetaPoco.Column("ConfirmFlg")]
		public String ConfirmFlg { get; set; }
		[PetaPoco.Column("ProfileFileName")]
		public String ProfileFileName { get; set; }
		[PetaPoco.Column("ProfileUrl")]
		public String ProfileUrl { get; set; }
    }


    public partial class AgentModelToken
    {
		public String token { get; set; }
    }

}
