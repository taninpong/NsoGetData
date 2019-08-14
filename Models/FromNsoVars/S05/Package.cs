using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NSOWater.HotMigration.Models
{
    public class Package
    {
        /// <summary>
        /// ชนิดของบรรจภุัณฑ์ 
        /// </summary>
        // TODO:
        public string Name { get; set; }

        /// <summary>
        /// ขนาดของภาชนะ 
        /// </summary>
        public double? Size { get; set; }

        /// <summary>
        /// จำนวนที่ซื้อต่อเดือน เพื่ออุปโภคบริโภค  
        /// </summary>
        public int? Drink { get; set; }

        /// <summary>
        /// จำนวนที่ซื้อต่อเดือน เพื่อทำเกษตร 
        /// </summary>        
        public int? Agriculture { get; set; }
        
        /// <summary>
        /// จำนวนที่ซื้อต่อเดือน เพื่อผลิตสินค้า  
        /// </summary>
        public int? Factory { get; set; }

        /// <summary>
        /// จำนวนที่ซื้อต่อเดือน เพื่อการบริการ
        /// </summary>
        public int? Service { get; set; }
    }
}