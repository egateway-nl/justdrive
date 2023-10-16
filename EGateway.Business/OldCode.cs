using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGateway.Business;

internal class OldCode
{
	#region iShare

	//public async Task<List<TrustedContract>> GetTrustedList()
	//{
	//	var client = new RestClient();
	//	var destinationAddress = "https://scheme.isharetest.net/";
	//	var trustedList = destinationAddress + "trusted_list";
	//	var request = new RestRequest(trustedList, Method.Get) { Timeout = -1 };
	//	request.AddHeader("Authorization", "Bearer " + await GetAccessToken());
	//	request.AddHeader("Do-Not-Sign", "true");
	//	//request.AddHeader("Cookie", "ARRAffinity=402e702c8a33b971f8bb793e6f823a97d10002230894a0caef6306d361e42b98; ARRAffinitySameSite=402e702c8a33b971f8bb793e6f823a97d10002230894a0caef6306d361e42b98");
	//	request.AlwaysMultipartFormData = true;
	//	var response = await client.ExecuteAsync<List<TrustedContract>>(request);
	//	return response.Data ?? new List<TrustedContract>();
	//}

	//private async Task<string> GetAccessToken()
	//{
	//	// private async Task<string> GetAccessToken(string client_id)
	//	// for client_assertion need the following parameters
	//	// for Service Provider ==>	iss = 'w13.client_id'     //aud = 'so.client_id' //x5c = 'w13.public_key' //privateKey = 'w13.private_key'
	//	// or for Service Consumer ==>	iss = 'abc.client_id' aud = 'w13.client_id' x5c = 'abc.public_key' privateKey = 'abc.private_key'
	//	// eval(globals.generateJwsToken)(iss, aud, x5c, privateKey);

	//	//https://scheme.isharetest.net
	//	var accessTokenAddress = $"{_configuration["SchemeOwner.BaseUri"]}/connect/token";
	//	var client = new RestClient();
	//	var request = new RestRequest(accessTokenAddress, Method.Post) { Timeout = -1 };
	//	request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
	//	//request.AddHeader("Cookie", "ARRAffinity=402e702c8a33b971f8bb793e6f823a97d10002230894a0caef6306d361e42b98; ARRAffinitySameSite=402e702c8a33b971f8bb793e6f823a97d10002230894a0caef6306d361e42b98");
	//	request.AddParameter("grant_type", "client_credentials");
	//	request.AddParameter("scope", "iSHARE");
	//	request.AddParameter("client_assertion_type", "urn:ietf:params:oauth:client-assertion-type:jwt-bearer");
	//	// This client_assertion must be created and used here.
	//	request.AddParameter("client_assertion", "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCIsIng1YyI6WyJNSUlFaGpDQ0FtNmdBd0lCQWdJSUZVY0l3c202am5rd0RRWUpLb1pJaHZjTkFRRUxCUUF3U0RFWk1CY0dBMVVFQXd3UWFWTklRVkpGVkdWemRFTkJYMVJNVXpFTk1Bc0dBMVVFQ3d3RVZHVnpkREVQTUEwR0ExVUVDZ3dHYVZOSVFWSkZNUXN3Q1FZRFZRUUdFd0pPVERBZUZ3MHhPREEzTWpVeE5UQXlOVFZhRncweU1EQTNNalF4TlRBeU5UVmFNRDB4R0RBV0JnTlZCQU1NRDBGM1pYTnZiV1VnVjJsa1oyVjBjekVVTUJJR0ExVUVCUk1MVGt3d01EQXdNREF3TURJeEN6QUpCZ05WQkFZVEFrNU1NSUlCSWpBTkJna3Foa2lHOXcwQkFRRUZBQU9DQVE4QU1JSUJDZ0tDQVFFQXVNejBFbVQ2Y3NJNlNrMkt5Nmk1MEVSakgwbndWeVR5VGs1cnBMcko3d1Zhdkx5WGxvdk93djVUUlN2aHE3bXZsTks0OGt5UDZmNE1OQ3BWNk85T3RHdEJZeWhzSmVuU2E4dXRRSlAwd0EzMFpXQmJqalZpTU43dlFBSWM2Q2dHVFZzaU10U2ViYmNRcVJYVmsxeEVvUkt1K0VaaGFQWkNHSW4zT2hwcXBlUzZCMXplc0Radjh4azJUekMrZ0FQUGo1NHNqenRsK0h4TXhITmRpTys5Y3RoNDBMeVp0WEo2UVZYaHZEakpNL1VCbTJjVHpJaXRlUG5SYmdWSW9QdTdwOXNLNElXTWp3NDhuV0Iwa3JmNE1yQ1AyaUx0WjduS0ZxMjZsMkFDaFdrb2RST2MzeUdJc3RkNG5xQWdvNTJpR1IxYXJtNXMxYlJVeXU2TUlpMllMUUlEQVFBQm8zOHdmVEFNQmdOVkhSTUJBZjhFQWpBQU1COEdBMVVkSXdRWU1CYUFGQlk4NXlEcDFwVHZIK1dpOGJqOHZ1cmZMRGVCTUIwR0ExVWRKUVFXTUJRR0NDc0dBUVVGQndNQ0JnZ3JCZ0VGQlFjREJEQWRCZ05WSFE0RUZnUVVXaUNMRzRpNUtRaXBESFpxWGlZTkRYN01XK293RGdZRFZSMFBBUUgvQkFRREFnWGdNQTBHQ1NxR1NJYjNEUUVCQ3dVQUE0SUNBUUNkVWRXMEJCVUhFRHJJNTZZZU9HNXYyaGZLbnNyZ2xBTWcyRlA1cUFJM3RMeSs2OFpuZ2Nxa0Z1T1FkcFB1czF1YTNlL3FLeXNWVnBvS1BPT0RsWWlYbmNPVFhvbHlzcC9jVHlnWHJPVnlaeDlmc2ZlUWxLeWVaeklYOUZxZUJNcVBHUFp3M0dzTUZBb0c1RUZCN0xyYld2NllUZFp5N1JhSEt0STJrQTNuNVlQQ1JNaE8ybzlvMitHd2xDRnlZTzQ5NHIzYVdoNkN3cDJscS9KdkZCRDVRRXhucXF0c0dmVmM1UmkrRkh1UEt4VTQyL1VUQVNHUEFiaWNKMnhUQ2NncUVDbEp2ZGhvZkVLYjBsS3c5UU83T2N2SVdDR1B3VmlNY2ZiSFVGNFNHU2gwVkVnWk1kbDFxeUJVaWtYcXk1LzBYMDdMc1cwSklZTEg3SjV6MDRVc0lwV2RXaVh3OTkyY2F4RTdFSlprK2MrSUZHclVvOUpUYVEzdGVvNVR3MDV0MUNIUTFZUStib0RDRHZYWC9BUVRJK085OWd4b0FENGFzYitjUFhLR08xRHRnRUVIZDltTlc5QUVreFdpbjJtajNtVmJvVkV5eGJOb05NZm1CWDdhT0UwQm15Tk1ZSXNVWExFTjVzMzZOcXZyaWZCL0M1Wkp4ZEhhMVUrWGlyVCt6TWRBdDVEUkxKRjAxNml1bThSSTVsczY0aS9uT2phWFY4NTZFTlFzL05ZQzVGT3hWWXB1dFlXa3dXb3VqTHhJNDlTbXBlR2RzL25teURTbmlRRDZFZUMvU3pTdVFpUzd6bmZpV1JjVFZLMm9Rdk9PV0l5VnlvUEx5UHpnZVJ5QnZGT1dlSkhXV1JJeFpHZnhSS1kweWFRSUxLcklBVWxUWUZpaU9BWnk5dz09Il19.eyJpc3MiOiJFVS5FT1JJLk5MMDAwMDAwMDAyIiwic3ViIjoiRVUuRU9SSS5OTDAwMDAwMDAwMiIsImF1ZCI6WyJFVS5FT1JJLk5MMDAwMDAwMDA0IiwiaHR0cHM6Ly9pc2hhcmUtYXItcWEuYXp1cmV3ZWJzaXRlcy5uZXQvY29ubmVjdC90b2tlbiJdLCJqdGkiOiI5OWFiNWJjYTQxYmI0NWI3OGQyNDJhNDZmMDE1N2I3ZCIsImlhdCI6IjE1NDA4Mjc0MzUiLCJleHAiOjE1NDA4Mjc0NjUsIm5iZiI6MTU0MDgyNzQzNX0.l1uh2Aj-T0ASeVF0st2tUuqcpm-yaSgM57A-_u7zUZ82QYnX9BK4B0d7RKDMM_n-hcXP7HQwn9nRkZcLjntK1bKNvTwTeK7RVaxIXXkW4QS_kTkCO7rPiyIxqpiK5yWxeOHlkM3t9zPbBclMHhhYiB6038A_8b5PA6D5QCE049ASJsLjcd5V4J2JqGZWMyJ7ipiBTQnc_v9BFOsgN_wMqHS_Nm6Wxv1EC8MRPkiKRpNXaSWDv7CflduyhylHKVh06PfITqhMLmbk3eu7NLmlAYKSrI0n-emRp5SMwklkO7chI9kDt9Nq2UcaBjLdZUmMhShMND15aOJs-FXI0I2C1Q");
	//	//"EU.EORI.NL000000003"
	//	request.AddParameter("client_id", _configuration["EGateway.ClientId"]);
	//	var response = await client.ExecuteAsync<LoginContract>(request);
	//	// response Data
	//	// {
	//	//	"access_token": "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYmYiOjE2NDUwMDI3MzksImV4cCI6MTY0NTAwNjMzOSwiaXNzIjoiRVUuRU9SSS5OTDAwMDAwMDAwMCIsImF1ZCI6IkVVLkVPUkkuTkwwMDAwMDAwMDAiLCJjbGllbnRfaWQiOiJFVS5FT1JJLk5MMDAwMDAwMDAzIiwic2NvcGUiOlsiaVNIQVJFIl19.bN5BnM7yiVwu4ItaZP7Zo5lrXuCyQy2N87Gk3pjvYr-Wlu0P52XndPBhf9LkPtj5PtOvWvwSXhvwVrdeCVmFZ2iErNWJ3QR7r7-U6D_VrhdOu33escNni4GC5k2nuYk7kp8d-ra4lGtbHTeyLQrqHJIYT_eL9tUjDfIW-dMo6UnPoV6b8zqZ-tA5b_OvhryKZQlxbMeEF6FUWnAW6eLwWbXj7rSEVO3h2yEKntwq5kRj4OaJG8DIgIaJjHEy55FZ6n0mLY8hJtIjdO52rs73HCwqg0XAqyTN0oVA8aVigR-sg2OpOyLDlw6EMvbbY6OlTPV62kyvpOX0PtqRxIquPg",
	//	//    "expires_in": 3600,
	//	//    "token_type": "Bearer",
	//	//    "scope": "iSHARE"
	//	// }

	//	return response.Data?.access_token ?? "";
	//}



	//private async Task<string> GenerateClientAssertion()
	//{
	//	// for client_assertion need the following parameters
	//	// for Service Provider ==>	iss = 'w13.client_id'     //aud = 'so.client_id' //x5c = 'w13.public_key' //privateKey = 'w13.private_key'
	//	// or for Service Consumer ==>	iss = 'abc.client_id' aud = 'w13.client_id' x5c = 'abc.public_key' privateKey = 'abc.private_key'
	//	// eval(globals.generateJwsToken)(iss, aud, x5c, privateKey);

	//	var destinationAddress = "https://scheme.isharetest.net/";
	//	var accessTokenAddress = destinationAddress + "connect/token";
	//	var client = new RestClient();
	//	var request = new RestRequest(accessTokenAddress, Method.Post) { Timeout = -1 };
	//	request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
	//	//request.AddHeader("Cookie", "ARRAffinity=402e702c8a33b971f8bb793e6f823a97d10002230894a0caef6306d361e42b98; ARRAffinitySameSite=402e702c8a33b971f8bb793e6f823a97d10002230894a0caef6306d361e42b98");
	//	request.AddParameter("grant_type", "client_credentials");
	//	//request.AddParameter("scope", "iSHARE");
	//	//request.AddParameter("client_assertion_type", "urn:ietf:params:oauth:client-assertion-type:jwt-bearer");
	//	//request.AddParameter("client_assertion", "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCIsIng1YyI6WyJNSUlFaGpDQ0FtNmdBd0lCQWdJSUZVY0l3c202am5rd0RRWUpLb1pJaHZjTkFRRUxCUUF3U0RFWk1CY0dBMVVFQXd3UWFWTklRVkpGVkdWemRFTkJYMVJNVXpFTk1Bc0dBMVVFQ3d3RVZHVnpkREVQTUEwR0ExVUVDZ3dHYVZOSVFWSkZNUXN3Q1FZRFZRUUdFd0pPVERBZUZ3MHhPREEzTWpVeE5UQXlOVFZhRncweU1EQTNNalF4TlRBeU5UVmFNRDB4R0RBV0JnTlZCQU1NRDBGM1pYTnZiV1VnVjJsa1oyVjBjekVVTUJJR0ExVUVCUk1MVGt3d01EQXdNREF3TURJeEN6QUpCZ05WQkFZVEFrNU1NSUlCSWpBTkJna3Foa2lHOXcwQkFRRUZBQU9DQVE4QU1JSUJDZ0tDQVFFQXVNejBFbVQ2Y3NJNlNrMkt5Nmk1MEVSakgwbndWeVR5VGs1cnBMcko3d1Zhdkx5WGxvdk93djVUUlN2aHE3bXZsTks0OGt5UDZmNE1OQ3BWNk85T3RHdEJZeWhzSmVuU2E4dXRRSlAwd0EzMFpXQmJqalZpTU43dlFBSWM2Q2dHVFZzaU10U2ViYmNRcVJYVmsxeEVvUkt1K0VaaGFQWkNHSW4zT2hwcXBlUzZCMXplc0Radjh4azJUekMrZ0FQUGo1NHNqenRsK0h4TXhITmRpTys5Y3RoNDBMeVp0WEo2UVZYaHZEakpNL1VCbTJjVHpJaXRlUG5SYmdWSW9QdTdwOXNLNElXTWp3NDhuV0Iwa3JmNE1yQ1AyaUx0WjduS0ZxMjZsMkFDaFdrb2RST2MzeUdJc3RkNG5xQWdvNTJpR1IxYXJtNXMxYlJVeXU2TUlpMllMUUlEQVFBQm8zOHdmVEFNQmdOVkhSTUJBZjhFQWpBQU1COEdBMVVkSXdRWU1CYUFGQlk4NXlEcDFwVHZIK1dpOGJqOHZ1cmZMRGVCTUIwR0ExVWRKUVFXTUJRR0NDc0dBUVVGQndNQ0JnZ3JCZ0VGQlFjREJEQWRCZ05WSFE0RUZnUVVXaUNMRzRpNUtRaXBESFpxWGlZTkRYN01XK293RGdZRFZSMFBBUUgvQkFRREFnWGdNQTBHQ1NxR1NJYjNEUUVCQ3dVQUE0SUNBUUNkVWRXMEJCVUhFRHJJNTZZZU9HNXYyaGZLbnNyZ2xBTWcyRlA1cUFJM3RMeSs2OFpuZ2Nxa0Z1T1FkcFB1czF1YTNlL3FLeXNWVnBvS1BPT0RsWWlYbmNPVFhvbHlzcC9jVHlnWHJPVnlaeDlmc2ZlUWxLeWVaeklYOUZxZUJNcVBHUFp3M0dzTUZBb0c1RUZCN0xyYld2NllUZFp5N1JhSEt0STJrQTNuNVlQQ1JNaE8ybzlvMitHd2xDRnlZTzQ5NHIzYVdoNkN3cDJscS9KdkZCRDVRRXhucXF0c0dmVmM1UmkrRkh1UEt4VTQyL1VUQVNHUEFiaWNKMnhUQ2NncUVDbEp2ZGhvZkVLYjBsS3c5UU83T2N2SVdDR1B3VmlNY2ZiSFVGNFNHU2gwVkVnWk1kbDFxeUJVaWtYcXk1LzBYMDdMc1cwSklZTEg3SjV6MDRVc0lwV2RXaVh3OTkyY2F4RTdFSlprK2MrSUZHclVvOUpUYVEzdGVvNVR3MDV0MUNIUTFZUStib0RDRHZYWC9BUVRJK085OWd4b0FENGFzYitjUFhLR08xRHRnRUVIZDltTlc5QUVreFdpbjJtajNtVmJvVkV5eGJOb05NZm1CWDdhT0UwQm15Tk1ZSXNVWExFTjVzMzZOcXZyaWZCL0M1Wkp4ZEhhMVUrWGlyVCt6TWRBdDVEUkxKRjAxNml1bThSSTVsczY0aS9uT2phWFY4NTZFTlFzL05ZQzVGT3hWWXB1dFlXa3dXb3VqTHhJNDlTbXBlR2RzL25teURTbmlRRDZFZUMvU3pTdVFpUzd6bmZpV1JjVFZLMm9Rdk9PV0l5VnlvUEx5UHpnZVJ5QnZGT1dlSkhXV1JJeFpHZnhSS1kweWFRSUxLcklBVWxUWUZpaU9BWnk5dz09Il19.eyJpc3MiOiJFVS5FT1JJLk5MMDAwMDAwMDAyIiwic3ViIjoiRVUuRU9SSS5OTDAwMDAwMDAwMiIsImF1ZCI6WyJFVS5FT1JJLk5MMDAwMDAwMDA0IiwiaHR0cHM6Ly9pc2hhcmUtYXItcWEuYXp1cmV3ZWJzaXRlcy5uZXQvY29ubmVjdC90b2tlbiJdLCJqdGkiOiI5OWFiNWJjYTQxYmI0NWI3OGQyNDJhNDZmMDE1N2I3ZCIsImlhdCI6IjE1NDA4Mjc0MzUiLCJleHAiOjE1NDA4Mjc0NjUsIm5iZiI6MTU0MDgyNzQzNX0.l1uh2Aj-T0ASeVF0st2tUuqcpm-yaSgM57A-_u7zUZ82QYnX9BK4B0d7RKDMM_n-hcXP7HQwn9nRkZcLjntK1bKNvTwTeK7RVaxIXXkW4QS_kTkCO7rPiyIxqpiK5yWxeOHlkM3t9zPbBclMHhhYiB6038A_8b5PA6D5QCE049ASJsLjcd5V4J2JqGZWMyJ7ipiBTQnc_v9BFOsgN_wMqHS_Nm6Wxv1EC8MRPkiKRpNXaSWDv7CflduyhylHKVh06PfITqhMLmbk3eu7NLmlAYKSrI0n-emRp5SMwklkO7chI9kDt9Nq2UcaBjLdZUmMhShMND15aOJs-FXI0I2C1Q");
	//	//request.AddParameter("client_id", "EU.EORI.NL000000003");
	//	var response = await client.ExecuteAsync<LoginContract>(request);
	//	return response.Data?.access_token ?? "";
	//}

	//private async Task<string> CheckServiceConsumerAtSchemeOwner()
	//{
	//	var client = new RestClient();
	//	//client.Timeout = -1;
	//	var request = new RestRequest("https://scheme.isharetest.net/parties?eori=EU.EORI.NL000000001&certificate_subject_name=C=NL, SERIALNUMBER=EU.EORI.NL000000001, CN=ABC Trucking", Method.Get) { Timeout = -1 };
	//	// 
	//	//request.AddHeader("Authorization", "Bearer eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYmYiOjE2NDUwMDI3MzksImV4cCI6MTY0NTAwNjMzOSwiaXNzIjoiRVUuRU9SSS5OTDAwMDAwMDAwMCIsImF1ZCI6IkVVLkVPUkkuTkwwMDAwMDAwMDAiLCJjbGllbnRfaWQiOiJFVS5FT1JJLk5MMDAwMDAwMDAzIiwic2NvcGUiOlsiaVNIQVJFIl19.bN5BnM7yiVwu4ItaZP7Zo5lrXuCyQy2N87Gk3pjvYr-Wlu0P52XndPBhf9LkPtj5PtOvWvwSXhvwVrdeCVmFZ2iErNWJ3QR7r7-U6D_VrhdOu33escNni4GC5k2nuYk7kp8d-ra4lGtbHTeyLQrqHJIYT_eL9tUjDfIW-dMo6UnPoV6b8zqZ-tA5b_OvhryKZQlxbMeEF6FUWnAW6eLwWbXj7rSEVO3h2yEKntwq5kRj4OaJG8DIgIaJjHEy55FZ6n0mLY8hJtIjdO52rs73HCwqg0XAqyTN0oVA8aVigR-sg2OpOyLDlw6EMvbbY6OlTPV62kyvpOX0PtqRxIquPg");
	//	//request.AddHeader("Do-Not-Sign", "true");
	//	request.AddHeader("Authorization", "Bearer " + await GetAccessToken());
	//	request.AddHeader("Do-Not-Sign", "true");
	//	request.AddHeader("Content-Type", "true");
	//	request.AddParameter("grant_type", "client_credentials");
	//	request.AddParameter("scope", "iSHARE");
	//	request.AddParameter("client_assertion_type", "urn:ietf:params:oauth:client-assertion-type:jwt-bearer");
	//	// This client_assertion must be created and used here.
	//	request.AddParameter("client_assertion", "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCIsIng1YyI6WyJNSUlFaGpDQ0FtNmdBd0lCQWdJSUZVY0l3c202am5rd0RRWUpLb1pJaHZjTkFRRUxCUUF3U0RFWk1CY0dBMVVFQXd3UWFWTklRVkpGVkdWemRFTkJYMVJNVXpFTk1Bc0dBMVVFQ3d3RVZHVnpkREVQTUEwR0ExVUVDZ3dHYVZOSVFWSkZNUXN3Q1FZRFZRUUdFd0pPVERBZUZ3MHhPREEzTWpVeE5UQXlOVFZhRncweU1EQTNNalF4TlRBeU5UVmFNRDB4R0RBV0JnTlZCQU1NRDBGM1pYTnZiV1VnVjJsa1oyVjBjekVVTUJJR0ExVUVCUk1MVGt3d01EQXdNREF3TURJeEN6QUpCZ05WQkFZVEFrNU1NSUlCSWpBTkJna3Foa2lHOXcwQkFRRUZBQU9DQVE4QU1JSUJDZ0tDQVFFQXVNejBFbVQ2Y3NJNlNrMkt5Nmk1MEVSakgwbndWeVR5VGs1cnBMcko3d1Zhdkx5WGxvdk93djVUUlN2aHE3bXZsTks0OGt5UDZmNE1OQ3BWNk85T3RHdEJZeWhzSmVuU2E4dXRRSlAwd0EzMFpXQmJqalZpTU43dlFBSWM2Q2dHVFZzaU10U2ViYmNRcVJYVmsxeEVvUkt1K0VaaGFQWkNHSW4zT2hwcXBlUzZCMXplc0Radjh4azJUekMrZ0FQUGo1NHNqenRsK0h4TXhITmRpTys5Y3RoNDBMeVp0WEo2UVZYaHZEakpNL1VCbTJjVHpJaXRlUG5SYmdWSW9QdTdwOXNLNElXTWp3NDhuV0Iwa3JmNE1yQ1AyaUx0WjduS0ZxMjZsMkFDaFdrb2RST2MzeUdJc3RkNG5xQWdvNTJpR1IxYXJtNXMxYlJVeXU2TUlpMllMUUlEQVFBQm8zOHdmVEFNQmdOVkhSTUJBZjhFQWpBQU1COEdBMVVkSXdRWU1CYUFGQlk4NXlEcDFwVHZIK1dpOGJqOHZ1cmZMRGVCTUIwR0ExVWRKUVFXTUJRR0NDc0dBUVVGQndNQ0JnZ3JCZ0VGQlFjREJEQWRCZ05WSFE0RUZnUVVXaUNMRzRpNUtRaXBESFpxWGlZTkRYN01XK293RGdZRFZSMFBBUUgvQkFRREFnWGdNQTBHQ1NxR1NJYjNEUUVCQ3dVQUE0SUNBUUNkVWRXMEJCVUhFRHJJNTZZZU9HNXYyaGZLbnNyZ2xBTWcyRlA1cUFJM3RMeSs2OFpuZ2Nxa0Z1T1FkcFB1czF1YTNlL3FLeXNWVnBvS1BPT0RsWWlYbmNPVFhvbHlzcC9jVHlnWHJPVnlaeDlmc2ZlUWxLeWVaeklYOUZxZUJNcVBHUFp3M0dzTUZBb0c1RUZCN0xyYld2NllUZFp5N1JhSEt0STJrQTNuNVlQQ1JNaE8ybzlvMitHd2xDRnlZTzQ5NHIzYVdoNkN3cDJscS9KdkZCRDVRRXhucXF0c0dmVmM1UmkrRkh1UEt4VTQyL1VUQVNHUEFiaWNKMnhUQ2NncUVDbEp2ZGhvZkVLYjBsS3c5UU83T2N2SVdDR1B3VmlNY2ZiSFVGNFNHU2gwVkVnWk1kbDFxeUJVaWtYcXk1LzBYMDdMc1cwSklZTEg3SjV6MDRVc0lwV2RXaVh3OTkyY2F4RTdFSlprK2MrSUZHclVvOUpUYVEzdGVvNVR3MDV0MUNIUTFZUStib0RDRHZYWC9BUVRJK085OWd4b0FENGFzYitjUFhLR08xRHRnRUVIZDltTlc5QUVreFdpbjJtajNtVmJvVkV5eGJOb05NZm1CWDdhT0UwQm15Tk1ZSXNVWExFTjVzMzZOcXZyaWZCL0M1Wkp4ZEhhMVUrWGlyVCt6TWRBdDVEUkxKRjAxNml1bThSSTVsczY0aS9uT2phWFY4NTZFTlFzL05ZQzVGT3hWWXB1dFlXa3dXb3VqTHhJNDlTbXBlR2RzL25teURTbmlRRDZFZUMvU3pTdVFpUzd6bmZpV1JjVFZLMm9Rdk9PV0l5VnlvUEx5UHpnZVJ5QnZGT1dlSkhXV1JJeFpHZnhSS1kweWFRSUxLcklBVWxUWUZpaU9BWnk5dz09Il19.eyJpc3MiOiJFVS5FT1JJLk5MMDAwMDAwMDAyIiwic3ViIjoiRVUuRU9SSS5OTDAwMDAwMDAwMiIsImF1ZCI6WyJFVS5FT1JJLk5MMDAwMDAwMDA0IiwiaHR0cHM6Ly9pc2hhcmUtYXItcWEuYXp1cmV3ZWJzaXRlcy5uZXQvY29ubmVjdC90b2tlbiJdLCJqdGkiOiI5OWFiNWJjYTQxYmI0NWI3OGQyNDJhNDZmMDE1N2I3ZCIsImlhdCI6IjE1NDA4Mjc0MzUiLCJleHAiOjE1NDA4Mjc0NjUsIm5iZiI6MTU0MDgyNzQzNX0.l1uh2Aj-T0ASeVF0st2tUuqcpm-yaSgM57A-_u7zUZ82QYnX9BK4B0d7RKDMM_n-hcXP7HQwn9nRkZcLjntK1bKNvTwTeK7RVaxIXXkW4QS_kTkCO7rPiyIxqpiK5yWxeOHlkM3t9zPbBclMHhhYiB6038A_8b5PA6D5QCE049ASJsLjcd5V4J2JqGZWMyJ7ipiBTQnc_v9BFOsgN_wMqHS_Nm6Wxv1EC8MRPkiKRpNXaSWDv7CflduyhylHKVh06PfITqhMLmbk3eu7NLmlAYKSrI0n-emRp5SMwklkO7chI9kDt9Nq2UcaBjLdZUmMhShMND15aOJs-FXI0I2C1Q");
	//	request.AddParameter("client_id", "EU.EORI.NL000000001");
	//	var response = await client.ExecuteAsync(request);
	//	//Console.WriteLine(response.Content);
	//	return response.Content ?? "";

	//}

	#endregion
}