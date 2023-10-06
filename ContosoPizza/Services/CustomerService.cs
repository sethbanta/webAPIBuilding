using ContosoPizza.Models;

namespace ContosoPizza.Services;

public static class CustomerService {
    //customer list
    static List<Customer> CustomerList { get; }
    static string authToken;
    static string currentToken;

    static CustomerService() {
        //create default list
        CustomerList = new List<Customer> {
            new Customer { Name = "Seth", PhoneNumber = 2081234567, Age = 23, FavoritePizza = "Classic Italian"},
            new Customer { Name = "Ethan", PhoneNumber = 2083219876, Age = 21, FavoritePizza = "Veggie"}
        };

        var allChar = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";  
        var random = new Random();  
        //Create a master token for this session
        var resultToken = new string(  
        Enumerable.Repeat(allChar , 8)  
        .Select(token => token[random.Next(token.Length)]).ToArray());
        authToken = resultToken.ToString();
        //Generate a random current token
        currentToken = GenerateToken();
    }

    //CREATE
    public static void Add(Customer customer) => CustomerList.Add(customer);
    //READ
    //Get all customers
    public static List<Customer> GetAllCustomers() => CustomerList;
    public static Customer? GetCustomer(string name) => CustomerList.FirstOrDefault(c => c.Name == name);

    public static Customer? GetCustomer(long number) => CustomerList.FirstOrDefault(c => c.PhoneNumber == number);
    //UPDATE
    public static void Update(Customer customer) {
        string token = GetToken(currentToken);
        if (token == "CEO" || token == "Manager") {
            //find which customer is being updated
            var index = CustomerList.FindIndex(c => c.PhoneNumber == customer.PhoneNumber);
            if (index is -1) {
                return;
            } else {
            //update the customer
            CustomerList[index] = customer;
            }
        } else {
            //standard employee access, do nothing
            return;
        }
    }

    public static void UpdateFromApp(string customerName, Customer customer) {
        //find which customer is being updated
        //for some reason when getting passed a name from the program it passes the text in as System.Windows.Forms.TextBox, Text: <name>
        //used a substring of the value that is passed in order to cut out all the text that we dont need and only grab the actual customer name
        var realCustomerName = customerName.Substring(36);
        var index = CustomerList.FindIndex(c => c.Name == realCustomerName);
        if (index is -1) {
            Console.WriteLine("Customer not found");
            return;
        } else {
            //update the customer
            CustomerList[index] = customer;
        }
    }

    //This method is called to update from my Python MVC
    //The update method above gets a System.Windows.Forms.TextBox, Text: <name> input when called from the csharp app
    //Separated the methods that way to avoid breaking the other application
    public static int UpdateFromMVC(string customerName, Customer customer) {
        //find which customer is being updated
        var index = CustomerList.FindIndex(c => c.Name == customerName);
        if (index is -1) {
            Console.WriteLine("Customer not found");
            return 0;
        } else {
            //if the number being updated
            var customerNumber = GetCustomer(customerName).PhoneNumber.ToString();
            if(customerNumber != customer.PhoneNumber.ToString()) {
                //check to see if the number being updated to, is already existing within the API
                var check = CustomerList.FindIndex(c => c.PhoneNumber.ToString() == customer.PhoneNumber.ToString());
                if(check is -1) {
                    //the number didnt exist, update the customer
                    CustomerList[index] = customer;
                    return 1;
                } else {
                    return 0;
                }
            } else {
                CustomerList[index] = customer;
                return 1;
            }
        }
    }

    public static void UpdateByNumber(Customer customer) {
        string token = GetToken(currentToken);
        if (token == "CEO" || token == "Manager") {
            //In the put method itself we already check if they entered an existing customer
            var index = CustomerList.FindIndex(c => c.Name == customer.Name);
            //if somehow we didn't find them
            if (index is -1) {
                return; //do nothing
            } else {
                CustomerList[index] = customer;
            }
        } else {
            //employee access, do nothing
            return;
        }
    }

    public static int UpdateByNumberFromApp(string customerNumber, Customer customer) {
        //In the put method itself we already check if they entered an existing customer
        var index = CustomerList.FindIndex(c => c.PhoneNumber.ToString() == customerNumber);
        //if somehow we didn't find them
        if (index is -1) {
            return 0; //do nothing
        } else {
            //if the number being updated
            if(customerNumber != customer.PhoneNumber.ToString()) {
                //check to see if the number being updated to, is already existing within the API
                var check = CustomerList.FindIndex(c => c.PhoneNumber.ToString() == customer.PhoneNumber.ToString());
                if(check is -1) {
                    //the number didnt exist, update the customer
                    CustomerList[index] = customer;
                    return 1;
                } else {
                    return 0;
                }
            } else {
                CustomerList[index] = customer;
                return 1;
            }
        }
    }

    //DELETE
    public static void Delete(Customer customer) {
        string token = GetToken(currentToken);
        if (token == "CEO" || token == "Manager") {
            //find which customer is being deleted
            var index = CustomerList.FindIndex(c => c.PhoneNumber == customer.PhoneNumber);
            //delete customer
            CustomerList.RemoveAt(index);
            //TODO: implement a get by phone number method then utilize that get method to find the customer that way and just use a standard .Remove instead of RemoveAt
        } else {
            //no access
            return;
        }
    }

        public static void DeleteFromApp(Customer customer) {
        string token = GetToken(currentToken);
        //find which customer is being deleted
        var index = CustomerList.FindIndex(c => c.PhoneNumber == customer.PhoneNumber);
        //delete customer
        CustomerList.RemoveAt(index);
    }

    //Fake token stuff
    public static string GetToken(string token) {
        //correct token
        if (token == authToken) {
            return "Manager";
        } else {
            return "Employee";
        }
    }

    private static string GenerateToken() {
        var allChar = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";  
        var random = new Random();
        string tempToken; 
        //Create a token for this session
        var resultToken = new string(  
        Enumerable.Repeat(allChar , 8)  
        .Select(token => token[random.Next(token.Length)]).ToArray());
        tempToken = resultToken.ToString();
        return tempToken;
    }

    public static void SetToken() {
        currentToken = authToken;
    }

    //method to save all of the current data to a list in json format, to be loaded next time the program is launched
    public static void saveList() {
        //bring in the list of all the customers
        List<Customer> tempList = CustomerService.GetAllCustomers();
        //begin a text writer for creating our file
        TextWriter tw = new StreamWriter("SavedList.json");
        tw.Write("[\n");
        //for every customer, add their info in JSON format with a comma at the end
        foreach(Customer c in tempList) {
            tw.Write("{\"name\":\"" + c.Name + "\",\"phoneNumber\":" + c.PhoneNumber + ",\"age\":" + c.Age + ",\"favoritePizza\":\"" + c.FavoritePizza + "\"},\n");
        }
        tw.Close();
        //grab the new files contents
        byte[] contents = System.IO.File.ReadAllBytes("SavedList.json");
        //create a new file stream in order to delete the comma left on the end
        FileStream fsOut = System.IO.File.OpenWrite("SavedList.json");
        fsOut.SetLength(fsOut.Length - 2);
        fsOut.Close();
        //open the file once again in order to append the ending bracket on the file
        using(StreamWriter sw = System.IO.File.AppendText("SavedList.json")) {
            sw.Write("\n]");
        }
        Console.WriteLine("Saved");
    }

    public static void importList() {
        //clear the current list of customers
        CustomerList.Clear();
        //create reader for parsing the text from the json file
        StreamReader streamReader = new StreamReader("SavedList.json");
        //read the first line
        var line = streamReader.ReadLine();
        //create a blank customer tempList
        List<Customer> tempList = new List<Customer> {};
        //first line should always be [
        if (line == "[") {
            //bring in the second line, this should be actual data
            line = streamReader.ReadLine();
            //while not the end of the file
            while (line != "]") {
                //grab the index of the , used to separate json objects within the file
                var commaIndex = line?.LastIndexOf(",");
                //to be honest i dont know why this is needed. for some reason on the last object of the json file, it still tries to remove a comma that is nonexistant
                //to work around this i add replace the } character with a , at the end of every line
                //this means that every line is going to have one extra comma and will have extra values within the split array, but for the last line in the file it allows it to function properly and have the normal amount splits
                //without this line the last line has 3 values in the array when the normal is 5, adding one comma in this spot adds two values in the array
                line = line?.Replace("}",",");
                line = line?.Replace("{","");
                var splitLine = line?.Split(",");
                var splitName = splitLine?[0].Split(":");
                var lineName = splitName?[1];
                lineName = lineName?.Replace("\"","");
                var splitNumber = splitLine?[1].Split(":");
                var linePhone = Convert.ToInt32(splitNumber?[1]);
                var splitAge = splitLine?[2].Split(":");
                var lineAge = Convert.ToInt32(splitAge?[1]);
                var splitPizza = splitLine?[3].Split(":");
                var linePizza = splitPizza?[1];
                linePizza = linePizza?.Replace("\"","");
                Customer newCustomer = new Customer { Name = lineName, PhoneNumber = linePhone, Age = lineAge, FavoritePizza = linePizza};
                CustomerList.Add(newCustomer);
                line = streamReader.ReadLine();
            }
        }

    }

}