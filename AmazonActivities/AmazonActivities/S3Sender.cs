using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using HL7Soup.Integrations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace AmazonActivities
{
    [DisplayName("Send S3")]
    [Parameter("Bucket Name", "Name of target AWS S3 Bucket", isRequired: true)]
    [Parameter("File Name", "Name to give your file in S3", isRequired: true)]
    [Parameter("Region", "Region your S3 Bucket is located in. NOTE: Must be in System Name format (eg: us-west-1)", isRequired: true)]
    [Parameter("Access Key ID", "ID of your AWS Access Key", isRequired: true)]
    [Parameter("Secret Access Key", "Your AWS Secret Access Key", isRequired: true)]
    [InMessage(@"", TypeOfMessages.HL7)]
    [OutMessage(@"Code Execute Successfully", TypeOfMessages.Text)]
    class S3Sender : CustomActivity
    {
        public override void Process(IWorkflowInstance workflowInstance, IActivityInstance activityInstance, Dictionary<string, string> parameters)
        {
            if(activityInstance.Message == null)
            {
                throw new Exception("Error: Activity Message not set.");
            }
            string message = activityInstance.Message.Text;
            byte[] byteArray = Encoding.ASCII.GetBytes(message);
            MemoryStream stream = new MemoryStream(byteArray);
            string bucketName = parameters["Bucket Name"];
            string keyName = parameters["File Name"];
            string inputRegion = parameters["Region"];
            string accessKeyId = parameters["Access Key ID"];
            string secretKey = parameters["Secret Access Key"];
            RegionEndpoint bucketRegion = RegionEndpoint.GetBySystemName(inputRegion);
            var credentials = new Amazon.Runtime.BasicAWSCredentials(accessKeyId, secretKey);
            IAmazonS3 s3Client = new AmazonS3Client(credentials, bucketRegion);
            var fileTransferUtility = new TransferUtility(s3Client);
            fileTransferUtility.Upload(stream, bucketName, keyName);
        }
    }
}
