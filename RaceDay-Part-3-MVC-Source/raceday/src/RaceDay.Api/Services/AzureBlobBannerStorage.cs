using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
namespace RaceDay.Api.Services;
/// <summary>Stores event banners in Azure Blob Storage.</summary>
public sealed class AzureBlobBannerStorage : IBannerStorage {
 private readonly BlobContainerClient container;
 public AzureBlobBannerStorage(IConfiguration config) {
  var connection=config["AzureBlobStorage:ConnectionString"] ?? throw new InvalidOperationException("Azure Blob Storage connection string is missing.");
  container=new BlobContainerClient(connection,config["AzureBlobStorage:Container"] ?? "raceday-banners");
 }
 public async Task<string> SaveBannerAsync(int eventId,Stream content,string contentType,string extension,CancellationToken cancellationToken=default) {
  await container.CreateIfNotExistsAsync(PublicAccessType.Blob,cancellationToken:cancellationToken);
  var blob=container.GetBlobClient($"event-{eventId}-{Guid.NewGuid():N}{extension}");
  await blob.UploadAsync(content,new BlobUploadOptions{HttpHeaders=new BlobHttpHeaders{ContentType=contentType}},cancellationToken);
  return blob.Uri.ToString();
 }
}