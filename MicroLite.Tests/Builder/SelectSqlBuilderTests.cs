﻿namespace MicroLite.Tests.Builder
{
    using System;
    using MicroLite.Builder;
    using MicroLite.Dialect;
    using MicroLite.Mapping;
    using MicroLite.Tests.TestEntities;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="SelectSqlBuilder"/> class.
    /// </summary>
    public class SelectSqlBuilderTests : UnitTest
    {
        public SelectSqlBuilderTests()
        {
            ObjectInfo.MappingConvention = new ConventionMappingConvention(
                UnitTest.GetConventionMappingSettings(IdentifierStrategy.DbGenerated));
        }

        /// <summary>
        /// Issue #223 - SqlBuilder Between is not appending AND or OR
        /// </summary>
        [Fact]
        public void BetweenShouldAppendOperandIfItIsAnAdditionalPredicate()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "Column1");

            var sqlQuery = sqlBuilder
                .From("Table")
                .Where("Column2").In("Opt1", "Opt2")
                .AndWhere("Column3").Between(1, 10)
                .ToSqlQuery();

            Assert.Equal("SELECT Column1 FROM Table WHERE (Column2 IN (?,?)) AND (Column3 BETWEEN ? AND ?)", sqlQuery.CommandText);
            Assert.Equal(4, sqlQuery.Arguments.Count);
            Assert.Equal("Opt1", sqlQuery.Arguments[0]);
            Assert.Equal("Opt2", sqlQuery.Arguments[1]);
            Assert.Equal(1, sqlQuery.Arguments[2]);
            Assert.Equal(10, sqlQuery.Arguments[3]);
        }

        [Fact]
        public void GroupByAppendsColumnsCorrectlyForMultipleColumns()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "CustomerId");

            var sqlQuery = sqlBuilder.From("Customer")
                .GroupBy("CustomerId", "Created")
                .ToSqlQuery();

            Assert.Equal("SELECT CustomerId FROM Customer GROUP BY CustomerId,Created", sqlQuery.CommandText);
        }

        [Fact]
        public void GroupByAppendsColumnsCorrectlyForSingleColumn()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "CustomerId");

            var sqlQuery = sqlBuilder.From("Customer")
                .GroupBy("CustomerId")
                .ToSqlQuery();

            Assert.Equal("SELECT CustomerId FROM Customer GROUP BY CustomerId", sqlQuery.CommandText);
        }

        [Fact]
        public void GroupByThrowsArgumentNullException()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "CustomerId");

            Assert.Throws<ArgumentNullException>(() => sqlBuilder.From("Customer").GroupBy((string)null));
            Assert.Throws<ArgumentNullException>(() => sqlBuilder.From("Customer").GroupBy((string[])null));
        }

        [Fact]
        public void InThrowArgumentNullExceptionForNullArgs()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty);

            var exception = Assert.Throws<ArgumentNullException>(
                () => sqlBuilder.From("").Where("").In((object[])(object[])null));

            Assert.Equal("args", exception.ParamName);
        }

        [Fact]
        public void InThrowArgumentNullExceptionForNullSqlQuery()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty);

            var exception = Assert.Throws<ArgumentNullException>(
                () => sqlBuilder.From("").Where("").In((SqlQuery)null));

            Assert.Equal("subQuery", exception.ParamName);
        }

        [Fact]
        public void NotInThrowsArgumentNullExceptionForNullArgs()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "CustomerId");

            Assert.Throws<ArgumentNullException>(
                () => sqlBuilder.From("Customer").Where("Column").NotIn((object[])null));
        }

        [Fact]
        public void NotInThrowsArgumentNullExceptionForNullSqlQuery()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "CustomerId");

            Assert.Throws<ArgumentNullException>(
                () => sqlBuilder.From("Customer").Where("Column").NotIn((SqlQuery)null));
        }

        [Fact]
        public void OrderByAscendingAppendsColumnsCorrectlyForMultipleColumns()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "CustomerId");

            var sqlQuery = sqlBuilder.From("Customer")
                .OrderByAscending("FirstName", "LastName")
                .ToSqlQuery();

            Assert.Equal("SELECT CustomerId FROM Customer ORDER BY FirstName ASC,LastName ASC", sqlQuery.CommandText);
        }

        [Fact]
        public void OrderByAscendingAppendsColumnsCorrectlyForSingleColumn()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "CustomerId");

            var sqlQuery = sqlBuilder.From("Customer")
                .OrderByAscending("CustomerId")
                .ToSqlQuery();

            Assert.Equal("SELECT CustomerId FROM Customer ORDER BY CustomerId ASC", sqlQuery.CommandText);
        }

        [Fact]
        public void OrderByAscendingThrowsArgumentNullException()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "CustomerId");

            Assert.Throws<ArgumentNullException>(
                () => sqlBuilder.From("Customer").OrderByAscending((string)null));

            Assert.Throws<ArgumentNullException>(
                () => sqlBuilder.From("Customer").OrderByAscending((string[])null));
        }

        [Fact]
        public void OrderByDescendingAppendsColumnsCorrectlyForMultipleColumns()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "CustomerId");

            var sqlQuery = sqlBuilder.From("Customer")
                .OrderByDescending("FirstName", "LastName")
                .ToSqlQuery();

            Assert.Equal("SELECT CustomerId FROM Customer ORDER BY FirstName DESC,LastName DESC", sqlQuery.CommandText);
        }

        [Fact]
        public void OrderByDescendingAppendsColumnsCorrectlyForSingleColumn()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "CustomerId");

            var sqlQuery = sqlBuilder.From("Customer")
                .OrderByDescending("CustomerId")
                .ToSqlQuery();

            Assert.Equal("SELECT CustomerId FROM Customer ORDER BY CustomerId DESC", sqlQuery.CommandText);
        }

        [Fact]
        public void OrderByDescendingThrowsArgumentNullException()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "CustomerId");

            Assert.Throws<ArgumentNullException>(
                () => sqlBuilder.From("Customer").OrderByDescending((string)null));

            Assert.Throws<ArgumentNullException>(
                () => sqlBuilder.From("Customer").OrderByDescending((string[])null));
        }

        [Fact]
        public void SelectAverage()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, null);

            var sqlQuery = sqlBuilder
                .Average("CreditLimit")
                .From(typeof(Customer))
                .Where("CustomerStatusId").IsEqualTo(CustomerStatus.Active)
                .ToSqlQuery();

            Assert.Equal(1, sqlQuery.Arguments.Count);
            Assert.Equal(CustomerStatus.Active, sqlQuery.Arguments[0]);

            Assert.Equal("SELECT AVG(CreditLimit) AS CreditLimit FROM Sales.Customers WHERE (CustomerStatusId = ?)", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectAverageWithAlias()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, null);

            var sqlQuery = sqlBuilder
                .Average("CreditLimit", columnAlias: "AverageCreditLimit")
                .From(typeof(Customer))
                .Where("CustomerStatusId").IsEqualTo(CustomerStatus.Active)
                .ToSqlQuery();

            Assert.Equal(1, sqlQuery.Arguments.Count);
            Assert.Equal(CustomerStatus.Active, sqlQuery.Arguments[0]);

            Assert.Equal("SELECT AVG(CreditLimit) AS AverageCreditLimit FROM Sales.Customers WHERE (CustomerStatusId = ?)", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectAverageWithOtherColumn()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "Id");

            var sqlQuery = sqlBuilder
                .Average("CreditLimit")
                .From(typeof(Customer))
                .Where("CustomerStatusId").IsEqualTo(CustomerStatus.Active)
                .GroupBy("CustomerStatusId")
                .ToSqlQuery();

            Assert.Equal(1, sqlQuery.Arguments.Count);
            Assert.Equal(CustomerStatus.Active, sqlQuery.Arguments[0]);

            Assert.Equal("SELECT Id,AVG(CreditLimit) AS CreditLimit FROM Sales.Customers WHERE (CustomerStatusId = ?) GROUP BY CustomerStatusId", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectAverageWithSqlCharacters()
        {
            var sqlBuilder = new SelectSqlBuilder(MsSqlCharacters.Instance, null);

            var sqlQuery = sqlBuilder
                .Average("CreditLimit")
                .From(typeof(Customer))
                .Where("CustomerStatusId").IsEqualTo(CustomerStatus.Active)
                .ToSqlQuery();

            Assert.Equal(1, sqlQuery.Arguments.Count);
            Assert.Equal(CustomerStatus.Active, sqlQuery.Arguments[0]);

            Assert.Equal("SELECT AVG([CreditLimit]) AS CreditLimit FROM [Sales].[Customers] WHERE ([CustomerStatusId] = @p0)", sqlQuery.CommandText);
        }

        /// <summary>
        /// Issue #221 - SqlBuilder does not allow chaining multiple function calls.
        /// </summary>
        [Fact]
        public void SelectChainingMultipleFunctions()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "Column1");

            var sqlQuery = sqlBuilder
                .Count("Column2", "Col2")
                .Max("Column3", "Col3")
                .From("Table")
                .ToSqlQuery();

            Assert.Empty(sqlQuery.Arguments);
            Assert.Equal("SELECT Column1,COUNT(Column2) AS Col2,MAX(Column3) AS Col3 FROM Table", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectCount()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, null);

            var sqlQuery = sqlBuilder
                .Count("Id")
                .From(typeof(Customer))
                .ToSqlQuery();

            Assert.Empty(sqlQuery.Arguments);
            Assert.Equal("SELECT COUNT(Id) AS Id FROM Sales.Customers", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectCountWithAlias()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, null);

            var sqlQuery = sqlBuilder
                .Count("Id", columnAlias: "CustomerCount")
                .From(typeof(Customer))
                .ToSqlQuery();

            Assert.Empty(sqlQuery.Arguments);
            Assert.Equal("SELECT COUNT(Id) AS CustomerCount FROM Sales.Customers", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectCountWithOtherColumn()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "CustomerStatusId");

            var sqlQuery = sqlBuilder
                .Count("Id")
                .From(typeof(Customer))
                .GroupBy("CustomerStatusId")
                .ToSqlQuery();

            Assert.Empty(sqlQuery.Arguments);
            Assert.Equal("SELECT CustomerStatusId,COUNT(Id) AS Id FROM Sales.Customers GROUP BY CustomerStatusId", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectCountWithSqlCharacters()
        {
            var sqlBuilder = new SelectSqlBuilder(MsSqlCharacters.Instance, null);

            var sqlQuery = sqlBuilder
                .Count("Id")
                .From(typeof(Customer))
                .Where("CustomerStatusId").IsEqualTo(CustomerStatus.Active)
                .ToSqlQuery();

            Assert.Equal(1, sqlQuery.Arguments.Count);
            Assert.Equal(CustomerStatus.Active, sqlQuery.Arguments[0]);

            Assert.Equal("SELECT COUNT([Id]) AS Id FROM [Sales].[Customers] WHERE ([CustomerStatusId] = @p0)", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectFromOrderByAscending()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "Column1", "Column2");

            var sqlQuery = sqlBuilder
                .From("Table")
                .OrderByAscending("Column1", "Column2")
                .ToSqlQuery();

            Assert.Empty(sqlQuery.Arguments);
            Assert.Equal("SELECT Column1,Column2 FROM Table ORDER BY Column1 ASC,Column2 ASC", sqlQuery.CommandText);
        }

        /// <summary>
        /// Issue #93 - SqlBuilder produces invalid Sql if OrderBy is called multiple times.
        /// </summary>
        [Fact]
        public void SelectFromOrderByAscendingThenDescending()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "Column1", "Column2");

            var sqlQuery = sqlBuilder
                .From("Table")
                .OrderByAscending("Column1")
                .OrderByDescending("Column2")
                .ToSqlQuery();

            Assert.Empty(sqlQuery.Arguments);
            Assert.Equal("SELECT Column1,Column2 FROM Table ORDER BY Column1 ASC,Column2 DESC", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectFromOrderByAscendingWithSqlCharacters()
        {
            var sqlBuilder = new SelectSqlBuilder(MsSqlCharacters.Instance, "Column1", "Column2");

            var sqlQuery = sqlBuilder
                .From("Table")
                .OrderByAscending("Column1", "Column2")
                .ToSqlQuery();

            Assert.Empty(sqlQuery.Arguments);
            Assert.Equal("SELECT [Column1],[Column2] FROM [Table] ORDER BY [Column1] ASC,[Column2] ASC", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectFromOrderByDescending()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "Column1", "Column2");

            var sqlQuery = sqlBuilder
                .From("Table")
                .OrderByDescending("Column1", "Column2")
                .ToSqlQuery();

            Assert.Empty(sqlQuery.Arguments);
            Assert.Equal("SELECT Column1,Column2 FROM Table ORDER BY Column1 DESC,Column2 DESC", sqlQuery.CommandText);
        }

        /// <summary>
        /// Issue #93 - SqlBuilder produces invalid Sql if OrderBy is called multiple times.
        /// </summary>
        [Fact]
        public void SelectFromOrderByDescendingThenAscending()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "Column1", "Column2");

            var sqlQuery = sqlBuilder
                .From("Table")
                .OrderByDescending("Column1")
                .OrderByAscending("Column2")
                .ToSqlQuery();

            Assert.Empty(sqlQuery.Arguments);
            Assert.Equal("SELECT Column1,Column2 FROM Table ORDER BY Column1 DESC,Column2 ASC", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectFromOrderByDescendingWithSqlCharacters()
        {
            var sqlBuilder = new SelectSqlBuilder(MsSqlCharacters.Instance, "Column1", "Column2");

            var sqlQuery = sqlBuilder
                .From("Table")
                .OrderByDescending("Column1", "Column2")
                .ToSqlQuery();

            Assert.Empty(sqlQuery.Arguments);
            Assert.Equal("SELECT [Column1],[Column2] FROM [Table] ORDER BY [Column1] DESC,[Column2] DESC", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectFromSpecifyingColumnsAndTableName()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "Column1", "Column2");

            var sqlQuery = sqlBuilder
                .From("Table")
                .ToSqlQuery();

            Assert.Empty(sqlQuery.Arguments);
            Assert.Equal("SELECT Column1,Column2 FROM Table", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectFromSpecifyingColumnsAndTableNameWithSqlCharacters()
        {
            var sqlBuilder = new SelectSqlBuilder(MsSqlCharacters.Instance, "Column1", "Column2");

            var sqlQuery = sqlBuilder
                .From("Table")
                .ToSqlQuery();

            Assert.Empty(sqlQuery.Arguments);
            Assert.Equal("SELECT [Column1],[Column2] FROM [Table]", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectFromSpecifyingColumnsAndType()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "Name", "DateOfBirth");

            var sqlQuery = sqlBuilder
                .From(typeof(Customer))
                .ToSqlQuery();

            Assert.Empty(sqlQuery.Arguments);
            Assert.Equal("SELECT Name,DateOfBirth FROM Sales.Customers", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectFromSpecifyingColumnsAndTypeWithSqlCharacters()
        {
            var sqlBuilder = new SelectSqlBuilder(MsSqlCharacters.Instance, "Name", "DateOfBirth");

            var sqlQuery = sqlBuilder
                .From(typeof(Customer))
                .ToSqlQuery();

            Assert.Empty(sqlQuery.Arguments);
            Assert.Equal("SELECT [Name],[DateOfBirth] FROM [Sales].[Customers]", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectFromType()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty);

            var sqlQuery = sqlBuilder
                .From(typeof(Customer))
                .ToSqlQuery();

            Assert.Empty(sqlQuery.Arguments);
            Assert.Equal("SELECT Created,CreditLimit,DateOfBirth,Id,Name,CustomerStatusId,Updated,Website FROM Sales.Customers", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectFromTypeWithSqlCharacters()
        {
            var sqlBuilder = new SelectSqlBuilder(MsSqlCharacters.Instance);

            var sqlQuery = sqlBuilder
                .From(typeof(Customer))
                .ToSqlQuery();

            Assert.Empty(sqlQuery.Arguments);
            Assert.Equal("SELECT [Created],[CreditLimit],[DateOfBirth],[Id],[Name],[CustomerStatusId],[Updated],[Website] FROM [Sales].[Customers]", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectFromWhere()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "Column1", "Column2");

            var sqlQuery = sqlBuilder
                .From("Table")
                .Where("Column1 = @p0", "Foo")
                .ToSqlQuery();

            Assert.Equal(1, sqlQuery.Arguments.Count);
            Assert.Equal("Foo", sqlQuery.Arguments[0]);

            Assert.Equal("SELECT Column1,Column2 FROM Table WHERE (Column1 = @p0)", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectFromWhereAnd()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "Column1", "Column2");

            var sqlQuery = sqlBuilder
                .From("Table")
                .Where("Column1").IsEqualTo("Foo")
                .AndWhere("Column2").IsEqualTo("Bar")
                .ToSqlQuery();

            Assert.Equal(2, sqlQuery.Arguments.Count);
            Assert.Equal("Foo", sqlQuery.Arguments[0]);
            Assert.Equal("Bar", sqlQuery.Arguments[1]);

            Assert.Equal("SELECT Column1,Column2 FROM Table WHERE (Column1 = ?) AND (Column2 = ?)", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectFromWhereComplex()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "Column1", "Column2", "Column3");

            var sqlQuery = sqlBuilder
                .From("Table")
                .Where("Column1 = @p0 OR @p0 IS NULL", "Foo")
                .AndWhere("Column2 BETWEEN @p0 AND @p1", DateTime.Today.AddDays(-1), DateTime.Today)
                .OrWhere("Column3 IN (@p0, @p1, @p2, @p3)", 1, 2, 3, 4)
                .ToSqlQuery();

            Assert.Equal(7, sqlQuery.Arguments.Count);
            Assert.Equal("Foo", sqlQuery.Arguments[0]);
            Assert.Equal(DateTime.Today.AddDays(-1), sqlQuery.Arguments[1]);
            Assert.Equal(DateTime.Today, sqlQuery.Arguments[2]);
            Assert.Equal(1, sqlQuery.Arguments[3]);
            Assert.Equal(2, sqlQuery.Arguments[4]);
            Assert.Equal(3, sqlQuery.Arguments[5]);
            Assert.Equal(4, sqlQuery.Arguments[6]);

            Assert.Equal("SELECT Column1,Column2,Column3 FROM Table WHERE (Column1 = @p0 OR @p0 IS NULL) AND (Column2 BETWEEN @p1 AND @p2) OR (Column3 IN (@p3, @p4, @p5, @p6))", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectFromWhereOr()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "Column1", "Column2");

            var sqlQuery = sqlBuilder
                .From("Table")
                .Where("Column1").IsEqualTo("Foo")
                .OrWhere("Column2").IsEqualTo("Bar")
                .ToSqlQuery();

            Assert.Equal(2, sqlQuery.Arguments.Count);
            Assert.Equal("Foo", sqlQuery.Arguments[0]);
            Assert.Equal("Bar", sqlQuery.Arguments[1]);

            Assert.Equal("SELECT Column1,Column2 FROM Table WHERE (Column1 = ?) OR (Column2 = ?)", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectMax()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, null);

            var sqlQuery = sqlBuilder
                .Max("CreditLimit")
                .From(typeof(Customer))
                .Where("CustomerStatusId").IsEqualTo(CustomerStatus.Active)
                .ToSqlQuery();

            Assert.Equal(1, sqlQuery.Arguments.Count);
            Assert.Equal(CustomerStatus.Active, sqlQuery.Arguments[0]);

            Assert.Equal("SELECT MAX(CreditLimit) AS CreditLimit FROM Sales.Customers WHERE (CustomerStatusId = ?)", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectMaxWithAlias()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, null);

            var sqlQuery = sqlBuilder
                .Max("CreditLimit", columnAlias: "MaxCreditLimit")
                .From(typeof(Customer))
                .Where("CustomerStatusId").IsEqualTo(CustomerStatus.Active)
                .ToSqlQuery();

            Assert.Equal(1, sqlQuery.Arguments.Count);
            Assert.Equal(CustomerStatus.Active, sqlQuery.Arguments[0]);

            Assert.Equal("SELECT MAX(CreditLimit) AS MaxCreditLimit FROM Sales.Customers WHERE (CustomerStatusId = ?)", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectMaxWithOtherColumn()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "Id");

            var sqlQuery = sqlBuilder
                .Max("CreditLimit")
                .From(typeof(Customer))
                .Where("CustomerStatusId").IsEqualTo(CustomerStatus.Active)
                .ToSqlQuery();

            Assert.Equal(1, sqlQuery.Arguments.Count);
            Assert.Equal(CustomerStatus.Active, sqlQuery.Arguments[0]);

            Assert.Equal("SELECT Id,MAX(CreditLimit) AS CreditLimit FROM Sales.Customers WHERE (CustomerStatusId = ?)", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectMaxWithSqlCharacters()
        {
            var sqlBuilder = new SelectSqlBuilder(MsSqlCharacters.Instance, null);

            var sqlQuery = sqlBuilder
                .Max("CreditLimit")
                .From(typeof(Customer))
                .Where("CustomerStatusId").IsEqualTo(CustomerStatus.Active)
                .ToSqlQuery();

            Assert.Equal(1, sqlQuery.Arguments.Count);
            Assert.Equal(CustomerStatus.Active, sqlQuery.Arguments[0]);

            Assert.Equal("SELECT MAX([CreditLimit]) AS CreditLimit FROM [Sales].[Customers] WHERE ([CustomerStatusId] = @p0)", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectMin()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, null);

            var sqlQuery = sqlBuilder
                .Min("CreditLimit")
                .From(typeof(Customer))
                .Where("CustomerStatusId").IsEqualTo(CustomerStatus.Active)
                .ToSqlQuery();

            Assert.Equal(1, sqlQuery.Arguments.Count);
            Assert.Equal(CustomerStatus.Active, sqlQuery.Arguments[0]);

            Assert.Equal("SELECT MIN(CreditLimit) AS CreditLimit FROM Sales.Customers WHERE (CustomerStatusId = ?)", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectMinWithAlias()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, null);

            var sqlQuery = sqlBuilder
                .Min("CreditLimit", columnAlias: "MinCreditLimit")
                .From(typeof(Customer))
                .Where("CustomerStatusId").IsEqualTo(CustomerStatus.Active)
                .ToSqlQuery();

            Assert.Equal(1, sqlQuery.Arguments.Count);
            Assert.Equal(CustomerStatus.Active, sqlQuery.Arguments[0]);

            Assert.Equal("SELECT MIN(CreditLimit) AS MinCreditLimit FROM Sales.Customers WHERE (CustomerStatusId = ?)", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectMinWithOtherColumn()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "Id");

            var sqlQuery = sqlBuilder
                .Min("CreditLimit")
                .From(typeof(Customer))
                .Where("CustomerStatusId").IsEqualTo(CustomerStatus.Active)
                .ToSqlQuery();

            Assert.Equal(1, sqlQuery.Arguments.Count);
            Assert.Equal(CustomerStatus.Active, sqlQuery.Arguments[0]);

            Assert.Equal("SELECT Id,MIN(CreditLimit) AS CreditLimit FROM Sales.Customers WHERE (CustomerStatusId = ?)", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectMinWithSqlCharacters()
        {
            var sqlBuilder = new SelectSqlBuilder(MsSqlCharacters.Instance, null);

            var sqlQuery = sqlBuilder
                .Min("CreditLimit")
                .From(typeof(Customer))
                .Where("CustomerStatusId").IsEqualTo(CustomerStatus.Active)
                .ToSqlQuery();

            Assert.Equal(1, sqlQuery.Arguments.Count);
            Assert.Equal(CustomerStatus.Active, sqlQuery.Arguments[0]);

            Assert.Equal("SELECT MIN([CreditLimit]) AS CreditLimit FROM [Sales].[Customers] WHERE ([CustomerStatusId] = @p0)", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectSum()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, null);

            var sqlQuery = sqlBuilder
                .Sum("CreditLimit")
                .From(typeof(Customer))
                .Where("CustomerStatusId").IsEqualTo(CustomerStatus.Active)
                .ToSqlQuery();

            Assert.Equal(1, sqlQuery.Arguments.Count);
            Assert.Equal(CustomerStatus.Active, sqlQuery.Arguments[0]);

            Assert.Equal("SELECT SUM(CreditLimit) AS CreditLimit FROM Sales.Customers WHERE (CustomerStatusId = ?)", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectSumWithAlias()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, null);

            var sqlQuery = sqlBuilder
                .Sum("CreditLimit", columnAlias: "SumCreditLimit")
                .From(typeof(Customer))
                .Where("CustomerStatusId").IsEqualTo(CustomerStatus.Active)
                .ToSqlQuery();

            Assert.Equal(1, sqlQuery.Arguments.Count);
            Assert.Equal(CustomerStatus.Active, sqlQuery.Arguments[0]);

            Assert.Equal("SELECT SUM(CreditLimit) AS SumCreditLimit FROM Sales.Customers WHERE (CustomerStatusId = ?)", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectSumWithOtherColumn()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "Id");

            var sqlQuery = sqlBuilder
                .Sum("CreditLimit")
                .From(typeof(Customer))
                .Where("CustomerStatusId").IsEqualTo(CustomerStatus.Active)
                .ToSqlQuery();

            Assert.Equal(1, sqlQuery.Arguments.Count);
            Assert.Equal(CustomerStatus.Active, sqlQuery.Arguments[0]);

            Assert.Equal("SELECT Id,SUM(CreditLimit) AS CreditLimit FROM Sales.Customers WHERE (CustomerStatusId = ?)", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectSumWithSqlCharacters()
        {
            var sqlBuilder = new SelectSqlBuilder(MsSqlCharacters.Instance, null);

            var sqlQuery = sqlBuilder
                .Sum("CreditLimit")
                .From(typeof(Customer))
                .Where("CustomerStatusId").IsEqualTo(CustomerStatus.Active)
                .ToSqlQuery();

            Assert.Equal(1, sqlQuery.Arguments.Count);
            Assert.Equal(CustomerStatus.Active, sqlQuery.Arguments[0]);

            Assert.Equal("SELECT SUM([CreditLimit]) AS CreditLimit FROM [Sales].[Customers] WHERE ([CustomerStatusId] = @p0)", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectWhereAndWhereInArgs()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "Column1");

            var sqlQuery = sqlBuilder
                .From("Table")
                .Where("Column2").IsEqualTo("FOO")
                .AndWhere("Column1")
                .In(1, 2, 3)
                .ToSqlQuery();

            Assert.Equal(4, sqlQuery.Arguments.Count);
            Assert.Equal("FOO", sqlQuery.Arguments[0]);
            Assert.Equal(1, sqlQuery.Arguments[1]);
            Assert.Equal(2, sqlQuery.Arguments[2]);
            Assert.Equal(3, sqlQuery.Arguments[3]);

            Assert.Equal("SELECT Column1 FROM Table WHERE (Column2 = ?) AND (Column1 IN (?,?,?))", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectWhereAndWhereInArgsWithSqlCharacters()
        {
            var sqlBuilder = new SelectSqlBuilder(MsSqlCharacters.Instance, "Column1");

            var sqlQuery = sqlBuilder
                .From("Table")
                .Where("Column2").IsEqualTo("FOO")
                .AndWhere("Column1").In(1, 2, 3)
                .ToSqlQuery();

            Assert.Equal(4, sqlQuery.Arguments.Count);
            Assert.Equal("FOO", sqlQuery.Arguments[0]);
            Assert.Equal(1, sqlQuery.Arguments[1]);
            Assert.Equal(2, sqlQuery.Arguments[2]);
            Assert.Equal(3, sqlQuery.Arguments[3]);

            Assert.Equal("SELECT [Column1] FROM [Table] WHERE ([Column2] = @p0) AND ([Column1] IN (@p1,@p2,@p3))", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectWhereAndWhereInSqlQuery()
        {
            var subQuery = new SqlQuery("SELECT Id FROM Table WHERE Column = ?", 1024);

            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "Column1");

            var sqlQuery = sqlBuilder
                .From("Table")
                .Where("Column2 = ?", "FOO")
                .AndWhere("Column1").In(subQuery)
                .ToSqlQuery();

            Assert.Equal(2, sqlQuery.Arguments.Count);
            Assert.Equal("FOO", sqlQuery.Arguments[0]);
            Assert.Equal(1024, sqlQuery.Arguments[1]);

            Assert.Equal("SELECT Column1 FROM Table WHERE (Column2 = ?) AND (Column1 IN (SELECT Id FROM Table WHERE Column = ?))", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectWhereAndWhereInSqlQueryWithSqlCharacters()
        {
            var subQuery = new SqlQuery("SELECT Id FROM Table WHERE Column = @p0", 1024);

            var sqlBuilder = new SelectSqlBuilder(MsSqlCharacters.Instance, "Column1");

            var sqlQuery = sqlBuilder
                .From("Table")
                .Where("Column2 = @p0", "FOO")
                .AndWhere("Column1").In(subQuery)
                .ToSqlQuery();

            Assert.Equal(2, sqlQuery.Arguments.Count);
            Assert.Equal("FOO", sqlQuery.Arguments[0]);
            Assert.Equal(1024, sqlQuery.Arguments[1]);

            Assert.Equal("SELECT [Column1] FROM [Table] WHERE (Column2 = @p0) AND ([Column1] IN (SELECT Id FROM Table WHERE Column = @p1))", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectWhereAndWhereNotInArgsWithSqlCharacters()
        {
            var sqlBuilder = new SelectSqlBuilder(MsSqlCharacters.Instance, "Column1");

            var sqlQuery = sqlBuilder
                .From("Table")
                .Where("Column2").IsEqualTo("FOO")
                .AndWhere("Column1").NotIn(1, 2, 3)
                .ToSqlQuery();

            Assert.Equal(4, sqlQuery.Arguments.Count);
            Assert.Equal("FOO", sqlQuery.Arguments[0]);
            Assert.Equal(1, sqlQuery.Arguments[1]);
            Assert.Equal(2, sqlQuery.Arguments[2]);
            Assert.Equal(3, sqlQuery.Arguments[3]);

            Assert.Equal("SELECT [Column1] FROM [Table] WHERE ([Column2] = @p0) AND ([Column1] NOT IN (@p1,@p2,@p3))", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectWhereAndWhereNotInSqlQuery()
        {
            var subQuery = new SqlQuery("SELECT Id FROM Table WHERE Column = ?", 1024);

            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "Column1");

            var sqlQuery = sqlBuilder
                .From("Table")
                .Where("Column2 = ?", "FOO")
                .AndWhere("Column1").NotIn(subQuery)
                .ToSqlQuery();

            Assert.Equal(2, sqlQuery.Arguments.Count);
            Assert.Equal("FOO", sqlQuery.Arguments[0]);
            Assert.Equal(1024, sqlQuery.Arguments[1]);

            Assert.Equal("SELECT Column1 FROM Table WHERE (Column2 = ?) AND (Column1 NOT IN (SELECT Id FROM Table WHERE Column = ?))", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectWhereBetweenUsing()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "Column1");

            var sqlQuery = sqlBuilder
                   .From("Table")
                   .Where("Column1")
                   .Between(1, 10)
                   .ToSqlQuery();

            Assert.Equal(2, sqlQuery.Arguments.Count);
            Assert.Equal(1, sqlQuery.Arguments[0]);
            Assert.Equal(10, sqlQuery.Arguments[1]);

            Assert.Equal("SELECT Column1 FROM Table WHERE (Column1 BETWEEN ? AND ?)", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectWhereBetweenUsingWithSqlCharacters()
        {
            var sqlBuilder = new SelectSqlBuilder(MsSqlCharacters.Instance, "Column1");

            var sqlQuery = sqlBuilder
                   .From("Table")
                   .Where("Column1")
                   .Between(1, 10)
                   .ToSqlQuery();

            Assert.Equal(2, sqlQuery.Arguments.Count);
            Assert.Equal(1, sqlQuery.Arguments[0]);
            Assert.Equal(10, sqlQuery.Arguments[1]);

            Assert.Equal("SELECT [Column1] FROM [Table] WHERE ([Column1] BETWEEN @p0 AND @p1)", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectWhereColumnIsEqualTo()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "Column1");

            var sqlQuery = sqlBuilder
                   .From("Table")
                   .Where("Column1")
                   .IsEqualTo("FOO")
                   .ToSqlQuery();

            Assert.Equal(1, sqlQuery.Arguments.Count);
            Assert.Equal("FOO", sqlQuery.Arguments[0]);

            Assert.Equal("SELECT Column1 FROM Table WHERE (Column1 = ?)", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectWhereColumnIsEqualToWithSqlCharacters()
        {
            var sqlBuilder = new SelectSqlBuilder(MsSqlCharacters.Instance, "Column1");

            var sqlQuery = sqlBuilder
                   .From("Table")
                   .Where("Column1")
                   .IsEqualTo("FOO")
                   .ToSqlQuery();

            Assert.Equal(1, sqlQuery.Arguments.Count);
            Assert.Equal("FOO", sqlQuery.Arguments[0]);

            Assert.Equal("SELECT [Column1] FROM [Table] WHERE ([Column1] = @p0)", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectWhereColumnIsGreaterThan()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "Column1");

            var sqlQuery = sqlBuilder
                   .From("Table")
                   .Where("Column1")
                   .IsGreaterThan("FOO")
                   .ToSqlQuery();

            Assert.Equal(1, sqlQuery.Arguments.Count);
            Assert.Equal("FOO", sqlQuery.Arguments[0]);

            Assert.Equal("SELECT Column1 FROM Table WHERE (Column1 > ?)", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectWhereColumnIsGreaterThanOrEqualTo()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "Column1");

            var sqlQuery = sqlBuilder
                   .From("Table")
                   .Where("Column1")
                   .IsGreaterThanOrEqualTo("FOO")
                   .ToSqlQuery();

            Assert.Equal(1, sqlQuery.Arguments.Count);
            Assert.Equal("FOO", sqlQuery.Arguments[0]);

            Assert.Equal("SELECT Column1 FROM Table WHERE (Column1 >= ?)", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectWhereColumnIsGreaterThanOrEqualToWithSqlCharacters()
        {
            var sqlBuilder = new SelectSqlBuilder(MsSqlCharacters.Instance, "Column1");

            var sqlQuery = sqlBuilder
                   .From("Table")
                   .Where("Column1")
                   .IsGreaterThanOrEqualTo("FOO")
                   .ToSqlQuery();

            Assert.Equal(1, sqlQuery.Arguments.Count);
            Assert.Equal("FOO", sqlQuery.Arguments[0]);

            Assert.Equal("SELECT [Column1] FROM [Table] WHERE ([Column1] >= @p0)", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectWhereColumnIsGreaterThanWithSqlCharacters()
        {
            var sqlBuilder = new SelectSqlBuilder(MsSqlCharacters.Instance, "Column1");

            var sqlQuery = sqlBuilder
                   .From("Table")
                   .Where("Column1")
                   .IsGreaterThan("FOO")
                   .ToSqlQuery();

            Assert.Equal(1, sqlQuery.Arguments.Count);
            Assert.Equal("FOO", sqlQuery.Arguments[0]);

            Assert.Equal("SELECT [Column1] FROM [Table] WHERE ([Column1] > @p0)", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectWhereColumnIsLessThan()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "Column1");

            var sqlQuery = sqlBuilder
                   .From("Table")
                   .Where("Column1")
                   .IsLessThan("FOO")
                   .ToSqlQuery();

            Assert.Equal(1, sqlQuery.Arguments.Count);
            Assert.Equal("FOO", sqlQuery.Arguments[0]);

            Assert.Equal("SELECT Column1 FROM Table WHERE (Column1 < ?)", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectWhereColumnIsLessThanOrEqualTo()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "Column1");

            var sqlQuery = sqlBuilder
                   .From("Table")
                   .Where("Column1")
                   .IsLessThanOrEqualTo("FOO")
                   .ToSqlQuery();

            Assert.Equal(1, sqlQuery.Arguments.Count);
            Assert.Equal("FOO", sqlQuery.Arguments[0]);

            Assert.Equal("SELECT Column1 FROM Table WHERE (Column1 <= ?)", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectWhereColumnIsLessThanOrEqualToWithSqlCharacters()
        {
            var sqlBuilder = new SelectSqlBuilder(MsSqlCharacters.Instance, "Column1");

            var sqlQuery = sqlBuilder
                   .From("Table")
                   .Where("Column1")
                   .IsLessThanOrEqualTo("FOO")
                   .ToSqlQuery();

            Assert.Equal(1, sqlQuery.Arguments.Count);
            Assert.Equal("FOO", sqlQuery.Arguments[0]);

            Assert.Equal("SELECT [Column1] FROM [Table] WHERE ([Column1] <= @p0)", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectWhereColumnIsLessThanWithSqlCharacters()
        {
            var sqlBuilder = new SelectSqlBuilder(MsSqlCharacters.Instance, "Column1");

            var sqlQuery = sqlBuilder
                   .From("Table")
                   .Where("Column1")
                   .IsLessThan("FOO")
                   .ToSqlQuery();

            Assert.Equal(1, sqlQuery.Arguments.Count);
            Assert.Equal("FOO", sqlQuery.Arguments[0]);

            Assert.Equal("SELECT [Column1] FROM [Table] WHERE ([Column1] < @p0)", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectWhereColumnIsLike()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "Column1");

            var sqlQuery = sqlBuilder
                   .From("Table")
                   .Where("Column1")
                   .IsLike("FOO")
                   .ToSqlQuery();

            Assert.Equal(1, sqlQuery.Arguments.Count);
            Assert.Equal("FOO", sqlQuery.Arguments[0]);

            Assert.Equal("SELECT Column1 FROM Table WHERE (Column1 LIKE ?)", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectWhereColumnIsLikeWithSqlCharacters()
        {
            var sqlBuilder = new SelectSqlBuilder(MsSqlCharacters.Instance, "Column1");

            var sqlQuery = sqlBuilder
                   .From("Table")
                   .Where("Column1")
                   .IsLike("FOO")
                   .ToSqlQuery();

            Assert.Equal(1, sqlQuery.Arguments.Count);
            Assert.Equal("FOO", sqlQuery.Arguments[0]);

            Assert.Equal("SELECT [Column1] FROM [Table] WHERE ([Column1] LIKE @p0)", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectWhereColumnIsNotEqualTo()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "Column1");

            var sqlQuery = sqlBuilder
                   .From("Table")
                   .Where("Column1")
                   .IsNotEqualTo("FOO")
                   .ToSqlQuery();

            Assert.Equal(1, sqlQuery.Arguments.Count);
            Assert.Equal("FOO", sqlQuery.Arguments[0]);

            Assert.Equal("SELECT Column1 FROM Table WHERE (Column1 <> ?)", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectWhereColumnIsNotEqualToWithSqlCharacters()
        {
            var sqlBuilder = new SelectSqlBuilder(MsSqlCharacters.Instance, "Column1");

            var sqlQuery = sqlBuilder
                   .From("Table")
                   .Where("Column1")
                   .IsNotEqualTo("FOO")
                   .ToSqlQuery();

            Assert.Equal(1, sqlQuery.Arguments.Count);
            Assert.Equal("FOO", sqlQuery.Arguments[0]);

            Assert.Equal("SELECT [Column1] FROM [Table] WHERE ([Column1] <> @p0)", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectWhereColumnIsNotNull()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "Column1");

            var sqlQuery = sqlBuilder
                   .From("Table")
                   .Where("Column1")
                   .IsNotNull()
                   .ToSqlQuery();

            Assert.Equal(0, sqlQuery.Arguments.Count);

            Assert.Equal("SELECT Column1 FROM Table WHERE (Column1 IS NOT NULL)", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectWhereColumnIsNotNullWithSqlCharacters()
        {
            var sqlBuilder = new SelectSqlBuilder(MsSqlCharacters.Instance, "Column1");

            var sqlQuery = sqlBuilder
                   .From("Table")
                   .Where("Column1")
                   .IsNotNull()
                   .ToSqlQuery();

            Assert.Equal(0, sqlQuery.Arguments.Count);

            Assert.Equal("SELECT [Column1] FROM [Table] WHERE ([Column1] IS NOT NULL)", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectWhereColumnIsNull()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "Column1");

            var sqlQuery = sqlBuilder
                   .From("Table")
                   .Where("Column1")
                   .IsNull()
                   .ToSqlQuery();

            Assert.Equal(0, sqlQuery.Arguments.Count);

            Assert.Equal("SELECT Column1 FROM Table WHERE (Column1 IS NULL)", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectWhereColumnIsNullWithSqlCharacters()
        {
            var sqlBuilder = new SelectSqlBuilder(MsSqlCharacters.Instance, "Column1");

            var sqlQuery = sqlBuilder
                   .From("Table")
                   .Where("Column1")
                   .IsNull()
                   .ToSqlQuery();

            Assert.Equal(0, sqlQuery.Arguments.Count);

            Assert.Equal("SELECT [Column1] FROM [Table] WHERE ([Column1] IS NULL)", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectWhereGroupByHavingOrderBy()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "CustomerId");

            var sqlQuery = sqlBuilder
                .Sum("Total")
                .From("Invoices")
                .Where("OrderDate").IsGreaterThan(new DateTime(2000, 1, 1))
                .GroupBy("Total")
                .Having("SUM(Total) > ?", 10000M)
                .OrderByDescending("OrderDate")
                .ToSqlQuery();

            Assert.Equal(2, sqlQuery.Arguments.Count);
            Assert.Equal(new DateTime(2000, 1, 1), sqlQuery.Arguments[0]);
            Assert.Equal(10000M, sqlQuery.Arguments[1]);

            Assert.Equal("SELECT CustomerId,SUM(Total) AS Total FROM Invoices WHERE (OrderDate > ?) GROUP BY Total HAVING SUM(Total) > ? ORDER BY OrderDate DESC", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectWhereGroupByOrderBy()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "CustomerId");

            var sqlQuery = sqlBuilder
                .Sum("Total")
                .From("Invoices")
                .Where("OrderDate").IsGreaterThan(new DateTime(2000, 1, 1))
                .GroupBy("Total")
                .OrderByDescending("OrderDate")
                .ToSqlQuery();

            Assert.Equal(1, sqlQuery.Arguments.Count);
            Assert.Equal(new DateTime(2000, 1, 1), sqlQuery.Arguments[0]);

            Assert.Equal("SELECT CustomerId,SUM(Total) AS Total FROM Invoices WHERE (OrderDate > ?) GROUP BY Total ORDER BY OrderDate DESC", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectWhereGroupByOrderByWithSqlCharacters()
        {
            var sqlBuilder = new SelectSqlBuilder(MsSqlCharacters.Instance, "CustomerId");

            var sqlQuery = sqlBuilder
                .Sum("Total")
                .From("Invoices")
                .Where("OrderDate").IsGreaterThan(new DateTime(2000, 1, 1))
                .GroupBy("Total")
                .OrderByDescending("OrderDate")
                .ToSqlQuery();

            Assert.Equal(1, sqlQuery.Arguments.Count);
            Assert.Equal(new DateTime(2000, 1, 1), sqlQuery.Arguments[0]);

            Assert.Equal("SELECT [CustomerId],SUM([Total]) AS Total FROM [Invoices] WHERE ([OrderDate] > @p0) GROUP BY [Total] ORDER BY [OrderDate] DESC", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectWhereInArgs()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "Column1");

            var sqlQuery = sqlBuilder
                .From("Table")
                .Where("Column1")
                .In(1, 2, 3)
                .ToSqlQuery();

            Assert.Equal(3, sqlQuery.Arguments.Count);
            Assert.Equal(1, sqlQuery.Arguments[0]);
            Assert.Equal(2, sqlQuery.Arguments[1]);
            Assert.Equal(3, sqlQuery.Arguments[2]);

            Assert.Equal("SELECT Column1 FROM Table WHERE (Column1 IN (?,?,?))", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectWhereInArgsWithSqlCharacters()
        {
            var sqlBuilder = new SelectSqlBuilder(MsSqlCharacters.Instance, "Column1");

            var sqlQuery = sqlBuilder
                .From("Table")
                .Where("Column1")
                .In(1, 2, 3)
                .ToSqlQuery();

            Assert.Equal(3, sqlQuery.Arguments.Count);
            Assert.Equal(1, sqlQuery.Arguments[0]);
            Assert.Equal(2, sqlQuery.Arguments[1]);
            Assert.Equal(3, sqlQuery.Arguments[2]);

            Assert.Equal("SELECT [Column1] FROM [Table] WHERE ([Column1] IN (@p0,@p1,@p2))", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectWhereInSqlQuery()
        {
            var subQuery = new SqlQuery("SELECT Id FROM Table WHERE Column = @p0", 1024);

            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "Column1");

            var sqlQuery = sqlBuilder
                .From("Table")
                .Where("Column1")
                .In(subQuery)
                .ToSqlQuery();

            Assert.Equal(1, sqlQuery.Arguments.Count);
            Assert.Equal(1024, sqlQuery.Arguments[0]);

            Assert.Equal("SELECT Column1 FROM Table WHERE (Column1 IN (SELECT Id FROM Table WHERE Column = @p0))", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectWhereNotInArgs()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "Column1");

            var sqlQuery = sqlBuilder
                .From("Table")
                .Where("Column1")
                .NotIn(1, 2, 3)
                .ToSqlQuery();

            Assert.Equal(3, sqlQuery.Arguments.Count);
            Assert.Equal(1, sqlQuery.Arguments[0]);
            Assert.Equal(2, sqlQuery.Arguments[1]);
            Assert.Equal(3, sqlQuery.Arguments[2]);

            Assert.Equal("SELECT Column1 FROM Table WHERE (Column1 NOT IN (?,?,?))", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectWhereNotInArgsWithSqlCharacters()
        {
            var sqlBuilder = new SelectSqlBuilder(MsSqlCharacters.Instance, "Column1");

            var sqlQuery = sqlBuilder
                .From("Table")
                .Where("Column1").NotIn(1, 2, 3)
                .ToSqlQuery();

            Assert.Equal(3, sqlQuery.Arguments.Count);
            Assert.Equal(1, sqlQuery.Arguments[0]);
            Assert.Equal(2, sqlQuery.Arguments[1]);
            Assert.Equal(3, sqlQuery.Arguments[2]);

            Assert.Equal("SELECT [Column1] FROM [Table] WHERE ([Column1] NOT IN (@p0,@p1,@p2))", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectWhereNotInSqlQuery()
        {
            var subQuery = new SqlQuery("SELECT Id FROM Table WHERE Column = @p0", 1024);

            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "Column1");

            var sqlQuery = sqlBuilder
                .From("Table")
                .Where("Column1").NotIn(subQuery)
                .ToSqlQuery();

            Assert.Equal(1, sqlQuery.Arguments.Count);
            Assert.Equal(1024, sqlQuery.Arguments[0]);

            Assert.Equal("SELECT Column1 FROM Table WHERE (Column1 NOT IN (SELECT Id FROM Table WHERE Column = @p0))", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectWhereOrWhereInArgs()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "Column1");

            var sqlQuery = sqlBuilder
                .From("Table")
                .Where("Column2").IsEqualTo("FOO")
                .OrWhere("Column1")
                .In(1, 2, 3)
                .ToSqlQuery();

            Assert.Equal(4, sqlQuery.Arguments.Count);
            Assert.Equal("FOO", sqlQuery.Arguments[0]);
            Assert.Equal(1, sqlQuery.Arguments[1]);
            Assert.Equal(2, sqlQuery.Arguments[2]);
            Assert.Equal(3, sqlQuery.Arguments[3]);

            Assert.Equal("SELECT Column1 FROM Table WHERE (Column2 = ?) OR (Column1 IN (?,?,?))", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectWhereOrWhereInArgsWithSqlCharacters()
        {
            var sqlBuilder = new SelectSqlBuilder(MsSqlCharacters.Instance, "Column1");

            var sqlQuery = sqlBuilder
                .From("Table")
                .Where("Column2").IsEqualTo("FOO")
                .OrWhere("Column1").In(1, 2, 3)
                .ToSqlQuery();

            Assert.Equal(4, sqlQuery.Arguments.Count);
            Assert.Equal("FOO", sqlQuery.Arguments[0]);
            Assert.Equal(1, sqlQuery.Arguments[1]);
            Assert.Equal(2, sqlQuery.Arguments[2]);
            Assert.Equal(3, sqlQuery.Arguments[3]);

            Assert.Equal("SELECT [Column1] FROM [Table] WHERE ([Column2] = @p0) OR ([Column1] IN (@p1,@p2,@p3))", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectWhereOrWhereInSqlQuery()
        {
            var subQuery = new SqlQuery("SELECT Id FROM Table WHERE Column = ?", 1024);

            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "Column1");

            var sqlQuery = sqlBuilder
                .From("Table")
                .Where("Column2 = ?", "FOO")
                .OrWhere("Column1")
                .In(subQuery)
                .ToSqlQuery();

            Assert.Equal(2, sqlQuery.Arguments.Count);
            Assert.Equal("FOO", sqlQuery.Arguments[0]);
            Assert.Equal(1024, sqlQuery.Arguments[1]);

            Assert.Equal("SELECT Column1 FROM Table WHERE (Column2 = ?) OR (Column1 IN (SELECT Id FROM Table WHERE Column = ?))", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectWhereOrWhereInSqlQueryWithSqlCharacters()
        {
            var subQuery = new SqlQuery("SELECT Id FROM Table WHERE Column = @p0", 1024);

            var sqlBuilder = new SelectSqlBuilder(MsSqlCharacters.Instance, "Column1");

            var sqlQuery = sqlBuilder
                .From("Table")
                .Where("Column2 = @p0", "FOO")
                .OrWhere("Column1")
                .In(subQuery)
                .ToSqlQuery();

            Assert.Equal(2, sqlQuery.Arguments.Count);
            Assert.Equal("FOO", sqlQuery.Arguments[0]);
            Assert.Equal(1024, sqlQuery.Arguments[1]);

            Assert.Equal("SELECT [Column1] FROM [Table] WHERE (Column2 = @p0) OR ([Column1] IN (SELECT Id FROM Table WHERE Column = @p1))", sqlQuery.CommandText);
        }

        /// <summary>
        /// Issue #224 - SqlBuilder SingleColumn predicates not appending AND or OR
        /// </summary>
        [Fact]
        public void SingleColumnPredicatesShouldAppendOperand()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "Column1");

            var sqlQuery = sqlBuilder
                .From("Table")
                .Where("Column2").In("Opt1", "Opt2")
                .AndWhere("Column3").IsEqualTo(1)
                .AndWhere("Column4").IsGreaterThan(2)
                .AndWhere("Column5").IsGreaterThanOrEqualTo(3)
                .AndWhere("Column6").IsLessThan(4)
                .AndWhere("Column7").IsLessThanOrEqualTo(5)
                .AndWhere("Column8").IsLike("%J")
                .AndWhere("Column9").IsNotEqualTo(6)
                .AndWhere("Column10").IsNotNull()
                .AndWhere("Column11").IsNull()
                .ToSqlQuery();

            Assert.Equal(@"SELECT Column1 FROM Table WHERE (Column2 IN (?,?)) AND (Column3 = ?) AND (Column4 > ?) AND (Column5 >= ?) AND (Column6 < ?) AND (Column7 <= ?) AND (Column8 LIKE ?) AND (Column9 <> ?) AND (Column10 IS NOT NULL) AND (Column11 IS NULL)", sqlQuery.CommandText);
        }

        [Fact]
        public void WhenCreatedWithoutSpecifyingAnyColumnsTheCommandTextIsSelectWildcard()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty);

            Assert.Equal("SELECT *", sqlBuilder.ToSqlQuery().CommandText);
        }
    }
}