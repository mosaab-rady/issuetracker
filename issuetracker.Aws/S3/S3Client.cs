using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace issuetracker.Aws;
public class S3Client : IS3Client
{
	private readonly IConfiguration configuration;
	public AmazonS3Client amazonS3 { get; set; }
	public S3Client(IConfiguration configuration)
	{
		this.configuration = configuration;
		amazonS3 = new AmazonS3Client(
			awsAccessKeyId: configuration["AWS:AccessKeyId"],
			awsSecretAccessKey: configuration["AWS:SecretAccessKey"],
			region: Amazon.RegionEndpoint.GetBySystemName(configuration["AWS:Region"]));
	}

	public async Task<string> UploadImage(IFormFile model, string name)
	{
		string uniqueFileName = $"{name}-{Guid.NewGuid().ToString()}.jpeg";

		using var image = Image.Load(model.OpenReadStream());
		Stream outStream = new MemoryStream();
		image.Save(outStream, new JpegEncoder());

		var request = new PutObjectRequest
		{
			BucketName = "mosaabportofoliobucket",
			Key = uniqueFileName,
			InputStream = outStream
		};


		request.Metadata.Add("Content-Type", "image/jpeg");
		await amazonS3.PutObjectAsync(request);

		return uniqueFileName;
	}

	public async Task DeleteImage(string name)
	{
		await amazonS3.DeleteObjectAsync("mosaabportofoliobucket", name);

	}
}
