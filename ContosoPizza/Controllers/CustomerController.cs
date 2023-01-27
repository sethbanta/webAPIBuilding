using ContosoPizza.Models;
using ContosoPizza.Services;
using Microsoft.AspNetCore.Mvc;

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