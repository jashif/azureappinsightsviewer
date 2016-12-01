// ***********************************************************************
// Assembly         : AzureInsightsAppLibrary
// Author           : Jashif
// Created          : 12-01-2016
//
// Last Modified By : Jashif
// Last Modified On : 12-01-2016
// ***********************************************************************
// <copyright file="TelemetrySettings.cs" company="Jashif">
//     Copyright ©  2016
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureInsightsAppLibrary
{
    /// <summary>
    /// Class TelemetrySettings.
    /// </summary>
    public class TelemetrySettings
    {
        /// <summary>
        /// Gets or sets the application key.
        /// </summary>
        /// <value>The application key.</value>
        public static string APP_KEY { get; set; }

        /// <summary>
        /// Gets or sets the application identifier.
        /// </summary>
        /// <value>The application identifier.</value>
        public static string APP_ID { get; set; }
    }
}
