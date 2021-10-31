namespace AwsWorker.Config
{
    public class AwsConfig
    {
       public string AccessKey { get; set; }
        public string SecretKey { get; set; }
        public string HostUrl { get; set; }
        public string ServiceUrl { get; set; }
        public string SQSUrl { get; set; }
        public string Topic { get; set; }
    }
}
