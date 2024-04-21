using RestSharp;
using RestSharp.Authenticators;
using StorySpoilerAPI_Testing.DTO_Models;
using System.Net;
using System.Text.Json;


namespace StorySpoilerAPI_Testing
{
	public class StorySpoilerAPI_Tests
	{
		private RestClient client;
		private static string BASEURL = "https://d5wfqm7y6yb3q.cloudfront.net";
		private static string USERNAME = "testuser3452";
		private static string PASSWORD = "1234567";

		private static string storyId;


		[OneTimeSetUp]
		public void Setup()
		{
			string jwtToken = GetJwtToken(USERNAME, PASSWORD);

			var options = new RestClientOptions(BASEURL)
			{
				Authenticator = new JwtAuthenticator(jwtToken)
			};

			client = new RestClient(options);
		}

		private string GetJwtToken(string username, string password)
		{
			RestClient tempClient = new RestClient(BASEURL);
			var tempRequest = new RestRequest("/api/User/Authentication");
			tempRequest.AddJsonBody(new { username, password });

			var response = tempClient.Execute(tempRequest, Method.Post);

			if (response.IsSuccessStatusCode)
			{
				var responseContent = JsonSerializer.Deserialize<JsonElement>(response.Content);
				var token = responseContent.GetProperty("accessToken").GetString();

				if (string.IsNullOrWhiteSpace(token))
				{
					throw new InvalidOperationException("Token is null or empty.");
				}
				return token;
			}
			else
			{
				throw new InvalidOperationException($"Authentication failed: {response.StatusCode} with data {response.Content}");
			}
		}

		[Test, Order(1)]
		public void CreateANewStoryWithValidData_ShouldSucceed()
		{
			// Arrange
			var requetData = new StoryDto()
			{
				Title = "New Story Created",
				Description = "Description of the new story"
			};
			var request = new RestRequest("/api/Story/Create");
			request.AddJsonBody(requetData);

			// Act
			var response = client.Execute(request, Method.Post);
			var responseJson = JsonSerializer.Deserialize<ResponseDto>(response.Content);

			// Assert
			Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
			Assert.IsNotEmpty(responseJson.StoryId);
			Assert.That(responseJson.Msg, Is.EqualTo("Successfully created!"));

			storyId = responseJson.StoryId;

		}

		[Test, Order(2)]
		public void EditTheCreatedStory_ShouldSuccess()
		{
			// Arrange
			var editedStory = new StoryDto()
			{
				Title = "Edited Story",
				Description = "Description of the edited story"
			};

			var request = new RestRequest($"/api/Story/Edit/{storyId}");
			request.AddJsonBody(editedStory);

			// Act
			var response = client.Execute(request, Method.Put);
			var responseJson = JsonSerializer.Deserialize<ResponseDto>(response.Content);

			// Assert
			Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
			Assert.That(responseJson.Msg, Is.EqualTo("Successfully edited"));

		}

		[Test, Order(3)]
		public void DeleteTheStoryWithValidStoryId_ShouldSucceed()
		{
			// Arrange
			var request = new RestRequest($"/api/Story/Delete/{storyId}");

			// Act
			var response = client.Execute(request, Method.Delete);
			var responseJson = JsonSerializer.Deserialize<ResponseDto>(response.Content);

			//Assert
			Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
			Assert.That(responseJson.Msg, Is.EqualTo("Deleted successfully!"));
		}

		[Test, Order(4)]
		public void CreateStoryWithoutRequiredFields_ShuoldFail()
		{
			// Arrange
			var requetData = new StoryDto()
			{
				Description = "Description of the new story"
			};
			var request = new RestRequest("/api/Story/Create");
			request.AddJsonBody(requetData);

			// Act
			var response = client.Execute(request, Method.Post);

			// Assert
			Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
		}

		[Test, Order(5)]
		public void EditNonExistingStory_ShouldFail()
		{
			// Arrange
			var editedStory = new StoryDto()
			{
				Title = "Edited Story",
				Description = "Description of the edited story"
			};

			var request = new RestRequest("/api/Story/Edit/77777");
			request.AddJsonBody(editedStory);

			// Act
			var response = client.Execute(request, Method.Put);
			var responseJson = JsonSerializer.Deserialize<ResponseDto>(response.Content);

			// Assert
			Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
			Assert.That(responseJson.Msg, Is.EqualTo("No spoilers..."));
		}

		[Test, Order(6)]
		public void DeleteNonExistingStory_ShouldFail()
		{
			// Arrange
			var request = new RestRequest("/api/Story/Delete/77777");

			// Act
			var response = client.Execute(request, Method.Delete);
			var responseJson = JsonSerializer.Deserialize<ResponseDto>(response.Content);

			//Assert
			Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
			Assert.That(responseJson.Msg, Is.EqualTo("Unable to delete this story spoiler!"));
		}
	}
}