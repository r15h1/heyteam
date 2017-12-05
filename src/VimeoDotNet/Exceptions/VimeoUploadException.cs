﻿using System;
using System.Runtime.Serialization;
using VimeoDotNet.Net;

namespace VimeoDotNet.Exceptions
{
    [Serializable]
    internal class VimeoUploadException : VimeoApiException
    {
        [NonSerialized] private IUploadRequest _request;

        public VimeoUploadException()
        {
        }

        public VimeoUploadException(IUploadRequest request)
        {
            Request = request;
        }

        public VimeoUploadException(string message)
            : base(message)
        {
        }

        public VimeoUploadException(string message, IUploadRequest request)
            : base(message)
        {
            Request = request;
        }

        public VimeoUploadException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public VimeoUploadException(string message, IUploadRequest request, Exception innerException)
            : base(message, innerException)
        {
            Request = request;
        }

        public IUploadRequest Request
        {
            get { return _request; }
            set { _request = value; }
        }
    }
}