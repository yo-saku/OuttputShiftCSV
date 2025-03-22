using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OuttputShiftCSV
{
    internal class PatternMaster
    {
        private XLWorkbook workBook;

        public PatternMaster()
        {
            throw new Exception();
        }
        public PatternMaster(string filePath)
        {
            workBook = new XLWorkbook(filePath);
        }

        public void ReadMasterExcel()
        {
            const int START_ROW_NUM = 13;
            const int PATTERN_CODE_COLOUMN_NUM = 2;
            const int START_TIME_COLOUMN_NUM = 9;
            const int END_TIME_COLOUMN_NUM = 10;

            IXLWorksheet workSheet = workBook.Worksheet("KOTスケジュールパターンリスト");

            //最大1000行まで読み取り
            //※1000行は超えない前提
            for (int i=START_ROW_NUM; i < 1000; i++)
            {
                string patternCode = workSheet.Cell(i, PATTERN_CODE_COLOUMN_NUM).Value.ToString();
                if(patternCode == "")
                {
                    break;
                }
                //baseTimeについてはエクセル上下記フォーマットで記載されている
                //ex) 当日09 時 00 分
                string startBaseTime = workSheet.Cell(i, START_TIME_COLOUMN_NUM).Value.ToString();
                string startTime = startBaseTime.Substring(2,2) + ":" + startBaseTime.Substring(7, 2);
                string endBaseTime = workSheet.Cell(i, END_TIME_COLOUMN_NUM).Value.ToString();
                string endTime = endBaseTime.Substring(2, 2) + ":" + endBaseTime.Substring(7, 2);
                ShiftPattern shiftPattern = new ShiftPattern(patternCode, startTime, endTime);
                Console.WriteLine(shiftPattern.ToString());
            }
        }
    }
}
