using System;
using System.Globalization;

namespace JapaneseEraSandBox
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var eras = new object[,] {
                {"明治", "明", "M", 1868, 01, 01},
                {"大正", "大", "T", 1912, 07, 30},
                {"昭和", "昭", "S", 1926, 12, 25},
                {"平成", "平", "H", 1989, 01, 09},
                {"臨時", "臨", "R", 2020, 01, 01}};
   
            var culinfo = new CultureInfo("ja-JP", true);
            var cal = new JapaneseCalendar();
   
            JapaneseEraManipulator.UpdateJapaneseEra(cal, eras);
            culinfo.DateTimeFormat.Calendar = cal;
            //DateTimeFormat.Calenderにカレンダーをセットした後にコールすること。
            JapaneseEraManipulator.UpdateJapaneseEra(culinfo.DateTimeFormat, eras);
   
            Console.WriteLine(new DateTime(2019, 12, 31).ToString("ggyy年MM月dd日",culinfo));
            Console.WriteLine(new DateTime(2020, 01, 01).ToString("ggyy年MM月dd日", culinfo));

            Console.ReadKey();
        }
    }
}