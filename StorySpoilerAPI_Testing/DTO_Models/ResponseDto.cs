using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace StorySpoilerAPI_Testing.DTO_Models
{
	public class ResponseDto
	{
		[JsonPropertyName("msg")]
        public string Msg { get; set; }

		[JsonPropertyName("storyId")]
		public string StoryId { get; set; }
	}
}
