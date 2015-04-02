//-----------------------------------------------------------------------
// <copyright file="CursorAssignment.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Microsoft.Multipoint.Sdk.Samples.Common
{
    using System.Drawing;

    /// <summary>
    /// Provides a link between MouseId and Mouse Cursors to ensure mice 
    ///  maintain their same cursors when other mice attached or removed
    /// </summary>
    internal class CursorAssignment
    {
        internal Bitmap Cursor { get; set; }
        internal int MouseId { get; set; }
    }
}