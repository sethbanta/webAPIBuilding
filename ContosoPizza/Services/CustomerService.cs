using ContosoPizza.Models;

namespace ContosoPizza.Services;

public class CustomerService {
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
    //UPDATE
    public static void Update(Customer customer) {
        //find which customer is being updated
        var index = CustomerList.FindIndex(c => c.PhoneNumber == customer.PhoneNumber);
        //update the customer
        CustomerList[index] = customer;
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