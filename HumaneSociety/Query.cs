using System;
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
            Employee employeeFromDb = db.Employees.Where(e => e.Email == email && e.EmployeeNumber == employeeNumber).FirstOrDefault();

            if (employeeFromDb == null)
            {
                throw new NullReferenceException();
            }
            else
            {
                return employeeFromDb;
            }
        }

        internal static Employee EmployeeLogin(string userName, string password)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.UserName == userName && e.Password == password).FirstOrDefault();

            return employeeFromDb;
        }

        internal static bool CheckEmployeeUserNameExist(string userName)
        {
            Employee employeeWithUserName = db.Employees.Where(e => e.UserName == userName).FirstOrDefault();

            return employeeWithUserName != null;
        }


        //// TODO Items: ////

        // TODO: Allow any of the CRUD operations to occur here
        //This is a method we changed - For Andrew's reference.
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
                    // Make some adjustments.
                    // ...
                    // Try again.
                    db.SubmitChanges();
                }
            }
            else if (crudOperation == "delete")
            {
                // Query the database for the rows to be deleted.
                var employeesToDelete =
                    from employees in db.Employees
                    where employees.EmployeeId == employee.EmployeeId
                    select employees;

                foreach (var item in employeesToDelete)
                {
                    db.Employees.DeleteOnSubmit(item);
                }
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
                var query =
                    from employees in db.Employees
                    where employees.EmployeeNumber == employee.EmployeeNumber
                    select employees;

                foreach (Employee employees in query)
                {
                    employee.EmployeeId = employees.EmployeeId;
                    employee.FirstName = employees.FirstName;
                    employee.LastName = employees.LastName;
                    employee.UserName = employees.UserName;
                    employee.Password = employees.Password;
                    employee.EmployeeNumber = employees.EmployeeNumber;
                    employee.Email = employees.Email;
                    // Insert any additional changes to column values.
                }
            }
            else if (crudOperation == "update")
            {
                var query =
                    from employees in db.Employees
                    where employees.EmployeeId == employee.EmployeeId
                    select employees;

                foreach (Employee employees in query)
                {
                    employees.FirstName = employee.FirstName;
                    employees.LastName = employee.LastName;
                    employees.UserName = employee.UserName;
                    employees.Password = employee.Password;
                    employees.EmployeeNumber = employee.EmployeeNumber;
                    employees.Email = employee.Email;
                }
                try
                {
                    db.SubmitChanges();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        // TODO: Animal CRUD Operations
        internal static void AddAnimal(Animal animal)
        {
            //This is a method we changed - For Andrew's reference.
            db.Animals.InsertOnSubmit(animal);
            try
            {
                db.SubmitChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                // Make some adjustments.
                // ...
                // Try again.
                db.SubmitChanges();
            }
        }

        internal static Animal GetAnimalByID(int id)
        {
            //This is a method we changed - For Andrew's reference.
            var query =
                from animals in db.Animals
                where animals.AnimalId == id
                select animals;

            Animal animalToReturn = new Animal();

            foreach (Animal animals in query)
            {
                animalToReturn.AnimalId = animals.AnimalId;
                animalToReturn.Name = animals.Name;
                animalToReturn.Weight = animals.Weight;
                animalToReturn.Age = animals.Age;
                animalToReturn.Demeanor = animals.Demeanor;
                animalToReturn.KidFriendly = animals.KidFriendly;
                animalToReturn.PetFriendly = animals.PetFriendly;
                animalToReturn.Gender = animals.Gender;
                animalToReturn.AdoptionStatus = animals.AdoptionStatus;
                animalToReturn.CategoryId = animals.CategoryId;
                animalToReturn.DietPlanId = animals.DietPlanId;
                animalToReturn.EmployeeId = animals.EmployeeId;
                // Insert any additional changes to column values.
            }

            return animalToReturn;
        }

        internal static void UpdateAnimal(int animalId, Dictionary<int, string> updates)
        {
            //This is a method we changed - For Andrew's reference.
            if (updates[1] == "1")
            {
                var animalsToUpdate = db.Animals.Where(a => a.AnimalId == animalId);
                foreach (Animal item in animalsToUpdate)
                {
                    item.CategoryId = Int32.Parse(updates[1]);
                }
            }
            else if (updates[2] == "2")
            {
                var animalsToUpdate = db.Animals.Where(a => a.AnimalId == animalId);
                foreach (Animal item in animalsToUpdate)
                {
                    item.Name = updates[2];
                }
            }
            else if (updates[3] == "3")
            {
                var animalsToUpdate = db.Animals.Where(a => a.AnimalId == animalId);
                foreach (Animal item in animalsToUpdate)
                {
                    item.Age = Int32.Parse(updates[3]);
                }
            }
            else if (updates[4] == "4")
            {
                var animalsToUpdate = db.Animals.Where(a => a.AnimalId == animalId);
                foreach (Animal item in animalsToUpdate)
                {
                    item.Demeanor = updates[4];
                }
            }
            else if (updates[5] == "5")
            {
                var animalsToUpdate = db.Animals.Where(a => a.AnimalId == animalId);
                foreach (Animal item in animalsToUpdate)
                {
                    item.KidFriendly = Convert.ToBoolean(updates[5]);
                }
            }
            else if (updates[6] == "6")
            {
                var animalsToUpdate = db.Animals.Where(a => a.AnimalId == animalId);
                foreach (Animal item in animalsToUpdate)
                {
                    item.PetFriendly = Convert.ToBoolean(updates[6]);
                }
            }
            else if (updates[7] == "7")
            {
                var animalsToUpdate = db.Animals.Where(a => a.AnimalId == animalId);
                foreach (Animal item in animalsToUpdate)
                {
                    item.Weight = Int32.Parse(updates[7]);
                }
            }
            else
            {
                UserInterface.DisplayUserOptions("Input not recognized please try again or type exit");
            }
        }

        internal static void RemoveAnimal(Animal animal)
        {
            // Query the database for the rows to be deleted.
            var animalsToDelete =
                from animals in db.Animals
                where animals.AnimalId == animal.AnimalId
                select animals;

            foreach (var item in animalsToDelete)
            {
                db.Animals.DeleteOnSubmit(item);
            }
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
        
        // TODO: Animal Multi-Trait Search
        internal static IQueryable<Animal> SearchForAnimalsByMultipleTraits(Dictionary<int, string> updates) // parameter(s)?
        {
            throw new NotImplementedException();
        }
         
        // TODO: Misc Animal Things
        internal static int GetCategoryId(string categoryName)
        {
            throw new NotImplementedException();
        }
        
        internal static Room GetRoom(int animalId)
        {
            throw new NotImplementedException();
        }
        
        internal static int GetDietPlanId(string dietPlanName)
        {
            throw new NotImplementedException();
        }

        // TODO: Adoption CRUD Operations
        internal static void Adopt(Animal animal, Client client)
        {
            throw new NotImplementedException();
        }

        internal static IQueryable<Adoption> GetPendingAdoptions()
        {
            throw new NotImplementedException();
        }

        internal static void UpdateAdoption(bool isAdopted, Adoption adoption)
        {
            throw new NotImplementedException();
        }

        internal static void RemoveAdoption(int animalId, int clientId)
        {
            throw new NotImplementedException();
        }

        // TODO: Shots Stuff
        internal static IQueryable<AnimalShot> GetShots(Animal animal)
        {
            throw new NotImplementedException();
        }

        internal static void UpdateShot(string shotName, Animal animal)
        {
            throw new NotImplementedException();
        }
    }
}