using Autofac;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using ProductReservationSystem.Public;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ProductReservationSystem
{
    [Trait("Category", "Stocks")]
    public class StockRequirement
    {
        [Theory(DisplayName = "When doing inventory count user can set the quantity for any product")]
        [InlineData("a product id", 0)]
        [InlineData("a another id", 10)]
        public void When_doing_inventory_count_user_can_set_the_quantity_for_any_product(string id, uint quantity)
        {
            var config = Program.SampleConfig(Storage.UseInMemory);
            InventoryCount stocks = new Program(config).Stocks;

            stocks[id] = quantity;

            stocks[id].Should().Be(quantity);
        }

        [Fact(DisplayName = "Quantity should be linked to its own product")]
        public void Quantity_should_be_linked_to_its_own_product()
        {
            var config = Program.SampleConfig(Storage.UseInMemory);
            InventoryCount stocks = new Program(config).Stocks;

            var stockForFirstProduct = new Stock("1",10);
            var anotherProductStock = new Stock("2", 5);

            stocks[stockForFirstProduct.ProductId] = stockForFirstProduct.Quantity;
            stocks[anotherProductStock.ProductId] = anotherProductStock.Quantity;

            stocks[stockForFirstProduct.ProductId].Should().Be(stockForFirstProduct.Quantity);
            stocks[anotherProductStock.ProductId].Should().Be(anotherProductStock.Quantity);
        }

        [Fact(DisplayName = "Filling Quantity for a product already in db should update the quantity")]
        public void Filling_Quantity_for_a_product_already_in_db_should_update_the_quantity()
        {
            var config = Program.SampleConfig(Storage.UseInMemory);
            InventoryCount stocks = new Program(config).Stocks;
            string id = "5";

            stocks[id] = 5;
            stocks[id] = 10;

            stocks[id].Should().Be(10);
        }

        [Fact(DisplayName = "All product for which a quantity has been set are in the stock listing")]
        public void All_product_for_which_a_quantity_has_been_set_are_in_the_stock_listing()
        {
            var config = Program.SampleConfig(Storage.UseInMemory);
            InventoryCount stocks = new Program(config).Stocks;
            stocks["id1"] = 1;
            stocks["id2"] = 2;

            stocks.PaginatedStockList(0,5).Should()
                .HaveCount(2)
                .And.Contain(s => s.ProductId == "id1" && s.Quantity == 1)
                .And.Contain(s => s.ProductId == "id2" && s.Quantity == 2);
        }

        [Theory(DisplayName = "When listing the stock it is paginated with a page length of limit")]
        [InlineData(10)]
        [InlineData(20)]
        [InlineData(30)]
        public void When_listing_the_stock_it_is_paginated_with_a_page_length_of_limit(int pagelength)
        {
            var config = Program.SampleConfig(TestHelper.MockVeryLargeDb);
            InventoryCount stocks = new Program(config).Stocks;

            stocks.PaginatedStockList(0, pagelength).Should()
                .HaveCount(pagelength);
        }

        [Theory(DisplayName = "When listing the stock the paginated elem start at the cursor th element")]
        [InlineData(0)]
        [InlineData(5)]
        [InlineData(100)]
        public void When_listing_the_stock_the_paginated_elem_start_at_the_cursor_th_element(int start)
        {
            var config = Program.SampleConfig(TestHelper.MockVeryLargeDb);
            InventoryCount stocks = new Program(config).Stocks;
            
            int lastIndex = start+5-1; // index start+5 not included otherwise limit+1 element would have been included

            stocks.PaginatedStockList(start, 5).Should()
                .HaveCount(5)
                .And.Contain(s => s.ProductId == $"id{start}" && s.Quantity == (uint)start)
                .And.Contain(s => s.ProductId == $"id{lastIndex}" && s.Quantity ==  (uint)lastIndex)
                .And.BeInAscendingOrder(s => s.ProductId);
        }
    }
}
