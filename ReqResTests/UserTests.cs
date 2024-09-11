using NUnit.Framework; // Import NUnit testing framework
using RestSharp; // Import RestSharp for HTTP requests
using Newtonsoft.Json.Linq; // Import Newtonsoft.Json to handle JSON responses

namespace ReqResTests // Define the namespace for the test cases
{
    public class UserTests // Define the test class
    {
        private RestClient client; // Declare a RestClient instance that will be used to make API calls

        [SetUp] // This method runs before each test case, initializing the client
        public void Setup()
        {
            // Initialize RestClient with the base URL of the ReqRes API
            client = new RestClient("https://reqres.in/api");
        }

        [Test] // Test case for listing all users
        public void ListUsers_ShouldReturnListOfUsers()
        {
            // Create a GET request to the "/users" endpoint
            var request = new RestRequest("/users", Method.Get);
            
            // Execute the request and store the response
            var response = client.Execute(request);

            // Assert that the request was successful (status code 200)
            Assert.AreEqual(200, (int)response.StatusCode, "Expected status code 200 for list users.");
            
            // Assert that the response contains some content
            Assert.IsNotNull(response.Content, "The response should contain user data.");
        }

        [Test] // Test case for getting a single user by ID
        public void GetSingleUser_ShouldReturnUser_WhenIdIsValid()
        {
            // Create a GET request for the user with ID 2
            var request = new RestRequest("/users/2", Method.Get);
            
            // Execute the request and store the response
            var response = client.Execute(request);
            
            // Parse the response content as a JSON object
            var jsonResponse = JObject.Parse(response.Content);

            // Assert that the status code is 200 (OK)
            Assert.AreEqual(200, (int)response.StatusCode, "Expected status code 200 for single user.");
            
            // Assert that the user ID matches the expected value ("2")
            Assert.AreEqual("2", jsonResponse["data"]["id"].ToString(), "User ID should match.");
        }

        [Test] // Test case for handling a non-existent user
        public void GetSingleUser_ShouldReturnNotFound_WhenIdIsInvalid()
        {
            // Create a GET request for a non-existent user with ID 999
            var request = new RestRequest("/users/999", Method.Get);
            
            // Execute the request and store the response
            var response = client.Execute(request);

            // Assert that the status code is 404 (Not Found)
            Assert.AreEqual(404, (int)response.StatusCode, "Expected status code 404 for invalid user.");
        }

        [Test] // Test case for creating a new user
        public void CreateUser_ShouldReturnCreatedUser()
        {
            // Create a POST request to the "/users" endpoint
            var request = new RestRequest("/users", Method.Post);
            
            // Add a JSON body to the request with user details (name and job)
            request.AddJsonBody(new { name = "morpheus", job = "leader" });
            
            // Execute the request and store the response
            var response = client.Execute(request);
            
            // Parse the response content as a JSON object
            var jsonResponse = JObject.Parse(response.Content);

            // Assert that the status code is 201 (Created)
            Assert.AreEqual(201, (int)response.StatusCode, "Expected status code 201 for resource creation.");
            
            // Assert that the returned name matches "morpheus"
            Assert.AreEqual("morpheus", jsonResponse["name"].ToString(), "The name of the created user is incorrect.");
            
            // Assert that the returned job matches "leader"
            Assert.AreEqual("leader", jsonResponse["job"].ToString(), "The job of the created user is incorrect.");
        }

        [Test] // Test case for updating a user using PUT
        public void UpdateUserWithPut_ShouldReturnUpdatedUser()
        {
            // Create a PUT request to update the user with ID 2
            var request = new RestRequest("/users/2", Method.Put);
            
            // Add a JSON body with the updated user details
            request.AddJsonBody(new { name = "morpheus", job = "zion resident" });
            
            // Execute the request and store the response
            var response = client.Execute(request);
            
            // Parse the response content as a JSON object
            var jsonResponse = JObject.Parse(response.Content);

            // Assert that the status code is 200 (OK)
            Assert.AreEqual(200, (int)response.StatusCode, "Expected status code 200 for user update.");
            
            // Assert that the returned name matches "morpheus"
            Assert.AreEqual("morpheus", jsonResponse["name"].ToString(), "The updated name is incorrect.");
            
            // Assert that the returned job matches "zion resident"
            Assert.AreEqual("zion resident", jsonResponse["job"].ToString(), "The updated job is incorrect.");
        }

        [Test] // Test case for partially updating a user using PATCH
        public void UpdateUserWithPatch_ShouldReturnUpdatedUser()
        {
            // Create a PATCH request to update the user with ID 2
            var request = new RestRequest("/users/2", Method.Patch);
            
            // Add a JSON body with the updated job field
            request.AddJsonBody(new { job = "updated job" });
            
            // Execute the request and store the response
            var response = client.Execute(request);
            
            // Parse the response content as a JSON object
            var jsonResponse = JObject.Parse(response.Content);

            // Assert that the status code is 200 (OK)
            Assert.AreEqual(200, (int)response.StatusCode, "Expected status code 200 for partial update.");
            
            // Assert that the returned job matches "updated job"
            Assert.AreEqual("updated job", jsonResponse["job"].ToString(), "The job update is incorrect.");
        }

        [Test] // Test case for deleting a user
        public void DeleteUser_ShouldReturnNoContent()
        {
            // Create a DELETE request to remove the user with ID 2
            var request = new RestRequest("/users/2", Method.Delete);
            
            // Execute the request and store the response
            var response = client.Execute(request);

            // Assert that the status code is 204 (No Content)
            Assert.AreEqual(204, (int)response.StatusCode, "Expected status code 204 for successful delete.");
        }

        [Test] // Test case for registering a user with valid data
        public void RegisterUser_ShouldReturnSuccess_WhenDataIsValid()
        {
            // Create a POST request to the "/register" endpoint
            var request = new RestRequest("/register", Method.Post);
            
            // Add a JSON body with the user's email and password
            request.AddJsonBody(new { email = "eve.holt@reqres.in", password = "pistol" });
            
            // Execute the request and store the response
            var response = client.Execute(request);
            
            // Parse the response content as a JSON object
            var jsonResponse = JObject.Parse(response.Content);

            // Assert that the status code is 200 (OK)
            Assert.AreEqual(200, (int)response.StatusCode, "Expected status code 200 for successful registration.");
            
            // Assert that a token is returned in the response
            Assert.IsNotNull(jsonResponse["token"], "Registration should return a token.");
        }

        [Test] // Test case for registration failure due to missing password
        public void RegisterUser_ShouldReturnBadRequest_WhenPasswordIsMissing()
        {
            // Create a POST request to the "/register" endpoint
            var request = new RestRequest("/register", Method.Post);
            
            // Add a JSON body with only the email (missing password)
            request.AddJsonBody(new { email = "eve.holt@reqres.in" });
            
            // Execute the request and store the response
            var response = client.Execute(request);

            // Assert that the status code is 400 (Bad Request)
            Assert.AreEqual(400, (int)response.StatusCode, "Expected status code 400 for missing password.");
        }

        [Test] // Test case for a successful login
        public void LoginUser_ShouldReturnSuccess_WhenDataIsValid()
        {
            // Create a POST request to the "/login" endpoint
            var request = new RestRequest("/login", Method.Post);
            
            // Add a JSON body with valid login credentials (email and password)
            request.AddJsonBody(new { email = "eve.holt@reqres.in", password = "cityslicka" });
            
            // Execute the request and store the response
            var response = client.Execute(request);
            
            // Parse the response content as a JSON object
            var jsonResponse = JObject.Parse(response.Content);

            // Assert that the status code is 200 (OK)
            Assert.AreEqual(200, (int)response.StatusCode, "Expected status code 200 for successful login.");
            
            // Assert that a token is returned in the response
            Assert.IsNotNull(jsonResponse["token"], "Login should return a token.");
        }

        [Test] // Test case for login failure due to missing password
        public void LoginUser_ShouldReturnBadRequest_WhenPasswordIsMissing()
        {
            // Create a POST request to the "/login" endpoint
            var request = new RestRequest("/login", Method.Post);
            
            // Add a JSON body with only the email (missing password)
            request.AddJsonBody(new { email = "eve.holt@reqres.in" });
            
            // Execute the request and store the response
            var response = client.Execute(request);

            // Assert that the status code is 400 (Bad Request)
            Assert.AreEqual(400, (int)response.StatusCode, "Expected status code 400 for missing password.");
        }

        [Test] // Test case to simulate a delayed response
        public void GetDelayedResponse_ShouldReturnResponseWithDelay()
        {
            // Create a GET request with a 3-second delay
            var request = new RestRequest("/users?delay=3", Method.Get);
            
            // Execute the request and store the response
            var response = client.Execute(request);

            // Assert that the status code is 200 (OK)
            Assert.AreEqual(200, (int)response.StatusCode, "Expected status code 200 for delayed response.");
            
            // Assert that the response contains some content
            Assert.IsNotNull(response.Content, "The response should contain user data.");
        }

        [TearDown] // This method runs after each test case to clean up
        public void Teardown()
        {
            // Dispose of the RestClient to release resources after each test
            client.Dispose();
        }
    }
}
