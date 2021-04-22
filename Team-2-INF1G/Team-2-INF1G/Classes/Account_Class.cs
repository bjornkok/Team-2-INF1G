﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using Formatting = Newtonsoft.Json.Formatting;
using JsonIgnoreAttribute = Newtonsoft.Json.JsonIgnoreAttribute;
using Hashing_Class;

namespace Account_Class
{
    public class Account
    {
        //// struct is a data type used for the serialization and deserialization of the JSON file.
        // It will be put in a list, that can be index with []
        // To call the value's in it, use the name and then the value name (accountDataList[0].Age)
        // It is the same as a Tuple, but it works like a class as reference (acc.accountDataList[0].Age)
        // You can also use a foreach to loop through all the items in the list.
            //foreach (var accountData in accountDataList)
            //{
            //Console.WriteLine(accountData.Password);
            //}

        public struct AccountData
        {
            public string Name;
            public string Password;
            
            public int Age;
            public string Gender;
            public string Email;
            public string bankingDetails;
            public string[] Allergies;
            public bool Permission;
        }

        //// Field
        // Creates a list, a data type that can be added upon, with the "AccountData" type
        public List<AccountData> accountDataList = new List<AccountData>();

        [JsonIgnore]
        public string jsonPath;
        [JsonIgnore]
        public string path; 

        // Constructor
        public Account()
        {
            // this.path is used in serializing the json data.
            this.path = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\..\Accounts.json"));
            // this.jsonPath is used in deserializing the json data.
            this.jsonPath = File.ReadAllText(path);
            // the constructor loads the json file first, so it can be modified later in the file.
            this.accountDataList = JsonConvert.DeserializeObject<List<AccountData>>(jsonPath);

        }

        // Method that is used to write data to the JSON file.
        public string ToJSON()
        {
            return JsonConvert.SerializeObject(this.accountDataList, Formatting.Indented);
        }

        // Method that checks if someone has entered anything
        public bool TextCheck(string[] textArr)
        {
            for (int i = 0; i < textArr.Length; i++)
            {
                if (textArr[i] == "")
                {
                    return true;
                }
            }
            return false;
        }

        // Method to check if username is already taken, returns bool (true if taken, false if not taken)
        public bool UsernameCheck(string username)
        {
            foreach (var accountData in accountDataList)
            {
                if(username == accountData.Name)
                {
                    return true;
                }
            }
            return false;
        }

        // Method that prints items with start and stop
        public void PrintItem(int start, int stop)
        {
            for (int i = 0; i < accountDataList.Count; i++)
            {
                try
                {
                    Console.WriteLine($"[{i + 1}] {accountDataList[i].Name}");
                }
                catch
                {
                    break;
                }
            }
        }



        // Method to login, returns int (the index of the list which corresponds to user selected) if user is found, else returns -1
        public int Login(string name, string password)
        {
            for (int i = 0; i < accountDataList.Count; i++)
            {
                // compares hashes
                using (SHA256 sha256Hash = SHA256.Create())
                    if (name == accountDataList[i].Name && Hashing.VerifyHash(sha256Hash, password, accountDataList[i].Password))
                {
                    return i;
                }
            }
            return -1;
        }

        // Method that returns user account index, based on input => calls login method.
        public int TextLogin()
        {
            // retry bool for if the user wants to try again
            bool retry = true;
            bool failed = false;

            while (retry == true)
            {
                // Clears the console for typing;
                Console.Clear();

                // Header
                Console.WriteLine("*---------------*");
                Console.WriteLine("*Login Page*");

                // Checks if there was a previous fail
                if (failed == true)
                {
                    Console.WriteLine("Gebruikersnaam of wachtwoord was incorrect, probeer het opnieuw\n");
                }

                // Ask for user input
                Console.WriteLine("Voer een gebruikersnaam in: ");
                string User_Name = Console.ReadLine();
                Console.WriteLine("\nVoer een wachtwoord in: ");
                string Password = Console.ReadLine();

                // Check if there was input
                string[] callValue = new string[]{ User_Name, Password };
                bool inputCheck = TextCheck(callValue);

                if (inputCheck == true)
                {
                    Console.WriteLine("\nBeide velden moeten een input hebben.\nOm opniew te proberen, toets 'r'\nOm terug te gaan, toets 'x'");
                }
                else
                {
                    Console.WriteLine($"\nGeselecteerde gebruikersnaam: {User_Name} | Geselecteerde wachtwoord: {Password} \nOm in te loggen toets ENTER\nOm opniew te proberen, toets 'r'\nOm terug te gaan, toets 'x'");
                }

                // Checked if user wants to retry or confirm username //
                string confirm = Console.ReadLine();
                if (confirm == "R" || confirm == "r" || confirm == "'R'" || confirm == "'r'" && inputCheck == false)
                {
                    retry = true;
                }

                else if (confirm == "X" || confirm == "x" || confirm == "'X'" || confirm == "'x'")
                {
                    retry = false;
                }
                else
                {
                    retry = false;
                    int login_Result = this.Login(User_Name, Password);
                    if (login_Result == -1)
                    {
                        failed = true;
                        retry = true;
                    }
                    else
                    {
                        return login_Result;
                    }
                }
            }
            return -1;
        }



        // Method that can be called to create a user.
        public void CreateUser(string name, string password)
        {
            // Hash the password
            using (SHA256 sha256Hash = SHA256.Create())
            {
                password = Hashing.GetHash(sha256Hash, password);
            }

            AccountData newAccountData = new AccountData();
            newAccountData.Name        = name;
            newAccountData.Password    = password;
            
            // placeHolders
            newAccountData.Age              = -1;
            newAccountData.Gender           = null;
            newAccountData.Email            = null;
            newAccountData.bankingDetails   = null;
            newAccountData.Allergies        = new string[] { };
            newAccountData.Permission       = false;

            // add to the list with the added data
            accountDataList.Add(newAccountData);

            // write to the JSON file (updates the file)
            System.IO.File.WriteAllText(this.path, ToJSON());
        }
        
        // Method that creates user, based on input => calls CreateUser method.
        public void TextCreateUser()
        {
            // retry bool for if the user wants to try again
            bool retry = true;
            
            while (retry == true){
                // Clears the console for typing;
                Console.Clear();

                // Ask for user input
                Console.WriteLine("*---------------*");
                Console.WriteLine("*User Creator*");
                Console.WriteLine("Voer een gebruikersnaam in: ");
                string User_Name = Console.ReadLine();

                bool recheck = true;
                while (UsernameCheck(User_Name) == true && recheck == true)
                {
                    Console.WriteLine("\nDeze gebruikersnaam bestaat al, probeer opnieuw: ");
                    User_Name = Console.ReadLine();
                    recheck = UsernameCheck(User_Name);
                }

                Console.WriteLine("\nVoer een wachtwoord in: ");
                string Password = Console.ReadLine();

                // Check if there was input
                string[] callValue = new string[] { User_Name, Password };
                bool inputCheck = TextCheck(callValue);

                if (inputCheck == true)
                {
                    Console.WriteLine("\nBeide velden moeten een input hebben.\nOm opniew te proberen, toets 'r'\nOm terug te gaan, toets 'x'");
                }
                else
                {
                    Console.WriteLine($"\nGeselecteerde gebruikersnaam: {User_Name} | Geselecteerde wachtwoord: {Password} \nOm in te loggen toets ENTER\nOm opniew te proberen, toets 'r'\nOm terug te gaan, toets 'x'");
                }
             
                // Checked if user wants to retry or confirm username //
                string confirm = Console.ReadLine();
                if (confirm == "R" || confirm == "r" || confirm == "'R'" || confirm == "'r'")
                {
                    retry = true;
                }
                else if (confirm == "X" || confirm == "x" || confirm == "'X'" || confirm == "'x'")
                {
                    retry = false;
                }
                else
                {
                    if (inputCheck == true)
                    {
                        retry = true;
                    }
                    else
                    {
                        retry = false;
                        this.CreateUser(User_Name, Password);
                    }
                    
                }
            }
        }


        // Method that deletes entry at certain index
        public void DeleteUser(int index)
        {
            accountDataList.RemoveAt(index);

            System.IO.File.WriteAllText(this.path, ToJSON());
        }

        // Prints all the users based on user input given in method
        public void printUsers()
        {
            string state = " ";
            int start = 0;
            int stop = 0;
            bool loop = true;

            while (loop == true)
            {
                bool executeRun = true;

                // runs code when page is at 0, no increase in value
                if (start == 0 && (state != ">" && state != "<"))
                {
                    if (start + 40 > accountDataList.Count)
                    {
                        if (start == accountDataList.Count)
                        {
                            executeRun = false;
                        }
                        stop = accountDataList.Count;
                    }
                    else
                    {
                        stop = start + 20;
                    }
                }

                // runs code when page is NOT at 0, no increase in value
                else if (start != 0 && (state != ">" && state != "<"))
                {
                    if (start + 40 > accountDataList.Count)
                    {
                        if (start == accountDataList.Count)
                        {
                            executeRun = false;
                        }
                        stop = accountDataList.Count;
                    }
                    else
                    {
                        stop = start + 20;
                    }
                }
                // runs code with 20 increment, stores value
                else if (state == ">")
                {
                    if ((start + 20) >= accountDataList.Count)
                    {
                        executeRun = false;
                    }
                    else
                    {
                        start += 20;
                        stop = start + 20;
                    }
                }

                // Runs code with 20 decrement, stores value
                else if (state == "<")
                {
                    if ((start - 20) < accountDataList.Count)
                    {
                        executeRun = false;
                    }
                    else
                    {
                        start -= 20;
                        stop = start + 20;
                    }
                }

                // Runs the code
                if (executeRun == true)
                {
                    // Header
                    Console.Clear();
                    Console.WriteLine("------------------------------");
                    Console.WriteLine("Vul < of > in om te navigeren tussen de bladzijden. \nVoer het corresponderende nummer in om de naar de user te gaan\nOm te stoppen toets X.\n");

                    // Print the users
                    PrintItem(start, stop);

                    // current page indicator
                    int pageCounterCurrent = (start / 20) + 1;
                    int pageCounterAll = (accountDataList.Count / 20) + 1;
                    Console.WriteLine($"\nPage {pageCounterCurrent}/{pageCounterAll}");
                }

                // Process user input
                string userInput = Console.ReadLine();
                try
                {
                    int convert = Convert.ToInt32(userInput);
                }
                catch
                {
                    if (userInput == ">" || userInput == "<")
                    {
                        state = userInput;
                    }
                    else if (userInput == "x" || userInput == "X")
                    {
                        loop = false;
                        break;
                    }
                    else
                    {
                        state = " ";
                    }
                }
            }   
        }


        // Method that updates selective user, requires to be logged in before or index!
        // Also requires string (name of the variable being changed) and the value in string
        public bool UpdateUser(string item, string value, int index)
        {
            // unloading the struct item at given index
            string name             = accountDataList[index].Name;
            string password         = accountDataList[index].Password;
            int age                 = accountDataList[index].Age;
            string gender           = accountDataList[index].Gender;
            string email            = accountDataList[index].Email;
            string bankingdetails   = accountDataList[index].bankingDetails;
            string[] allergies      = accountDataList[index].Allergies;
            bool permission         = accountDataList[index].Permission;

            // Checks what needs to be changed, and assigns the value
            bool returnValue = true;

            if (item == "name")
            {
                try
                {
                    name = value;
                }
                catch
                {
                    returnValue = false;
                }
            }
            else if (item == "password")
            {
                try
                {
                    // Hash the password
                    using (SHA256 sha256Hash = SHA256.Create())
                    {
                        password = Hashing.GetHash(sha256Hash, value);
                    }
                }
                catch
                {
                    returnValue = false;
                }
            }
            else if(item == "age")
            {
                try
                {
                    int temp = Convert.ToInt32(value);
                    age = temp;
                }
                catch
                {
                    returnValue = false;
                }
            }
            else if(item == "gender")
            {
                try
                {
                    gender = value;
                }
                catch
                {
                    returnValue = false;
                }
            }
            else if(item == "email")
            {
                try
                {
                    email = value;
                }
                catch
                {
                    returnValue = false;
                }
            }
            else if(item == "bankingdetails")
            {
                try
                {
                    bankingdetails = value;
                }
                catch
                {
                    returnValue = false;
                }
            }
            else if(item == "allergiesAdd")
            {
                try
                {
                    bool check = false;
                    for (int i = 0; i < allergies.Length; i++)
                    {
                        if (value == allergies[i])
                        {
                            check = true;
                            break;
                        }
                    }
                    if (check == false)
                    {
                        string[] temp = new string[allergies.Length + 1];
                        for (int i = 0; i < allergies.Length; i++)
                        {
                            temp[i] = allergies[i];
                        }
                        temp[allergies.Length] = value;
                        allergies = temp;
                    }
                    else
                    {
                        returnValue = false;
                    }
                }
                catch
                {
                    returnValue = false;
                }
            }
            else if (item == "allergiesRemove")
            {
                try
                {
                    string[] temp = new string[allergies.Length - 1];
                    int count = 0;
                    for (int i = 0; i < temp.Length; i++)
                    {
                        if (value == allergies[count])
                        {
                            count += 1;
                        }
                        temp[i] = allergies[count];
                        count += 1;
                    }
                    allergies = temp;
                }
                catch
                {
                    returnValue = false;
                }
            }
            else if (item == "permission")
            {
                try
                {
                    if (value == "true" || value == "True")
                    {
                        permission = true;
                    }
                    else
                    {
                        returnValue = false;
                    }
                }
                catch
                {
                    returnValue = false;
                }
            }

            // Creating the struct item
            AccountData newAccountData = new AccountData();

            newAccountData.Name             = name;
            newAccountData.Password         = password;
            newAccountData.Age              = age;
            newAccountData.Gender           = gender;
            newAccountData.Email            = email;
            newAccountData.bankingDetails   = bankingdetails;
            newAccountData.Allergies        = allergies;
            newAccountData.Permission       = permission;

            // deletes entry (as struct is immutable)
            DeleteUser(index);

            // add to the list with the added data [indexed]!
            accountDataList.Insert(index, newAccountData);

            // write to the JSON file (updates the file)
            System.IO.File.WriteAllText(this.path, ToJSON());

            return returnValue;
        }
    }
}
