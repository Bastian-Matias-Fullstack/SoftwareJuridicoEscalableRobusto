namespace Aplicacion.DTOs
{
    public class ResultadoPaginado<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalRegistros { get; set; }
        public int Pagina { get; set; }
        public int Tamanio { get; set; }
    }
}
