namespace Microsoft.Multipoint.Sdk.Samples.Common
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;

    /// <summary>
    /// Contains a collection of CursorAssignment objects
    /// </summary>
    public sealed class CursorAssignments
    {
        private static readonly CursorAssignments instance = new CursorAssignments();

        /// <summary>
        /// Prevents a default instance of the CursorAssignments class from being created
        /// other than the singleton
        /// </summary>
        private CursorAssignments()
        {
            this.teacherAssignment = new CursorAssignment { Cursor = Cursors.Resources.Teacher, MouseId = -1 };

            this.assignments = new List<CursorAssignment>
              {
                  new CursorAssignment {MouseId = -1, Cursor = Cursors.Resources.Player1},
                  new CursorAssignment {MouseId = -1, Cursor = Cursors.Resources.Player2},
                  new CursorAssignment {MouseId = -1, Cursor = Cursors.Resources.Player3},
                  new CursorAssignment {MouseId = -1, Cursor = Cursors.Resources.Player4},
                  new CursorAssignment {MouseId = -1, Cursor = Cursors.Resources.Player5},
                  new CursorAssignment {MouseId = -1, Cursor = Cursors.Resources.Player6},
                  new CursorAssignment {MouseId = -1, Cursor = Cursors.Resources.Player7},
                  new CursorAssignment {MouseId = -1, Cursor = Cursors.Resources.Player8},
                  new CursorAssignment {MouseId = -1, Cursor = Cursors.Resources.Player9},
                  new CursorAssignment {MouseId = -1, Cursor = Cursors.Resources.Player10},
                  new CursorAssignment {MouseId = -1, Cursor = Cursors.Resources.Player11},
                  new CursorAssignment {MouseId = -1, Cursor = Cursors.Resources.Player12},
                  new CursorAssignment {MouseId = -1, Cursor = Cursors.Resources.Player13},
                  new CursorAssignment {MouseId = -1, Cursor = Cursors.Resources.Player14},
                  new CursorAssignment {MouseId = -1, Cursor = Cursors.Resources.Player15},
                  new CursorAssignment {MouseId = -1, Cursor = Cursors.Resources.Player16},
                  new CursorAssignment {MouseId = -1, Cursor = Cursors.Resources.Player17},
                  new CursorAssignment {MouseId = -1, Cursor = Cursors.Resources.Player18},
                  new CursorAssignment {MouseId = -1, Cursor = Cursors.Resources.Player19},
                  new CursorAssignment {MouseId = -1, Cursor = Cursors.Resources.Player20},
                  new CursorAssignment {MouseId = -1, Cursor = Cursors.Resources.Player21},
                  new CursorAssignment {MouseId = -1, Cursor = Cursors.Resources.Player22},
                  new CursorAssignment {MouseId = -1, Cursor = Cursors.Resources.Player23},
                  new CursorAssignment {MouseId = -1, Cursor = Cursors.Resources.Player24},
                  new CursorAssignment {MouseId = -1, Cursor = Cursors.Resources.Player25},
                  new CursorAssignment {MouseId = -1, Cursor = Cursors.Resources.Player26},
                  new CursorAssignment {MouseId = -1, Cursor = Cursors.Resources.Player27},
                  new CursorAssignment {MouseId = -1, Cursor = Cursors.Resources.Player28},
                  new CursorAssignment {MouseId = -1, Cursor = Cursors.Resources.Player29},
                  new CursorAssignment {MouseId = -1, Cursor = Cursors.Resources.Player30},
                  new CursorAssignment {MouseId = -1, Cursor = Cursors.Resources.Player31},
                  new CursorAssignment {MouseId = -1, Cursor = Cursors.Resources.Player32},
                  new CursorAssignment {MouseId = -1, Cursor = Cursors.Resources.Player33},
                  new CursorAssignment {MouseId = -1, Cursor = Cursors.Resources.Player34},
                  new CursorAssignment {MouseId = -1, Cursor = Cursors.Resources.Player35},
                  new CursorAssignment {MouseId = -1, Cursor = Cursors.Resources.Player36},
                  new CursorAssignment {MouseId = -1, Cursor = Cursors.Resources.Player37},
                  new CursorAssignment {MouseId = -1, Cursor = Cursors.Resources.Player38},
                  new CursorAssignment {MouseId = -1, Cursor = Cursors.Resources.Player39},
                  new CursorAssignment {MouseId = -1, Cursor = Cursors.Resources.Player40},
                  new CursorAssignment {MouseId = -1, Cursor = Cursors.Resources.Player41},
                  new CursorAssignment {MouseId = -1, Cursor = Cursors.Resources.Player42},
                  new CursorAssignment {MouseId = -1, Cursor = Cursors.Resources.Player43},
                  new CursorAssignment {MouseId = -1, Cursor = Cursors.Resources.Player44},
                  new CursorAssignment {MouseId = -1, Cursor = Cursors.Resources.Player45},
                  new CursorAssignment {MouseId = -1, Cursor = Cursors.Resources.Player46},
                  new CursorAssignment {MouseId = -1, Cursor = Cursors.Resources.Player47},
                  new CursorAssignment {MouseId = -1, Cursor = Cursors.Resources.Player48},
                  new CursorAssignment {MouseId = -1, Cursor = Cursors.Resources.Player49},
                  new CursorAssignment {MouseId = -1, Cursor = Cursors.Resources.Player50},
                  new CursorAssignment {MouseId = -1, Cursor = Cursors.Resources.Player51},
                  new CursorAssignment {MouseId = -1, Cursor = Cursors.Resources.Player52},
                  new CursorAssignment {MouseId = -1, Cursor = Cursors.Resources.Player53}
              };
        }

        public static CursorAssignments Instance 
        {
            get
            {
                return instance;
            }
        }

        /// <summary>
        /// Gets the mouse that has been assigned to the teacher.
        /// </summary>
        private CursorAssignment teacherAssignment { get; set; }

        private readonly List<CursorAssignment> assignments;

        /// <summary>
        /// The count of available cursors for players
        /// </summary>
        public int PlayerCursorsCount
        {
            get
            {
                return assignments.Count;
            }
        }

        public int TeacherMouseId
        {
            get
            {
                return teacherAssignment.MouseId;
            }
        }

        /// <summary>
        /// Assign cursor to mouse object
        /// </summary>
        /// <param name="mouseObject">The mouse to assign a cursor to</param>
        public void AssignCursorToMouse(DeviceInfo mouseObject)
        {
            if (mouseObject == null)
            {
                throw new ArgumentNullException("mouseObject");
            }

            var cursorAssignment = assignments.FirstOrDefault(ca => ca.MouseId == -1);

            if (cursorAssignment == null)
            {
                throw new InvalidOperationException("Maximum number of mice exceeded");
            }

            var mouseDevice = mouseObject.DeviceVisual;
            Debug.Assert(mouseDevice != null);

            mouseDevice.CursorBitmap = cursorAssignment.Cursor;
            cursorAssignment.MouseId = int.Parse(mouseObject.DeviceId, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Remove cursor from mouse object
        /// </summary>
        /// <param name="mouseObject">The mouse to remove the cursor from</param>
        public void RemoveCursorFromMouse(DeviceInfo mouseObject)
        {
            if (mouseObject == null)
            {
                throw new ArgumentNullException("mouseObject");
            }

            var mouseId = int.Parse(mouseObject.DeviceId, CultureInfo.InvariantCulture);

            if (Instance.teacherAssignment.MouseId == mouseId)
            {
                Instance.teacherAssignment.MouseId = -1;
            }
            else
            {
                var cursorAssignment = assignments.FirstOrDefault(ca => ca.MouseId == mouseId);

                if (cursorAssignment != null)
                {
                    cursorAssignment.MouseId = -1;
                }
            }
        }

        /// <summary>
        /// Associates the supplied mouse with the teacher.
        /// </summary>
        /// <param name="mouseObject">The mouse to assign to the teacher.</param>
        public void AssignTeacherMouse(DeviceInfo mouseObject)
        {
            if (mouseObject == null)
            {
                throw new ArgumentNullException("mouseObject");
            }

            RemoveCursorFromMouse(mouseObject);

            var mouseDevice = mouseObject.DeviceVisual;
            mouseDevice.CursorBitmap = this.teacherAssignment.Cursor;

            int mouseID = int.Parse(mouseObject.DeviceId, CultureInfo.InvariantCulture);
            this.teacherAssignment.MouseId = mouseID;
        }
    }
}