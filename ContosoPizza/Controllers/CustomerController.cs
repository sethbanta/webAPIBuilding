using ContosoPizza.Models;
using ContosoPizza.Services;
using Microsoft.AspNetCore.Mvc;
using Google.Cloud.Storage.V1;
using Google.Apis.Auth.OAuth2;
namespace ContosoPizza.Controllers;

[ApiController]
[Route("[controller]")]
public class CustomerController : ControllerBase {
    public CustomerController() {
    }
    //GET
    //GET ALL
    [HttpGet]
    public ActionResult<List<Customer>> GetAll() => CustomerService.GetAllCustomers();
    [HttpGet("{name}")]
    public ActionResult<Customer?> Get(string name) => CustomerService.GetCustomer(name);

    [HttpGet("{number:int}")]
    public ActionResult<Customer?> Get(int number) => CustomerService.GetCustomer(number);

    [HttpGet("{input:guid}")]
    public IActionResult Login(Guid input) {
        Guid master = Guid.Parse("0f8fad5b-d9cb-469f-a165-70867728950e");
        Guid saveGuid = Guid.Parse("0f8fad5b-d9cb-469f-a165-70867728950b");
        Guid grabGuid = Guid.Parse("0f8fad5b-d9cb-469f-a165-70867728950c");
        //master guid entered, give authorization
        if(input == master) {
            //set the current token to master allowing auth for everything
            CustomerService.SetToken();
            return NoContent();
        } 
        //save guid entered
        else if (input == saveGuid) {
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
            return NoContent();
        }
        //cloud grab guid entered
        else if (input == grabGuid) {
            //set credentials for the api to use to contact google cloud
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", "C:\\Users\\Seth\\Documents\\apilearning\\credentials.json");
            //define our bucket
            string bucketName = "seth_bucket_test";
            //define what we are grabbing out of the bucket
            string objectName = "quickstart-folder/ContosoPizza/SavedList_2.json";
            //local file path for our data to be saved
            string localPath = "Grabbed.json";

            //create a storage client to provide operations to google cloud
            var storageClient = StorageClient.Create();
            //use our output file in order to download the object from the bucket
            using var outputFile = System.IO.File.OpenWrite(localPath);
            storageClient.DownloadObject(bucketName, objectName, outputFile);
            //TODO: download every object from within this path of the bucket
            return NoContent();

        }
        //they failed the login, bad request
        return BadRequest();
    }
    //PUT
    [HttpPut("{name}")]
    public IActionResult Update(string name, Customer customer) {
            if(name != customer.Name) {
                return BadRequest();
            }
            var existingCustomer = Get(name);
            if(existingCustomer is null) {
                return NotFound();
            }
            CustomerService.Update(customer);
            return NoContent();
    }

    [HttpPut("{number:int}")]
    public IActionResult Update(int number, Customer customer) {
        //need to check if they are referencing the right customer by pulling the name WITH the number then check against the name of WHO they are modifying
        var existingCustomer = CustomerService.GetCustomer(number);
        if(existingCustomer is null)
            return NotFound();

        if(existingCustomer.Name != customer.Name) {
            return BadRequest();
        }

        CustomerService.UpdateByNumber(customer);
        return NoContent();
    }
    //POST
    [HttpPost]
    public IActionResult Create(Customer customer) {
        CustomerService.Add(customer);
        return CreatedAtAction(nameof(Get), new { name = customer.Name }, customer);
    }
    //DELETE
    [HttpDelete("{name}")]
    public IActionResult Delete(string name) {
        var existingCustomer = CustomerService.GetCustomer(name);
        if(existingCustomer is null)
            return NotFound();
        CustomerService.Delete(existingCustomer);
        return NoContent();
    }
}