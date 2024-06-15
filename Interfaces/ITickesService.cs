using Tickest.Data;

public interface ITickesService
{
    Task SomeMethodAsync();
    // Outros métodos necessários...
}

public class TickesService : ITickesService
{
    private readonly TickestContext _context;

    public TickesService(TickestContext context)
    {
        _context = context;
    }

    public async Task SomeMethodAsync()
    {
        // Implementação do método...
    }
}
