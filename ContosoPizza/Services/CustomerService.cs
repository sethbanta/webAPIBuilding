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

    public static Customer? GetCustomer(int number) => CustomerList.FirstOrDefault(c => c.PhoneNumber == number);
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

    public static void UpdateByNumberFromApp(string customerNumber, Customer customer) {
        //In the put method itself we already check if they entered an existing customer
        var index = CustomerList.FindIndex(c => c.PhoneNumber.ToString() == customerNumber);
        //if somehow we didn't find them
        if (index is -1) {
            return; //do nothing
        } else {
            CustomerList[index] = customer;
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
        tw.Write("[");
        //for every customer, add their info in JSON format with a comma at the end
        foreach(Customer c in tempList) {
            tw.Write("{\"name\":\"" + c.Name + "\",\"phoneNumber\":" + c.PhoneNumber + ",\"age\":" + c.Age + ",\"favoritePizza\":\"" + c.FavoritePizza + "\"},");
        }
        tw.Close();
        //grab the new files contents
        byte[] contents = System.IO.File.ReadAllBytes("SavedList.json");
        //create a new file stream in order to delete the comma left on the end
        FileStream fsOut = System.IO.File.OpenWrite("SavedList.json");
        fsOut.SetLength(fsOut.Length - 1);
        fsOut.Close();
        //open the file once again in order to append the ending bracket on the file
        using(StreamWriter sw = System.IO.File.AppendText("SavedList.json")) {
            sw.Write("]");
        }
        Console.WriteLine("Saved");
    }

}