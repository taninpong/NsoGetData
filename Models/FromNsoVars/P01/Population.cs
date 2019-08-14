using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NSOWater.HotMigration.Models
{
    /// <summary>
    /// สำมะโนประชากร
    /// </summary>
    public class Population
    {
        /// <summary>
        /// ครัวเรือนนี้เป็นครัวเรือนสถาบันหรือไม่
        /// </summary>
        public bool? Skip { get; set; }
        
        /// <summary>
        /// จำนวนคน
        /// </summary>
        public int? PersonCount { get; set; }

        /// <summary>
        /// ข้อมูลบุคคล
        /// </summary>
        public List<Person> Persons { get; set; }

    }
}