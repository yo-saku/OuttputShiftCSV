using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClosedXML.Excel;

namespace OuttputShiftCSV
{
    internal class ShiftExcel
    {

        const int START_COLUMN_NUM = 4;

        private XLWorkbook workBook;
        private PatternMaster patternMaster;
        private List<Employee> employeeList;

        public ShiftExcel()
        {
            throw new NotImplementedException();
        }

        public ShiftExcel(string filePath, PatternMaster patternMaster) 
        {
            this.workBook = new XLWorkbook(filePath);
            this.patternMaster = patternMaster;
            this.employeeList = new List<Employee>();
        }

        /// <summary>
        /// シフト情報記載のエクセル読み込み
        /// </summary>
        /// <exception cref="Exception"></exception>
        public void ReadShiftExcel()
        {
            const int START_ROW_NUM = 4;

            //読み取り対象のエクセルオブジェクト作成
            IXLWorksheet workSheet = workBook.Worksheet("先生");

            //保育士
            int nurseryTeacherEndRowNum = GetNurseryTeacherShiftEndRowNum(workSheet, START_ROW_NUM);
            if (nurseryTeacherEndRowNum == 0)
            {
                throw new Exception("B列に保育士欄に「計」の項目が存在しません。");
            }
            for (int i = START_ROW_NUM; i < nurseryTeacherEndRowNum; i = i + 2)
            {
                Employee employee = CreateShiftInfo(workSheet, i);
                if(employee != null)
                {
                    this.employeeList.Add(employee);
                }
            }

            //調理師
            int cookStartRowNum = nurseryTeacherEndRowNum + 1;
            int cookEndRowNum = GetCookShiftEndRowNum(workSheet, cookStartRowNum);
            if (nurseryTeacherEndRowNum == 0)
            {
                throw new Exception("A列の調理師欄の記載が想定と異なります。");
            }
            for (int i = cookStartRowNum; i < cookEndRowNum; i = i + 2)
            {
                Employee employee = CreateShiftInfo(workSheet, i);
                if (employee != null)
                {
                    this.employeeList.Add(employee);
                }
            }

        }

        /// <summary>
        /// CSVファイルへの書き込み
        /// </summary>
        /// <param name="fileFullPath">ファイルフルパス</param>
        public void CreateCsvFile()
        {
            //ファイル名(出力場所はドラッグアンドドロップしたファイルと同じ場所)
            string fileFullPath = DateTime.Now.ToString("yyyy-MM-dd-HHmmss") + "_shift.csv";

            foreach (Employee employee in this.employeeList)
            {
                WriteCSV(fileFullPath, employee.GetShiftCsvData());
            }
            //最終行の改行のみ出力
            WriteCSV(fileFullPath, "");
        }

        /// <summary>
        /// シフト情報の作成
        /// </summary>
        /// <param name="sheet">エクセルシート</param>
        /// <param name="ronwNum">行数</param>
        /// <returns></returns>
        private Employee CreateShiftInfo(IXLWorksheet sheet,int ronwNum)
        {
            const int EMPLOYEE_ID_COLUMN_NUM = 2;

            const int DATE_HEADER_ROW_NUM = 2;

            //記載対象の判断
            // 人ごとにID指定があるかを確認。なければ処理対象から飛ばす
            string employeeId = sheet.Cell(ronwNum + 1, EMPLOYEE_ID_COLUMN_NUM).Value.ToString();
            if (employeeId == "")
            {
                return null;
            }
            else
            {
                int id;
                if (!int.TryParse(employeeId, out id))
                {
                    //ID自体は数値想定なので、数値変換できない値が記載されてる場合飛ばす
                    return null;
                }
            }

            //勤務者
            string name = sheet.Cell(ronwNum, EMPLOYEE_ID_COLUMN_NUM).Value.ToString();
            //Console.WriteLine(sheet.Cell(ronwNum, 2).Value.ToString());
            Employee employee = new Employee(employeeId, name);

            //各日程の勤務予定の確認
            int endDateColoumnNum = GetEndDateColoumnNum(sheet);
            for (int k = START_COLUMN_NUM; k < endDateColoumnNum; k++)
            {
                //開始、終了時刻の取得
                string startDateStr = sheet.Cell(ronwNum, k).Value.ToString();
                if (startDateStr == "")
                {
                    continue;
                }
                string endDateStr = sheet.Cell(ronwNum + 1, k).Value.ToString();
                if (endDateStr == "")
                {
                    continue;
                }

                //勤務予定日の情報取得
                string workDateStr = sheet.Cell(DATE_HEADER_ROW_NUM, k).Value.ToString();
                DateTime workDate = DateTime.Parse(workDateStr);

                //パターンコードの取得
                Shift shift = new Shift(startDateStr, endDateStr, workDate);
                string patternCode = this.patternMaster.GetShiftPatternCode(shift);
                ShiftPattern shiftPattern = new ShiftPattern(patternCode, shift);

                string shiftStr = workDate.ToString("yyyyMMdd") + "," + employeeId + "," + patternCode;
                //Console.WriteLine(shiftStr);

                employee.AddShift(shiftPattern);
            }

            return employee;
        }

        /// <summary>
        /// 保育士欄の終了行の行数を返却する
        /// </summary>
        /// <param name="sheet">対象のシート</param>
        /// <param name="startRowNum">開始行</param>
        /// <returns></returns>
        /// <remarks>前提：B列の引数の開始行以降で200行以前に「計」が存在する</remarks>
        /// 
        private int GetNurseryTeacherShiftEndRowNum(IXLWorksheet sheet,int startRowNum)
        {
            int result = 0;
            for(int i = startRowNum; i < 200; i++)
            {
                if(sheet.Cell(i, 2).Value.ToString() == "計")
                {
                    result = i;
                    break;
                }
            }
            return result;
        }

        /// <summary>
        /// 調理師行の終了行の行数を返却する
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="startRowNum"></param>
        /// <returns>前提：A列のセル結合が調理師が存在する行までセル結合されていること</returns>
        private int GetCookShiftEndRowNum(IXLWorksheet sheet, int startRowNum)
        {
            int result = 0;
            IXLCell cookTitleCell= sheet.Cell(startRowNum, 1);
            if(cookTitleCell.IsMerged())
            {
                result = cookTitleCell.MergedRange().LastRow().RowNumber();
            }

            return result;
        }

        /// <summary>
        /// 日付の終了列数の取得
        /// </summary>
        /// <param name="sheet"></param>
        /// <returns></returns>
        private int GetEndDateColoumnNum(IXLWorksheet sheet)
        {
            const int DATE_HEADER_ROW_NUM = 2;

            int result=0;
            for (int i = START_COLUMN_NUM; i < 200; i++)
            {
                string dateStr = sheet.Cell(DATE_HEADER_ROW_NUM, i).Value.ToString();

                DateTime date;
                if (!DateTime.TryParse(dateStr, out date))
                {
                    result = i-1;
                    break;
                }
            }
            return result;
        }

        /// <summary>
        /// CSVファイルへの書き込み
        /// </summary>
        /// <param name="fileFullPath">ファイルフルパス</param>
        /// <param name="text">書き込み内容</param>
        private void WriteCSV(string fileFullPath,string text)
        {
            File.AppendAllText(@fileFullPath, text + Environment.NewLine);

        }

    }
}
