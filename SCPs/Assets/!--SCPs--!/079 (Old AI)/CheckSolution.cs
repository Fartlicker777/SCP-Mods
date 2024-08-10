using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using KModkit;
using Rnd = UnityEngine.Random;
public class CheckSolution : MonoBehaviour {

   // Use this for initialization
   public static bool IsCorrect (int Seed, int Input) {

      string Pi = "31415926535897932384626433832795028841971693993751058209749445923078164062862089986280348253421170679821480865132823066470938446095505822317253594081284811174502841027019385211055596446229489549303819644288109756659334461284756482337867831652712019091456485669234603486104543266482133936072602491412737245870066063155881748815209209628292540917153643678925903600113305305488204665213841469519415116094330572703657595919530921861173819326117931051185480744623799627495673518857527248912279381830119491";
      string MarkiplierE = "271828182845904523536028747135266249775724709369995957496696762772407663035354759457138217852516642742746639193200305992181741359662904357290033429526059563073813232862794349076323382988075319525101901157383418793070215408914993488416750924476146066808226480016847741185374234544243710753907774499206955170276183860626133138458300075204493382656029760673711320070932870912744374704723069697720931014169283681902551510865746377211125238978442505695369677078544996996794686445490598793163688923009879312";
      int[] NumberLetterLengths = { 4, 3, 3, 5, 4, 4, 3, 5, 5, 4 };

      switch (Seed) {
         case 1: //Sum of letters
            int LetterSum = 0;
            int Case1Temp = Input;
            while (Case1Temp != 0) {
               LetterSum += NumberLetterLengths[Case1Temp % 10];
               Case1Temp /= 10;
            }
            return Input % LetterSum == 0;

         case 2: //Pi
            return Pi.Contains(Input.ToString());

         case 3: //2^n - 1
            return Math.Log(Input + 1, 2) % 1 == 0;

         case 4: //Two cubes concatenated
            string I = Input.ToString();
            for (int i = 1; i < I.Length; i++) {
               if (Math.Pow(int.Parse(I.Substring(0, i)), (double) 1 / 3) >= .99f && Math.Pow(int.Parse(I.Substring(i + 1)), (double) 1 / 3) >= .99f) { //Can't be perfect, so margin of error by .01f
                  return true;
               }
            }
            return false;

         case 5: //Divisble 7
            return Input % 7 == 0;

         case 6: //Divisible by 2 or 3
            return Input % 2 == 0 ^ Input % 3 == 0;

         case 7: //4 in odd
            for (int i = 0; i < Input.ToString().Length; i += 2) {
               if (Input.ToString()[i].ToString() == "4") {
                  return true;
               }
            }
            return false;

         case 8: //8
            return Input == 8;

         case 9: //Swan
            bool Has1 = false;
            bool Has0 = false;
            for (int i = 0; i < Input.ToString().Length; i++) {
               if (Input.ToString()[i].ToString() == "1") {
                  Has1 = true;
               }
               if (Input.ToString()[i].ToString() == "0" && Has1) {
                  Has0 = true;
               }
               if (Input.ToString()[i].ToString() == "8" && Has0) {
                  return true;
               }
            }
            return false;

         case 10: //EEEEEEEEEEEEEEEEE
            return MarkiplierE.Contains(Input.ToString());

         case 11: //Is square
            return Math.Sqrt(Input) % 1 == 0;

         case 12: //DEAF
            //Debug.Log(ExMath.ConvertToBase(Input, 16));
            return ExMath.ConvertToBase(Input, 16).Any(x => "DEAF".Contains(x));

         case 13: //Sum prime
            return ExMath.IsPrime(ExMath.DigitSum(Input));

         case 14: //Repeats

            int Case14Temp = 0;
            bool[] Case14BoolArr = new bool[10];
            bool Case14Flag = true;

            Case14Temp = Input;
            Case14Flag = true;
            for (int i = 0; i < 10; i++) {
               Case14BoolArr[i] = false;
            }
            while (Case14Temp != 0) {
               if (Case14BoolArr[Case14Temp % 10]) {
                  Case14Flag = false;
               }
               Case14BoolArr[Case14Temp % 10] = true;
               Case14Temp /= 10;
            }

            return Case14Flag;

         case 15: //4 consecuative ascending

            int Case15Count = 0;
            int Case15Best = 0;

            Case15Count = 0;
            int[] ar = ExMath.ToIntArray(Input);
            for (int i = 1; i < ar.Length; i++) {
               if (ar[i] > ar[i - 1]) {
                  Case15Count++;
                  if (Case15Count > Case15Best) {
                     Case15Best = Case15Count;
                  }
               }
               else {
                  Case15Count = 0;
               }
            }

            return Case15Best >= 4;

         case 16: //Prime
            return ExMath.IsPrime(Input);

         case 17: //Double or half
            bool Case17Flag = true;

            Case17Flag = true;
            int[] AnswerArr = ExMath.ToIntArray(Input);
            for (int i = 1; i < AnswerArr.Length; i++) {
               if (AnswerArr.Join("").Contains("0") || !(AnswerArr[i - 1] / 2 == AnswerArr[i] || AnswerArr[i - 1] * 2 == AnswerArr[i])) {
                  Case17Flag = false;
               }
            }
            return Case17Flag;

         case 18: //Leading Zero, check in other script
            return false;

         case 19: //Month or day
            return Input.ToString().Contains(System.DateTime.Today.Year.ToString()) || Input.ToString().Contains(System.DateTime.Today.Month.ToString());

         case 20: //1234 to 4321
            bool Case20Temp = false;

               for (int i = 0; i < Input.ToString().Length - 4; i++) {
                    int validNum;
                  if (int.TryParse(Input.ToString().Substring(i, 4), out validNum) && validNum >= 1234 && validNum <= 4321) {
                     Case20Temp = true;
                  }
               }
            return Case20Temp;

         case 21: //+-5 from cube
            for (int i = -5; i < 6; i++) {
               if (Math.Pow(Input + i, (double) 1 / 3) >= .99f) {
                  return true;
               }
            }
            return false;

         case 22: //Palindromic
            if (Input >= 10000 && Input < 100000) {
               if (Input / 10000 == Input % 10 && Input / 1000 % 10 == Input % 100 / 10) {
                  return true;
               }
               return false;
            }
            else if (Input < 1000000) {
               if (Input / 100000 == Input % 10 && Input / 10000 % 10 == Input % 100 / 10 && Input / 1000 % 10 == Input % 1000 / 100) {
                  return true;
               }
               return false;
            }
            else if (Input < 10000000) {
               if (Input / 1000000 == Input % 10 && Input / 100000 % 10 == Input % 100 / 10 && Input / 10000 % 10 == Input % 1000 / 100) {
                  return true;
               }
               return false;
            }
            return false;

         case 23: //Greater than or 0
            bool Case23Flag = true;
               int[] AnswerArr23 = ExMath.ToIntArray(Input);
               for (int i = 1; i < AnswerArr23.Length; i++) {
                  if (!(AnswerArr23[i - 1] < AnswerArr23[i] || AnswerArr23[i] == 0)) {
                     Case23Flag = false;
                  }
               }

            return Case23Flag;

         case 24:
            return Input == 9999999;

         case 25:
            return Input % 9 % 2 == 1;

         default:
            return false;
      }
   }
}
