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
                Console.WriteLine("■スケジュールパターンリストの読み取り");
                PatternMaster patternMaster = new PatternMaster();
                patternMaster.ReadMasterExcel();
                //読み取り結果確認
                List<string> samePatternInfoList = patternMaster.GetSamePatternCodesInfomation();
                if(samePatternInfoList.Count > 0)
                {
                    Console.WriteLine("スケジュールパターンリストに出勤時刻と退勤時刻の組み合わせに重複が見つかりました。");
                    Console.WriteLine("重複パターンは先頭のパターンコードを使用してcsvを作成します。");
                    Console.WriteLine("重複パターン：");
                    foreach (string samePatternInfo in samePatternInfoList)
                    {
                        Console.WriteLine(samePatternInfo);
                    }
                }
                //シフトエクセルの読み取り
                Console.WriteLine("■シフトエクセルの読み取り");
                ShiftExcel shiftExcel = new ShiftExcel(@args[0], patternMaster);
                shiftExcel.ReadShiftExcel();

                //CSVファイルの作成
                Console.WriteLine("■CSVファイルの作成");
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
