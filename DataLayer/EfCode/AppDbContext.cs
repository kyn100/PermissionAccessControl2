﻿// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using DataAuthorize;
using DataLayer.AppClasses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Linq;
using System.Linq.Expressions;

namespace DataLayer.EfCode
{
    public class AppDbContext : DbContext
    {
        private readonly string _userId;
        private readonly string _accessKey;

        public DbSet<GeneralNote> GeneralNotes { get; set; }
        public DbSet<PersonalData> PersonalDatas { get; set; }
        public DbSet<ShopDefinition> ShopDefinitions { get; set; }
        public DbSet<ShopStock> ShopStocks { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options, IGetClaimsProvider claimsProvider)
            : base(options)
        {
            _userId = claimsProvider.UserId;
            _accessKey = claimsProvider.AccessKey;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            AddQueryFilterAndIndex(modelBuilder.Entity<PersonalData>(), x => x.DataKey == _userId);
            AddQueryFilterAndIndex(modelBuilder.Entity<ShopStock>(), x => x.DataKey == _accessKey);
        }

        /// <summary>
        /// This applies the correct query filter to the entity based on its interface type
        /// NOTE: OnModelCreating is only run once and the results cached so you can't dynamically change filters.
        ///       If you need dynamic query filters you need to build it into the lambda 
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="filter"></param>
        private void AddQueryFilterAndIndex<T>(EntityTypeBuilder<T> builder, Expression<Func<T, bool>> filter) where T : class, IDataKey
        {
            builder.HasQueryFilter(filter);
            //add an index to make the filter quicker
            builder.HasIndex(nameof(IDataKey.DataKey));
        }
    }
}