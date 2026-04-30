namespace ApiUsuariosRC.Entities.Dtos;

public class PagedResultDto<T>
{
    public IReadOnlyCollection<T> Items { get; set; } = [];

    public int Pagina { get; set; }

    public int TamanoPagina { get; set; }

    public int TotalRegistros { get; set; }

    public int TotalPaginas { get; set; }

    public bool TienePaginaAnterior => Pagina > 1;

    public bool TienePaginaSiguiente => Pagina < TotalPaginas;

    public static PagedResultDto<T> Create(
        IReadOnlyCollection<T> items,
        int pagina,
        int tamanoPagina,
        int totalRegistros)
    {
        return new PagedResultDto<T>
        {
            Items = items,
            Pagina = pagina,
            TamanoPagina = tamanoPagina,
            TotalRegistros = totalRegistros,
            TotalPaginas = totalRegistros == 0
                ? 0
                : (int)Math.Ceiling(totalRegistros / (double)tamanoPagina)
        };
    }
}
