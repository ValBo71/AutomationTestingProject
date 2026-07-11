namespace AutomationExercise.Tests.Selectors
{
    public static class ProductsPageSelectors
    {
        public const string SearchInput = "#search_product";
        public const string SearchButton = "#submit_search";
        public const string ProductsHeader = ".features_items h2.title";
        public const string ProductListItems = ".single-products";
        public const string ModalViewCartButton = "p.text-center a:has-text('View Cart')";
        public const string ModalContinueShoppingButton = "button.close-modal";
        public const string ProductsTitle = "h2.title.text-center";
        public const string CategoryWomenLink = "a[href='#Women']";
        public const string CategoryWomenDressLink = "a[href='/category_products/1']";
        public const string CategoryMenLink = "a[href='#Men']";
        public const string CategoryMenTshirtsLink = "a[href='/category_products/3']";

        // Product Details
        public const string DetailProductName = "div.product-information h2";
        public const string DetailProductCategory = "div.product-information p:has-text('Category:')";
        public const string DetailProductPrice = "div.product-information span span";
        public const string DetailProductAvailability = "div.product-information p:has-text('Availability:')";
        public const string DetailProductCondition = "div.product-information p:has-text('Condition:')";
        public const string DetailProductBrand = "div.product-information p:has-text('Brand:')";
        public const string DetailProductQuantityInput = "#quantity";
        public const string DetailProductAddToCartButton = "button.cart";
        
        // Review Form
        public const string ReviewNameInput = "#name";
        public const string ReviewEmailInput = "#email";
        public const string ReviewTextInput = "#review";
        public const string ReviewSubmitButton = "#button-review";
        public const string ReviewSuccessAlert = "div.alert-success:has-text('Thank you for your review.')";
    }
}