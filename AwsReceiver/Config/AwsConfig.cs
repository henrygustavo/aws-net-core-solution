namespace AwsReceiver.Config
{
    public class AwsConfig
    {
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
        public string HostUrl { get; set; }
        public string ServiceUrl { get; set; }
        public string SQSUrl { get; set; }
        public string MessageEventTopic { get; set; }
        public string UserEventTopic { get; set; }
        public string Queue { get; set; }
    }
}
