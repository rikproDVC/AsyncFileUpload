﻿namespace AspNet.AzureStorage.Sample.Models
{
    public class File
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ContentLength { get; set; }
        public string ContentType { get; set; }
        public string Key { get; set; }
        public string Container { get; set; }
        public byte[] Md5 { get; set; }
    }
}