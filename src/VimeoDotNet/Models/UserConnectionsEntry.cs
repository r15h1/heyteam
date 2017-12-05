﻿using System.Collections.Generic;

namespace VimeoDotNet.Models
{
    /// <summary>
    /// User connection
    /// </summary>
    public class UserConnectionsEntry
    {
        /// <summary>
        /// URI
        /// </summary>
        public string uri { get; set; }
        
        /// <summary>
        /// Options
        /// </summary>
        public List<string> options { get; set; }

        /// <summary>
        /// Total
        /// </summary>
        public int total { get; set; }
    }
}