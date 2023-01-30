using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using KModkit;

public class DeadByDesklightScript : MonoBehaviour {

   //public variables
   public KMBombInfo bomb;
   public KMAudio audio;

   //Add buttons to selectable list
   public KMSelectable button0;     //add button 0 for SurvivorButton0
   public KMSelectable button1;     //add button 1 for SurvivorButton1
   public KMSelectable button2;     //add button 2 for SurvivorButton2
   public KMSelectable button3;     //add button 3 for SurvivorButton3
   public KMSelectable button4;     //add button 4 for KillerButton


   //Sound name list (names of sounds in SFX folder)
   string[] Sounds = {"Trapper_trap_placed", "Trapper_trap_activated"};
      /*/array of sound names used by audio.PlaySoundAtTransform(Sounds[i], transform);
                           change [i] to whatever index is the correct name of the sound
                           Alternatively just put the name in the line instead such as: audio.PlaySoundAtTransform("Trapper_trap_placed", transform);
      names as follows:
         0 = Trapper_trap_placed
         1 = Trapper_trap_activated
      /*/
   // Add texts generator count
   public TextMesh generatortext;
   // Add texts optionskillertexts
   public TextMesh killeroptionaltext;

   //Add an option for disabling the killer icon
   public GameObject disableKillerButton;

   // Add options for images
   public Material[] survivorStatusOptions;
      /*/Array of survivor status image materials:
            0 = Dead
            1 = Hooked
            2 = Downed
            3 = Injured
            4 = Healthy
            5 = Escaped
            6 = Caught in bear-trap (Trapper)
         /*/
   public Material[] portraitOptions;
      /*/Array of survivor portrait image materials
            Names are as follow:
            0 = Dwight Fairfield
            1 = Meg Thomas
            2 = Claudette Morel
            3 = Jake Park
            4 = Nea Karlsson
            5 = Laurie Strode
            6 = Ace Visconti
            7 = William 'Bill' Overbeck
      /*/
   public Material[] killerOptions;
         /*/Array of killer icon image materials
            Names are as follow:
            0 = Trapper
            1 = Wraith
            2 = Hillbilly
         /*/ 
   public Material[] generatorOptions;
      /*/Array of generator status image materials:
            0 = Escape Available
            1 = Gens need repairing
            2 = Gens need repairing
            3 = Gens need repairing
            4 = Gens need repairing
            5 = Gens need repairing


         /*/
   public Material[] optionalInfoButtonTextureOptions;
         /*/  Used to turn optional killer info button visible/invisible
            0 = Invisible
            1 = dbd_brown
            2 = hillbilly_red
         /*/
   public Material[] optionalInfoImageTextureOptions;
         /*/  Used to proive an optional killer info image
            0 = Invisible
            1 = WraithImage
         /*/
   public Renderer button0image;
   public Renderer button1image;
   public Renderer button2image;
   public Renderer button3image;
   public Renderer button4image;
   public Renderer generatorimage;
   public Renderer killerOptionalButton;
   public Renderer killerOptionalImage;

   //Private variables

      //Survivor Variables
      
         string[] survivorNames = {"Dwight Fairfield",  "Meg Thomas", "Claudette Morel", "Jake Park", "Nea Karlsson", "Laurie Strode", "Ace Visconti", "William 'Bill' Overbeck"};
         //name list only for logging
            /*/Names are as follow:
               0 = Dwight Fairfield
               1 = Meg Thomas
               2 = Claudette Morel
               3 = Jake Park
               4 = Nea Karlsson
               5 = Laurie Strode
               6 = Ace Visconti
               7 = William 'Bill' Overbeck
            /*/ 
         //Survivor status values            Length 4 because 4 survivors
         int[] survivorstatusarray = new int[4];
            /*/Statuses are integers that represent the following health states for survivors:
               0 = Dead
               1 = Hooked
               2 = Downed
               3 = Injured
               4 = Healthy
               5 = Escaped
               6 = Caught in bear-trap (Trapper)
            /*/
            //name list used for logging, names are same as above.
            string[] survivorStatusNames = {"Dead",  "Hooked", "Downed", "Injured", "Healthy", "Escaped", "Caught in bear-trap"};


         //Survivor portrait values          Length 4 because 4 survivors
         int[] survivorvaluearray = new int[4];
            //survivor value used to determine name, associated priorities and the healthy portrait image

         //Survivor injure priority values
         int[] injureprioritylist = {1, 2, 3, 4, 5, 6, 7 ,8};
            //Injure priority in same order/index as the survivorNames array
         decimal[] survivorinjurepriorityarray = new decimal[4];
                                                      //Length 4 because 4 survivors

         //Survivor down priority values
         int[] downprioritylist = {5, 6, 7, 8, 1, 2, 3, 4};
            //Down priority in same order/index as the survivorNames array
         decimal[] survivordownpriorityarray = new decimal[4];

         //Survivor hook priority values
         int[] hookprioritylist = {8, 7, 6, 5, 4, 3, 2, 1};
            //Hook priority in same order/index as the survivorNames array
         decimal[] survivorhookpriorityarray = new decimal[4];

         //Survivor camp priority values
         int[] campprioritylist = {4, 3, 2, 1, 8, 7, 6, 5};
            //Camp priority in same order/index as the survivorNames array
         decimal[] survivorcamppriorityarray = new decimal[4];

         //Survivor hook count
         decimal[] survivorhookcountarray = new decimal[4];
                                                      //Length 4 because 4 survivors

         //Count variables for other survivor logic
         private int healthyCount = 0;
         private int escapedCount = 0;
         private int hookedCount = 0;
         private int deadCount = 0;

      //Killer variables
         private int killervalue = 0;

         string[] killerNames = {"Trapper",  "Wraith", "Hillbilly"};
            //name list only for logging
            /*/Names are as follow:
               0 = Trapper
               1 = Wraith
               2 = Hillbilly
            /*/ 

         //Trapper Variables
         private int maxBearTrapCount = 0;            //Maximum number of active bear-traps allowed
         private int currentActiveBearTrapCount = 0;  //Number of currently active bear-traps

         //Wraith Variables
         private int cloakedStatus = 0; //0 = cloaked, 1 = uncloaked

         //Hillbilly Variables
         private int hillbillyCurrentTemperature;  //A changing integer that is the current temperature of the hillbilly's chainsaw
         private bool hillbillyOverheat;            //false = not overheated, true = overheated 
         private int timeInteger;                  //Used as part of temperature countdown coroutine
         private int hillbillyTemperatureMaximum = 99; //The overheat value for the hillbilly chainsaw
         private int hillbillyTemperatureIncrement = 40; //How much using the chainsaw increases the temperature by

      //Generator variables
         private int generatorCount = 5; 

      //Other random variables

         private float speedX = 1, speedY = 1, speedZ = 1;

         private int predictedActionValue = 0;
            /*/predictedActionValue is an integer that represents the following possible actions:
               0 = Not Determined (aka error value)
               1 = Camp a hooked survivor
               2 = Hook a downed survivor
               3 = Chase an injured survivor
               4 = Chase a healthy survivor
               5 = Use killer power
               6 = Chase a survivor caught in bear-trap (Trapper)
            /*/   
         private int mostRecentAction = 0;
            //mostRecentAction uses same integer values as predictedActionValue  

         private int predictedButtonValue = 5;
            /*/predictedButtonValue is an integer that represents the following possible buttons:
               0 = Survivor 0
               1 = Survivor 1
               2 = Survivor 2
               3 = Survivor 3
               4 = Killer Icon
               5 = Not Determined (aka error value)
            /*/   
         private int mostRecentButton = 5;
            //mostRecentButton uses same integer values as predictedButtonValue
               
         private decimal hookcountincrement = 1;   //how much does hookcount increases by for every stage they're hooked
         private decimal hookcountrequired = 10;   //hook count required for survivor to die

         //Chance variables

            //These are used to determine non-clicked survivor actions, with the chance rolled 0 to 99 inclusive, i.e 10m has a 10% chance of succeeding
         private decimal chancevalue = 0m;                
         private decimal exitescapechance = 30m;         //chance of a healthy survivor to escape once exit gates are powered
         private decimal generatorrepairchance = 10m;    //chance of a healthy survivor to repair a generator before  exit gates are powered
                                                            //10m ≈ 35% chance of atleast 1 generator being repaired if all 4 survs are healthy on any given stage
                                                            //5m ≈ 18% chance of atleast 1 generator being repaired if all 4 survs are healthy on any given stage
                                                            //3m ≈ 11% chance of atleast 1 generator being repaired if all 4 survs are healthy on any given stage
                                                            //2m ≈ 7.7% chance of atleast 1 generator being repaired if all 4 survs are healthy on any given stage
         private decimal healchance = 30m;               //chance of an injured or downed survivor to heal themselves if not interacted with by the killer
         private decimal unhookchance = 20m;             //chance of a hooked survivor to unhook themselves if not camped by the killer

            //Trapper chances
            private decimal beartrapescapechance = 40m;     //chance of a bear-trapped survivor to free themselves if not interacted with by the killer
            private decimal beartraptrappedchance = 5m;     //chance of a healthy survivor to trap themselves in a bear-trap if not interacted with by the killer, and if they don't repair a generator



   static int ModuleIdCounter = 1;
   int ModuleId;
   private bool ModuleSolved;


   //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
   void Awake () {
      ModuleId = ModuleIdCounter++;

      // Make functions called PressButtonX occur when you press buttonX
      button0.OnInteract += delegate () { PressButton0(button0); return false; };    //Survivor0 button
      button1.OnInteract += delegate () { PressButton1(button1); return false; };    //Survivor1 button
      button2.OnInteract += delegate () { PressButton2(button2); return false; };    //Survivor2 button
      button3.OnInteract += delegate () { PressButton3(button3); return false; };    //Survivor3 button
      button4.OnInteract += delegate () { PressButton4(button4); return false; };    //Killer icon button

   }
   //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
   void Start () {


      //Setup survivors
      SurvivorsSetup();
  
      //Setup killer
      KillerSetup();

      //No generator setup needed, is setup in variable declaration and imageupdate

      //Call first prediction
      DetermineNextActionLogic();
      
      //Image refresh
      imageupdate();
   }
   //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
   void Update () {
      //Currently does nothing
   }
   //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
   void PressButton0(KMSelectable lol)
   {    //Pressed survivor 0
            //Log press
            Debug.LogFormat("[DeadByDesklight #{0}] You pressed the first survivor button.", ModuleId);

      lol.AddInteractionPunch();
      //button0startposition = buttons[0].transform.position;

      //Check if button was correct, strike if wrong
      mostRecentButton = 0;
         //If Button0 was not the predicted button to press
         if (mostRecentButton != predictedButtonValue){
            //Then strike the module
            Debug.LogFormat("[DeadByDesklight #{0}] Incorrect button pressed! Strike!", ModuleId);
            GetComponent<KMBombModule>().HandleStrike();
            //But continue on anyway
         }
         else {
            Debug.LogFormat("[DeadByDesklight #{0}] Correct button pressed.", ModuleId);
         }

      //Make the button not do anything if survivor shouldn't be interacted with   
         //If ModuleSolved = true or survivor0 is dead or escaped
         if(ModuleSolved == true || survivorstatusarray[0] == 0 || survivorstatusarray[0] == 5){
            //Return
            return;
         }

      //Take action on clicked survivor
      ClickedSurvivorLogic();

      //Logic other survivors
      OtherSurvivorLogic();

      //Check if defused
      CheckIfDefused();

      //Call next prediction
      DetermineNextActionLogic();
      
      //Image refresh
      imageupdate();
   }
   //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
   void PressButton1(KMSelectable lol)
   {    //Pressed survivor 1
            //Log press
            Debug.LogFormat("[DeadByDesklight #{0}] You pressed the second survivor button.", ModuleId);

      lol.AddInteractionPunch();
     //Check if button was correct, strike if wrong
      mostRecentButton = 1;
       //If Button1 was not the predicted button to press
         if (mostRecentButton != predictedButtonValue){
            //Then strike the module
            Debug.LogFormat("[DeadByDesklight #{0}] Incorrect button pressed! Strike!", ModuleId);
            GetComponent<KMBombModule>().HandleStrike();
            //But continue on anyway
         }else {
            Debug.LogFormat("[DeadByDesklight #{0}] Correct button pressed.", ModuleId);
         }

     //Make the button not do anything if survivor shouldn't be interacted with   
         //If ModuleSolved = true or survivor1 is dead or escaped
         if(ModuleSolved == true || survivorstatusarray[1] == 0 || survivorstatusarray[1] == 5){
            //Return
            return;
         }

    //Take action on clicked survivor
      ClickedSurvivorLogic();

      //Logic other survivors
      OtherSurvivorLogic();

      //Check if defused
      CheckIfDefused();

      //Call next prediction
      DetermineNextActionLogic();
      
      //Image refresh
      imageupdate();
   }
   //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
   void PressButton2(KMSelectable lol)
   {  //Pressed survivor 2
         //Log press
         Debug.LogFormat("[DeadByDesklight #{0}] You pressed the third survivor button.", ModuleId);
      
      lol.AddInteractionPunch();
      //Check if button was correct, strike if wrong
      mostRecentButton = 2;
       //If Button2 was not the predicted button to press
         if (mostRecentButton != predictedButtonValue){
            //Then strike the module
            Debug.LogFormat("[DeadByDesklight #{0}] Incorrect button pressed! Strike!", ModuleId);
            GetComponent<KMBombModule>().HandleStrike();
            //But continue on anyway
         }else {
            Debug.LogFormat("[DeadByDesklight #{0}] Correct button pressed.", ModuleId);
         }

      //Make the button not do anything if survivor shouldn't be interacted with   
         //If ModuleSolved = true or survivor2 is dead or escaped 
         if(ModuleSolved == true || survivorstatusarray[2] == 0 || survivorstatusarray[2] == 5){
            //Return
            return;
         }

      //Take action on clicked survivor
      ClickedSurvivorLogic();

      //Logic other survivors
      OtherSurvivorLogic();

      //Check if defused
      CheckIfDefused();

      //Call next prediction
      DetermineNextActionLogic();
      
      //Image refresh
      imageupdate();
   }
   //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
   void PressButton3(KMSelectable lol)
   {  //Pressed survivor 3
         //Log press
         Debug.LogFormat("[DeadByDesklight #{0}] You pressed the fourth survivor button.", ModuleId);
      
      lol.AddInteractionPunch();
      //Check if button was correct, strike if wrong
      mostRecentButton = 3;
       //If Button3 was not the predicted button to press
         if (mostRecentButton != predictedButtonValue){
            //Then strike the module
            Debug.LogFormat("[DeadByDesklight #{0}] Incorrect button pressed! Strike!", ModuleId);
            GetComponent<KMBombModule>().HandleStrike();
            //But continue on anyway
         }else {
            Debug.LogFormat("[DeadByDesklight #{0}] Correct button pressed.", ModuleId);
         }

      //Make the button not do anything if survivor shouldn't be interacted with   
         //If ModuleSolved = true or survivor3 is dead or escaped
         if(ModuleSolved == true || survivorstatusarray[3] == 0 || survivorstatusarray[3] == 5){
            //Return
            return;
         }
   
      //Take action on clicked survivor
      ClickedSurvivorLogic();

      //Logic other survivors
      OtherSurvivorLogic();

      //Check if defused
      CheckIfDefused();

      //Call next prediction
      DetermineNextActionLogic();
      
      //Image refresh
      imageupdate();
   }
   //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
   void PressButton4(KMSelectable lol)
   {  //Pressed killer icon
         //Log press
         Debug.LogFormat("[DeadByDesklight #{0}] You pressed the killer icon button.", ModuleId);
      
      lol.AddInteractionPunch();
      //Check if button was correct, strike if wrong
      mostRecentButton = 4;
       //If Button4 was not the predicted button to press
         if (mostRecentButton != predictedButtonValue){
            //Then strike the module
            Debug.LogFormat("[DeadByDesklight #{0}] Incorrect button pressed! Strike!", ModuleId);
            GetComponent<KMBombModule>().HandleStrike();
            //But continue on anyway
         }else {
            Debug.LogFormat("[DeadByDesklight #{0}] Correct button pressed.", ModuleId);
         }
      
       //If modules solved, make the button not do anything
         if (ModuleSolved == true){  
         return;
          }

      //Take killer action
      if (killervalue == 0)
      {  //Do Trapper Action if Trapper
         PowerLogicTrapper();
      }
      if (killervalue == 1)
      {  //Do Wraith Action if Wraith
         PowerLogicWraith();
      }
      if (killervalue == 2)
      {  //Do Wraith Action if Hillbilly
         PowerLogicHillbilly();
      } //Other killers would go here

      //Logic survivors
      OtherSurvivorLogic();

      //Check if defused
      CheckIfDefused();

      //Call next prediction
      DetermineNextActionLogic();

      //Image refresh
      imageupdate();
   }
   //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
   void SurvivorsSetup()
   //Set up survivor variables at bomb start
   {
         //For each survivor
         for (int i = 0; i < 4; i++) 
          {
            //set status = 4 (healthy)
            survivorstatusarray[i] = 4;

            //survivorvalue = random between 0 and 7
            survivorvaluearray[i] = UnityEngine.Random.Range(0,8);

            //set injure priority
            survivorinjurepriorityarray[i] = ((injureprioritylist[survivorvaluearray[i]]) + 0.1m + (i * 0.1m));
         
            //set down priority
            survivordownpriorityarray[i] = ((downprioritylist[survivorvaluearray[i]])  + 0.1m + (i * 0.1m));

            //set hook priority
            survivorhookpriorityarray[i] = ((hookprioritylist[survivorvaluearray[i]])  + 0.1m + (i * 0.1m));

            //set camp priority
            survivorcamppriorityarray[i] = ((campprioritylist[survivorvaluearray[i]])  + 0.1m + (i * 0.1m));

            //set hook count to zero
            survivorhookcountarray[i] = 0;

            Debug.LogFormat("[DeadByDesklight #{0}] Created survivor {1} named {2} with chase healthy priority {3}, chase injured priority {4}, hook downed priority {5} and camp hooked priority {6}", ModuleId, i + 1, survivorNames[survivorvaluearray[i]], survivorinjurepriorityarray[i], survivordownpriorityarray[i], survivorhookpriorityarray[i], survivorcamppriorityarray[i]);
         }


   }
   //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
   void KillerSetup()
   {  //Initial setup of killer stuff at bomb start
      //get random killer value
         //killervalue = UnityEngine.Random.Range(0,3);
         killervalue = 0;   //alternatively, force value for testing purposes
   
      //set up killer image
      button4image.sharedMaterial = killerOptions[killervalue];

      //Log killer name
      Debug.LogFormat("[DeadByDesklight #{0}] Chosen killer is {1}.", ModuleId, killerNames[killervalue]);

      //Call the right killer setup
      if (killervalue == 0)
      {//Setup Trapper if Trapper
         TrapperSetup();  
         return;
      } 
      else if (killervalue == 1)
      {//Setup Wraith if Wraith
         WraithSetup();
         return;
      }
      else if (killervalue == 2)
      {//Setup Wraith if Wraith
         HillbillySetup();
         return;
      }//Other killers would go here  
   }
   //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
   void TrapperSetup()
   {//Initial setup for Trapper

      //Optional info button setup
         //Make optional info button material dbd_brown
         killerOptionalButton.sharedMaterial = optionalInfoButtonTextureOptions[1];
         //Make optional info image material invisible
         killerOptionalImage.sharedMaterial = optionalInfoImageTextureOptions[0];


      //Get maxBearTrapCount
         //Start with 3
         maxBearTrapCount = 3;

         //check 1st cha for indoor map
         //if digital root + 1 = 1 or 2, maxBearTrapCount decrease by 1
         if (bomb.GetSerialNumber().First() == 'A' ||  bomb.GetSerialNumber().First() == 'J' ||  bomb.GetSerialNumber().First() == 'S'||  bomb.GetSerialNumber().First() == '0'|| bomb.GetSerialNumber().First() == '1')
         {
               maxBearTrapCount = maxBearTrapCount - 1;
               Debug.LogFormat("[DeadByDesklight #{0}] You are on an indoor map.", ModuleId);
         }
         //check 2nd cha for add-on
         //if digital root + 1 = 1 or 2, maxBearTrapCount increase by 2
         if (bomb.GetSerialNumber().ToArray()[1] == 'A' ||  bomb.GetSerialNumber().ToArray()[1] == 'J' ||  bomb.GetSerialNumber().ToArray()[1] == 'S'||  bomb.GetSerialNumber().ToArray()[1] == '0'|| bomb.GetSerialNumber().ToArray()[1] == '1')
         {
               maxBearTrapCount = maxBearTrapCount + 2;
               Debug.LogFormat("[DeadByDesklight #{0}] You have the Trapper Sack add-on.", ModuleId);
         }
         //if digital root + 1 = 3 or 4, maxBearTrapCount increase by 1
          if (bomb.GetSerialNumber().ToArray()[1] == 'B' || bomb.GetSerialNumber().ToArray()[1] == 'C' || bomb.GetSerialNumber().ToArray()[1] == 'K' || bomb.GetSerialNumber().ToArray()[1] == 'L' || bomb.GetSerialNumber().ToArray()[1] == 'T'|| bomb.GetSerialNumber().ToArray()[1] == 'U'|| bomb.GetSerialNumber().ToArray()[1] == '2'|| bomb.GetSerialNumber().ToArray()[1] == '3')
         {
               maxBearTrapCount = maxBearTrapCount + 1;
               Debug.LogFormat("[DeadByDesklight #{0}] You have the Trapper Bag add-on.", ModuleId);
         }

         //Log maxBearTrapCount
         Debug.LogFormat("[DeadByDesklight #{0}] Max bear-traps set to {1}.", ModuleId, maxBearTrapCount);
   }
   //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
   void WraithSetup()
   {//Initial setup for Wraith

      //Optional info button setup
         //Make optional info buttons' material invisible
         killerOptionalButton.sharedMaterial = optionalInfoButtonTextureOptions[0];
         //Make optional info image material invisible
         killerOptionalImage.sharedMaterial = optionalInfoImageTextureOptions[0];

      //Start cloaked
      cloakedStatus = 0;
   }
      //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
   void HillbillySetup()
   {//Initial setup for Hillbilly

      //Optional info button setup
         //Make optional info buttons' material dbd_brown
         killerOptionalButton.sharedMaterial = optionalInfoButtonTextureOptions[1];
         //Make optional info image material invisible
         killerOptionalImage.sharedMaterial = optionalInfoImageTextureOptions[0];
         //Blank the optional info text
         killeroptionaltext.text = "";
         //Make optional info text fontSize smaller   //100 for double digits to fit on the button
                                                      //70 for  double digits with degrees C
         killeroptionaltext.fontSize = 60;

         //Call the Coroutine for the chainsaw's temperature
         StartCoroutine(mycountdownCoroutine());

      //Make generator repair chance a little bit fairer for the hillbilly
      generatorrepairchance = 5m;   


   }
   //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
   void ClickedSurvivorLogic()
   {//state logic for the survivor that was clicked 

      //If healthy -> injure
      if (survivorstatusarray[mostRecentButton] == 4)
      {  //Set status = 3 (injured)
         survivorstatusarray[mostRecentButton] = 3;
         //Set mostRecentAction = 4 (Chase a healthy survivor)
         mostRecentAction = 4;
         //Log
         Debug.LogFormat("[DeadByDesklight #{0}] Survivor {1} has been injured.", ModuleId, mostRecentButton + 1);
          return;
         }
      
      //If injured -> down   
      if (survivorstatusarray[mostRecentButton] == 3)
      {  //Set status = 2 (downed)
         survivorstatusarray[mostRecentButton] = 2;
         //Set mostRecentAction = 3 (Chase an injured survivor)
         mostRecentAction = 3;
         //Log
         Debug.LogFormat("[DeadByDesklight #{0}] Survivor {1} has been downed.", ModuleId, mostRecentButton + 1);
          return;
         }
      
      //If down -> hook
      if (survivorstatusarray[mostRecentButton] == 2)
      {  //Set status = 1 (hooked)
         survivorstatusarray[mostRecentButton] = 1;
         //Set mostRecentAction = 2 (Hook a downed survivor)
         mostRecentAction = 2;
         //Log
         Debug.LogFormat("[DeadByDesklight #{0}] Survivor {1} has been hooked.", ModuleId, mostRecentButton + 1);
         //Increase hook count by hookcountincrement
         survivorhookcountarray[mostRecentButton] = (survivorhookcountarray[mostRecentButton] + hookcountincrement);
            //Check if survivor should die
            if (survivorhookcountarray[mostRecentButton] >= hookcountrequired)
            {  //Set status = 0 (dead)
            survivorstatusarray[mostRecentButton] = 0;
            Debug.LogFormat("[DeadByDesklight #{0}] Survivor {1} has died due to being hooked too much.", ModuleId, mostRecentButton + 1);
            }
         return;
       }

      //If hooked -> camp
      if (survivorstatusarray[mostRecentButton] == 1)
      { 
         //Set mostRecentAction = 1 (Camp a hooked survivor)
         mostRecentAction = 1;
         //Log
         Debug.LogFormat("[DeadByDesklight #{0}] Survivor {1} has been camped.", ModuleId, mostRecentButton + 1);
         //Increase hook count by hookcountincrement
         survivorhookcountarray[mostRecentButton] = (survivorhookcountarray[mostRecentButton] + hookcountincrement);
            //Check if survivor should die
            if (survivorhookcountarray[mostRecentButton] >= hookcountrequired)
            {  //Set status = 0 (dead)
            survivorstatusarray[mostRecentButton] = 0;
            Debug.LogFormat("[DeadByDesklight #{0}] Survivor {1} has died due to being hooked too much.", ModuleId, mostRecentButton + 1);
            } 
          return;
       }

      //If bear-trapped -> down
      if (survivorstatusarray[mostRecentButton] == 6)
      {  //Set status = 2 (downed)
         survivorstatusarray[mostRecentButton] = 2;
         //Set mostRecentAction = 6 (Chase a caught in bear-trap survivor)
         mostRecentAction = 6;
         //Log
         Debug.LogFormat("[DeadByDesklight #{0}] Survivor {1} has been downed.", ModuleId, mostRecentButton + 1);
          return;
      }
   }
   //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
   void OtherSurvivorLogic()
   {  //Logic that execute state changes for survivors that are not clicked on a stage
        
         //First count each survivors states for how many survivors there are for help with logic later

            //Reset their values
            healthyCount = 0;
            escapedCount = 0;
            hookedCount = 0;
            deadCount = 0;

            //For each survivor
            for (int i = 0; i < 4; i++) 
            {  //If status = 0 (dead)
               if (survivorstatusarray[i] == 0)
                  {//add 1 to deadCount
                  deadCount = deadCount + 1;
                  }  //Else if status = 1 (hooked)
               else if (survivorstatusarray[i] == 1)  
                  {//add 1 to hookedCount
                  hookedCount = hookedCount + 1;
                  }  //Else if status = 4 (healthy)
               else if (survivorstatusarray[i] == 4)  
                  {//add 1 to healthyCount
                  healthyCount = healthyCount + 1;
                 }  //Else if status = 5 (escaped)
               else if (survivorstatusarray[i] == 5)  
                  {//add 1 to escapedCount
                  escapedCount = escapedCount + 1;
                  } 
            }

         //Then main logic for each survivor
         for (int i = 0; i < 4; i++)   
         {
            //skip logic if survivor is most recent button cause their logic has already been done
            if (i == mostRecentButton){
               continue;
            }
            //skip logic if they are dead    0 = dead
            if (survivorstatusarray[i] == 0 ){
               continue;
            }
            //skip logic if they have escaped
            if (survivorstatusarray[i] == 5 ){
               continue;
            }

            //Otherwise, do some logic
               //Survivors only get to do 1 action, informed by their state and other general info (gen count), prioritised in order from top to bottom
               //Once an action has been done by a survivor, it skips to the next survivor

                  //1: If healthy, escape% if possible
                     //If status = 4 (healthy) and exit gates are powered (gens = 0)
                     if (survivorstatusarray[i] == 4 && generatorCount == 0){
                           //Roll some chance between 0 and 99 inclusive
                           chancevalue = UnityEngine.Random.Range(0,100);
                           //If the chance is less than required chance to escape
                           if (chancevalue < exitescapechance){
                              //Survivor escapes
                              survivorstatusarray[i] = 5;
                              Debug.LogFormat("[DeadByDesklight #{0}] Survivor {1} has escaped.", ModuleId, i + 1);
                              //And go to next survivor
                              continue;
                           }   
                       }
                        
                  //2: If healthy, repair%
                     //If status = 4 (healthy) and exit gates are not powered (gens >= 1)
                     if (survivorstatusarray[i] == 4 && generatorCount >= 1){
                           //Roll some chance between 0 and 99 inclusive
                           chancevalue = UnityEngine.Random.Range(0,100);
                           //If the chance is less than required chance to repair
                           if (chancevalue < generatorrepairchance){
                              //Generatorcount increases
                              generatorCount = generatorCount - 1;
                              Debug.LogFormat("[DeadByDesklight #{0}] Survivor {1} has repaired a generator.", ModuleId, i + 1);
                              Debug.LogFormat("[DeadByDesklight #{0}] Number of generators left to repair is {1}.", ModuleId, generatorCount);
                              //And go to next survivor
                              continue;
                           }
                       }

                  //3: If injured, heal%
                     //If status = 3 (injured) 
                     if (survivorstatusarray[i] == 3){
                           //Roll some chance between 0 and 99 inclusive
                           chancevalue = UnityEngine.Random.Range(0,100);
                           //If the chance is less than required chance to heal
                           if (chancevalue < healchance){
                              //Survivor is now healthy
                              survivorstatusarray[i] = 4;
                              Debug.LogFormat("[DeadByDesklight #{0}] Survivor {1} has healed from injured to healthy.", ModuleId, i + 1);
                              //And go to next survivor
                              continue;
                           }     
                       }
                       
                  //4: If downed, heal%
                     //If status = 2 (downed) 
                     if (survivorstatusarray[i] == 2){
                           //Roll some chance between 0 and 99 inclusive
                           chancevalue = UnityEngine.Random.Range(0,100);
                           //If the chance is less than required chance to heal
                           if (chancevalue < healchance){
                              //Survivor is now injured
                              survivorstatusarray[i] = 3;
                              Debug.LogFormat("[DeadByDesklight #{0}] Survivor {1} has healed from downed to injured.", ModuleId, i + 1);
                              //And go to next survivor
                              continue;
                           }     
                       }

                  //5: If hooked, unhook%
                     //If status = 1 (hooked) and hookedCount != (4 - (escapedCount + deadCount))
                                                   //this is to prevent unhooking when all remaining alive survivors are hooked and they should all die
                     if (survivorstatusarray[i] == 1 && hookedCount != (4 - (escapedCount + deadCount))){
                           //Roll some chance between 0 and 99 inclusive
                           chancevalue = UnityEngine.Random.Range(0,100);
                           //If the chance is less than required chance to heal
                           if (chancevalue < unhookchance){
                              //Survivor is now injured
                              survivorstatusarray[i] = 3;
                              Debug.LogFormat("[DeadByDesklight #{0}] Survivor {1} has unhooked and is now injured.", ModuleId, i + 1);
                              //And go to next survivor
                              continue;
                           }    
                       }

                  //6: If still hooked, increase hook count by hookcountincrement
                     //If status = 1 (hooked) 
                     if (survivorstatusarray[i] == 1){
                           //Increase hook count
                           survivorhookcountarray[i]  = (survivorhookcountarray[i] + hookcountincrement);
                       }
                     //6b: If hook count >= hookcountrequired, death
                        //
                        if (survivorhookcountarray[i] >= hookcountrequired){
                              //Survivor is now dead
                              survivorstatusarray[i]  = 0;
                              Debug.LogFormat("[DeadByDesklight #{0}] Survivor {1} has died due to being hooked too much.", ModuleId, i + 1);
                              //And go to next survivor
                              continue;
                           }
                     //6c: if you are still hooked, don't attempt further actions this stage
                        //If status = 1 (hooked)
                        if (survivorstatusarray[i] == 1){
                              //Go to next survivor
                              Debug.LogFormat("[DeadByDesklight #{0}] Survivor {1} has remained hooked.", ModuleId, i + 1);
                              continue;
                         }

                  //7: If bear-trapped, beartrapescape%
                     //If status = 6 (bear-trapped)
                     if (survivorstatusarray[i] == 6){
                           //Roll some chance between 0 and 99 inclusive
                           chancevalue = UnityEngine.Random.Range(0,100);
                           //If the chance is less than required chance to escape a bear-trap
                           if (chancevalue < beartrapescapechance){
                              //Survivor is now injured
                              survivorstatusarray[i] = 3;
                              Debug.LogFormat("[DeadByDesklight #{0}] Survivor {1} has escaped a bear-trap and is now injured.", ModuleId, i + 1);
                              //And go to next survivor
                              continue;
                           }    
                       }

                  //8: If killer = trapper, survivor is healthy, and there are active bear-traps, getTrapped%  
                    //If killervalue = 0 (Trapper) and status = 4 (healthy) and currentActiveBearTrapCount >= 1
                     if (killervalue == 0 && survivorstatusarray[i] == 4 && currentActiveBearTrapCount >= 1){
                           //Roll some chance between 0 and 99 inclusive
                           chancevalue = UnityEngine.Random.Range(0,100);
                           //If the chance is less than required chance to get caught in a bear-trap times by the number of bear-traps active
                           if (chancevalue < ((beartraptrappedchance)*(currentActiveBearTrapCount))){
                              //Survivor is now bear-trapped
                              survivorstatusarray[i] = 6;
                              Debug.LogFormat("[DeadByDesklight #{0}] Survivor {1} has become trapped in a bear-trap.", ModuleId, i + 1);
                              //Decrease active number of bear-traps by 1
                              currentActiveBearTrapCount = currentActiveBearTrapCount - 1;
                              Debug.LogFormat("[DeadByDesklight #{0}] Currently active bear-traps is now {1}.", ModuleId, currentActiveBearTrapCount);
                              //And go to next survivor
                              continue;
                           }    
                       } 

                  //Otherwise: do nothing
                  Debug.LogFormat("[DeadByDesklight #{0}] Survivor {1} has done nothing and remains {2}.", ModuleId, i + 1, survivorStatusNames[survivorstatusarray[i]]);
         }
   }
   //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
   void DetermineNextActionLogic(){
      //Choose the right action logic depending on which killer is currently active

      //Return if module is solved
      if (ModuleSolved == true){
         return;
      }

      //Call the right killer action logic
      if (killervalue == 0)
      {  //Call Trapper logic if Trapper
         DetermineActionLogicTrapper();  
         return;
      } 
      if (killervalue == 1)
      {  //Call Wraith logic if Wraith
         DetermineActionLogicWraith();
         return;
      }
      if (killervalue == 2)
      {  //Call Wraith logic if Hillbilly
         DetermineActionLogicHillbilly();
         return;
      }//More killers would go here if they were added 
   }
   //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
   void DetermineActionLogicTrapper(){
      //Prediction logic for Trapper

      //Predict the first priority to apply and call its relevant DeterminedAction function, then stop the prediction


         //1:If there's only one survivor left alive, and they are healthy or injured, chase them. 
            if(escapedCount + deadCount == 3){   
               //For each survivor
               for (int i = 0; i < 4; i++)   
               {  //If they are healthy
                  if(survivorstatusarray[i] == 4){
                     //Determined action chase healthy
                     Debug.LogFormat("[DeadByDesklight #{0}] Correct Trapper priority is 1.", ModuleId);
                     DeterminedActionChaseHealthy();
                     //Stop prediction
                     return;
                  }  //otherwise if they are injured
                  if(survivorstatusarray[i] == 3){
                      //Determined action chase injured
                     Debug.LogFormat("[DeadByDesklight #{0}] Correct Trapper priority is 1.", ModuleId);
                     DeterminedActionChaseInjured();
                     //Stop prediction
                     return;
                  } 
               }
            }

         //2: If there are no escapes and the exit gates are powered make sure there are atleast 2 active bear-traps if possible.
            //If escapedCount = 0 and generatorCount = 0 and there are less than 2 active bear-traps and it is possible to place a new bear-trap
            if (escapedCount == 0 && generatorCount == 0 && currentActiveBearTrapCount < 2 && currentActiveBearTrapCount < maxBearTrapCount )
            { //Determined action power Trapper
               Debug.LogFormat("[DeadByDesklight #{0}] Correct Trapper priority is 2.", ModuleId);
               DeterminedActionPowerTrapper();
               //Stop prediction
               return;
            }

         //3: If any survivors have escaped, hook any downed survivors
            //For each survivor
            for (int i = 0; i < 4; i++){
               //If status = 5 (escaped)
               if( survivorstatusarray[i] == 5){
                  //Then for each other survivor   (including yourself but that don't matter)
                  for (int j = 0; j < 4; j++){
                     //If their status = 2 (downed)
                     if(survivorstatusarray[j] == 2){
                        //Determined action hook downed
                        Debug.LogFormat("[DeadByDesklight #{0}] Correct Trapper priority is 3.", ModuleId);
                        DeterminedActionHookDowned();
                        //Stop prediction
                        return;
                     } 
                  }      
               }         
            }
            
         //4: Chase any survivors caught in bear-traps
            //For each survivor
            for (int i = 0; i < 4; i++){
               //If status = 6 (bear-trapped)
               if( survivorstatusarray[i] == 6){
                  //Determined action chase a bear-trapped survivor
                  Debug.LogFormat("[DeadByDesklight #{0}] Correct Trapper priority is 4.", ModuleId);
                  DeterminedActionChaseBearTrapped();
                  //Stop prediction
                  return;
               }
            }

         //5: Make sure there is atleast 1 active bear-trap
            //If there is less than 1 active bear-trap and it is possible to place a new bear-trap
            if(currentActiveBearTrapCount < 1 && currentActiveBearTrapCount < maxBearTrapCount ){
               //Determined action use Trapper Power
               Debug.LogFormat("[DeadByDesklight #{0}] Correct Trapper priority is 5.", ModuleId);
               DeterminedActionPowerTrapper();
               //Stop prediction
               return;
            }

         //6: Hook any downed survivors
            //For each survivor
            for (int i = 0; i < 4; i++){
               //If status = 2 (downed)
               if( survivorstatusarray[i] == 2){
                  //Determined action hook downed
                  Debug.LogFormat("[DeadByDesklight #{0}] Correct Trapper priority is 6.", ModuleId);
                  DeterminedActionHookDowned();
                  //Stop prediction
                  return;
               }
            }

         //7: If there are hooked survivors, and it is possible to do so, protect your sacrifice by making sure there are atleast 2 active bear-traps
            //For each survivor
            for (int i = 0; i < 4; i++){
               //if status = 1 (hooked)
               if( survivorstatusarray[i] == 1){
                     //If there are less than 2 active bear-traps and it is possible to place a new bear-trap
                     if(currentActiveBearTrapCount < 2 && currentActiveBearTrapCount < maxBearTrapCount ){
                      //Determined action power Trapper
                      Debug.LogFormat("[DeadByDesklight #{0}] Correct Trapper priority is 7.", ModuleId);
                       DeterminedActionPowerTrapper();
                      //Stop prediction
                      return;
                   }
               }  
            }

         //8: Chase any injured survivors
            //For each survivor
            for (int i = 0; i < 4; i++){
               //If status = 3 (injured)
               if( survivorstatusarray[i] == 3){
                  //Determined action chase injured
                  Debug.LogFormat("[DeadByDesklight #{0}] Correct Trapper priority is 8.", ModuleId);
                  DeterminedActionChaseInjured();
                  //Stop prediction
                  return;
               }
            }

         //9: Camp any hooked survivors
            //For each survivor
            for (int i = 0; i < 4; i++){
               //If status = 1 (hooked)
               if( survivorstatusarray[i] == 1){
                  //Determined action camp hooked
                  Debug.LogFormat("[DeadByDesklight #{0}] Correct Trapper priority is 9.", ModuleId);
                  DeterminedActionCampHooked();
                  //Stop prediction
                  return;
               }
            }

         //10: Make sure the maximum number of bear-traps are active
            //If active bear-traps is less than maximum bear-traps
            if( currentActiveBearTrapCount < maxBearTrapCount ){
               //Determined action power Trapper
               Debug.LogFormat("[DeadByDesklight #{0}] Correct Trapper priority is 10.", ModuleId);
               DeterminedActionPowerTrapper();
               //Stop prediction
               return;
            }

         //11: Chase healthy survivors
            //For each survivor
            for (int i = 0; i < 4; i++){
               //If status = 4 (healthy)
               if( survivorstatusarray[i] == 4){
                  //Determined action chase healthy
                  Debug.LogFormat("[DeadByDesklight #{0}] Correct Trapper priority is 11.", ModuleId);
                  DeterminedActionChaseHealthy();
                  //Stop prediction
                  return;
               }
            }

         //Should never get to this point, one of the above priorities should always apply 
            //So solve the module and log as an error
            Debug.LogFormat("[DeadByDesklight #{0}] ERROR: No Trapper priority found to be valid. FORCING MODULE SOLVE. PLEASE REPORT THIS AS A BUG.", ModuleId);
            ModuleSolved = true;
            GetComponent<KMBombModule>().HandlePass();
          
   }
   //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
   void DetermineActionLogicWraith(){
      //Prediction logic for Wraith

      //If last action was to injure, down or hook a survivor, immediately become uncloaked if you were cloaked
      if (cloakedStatus == 0){
         if(mostRecentAction == 4 || mostRecentAction == 3 || mostRecentAction == 2){
         cloakedStatus = 1;
         Debug.LogFormat("[DeadByDesklight #{0}] Uncloaking because last action was to injure, down or hook a survivor.", ModuleId);
         }
      }

      //Predict the first priority to apply and call its relevant DeterminedAction function, then stop the prediction

         //1:If your last action was to chase a healthy survivor, continue chasing them.
            //If mostRecentAction = 4 (Chase a healthy survivor)
            if (mostRecentAction == 4)
               { //Determined action is to chase them specifically (this skips determine action step)
                  Debug.LogFormat("[DeadByDesklight #{0}] Correct Wraith priority is 1.", ModuleId);
                  predictedButtonValue = mostRecentButton;     //press the same button
                  predictedActionValue = 4;                    //predictedActionValue = 4 (Chase a healthy survivor)
                  Debug.LogFormat("[DeadByDesklight #{0}] Correct button is survivor {1}.", ModuleId, predictedButtonValue + 1);
                  //Stop prediction
                  return;
               }

         //2:Hook any downed survivors
            //For each survivor
               for (int i = 0; i < 4; i++){
                  //If status = 2 (downed)
                  if( survivorstatusarray[i] == 2){
                     //Determined action hook downed
                     Debug.LogFormat("[DeadByDesklight #{0}] Correct Wraith priority is 2.", ModuleId);
                     DeterminedActionHookDowned();
                     //Stop prediction
                     return;
                  }
               }

         //3:If your most recent action was to hook a survivor, cloak yourself.
            //If mostRecentAction = 2 (Hook a downed survivor)
               if (mostRecentAction == 2)
                  { //Determined action is to use your killer power
                     Debug.LogFormat("[DeadByDesklight #{0}] Correct Wraith priority is 3.", ModuleId);
                      DeterminedActionPowerWraith();
                     //Stop prediction
                     return;
               }

         //4:Chase injured survivors if you are cloaked.
            //If you are cloaked
               if(cloakedStatus == 0){
                  //Then for each survivor
                  for (int i = 0; i < 4; i++){
                     //If status = 3 (injured)
                     if( survivorstatusarray[i] == 3){
                        //Determined action chase injured
                        Debug.LogFormat("[DeadByDesklight #{0}] Correct Wraith priority is 4.", ModuleId);
                        DeterminedActionChaseInjured();
                        //Stop prediction
                        return;
                     }
                  }
               }

         //5:If your most recent action was to camp a hooked survivor, then chase a healthy survivor if there are any and you are cloaked.
            //If you are cloaked and mostRecentAction = 1 (Camp a hooked survivor)
               if(cloakedStatus == 0 && mostRecentAction == 1){
                  //Then for each survivor
                  for (int i = 0; i < 4; i++){
                     //If status = 4 (healthy)
                     if( survivorstatusarray[i] == 4){
                        //Determined action chase healthy
                        Debug.LogFormat("[DeadByDesklight #{0}] Correct Wraith priority is 5.", ModuleId);
                        DeterminedActionChaseHealthy();
                        //Stop prediction
                        return;
                     }
                  }
               }

         //6:Camp any hooked survivors.
            //For each survivor
            for (int i = 0; i < 4; i++){
               //If status = 1 (hooked)
               if( survivorstatusarray[i] == 1){
                  //Determined action camp hooked
                  Debug.LogFormat("[DeadByDesklight #{0}] Correct Wraith priority is 6.", ModuleId);
                  DeterminedActionCampHooked();
                  //Stop prediction
                  return;
               }
            }

         //7:Chase healthy survivors if you are cloaked.
         if(cloakedStatus == 0){
            //For each survivor
            for (int i = 0; i < 4; i++){
               //If status = 4 (healthy)
               if( survivorstatusarray[i] == 4){
                  //Determined action chase healthy
                  Debug.LogFormat("[DeadByDesklight #{0}] Correct Wraith priority is 7.", ModuleId);
                  DeterminedActionChaseHealthy();
                  //Stop prediction
                  return;
               }
            }
         }

         //8:Use your killer power to become cloaked if you are not.
         if(cloakedStatus == 1){
            //Determined action is to use your killer power
            Debug.LogFormat("[DeadByDesklight #{0}] Correct Wraith priority is 8.", ModuleId);
            DeterminedActionPowerWraith();
            //Stop prediction
            return;

         }

         //Should never get to this point, one of the above priorities should always apply 
            //So solve the module and log as an error
            Debug.LogFormat("[DeadByDesklight #{0}] ERROR: No Wraith priority found to be valid. FORCING MODULE SOLVE. PLEASE REPORT THIS AS A BUG.", ModuleId);
            ModuleSolved = true;
            GetComponent<KMBombModule>().HandlePass();
         
   }
   //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
   void DetermineActionLogicHillbilly(){
      //Prediction logic for Hillbilly

      //Predict the first priority to apply and call its relevant DeterminedAction function, then stop the prediction

         //1: If you've just used your chainsaw power, and there is a healthy survivor, chase them.

            //If  mostRecentAction = 1 (Use killer icon)
            if(mostRecentAction == 5){
               //Then for each survivor
               for (int i = 0; i < 4; i++){
                  //If status = 4 (healthy)
                  if( survivorstatusarray[i] == 4){
                     //Determined action chase healthy
                     Debug.LogFormat("[DeadByDesklight #{0}] Correct Hillbilly priority is 1.", ModuleId);
                     DeterminedActionChaseHealthy();
                     //Stop prediction
                     return;
                  }
               }
            }

         //2:Hook downed survivors
            //For each survivor
            for (int i = 0; i < 4; i++){
               //If status = 2 (downed)
               if( survivorstatusarray[i] == 2){
                  //Determined action hook downed
                  Debug.LogFormat("[DeadByDesklight #{0}] Correct Hillbilly priority is 2.", ModuleId);
                  DeterminedActionHookDowned();
                  //Stop prediction
                  return;
               }
            }

         //3:Chase injured survivors
            //For each survivor
            for (int i = 0; i < 4; i++){
               //If status = 3 (injured)
               if( survivorstatusarray[i] == 3){
                  //Determined action chase injured
                  Debug.LogFormat("[DeadByDesklight #{0}] Correct Hillbilly priority is 3.", ModuleId);
                  DeterminedActionChaseInjured();
                  //Stop prediction
                  return;
               }
            }

         //4:If your most recent action was to hook a survivor, camp a survivor.
            //If mostRecentAction = 2 (Hook a downed survivor)
            if (mostRecentAction == 2){
               //For each survivor
               for (int i = 0; i < 4; i++){
                  //If status = 1 (hooked)
                  if( survivorstatusarray[i] == 1){
                     //Determined action is to camp a survivor
                     Debug.LogFormat("[DeadByDesklight #{0}] Correct Hillbilly priority is 4.", ModuleId);
                     DeterminedActionCampHooked();
                     //Stop prediction
                     return;
                  }
               }
            }   

         //5: Chase healthy survivors (and power check)
            //For each survivor
            for (int i = 0; i < 4; i++){
               //If status = 4 (healthy)
               if( survivorstatusarray[i] == 4){
                     //If you did not recently use your chainsaw
                     if(mostRecentAction != 5){
                        //Determined action chase healthy
                        Debug.LogFormat("[DeadByDesklight #{0}] Correct Hillbilly priority is 5, but you need to use your chainsaw before chasing a healthy survivor.", ModuleId);
                        DeterminedActionPowerHillbilly();
                        //Stop prediction
                        return;
                     }
                  //Determined action chase healthy
                  Debug.LogFormat("[DeadByDesklight #{0}] Correct Hillbilly priority is 5.", ModuleId);
                  DeterminedActionChaseHealthy();
                  //Stop prediction
                  return;
               }
            }

         //Should never get to this point, one of the above priorities should always apply 
            //So solve the module and log as an error
            Debug.LogFormat("[DeadByDesklight #{0}] ERROR: No Hillbilly priority found to be valid. FORCING MODULE SOLVE. PLEASE REPORT THIS AS A BUG.", ModuleId);
            ModuleSolved = true;
            GetComponent<KMBombModule>().HandlePass();
   }
   //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    void DeterminedActionHookDowned(){
      //Prediction logic if determined action is to hook a downed survivor

      //Set predictedActionValue = 2 (Hook a downed survivor)
      predictedActionValue = 2;

      //For each survivor
      for (int i = 0; i < 4; i++){
         //If status = 2 (downed)
         if (survivorstatusarray[i] == 2){
                  //reset k
                   int k = 0;
                  //Then for each other survivor   (including yourself but that don't matter)
                     for (int j = 0; j < 4; j++){
                      //If they are also downed and have a lower hook priority value  
                      if (survivorstatusarray[j] == 2 && survivorhookpriorityarray[j] < survivorhookpriorityarray[i]){  
                        //Then increment k
                        k = k + 1;
                        }
                      }
                  //Then if k has remained zero    
                   if (k == 0){
                  //You are the correct button to press
                  predictedButtonValue = i;
                  Debug.LogFormat("[DeadByDesklight #{0}] Correct button is survivor {1}.", ModuleId, predictedButtonValue + 1);
                  //Stop prediction
                  return;
               }
          }
      }
   }
   //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    void DeterminedActionChaseInjured(){
      //Prediction logic if determined action is to chase an injured survivor
      
      //Set predictedActionValue = 3 (Chase an injured survivor)
      predictedActionValue = 3;

      //For each survivor
      for (int i = 0; i < 4; i++){
         //If status = 3 (injured)
         if (survivorstatusarray[i] == 3){
                  //reset k
                   int k = 0;
                  //Then for each other survivor   (including yourself but that don't matter)
                     for (int j = 0; j < 4; j++){
                      //If they are also injured and have a lower down priority value  
                      if (survivorstatusarray[j] == 3 && survivordownpriorityarray[j] < survivordownpriorityarray[i]){  
                        //Then increment k
                        k = k + 1;
                        }
                      }
                  //Then if k has remained zero    
                   if (k == 0){
                  //You are the correct button to press
                  predictedButtonValue = i;
                  Debug.LogFormat("[DeadByDesklight #{0}] Correct button is survivor {1}.", ModuleId, predictedButtonValue + 1);
                  //Stop prediction
                  return;
               }
          }
      }
   }
   //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    void DeterminedActionChaseHealthy(){
      //Prediction logic if determined action is to chase a healthy survivor
      
      //Set predictedActionValue = 4 (Chase an healthy survivor)
      predictedActionValue = 4;

      //For each survivor
      for (int i = 0; i < 4; i++){
         //If status = 4 (healthy)
         if (survivorstatusarray[i] == 4){
                  //reset k
                   int k = 0;
                  //Then for each other survivor   (including yourself but that don't matter)
                     for (int j = 0; j < 4; j++){
                      //If they are also healthy and have a lower injure priority value  
                      if (survivorstatusarray[j] == 4 && survivorinjurepriorityarray[j] < survivorinjurepriorityarray[i]){  
                        //Then increment k
                        k = k + 1;
                        }
                      }
                  //Then if k has remained zero    
                   if (k == 0){
                  //You are the correct button to press
                  predictedButtonValue = i;
                  Debug.LogFormat("[DeadByDesklight #{0}] Correct button is survivor {1}.", ModuleId, predictedButtonValue + 1);
                  //Stop prediction
                  return;
               }
          }
      }
   }
   //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    void DeterminedActionCampHooked(){
      //Prediction logic if determined action is to camp a hooked survivor
      
      //Set predictedActionValue = 1 (Camp a healthy survivor)
      predictedActionValue = 1;

      //For each survivor
      for (int i = 0; i < 4; i++){
         //If status = 1 (hooked)
         if (survivorstatusarray[i] == 1){
                  //reset k
                   int k = 0;
                  //Then for each other survivor   (including yourself but that don't matter)
                     for (int j = 0; j < 4; j++){
                      //If they are also hooked and have a lower camp priority value  
                      if (survivorstatusarray[j] == 1 && survivorcamppriorityarray[j] < survivorcamppriorityarray[i]){  
                        //Then increment k
                        k = k + 1;
                        }
                      }
                  //Then if k has remained zero    
                   if (k == 0){
                  //You are the correct button to press
                  predictedButtonValue = i;
                  Debug.LogFormat("[DeadByDesklight #{0}] Correct button is survivor {1}.", ModuleId, predictedButtonValue + 1);
                  //Stop prediction
                  return;
               }
          }
      }
   }
   //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    void DeterminedActionPowerTrapper(){
      //Prediction logic if determined action is to use the Trapper power
      
      //Set predictedActionValue = 5 (Use killer power)
      predictedActionValue = 5;

      //There is only 1 valid button to press (button 4 = killer icon)
      predictedButtonValue = 4;
      Debug.LogFormat("[DeadByDesklight #{0}] Correct button is the Trapper killer icon.", ModuleId);
   }
   //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    void DeterminedActionPowerWraith(){
      //Prediction logic if determined action is to use the Wraith power

      //Set predictedActionValue = 5 (Use killer power)
      predictedActionValue = 5;

      //There is only 1 valid button to press (button 4 = killer icon)
      predictedButtonValue = 4;
      Debug.LogFormat("[DeadByDesklight #{0}] Correct button is the Wraith killer icon.", ModuleId);
   }
   //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    void DeterminedActionPowerHillbilly(){
      //Prediction logic if determined action is to use the Wraith power

      //Set predictedActionValue = 5 (Use killer power)
      predictedActionValue = 5;

      //There is only 1 valid button to press (button 4 = killer icon)
      predictedButtonValue = 4;
      Debug.LogFormat("[DeadByDesklight #{0}] Correct button is the Hillbilly killer icon.", ModuleId);
    }
   //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    void DeterminedActionChaseBearTrapped(){
      //Prediction logic if determined action is to chase a bear-trapped survivor

      //Set predictedActionValue = 6 (Chase a survivor caught in a bear-trap)
      predictedActionValue = 6;

      //For each survivor
      for (int i = 0; i < 4; i++){
         //If status = 6 (caught in bear-trap)
         if (survivorstatusarray[i] == 6){
            //reset k
            int k = 0;
            //Then for each other survivor   (including yourself but that don't matter)
            for (int j = 0; j < 4; j++)
               {//If they are also caught in a bear-trap and have a lower down priority value  
               if (survivorstatusarray[j] == 6 && survivordownpriorityarray[j] < survivordownpriorityarray[i])
                  {//Then increment k
                  k = k + 1;
                  }
               }
            //Then if k has remained zero    
            if (k == 0)
            {//You are the correct button to press
               predictedButtonValue = i;
               Debug.LogFormat("[DeadByDesklight #{0}] Correct button is survivor {1}.", ModuleId, predictedButtonValue + 1);
               //Stop prediction
               return;
            }
         }
      }
   }
   //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
   void PowerLogicTrapper()
   {  //Called when you press killer icon as Trapper

      //Set mostRecentAction = 5 (Use killer icon)
         mostRecentAction = 5;

      if(currentActiveBearTrapCount < maxBearTrapCount)
         {//if less than max bear-traps, increase current active bear-traps
         currentActiveBearTrapCount = currentActiveBearTrapCount + 1;
         //Sound effect
         audio.PlaySoundAtTransform("Trapper_trap_placed", transform);
         //Log new bear-trap count
         Debug.LogFormat("[DeadByDesklight #{0}] Current active bear-traps is now {1}.", ModuleId, currentActiveBearTrapCount);
         }
   }
   //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
   void PowerLogicWraith()
   {//Called when you press killer icon as Wraith

      //Set mostRecentAction = 5 (Use killer icon)
      mostRecentAction = 5;

      if(cloakedStatus == 1)
         {//if uncloaked, cloak yourself
         cloakedStatus = 0;
         Debug.LogFormat("[DeadByDesklight #{0}] You are now cloaked.", ModuleId);
         }
   }
   //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
   void PowerLogicHillbilly()
   {//Called when you press killer icon as Hillbilly

      //Set mostRecentAction = 5 (Use killer icon)
      mostRecentAction = 5;

      //If currently overheated, do nothing
      if(hillbillyOverheat == true){return;}

      //Do overheating check
         //If hillbillyCurrentTemperature is to close to maximum to increase by increment
         if (hillbillyCurrentTemperature > hillbillyTemperatureMaximum - hillbillyTemperatureIncrement)
         {//increase hillbillyCurrentTemperature to maximum 
         hillbillyCurrentTemperature = hillbillyTemperatureMaximum;
         //overheat - true
         hillbillyOverheat = true;
         Debug.LogFormat("[DeadByDesklight #{0}] Hillbilly chainsaw temperature now exceeds 99°C causing overheating.", ModuleId);
         //and disable the killer button
         disableKillerButton.SetActive(false);
         //make optional button material hillbilly_red
         killerOptionalButton.sharedMaterial = optionalInfoButtonTextureOptions[2];
         } 
         //If hillbillyCurrentTemperature can increase by the increment without reaching maximum
         if(hillbillyCurrentTemperature <= hillbillyTemperatureMaximum - hillbillyTemperatureIncrement)
            {//increase hillbillyCurrentTemperature by hillbillyTemperatureIncrement
            hillbillyCurrentTemperature = hillbillyCurrentTemperature + hillbillyTemperatureIncrement;
            Debug.LogFormat("[DeadByDesklight #{0}] Hillbilly chainsaw temperature increased to {1}°C.", ModuleId, hillbillyCurrentTemperature);
            }
  
   }
   //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
   void CheckIfDefused()
   {  //Check if the modules been defused
      
      int k = 0;
      //For each survivor   
      for (int i = 0; i < 4; i++){
        //If status = 0 (dead) or status = 1 (hooked) or status = 5 (escaped) 
          if (survivorstatusarray[i] == 0 || survivorstatusarray[i] == 1 || survivorstatusarray[i] == 5 ){  
            //Then increment k
            k = k + 1;
             }
      }
      //Then if k = 4   
      if (k == 4){
         //Log it
         Debug.LogFormat("[DeadByDesklight #{0}] All survivors are now dead, escaped or hooked.", ModuleId);
         //Make sure all hooked survivors immediately die
            //For each survivor   
            for (int i = 0; i < 4; i++){
               //If status = 1 (hooked)
               if (survivorstatusarray[i] == 1 ){  
                 //Make them dead instead 
                 survivorstatusarray[i] = 0; 
               }
            }
         //The module is solved
         ModuleSolved = true;
         GetComponent<KMBombModule>().HandlePass();
       }
   }
   //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
   void imageupdate()
   {//Purpose is to make the visuals of the module line up with the module's code
      //Called at Start after every other setup and every button press after all other logic has occured

         //update survivor images one at a time
            //portraitimage is used to identify survivor name, statusimage used to identify survivor status

            //survivor0
               //if healthy
               if (survivorstatusarray[0] == 4)
                 {  //set image as portraitimage
               button0image.sharedMaterial = portraitOptions[survivorvaluearray[0]];
                 }  
              else  //else if not healthy
                 {  //set image as statusimage
               button0image.sharedMaterial = survivorStatusOptions[survivorstatusarray[0]];
                  }

            //survivor1
               //if healthy
               if (survivorstatusarray[1] == 4)
                 {  //set image as portraitimage
               button1image.sharedMaterial = portraitOptions[survivorvaluearray[1]];
                 }  
              else  //else if not healthy
                 {  //set image as statusimage
               button1image.sharedMaterial = survivorStatusOptions[survivorstatusarray[1]];
                  }

            //survivor2
               //if healthy
               if (survivorstatusarray[2] == 4)
                 {  //set image as portraitimage
               button2image.sharedMaterial = portraitOptions[survivorvaluearray[2]];
                 }  
              else  //else if not healthy
                 {  //set image as statusimage
               button2image.sharedMaterial = survivorStatusOptions[survivorstatusarray[2]];
                  }

            //survivor3
               //if healthy
               if (survivorstatusarray[3] == 4)
                 {  //set image as portraitimage
               button3image.sharedMaterial = portraitOptions[survivorvaluearray[3]];
                 }  
              else  //else if not healthy
                 {  //set image as statusimage
               button3image.sharedMaterial = survivorStatusOptions[survivorstatusarray[3]];
                  }

         
         //Generator button
             //Generator image
             generatorimage.sharedMaterial = generatorOptions[generatorCount];

            //Generator text
            generatortext.text = "" + generatorCount;

         //killer images
            //killer icon
             //killer icon not updated rn, its currently only set at killersetup


            //killer optional info

               //Trapper
                  if (killervalue == 0){ 
                        //Update optional text to be active bear-trap count
                        killeroptionaltext.text = "" + currentActiveBearTrapCount;
                 }

               //Wraith
                  if (killervalue == 1) {
                        //Cloaked status image
                           //If cloaked, make it invisible
                           if(cloakedStatus == 0){
                              killerOptionalImage.sharedMaterial = optionalInfoImageTextureOptions[0];
                              }
                           //if uncloaked, make it a picture of the wraith
                           if(cloakedStatus == 1){
                              killerOptionalImage.sharedMaterial = optionalInfoImageTextureOptions[1];
                              }
                  }
   } 
   //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
   private IEnumerator mycountdownCoroutine()
   {
      //While bomb is not solved
      while(ModuleSolved == false){
         //if hillbillyCurrentTemperature is > 0
         if (hillbillyCurrentTemperature > 0)
         {//reduce the countdown by 1 every second
             if (timeInteger != (int)bomb.GetTime()){
               hillbillyCurrentTemperature = hillbillyCurrentTemperature - 1;
              }
         }

         //Then If temperature is zero and overheat was true
         if (hillbillyCurrentTemperature <= 0 && hillbillyOverheat == true)
            {//set overheat to false (not overheated)
               hillbillyOverheat = false;
               Debug.LogFormat("[DeadByDesklight #{0}] Hillbilly chainsaw has reached 0°C and is now no longer overheated.", ModuleId);   
            //and re-enable the killer button
            disableKillerButton.SetActive(true);
            //and return button material to dbd_brown  
             killerOptionalButton.sharedMaterial = optionalInfoButtonTextureOptions[1];
                }
        //Update the text        
         killeroptionaltext.text = "" + hillbillyCurrentTemperature + "°C"; 
         //update timeInteger to current time
         timeInteger = (int)bomb.GetTime(); 
         yield return new WaitForSeconds(0.05f);     
      }
      
   }
      
}



