using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using webapp.Util.Dto.Configuration;

namespace webapp.Services.Storage
{
    public class S3StorageService : IStorageService
    {
        //AmazonS3Config
        private readonly IAmazonS3 s3Client;
        private readonly S3Configuration config;

        public S3StorageService(S3Configuration config)
        {
            // https://www.digitalocean.com/community/questions/how-to-use-digitalocean-spaces-with-the-aws-s3-sdks?answer=44888
            AmazonS3Config clientConfig = new AmazonS3Config();
            clientConfig.ServiceURL = config.Host;
            s3Client = new AmazonS3Client(config.AccessKey, config.SecretKey, clientConfig);
            this.config = config;
        }

        public async Task Copy(string fromDir, string toDir)
        {
            var fileTransferUtility = new TransferUtility(s3Client);
            await fileTransferUtility.DownloadDirectoryAsync(config.Bucket, fromDir, toDir);
        }

        public async Task<bool> Exists(string fileOrDir)
        {
            var request = new ListObjectsRequest
            {
                BucketName = config.Bucket,
                Prefix = fileOrDir,
                MaxKeys = 1
            };

            var response = await s3Client.ListObjectsAsync(request, CancellationToken.None);
            return response.S3Objects.Any();
        }
    }
}
