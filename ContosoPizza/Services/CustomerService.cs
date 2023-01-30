using ContosoPizza.Models;

namespace ContosoPizza.Services;

public static class CustomerService {
    //customer list
    static List<Customer> CustomerList { get; }
    
    static CustomerService() {
        //create default list
        CustomerList = new List<Customer> {
            new Customer { Name = "Seth", PhoneNumber = 2081234567, Age = 23, FavoritePizza = "Classic Italian"},
            new Customer { Name = "Ethan", PhoneNumber = 2083219876, Age = 21, FavoritePizza = "Veggie"}
        };
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
        //find which customer is being updated
        var index = CustomerList.FindIndex(c => c.PhoneNumber == customer.PhoneNumber);
        if (index is -1) {
            return;
        } else {
        //update the customer
        CustomerList[index] = customer;
        }
    }

    public static void UpdateByNumber(Customer customer) {
        //In the put method itself we already check if they entered an existing customer
        var index = CustomerList.FindIndex(c => c.Name == customer.Name);
        //if somehow we didn't find them
        if (index is -1) {
            return; //do nothing
        } else {
            CustomerList[index] = customer;
        }
    }
    //DELETE
    public static void Delete(Customer customer) {
        //find which customer is being deleted
        var index = CustomerList.FindIndex(c => c.PhoneNumber == customer.PhoneNumber);
        //delete customer
        CustomerList.RemoveAt(index);
        //TODO: implement a get by phone number method then utilize that get method to find the customer that way and just use a standard .Remove instead of RemoveAt
    }
}