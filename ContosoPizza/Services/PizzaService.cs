using ContosoPizza.Models;

namespace ContosoPizza.Services;

public static class PizzaService {
    static List<Pizza> Pizzas { get; }
    static List<Pizza?> BooleanPizzas { get; set; }
    static int nextId = 3;
    static PizzaService() {
        Pizzas = new List<Pizza> {
            new Pizza { Id = 1, Name = "Classic Italian", IsGlutenFree = false },
            new Pizza { Id = 2, Name = "Veggie", IsGlutenFree = true}
        };
        BooleanPizzas = new List<Pizza?> {
            //initialize an empty list of pizzas
        };
    }

    public static List<Pizza> GetAll() => Pizzas;

    public static Pizza? Get(int id) => Pizzas.FirstOrDefault(p => p.Id == id);

    public static Pizza? Get(string name) => Pizzas.FirstOrDefault(p => p.Name == name);

    public static List<Pizza?> GetAllByGluten(bool input) {
        bool isTrue = input;
        //clean the list
        BooleanPizzas.Clear();
        //grab new list
        foreach(Pizza p in Pizzas) {
            if(p.IsGlutenFree.Equals(isTrue)) {
                BooleanPizzas.Add(p);
            }
        }
        return BooleanPizzas;
    }

    public static void Add (Pizza pizza) {
        pizza.Id = nextId++;
        Pizzas.Add(pizza);
    }

    public static void Delete(int id) {
        var pizza = Get(id);
        if(pizza is null)
            return;

        Pizzas.Remove(pizza);
    }

    public static void Update(Pizza pizza) {
        var index = Pizzas.FindIndex(p => p.Id == pizza.Id);
        if(index == 1)
            return;

        Pizzas[index] = pizza;
    }
}