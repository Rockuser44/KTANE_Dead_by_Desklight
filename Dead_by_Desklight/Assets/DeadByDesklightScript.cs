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

   // Add texts generator count
   public TextMesh generatortext;
   // Add texts optionskillertexts
   public TextMesh killeroptionaltext;

   // Add options for images
   public Material[] survivorStatusOptions;
   public Material[] portraitOptions;
   public Material[] killerOptions;
   public Material[] generatorOptions;
   public Material[] optionalInfoTextureOptions;
   public Renderer button0image;
   public Renderer button1image;
   public Renderer button2image;
   public Renderer button3image;
   public Renderer button4image;
   public Renderer generatorimage;
   public Renderer killerOptionalInfo;

   //private variables

   //Survivor Variables
   
      //name list only for documentation, not actually used
      string[] survivorNames = {"Dwight Fairfield",  "Meg Thomas", "Claudette Morel", "Jake Park", "Nea Karlsson", "Laurie Strode", "Ace Visconti", "William 'Bill' Overbeck"};
   
      //Survivor status values            4 because 4 survivors
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
      //Survivor portrait values
      int[] survivorvaluearray = new int[4];
         //survivor value used to determine name, associated priorities and the healthy portrait image

      //Survivor injure priority values
      int[] injureprioritylist = {1, 2, 3, 4, 5, 6, 7 ,8};
      decimal[] survivorinjurepriorityarray = new decimal[4];
 
      //Survivor down priority values
      int[] downprioritylist = {5, 6, 7, 8, 1, 2, 3, 4};
      decimal[] survivordownpriorityarray = new decimal[4];

      //Survivor hook priority values
      int[] hookprioritylist = {8, 7, 6, 5, 4, 3, 2, 1};
      decimal[] survivorhookpriorityarray = new decimal[4];

      //Survivor camp priority values
      int[] campprioritylist = {4, 3, 2, 1, 8, 7, 6, 5};
      decimal[] survivorcamppriorityarray = new decimal[4];

      //Survivor hook count
      decimal[] survivorhookcountarray = new decimal[4];

      //Count variables for other survivor logic
      private int healthyCount = 0;
      private int injuredCount = 0;
      private int escapedCount = 0;

   //Killer variables
      private int killervalue = 0;
      private bool killergenerated;

      //name list only for documentation, not actually used
      string[] killerNames = {"Trapper",  "Wraith"};

      //Trapper Variables
      private int maxBearTrapCount = 0;
      private int currentActiveBearTrapCount = 0;

      //Wraith Variables
      private int cloakedStatus = 1; //1 = cloaked, 0 = uncloaked


   //Generator variables
      private int generatorCount = 5; 

   //Other random variables
      private int predictedActionValue = 0;
         /*/predictedActionValue is an integer that represents the following possible actions:
            0 = Not Determined (aka error value)
            1 = Hook a downed survivor
            2 = Chase an injured survivor
            3 = Chase a healthy survivor
            4 = Camp a hooked survivor
            5 = Use killer power
            6 = Chase a survivor caught in bear-trap (Trapper)
         /*/   
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
      private decimal healchance = 30m;               //chance of an injured or downed survivor to heal themselves if not interacted with by the killer
      private decimal unhookchance = 20m;             //chance of a hooked survivor to unhook themselves if not camped by the killer
         //Trapper chances
         private decimal beartrapescapechance = 40m;     //chance of a bear-trapped survivor to free themselves if not interacted with by the killer
         private decimal beartraptrappedchance = 5m;     //chance of a healthy survivor to trap themselves in a bear trap if not interacted with by the killer, and if they don't repair a generator



   static int ModuleIdCounter = 1;
   int ModuleId;
   private bool ModuleSolved;

   //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
   void Awake () {
      ModuleId = ModuleIdCounter++;

      // Make functions called PressButtonX occur when you press buttonX
      button0.OnInteract += delegate () { PressButton0(); return false; };    //Survivor0 button
      button1.OnInteract += delegate () { PressButton1(); return false; };    //Survivor1 button
      button2.OnInteract += delegate () { PressButton2(); return false; };    //Survivor2 button
      button3.OnInteract += delegate () { PressButton3(); return false; };    //Survivor3 button
      button4.OnInteract += delegate () { PressButton4(); return false; };    //Killer icon button

   }
   //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
   void Start () {

      //Setup survivors
      SurvivorsSetup();
  
      //Setup killer
      KillerSetup();

      //No generator setup needed, is setup in variable declaration and imageupdate

      //update images
      imageupdate();

      //Call first prediction
      DetermineNextActionLogic();
   }
   //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
   void Update () {
      //Currently does nothing
   }
   //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
   void PressButton0()
   {    //Pressed survivor 0
            //Log press
            Debug.LogFormat("[DeadByDesklight #{0}] You pressed the first survivor button.", ModuleId);

      //Check if button was correct, strike if wrong
      mostRecentButton = 0;
         //If Button0 was not the predicted button to press
         if (mostRecentButton != predictedButtonValue){
            //Then strike the module
            GetComponent<KMBombModule>().HandleStrike();
            //But continue on anyway
         }

      //Make the button not do anything if survivor shouldn't be interacted with   
         //If survivor0 is dead or escaped
         if(survivorstatusarray[0] == 0 || survivorstatusarray[0] == 5){
            //Return
            return;
         }

      //Take action on clicked survivor
      ClickedSurvivorLogic();

      //Logic other survivors
      OtherSurvivorLogic();

      //Check if defused
      CheckIfDefused();

      //Image refresh
      imageupdate();

      //Call next prediction
      DetermineNextActionLogic();
   }
   //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
   void PressButton1()
   {    //Pressed survivor 1
            //Log press
            Debug.LogFormat("[DeadByDesklight #{0}] You pressed the second survivor button.", ModuleId);


     //Check if button was correct, strike if wrong
      mostRecentButton = 1;
       //If Button1 was not the predicted button to press
         if (mostRecentButton != predictedButtonValue){
            //Then strike the module
            GetComponent<KMBombModule>().HandleStrike();
            //But continue on anyway
         }

     //Make the button not do anything if survivor shouldn't be interacted with   
         //If survivor1 is dead or escaped
         if(survivorstatusarray[1] == 0 || survivorstatusarray[1] == 5){
            //Return
            return;
         }

    //Take action on clicked survivor
      ClickedSurvivorLogic();

      //Logic other survivors
      OtherSurvivorLogic();

      //Check if defused
      CheckIfDefused();

      //Image refresh
      imageupdate();

      //Call next prediction
      DetermineNextActionLogic();
   }
   //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
   void PressButton2()
   {  //Pressed survivor 2
         //Log press
         Debug.LogFormat("[DeadByDesklight #{0}] You pressed the third survivor button.", ModuleId);
      

      //Check if button was correct, strike if wrong
      mostRecentButton = 2;
       //If Button2 was not the predicted button to press
         if (mostRecentButton != predictedButtonValue){
            //Then strike the module
            GetComponent<KMBombModule>().HandleStrike();
            //But continue on anyway
         }

      //Make the button not do anything if survivor shouldn't be interacted with   
         //If survivor0 is dead or escaped
         if(survivorstatusarray[2] == 0 || survivorstatusarray[2] == 5){
            //Return
            return;
         }

      //Take action on clicked survivor
      ClickedSurvivorLogic();

      //Logic other survivors
      OtherSurvivorLogic();

      //Check if defused
      CheckIfDefused();

      //Image refresh
      imageupdate();

      //Call next prediction
      DetermineNextActionLogic();
   }
   //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
   void PressButton3()
   {  //Pressed survivor 3
         //Log press
         Debug.LogFormat("[DeadByDesklight #{0}] You pressed the fourth survivor button.", ModuleId);
      
      //Check if button was correct, strike if wrong
      mostRecentButton = 3;
       //If Button3 was not the predicted button to press
         if (mostRecentButton != predictedButtonValue){
            //Then strike the module
            GetComponent<KMBombModule>().HandleStrike();
            //But continue on anyway
         }

      //Make the button not do anything if survivor shouldn't be interacted with   
         //If survivor0 is dead or escaped
         if(survivorstatusarray[3] == 0 || survivorstatusarray[3] == 5){
            //Return
            return;
         }
   
      //Take action on clicked survivor
      ClickedSurvivorLogic();

      //Logic other survivors
      OtherSurvivorLogic();

      //Check if defused
      CheckIfDefused();

      //Image refresh
      imageupdate();

      //Call next prediction
      DetermineNextActionLogic();
   }
   //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
   void PressButton4()
   {  //Pressed killer power
         //Log press
         Debug.LogFormat("[DeadByDesklight #{0}] You pressed the killer icon button.", ModuleId);

      //Check if button was correct, strike if wrong
      mostRecentButton = 4;
       //If Button4 was not the predicted button to press
         if (mostRecentButton != predictedButtonValue){
            //Then strike the module
            GetComponent<KMBombModule>().HandleStrike();
            //But continue on anyway
         }

       //If all survivors are dead or escaped, make the button not do anything
         int k = 0;
         //For each other survivor   
         for (int i = 0; i < 4; i++){
            //If status = 0 (dead) or status = 5 (escaped)
            if (survivorstatusarray[i] == 0 || survivorstatusarray[i] == 5 ){  
            //Then increment k
            k = k + 1;
             }
          }
         //Then if k = 4   
         if (k == 4){
            //reurn
            return;
          }

      //Take killer action
      if (killervalue == 0)
      {  //Do Trapper Action if Trapper
         PowerLogicTrapper();
      }
      else if (killervalue == 1)
      {  //Do Wraith Action if Wraith
         PowerLogicWraith();
      } //Other killers would go here

      //Logic survivors
      OtherSurvivorLogic();

      //Check if defused
      CheckIfDefused();

      //Image refresh
      imageupdate();

      //Call next prediction
      DetermineNextActionLogic();
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
      //killervalue = UnityEngine.Random.Range(0,2);
      killervalue = 0;   //force value for testing purposes
   
         //set up killer image
         button4image.sharedMaterial = killerOptions[killervalue];

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
      }//Other killers would go here    
   }
   //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
   void TrapperSetup()
   {//Initial setup for Trapper

      //Get maxBearTrapCount
         //Start with 3
         maxBearTrapCount = 3;

         //check 1st cha for indoor map
         //if digital root + 1 = 1 or 2, maxBearTrapCount decrease by 1
         if (bomb.GetSerialNumber().First() == 'A' ||  bomb.GetSerialNumber().First() == 'J' ||  bomb.GetSerialNumber().First() == 'S'||  bomb.GetSerialNumber().First() == '0'|| bomb.GetSerialNumber().First() == '1')
         {
               maxBearTrapCount = maxBearTrapCount - 1;
         }
         //check 2nd cha for add-on
         //if digital root + 1 = 1 or 2, maxBearTrapCount increase by 2
         if (bomb.GetSerialNumber().ToArray()[1] == 'A' ||  bomb.GetSerialNumber().ToArray()[1] == 'J' ||  bomb.GetSerialNumber().ToArray()[1] == 'S'||  bomb.GetSerialNumber().ToArray()[1] == '0'|| bomb.GetSerialNumber().ToArray()[1] == '1')
         {
               maxBearTrapCount = maxBearTrapCount + 2;
         }
         //if digital root + 1 = 3 or 4, maxBearTrapCount increase by 1
          if (bomb.GetSerialNumber().ToArray()[1] == 'B' || bomb.GetSerialNumber().ToArray()[1] == 'C' || bomb.GetSerialNumber().ToArray()[1] == 'K' || bomb.GetSerialNumber().ToArray()[1] == 'L' || bomb.GetSerialNumber().ToArray()[1] == 'T'|| bomb.GetSerialNumber().ToArray()[1] == 'U'|| bomb.GetSerialNumber().ToArray()[1] == '2'|| bomb.GetSerialNumber().ToArray()[1] == '3')
         {
               maxBearTrapCount = maxBearTrapCount + 1;
         }
   }
   //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
   void WraithSetup()
   {//Initial setup for Wraith

      //Remove optional info
      killerOptionalInfo.sharedMaterial = optionalInfoTextureOptions[0];
      killeroptionaltext.text = "";
   }
   //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
   void ClickedSurvivorLogic()
   {//state logic for the survivor that was clicked 

      //If healthy -> injure
      if (survivorstatusarray[mostRecentButton] == 4)
      {  //Set status = 3 (injured)
         survivorstatusarray[mostRecentButton] = 3;
   
          return;
         }
      
      //If injured -> down   
      if (survivorstatusarray[mostRecentButton] == 3)
      {  //Set status = 2 (downed)
         survivorstatusarray[mostRecentButton] = 2;
   
          return;
         }
      
      //If down -> hook
      if (survivorstatusarray[mostRecentButton] == 2)
      {  //Set status = 1 (hooked)
         survivorstatusarray[mostRecentButton] = 1;
            //Increase hook count by 1
         survivorhookcountarray[mostRecentButton] = (survivorhookcountarray[mostRecentButton] + hookcountincrement);
            //Check if survivor should die
            if (survivorhookcountarray[mostRecentButton] >= hookcountrequired)
            {  //Set status = 0 (dead)
            survivorstatusarray[mostRecentButton] = 0;
            }
            
          return;
       }

      //If hooked -> camp
      if (survivorstatusarray[mostRecentButton] == 1)
      {  //Increase hook count by hookcountincrement
         survivorhookcountarray[mostRecentButton] = (survivorhookcountarray[mostRecentButton] + hookcountincrement);
            //Check if survivor should die
            if (survivorhookcountarray[mostRecentButton] >= hookcountrequired)
            {  //Set status = 0 (dead)
            survivorstatusarray[mostRecentButton] = 0;
            }
            
          return;
       }

      //If bear trapped -> down
      if (survivorstatusarray[mostRecentButton] == 6)
      {  //Set status = 2 (downed)
         survivorstatusarray[mostRecentButton] = 2;
   
          return;
         }
   }
   //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
   void OtherSurvivorLogic()
   {  //Logic that execute state changes for survivors that are not clicked on a stage
        
         //First count the amount of healthy and injured survivors there are for help with logic later

            //Reset their values
            healthyCount = 0;
            injuredCount = 0;

            //For each survivor
            for (int i = 0; i < 4; i++) 
            {  //If status = 4 (healthy)
               if (survivorstatusarray[i] == 4)
                  {//add 1 to healthyCount
                  healthyCount = healthyCount + 1;
                 }   //Else if status = 3 (injured)
               else if (survivorstatusarray[i] == 3)
                  {//add 1 to injuredCount
                  injuredCount = injuredCount + 1;
                  }  //Else if status = 0 (escaped)
               else if (survivorstatusarray[i] == 0)  
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
                              //And go to next survivor
                              continue;
                           }   
                       }
                        
                  //2: If healthy, repair%
                     //If status = 4 (healthy) and exit gates are not powered (gens >= 1)
                     if (survivorstatusarray[i] == 4 && generatorCount >= 0){
                           //Roll some chance between 0 and 99 inclusive
                           chancevalue = UnityEngine.Random.Range(0,100);
                           //If the chance is less than required chance to repair
                           if (chancevalue < generatorrepairchance){
                              //Generatorcount increases
                              generatorCount = generatorCount - 1;
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
                              //And go to next survivor
                              continue;
                           }     
                       }

                  //5: If hooked, unhook%
                     //If status = 1 (hooked) 
                     if (survivorstatusarray[i] == 1){
                           //Roll some chance between 0 and 99 inclusive
                           chancevalue = UnityEngine.Random.Range(0,100);
                           //If the chance is less than required chance to heal
                           if (chancevalue < unhookchance){
                              //Survivor is now injured
                              survivorstatusarray[i] = 3;
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
                              //And go to next survivor
                              continue;
                           }
                     //6c: if you are still hooked, don't attempt further actions this stage
                        //If status = 1 (hooked)
                        if (survivorstatusarray[i] == 1){
                              //Go to next survivor
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
                              //And go to next survivor
                              continue;
                           }    
                       }

                  //8: If killer = trapper, survivor is healthy, and there are active bear traps, getTrapped%  
                    //If killervalue = 0 (Trapper) and status = 4 (healthy) and currentActiveBearTrapCount >= 1
                     if (killervalue == 0 && survivorstatusarray[i] == 4 && currentActiveBearTrapCount >= 1){
                           //Roll some chance between 0 and 99 inclusive
                           chancevalue = UnityEngine.Random.Range(0,100);
                           //If the chance is less than required chance to get caught in a bear-trap times by the number of bear traps active
                           if (chancevalue < ((beartraptrappedchance)*(currentActiveBearTrapCount))){
                              //Survivor is now bear-trapped
                              survivorstatusarray[i] = 6;
                              //Decrease active number of bear traps by 1
                              currentActiveBearTrapCount = currentActiveBearTrapCount - 1;
                              //And go to next survivor
                              continue;
                           }    
                       } 

                  //Otherwise: do nothing
         }
   }
   //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
   void DetermineNextActionLogic(){
      //Choose the right action logic depending on which killer is currently active

      //Call the right killer action logic
      if (killervalue == 0)
      {  //Call Trapper logic if Trapper
         DetermineActionLogicTrapper();  
         return;
      } 
      else if (killervalue == 1)
      {  //Call Wraith logic if Wraith
         DetermineActionLogicWraith();
         return;
      }//More killers would go here if they were added    
   }
   //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
   void DetermineActionLogicTrapper(){
      //Prediction logic for Trapper

      //Predict the first priority to apply and call its relevant DeterminedAction function, then stop the prediction

         //1: If there are no escapes and the exit gates are powered make sure there are atleast 2 active bear traps if possible.
            //If escapedCount = 0 and generatorCount = 0 and there are less than 2 active bear traps and it is possible to place a new bear trap
            if (escapedCount == 0 && generatorCount == 0 && currentActiveBearTrapCount < 2 && currentActiveBearTrapCount < maxBearTrapCount )
            { //Determined action power Trapper
               DeterminedActionPowerTrapper();
               //Stop prediction
               return;
            }

         //2: If any survivors have escaped, hook any downed survivors
            //For each survivor
            for (int i = 0; i < 4; i++){
               //If status = 0 (escaped)
               if( survivorstatusarray[i] == 0){
                  //Then for each other survivor   (including yourself but that don't matter)
                  for (int j = 0; j < 4; j++){
                     //If their status = 2 (downed)
                     if(survivorstatusarray[j] == 2){
                        //Determined action hook downed
                        DeterminedActionHookDowned();
                        //Stop prediction
                        return;
                     } 
                  }      
               }         
            }
            
         //3: Chase any survivors caught in bear traps
            //For each survivor
            for (int i = 0; i < 4; i++){
               //If status = 6 (bear-trapped)
               if( survivorstatusarray[i] == 6){
                  //Determined action chase a bear-trapped survivor
                  DeterminedActionChaseBearTrapped();
                  //Stop prediction
                  return;
               }
            }

         //4: Make sure there is atleast 1 active bear trap
            //If there is less than 1 active bear trap and it is possible to place a new bear trap
            if(currentActiveBearTrapCount < 1 && currentActiveBearTrapCount < maxBearTrapCount ){
               //Determined action use Trapper Power
               DeterminedActionPowerTrapper();
               //Stop prediction
               return;
            }

         //5: Hook any downed survivors
            //For each survivor
            for (int i = 0; i < 4; i++){
               //If status = 2 (downed)
               if( survivorstatusarray[i] == 2){
                  //Determined action hook downed
                  DeterminedActionHookDowned();
                  //Stop prediction
                  return;
               }
            }

         //6: If there are hooked survivors, and it is possible to do so, protect your sacrifice by making sure there are atleast 2 active bear traps
            //For each survivor
            for (int i = 0; i < 4; i++){
               //if status = 1 (hooked)
               if( survivorstatusarray[i] == 1){
                     //If there are less than 2 active bear traps and it is possible to place a new bear trap
                     if(currentActiveBearTrapCount < 2 && currentActiveBearTrapCount < maxBearTrapCount ){
                      //Determined action power Trapper
                       DeterminedActionPowerTrapper();
                      //Stop prediction
                      return;
                   }
               }  
            }

         //7: Chase any injured survivors
             //For each survivor
            for (int i = 0; i < 4; i++){
               //If status = 3 (injured)
               if( survivorstatusarray[i] == 3){
                  //Determined action hook downed
                  DeterminedActionChaseInjured();
                  //Stop prediction
                  return;
               }
            }

         //8: Camp any hooked survivors
            //For each survivor
            for (int i = 0; i < 4; i++){
               //If status = 1 (hooked)
               if( survivorstatusarray[i] == 1){
                  //Determined action camp hooked
                  DeterminedActionCampHooked();
                  //Stop prediction
                  return;
               }
            }

         //9: Make sure the maximum number of bear traps are active
            //If active bear traps is less than maximum bear traps
            if( currentActiveBearTrapCount < maxBearTrapCount ){
               //Determined action power Trapper
               DeterminedActionPowerTrapper();
               //Stop prediction
               return;
            }

         //10: Chase healthy survivors
            //For each survivor
            for (int i = 0; i < 4; i++){
               //If status = 4 (healthy)
               if( survivorstatusarray[i] == 4){
                  //Determined action chase healthy
                  DeterminedActionChaseHealthy();
                  //Stop prediction
                  return;
               }
            }

         //Should never get to this point, one of the above priorities should always apply 
            //So make predictions = error values
            predictedActionValue = 0;
            predictedButtonValue = 5;
          
   }
   //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
   void DetermineActionLogicWraith(){
      //Prediction logic for Wraith
      //TBD
   }
   //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    void DeterminedActionHookDowned(){
      //Prediction logic if determined action is to hook a downed survivor

      //Set predictedActionValue = 1 (Hook a downed survivor)
      predictedActionValue = 1;

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
                  //Stop prediction
                  return;
               }
          }
      }
   }
   //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    void DeterminedActionChaseInjured(){
      //Prediction logic if determined action is to chase an injured survivor
      
      //Set predictedActionValue = 2 (Chase an injured survivor)
      predictedActionValue = 2;

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
                  //Stop prediction
                  return;
               }
          }
      }
   }
   //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    void DeterminedActionChaseHealthy(){
      //Prediction logic if determined action is to chase a healthy survivor
      
      //Set predictedActionValue = 3 (Chase an healthy survivor)
      predictedActionValue = 3;

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
                  //Stop prediction
                  return;
               }
          }
      }
   }
   //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    void DeterminedActionCampHooked(){
      //Prediction logic if determined action is to camp a hooked survivor
      
      //Set predictedActionValue = 4 (Chase an healthy survivor)
      predictedActionValue = 4;

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
   }
   //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    void DeterminedActionPowerWraith(){
      //Prediction logic if determined action is to use the Wraith power

      //Set predictedActionValue = 5 (Use killer power)
      predictedActionValue = 5;

      //There is only 1 valid button to press (button 4 = killer icon)
      predictedButtonValue = 4;
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
                     for (int j = 0; j < 4; j++){
                      //If they are also caught in a bear trap and have a lower down priority value  
                      if (survivorstatusarray[j] == 6 && survivordownpriorityarray[j] < survivordownpriorityarray[i]){  
                        //Then increment k
                        k = k + 1;
                        }
                      }
                  //Then if k has remained zero    
                   if (k == 0){
                  //You are the correct button to press
                  predictedButtonValue = i;
                  //Stop prediction
                  return;
               }
          }
      }
   }
   //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
   void PowerLogicTrapper()
   {
      if(currentActiveBearTrapCount < maxBearTrapCount)
      {//if less than max bear traps, increase current active bear traps
      currentActiveBearTrapCount = currentActiveBearTrapCount + 1;
      }
   }
   //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
   void CheckIfDefused()
   {  //Check if the modules been defused
      
      int k = 0;
       //For each other survivor   
      for (int i = 0; i < 4; i++){
        //If status = 0 (dead) or status = 5 (escaped)
          if (survivorstatusarray[i] == 0 || survivorstatusarray[i] == 5 ){  
            //Then increment k
            k = k + 1;
             }
      }
       //Then if k = 4   
      if (k == 4){
         //The module is solved
         ModuleSolved = true;
         GetComponent<KMBombModule>().HandlePass();
       }

      //If all survivors are dead or escaped
         //ModuleSolved = true;
      //Otherwise don't do anything
      
   }
   //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
   void PowerLogicWraith()
   {
      if(cloakedStatus == 0)
      {//if uncloaked, cloak yourself
      cloakedStatus = 1;
      }
   }
   //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
   void imageupdate()
   {
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
             //killer image not updated rn, its currently only set at killersetup

             if (killervalue==0)
             { //If trapper, update optional text to be active bear trap count
               killeroptionaltext.text = "" + currentActiveBearTrapCount;
             }
   } 
}
