using Mi5hmasH.WpfHelper.Models;

namespace Mi5hmasH.WpfHelper.Interfaces;

public interface IPageRegistry
{
    /// <summary>
    /// A collection of PageModel instances representing the pages in the application.
    /// </summary>
    IEnumerable<PageModel> Pages { get; }
}