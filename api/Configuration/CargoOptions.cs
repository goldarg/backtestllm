namespace api.Configuration;

public static class CargoOptions
{
    public static readonly HashSet<string> OpcionesValidas = new()
    {
        "ANALISTA",
        "COMERCIAL",
        "DIRECTOR",
        "GERENTE",
        "JEFE",
        "SECRETARIA/O",
        "SUPERVISOR",
        "TRANSPORTE",
        "COMPRAS",
        "RRHH",
        "RRII"
    };
}

