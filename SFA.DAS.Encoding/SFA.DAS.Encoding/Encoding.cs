﻿namespace SFA.DAS.Encoding
{
    public class Encoding
    {
        public EncodingType EncodingType { get; set; }
        public string Salt { get; set; }
        public int MinHashLength { get; set; }
        public string Alphabet { get; set; }
    }
}