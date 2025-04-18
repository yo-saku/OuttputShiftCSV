using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OuttputShiftCSV
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {

                //ドラッグアンドドロップで実行されているときのみ処理を実行
                if (args.Length == 0)
                {
                    return;
                }
                Console.WriteLine("処理を開始します。");
                Console.WriteLine(@args[0]);

                //パターンコードの設定ファイルの読み取り
                PatternMaster patternMaster = new PatternMaster();
                patternMaster.ReadMasterExcel();
                //シフトエクセルの読み取り
                //ShiftExcel shiftExcel = new ShiftExcel(@Console.ReadLine(), patternMaster);
                ShiftExcel shiftExcel = new ShiftExcel(@args[0], patternMaster);
                shiftExcel.ReadShiftExcel();

                //CSVファイルの作成
                shiftExcel.CreateCsvFile();

                Console.WriteLine("出力が完了しました。");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                Console.WriteLine("終了するには、任意のキーを押下してください。");
                Console.ReadLine();
            }

        }
    }
}
