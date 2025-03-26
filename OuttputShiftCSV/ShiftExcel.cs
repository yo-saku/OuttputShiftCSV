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
        private XLWorkbook workBook;
        private PatternMaster patternMaster;

        public ShiftExcel()
        {
            throw new NotImplementedException();
        }

        public ShiftExcel(string filePath, PatternMaster patternMaster) 
        {
            this.workBook = new XLWorkbook(filePath);
            this.patternMaster = patternMaster;
        }

        public void ReadShiftExcel()
        {
            const int START_ROW_NUM = 4;

            //読み取り対象のエクセルオブジェクト作成
            IXLWorksheet workSheet = workBook.Worksheet("先生");

            //書き込み先CSVファイル名の作成
            string csvFileFullPath = ".\\" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_shift.csv";

            //保育士
            int nurseryTeacherEndRowNum = GetNurseryTeacherShiftEndRowNum(workSheet, START_ROW_NUM);
            if (nurseryTeacherEndRowNum == 0)
            {
                throw new Exception("B列に保育士欄に「計」の項目が存在しません。");
            }
            for (int i = START_ROW_NUM; i < nurseryTeacherEndRowNum; i = i + 2)
            {
                CreateShiftInfo(workSheet, i, csvFileFullPath);
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
                CreateShiftInfo(workSheet, i, csvFileFullPath);
            }

            //TODO 書き込み処理は分離したい
            //最終行の改行のみ出力
            WriteCSV(csvFileFullPath, "");

        }

        private void CreateShiftInfo(IXLWorksheet sheet,int ronwNum,string fileTullPath)
        {
            const int START_COLOUMN_NUM = 4;
            const int WORKER_CODE_COLUMN_NUM = 2;

            const int DATE_HEADER_ROW_NUM = 2;

            //先生の名前の出力
            Console.WriteLine(sheet.Cell(ronwNum, 2).Value.ToString());
            // 人ごとにID指定があるかを確認。なければ処理対象から飛ばす
            string employeeId = sheet.Cell(ronwNum + 1, WORKER_CODE_COLUMN_NUM).Value.ToString();
            if (employeeId == "")
            {
                return;
            }
            else
            {
                int id;
                if (!int.TryParse(employeeId, out id))
                {
                    //ID自体は数値想定なので、数値変換できない値が記載されてる場合飛ばす
                    return;
                }
            }

            //各日程の勤務予定の確認
            int endDateColoumnNum = GetEndDateColoumnNum(sheet);
            for (int k = START_COLOUMN_NUM; k < endDateColoumnNum; k++)
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
                string patternCode = this.patternMaster.GetShiftPatternCode(new Shift(startDateStr, endDateStr, workDate));

                string shiftStr = workDate.ToString("yyyyMMdd") + "," + employeeId + "," + patternCode;
                Console.WriteLine(shiftStr);

                WriteCSV(fileTullPath, shiftStr);
            }
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
            const int START_COLUMN_NUM = 4;
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

        private void WriteCSV(string fileFullPath,string text)
        {
            File.AppendAllText(@fileFullPath, text + Environment.NewLine);

        }

    }
}
