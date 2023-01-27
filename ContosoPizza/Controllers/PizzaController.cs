using ContosoPizza.Models;
using ContosoPizza.Services;
using Microsoft.AspNetCore.Mvc;

namespace ContosoPizza.Controllers;

[ApiController]
[Route("[controller]")]
public class PizzaController : ControllerBase {
    public PizzaController() {
    }

    //GET all action
    [HttpGet]
    public ActionResult<List<Pizza>> GetAll() =>
    PizzaService.GetAll();
    //GET by Id action

    [HttpGet("{id:int}")]
    public ActionResult<Pizza> Get(int id) {
        var pizza = PizzaService.Get(id);

        if(pizza == null) 
            return NotFound();

        return pizza;
    }

    [HttpGet("{name}")]
    public ActionResult<Pizza> Get(string name) {
        var pizza = PizzaService.Get(name);

        if(pizza == null)
            return NotFound();
        
        return pizza;
    }

    [HttpGet("{isGlutenFreeInput:bool}")]
    public ActionResult<List<Pizza?>> GetAllByGluten(bool isGlutenFreeInput) => PizzaService.GetAllByGluten(isGlutenFreeInput);

    //POST action -- read? 201 CreatedAtAction, 400 BadRequest body object invalid
    [HttpPost]
    public IActionResult Create(Pizza pizza) {
        PizzaService.Add(pizza);
        return CreatedAtAction(nameof(Get), new { id = pizza.Id }, pizza);
    }
    //PUT action, 204 NoContent, 400 BadRequest either id mismatch or invalid body object
    [HttpPut("{id}")]
    public IActionResult Update(int id, Pizza pizza) {
        if (id != pizza.Id)
            return BadRequest();

        var existingPizza = PizzaService.Get(id);
        if(existingPizza is null)
            return NotFound();

        PizzaService.Update(pizza);

        return NoContent();
    }
    //DELETE action
    [HttpDelete("{id}")]
    public IActionResult Delete(int id) {
        var pizza = PizzaService.Get(id);

        if(pizza is null)
            return NotFound();

        PizzaService.Delete(id);

        return NoContent();
    }
}