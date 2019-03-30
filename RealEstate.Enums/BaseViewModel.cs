using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RealEstate.Resources;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Base
{
    public class BaseViewModel : BaseAbstractModel
    {
        [HiddenInput]
        [JsonProperty("id")]
        [Display(ResourceType = typeof(SharedResource), Name = "Id")]
        public string Id { get; set; }
    }
}