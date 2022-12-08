using Microsoft.AspNetCore.Http;

namespace issuetracker.Aws;

public interface IS3Client
{
	Task<string> UploadImage(IFormFile model, string name);
	Task DeleteImage(string name);
}