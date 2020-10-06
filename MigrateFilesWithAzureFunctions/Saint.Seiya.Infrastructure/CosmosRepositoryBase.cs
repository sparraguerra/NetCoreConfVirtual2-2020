using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks; 
using Microsoft.Azure.Cosmos;
using Saint.Seiya.Infrastructure.Abstract;
using Saint.Seiya.Shared.Models;
using Saint.Seiya.Shared.Models.Exceptions;

namespace Saint.Seiya.Infrastructure
{
    public class CosmosRepositoryBase<T> : ICosmosRepositoryBase<T> where T : Entity
    { 
        private readonly Container container;

        public CosmosRepositoryBase(CosmosInitializer cosmosInitializer, string containerName)
        { 
            this.container = cosmosInitializer.GetClient().GetContainer(cosmosInitializer.Database, containerName);
        }

        public async Task<T> AddOrUpdateAsync(T entity)
        {
            try
            {
                var partition = GeneratePartition(entity.PartitionKey); 
                var upsertedDoc = await container.UpsertItemAsync<T>(entity, partition);

                return upsertedDoc;
            }
            catch (Exception ex)
            {
                throw new SeiyaDataAccessException($"{entity} - Error while adding or updating a document.", ex);
            }
        }

        public int CountAll()
        {
            var result = container.GetItemLinqQueryable<T>(true).Count();

            return result;
        }

        public int CountAll(Expression<Func<T, bool>> predicate)
        {
            var result = container.GetItemLinqQueryable<T>(true).Count(predicate);

            return result;
        }

        public async Task<bool> DeleteAsync(string itemId, string partitionKey)
        {
            try
            {
                var partition = GeneratePartition(partitionKey);

                await container.DeleteItemAsync<T>(itemId, partition);

                return true;
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return false;
            }
            catch (CosmosException ex)
            {
                throw new SeiyaDataAccessException($"{itemId} - Error while getting a document by id", ex);
            }
        }

        public async Task<T> ExecuteStoredProcedureAsync(string storedProcedure, string partitioKey, dynamic[] procedureParams)
        {
            try
            {
                return await container.Scripts.ExecuteStoredProcedureAsync<T>(storedProcedure, GeneratePartition(partitioKey), procedureParams);
            }
            catch (CosmosException ex)
            {
                throw new SeiyaDataAccessException($"{storedProcedure} - Error while executing stored procedure", ex);
            }
        }

        public async Task<IEnumerable<T>> Get(string partitionKey, string query, int skip, int take)
        {
            var result = new List<T>();

            var pagingQuery = $"{query} OFFSET {skip} LIMIT {take}";
            var queryDefinition = new QueryDefinition(pagingQuery);
            var queryResultSetIterator = container.GetItemQueryIterator<T>(queryDefinition);

            while (queryResultSetIterator.HasMoreResults)
            {
                var currentResultSet = await queryResultSetIterator.ReadNextAsync();
                foreach (var item in currentResultSet)
                {
                    result.Add(item);
                }
            }

            return result;
        }

        public IEnumerable<T> GetAll()
        {
            var results = new List<T>();

            var query = container.GetItemLinqQueryable<T>(true);
            results.AddRange(query);

            return results;
        }

        public IEnumerable<T> GetAll(Expression<Func<T, bool>> predicate)
        {
            var results = new List<T>();
            var query = container.GetItemLinqQueryable<T>(true).Where(predicate);

            results.AddRange(query);

            return results;
        }

        public IEnumerable<T> GetAll(Expression<Func<T, bool>> predicate, Expression<Func<T, dynamic>> order, bool descending)
        {
            var results = new List<T>();

            if (descending)
            {
                var query = container.GetItemLinqQueryable<T>(true).Where(predicate).OrderByDescending(order);
                results.AddRange(query);
            }
            else
            {
                var query = container.GetItemLinqQueryable<T>(true).Where(predicate).OrderBy(order);
                results.AddRange(query);
            }

            return results;
        }

        public IEnumerable<T> GetAll(Expression<Func<T, bool>> predicate, Expression<Func<T, dynamic>> order, bool descending, int top)
        {
            var results = new List<T>();

            if (descending)
            {
                var query = container.GetItemLinqQueryable<T>(true).Where(predicate).OrderByDescending(order).Take(top);
                results.AddRange(query);
            }
            else
            {
                var query = container.GetItemLinqQueryable<T>(true).Where(predicate).OrderBy(order).Take(top);
                results.AddRange(query);
            }

            return results;
        }

        public IEnumerable<T> GetAll(Expression<Func<T, bool>> predicate, Expression<Func<T, dynamic>> order, bool descending, int skip, int take)
        {
            var results = new List<T>();

            if (descending)
            {
                var query = container.GetItemLinqQueryable<T>(true).Where(predicate).OrderByDescending(order).Skip(skip).Take(take);
                results.AddRange(query);
            }
            else
            {
                var query = container.GetItemLinqQueryable<T>(true).Where(predicate).OrderBy(order).Skip(skip).Take(take);
                results.AddRange(query);
            }

            return results;
        }

        public async Task<T> GetByIdAsync(string itemId, string partitionKey)
        {
            try
            {
                var partition = GeneratePartition(partitionKey);

                var item = await container.ReadItemAsync<T>(itemId, partition);

                return item;
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return default(T);
            }
            catch (CosmosException ex)
            {
                throw new SeiyaDataAccessException($"{itemId} - Error while getting a document by id", ex);
            }
        }

        private static PartitionKey GeneratePartition(string partitionKey)
        {
            var partition = new PartitionKey();
            if (partitionKey != null)
            {
                partition = new PartitionKey(partitionKey);
            }

            return partition;
        }
    }
}
