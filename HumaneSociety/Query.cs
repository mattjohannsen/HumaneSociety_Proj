﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumaneSociety
{
    public static class Query
    {        
        static HumaneSocietyDataContext db;

        static Query()
        {
            db = new HumaneSocietyDataContext();
        }

        internal static List<USState> GetStates()
        {
            List<USState> allStates = db.USStates.ToList();       

            return allStates;
        }
            
        internal static Client GetClient(string userName, string password)
        {
            Client client = db.Clients.Where(c => c.UserName == userName && c.Password == password).Single();

            return client;
        }

        internal static List<Client> GetClients()
        {
            List<Client> allClients = db.Clients.ToList();

            return allClients;
        }

        internal static void AddNewClient(string firstName, string lastName, string username, string password, string email, string streetAddress, int zipCode, int stateId)
        {
            Client newClient = new Client();

            newClient.FirstName = firstName;
            newClient.LastName = lastName;
            newClient.UserName = username;
            newClient.Password = password;
            newClient.Email = email;

            Address addressFromDb = db.Addresses.Where(a => a.AddressLine1 == streetAddress && a.Zipcode == zipCode && a.USStateId == stateId).FirstOrDefault();

            // if the address isn't found in the Db, create and insert it
            if (addressFromDb == null)
            {
                Address newAddress = new Address();
                newAddress.AddressLine1 = streetAddress;
                newAddress.City = null;
                newAddress.USStateId = stateId;
                newAddress.Zipcode = zipCode;                

                db.Addresses.InsertOnSubmit(newAddress);
                db.SubmitChanges();

                addressFromDb = newAddress;
            }

            // attach AddressId to clientFromDb.AddressId
            newClient.AddressId = addressFromDb.AddressId;

            db.Clients.InsertOnSubmit(newClient);

            db.SubmitChanges();
        }

        internal static void UpdateClient(Client clientWithUpdates)
        {
            // find corresponding Client from Db
            Client clientFromDb = null;

            try
            {
                clientFromDb = db.Clients.Where(c => c.ClientId == clientWithUpdates.ClientId).Single();
            }
            catch(InvalidOperationException e)
            {
                Console.WriteLine("No clients have a ClientId that matches the Client passed in.");
                Console.WriteLine("No update have been made.");
                return;
            }
            
            // update clientFromDb information with the values on clientWithUpdates (aside from address)
            clientFromDb.FirstName = clientWithUpdates.FirstName;
            clientFromDb.LastName = clientWithUpdates.LastName;
            clientFromDb.UserName = clientWithUpdates.UserName;
            clientFromDb.Password = clientWithUpdates.Password;
            clientFromDb.Email = clientWithUpdates.Email;

            // get address object from clientWithUpdates
            Address clientAddress = clientWithUpdates.Address;

            // look for existing Address in Db (null will be returned if the address isn't already in the Db
            Address updatedAddress = db.Addresses.Where(a => a.AddressLine1 == clientAddress.AddressLine1 && a.USStateId == clientAddress.USStateId && a.Zipcode == clientAddress.Zipcode).FirstOrDefault();

            // if the address isn't found in the Db, create and insert it
            if(updatedAddress == null)
            {
                Address newAddress = new Address();
                newAddress.AddressLine1 = clientAddress.AddressLine1;
                newAddress.City = null;
                newAddress.USStateId = clientAddress.USStateId;
                newAddress.Zipcode = clientAddress.Zipcode;                

                db.Addresses.InsertOnSubmit(newAddress);
                db.SubmitChanges();

                updatedAddress = newAddress;
            }

            // attach AddressId to clientFromDb.AddressId
            clientFromDb.AddressId = updatedAddress.AddressId;
            
            // submit changes
            db.SubmitChanges();
        }
        
        internal static void AddUsernameAndPassword(Employee employee)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.EmployeeId == employee.EmployeeId).FirstOrDefault();

            employeeFromDb.UserName = employee.UserName;
            employeeFromDb.Password = employee.Password;

            db.SubmitChanges();
        }

        internal static Employee RetrieveEmployeeUser(string email, int employeeNumber)
        {
            Employee employee = db.Employees.Where(e => e.Email == email && e.EmployeeNumber == employeeNumber).FirstOrDefault();

            if (employee == null)
            {
                throw new NullReferenceException();
            }
            else
            {
                return employee;
            }
        }

        internal static Employee EmployeeLogin(string userName, string password)
        {
            Employee employee = db.Employees.Where(e => e.UserName == userName && e.Password == password).FirstOrDefault();
            return employee;
        }

        internal static bool CheckEmployeeUserNameExist(string userName)
        {
            Employee employee = db.Employees.Where(e => e.UserName == userName).FirstOrDefault();
            return employee != null;
        }

        internal static void RunEmployeeQueries(Employee employee, string crudOperation)
        {
            if (crudOperation == "create")
            {
                db.Employees.InsertOnSubmit(employee);
                try
                {
                    db.SubmitChanges();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    db.SubmitChanges();
                }
            }
            else if (crudOperation == "delete")
            {

                Employee employeeFromDb = db.Employees.Where(a => a.EmployeeId == employee.EmployeeId).FirstOrDefault();
                db.Employees.DeleteOnSubmit(employeeFromDb);

                try
                {
                    db.SubmitChanges();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    // Provide for exceptions.
                }
            }
            else if (crudOperation == "read")
            {
                Employee employeeFromDb = db.Employees.Where(a => a == employee).FirstOrDefault();
                Console.WriteLine(employeeFromDb.FirstName + "\n" +
                    employeeFromDb.LastName + "\n" +
                    employeeFromDb.UserName + "\n" +
                    employeeFromDb.Password + "\n" +
                    employeeFromDb.EmployeeNumber + "\n" +
                    employeeFromDb.Email);
            }
            else if (crudOperation == "update")
            {
                Employee employeeFromDb = db.Employees.Where(a => a.EmployeeId == employee.EmployeeId).FirstOrDefault();
                employeeFromDb.FirstName = employee.FirstName;
                employeeFromDb.LastName = employee.LastName;
                employeeFromDb.UserName = employee.UserName;
                employeeFromDb.Password = employee.Password;
                employeeFromDb.EmployeeNumber = employee.EmployeeNumber;
                employeeFromDb.Email = employee.Email;
                
                try
                {
                    db.SubmitChanges();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        internal static void AddAnimal(Animal animal)
        {
            db.Animals.InsertOnSubmit(animal);
            try
            {
                db.SubmitChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                db.SubmitChanges();
            }
        }

        internal static Animal GetAnimalByID(int id)
        {
            Animal animal = db.Animals.Where(a => a.AnimalId == id).FirstOrDefault();
            return animal;
        }

        internal static void UpdateAnimal(int animalId, Dictionary<int, string> updates)
        {
            Animal animal = db.Animals.Where(a => a.AnimalId == animalId).FirstOrDefault();
            animal.CategoryId = Convert.ToInt32(updates[1]);
            animal.Name = updates[2];
            animal.Age = Convert.ToInt32(updates[3]);
            animal.Demeanor = updates[4];
            animal.KidFriendly = Convert.ToBoolean(updates[5]);
            animal.PetFriendly = Convert.ToBoolean(updates[6]);
            animal.Weight = Convert.ToInt32(updates[7]);
            try
            {
                db.SubmitChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                db.SubmitChanges();
            }
        }

        internal static void RemoveAnimal(Animal animal)
        {
            animal = db.Animals.Where(a => a == animal).FirstOrDefault();
            db.Animals.DeleteOnSubmit(animal);
            
            try
            {
                db.SubmitChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                db.SubmitChanges();
            }
        }
        
        internal static IQueryable<Animal> SearchForAnimalsByMultipleTraits(Dictionary<int, string> updates)
        {
            IQueryable<Animal> theAnimals = db.Animals.Where(a => a.CategoryId == Convert.ToInt32(updates[1]) && a.Name == updates[2]
            && a.Age == Convert.ToInt32(updates[3]) && a.Demeanor == updates[4] && a.KidFriendly == Convert.ToBoolean(updates[5]) &&
            a.PetFriendly == Convert.ToBoolean(updates[6]) && a.Weight == Convert.ToInt32(updates[7]));

            return theAnimals;
        }         

        internal static int GetCategoryId(string categoryName)
        {
            int CatId = Convert.ToInt32(db.Categories.Where(a => a.Name == categoryName).Select(a => a.CategoryId));
            return CatId;
        }
        
        internal static Room GetRoom(int animalId)
        {
            Room room = db.Rooms.Where(a => a.AnimalId == animalId).FirstOrDefault();
            return room;
        }
        
        internal static int GetDietPlanId(string dietPlanName)
        {
            int dietPlanId = Convert.ToInt32(db.DietPlans.Where(a => a.Name == dietPlanName).Select(a => a.DietPlanId));
            return dietPlanId;
        }

        internal static void Adopt(Animal animal, Client client)
        {
            client = db.Clients.Where(a => a == client).FirstOrDefault();
            animal = db.Animals.Where(a => a == animal).FirstOrDefault();
            Adoption adoption = db.Adoptions.Where(a => a.ClientId == client.ClientId && a.AnimalId == animal.AnimalId).FirstOrDefault();
            animal.AdoptionStatus = "Adopted by: " + client.FirstName + " " + client.LastName;
            adoption.ApprovalStatus = "approved";
            adoption.PaymentCollected = true;
            
            try
            {
                db.SubmitChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                db.SubmitChanges();
            }
        }

        internal static IQueryable<Adoption> GetPendingAdoptions()
        {
            IQueryable<Adoption> pendingAdoptions = db.Adoptions.Where(a => a.ApprovalStatus == "pending");
            return pendingAdoptions;
        }

        internal static void UpdateAdoption(bool isAdopted, Adoption adoption)
        {
            if (isAdopted == true)
            {
                adoption.ApprovalStatus = "approved";
                adoption.PaymentCollected = true;
            }
            else
            {
                adoption.ApprovalStatus = "pending";
                adoption.PaymentCollected = false;
            }
            
            try
            {
                db.SubmitChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                db.SubmitChanges();
            }
        }

        internal static void RemoveAdoption(int animalId, int clientId)
        {
            Adoption adoption = db.Adoptions.Where(a => a.AnimalId == animalId && a.ClientId == clientId).FirstOrDefault();
            db.Adoptions.DeleteOnSubmit(adoption);
            
            try
            {
                db.SubmitChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                db.SubmitChanges();
            }
        }

        internal static IQueryable<AnimalShot> GetShots(Animal animal)
        {
            IQueryable<AnimalShot> shots = db.AnimalShots.Where(a => a.Animal == animal);
            return shots;
        }

        internal static void UpdateShot(string shotName, Animal animal)
        {
            Shot shot = db.Shots.Where(a => a.Name == shotName).FirstOrDefault();
            AnimalShot animalShot = db.AnimalShots.Where(a => a.AnimalId == animal.AnimalId && a.ShotId == shot.ShotId).FirstOrDefault();
            animalShot.DateReceived = DateTime.Now;
            
            try
            {
                db.SubmitChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                db.SubmitChanges();
            }
        }
    }
}