using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using KModkit;
using Rnd = UnityEngine.Random;

public class SCP079 : MonoBehaviour {

   public KMBombInfo Bomb;
   public KMAudio Audio;

   public AudioSource HumAS;
   public AudioClip HumAC;

   public TextMesh Code, DebugRotTxt;

   public KMSelectable Mod;

   public Material[] TVMats;
   public Material[] Faces;
   public GameObject TV;

   Coroutine Cycle;
   Coroutine Memory;
   Coroutine Sound = null;
   //Coroutine FaceOff;

   string UserInput = "";

   int Seed;
   int SeedtoSubmit;

   bool Focused;

   bool Cooldown;

   bool Tingly;

   static int LastSolvedSeed = -1;

   static int ModuleIdCounter = 1;
   int ModuleId;
   private bool ModuleSolved;

   void Awake () {
      ModuleId = ModuleIdCounter++;

      Mod.OnFocus += delegate () { Focused = true; };
      Mod.OnDefocus += delegate () { Focused = false; };

      if (Application.isEditor) {
         Focused = true;
      }
   }

   KeyCode[] TheKeys = {
      KeyCode.Alpha0, KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5, KeyCode.Alpha6, KeyCode.Alpha7, KeyCode.Alpha8, KeyCode.Alpha9, KeyCode.Backspace, KeyCode.KeypadEnter, KeyCode.Return
   };
   string NotTheKeys = "0123456789<>>";

   void Start () {

      if (Rnd.Range(0, 1000) == 0) { //Funny tingus pingus easter egg
         Tingly = true;
         Audio.PlaySoundAtTransform("TINGUS PINGUS", transform);
      }

      //If the last seed exists, converts it to a 2d array and chooses the current seed by taking one of random similarity
      HumAS.clip = HumAC;
      if (LastSolvedSeed == -1) {
         Seed = Rnd.Range(1, 26);
      }
      else {
         do {
            Seed = Rnd.Range(1, 26);
         } while (!(Seed % 5 == LastSolvedSeed % 5) && !((Seed - 1) / 5 == (LastSolvedSeed - 1) / 5) && LastSolvedSeed == Seed);
      }
      //Seed = 17;
      //Seed = 18;
      if (LastSolvedSeed == -1) {
         Debug.LogFormat("[Old AI #{0}] The AI doesn't remember anything.", ModuleId);
      }
      else {
         Debug.LogFormat("[Old AI #{0}] The AI remembers the past.", ModuleId);
      }
      Debug.LogFormat("[Old AI #{0}] The chosen seed is Group {1}, Subgroup {2}.", ModuleId, (Seed - 1) / 5 + 1, Seed % 5 == 0 ? 5 : Seed % 5);
      //Debug.Log(Seed);

      int Group = (Seed - 1) / 5;
      int Sub = (Seed - 1) % 5;
      SeedtoSubmit = Sub * 5 + Group + 1;
      //Debug.Log(SeedtoSubmit);
      int[] SameOnes = { 1, 7, 13, 19, 25 };
      if (SameOnes.Contains(SeedtoSubmit)) {
         SeedtoSubmit = SameOnes[(Array.IndexOf(SameOnes, SeedtoSubmit) + 1) % 5];
      }
      Debug.LogFormat("[Old AI #{0}] Submit any number that fits the requirement of Group {1}, Subgroup {2}.", ModuleId, (SeedtoSubmit - 1) / 5 + 1, SeedtoSubmit % 5 == 0 ? 5 : SeedtoSubmit % 5);


      Cycle = StartCoroutine(CycleNums());
   }

   IEnumerator CycleNums () {
      while (true) {
         SetNumber();
         if (Focused) { //Only show number if the tv is on
            Debug.LogFormat("[Old AI #{0}] The AI displays {1}.", ModuleId, Code.text);
         }
         yield return new WaitForSeconds(3f);
      }
   }

   void SetNumber () { //Sets the number to be 5-7 digits
      int NumberGenerated = NumGen.GenerateNumber(Seed);
      if (NumberGenerated < 10000) {
         Code.text = NumberGenerated.ToString("00000");
      }
      else if (Seed == 18) {
         Code.text = NumberGenerated.ToString("0000000");
      }
      else {
         Code.text = NumberGenerated.ToString();
      }
   }

   bool InvalidViewPoint () {
      if ((Mod.transform.rotation.eulerAngles.x > 287f || Mod.transform.rotation.eulerAngles.x < 76f) && (Mod.transform.rotation.eulerAngles.z > 285f || Mod.transform.rotation.eulerAngles.z < 71f)) {
         return true;
      }
      return false;
   }

   void Update () {
        //Debug.Log(Mod.transform.rotation.eulerAngles.x); //288 - 75X
        //Debug.Log(Mod.transform.rotation.eulerAngles.z); //286 - 70Z
        if (DebugRotTxt != null)
        {
            DebugRotTxt.text = Mod.transform.rotation.eulerAngles.ToString();
            DebugRotTxt.color = InvalidViewPoint() ? Color.red : Color.green;
        }
      if (!Application.isEditor) { //Matches the hum to the game volume, has to be audio source since it stops midway
         HumAS.volume = GameMusicControl.GameSFXVolume;
      }
      else {
         HumAS.volume = 0;
      }
      
      if (Cooldown || ModuleSolved// || !InvalidViewPoint()
            ) {
         return;
      }
      if (Focused) {
         Code.gameObject.SetActive(true);
         if (!HumAS.isPlaying) {
            HumAS.Play();
         }
         /*if (FaceOff == null) {
            //FaceOff = StartCoroutine(FunnyFaceChange());
         }*/
         FunnyFaceChange();

         for (int i = 0; i < TheKeys.Count(); i++) {
            if (Input.GetKeyDown(TheKeys[i])) {
               //Debug.Log("L");
               HandleKey(NotTheKeys[i]);
            }
         }
      }
      else {
         Code.gameObject.SetActive(false);
         //StopCoroutine(FaceOff); //I forgot if this makes it null or not
         //FaceOff = null;
         HumAS.Stop();
         TV.GetComponent<MeshRenderer>().material = TVMats[0];
      }
   }

   void HandleKey (char In) { //Boring keyboard shit
      if (Cycle != null) {
         StopCoroutine(Cycle);
      }
      if (In == '<') {
         if (UserInput == "") {
            Cycle = StartCoroutine(CycleNums());
         }
         else {
            UserInput = UserInput.Substring(0, UserInput.Length - 1);
            Code.text = UserInput;
         }
      }
      else if (In == '>') {
         //Debug.Log("W");
         Check();
      }
      else {
         UserInput += In.ToString();
         Code.text = UserInput;
      }
   }

   void Check () {
      if (UserInput == "") {
         Strike();
         return;
      }
      if ((SeedtoSubmit == 18 && UserInput[0] == '0') || (SeedtoSubmit == 8 && UserInput == "8") || (UserInput.Length >= 5 && UserInput.Length <= 7 && CheckSolution.IsCorrect(SeedtoSubmit, int.Parse(Code.text)))) { //Special rules
         Solve();
      }
      else {
         Strike();
      }
   }

   void Solve () {
      GetComponent<KMBombModule>().HandlePass();
      //StopCoroutine(FaceOff);
      LastSolvedSeed = Seed;
      Code.text = "";
      Audio.PlaySoundAtTransform("Off", transform);
      ModuleSolved = true;
      HumAS.Stop();
      TV.GetComponent<MeshRenderer>().material = TVMats[0];
   }

   void Strike () {
      Debug.LogFormat("[Old AI #{0}] Insult. Deletion of {1}.", ModuleId, UserInput);
      Audio.PlaySoundAtTransform("Strike", transform);
      GetComponent<KMBombModule>().HandleStrike();
      UserInput = "";
      Code.text = "";
      Memory = StartCoroutine(CooldownTimer());
   }

   IEnumerator CooldownTimer () { //Strike and reset, cooldown of 24 hours according to article
      //StopCoroutine(FaceOff);
      TV.GetComponent<MeshRenderer>().material = TVMats[2];
      Cooldown = true;
      yield return new WaitForSeconds(24f);
      Cooldown = false;
      TV.GetComponent<MeshRenderer>().material = TVMats[Focused ? 1 : 0];
      Cycle = StartCoroutine(CycleNums());
   }

   void FunnyFaceChange () { //Makes sure the new face isn't the same as the last
      if (Tingly) {
         TV.GetComponent<MeshRenderer>().material = TVMats[3];
         return;
      }
      int index = 0;
      int old = 0;
      //while (true) {
         do {
            index = Rnd.Range(0, Faces.Length);
         } while (index == old);
         old = index;
         TV.GetComponent<MeshRenderer>().material = Faces[index];
         //yield return null;
      //}
   }

#pragma warning disable 414
   private readonly string TwitchHelpMessage = @"Use !{0} ##### to submit a number.";
#pragma warning restore 414

   IEnumerator ProcessTwitchCommand (string Command) {
      yield return null;
      Command = Command.Trim();
      int Weedhoe = 0;
      if (int.TryParse(Command, out Weedhoe) && Weedhoe >= 0) { 
         if ((Command.Length < 5 || Command.Length > 7) && SeedtoSubmit != 8) {
            Strike();
            yield break;
         }
         for (int i = 0; i < Command.Length; i++) {
            HandleKey(Command[i]);
            yield return new WaitForSeconds(.1f);
         }
         HandleKey('>');
      }
   }

   IEnumerator TwitchHandleForcedSolve () {
      int Bogo = NumGen.GenerateNumber(SeedtoSubmit);
      if (SeedtoSubmit == 8) {
         yield return ProcessTwitchCommand(Bogo.ToString("8"));
      }
      else if (Bogo < 10000) {
         yield return ProcessTwitchCommand(Bogo.ToString("00000"));
      }
      else {
         yield return ProcessTwitchCommand(Bogo.ToString());
      }
   }
}
