using AutoMapper;
using AutoMapper.QueryableExtensions;
using System.Linq.Expressions;

namespace Academix.Application.Common.Mappings
{
    public static class QueryableExtensions
    {
        #region IQueryable Extensions

        /// <summary>
        /// Projects IQueryable to destination type using AutoMapper
        /// </summary>
        public static IQueryable<TDestination> ProjectTo<TDestination>(
            this IQueryable queryable, 
            IMapper mapper)
        {
            return queryable.ProjectTo<TDestination>(mapper.ConfigurationProvider);
        }

        /// <summary>
        /// Projects IQueryable to destination type with parameters using AutoMapper
        /// </summary>
        public static IQueryable<TDestination> ProjectTo<TDestination>(
            this IQueryable queryable, 
            IMapper mapper, 
            params Expression<Func<TDestination, object>>[] membersToExpand)
        {
            return queryable.ProjectTo(mapper.ConfigurationProvider, membersToExpand);
        }

        /// <summary>
        /// Projects IQueryable to destination type with culture-specific parameters
        /// </summary>
        public static IQueryable<TDestination> ProjectToWithCulture<TDestination>(
            this IQueryable queryable,
            IMapper mapper,
            string culture = "en")
        {
            var parameters = new Dictionary<string, object>
            {
                ["Culture"] = culture
            };
            
            return queryable.ProjectTo<TDestination>(mapper.ConfigurationProvider, parameters);
        }

        /// <summary>
        /// Applies pagination to IQueryable
        /// </summary>
        public static IQueryable<T> ApplyPaging<T>(
            this IQueryable<T> queryable,
            int pageNumber,
            int pageSize)
        {
            return queryable
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);
        }

        /// <summary>
        /// Applies ordering by property name using reflection with culture support
        /// </summary>
        public static IQueryable<T> OrderByPropertyName<T>(
            this IQueryable<T> queryable,
            string propertyName,
            bool descending = false,
            string culture = "en")
        {
            if (string.IsNullOrEmpty(propertyName))
                return queryable;

            var property = typeof(T).GetProperty(propertyName);
            if (property == null)
            {
                // Try culture-specific property if not found
                var cultureSuffix = culture == "ar" ? "Ar" : "";
                property = typeof(T).GetProperty(propertyName + cultureSuffix);
            }

            if (property == null)
                return queryable;

            var parameter = Expression.Parameter(typeof(T), "x");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExpression = Expression.Lambda(propertyAccess, parameter);

            var resultExpression = Expression.Call(
                typeof(Queryable),
                descending ? "OrderByDescending" : "OrderBy",
                new Type[] { typeof(T), property.PropertyType },
                queryable.Expression,
                Expression.Quote(orderByExpression));

            return queryable.Provider.CreateQuery<T>(resultExpression);
        }

        /// <summary>
        /// Applies conditional WHERE clause
        /// </summary>
        public static IQueryable<T> WhereIf<T>(
            this IQueryable<T> queryable,
            bool condition,
            Expression<Func<T, bool>> predicate)
        {
            return condition ? queryable.Where(predicate) : queryable;
        }

        /// <summary>
        /// Gets paginated result with metadata and culture support
        /// </summary>
        public static async Task<PaginatedResult<T>> ToPaginatedListAsync<T>(
            this IQueryable<T> queryable,
            int pageNumber,
            int pageSize,
            string culture = "en",
            CancellationToken cancellationToken = default)
        {
            var totalCount = queryable.Count();
            var items = await Task.FromResult(queryable
                .ApplyPaging(pageNumber, pageSize)
                .ToList());

            return new PaginatedResult<T>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                HasPreviousPage = pageNumber > 1,
                HasNextPage = pageNumber < (int)Math.Ceiling(totalCount / (double)pageSize),
                Culture = culture
            };
        }

        #endregion

        #region IEnumerable Extensions

        /// <summary>
        /// Maps IEnumerable to destination type using AutoMapper
        /// </summary>
        public static IEnumerable<TDestination> MapTo<TDestination>(
            this IEnumerable<object> enumerable,
            IMapper mapper)
        {
            return mapper.Map<IEnumerable<TDestination>>(enumerable);
        }

        /// <summary>
        /// Maps IEnumerable to List using AutoMapper
        /// </summary>
        public static List<TDestination> MapToList<TDestination>(
            this IEnumerable<object> enumerable,
            IMapper mapper)
        {
            return mapper.Map<List<TDestination>>(enumerable);
        }

        /// <summary>
        /// Maps strongly typed IEnumerable to destination type
        /// </summary>
        public static IEnumerable<TDestination> MapTo<TSource, TDestination>(
            this IEnumerable<TSource> enumerable,
            IMapper mapper)
        {
            return mapper.Map<IEnumerable<TDestination>>(enumerable);
        }

        /// <summary>
        /// Maps strongly typed IEnumerable to List
        /// </summary>
        public static List<TDestination> MapToList<TSource, TDestination>(
            this IEnumerable<TSource> enumerable,
            IMapper mapper)
        {
            return mapper.Map<List<TDestination>>(enumerable);
        }

        /// <summary>
        /// Maps IEnumerable to destination type with culture support
        /// </summary>
        public static IEnumerable<TDestination> MapToWithCulture<TSource, TDestination>(
            this IEnumerable<TSource> enumerable,
            IMapper mapper,
            string culture = "en")
            where TSource : class
        {
            var mappingOptions = new Action<IMappingOperationOptions>(opts => 
            {
                opts.Items["Culture"] = culture;
            });

            return enumerable.Select(item => mapper.Map<TDestination>(item, mappingOptions));
        }

        /// <summary>
        /// Maps IEnumerable to List with culture support
        /// </summary>
        public static List<TDestination> MapToListWithCulture<TSource, TDestination>(
            this IEnumerable<TSource> enumerable,
            IMapper mapper,
            string culture = "en")
            where TSource : class
        {
            return enumerable.MapToWithCulture<TSource, TDestination>(mapper, culture).ToList();
        }

        /// <summary>
        /// Applies pagination to IEnumerable
        /// </summary>
        public static IEnumerable<T> ApplyPaging<T>(
            this IEnumerable<T> enumerable,
            int pageNumber,
            int pageSize)
        {
            return enumerable
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);
        }

        /// <summary>
        /// Converts IEnumerable to paginated result with culture support
        /// </summary>
        public static PaginatedResult<T> ToPaginatedList<T>(
            this IEnumerable<T> enumerable,
            int pageNumber,
            int pageSize,
            string culture = "en")
        {
            var totalCount = enumerable.Count();
            var items = enumerable
                .ApplyPaging(pageNumber, pageSize)
                .ToList();

            return new PaginatedResult<T>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                HasPreviousPage = pageNumber > 1,
                HasNextPage = pageNumber < (int)Math.Ceiling(totalCount / (double)pageSize),
                Culture = culture
            };
        }

        /// <summary>
        /// Groups by and maps to destination type with culture support
        /// </summary>
        public static IEnumerable<TDestination> GroupByAndMapTo<TSource, TKey, TDestination>(
            this IEnumerable<TSource> enumerable,
            Func<TSource, TKey> keySelector,
            IMapper mapper,
            string culture = "en")
            where TSource : class
        {
            return enumerable
                .GroupBy(keySelector)
                .Select(group => mapper.Map<TDestination>(group.AsEnumerable(), opts => 
                {
                    opts.Items["Culture"] = culture;
                }));
        }

        /// <summary>
        /// Applies conditional filter
        /// </summary>
        public static IEnumerable<T> WhereIf<T>(
            this IEnumerable<T> enumerable,
            bool condition,
            Func<T, bool> predicate)
        {
            return condition ? enumerable.Where(predicate) : enumerable;
        }

        /// <summary>
        /// Safe FirstOrDefault with mapping and culture support
        /// </summary>
        public static TDestination? FirstOrDefaultMapped<TSource, TDestination>(
            this IEnumerable<TSource> enumerable,
            IMapper mapper,
            Func<TSource, bool>? predicate = null,
            string culture = "en")
            where TSource : class
        {
            var source = predicate != null 
                ? enumerable.FirstOrDefault(predicate) 
                : enumerable.FirstOrDefault();

            return source != null 
                ? mapper.Map<TDestination>(source, opts => opts.Items["Culture"] = culture) 
                : default;
        }

        /// <summary>
        /// Batch processing for large collections
        /// </summary>
        public static IEnumerable<IEnumerable<T>> Batch<T>(
            this IEnumerable<T> enumerable,
            int batchSize)
        {
            var batch = new List<T>(batchSize);
            
            foreach (var item in enumerable)
            {
                batch.Add(item);
                
                if (batch.Count == batchSize)
                {
                    yield return batch;
                    batch = new List<T>(batchSize);
                }
            }
            
            if (batch.Count > 0)
                yield return batch;
        }

        #endregion

        #region Async Extensions with Localization

        /// <summary>
        /// Async mapping for IEnumerable with culture support
        /// </summary>
        public static async Task<List<TDestination>> MapToListAsync<TSource, TDestination>(
            this IEnumerable<TSource> enumerable,
            IMapper mapper,
            string culture = "en",
            CancellationToken cancellationToken = default)
            where TSource : class
        {
            return await Task.Run(() => 
                enumerable.MapToListWithCulture<TSource, TDestination>(mapper, culture), 
                cancellationToken);
        }

        /// <summary>
        /// Parallel mapping for large collections with culture support
        /// </summary>
        public static ParallelQuery<TDestination> MapToParallel<TSource, TDestination>(
            this IEnumerable<TSource> enumerable,
            IMapper mapper,
            string culture = "en")
            where TSource : class
        {
            return enumerable.AsParallel().Select(item => 
                mapper.Map<TDestination>(item, opts => opts.Items["Culture"] = culture));
        }

        /// <summary>
        /// Async paginated mapping with culture support
        /// </summary>
        public static async Task<PaginatedResult<TDestination>> MapToPaginatedListAsync<TSource, TDestination>(
            this IEnumerable<TSource> enumerable,
            IMapper mapper,
            int pageNumber,
            int pageSize,
            string culture = "en",
            CancellationToken cancellationToken = default)
            where TSource : class
        {
            var totalCount = enumerable.Count();
            var pagedItems = enumerable.ApplyPaging(pageNumber, pageSize);
            
            var mappedItems = await Task.Run(() => 
                pagedItems.MapToListWithCulture<TSource, TDestination>(mapper, culture), 
                cancellationToken);

            return new PaginatedResult<TDestination>
            {
                Items = mappedItems,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                HasPreviousPage = pageNumber > 1,
                HasNextPage = pageNumber < (int)Math.Ceiling(totalCount / (double)pageSize),
                Culture = culture
            };
        }

        #endregion

        #region Culture-Specific Search Extensions

        /// <summary>
        /// Searches in culture-specific text fields
        /// </summary>
        public static IQueryable<T> SearchInCultureFields<T>(
            this IQueryable<T> queryable,
            string searchTerm,
            string culture = "en",
            params string[] fieldNames)
        {
            if (string.IsNullOrWhiteSpace(searchTerm) || !fieldNames.Any())
                return queryable;

            var parameter = Expression.Parameter(typeof(T), "x");
            Expression? combinedExpression = null;

            foreach (var fieldName in fieldNames)
            {
                var cultureSuffix = culture == "ar" ? "Ar" : "";
                var cultureFieldName = fieldName + cultureSuffix;
                
                // Try culture-specific field first
                var property = typeof(T).GetProperty(cultureFieldName) ?? typeof(T).GetProperty(fieldName);
                
                if (property != null && property.PropertyType == typeof(string))
                {
                    var propertyAccess = Expression.MakeMemberAccess(parameter, property);
                    var toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes)!;
                    var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) })!;
                    
                    var toLowerCall = Expression.Call(propertyAccess, toLowerMethod);
                    var containsCall = Expression.Call(toLowerCall, containsMethod, 
                        Expression.Constant(searchTerm.ToLower()));
                    
                    combinedExpression = combinedExpression == null 
                        ? containsCall 
                        : Expression.OrElse(combinedExpression, containsCall);
                }
            }

            if (combinedExpression != null)
            {
                var lambda = Expression.Lambda<Func<T, bool>>(combinedExpression, parameter);
                return queryable.Where(lambda);
            }

            return queryable;
        }

        /// <summary>
        /// Orders by culture-specific field
        /// </summary>
        public static IQueryable<T> OrderByCultureField<T>(
            this IQueryable<T> queryable,
            string fieldName,
            string culture = "en",
            bool descending = false)
        {
            var cultureSuffix = culture == "ar" ? "Ar" : "";
            var cultureFieldName = fieldName + cultureSuffix;
            
            // Try culture-specific field first
            var property = typeof(T).GetProperty(cultureFieldName) ?? typeof(T).GetProperty(fieldName);
            
            if (property == null)
                return queryable;

            var parameter = Expression.Parameter(typeof(T), "x");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExpression = Expression.Lambda(propertyAccess, parameter);

            var resultExpression = Expression.Call(
                typeof(Queryable),
                descending ? "OrderByDescending" : "OrderBy",
                new Type[] { typeof(T), property.PropertyType },
                queryable.Expression,
                Expression.Quote(orderByExpression));

            return queryable.Provider.CreateQuery<T>(resultExpression);
        }

        #endregion
    }

    #region Supporting Classes

    /// <summary>
    /// Paginated result container with culture support
    /// </summary>
    public class PaginatedResult<T>
    {
        public List<T> Items { get; set; } = new();
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }
        public bool IsFirstPage => PageNumber == 1;
        public bool IsLastPage => PageNumber == TotalPages;
        public string Culture { get; set; } = "en";
        
        /// <summary>
        /// Gets localized pagination info
        /// </summary>
        public string GetPaginationInfo()
        {
            var startItem = (PageNumber - 1) * PageSize + 1;
            var endItem = Math.Min(PageNumber * PageSize, TotalCount);
            
            return Culture switch
            {
                "ar" => $"عرض {startItem} إلى {endItem} من {TotalCount} عنصر",
                _ => $"Showing {startItem} to {endItem} of {TotalCount} items"
            };
        }
    }

    #endregion
}
