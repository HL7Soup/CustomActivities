using Azure.Storage.Blobs;
using HL7Soup.Integrations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace AzureActivities
{
    [DisplayName("Send Blob")]
    [Parameter("Connection String", "Connection String to your Azure Blob Storage account.", isRequired: true)]
    [Parameter("Container Name", "Name of your Azure Blob Container to upload the file to.", isRequired: true)]
    [Parameter("File Name", "Name to give your file in Blob Storage.", isRequired: true)]
    [InMessage(@"", TypeOfMessages.HL7)]
    [OutMessage(@"Code Executed Successfully", TypeOfMessages.Text)]
    internal class BlobSender : CustomActivity
    {
        public override void Process(IWorkflowInstance workflowInstance, IActivityInstance activityInstance, Dictionary<string, string> parameters)
        {
            string connectionString = parameters["Connection String"];
            string containerName = parameters["Container Name"];
            string containerLower = containerName.ToLower();
            string fileName = parameters["File Name"];
            string message = activityInstance.Message.Text;
            byte[] byteArray = Encoding.ASCII.GetBytes(message);
            MemoryStream stream = new MemoryStream(byteArray);

            BlobContainerClient container = new BlobContainerClient(connectionString, containerLower);
            var createResponse = container.CreateIfNotExists();
            if (createResponse != null && createResponse.GetRawResponse().Status == 201)
                container.SetAccessPolicy(Azure.Storage.Blobs.Models.PublicAccessType.Blob);

            BlobClient blob = container.GetBlobClient(fileName);
            blob.DeleteIfExists(Azure.Storage.Blobs.Models.DeleteSnapshotsOption.IncludeSnapshots);
            blob.Upload(stream);
        }
    }
}
