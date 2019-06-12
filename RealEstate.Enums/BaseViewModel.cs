using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RealEstate.Resources;
using System.ComponentModel.DataAnnotations;
using RealEstate.Base.Attributes;

namespace RealEstate.Base
{
    public class BaseViewModel : BaseAbstractModel
    {
        [HiddenInput]
        [JsonProperty("id")]
        [Order(0)]
        [Display(ResourceType = typeof(SharedResource), Name = "Id")]
        public string Id { get; set; }
    }
}