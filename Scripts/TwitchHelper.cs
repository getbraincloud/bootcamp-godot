// Copyright 2023 bitHeads, Inc. All Rights Reserved.

using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using BrainCloud.JsonFx.Json;
using BrainCloud.UnityWebSocketsForWebGL.WebSocketSharp;
using System.Runtime;


public class TwitchHelper
{
	public delegate void AuthorizationGranted(string accessToken, string email, string username);
	public delegate void AuthorizationError(string errorMessage);
	public delegate void AuthorizationDenied();

	private AuthorizationGranted m_AuthorizationGranted = null;
	private AuthorizationError m_AuthorizationError = null;
	private AuthorizationDenied m_AuthorizationDenied = null;
	private string m_ClientId;
	private string m_ClientSecret;
	private string m_RedirectUrl;
	private string m_AuthState;
	private string m_AccessToken;
	private string m_UserEmail;
	private string m_Username;
	private System.Threading.SynchronizationContext m_SyncContext = null;
	private HttpListener m_HttpListener;

	public string AccessToken
	{
		get { return m_AccessToken; }
	}

	public string UserEmail
	{
		get { return m_UserEmail; }
	}

	public string Username
	{
		get { return m_Username; }
	}

	public TwitchHelper(string clientId, string clientSecret, string redirectUrl)
	{
		m_ClientId = clientId;
		m_ClientSecret = clientSecret;
		m_RedirectUrl = redirectUrl;
		m_AuthState = "";
		m_AccessToken = "";
		m_UserEmail = "";
		m_Username = "";
	}

	public async Task<bool> ValidateAccessToken(string accessToken)
	{
		string apiUrl = "https://id.twitch.tv/oauth2/validate";

		HttpClient http = new HttpClient();
		http.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

		HttpResponseMessage response = await http.GetAsync(apiUrl);
		return response.StatusCode == System.Net.HttpStatusCode.OK;
	}

	public async Task<string> GetUser(string accessToken)
	{
		string apiUrl = "https://api.twitch.tv/helix/users";

		HttpClient http = new HttpClient();
		http.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
		http.DefaultRequestHeaders.Add("Client-Id", m_ClientId);

		HttpResponseMessage response = await http.GetAsync(apiUrl);
		if (response.StatusCode == System.Net.HttpStatusCode.OK)
		{
			string json = await response.Content.ReadAsStringAsync();
			return json;
		}
		else
			return null;
	}

	public async Task<string> GetAcessToken(string authCode)
	{
		string apiUrl = "https://id.twitch.tv/oauth2/token" +
						"?client_id=" + m_ClientId +
						"&client_secret=" + m_ClientSecret +
						"&code=" + authCode +
						"&grant_type=authorization_code" +
						"&redirect_uri=" + Godot.StringExtensions.URIEncode(m_RedirectUrl);

		HttpClient http = new HttpClient();
		HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Post, apiUrl);
		httpRequest.Headers.TryAddWithoutValidation("Content-Type", "application/json");

		HttpResponseMessage response = await http.SendAsync(httpRequest);
		if (response.StatusCode == System.Net.HttpStatusCode.OK)
		{
			string json = await response.Content.ReadAsStringAsync();
			return json;
		}
		else
			return null;
	}

	public void StartLocalCallbackServer(string authState, AuthorizationGranted authorizationGranted, AuthorizationError authorizationError, AuthorizationDenied authorizationDenied)
	{
		// Initialize the Twitch properties
		m_AccessToken = "";
		m_UserEmail = "";
		m_Username = "";
		m_AuthState = authState;
		m_AuthorizationGranted = authorizationGranted;
		m_AuthorizationError = authorizationError;
		m_AuthorizationDenied = authorizationDenied;
		m_SyncContext = System.Threading.SynchronizationContext.Current;

		// Create and start the HttpListener for the Twitch redirect callback
		m_HttpListener = new HttpListener();
		m_HttpListener.Prefixes.Add(m_RedirectUrl);
		m_HttpListener.Start();
		m_HttpListener.BeginGetContext(new AsyncCallback(CallbackHttpRequest), m_HttpListener);
	}

	public void StopLocalCallbackServer()
	{
		m_HttpListener.Stop();
	}

	private void CallbackHttpRequest(IAsyncResult result)
	{
		bool hasError = false;
		string errorMessage = "";
		HttpListener httpListener;
		HttpListenerContext httpContext;
		HttpListenerRequest httpRequest;
		HttpListenerResponse httpResponse;

		// Get the reference to the HttpListener
		httpListener = (HttpListener)result.AsyncState;

		// Fetch the context object
		httpContext = httpListener.EndGetContext(result);

		// The context object has the request object for us, that holds details about the incoming request
		httpRequest = httpContext.Request;

		string code = httpRequest.QueryString.Get("code");
		string state = httpRequest.QueryString.Get("state");
		string error = httpRequest.QueryString.Get("error");

		// check that we got a code value and the state value matches our remembered one
		if(string.IsNullOrEmpty(error))
		{
			if (code.Length > 0 && state == m_AuthState)
			{
				Godot.GD.Print("TwitchHelper recieved auth code: " + code);

				Task<string> responseData = GetAcessToken(code);
				while (!responseData.IsCompleted) { }

				Godot.GD.Print("TwitchHelper recieved access token: " + responseData.Result);

				Dictionary<string, object> jsonData = JsonReader.Deserialize<Dictionary<string, object>>(responseData.Result);
				m_AccessToken = jsonData["access_token"].ToString();
			}
			else
			{
				hasError = true;

				if(state != m_AuthState && code.Length > 0)
					errorMessage = "Error: The Twitch auth state value that was echoed back is different that expected.";
				else
					errorMessage = "Error: The Twitch auth code string that was returned is empty.";
			}
		}
		else
		{
			hasError = true;
			errorMessage = "An error was returned while trying to retrieve the Twitch auth code: " + error;
		}

		// Build a response to send an "ok" back to the browser for the user to see
		httpResponse = httpContext.Response;
		string responseString = "<html><body><b>DONE!</b><br>(You can close this tab/window now)</body></html>";
		byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);

		// Send the output to the client browser
		httpResponse.ContentLength64 = buffer.Length;
		System.IO.Stream output = httpResponse.OutputStream;
		output.Write(buffer, 0, buffer.Length);
		output.Close();

		// The HTTP listener has served it's purpose, shut it down
		httpListener.Stop();

		// Is the access token empty or not
		if (!string.IsNullOrEmpty(m_AccessToken))
		{
			Godot.GD.Print("Validating Twitch access token...");

			// Validate the access token with the Twitch server
			Task<bool> accessTokenValidation = ValidateAccessToken(m_AccessToken);
			while (!accessTokenValidation.IsCompleted) { }

			if (accessTokenValidation.Result)
			{
				Godot.GD.Print("Twitch access token Validated");

				// Get the user's data from the Twitch resource server, including the email address
				// Wait for the task to complete
				Task<string> responseData = GetUser(m_AccessToken);
				while (!responseData.IsCompleted) { }

				Godot.GD.Print("TwitchHelper recieved user data: " + responseData.Result);

				// Parse the Json data
				Dictionary<string, object> jsonData = JsonReader.Deserialize<Dictionary<string, object>>(responseData.Result);
				Array userDataArray = jsonData["data"] as Array;

				// Ensure there is data in the user array
				if (userDataArray.Length > 0)
				{
					Dictionary<string, object> userData = userDataArray.GetValue(0) as Dictionary<string, object>;

					// Parse the user's email and display name
					m_UserEmail = userData["email"].ToString();
					m_Username = userData["display_name"].ToString();
				}
				else
				{
					hasError = true;
					errorMessage = "Error: The request Twitch user scope data was returned and did not contain any data.";
				}

				// If success == true, then authentication with Twitch succeeded,
				// invoke the authorization granted delegate
				if (hasError == false)
				{
					// This is most likely not the main thread, use the SynchronizationContext to
					// ensure the callback is delegate is invoked on the main thread
					m_SyncContext.Post(_ =>
					{
						if (m_AuthorizationGranted != null)
							m_AuthorizationGranted(m_AccessToken, m_UserEmail, m_Username);
					}, null);

                    return;
				}
			}
			else
			{
				hasError = true;
				errorMessage = "Error: Failed to validate the Twitch Access Token.";
			}
		}
		else
		{
			hasError = true;
			errorMessage = "Error: The Twitch Access Token string that was returned is empty.";
		}

		// If success == false, if so authentication with Twitch failed,
		// invoke the authorization denied delegate
		if (hasError == false)
		{
			// This is most likely not the main thread, use the SynchronizationContext to
			// ensure the callback is delegate is invoked on the main thread
			m_SyncContext.Post(_ =>
			{
				if (m_AuthorizationDenied != null)
					m_AuthorizationDenied();
			}, null);
		}
		else
		{
			if (m_AuthorizationError != null)
				m_AuthorizationError(errorMessage);
		}
	}
}
