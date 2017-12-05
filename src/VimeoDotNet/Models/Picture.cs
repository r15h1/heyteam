﻿using System;
using System.Collections.Generic;

namespace VimeoDotNet.Models {
    /// <summary>
    /// Picture
    /// </summary>
    [Serializable]
    public class Picture {
        /// <summary>
        /// Active
        /// </summary>
        public bool active { get; set; }
        /// <summary>
        /// URI
        /// </summary>
        public string uri { get; set; }
        /// <summary>
        /// Sizes
        /// </summary>
        public List<Size> sizes { get; set; }
        ///
        /// link
        ///
        public string link { get; set; }
        ///
        /// resource_key
        ///
        public string resource_key { get; set; }
    }
}